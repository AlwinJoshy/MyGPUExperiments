Shader "Unlit/DerivativeTest1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;

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
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed2 newUV = i.uv * 10;
                fixed height = sin(newUV.x) * sin(newUV.y);

                float dpdx = ddx(height) * 10;
                float dpdy = ddy(height) * 10;

                return fixed4(dpdx, 0, dpdy, 0);

                //fixed3 normal = normalize(cross(dpdy, dpdx));

                // return fixed4(normal * 2 - 1, 1);
            }
            ENDCG
        }
    }
}
