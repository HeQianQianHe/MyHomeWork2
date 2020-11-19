Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200  

        CGPROGRAM
		//                Input                SurfaceOutput
		//Vertexmodifier   -→   SurfaceFunction   -→   Lightmodel
        #pragma surface surf Standard fullforwardshadows
		//surf为SurfaceFunction函数  Standard为光照模型  fullforwardshadows为接收阴影
        #pragma target 3.0

        struct Input
        {
		    //这里是获取到模型的某个贴图的uv信息
            float2 uv_MainTex;
			//这里至少有一个这个，就算没用也要有
        };

		sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//tex2D对纹理进行采样得到颜色图 然后乘以 _Color
            
			//给输出结构体装值，这样灯光模型就会用这些值计算
			o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
