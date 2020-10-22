Shader "Custom/ExplosionShader"
{
    Properties
    {
        _RampTex ("RampTex", 2D) = "white" {}
		_RampOffect("RampOffect",Range(-1,1)) = 0
		_NoiseTex("NoiseTex",2D) = "gray" {}
		_Period("Period",Range(0,1)) = 0.5
		_Amount("Amount",Range(0,1))=0.1
		_Clip("Clip",Range(0,1))=1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200 Cull Off
		Lighting Off
        CGPROGRAM

        #pragma surface surf Standard vertex:vert nolightmap
		//定义顶点函数
        #pragma target 3.0
        struct Input
        {
            float2 uv_NoiseTex;
			float _SinValue;
			//声明自定义变量存储sin值
        };
		sampler2D _RampTex;
		float _RampOffect;
		sampler2D _NoiseTex;
		float _Period;
		float _Amount;
		float _Clip;
	   void vert(inout appdata_full v,out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input , o);//初始化
			fixed4 noiseTex = tex2Dlod(_NoiseTex , float4(v.texcoord.xy,0,0));//采样躁波图
			o._SinValue = sin(_Time.z * _Period + noiseTex.r * 10) * _Amount;//加noise是为了让每个地方sin值都不一样
			//这里把sin值顺便存入输入结构体
			v.vertex.xyz += v.normal * noiseTex.r * o._SinValue;
		}
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed4 noiseTex = tex2D (_NoiseTex, IN.uv_NoiseTex);
			fixed4 rampTex = tex2D (_RampTex, float2(noiseTex.r + _RampOffect ,0.5));
			//采样渐变纹理，采样的uv利用noise值，noise值在0-1之间，这样noise的色和白色部分就会显示出ramp的颜色
			clip(_Clip - noiseTex.r - IN._SinValue);
			//clip函数用于裁切像素，括号内参数小于0就把这个像素裁剪掉
			//减去noiseTex.r 是为了让noise图对应的数值大的地方被裁剪掉
			//_Clip是自定义变量，他等于0时候基本都被裁剪
			// IN._SinValue是vert函数中存的值，会随时间变化
            o.Albedo = rampTex.rgb;
            o.Emission = rampTex.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
