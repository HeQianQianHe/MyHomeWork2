Shader "Unlit/GrabShader"
{
    Properties
    {
       _MainTex("MainTex",2D) = "white"{}
	   _BumpMap("BumpMap",2D) = "bump"{}
	   _Magnitude("Magnitude",Range(0,2))=0.5
	   _ColorTint("ColorTint",Color) = (1,1,1,1)
	   _Power("Power",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent"}
		//这里物体的渲染队列要尽可能的往后面排
		//因为捕捉通道渲染时候需要场景里已经把其他物体渲染了再捕捉不然捕捉不到东西
        LOD 100
		//捕捉通道写在Pass前面，大括号里可以写引号“name”
		//这样捕捉到的图就会存入对应名字的变量里，多个shader都可以共享一个“name”里的图
		//不写的话默认是存在_GrabTexture这里
		GrabPass{}
		 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
			sampler2D _GrabTexture;
			//捕捉通道的图，就是相当于渲染该物体前给屏幕来了个截图

			sampler2D _MainTex;
			sampler2D _BumpMap;
			float _Magnitude;
			float4 _ColorTint;
			float _Power;
            struct vertInput
            {
                float4 pos : POSITION;
				float2 uv :TEXCOORD0;
            };
            struct vertOutput
            {
				float2 uv :TEXCOORD0;
				float4 uvgrab :TEXCOORD1;
				//捕捉通道也需要uv而且是float4类型的，TEXCOORD1要使用没有占用的语义
				//这个uvgrab感觉就是屏幕坐标
                float4 pos : SV_POSITION;
            };			

            vertOutput vert (vertInput v)
            {
                vertOutput o;
                o.pos = UnityObjectToClipPos(v.pos);//转换顶点坐标到剪裁空间
				o.uv=v.uv;//传递uv
				o.uvgrab = ComputeGrabScreenPos(o.pos);
				//这里生成grabuv时候要注意使用的是顶点剪裁空间的坐标
				//要是使用v.pos也就是模型空间的话，那么uv就错了
                return o;
            }

            half4 frag (vertOutput i) : Color
            {
				half4 mainColor = tex2D(_MainTex,i.uv) * _ColorTint;
				//采样主纹理乘上色调

				half2 bump = UnpackNormal( tex2D(_BumpMap,i.uv)).rg;
				//采样法线图，但是只要xy通道，以为过会加给uv就是俩通道

				i.uvgrab.xy += bump * _Magnitude ;
				//对grab的uv做改变，这样原本在这个物体后面的图像就会扭曲
				//不加这个uv干扰的话grab里面的图像就是物体背后的图像

				half4 grabTex = tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(i.uvgrab));
				//采样grab图像，固定写法
				//如果这样写tex2D(_GrabTexture,i.uv)用物体uv和普通采样，那么得到的就是类似RenderTexture一样
				//把后面图像渲染并贴上物体而已

				mainColor = pow( mainColor,_Power);//这就是改变一下颜色亮暗

                return grabTex * mainColor;
            }
            ENDCG
        }
    }
}
