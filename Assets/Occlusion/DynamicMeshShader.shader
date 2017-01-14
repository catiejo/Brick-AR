Shader "CatieJo/Dynamic Mesh Shader" 
{ 
	 SubShader 
	 {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		CGPROGRAM
		#pragma surface surf Lambert alpha vertex:vert addshadow
		struct Input {
			float3 customColor;
		};
		void vert (inout appdata_full v, out Input o) {
          UNITY_INITIALIZE_OUTPUT(Input,o);
          o.customColor = abs(v.normal);
      	}
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.customColor;
			o.Alpha = 0.75;
		}
		ENDCG
    }
	Fallback "Transparent/Cutout/VertexLit"
 }