Shader "Unlit/OldTvFilmEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_OldFilmEffectAmount("OldFilmEffectAmount",Range(0,1)) = 1
		_vignetteAmount("vignetteAmount",Range(0,1)) = 1
		_scratchesXSpeed("scratchesXSpeed",Range(-50,50)) = 1
		_scratchesYSpeed("scratchesYSpeed",Range(-50,50)) = 1
		_dustYspeed("dustYspeed",Range(-50,50)) = 1
		_dustXspeed("dustXspeed",Range(-50,50)) = 1
		_randomValue("randomValue",float) =  0
		_vignetteTexture("vignetteTexture",2D) = "white" {}
		_scratchesTexture("scratchesTexture",2D) = "white" {}
		_dustTexture("dustTexture",2D) = "white" {}
		_sepiaColor("sepiaColor",Color) = (1,1,1,1)

		_contrast("contrast",float) = 1
		_effectAmount("effectAmount",Range(-10,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _sepiaColor;
			fixed _OldFilmEffectAmount;
			fixed _vignetteAmount;
			fixed _scratchesYSpeed;
			fixed _scratchesXSpeed;
			fixed _dustYspeed;
			fixed _dustXspeed;
			fixed _randomValue;
			sampler2D _vignetteTexture;
			sampler2D _scratchesTexture;
			sampler2D _dustTexture;
			fixed _contrast;
			fixed _effectAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				//扭曲相机的渲染图
				half2 distortedUV = half2(i.uv.x,i.uv.y + (_randomValue*_SinTime.z*0.005));
				fixed4 renderTexture = tex2D(_MainTex, distortedUV);

				//暗角图片
                fixed4 vignetteTexture = tex2D(_vignetteTexture, i.uv);

				//划痕图片
				half2 scratchesUV = half2(i.uv.x+ (_randomValue*_SinTime.z*_scratchesXSpeed),i.uv.y + (_Time.x*_scratchesYSpeed));
				fixed4 scratchesTexture = tex2D(_scratchesTexture, scratchesUV);
				
				//尘土图片
				half2 dustUV = half2(i.uv.x+ (_randomValue*_SinTime.z*_dustXspeed),i.uv.y + (_Time.x*_dustYspeed));
				fixed4 dustTexture = tex2D(_dustTexture, dustUV);

				//计算亮度 lum出来其实是黑白图
				fixed lum = dot(fixed3(0.299,0.587,0.144),renderTexture.rgb);
				fixed4 finalColor = lum + lerp(_sepiaColor,_sepiaColor+fixed4(0.1f,0.1f,0.1f,1.0f),_randomValue);
				finalColor = pow(finalColor,_contrast);

				//叠加各种图
				fixed3 white = fixed3(1,1,1);
				finalColor = lerp(finalColor,finalColor*vignetteTexture,_vignetteAmount);
				finalColor.rgb *= lerp(scratchesTexture,white,_randomValue);//让他们和一个白色混合 来控制这个图起作用的强度
				finalColor.rgb  *= lerp(dustTexture,white,_randomValue*_SinTime.z);
				finalColor = lerp(renderTexture,finalColor,_effectAmount);

                return finalColor;
            }
            ENDCG
        }
    }
}
