Shader "Custom/VertexShader"
{
     Properties 
	 {
		  _Intensity("Intensity",float) = 1
     }
     SubShader 
	 {
		  Tags { "RenderType" = "Opaque" }
		  CGPROGRAM
		  #pragma surface surf MyShow vertex:vert

		  #pragma target 3.0

		  struct Input
		  {
			  float3 customNormal;
			  float3 worldNormal;
			  //这就是真的世界法线
		  };
		  float _Intensity;

		  void vert (inout appdata_full v, out Input o)
		  {
			  UNITY_INITIALIZE_OUTPUT(Input , o);
			  		  
			  //o.customNormal = mul(unity_ObjectToWorld,v.normal);
			  //世界空间法线，通过矩阵转化模型坐标至世界坐标

			  //o.customNormal = UnityObjectToWorldDir(v.normal);
			  //世界空间法线，通过函数转化模型坐标至世界坐标

			  o.customNormal = v.normal;
			  //模型空间法线
			 
			  //如果模型有顶点颜色，这里也可以通过v.color来获取到
		  }
		  
		  void surf (Input IN , inout SurfaceOutput o) 
		  {
			  o.Emission = IN.customNormal * 0.5 + 0.5 ;
		  }

		  half4 LightingMyShow(SurfaceOutput o,half3 lightDir,half atten)
		 {
			 fixed4 c;
			 c.rgb = o.Emission;
			 c.a = 1;
			 return c;
		 }

		  ENDCG
    } 
    FallBack "Diffuse"
}
