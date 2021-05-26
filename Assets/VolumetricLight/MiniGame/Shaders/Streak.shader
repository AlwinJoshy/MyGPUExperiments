Shader "Unlit/Streak"
{
    Properties
    {
        _isBending("is Bending", Float) = 0
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dither Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Active" }
        Cull OFF
        LOD 100

        Stencil {
                Ref [_isBending]
                Comp greater
            }

        Pass
        {
            CGPROGRAM
            #pragma target 2
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : color;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
                fixed4 screenPosition : TEXCOORD1;
                fixed lighting : COLOR1;
            };

            sampler2D _MainTex, _DitherPattern;
            float4 _MainTex_ST;
            float4 _DitherPattern_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPosition = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
                o.lighting = (dot(_WorldSpaceLightPos0.xyz, v.normal) * 0.5) + 0.5;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = tex2D(_DitherPattern, ditherCoordinate + fixed2(0.001, -0.001)).r;

                fixed col = tex2D(_MainTex, i.uv).r;
           ///     clip(i.uv.x - 0.5);
                clip((col * 1.5) - (1 - i.color.a) - ditherValue);
                return saturate(fixed4(i.color.rgb * lerp(2, i.lighting, pow(i.color.a, 2)), 1));
            }
            ENDCG
        }
          // shadow casting support
          Pass
        {
            Tags {"LightMode"="ShadowCaster"}
            Cull OFF
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            sampler2D _DitherPattern, _MainTex;
            float4 _DitherPattern_TexelSize;
            float4 _MainTex_ST;

            struct v2f { 
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD0;
                fixed4 screenPosition : TEXCOORD1;
                fixed4 color : COLOR;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.screenPosition = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = tex2D(_DitherPattern, ditherCoordinate + fixed2(0.001, -0.001)).r;

                fixed col = tex2D(_MainTex, i.uv).r;
              //  clip(i.uv.x - 0.5);
                clip((col * 1.5) - (1 - i.color.a) - ditherValue);

                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
