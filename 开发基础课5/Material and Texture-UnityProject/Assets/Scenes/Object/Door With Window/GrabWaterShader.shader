Shader "Unlit/GrabShader2"
{
    Properties
    {	_ColorTint("ColorTint",Color) = (1,1,1,1)
       _MainTex("MainTex",2D) = "white"{}
	   _NoiseMap("NoiseMap",2D) = "bump"{}
	   _Magnitude("Magnitude",Range(0,2))=0.5
	   
	   _Speed("Speed",Range(-1,1)) = 1
	   _Scale("Scale",Range(0,1)) = 0.5
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
			float _Speed;
			float _Scale;

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
				float sinValue = sin(_Time.z*_Speed);
				
				float2 grapDistortion =float2( tex2D(_NoiseMap,i.worldPos.xy * _Scale+float2(sinValue,0)).r-0.5,
				                               tex2D(_NoiseMap,i.worldPos.xy * _Scale+float2(0,sinValue)).r-0.5);
				


				//制作在等会加给graphuv的扭曲值，主要思路是采样躁波纹理，利用躁波纹理的-0.5到0.5的黑白通道值来偏移grapuv
				//但是这里采样躁波纹理不能直接用模型uv，因为这样物体移动会使得扭曲特效跟着走，所以这里用世界坐标点采样
				//相当于躁波在xy方向上铺满了整个世界
				//后面乘以_Scale是为了放大缩小躁波图，加上sin是为了随时间律动
				//减去0.5是为了让值在-0.5到0.5之间，否则显示的图像会偏离原位而且都是一个方向的感觉不好看
				//如果是0-1的话图像只会往一边偏
				//这里float2里面采样了两次躁波图相当于grap的uv横向
				//我发现不要加sin效果会更好,加了sin就会来回走了

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
