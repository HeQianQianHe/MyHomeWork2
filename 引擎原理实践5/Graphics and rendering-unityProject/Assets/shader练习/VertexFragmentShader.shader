Shader "Unlit/VertexFragmentPixelClipShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color ("Color",Color)=(1,1,1,1)
		_Mul("mul",float) = 0.001
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
			//fragment他又叫像素着色器，它会执行该模型在屏幕上占据像素的逐像素操作，性能主要由其决定
			//声明顶点和片元处理函数，vertex是关键字vert是函数名
			//片元着色器包含一个像素所需要的的各种信息
			//多个片元着色器共同作用于一个像素

			sampler2D _MainTex;
            half4 _Color;
            float _Mul;
			//顶点函数的输入结构体，里面的变量由系统为我们填充
			struct vertInput
            {//冒号后面的POSITION是绑定语义，表示让系统辨认要填充的是什么
			//必须要有绑定语义，如果是自定义的变量没对应的绑定语义就用没使用过的TEXCOORDn来绑
                float4 pos : POSITION;
				//这里是模型顶点坐标
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

			//顶点函数输出结构体，会被片元函数使用，需要我们在vert里填充
            struct vertOutput
            {//SV是SystemValue，和底层渲染管线有关我们不用理
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };          

			//顶点函数，返回值为输出结构体，传入参数为输入结构体
            vertOutput vert (vertInput input)
            {
                vertOutput o;//声明输出结构体开始填充
                o.pos = UnityObjectToClipPos(input.pos);//必须将模型坐标转换为裁剪空间
				//原来是 mul(UNITY_MATRIX_MVP,input.pos)矩阵乘法来变换，现在简单了

				//模型空间-世界空间-观察空间-裁剪空间
				//从模型空间到世界空间的变换叫做模型变换
				//从世界空间到观察空间的变换叫做观察变换（视图变换）
				//从观察空间到裁剪空间的变换叫做投影变换

				//经过裁剪操作之后，还需将裁剪空间的坐标投影到屏幕空间,从裁剪空间到屏幕空间由unity直接进行
				//也就是说我们向vertOutput（输出结构体）里填充了裁剪空间顶点位置，unity会帮我转化为屏幕空间顶点坐标

				//SV_POSITION语义定义的是裁剪空间坐标？左下角为（0,0,0),z轴值始终为0
				o.normal =   mul(UNITY_MATRIX_M,input.normal);
				o.uv = input.uv;
                return o;
            }

            half4 frag (vertOutput output) : Color
            {
                half4 mainColor = tex2D(_MainTex, output.uv);

				output.pos.xy = floor(output.pos.xy * 0.1) * 0.5;
				//首先对屏幕像素乘0.1后的值向下取整可以得到像素的pos值呈阶梯状
				//例如屏幕长1000，乘以0.1后变为0-100，每10个像素值一样，例如前十个都是0，后十个都是1
				//然后再乘0.5，这样变为0-50，前十个为0，后十个为0.5，再十个为1
				
                float checker = - frac(output.pos.x + output.pos.y);
				//这里xy相加取小数就是有的是0.5结尾的能取到0.5这个值，有的整数的只能取到0，再加个负号
				//就成了像素要么是-0.5要么是0

				//clip(checker);
				//剪裁checker小于0的像素，等于0的不过滤

                //return mainColor * _Color;	
				return half4(output.normal,1);
				//这里的output.pos是屏幕坐标
            }
            ENDCG
        }
    }
}
