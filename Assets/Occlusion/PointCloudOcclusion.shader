Shader "CatieJo/PointCloudOcclusion" {
    SubShader {
        Tags { "Queue" = "Overlay" }
        Pass {
//            ColorMask 0
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
               float4 vertex : POSITION;
            };
       
            struct v2f
            {
               float4 vertex : SV_POSITION;
               float size : PSIZE;
            };
           
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.size = 30;
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
            	float depth = Linear01Depth(i.vertex.z);
                return fixed4(i.vertex.z, 1, 1, 1);
	
            }
            ENDCG
        }
    }
}
