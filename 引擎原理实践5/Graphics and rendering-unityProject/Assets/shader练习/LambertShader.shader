Shader "Custom/LambertShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf MyLambert 
		//这里吧Standard（unity内置的）光照模型换为我们自定义的MyLambert
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;

		//这里输出结构体要改为SurfaceOutput
        void surf (Input IN, inout SurfaceOutput o)
        {
			o.Albedo = tex2D(_MainTex,IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_MainTex,IN.uv_MainTex).a;
			//把贴图的颜色和透明度传递给输出结构体
        }

		//这是自定义光照模型的固定实现方法
		//返回值是half4，就是返回计算出的颜色  名字是：Lighting+MyLambert
		//参数有三个，第一个是前面的输出结构体 后两个一个是灯光方向（从模型指向灯光），另一个是灯光衰减
		half4 LightingMyLambert(SurfaceOutput o,half3 lightDir,half atten)
		{
			half LdotN = dot(lightDir,o.Normal);
			//先做一个灯光方向和法线的点乘，得到的值垂直最小平行同向最大
			//以他作为下面的乘法系数

			half4 c;
			c.rgb = o.Albedo.rgb * _LightColor0.rgb * LdotN * atten;
			//贴图颜色*灯光颜色*乘法系数（灯方向和法线点乘）*衰减
			c.a = o.Alpha;
			return c;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
