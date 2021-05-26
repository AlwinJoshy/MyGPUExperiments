Shader "Test/RefractionShader"
{
    Properties
    {
        _RefIndex("Ref Index", range(0, 3)) = 0.5
        _Color("Color", color) = (1,1,1,1)
        _RimGlow("Rim Glow", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        GrabPass { "_RefractionTex" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 vNorm : NORMAL;
                float3 vDir : TEXCOORD2;
                fixed4 grabPos : TEXCOORD3;
                fixed3 wNormal : TEXCOORD4;
            };

            fixed _RefIndex;
            sampler2D _RefractionTex;
            fixed4 _Color, _RimGlow;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.grabPos = ComputeGrabScreenPos(o.vertex);
                
                o.vNorm.xyz = mul(UNITY_MATRIX_MV, v.normal);
                fixed3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.vDir = normalize(UnityWorldSpaceViewDir(wPos));
                o.wNormal = UnityObjectToWorldNormal(v.normal);

            
                // o.normal = v.normal;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = 0;
                col.xyz = i.vNorm;
                i.vNorm.xy /= i.vNorm.z;
                // apply fog
                //  UNITY_APPLY_FOG(i.fogCoord, col);
               // return col;

                fixed3 hVec = normalize(_WorldSpaceLightPos0 + i.vDir);

                half4 bgcolor = tex2Dproj(_RefractionTex, fixed4(i.grabPos.xy + i.vNorm.xy * -_RefIndex, i.grabPos.zw));
                return bgcolor * _Color + _RimGlow * max(1 - dot(i.vDir, i.wNormal), 0) + pow(saturate(dot(hVec, i.wNormal)), 15) * 2;
            }
            ENDCG
        }
    }
}
