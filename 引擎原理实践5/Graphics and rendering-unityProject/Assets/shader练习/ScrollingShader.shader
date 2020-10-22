Shader "Custom/ScrollingShader"
{
    Properties
    {
        _MainTint("ColorTint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ScrollingXSpeed("X Scrolling Speed",Range(0,10)) = 2
		_ScrollingYSpeed("Y Scrolling Speed",Range(0,10)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

		sampler2D _MainTex;
        fixed4 _MainTint;
		fixed _ScrollingXSpeed;
		fixed _ScrollingYSpeed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed2 scrolledUV = IN.uv_MainTex + fixed2(_ScrollingXSpeed * _Time.x , _ScrollingYSpeed * _Time.x);
			
			//IN.uv_MainTex是原来从输入结构体里获得的uv  加上的fixed2是要偏移的量 给x和y的speed都乘以了_Time.x
			//让他们能随着时间偏移
			//_Time是个float4类型的  里面四个值是 t/20,t,t*2,t*3
			//_SinTime是个float4类型的  里面四个值是 Sint/8,Sint/4,Sint/2,Sint
			//_CosTime是个float4类型的  里面四个值是 Cost/8,Cost/4,Cost/2,Cost
			//unity_DeltaTime是个float4类型的  里面四个值是 dt,1/dt,smoothDt,1/smoothDt

			o.Albedo = tex2D(_MainTex,scrolledUV)*_MainTint;
			//用偏移UV采样主纹理，得到的颜色值乘以我们设置的色相

			o.Alpha = tex2D(_MainTex,scrolledUV).a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
