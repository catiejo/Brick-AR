Shader "CatieJo/Occlusion Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
    SubShader
    {
        Tags { "Queue"="Overlay" }
 
        Pass
        {
 			ZWrite Off
 			ZTest Always

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            uniform sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            v2f vert(float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, pos);
                o.screenPos = ComputeScreenPos(o.pos);
 				o.uv = uv;
                return o;
            }
 
            fixed4 frag(v2f i) : SV_Target
            {
                //Grab the depth value from the depth texture
                //Linear01Depth restricts this value to [0, 1]
                float cameraDepth = Linear01Depth (tex2Dproj(_CameraDepthTexture,
                                                             UNITY_PROJ_COORD(i.screenPos)).r);

                float goDepth = Linear01Depth(i.pos.z);
                if (abs(goDepth - cameraDepth) > 0.02) {
                	discard;
                }

                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
 
            ENDCG
        }
    }
    FallBack "VertexLit"

}