Shader "Unlit/GrabscrewShader"
{
    Properties
    {	_ColorTint("ColorTint",Color) = (1,1,1,1)
       _MainTex("MainTex",2D) = "white"{}
	   _NoiseMap("NoiseMap",2D) = "bump"{}
	   _Magnitude("Magnitude",Range(0,2))=0.5
	   
	   _Scale("Scale",Range(0,1)) = 0.5

	   _OffectX("OffectX",float) = 0
	   _OffectY("OffectY",float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
        LOD 100
		//定义捕捉通道
		GrabPass{}
		 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _GrabTexture;
			sampler2D _MainTex;
			sampler2D _NoiseMap;
			float _Magnitude;
			float4 _ColorTint;
			float _Scale;
			float _OffectX;
			float _OffectY;

			//输入结构体
            struct vertInput
            {
                float4 pos : POSITION;
				float2 uv :TEXCOORD0;
            };
			//输出结构体
            struct vertOutput
            {
				float2 uv :TEXCOORD0;
				float4 uvgrab :TEXCOORD1;
                float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD2;
            };		

            vertOutput vert (vertInput v)
            {
                vertOutput o;
                o.pos = UnityObjectToClipPos(v.pos);
				o.uv=v.uv;
				o.uvgrab = ComputeGrabScreenPos(o.pos);//捕捉通道uv
				o.worldPos = mul(UNITY_MATRIX_M,v.pos);//模型到世界坐标转换
                return o;
            }

            half4 frag (vertOutput i) : Color
            {
				half4 mainColor = tex2D(_MainTex,i.uv) * _ColorTint;//采样主纹理乘上色调


				float2 grapDistortion =float2( tex2D(_NoiseMap,i.worldPos.xy * _Scale + _OffectX).r-0.5,
				                               tex2D(_NoiseMap,i.worldPos.xy * _Scale + _OffectY).r-0.5);


				i.uvgrab.xy += grapDistortion * _Magnitude ;



				half4 grabTex = tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(i.uvgrab));
                return grabTex * mainColor;
				//return tex2D(_NoiseMap,i.worldPos.xy );
				//返回这个从xy方向铺满世界的躁波图就能发现，其实就像躁波图从z轴向xy屏幕投影一样
            }
            ENDCG
        }
    }
}
