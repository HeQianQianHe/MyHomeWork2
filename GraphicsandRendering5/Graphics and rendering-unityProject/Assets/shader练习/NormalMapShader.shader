Shader "Custom/NormalMapShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_NormalTex("Normal Map",2D) = "bump"{}
		//法线图的后面默认值里面写“bump”
		_NormalIntensity("NormalIntensity" , Range(0,1)) = 1
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
            float2 uv_NormalTex;
        };
		float4 _Color;
		sampler2D _NormalTex;
        float _NormalIntensity;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 normalMap = UnpackNormal(tex2D(_NormalTex,IN.uv_NormalTex));
			//这个UnpackNormal方法是写在unity安装目录下的CGIncludes下的UnityCG.cginc中的
			//因为法线图是rgb都是0 - 1的，但是法线需要-1 - 1的，所以需要 tex2D(...).xyz*2-1这样子变换
			//UnpackNormal方法就是做这个变换的

			normalMap.x *= _NormalIntensity;
			normalMap.y *= _NormalIntensity;
			normalMap = normalize(normalMap);
			//将法线的xy乘以强度后再标准化才能使用,不标准化也可以好像

			o.Normal = normalMap;
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
