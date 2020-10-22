Shader "Custom/PhongSpeculaterShader"
{
    Properties
    {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)
        _SpecularPower("SpecularPower",Range(0.1,30)) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf MyPhongSpecular fullforwardshadows
		//用自定义光照模型Phong式反射
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

		sampler2D _MainTex;
        fixed4 _SpecularColor;
		float _SpecularPower;

        void surf (Input IN, inout SurfaceOutput o)
        {
			fixed4 c = tex2D(_MainTex,IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
        }
		
		//这里的自定义光照模型多了个viewDir的参数
		//half4 Lighting+Nmae(SurfaceOutput o,fixed3 lightDir,fixed atten)
		//half4 Lighting+Name(SurfaceOutput o,fixed3 lightDir,half3 viewDir,fixed atten)
		half4 LightingMyPhongSpecular(SurfaceOutput o,fixed3 lightDir,half3 viewDir,fixed atten)
		{
			float LdotN = dot(lightDir,o.Normal);
			//先点积求得lightDir在Normal上的投影长度
			float3 reflectionVector = normalize(2 * o.Normal * LdotN - lightDir);
			//根据向量加减变换得到lightDir入射后与Normal对称的反射向量reflectionVector
			float spec = pow(max(0,dot(reflectionVector,viewDir)),_SpecularPower);
			//这里求反射强度
			//根据公式 spec = dot（reflectionVector，viewDir）^ p 他俩点积的次方，次方数可控制 
			//这里本质上是取viewDir和反射向量的点积的次方，要用max取点积后的正值，否则负值会导致次方后也是负值，和下面的
			//漫反射相加后还是负值，表现出黑色
			float4 finalSpec = spec * _SpecularColor;
			//将反射强度系数和颜色相乘得到反射具体颜色
			fixed4 c;
			c.rgb = (o.Albedo * _LightColor0.rgb * max(0,LdotN) * atten) + (finalSpec * _LightColor0.rgb);
			//最终颜色 = Diffuse + Specular
			c.a = o.Alpha;
			return c;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
