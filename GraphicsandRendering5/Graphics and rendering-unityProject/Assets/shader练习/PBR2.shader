Shader "MyShader/PBR"
{
    Properties
    {
        _MaskTex("Mask Texture",2D)="white"{}  
        _Color("Color",Color)=(1,1,1,1)
        _MainTex ("Albedo(RGB)", 2D) = "white" {}
		_ARMTex ("ARMTex", 2D) = "white" {}
        [NoScaleOffset]_BumpTex("Normal Map(RGB)",2D)="bump"{}
        _BumpSacle("Bump Sacle",Range(-1,1))=1
        _Roughness("Roughness",Range(0,3))=0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardAdd"}
        LOD 100

        CGINCLUDE
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
		#pragma multi_compile_fwdadd

        float3 FresnelSchlick(float cosTheta,float3 F0)
        {
            return F0+(1.0-F0)*pow(1.0-cosTheta,5.0);
        }

        float DistributionGGX(float3 N,float3 H,float roughness)
        {
            float a2=roughness*roughness;
            a2=a2*a2;
            float NdotH=saturate(dot(N,H));
            float NdotH2=NdotH*NdotH;

            float denom=(NdotH2*(a2-1.0)+1.0);
            denom=UNITY_PI*denom*denom;
            return a2/denom;
        }

        float3 GeometrySchlickGGX(float NdotV,float roughness)
        {

            float r=roughness+1.0;
            float k=r*r/8.0;

            float denom=NdotV*(1.0-k)+k;
            return NdotV/denom;
        }

        float GeometrySmith(float3 N,float3 V,float3 L,float roughness)
        {
            float NdotV=saturate(dot(N,V));
            float NdotL=saturate(dot(N,L));
            float ggx1=GeometrySchlickGGX(NdotV,roughness);
            float ggx2=GeometrySchlickGGX(NdotL,roughness);

            return ggx1*ggx2;
        }

        ENDCG


        Pass
        {
			Tags{"LightMode" = "ForwardAdd"}
		
			//Blend One One
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                float4 tangent:TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 lightDir:TEXCOORD1;
                float3 viewDir:TEXCOORD2;
                float3 TtoW0:TEXCOORD3;
                float3 TtoW1:TEXCOORD4;
                float3 TtoW2:TEXCOORD5;
            };
 
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpTex;
            float _BumpSacle;
            float _Roughness;
            sampler2D _BRDF;
            float _AO;
            fixed4 _Color;
			sampler2D _ARMTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                float3 tangent=UnityObjectToWorldDir(v.tangent);
                float3 normal=UnityObjectToWorldNormal(v.normal);
                float3 binormal=cross(normal,tangent)*v.tangent.w;
                o.TtoW0=float3(tangent.x,binormal.x,normal.x);
                o.TtoW1=float3(tangent.y,binormal.y,normal.y);
                o.TtoW2=float3(tangent.z,binormal.z,normal.z);

                float3 worldPos=mul(unity_ObjectToWorld,v.vertex);
                o.lightDir=UnityWorldSpaceLightDir(worldPos);
                o.viewDir=UnityWorldSpaceViewDir(worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 ARMTex = tex2D(_ARMTex,i.uv);

                float4 packNormal=tex2D(_BumpTex,i.uv);
                float3 normal=UnpackNormal(packNormal);
                normal.xy*=_BumpSacle;
                normal.z=sqrt(1.0- saturate(dot(normal.xy,normal.xy)));
                normal=normalize(float3(dot(i.TtoW0.xyz,normal),dot(i.TtoW1.xyz,normal),dot(i.TtoW2.xyz,normal)));
                float3 lightDir=normalize(i.lightDir);
                float3 viewDir=normalize(i.viewDir);
                float3 halfDir=normalize(lightDir+viewDir);

                fixed4 albedo =tex2D(_MainTex, i.uv)*_LightColor0*_Color;
                float3 F0=float3(0.01,0.01,0.01);

                float3 metallic=ARMTex.rgb;
                F0=lerp(F0,albedo,metallic);
                float3 Lo=float3(0,0,0);

                float roughness=ARMTex.g*_Roughness;

                float NDF=DistributionGGX(normal,halfDir,roughness);
                float G=GeometrySmith(normal,viewDir,lightDir,roughness);
                float3 F=FresnelSchlick(clamp(dot(halfDir,viewDir),0,1),F0);

                float3 nom=NDF*G*F;
                float3 denom=4*max(dot(normal,viewDir),0)*saturate(dot(normal,lightDir));
                float3 specular=nom/max(denom,0.001);

                float3 Ks=F;
                float3 Kd=1-Ks;
                Kd*=1.0-metallic;
                float NdotL=max(dot(normal,lightDir),0.0);
                Lo=(Kd*albedo+specular)*NdotL;

                half3 irradiance=ShadeSH9(float4(normal,1));
                half3 ambient=UNITY_LIGHTMODEL_AMBIENT;
                half3 diffuse=max(half3(0,0,0),ambient+irradiance)*albedo;

                roughness=roughness*(1.7-0.7*roughness);
                half mip=roughness*UNITY_SPECCUBE_LOD_STEPS;
                float3 R=reflect(-viewDir,normal);
                half4 rgbm=UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0,R,mip);
                half3 preColor=DecodeHDR(rgbm,unity_SpecCube0_HDR);

                half NdotV=saturate(dot(normal,viewDir));
                half2 brdf=tex2D(_BRDF,half2(lerp(0, 0.99,NdotV),lerp(0, 0.99,_Roughness))).rg;
                specular=preColor*(F*brdf.x+brdf.y);
                Lo+=Kd*diffuse+specular;

                float4 finalColor=float4(Lo,1.0);
               

                return finalColor;
            }
            ENDCG
        }
    }
}