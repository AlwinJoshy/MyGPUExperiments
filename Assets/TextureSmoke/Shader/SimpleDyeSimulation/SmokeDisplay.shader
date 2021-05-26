Shader "Unlit/Simple/SmokeDisplay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _ColorTex ("Dencity Map", 2D) = "black" {}
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _ColorTex;
            float4 _MainTex_ST;
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
                fixed value = tex2D(_MainTex, i.uv).r;
                return tex2D(_ColorTex, fixed2(value, 0.1));
               // return step( 0, value);
              //  return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
