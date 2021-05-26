Shader "Unlit/AutoDiserseCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WavePattern("Ring Wave Count", float) = 2
        _WavePower("Wave Power", range(0,1)) = 0.5
        _RingSize ("Size", range(0, 1)) = 0.5
        _RingThickness ("Thickness", range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags {"RenderType"="Opaque + 10"}
        LOD 100
        Lighting Off 
        Cull Off 
      //  ZWrite Off 
        ZTest Always
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _RingThickness, _RingSize, _WavePattern, _WavePower;

            fixed GetValueInRange01(fixed value, fixed start, fixed end){
                fixed delta = end - start;
                fixed sizeFromStart = max(start - value, 0);
                return sizeFromStart/delta;
            }

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
                fixed2 uvNex = (i.uv * 2) - 1;

                fixed slop = uvNex.y/uvNex.x;
                fixed theta = atan2(-uvNex.y, -uvNex.x);
                fixed vectMag = length(uvNex) + sin(theta * _WavePattern) * _WavePower;
                fixed newRingThickness = _RingThickness * GetValueInRange01(i.color.a, 0.8, 1);
                fixed ringSize = step(vectMag - newRingThickness, _RingSize) * step(_RingSize, vectMag + newRingThickness);
                clip(ringSize - 0.001);
                return fixed4(i.color.xyz, 1);
            }
            ENDCG
        }
    }
}
