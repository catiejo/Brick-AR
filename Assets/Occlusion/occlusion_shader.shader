Shader "CatieJo/occlusion_shader"
{
//    SubShader
//    {
//        Tags { "RenderType"="Opaque" }
//
//        Pass
//        {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//            };
//
//            struct v2f
//            {
//                float4 vertex : SV_POSITION;
//            };
//
//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                return o;
//            }
//            
//            fixed4 frag (v2f i) : SV_Target
//            {
//            	float depth = 1 - i.vertex.z;
//                return float4(depth, depth, depth, 1);
//            }
//            ENDCG
//        }
//    }
//    SubShader
//    {
//        Tags { "RenderType"="Opaque" }
// 
//        Pass
//        {
// 
//            CGPROGRAM
//            #pragma target 3.0
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
// 
//            uniform sampler2D _CameraDepthTexture; //the depth texture
// 
//            struct v2f
//            {
//                float4 pos : SV_POSITION;
//                float4 projPos : TEXCOORD1; //Screen position of pos
//            };
// 
//            v2f vert(appdata_base v)
//            {
//                v2f o;
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                o.projPos = ComputeScreenPos(o.pos);
// 
//                return o;
//            }
// 
//            half4 frag(v2f i) : COLOR
//            {
//                //Grab the depth value from the depth texture
//                //Linear01Depth restricts this value to [0, 1]
//                float depth = Linear01Depth (tex2Dproj(_CameraDepthTexture,
//                                                             UNITY_PROJ_COORD(i.projPos)).r);
// 
//                half4 c;
//                c.r = depth;
//                c.g = 1 - depth;
//                c.b = depth;
//                c.a = 1;
// 
//                return c;
//            }
// 
//            ENDCG
//        }
//    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
 
        Pass
        {
 			ZWrite Off
 			ZTest Always

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            uniform sampler2D _CameraDepthTexture; //the depth texture
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD1; //Screen position of pos
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
 
                return o;
            }
 
            half4 frag(v2f i) : COLOR
            {
                //Grab the depth value from the depth texture
                //Linear01Depth restricts this value to [0, 1]
                float depth = Linear01Depth (tex2Dproj(_CameraDepthTexture,
                                                             UNITY_PROJ_COORD(i.projPos)).r);

                float localDepth = Linear01Depth(i.pos.z);
                if (abs(localDepth - depth) > 0.01) {
                	discard;
                }
            	half4 c;
                c.r = depth;
                c.g = 1 - depth;
                c.b = depth;
                c.a = 1;
 
                return c;

            }
 
            ENDCG
        }
    }
    FallBack "VertexLit"

}