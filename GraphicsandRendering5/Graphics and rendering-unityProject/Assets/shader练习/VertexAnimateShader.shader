Shader "Custom/VertexAnimateShader"
{
    Properties
    {
        _MainTint ("MainTint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

		 _ColorA("ColorA", Color) = (1,1,1,1)
		 _ColorB("ColorB", Color) = (1,1,1,1)
		 _Speed("Speed",Float) = 1
		 _Frequency("Frequency",Float) = 1
		 _Amplitude("Amplitude",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent" }
        LOD 200 Cull off

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
		//要想使用顶点处理函数，需要在这定义vertex:vert
        #pragma target 3.0
        
        struct Input
        {
            float2 uv_MainTex;
			float waveValue;
        };
		fixed4 _MainTint;
		sampler2D _MainTex;
        fixed4 _ColorA;
		fixed4 _ColorB;
		float _Speed;
		float _Frequency;
		float _Amplitude;

		void vert(inout appdata_full v,out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input , o);
			//这是对输入结构进行初始化，因为我们在输入结构体里定义了uv_MainTex
			//但是我们不会在vert函数里手动初始化他，我们只能手动初始waveValue
			//这样没初始uv_MainTex他就会报错，所以要让系统帮我们初始
			//可以想成是我们不自定义vert时候系统自己的vert是会初始化所有的
			//但是我们自定义了，那就要手动初始化所有，或者调用上面这个方法
			float time = _Time *_Speed;
			float waveValue = sin(time+v.vertex.x*_Frequency) * _Amplitude; 
			//不乘_Amplitude时候，waveValue取值为-1到1
			//sin(time+v.vertex.x)表示依据x轴的位置，同一时刻每个点的waveValue都不一样
			//v.vertex.x乘上_Frequency表示让每个点间隔增大或缩小
			//最后乘_Amplitude就是整体控制值大小
			v.vertex.y = waveValue + v.vertex.y;
			//这里可以直接让y等于waveValue，加上y是为了让球体产生波浪时候不被挤成面片
			//这里v.vertex.y 每一帧都是取自原始模型，不能累加
			//就是说这一帧将其改变后，下一帧再取这个值还是原来的不会变
			v.normal = float3(v.normal.x +waveValue*0.3 ,v.normal.y ,v.normal.z ) ;
			//不懂为啥这样改法线
			o.waveValue = waveValue;
			//把这个值保存进输入结构体里，这样好在surf里调用
			//其实我发现可以新定义一个变量来存储他，更方便(不可行，值无法传递)
		}

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 c = tex2D(_MainTex ,IN.uv_MainTex)*_MainTint;
			float3 waveColor = lerp(_ColorA,_ColorB, IN.waveValue).rgb;
			//根据waveValue值对
			o.Albedo = c.rgb * waveColor;
			o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
