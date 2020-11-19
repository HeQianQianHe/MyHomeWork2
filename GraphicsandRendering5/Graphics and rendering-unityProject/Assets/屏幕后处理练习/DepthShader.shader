Shader "Unlit/DepthShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DepthScale("DepthScale",Range(-1,6)) = 1
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
			float _DepthScale;
			//_CameraDepthTexture是unity内置变量
			sampler2D _CameraDepthTexture;


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
			
				//这里采样深度图，固定流程
                fixed4 col = tex2D(_CameraDepthTexture, i.uv.xy);

				float d = UNITY_SAMPLE_DEPTH(col);

				d = Linear01Depth(d);

				d = pow(d,_DepthScale);

                return d ;
            }
            ENDCG
        }
    }
}
