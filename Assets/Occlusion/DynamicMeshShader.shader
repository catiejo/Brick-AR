Shader "CatieJo/Dynamic Mesh Shader" 
{ 
	Properties {
		_Alpha ("Alpha", Range (0.0,1.0)) = 0.75
	}
	SubShader 
	{
		Tags { "Queue"="Geometry" }
		CGPROGRAM
		#pragma surface surf Lambert alpha vertex:vert addshadow
		float _Alpha;
		struct Input {
			float3 customColor;
		};
		void vert (inout appdata_full v, out Input o) {
        	UNITY_INITIALIZE_OUTPUT(Input,o);
        	o.customColor = abs(v.normal);
      	}
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.customColor;
			o.Alpha = _Alpha;
		}
		ENDCG
    }
	Fallback "Transparent/Cutout/VertexLit"
 }