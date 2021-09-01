Shader "Alwin_BS/SunFlowFX"
{
    Properties
    {
        _Color ("Color", color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _TexScale("Scale", range(0, 10)) = 0.2
        _FlowTex ("Flow Texture", 2D) = "white" {}
        _FlowEffect("Flow Effect", range(0, 1)) = 0.2
        _FlowSpeed("Flow Speed", range(0, 10)) = 0.2
        

        _SpreadFade("Spread Fade", range(0, 5)) = 0.2
        _ScrollSpeed("Move Speed", range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100
        Blend One One
        ZWrite OFF

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
                fixed4 color : Color;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : Color;
            };

            sampler2D _MainTex, _FlowTex;
            float4 _MainTex_ST, _Color;
            fixed _FlowEffect, _FlowSpeed, _SpreadFade, _StartSize, _TexScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float3 FlowUVW (float2 uv, float2 flowVector, float time, bool flowB, fixed offset) {
                float phaseOffset = flowB ? 0.5 : 0;
                float progress = frac(time * _FlowSpeed + phaseOffset + offset);
                float3 uvw;
                uvw.xy = uv - flowVector * progress * _FlowEffect;
                uvw.z = 1 - abs(1 - 2 * progress);
                return uvw;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed spreadFade = saturate(1 - (length((i.uv - 0.5) * _SpreadFade) - _StartSize));
                fixed2 flowVal = tex2D(_FlowTex, i.uv).rg * 2 - 1;
                fixed timeOffset = i.color.r;
                float3 uvwA = FlowUVW(i.uv, flowVal, _Time.x, false, timeOffset);
                float3 uvwB = FlowUVW(i.uv, flowVal, _Time.x, true, timeOffset);

                fixed4 texA = tex2D(_MainTex, uvwA.xy * _TexScale) * uvwA.z;
                fixed4 texB = tex2D(_MainTex, uvwB.xy * _TexScale) * uvwB.z;
                fixed flip = 1 - spreadFade;
                return saturate((texA + texB) - fixed4(flip, flip, flip, 0)) * _Color * 2 * i.color.a;
            }
            ENDCG
        }
    }
}
