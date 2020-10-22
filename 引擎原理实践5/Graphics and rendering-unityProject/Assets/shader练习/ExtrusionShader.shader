Shader "Custom/Extrued"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ExtrusionIntensity ("Intensity",Range(-0.1,0.1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };
		sampler2D _MainTex;
        fixed4 _Color;
		float _ExtrusionIntensity;

		void vert(inout appdata_full v)
		{
			//这里也可以对贴图进行采样，但是方式和surf函数里的不一样
			//float4 map = tex2Dlod(_Map , float4(v.texcoord.xy,0,0));
			v.vertex.xyz += v.normal * _ExtrusionIntensity ;			
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
