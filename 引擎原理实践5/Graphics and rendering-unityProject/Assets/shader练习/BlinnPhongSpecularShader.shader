Shader "Custom/PhongSpecularShader"
{
    Properties
    {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)
        _SpecularPower("SpecularPower",Range(0.1,60)) = 1//Blinn反射要求次方值大一些
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf MyBlinnPhongSpecular fullforwardshadows
		//用自定义光照模型
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
			o.Albedo = tex2D(_MainTex,IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_MainTex,IN.uv_MainTex).a;
        }
		half4 LightingMyBlinnPhongSpecular(SurfaceOutput o,fixed3 lightDir,half3 viewDir,fixed atten)
		{
			float3 halfVector = normalize(lightDir + viewDir);
			//求得半矢量halfVector = 灯光方向+视角方向 再标准化
			float spec = pow( max(0,dot(o.Normal,halfVector)),_SpecularPower);
			//这里求反射强度
			//根据blinn公式 spec = dot（Normal，halfVector）^ p 他俩点积的次方，次方数可控制 
			float4 finalSpec = spec * _SpecularColor;
			//将反射强度系数和颜色相乘得到反射具体颜色
			fixed4 c;
			c.rgb = (o.Albedo * _LightColor0.rgb * max(0,dot(lightDir,o.Normal)) * atten) + (finalSpec * _LightColor0.rgb);
			//最终颜色 = Diffuse + Specular
			c.a = o.Alpha;
			return c;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
