Shader "VoxelTex/VoxelTex_Billboard"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ParticleSpread("Spread Scale", range(0, 10)) = 1
        _ParticleExtrude("Spread Extrude", range(0, 10)) = 1
        _ColorResolution("Color Resolution", int) = 1
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
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainTex_TexelSize;
            fixed _ParticleSpread, _ParticleExtrude;
            fixed3 vPos;
            int _ColorResolution;

            fixed3 IntToColor(float value)
            {
                float multiPlier = pow(_ColorResolution, 3) + pow(_ColorResolution, 2) + _ColorResolution;
                int intValue = (value * multiPlier);
                int resX2 = _ColorResolution * _ColorResolution;
                int zValue = intValue / resX2;
                zValue = _ColorResolution - zValue < 0 ? _ColorResolution : zValue;

                int yxValue = intValue - (zValue * resX2);

                int yValue = yxValue / _ColorResolution;
                yValue = _ColorResolution - yValue < 0 ? _ColorResolution : yValue;

                int xValue = yxValue - (yValue * _ColorResolution);

                float r = (float)xValue / (float)_ColorResolution;
                float g = (float)yValue / (float)_ColorResolution;
                float b = (float)zValue / (float)_ColorResolution;

                return fixed3(r, g, b);
            }

            v2f vert (appdata v)
            {
                v2f o;
                // v.vertex.y += sin(floor(v.vertex.x + 0.5) + _Time.y);
                fixed trueX = floor(v.vertex.x + 0.5);
                v.vertex.x -= trueX;
                int yStack = floor(trueX/_MainTex_TexelSize.z);
                v.vertex.xyz *= _ParticleExtrude;
                float4 shiftSapmle = tex2Dlod(_MainTex, float4(
                (
                (trueX - (yStack * _MainTex_TexelSize.z)) + 0.5) * 
                _MainTex_TexelSize.x, _MainTex_TexelSize.y * 
                (0.5 + yStack), 0, 0
                )
                );
                
                vPos.xy += v.vertex.zy;
                v.vertex.xyz = mul(UNITY_MATRIX_T_MV, vPos).xyz;

                v.vertex.xyz += ((shiftSapmle.xyz - 0.5) * 2) * _ParticleSpread;
                //   v.vertex.xyz += shiftSapmle * _ParticleSpread;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color.xyz = IntToColor(shiftSapmle.w);
                o.uv = v.uv;
                return o;
            }

            fixed InOrOutCircle(fixed2 vec)
            {
                return vec.x * vec.x + vec.y * vec.y;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
               // fixed4 col = tex2D(_MainTex, i.uv);
                //return fixed4(i.normal * 0.5 + 0.5,0);
                clip(1 - InOrOutCircle((i.uv - 0.5) * 2));
                return i.color;
            }
            ENDCG
        }
    }
}
