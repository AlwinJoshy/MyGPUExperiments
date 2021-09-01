Shader "Alwin_BS/SunHaze"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StartSize("Start Range", range(0, 1)) = 0.2
        _SpreadFade("Spread Fade", range(0, 5)) = 0.2
        _ScrollSpeed("Move Speed", range(0, 1)) = 0.2
        _ShiftSpeed("Color Shift Speed", range(0, 1)) = 0.2
        _Color("Color", color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend OneMinusDstColor One // Soft Additive
        ZWrite OFF

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed4 color : COLOR;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST, _Color;
            fixed _StartSize, _SpreadFade, _ScrollSpeed, _ShiftSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed spreadFade = saturate(1 - (length((i.uv - 0.5) * _SpreadFade) - _StartSize));
                return _Color * pow(saturate(spreadFade - tex2D(_MainTex, i.uv + fixed2((_Time.x * _ScrollSpeed) + (i.color.r * _ShiftSpeed), (-_Time.x * _ScrollSpeed) + (i.color.g * _ShiftSpeed)))), 2) * i.color.a;
            }
            ENDCG
        }
    }
}
