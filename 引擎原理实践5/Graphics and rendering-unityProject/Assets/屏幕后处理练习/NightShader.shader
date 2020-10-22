Shader "Unlit/NightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_nightVignetteTexture ("nightVignetteTexture", 2D) = "white" {}
		_scanLineTexture ("scanLineTexture", 2D) = "white" {}
		_nightVsionNoiseTexture ("nightVsionNoiseTexture", 2D) = "white" {}
		_nightContrast("nightContrast",float) = 1
		_nightbrightness("nightbrightness",float) = 1
		_scanLineTileAmount("scanLineTileAmount",float) = 1
		_noiseXspeed("noiseXspeed",float) = 1
		_noiseYspeed("noiseYspeed",float) = 1
		_scale("scale",float) = 1
		_distortion("distortion",float) = 1
		_randomValue("randomValue",float) = 1
		_nightVisionColor("nightVisionColor",Color) = (1,1,1,1)
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
			sampler2D _nightVignetteTexture;
			sampler2D _scanLineTexture;
			sampler2D _nightVsionNoiseTexture;
			fixed _nightContrast;
			fixed _nightbrightness;
			fixed _scanLineTileAmount;
			fixed _noiseXspeed;
			fixed _noiseYspeed;
			fixed _scale;
			fixed _distortion;
			fixed _randomValue;
			fixed4 _nightVisionColor;

			//桶形畸变算法
			float2 barrelDistortion(float2 coor)
			{
				float2 h = coor.xy-float2(0.5,0.5);
				float2 r2 = h.x * h.x + h.y*h.y;
				float f = 1.0+r2*(_distortion*sqrt(r2));
				return f * _scale *h + 0.5;
			}

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
				half2 distortionUV = barrelDistortion(i.uv);
                fixed4 renderTex = tex2D(_MainTex, distortionUV);

				//暗角图片采样
				fixed4 vignetteTex = tex2D(_nightVignetteTexture, i.uv);
				
				//扫描线图片采样
				half2 scanLineUV = half2(i.uv.x*_scanLineTileAmount,i.uv.y*_scanLineTileAmount);//uv乘一个值，让贴图缩放重复
				fixed4 scanLineTex = tex2D(_scanLineTexture, scanLineUV);

				//噪波图采样
				half2 noiseUV = half2(i.uv.x+(_randomValue*_SinTime.z*_noiseXspeed),i.uv.y+(_SinTime.x*_noiseXspeed));//加上sin表示上下或者左右来回运动
				fixed4 noiseTex = tex2D(_nightVsionNoiseTexture, noiseUV);                                            //乘以randomValue表示会跳跃的显示

				//计算亮度
				fixed lum = dot(fixed3(0.299,0.587,0.144),renderTex.rgb);
				lum += _nightbrightness;//调整亮度
				fixed4 finalColor = lum + _nightVisionColor;//色调
				finalColor = pow(finalColor,_nightContrast);//对比度
				finalColor = finalColor * vignetteTex * scanLineTex * noiseTex;//合并各种图层

                return finalColor;
            }
            ENDCG
        }
    }
}
