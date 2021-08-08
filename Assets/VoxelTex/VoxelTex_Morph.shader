Shader "VoxelTex/VoxelTex_Morph"
{
    Properties
    {
        _TexA ("Texture A", 2D) = "white" {}
        _TexB ("Texture B", 2D) = "white" {}
        _Morph("Morph Effect", range(0, 1)) = 1
        _ParticleSpread("Spread Scale", range(0, 10)) = 1
        _ParticleExtrude("Spread Extrude", range(0, 10)) = 1
        _yVal ("Y Value", int) = 1
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _TexA, _TexB;
            float4 _TexA_ST, _TexA_TexelSize;
            fixed _ParticleSpread, _ParticleExtrude, _Morph;
            int _yVal;

            v2f vert (appdata v)
            {
                v2f o;
               // v.vertex.y += sin(floor(v.vertex.x + 0.5) + _Time.y);
               fixed trueX = floor(v.vertex.x + 0.5);
                v.vertex.x -= trueX;
                int yStack = floor(trueX/_TexA_TexelSize.z);
                v.vertex.xyz *= _ParticleExtrude;
                fixed4 shiftA = ((tex2Dlod(_TexA, fixed4(((trueX - (yStack * _TexA_TexelSize.z)) + 0.5) * _TexA_TexelSize.x, _TexA_TexelSize.y * (0.5 + yStack + _yVal), 0, 0)) - 0.5) * 2);
                fixed4 shiftB = ((tex2Dlod(_TexB, fixed4(((trueX - (yStack * _TexA_TexelSize.z)) + 0.5) * _TexA_TexelSize.x, _TexA_TexelSize.y * (0.5 + yStack + _yVal), 0, 0)) - 0.5) * 2);
                v.vertex.xyz *= lerp(shiftA.w, shiftB.w, _Morph);
                v.vertex.xyz += lerp(shiftA.xyz, shiftB.xyz, _Morph) * _ParticleSpread;
             //   v.vertex.xyz += shiftSapmle * _ParticleSpread;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
               // fixed4 col = tex2D(_MainTex, i.uv);
                return fixed4(i.normal * 0.5 + 0.5,0);
            }
            ENDCG
        }
    }
}
