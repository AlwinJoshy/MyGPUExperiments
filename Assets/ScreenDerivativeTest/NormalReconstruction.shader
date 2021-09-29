Shader "Unlit/NormalReconstruction"
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
                fixed heightValue : COLOR0;
                fixed3 vert : COLOR1;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;

            fixed CalculateHeight(fixed3 pos){
                return sin(pos.x + _Time.x * 2) * sin(pos.z + _Time.x * 2);
            }

            v2f vert (appdata v)
            {
                v2f o;
                fixed3 tempPos = v.vertex;
                o.heightValue = CalculateHeight(tempPos);
                tempPos.y += o.heightValue;
                o.vertex = UnityObjectToClipPos(tempPos);

                o.vert = v.vertex;
                
                // o.normal = v.normal;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed3 CalculateNormal(fixed3 pos){
                fixed res = 0.05;
                fixed ctrl = 3;
                fixed dX = (CalculateHeight(pos) - CalculateHeight(pos + fixed3(res,0,0))) * ctrl;
                fixed dZ = (CalculateHeight(pos) - CalculateHeight(pos + fixed3(0,0,res))) * ctrl;
                return normalize(fixed3(dX, ((0.25 - abs(dX)) + (0.25 - abs(dZ))) * 0.5, dZ));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed3 newVert = i.vert;
                newVert.y = i.heightValue;
                float dxVal = ddx(i.heightValue);
                float dyVal = ddy(i.heightValue);
                float3 dpdx = ddx(newVert);
                float3 dpdy = ddy(newVert);
                fixed3 normal = normalize(cross(dpdy, dpdx));

                return dot(CalculateNormal(i.vert), normalize(fixed3(0.5, 1, 0.1)));
            }
            ENDCG
        }
    }
}
