Shader "Unlit/BlendImageShader"
{
    Properties
    {
		_MainTex ("Texture", 2D) = "white" {}
        _BlendTex ("BlendTexture", 2D) = "white" {}
		_Opcity("Opcity",Range(0,1)) = 1

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
			sampler2D _BlendTex;
			fixed _Opcity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			//用于OverLay混合的函数
			fixed4 OverLayBlendFun(fixed basePixel,fixed blendPixel)
			{
				if(basePixel<0.5)
				{
					return 2 * basePixel * blendPixel;

				}else
				{
					return 1 - (2 * (1-basePixel) * (1-blendPixel));
				}
			}


            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 renderTex = tex2D(_MainTex, i.uv);
				fixed4 blendTex = tex2D(_BlendTex, i.uv);

				//乘法混合
				//fixed4 blendMul = renderTex * blendTex;
				//renderTex = lerp(renderTex,blendMul,_Opcity);

				//加法混合
				//fixed4 blendAdd = renderTex + blendTex;
				//renderTex = lerp(renderTex,blendAdd,_Opcity);

				//图层混合
				//fixed4 blendLayer =1 - ((1-renderTex) * (1-blendTex));
				//renderTex = lerp(renderTex,blendLayer,_Opcity);

				//OverLay混合
				fixed4 blendedImage = renderTex;
				blendedImage.r = OverLayBlendFun(blendedImage.r,blendTex.r);
				blendedImage.g = OverLayBlendFun(blendedImage.g,blendTex.g);
				blendedImage.b= OverLayBlendFun(blendedImage.b,blendTex.b);
				renderTex = lerp(renderTex,blendedImage,_Opcity);

                return renderTex;
            }
            ENDCG
        }
    }
}
