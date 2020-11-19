Shader "Unlit/ImageEffectSahder"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Brightness("_Brightness",Range(0,2)) = 1
		_Saturation("_Saturation",Range(0,2)) = 1
		_Contrast("_Contrast",Range(0,2)) = 1

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
			fixed _Brightness;
			fixed _Saturation;
			fixed _Contrast;

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

                fixed4 renderTex  = tex2D(_MainTex, i.uv);

				// 亮度值调整
                fixed3 finalColor = renderTex.rgb * _Brightness;
                
                // 该像素对应的亮度值
                fixed luminance = 0.2125 * renderTex.r + 0.7154 * renderTex.g + 0.0721 * renderTex.b;
                //使用该亮度值创建一个饱和度为0的颜色
                fixed3 luminanceColor = fixed3(luminance, luminance, luminance);
                //将之前的颜色与该颜色进行插值运算，得到调整饱和度后的颜色
                finalColor = lerp(luminanceColor, finalColor, _Saturation);
                
                // 创建一个对比度度为0的颜色
                fixed3 avgColor = fixed3(0.5, 0.5, 0.5);
                //将之前的颜色与该颜色进行插值运算，得到调整对比度后的颜色
                finalColor = lerp(avgColor, finalColor, _Contrast);
                 //返回最终颜色   
                return fixed4(finalColor, renderTex.a); 


            }
            ENDCG
        }
    }
}
