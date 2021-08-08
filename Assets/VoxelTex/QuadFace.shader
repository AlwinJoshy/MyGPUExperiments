Shader "VoxelTex/QuadFaceQuadFace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed3 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed3 vPos;

            v2f vert (appdata v)
            {
                v2f o;
                vPos.xy += v.vertex.xy;
                o.vertex = mul(UNITY_MATRIX_T_MV, vPos);
                o.vertex = UnityObjectToClipPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            /*

                            v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                fixed3 wPos = mul(UNITY_MATRIX_M, v.vertex);
                fixed4 worldCoord = float4(
                    unity_ObjectToWorld._m03, 
                    unity_ObjectToWorld._m13, 
                    unity_ObjectToWorld._m23, 
                    1);
                fixed4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(wPos, 0);
                o.vertex = mul(UNITY_MATRIX_P, viewPos);
                
              //  o.color = mul(UNITY_MATRIX_T_MV, fixed4(viewPos, 1));
                //o.color = viewPos;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;

            */

            fixed4 frag (v2f i) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
