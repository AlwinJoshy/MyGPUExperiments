Shader "Unlit/CyberGround"
{
    Properties
    {
        _Color ("Color", color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _Spacing ("Spacing", range(0, 10)) = 1
        _Width ("Width", range(0, 1)) = 1
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
                fixed3 wPos : NORMAL1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;
            fixed _Spacing,_Width;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed gridValue = saturate(step(frac(i.wPos.x * _Spacing), _Width) + step(frac(i.wPos.z * _Spacing), _Width)) * saturate(1 - length((i.uv - 0.5) * 2));
                return gridValue * _Color;
            }
            ENDCG
        }
    }
}
