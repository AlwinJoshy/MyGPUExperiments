Shader "MyTest/PlaneUV"
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
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 wPos : POSITION1;
                fixed3 vDir : POSITION2;
            };

            struct planeData{
                fixed2 uv : TEXCOORD0;
                fixed facing : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vDir = UnityWorldSpaceViewDir(o.wPos);
                return o;
            }

            planeData PointOnPlane(fixed3 oV, fixed3 dV, fixed3 oP, fixed3 nP, fixed3 x, fixed3 y){

                planeData p;

                fixed3 dirVP = oV - oP;
                fixed perpVP = dot(nP, dirVP);
                
                fixed projVD = dot(dV, -nP);
                fixed minStep = perpVP/projVD;
                fixed3 ptVec = (oV + dV * minStep) - oP;
                p.uv = fixed2(dot(ptVec, x), dot(ptVec, y));
                p.facing = sign(perpVP);
                return p;


            }

            int SquareColor(planeData uvPoint){
                return (1 - step(abs(uvPoint.uv.x), 0.9) * step(abs(uvPoint.uv.y), 0.9)) * step(abs(uvPoint.uv.x), 1) * step(abs(uvPoint.uv.y), 1) * step(0, uvPoint.facing);
            }

            fixed DrawCube(fixed3 vDir){
                fixed col = SquareColor(PointOnPlane(_WorldSpaceCameraPos, normalize(vDir), fixed3(0,1,0), fixed3(0,1,0), fixed3(1,0,0), fixed3(0,0,1)));
                col += SquareColor(PointOnPlane(_WorldSpaceCameraPos, normalize(vDir), fixed3(0,-1,0), fixed3(0,-1,0), fixed3(1,0,0), fixed3(0,0,1)));

                col += SquareColor(PointOnPlane(_WorldSpaceCameraPos, normalize(vDir), fixed3(1,0,0), fixed3(1,0,0), fixed3(0,0,1), fixed3(0,1,0)));
                col += SquareColor(PointOnPlane(_WorldSpaceCameraPos, normalize(vDir), fixed3(-1,0,0), fixed3(-1,0,0), fixed3(0,0,1), fixed3(0,1,0)));

                col += SquareColor(PointOnPlane(_WorldSpaceCameraPos, normalize(vDir), fixed3(0,0,1), fixed3(0,0,1), fixed3(1,0,0), fixed3(0,1,0)));
                col += SquareColor(PointOnPlane(_WorldSpaceCameraPos, normalize(vDir), fixed3(0,0,-1), fixed3(0,0,-1), fixed3(1,0,0), fixed3(0,1,0)));
                return saturate(col);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return DrawCube(-i.vDir);
            }
            ENDCG
        }
    }
}
