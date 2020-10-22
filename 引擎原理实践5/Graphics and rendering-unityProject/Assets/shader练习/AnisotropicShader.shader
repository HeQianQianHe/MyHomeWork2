Shader "Custom/AnisotropicShader"
{
    Properties
    {
        _MainTint ("MainTint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_AnisotropicDir("AnisotropicDir", 2D) = "white" {} //这里放用于各项异形的法线图
		_AnisotropicOffect("AnisotropicOffect",Range(-1,1)) = -0.2  //这个控制各向异性圆圈偏移距离
		_SpecularColor ("SpecularColor", Color) = (1,1,1,1)  //控制高光颜色
		_SpecularAmount("SpecularAmount",Range(0,1)) = 0.5   //控制整体高光有无
		_SpecularPower("SpecularPower",Range(0,1)) = 0.5     //控制高光清晰还是模糊（次方数，称了128比例系数）
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf AnisotropicSpecular fullforwardshadows
		//用自定义光照模型
        #pragma target 3.0
        struct Input
        {
            float2 uv_MainTex;
			float2 uv_AnisotropicDir;
        };
		sampler2D _MainTex;
        fixed4 _MainTint;
		sampler2D _AnisotropicDir;
		float _AnisotropicOffect;
		fixed4 _SpecularColor;
		float _SpecularAmount;
		float _SpecularPower;

		//用自定义输出结构体，末尾记得分号
		struct SurfaceAnisotropicOutput
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed3 AnisotropicDir;
			half Specular;
			fixed Gloss;
			fixed Alpha;
		};

        void surf (Input IN, inout SurfaceAnisotropicOutput o)
        {
			fixed4 c = tex2D(_MainTex,IN.uv_MainTex) * _MainTint;
			float3 AnisoTex = UnpackNormal(tex2D(_AnisotropicDir,IN.uv_AnisotropicDir));
			//这里法线图作为影响高光产生各向异性效果的贴图
			//主要由这个法线图和物体本身法线相加做出假法线来参与blinn高光计算
			//(后面除了对法线相加操作外，还有对法线值取sin来制作圆环的效果)
			//为什么自定义输出结构体呢？
			//这里要想采样自定义贴图必须要输入结构体里的uv，所以我们采样完要将其装进自定义输出结构体里
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.AnisotropicDir = AnisoTex;
			o.Specular = _SpecularAmount;
			o.Gloss = _SpecularPower;
        }

		half4 LightingAnisotropicSpecular(SurfaceAnisotropicOutput o,fixed3 lightDir,fixed3 viewDir,fixed atten)
		{
			fixed3 halfVector = normalize(normalize(lightDir)+normalize(viewDir));
			//取的半矢量的标准化
			fixed HdotN = dot(normalize(o.Normal+o.AnisotropicDir),halfVector); //dot后的值是标准化法线和半矢量夹角的cos值，-1到1
			//将修改过的法线（物体法线+贴图法线）（标准化）与半矢量做点积
			fixed FixHdotN = max(0,sin(radians(( HdotN + _AnisotropicOffect )*180))); 
			//radians(degree)角度变为弧度
			//sin(radians)
			//HdotN + _AnisotropicOffect的范围在-2到2之间，乘180后在-360到360之间，转为弧度后再sin后在-1到1之间，max后在0-1之间
			//按照公式 S(Blinn)=（N点积H）^ p  这里对N点积H一顿操作后再次方
			float spec = max(0,pow(FixHdotN , o.Gloss * 128) * o.Specular);
			//这个Gloss是次方数，控制高光聚集还是分散，Specular是后乘的数控制高光有无
			fixed4 c;
			c.rgb = (o.Albedo * _LightColor0.rgb * max(0,dot(o.Normal,lightDir)) + (spec * _SpecularColor.rgb * _LightColor0.rgb)) * atten;
			c.a = o.Alpha;
			return c;
		}
        ENDCG
    }
    FallBack "Diffuse"
}
