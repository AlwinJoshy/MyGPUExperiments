Shader "MiniGame/DeathEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dither Texture", 2D) = "white" {}
        _VisiblityCutColor ("Vinget Color", color) = (1,1,1,1)
        _VingetCtrl("Vinget Control", range(0, 1)) = 0.1
        _VingetSize("Vinget Size", range(1, 10)) = 0.1
        _HitImpactColor ("Hit Impact Color", color) = (1,1,1,1)
        _HitImpactCtrl("Hit Control", range(0, 1)) = 0.1
        _StackingLayer("Stacking Ctrl", range(-10, 10)) = 0.1
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
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 screenPosition : TEXCOORD1;
            };

            sampler2D _MainTex, _DitherPattern;
            fixed _VingetCtrl, _HitImpactCtrl, _StackingLayer, _VingetSize;
            fixed4 _MainTex_ST, _VisiblityCutColor, _DitherPattern_TexelSize, _HitImpactColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                fixed2 centUV = (i.uv * 2) - 1;

                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;

                clip((length(centUV) *_VingetCtrl * _VingetSize) - ditherValue + _StackingLayer);
                return lerp(_VisiblityCutColor, _HitImpactColor, _HitImpactCtrl) * ditherValue;
            }
            ENDCG
        }
    }
}
