// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dojo/Test//HoneyCrystalShader"
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
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed3 normal : NORMAL;
                fixed3 vDir : NORMAL2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vDir = normalize(UnityWorldSpaceViewDir(worldPos));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                ///fixed4 col = tex2D(_MainTex, i.uv);

                fixed3 shiftedNormal = i.normal + dot(i.vDir, i.normal) * -i.vDir;

                fixed lightAngle = dot(shiftedNormal, _WorldSpaceLightPos0);

                fixed4 col = 0;
                col.xyz = lerp(fixed4(1,0.5,0,1) * 0.1, fixed4(1,0.5,0,1), 1 - ((lightAngle + 1) * 0.5));
                col.xyz += pow(1 - dot(i.vDir, i.normal), 2) * 0.3;
                col.xyz += pow(max(dot(reflect(-normalize(_WorldSpaceLightPos0), i.normal), i.vDir), 0), 10) * 0.8;
                return col;
            }
            ENDCG
        }
    }
}
