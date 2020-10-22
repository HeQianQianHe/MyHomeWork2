Shader "Custom/CircleShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Center("Center",vector) = (0,0,0,0)
		_Radius("Radius",float) = 1
		_RadiusWidth("RadiusWidth",float) = 1
		_CircleColor("CircleColor",Color) = (1,0,0,1)
    }
    SubShader
    {
        Tags {  "RenderType"="Transparent"  "Queue" = "Transparent-1"}
		//这里Queue后面的参数可以加一或者减一
        LOD 200 Cull Off

        CGPROGRAM

        #pragma surface surf Standard alpha:fade
		//不接受阴影并且改为alpha:fade
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
        };
		sampler2D _MainTex;
		float4 _Center;
		float _Radius;
		float _RadiusWidth;
		float4 _CircleColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float dis = distance(_Center,IN.worldPos);
			//算一下自定义位置和每个面之间的距离
			if(dis>_Radius&&dis<_Radius+_RadiusWidth)
			{//若这个距离在（半径）和（半径+圆环宽度）之间就显示红色，否则显示原来的贴图颜色
				o.Albedo = _CircleColor;
				o.Alpha = _CircleColor.a;
			}else
			{
				o.Albedo = tex2D(_MainTex,IN.uv_MainTex).rgb;
				o.Alpha = tex2D(_MainTex,IN.uv_MainTex).a;
			}
        }
        ENDCG
    }
    FallBack "Diffuse"
}
