Shader "Custom/SnowShader"
{	Properties
    {
        _MainTint("MainTint", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex("Normal Map",2D) = "bump"{}
		_SnowLevel("SnowLevel",Range(0,1)) = 0.5
		_SnowColor("SnowColor", Color) = (1,1,1,1)
		_SnowDir("_SnowDir",vector)=(0,1,0,0)
		_SnowDepth("_SnowDepth",Range(0,1)) = 0.5
    }
    SubShader
    {	Tags { "RenderType"="Opaque" "Queue" = "Transparent" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #pragma target 3.0
        struct Input
        {
            float2 uv_MainTex;
			float3 customNormal;//自定义输入结构体，要在vert里赋值
			float3 worldNormal;
			INTERNAL_DATA
        };
        fixed4 _MainTint;
		sampler2D _MainTex;
		sampler2D _NormalTex;
		float _SnowLevel;
		fixed4 _SnowColor;
		fixed4 _SnowDir;
		float _SnowDepth;

		void vert (inout appdata_full v, out Input o)
		{
			  UNITY_INITIALIZE_OUTPUT(Input , o);
			  o.customNormal = v.normal;
			  //o.customNormal = mul(unity_ObjectToWorld,v.normal);

			  float3 normalMap = UnpackNormal(tex2Dlod(_NormalTex , float4(v.texcoord.xy,0,0)));
			  //采样法线图
			  float3 transNormal =  mul(unity_ObjectToWorld,v.normal +normalMap);
			  //将法线图和模型法线相加，然后乘以矩阵做转换，转换为世界空间
			  if(dot(transNormal,_SnowDir.xyz) >= _SnowLevel)
			  //两个世界坐标向量点乘，判断夹角cos值，夹角小的就偏移位置坐标模拟下雪
				v.vertex.xyz += v.normal.xyz *_SnowDepth;
			  
		}
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _MainTint;

				//方法①
				//先采样法线，然后装载给输出结构体
				float3 normalMap = UnpackNormal(tex2D(_NormalTex,IN.uv_MainTex));
				o.Normal = normalMap;
				//然后通过WorldNormalVector(IN,o.Normal)将贴上了法线图的模型转化为世界法线
				//WorldNormalVector需要在输入结构体里worldNormal旁写INTERNAL_DATA
				//然后将两个世界向量点乘判断夹角
				//这里也可以直接用模型坐标法线，那么就只会在模型坐标上端产生雪
				if( dot( WorldNormalVector(IN,o.Normal) ,_SnowDir.xyz) >= _SnowLevel)
					o.Albedo = _SnowColor.rgb;
				else
					o.Albedo = c.rgb;
				
				//方法②
				//自己转换（模型法线和法线图相加）到世界坐标
				float3 transNormal = mul(unity_ObjectToWorld ,IN.customNormal + normalMap);
				if(dot(transNormal ,_SnowDir.xyz)>=_SnowLevel)
					o.Albedo = _SnowColor.rgb;
				else
					o.Albedo = c.rgb;
     
			   o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
