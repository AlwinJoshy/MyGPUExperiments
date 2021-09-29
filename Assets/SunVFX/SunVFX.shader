Shader "Alwin_BS/SunVFX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpTex ("Flow Map", 2D) = "bump" {}
        _FlowSpeed("Flow Speed", range(0, 5)) = 1
        _FlowEffect("Flow Effect", range(0, 0.3)) = 0.25
        _GlowColor ("Glow Color", color) = (1,1,1,1)
        _NormalColor ("Normal Color", color) = (1,1,1,1)
        _RimGlowDefinition("Glow spread", range(0, 4)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
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

            sampler2D _MainTex, _BumpTex;
            fixed4 _MainTex_ST, _GlowColor, _NormalColor;
            fixed _RimGlowDefinition, _FlowSpeed, _FlowEffect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vDir = UnityWorldSpaceViewDir(mul((fixed3x3)unity_ObjectToWorld, v.vertex));
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 SunSurface(fixed timeVal, fixed2 shiftVec, fixed2 uv)
            {
                fixed fracTimeA = frac(timeVal);
                fixed fracTimeB = frac(timeVal + 0.5);

                fixed timeA = abs(0.5 - fracTimeA) * 2;
                fixed timeB = abs(0.5 - fracTimeB) * 2;

                return 
                tex2D(_MainTex, uv + shiftVec * _FlowEffect * fracTimeB) * timeA + 
                tex2D(_MainTex, uv + shiftVec * _FlowEffect * fracTimeA) * timeB;
                //return tex2D(_MainTex, uv + shiftVec);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = SunSurface(_Time.y * _FlowSpeed, tex2D(_BumpTex, i.uv + fixed2(_Time.x, 0)).xy * 2 - 1, i.uv);
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
