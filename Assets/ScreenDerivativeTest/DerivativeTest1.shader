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
                fixed3 locPos : COLOR0;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                fixed3 tempPos = v.vertex;
                tempPos.y += sin(tempPos.x + _Time.x * 2) * sin(tempPos.z + _Time.x * 2) * 2;
                o.vertex = UnityObjectToClipPos(tempPos);

                o.locPos = tempPos;
               // o.normal = v.normal;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float3 dpdx = ddx(i.locPos);
	            float3 dpdy = ddy(i.locPos);
                fixed3 normal = normalize(cross(dpdy, dpdx));

                return fixed4(normal * 2 - 1, 1);
            }
            ENDCG
        }
    }
}
