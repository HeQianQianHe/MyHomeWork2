Shader "Unlit/GrayScaleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_GrayScale("GrayScale",Range(0,1)) = 1
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
			float _GrayScale;


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
			    //c#脚本里的OnRenderImage方法调用的时候会通过Graphics.Blit(src, dest, postMaterial)把屏幕图像传送给_MainTex里
                fixed4 col = tex2D(_MainTex, i.uv);

				//最暴力的算法是平均值Gray = (Red + Green + Blue) / 3，但是由于人眼对三种颜色的敏感度不同 绿>红>蓝 所以有了以下公式
				float lum = 0.299*col.r+0.587*col.g+0.114*col.b;

				//在灰色图像和原始彩色图像之间插值
				fixed4 finalColor = lerp(col,fixed4(lum,lum,lum,1),_GrayScale);

                return finalColor;
            }
            ENDCG
        }
    }
}
