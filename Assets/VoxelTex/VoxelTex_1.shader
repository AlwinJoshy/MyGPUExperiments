Shader "VoxelTex/VoxelTex_1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainTex_TexelSize;
            fixed _ParticleSpread, _ParticleExtrude;
            int _yVal;

            v2f vert (appdata v)
            {
                v2f o;
               // v.vertex.y += sin(floor(v.vertex.x + 0.5) + _Time.y);
               fixed trueX = floor(v.vertex.x + 0.5);
                v.vertex.x -= trueX;
                int yStack = floor(trueX/_MainTex_TexelSize.z);
                v.vertex.xyz *= _ParticleExtrude;
                fixed3 shiftSapmle = tex2Dlod(_MainTex, fixed4(((trueX - (yStack * _MainTex_TexelSize.z)) + 0.5) * _MainTex_TexelSize.x, _MainTex_TexelSize.y * (0.5 + yStack + _yVal), 0, 0));
                v.vertex.xyz += ((shiftSapmle - 0.5) * 2) * _ParticleSpread;
                
             //   v.vertex.xyz += shiftSapmle * _ParticleSpread;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return fixed4(i.normal * 0.5 + 0.5,0);
            }
            ENDCG
        }
    }
}
