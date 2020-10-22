Shader "Custom/TransparentShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
		{ "RenderType"="Transparent" 
		  "IgnoreProjector" = "True"
		  "Queue" = "Transparent"
		}
		//这里的 "Queue" = "Transparent"必须设置是渲染顺序，不设置渲染会出错
		//这是四个顺序Background,Geometry,Transparent,Overlay
        LOD 200  Cull off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
		//这里将fullforwardshadows删掉表示不接受阴影
		//alpha:fade不加上的话这个物体就是不透的，加上后
		//透明的部分才能和后面的物体混合
		//这部分属于[optionalparams]可选参数，在unity文档里有
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };
		sampler2D _MainTex;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
