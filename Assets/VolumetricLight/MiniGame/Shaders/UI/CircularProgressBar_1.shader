Shader "MiniGame/Unlit/CircularProgressBar_1"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        _RingRadius("Radius", float) = 0.1
        _RingThickness("Radius", range(0,5)) = 0.1
    }
    SubShader
    {
        Tags {   "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        "CanUseSpriteAtlas"="True"}
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;
            fixed _RingRadius, _RingThickness;

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                fixed2 shiftedUV = (i.uv * 2) - 1;
                fixed lineLength = length(shiftedUV);

                fixed circleArea = step(_RingRadius, lineLength + _RingThickness * 0.5) * step(lineLength - _RingThickness * 0.5, _RingRadius);
                // step(lineLength + _RingThickness * 0.5, _RingRadius)


                fixed slop = shiftedUV.y/shiftedUV.x;
                fixed theta = (atan2(-shiftedUV.y, -shiftedUV.x)/(3.14) * 0.5) + 0.5;
                clip(circleArea * step(theta, i.color.a) - 0.1);

                return fixed4(i.color.rgb, 1);
            }
            ENDCG
        }
    }
}
