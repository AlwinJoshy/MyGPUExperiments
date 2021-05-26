Shader "Unlit/FontEnergy"
{
    Properties
    {
        _Color ("Color", color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _HeatSpeed("Speed", range(0, 3)) = 1
        _Scale("Scale", vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend One One
        Cull OFF 

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _Scale, _Color;
            fixed _HeatSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, fixed2(i.uv.x * _Scale.x, i.uv.y * _Scale.y) - fixed2(0, _Time.z * _HeatSpeed)).r * (1 - i.uv.y);
                return pow(col, 2) * 2 * _Color;
            }
            ENDCG
        }
    }
}
