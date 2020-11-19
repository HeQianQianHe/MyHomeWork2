Shader "Custom/AlphaShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" { }
		_Intensity("Intensity",Float) = 0

		_Step1("Step1",Float) = 0
		_Step2("Step2",Float) = 1

		_Position("Pos",vector) = (0,0,0,0)

    }
    SubShader
    {
        Tags 
		{ "RenderType"="Opaque" "IgnoreProjector" = "True" "Queue" = "Transparent" }
        LOD 200 
		//Cull off
		Lighting Off
		//加上了灯光和双面渲染
	
        CGPROGRAM
        #pragma surface surf Standard alpha:fade 
		//这里加上alpha:fade 
        #pragma target 3.0
        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
        };
		sampler2D _MainTex;
        fixed4 _Color;
		float _Intensity;
		float _Step1;float _Step2;
		float4 _Position;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
			float n = 1 - distance(_Position,IN.worldPos) + _Intensity;
			//这里算一下我们自定义的位置和模型表面每个面之间的距离 距离在0-1时候输出1-0，大于1时候输出负数（消失掉）
			n = smoothstep(_Step1,_Step2,n);
			//这里的smoothstep和连连看里面的一样 可以让输入的n的值若是step1-step2之间的就将其映射为0-1之间的
            o.Alpha = c.a * n * 0.5;  o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
