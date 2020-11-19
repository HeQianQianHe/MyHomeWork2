Shader "Custom/ToonShader"
{
    Properties
    {   //Toon1
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ToonRemp("ToonRemp",2D) = "gray"{}
		_ToonOffect("Offect", float) = 0
		//Toon2
		_ToonLevels("ToonLevels", Range(2,10)) = 4
		//Toon3
		_ToonColor1("ToonColor1",Color)=(1,1,1,1)
		_ToonColor2("ToonColor2",Color)=(0.75,0.75,0.75,1) _ToonColor3("ToonColor3",Color)=(0.5,0.5,0.5,1)
		_ToonColor4("ToonColor4",Color)=(0.25,0.25,0.25,1) _ToonColor5("ToonColor5",Color)=(0,0,0,1)
		
		_ToonValue1("ToonValue1",float)=0.75 _ToonValue2("ToonValue2",float)=0.55
		_ToonValue3("ToonValue3",float)=0.35 _ToonValue4("ToonValue4",float)=0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Toon3
		//这里用自定义卡通渲染光照模型 Toon1,Toon2,Toon3
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };
		sampler2D _MainTex;
		sampler2D _ToonRemp;
		float _ToonOffect;
		float _ToonLevels;
		float4 _ToonColor1;float4 _ToonColor2;float4 _ToonColor3;float4 _ToonColor4;float4 _ToonColor5;
		float _ToonValue1;float _ToonValue2;float _ToonValue3;float _ToonValue4;

        void surf (Input IN, inout SurfaceOutput o)
        {   //采样贴图，传入信息进输出结构体
			o.Albedo = tex2D(_MainTex,IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_MainTex,IN.uv_MainTex).a;
        }

		//接下来有3个卡通光照模型
		half4 LightingToon(SurfaceOutput o,half3 lightDir,half atten)
		{
			half LdotN = dot(lightDir,o.Normal);
			//先点乘，获取到随着角度变化的 -1 到 1的值
			half4 toonColor = tex2D(_ToonRemp,fixed2( LdotN + _ToonOffect ,0.5));
			//采样准备好的阶梯状纹理图，利用刚刚求得的dot值（-1到1）作为uv坐标的u向（横着），v向随意反正都一样
			//这里记得把贴图模式改为Clamp，因为uv是从0-1，而dot的值有负数，所以设置Clamp使得不在0-1内的uv取边缘延伸值
			half4 c;
			c.rgb = o.Albedo.rgb * _LightColor0.rgb * atten * toonColor;
			c.a = o.Alpha;
			return c;
		}


		half4 LightingToon2(SurfaceOutput o,half3 lightDir,half atten)
		{
			half LdotN = dot(lightDir,o.Normal);
			//先点乘，获取到随着角度变化的 -1 到 1的值
			half toonColor = floor(LdotN * _ToonLevels)/(_ToonLevels );
			//这里是个骚操作，floor是向下取整，相当于把dot值先扩大在划分到整数区域内，再除个数使其变回-1到1
			half4 c;
			c.rgb = o.Albedo.rgb * _LightColor0.rgb * toonColor * atten;
			c.a = o.Alpha;
			return c;
		}


		half4 LightingToon3(SurfaceOutput o,half3 lightDir,half atten)
		{   //这里是傻办法，就是直接根据dot的值（-1到1）来指定不同数值范围对应不同颜色
			half LdotN = dot(lightDir,o.Normal);
			half4 toonColor;
			if(LdotN < 1 && LdotN > _ToonValue1)
			{
				toonColor = _ToonColor1;
			}else if(LdotN < _ToonValue1 && LdotN > _ToonValue2)
			{
				toonColor = _ToonColor2;
			}else if(LdotN < _ToonValue2 && LdotN > _ToonValue3)
			{
				toonColor = _ToonColor3;
			}else if(LdotN < _ToonValue3 && LdotN > _ToonValue4)
			{
				toonColor = _ToonColor4;
			}else if(LdotN < _ToonValue4 && LdotN > -1)
			{
				toonColor = _ToonColor5;
			}
			half4 c;
			c.rgb = o.Albedo.rgb * _LightColor0.rgb * toonColor.rgb * atten;
			c.a = o.Alpha;
			return c;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
