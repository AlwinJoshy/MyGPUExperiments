Shader "Alwin_BS/SunVFX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpTex ("Bump", 2D) = "bump" {}
        _GlowColor ("Glow Color", color) = (1,1,1,1)
        _NormalColor ("Normal Color", color) = (1,1,1,1)
        _RimGlowDefinition("Glow spread", range(0, 4)) = 0.5
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
                fixed4 vertex : POSITION;
                fixed3 normal : NORMAL;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed3 normal : NORMAL;
                fixed3 vDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST, _GlowColor, _NormalColor;
            fixed _RimGlowDefinition;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vDir = UnityWorldSpaceViewDir(mul((fixed3x3)unity_ObjectToWorld, v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 vDirNormalized = normalize(i.vDir);
                fixed emission = length(col.xyz);
                fixed rimEffect = pow(saturate(1 - dot(i.normal, vDirNormalized)), _RimGlowDefinition);
                fixed4 rimGlowColor = lerp(_NormalColor, _GlowColor, rimEffect);
                return (col * emission) + rimGlowColor;
                return lerp(_NormalColor, _GlowColor, pow(saturate(1 - dot(i.normal, vDirNormalized)), _RimGlowDefinition));
            }
            ENDCG
        }
    }
}
