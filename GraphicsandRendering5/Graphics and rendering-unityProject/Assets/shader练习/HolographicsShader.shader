Shader "Custom/HolographicsShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_EmissionColor ("EmissionColor", Color) = (1,1,1,1)
		_EmissionIntensity("EmissionIntensity",float) = 1
        _MainTex ("Albedo (RGB)", 2D) = "white" { }
		_HoloIntensity("HoloIntensity",Range(0,1)) = 0.25//全息效果的强度（就是菲涅尔效果
		_Power("Power",Range(-1,5)) = 1
    }
    SubShader
    {
        Tags 
		{ "RenderType"="Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent"
		}
        LOD 200 
		Cull off
		//Lighting Off 加上了灯光和双面渲染
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade nolightmap
		//这里节省运算性能所以改为Lambert 去掉fullforwardshadows 加上alpha:fade 
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
			float3 worldNormal;
			float3 viewDir;
        };

		sampler2D _MainTex; fixed4 _Color;fixed4 _EmissionColor;
		float _HoloIntensity;float _Power;float _EmissionIntensity;
		
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

			float border = 1 - abs(dot(normalize(IN.worldNormal),normalize(IN.viewDir)));
			//法线和观察视角做点乘得到垂直时为0，平行时为1的值，用1减去他的绝对值
			//这样垂直时候是1，平行时候是0
			float alpha = border * (1 - _HoloIntensity) + _HoloIntensity;
			//这里是线性差值公式lerp（a,b,t）→ a+(b-a)t 让我们得到的这个值在 他自己 与 1 之间变换
			//lerp（border,1,_HoloIntensity）→ border + (1 - border) * _HoloIntensity
			//border + _HoloIntensity - _HoloIntensity*border = border * (1 - _HoloIntensity) + _HoloIntensity
			alpha = pow(alpha,_Power);
            o.Alpha = c.a * alpha;//将其与原本的alpha相乘
			o.Emission = o.Alpha * _EmissionColor.rgb * _EmissionIntensity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
