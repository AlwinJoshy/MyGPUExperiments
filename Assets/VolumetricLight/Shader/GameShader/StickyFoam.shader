// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MiniGame/Ship/StickyFoam"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FoamSize ("Foam Size", range(0.01, 20)) = 1
        _ClipThrushold ("Thrudhold", range(0.01, 1)) = 0.5
        _GradientEffect ("Gradient Effect", range(0.01, 2)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="WaterEffect" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest

            uniform fixed4 _WaveParams;
            sampler2D _MainTex;
            fixed _FoamSize, _ClipThrushold, _GradientEffect;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                fixed3 wPos : POSITION1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                fixed3 wVert = mul(unity_ObjectToWorld, v.vertex);
                fixed3 rotOffset = 0;
                wVert = _WaveParams.w;
                rotOffset.y += sin(wVert.z * _WaveParams.z + _Time.y * _WaveParams.x);
                rotOffset.z = cos(wVert.z * _WaveParams.z + _Time.y * _WaveParams.x);
                v.vertex.xyz += mul(unity_WorldToObject, rotOffset * _WaveParams.y);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed isFormey = step(0.5, i.color.g);
                fixed foamHole = tex2D(_MainTex, i.wPos.xz * _FoamSize + fixed2(_Time.x, -_Time.x * 2)) * tex2D(_MainTex, (i.wPos.xz * _FoamSize) + fixed2(0.1 - _Time.x, 0.1 + _Time.x * 2));
                clip(_ClipThrushold - foamHole * (1 - i.color.r * _GradientEffect));
                return i.color;
            }
            ENDCG
        }
    }
}
