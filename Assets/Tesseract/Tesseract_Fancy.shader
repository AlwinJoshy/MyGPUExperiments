Shader "MyTest/Tesseract_Fancy"
{
    Properties
    {
        _Color("Color", color) = (1,1,1,1)
        _ColorB("Color B", color) = (1,1,1,1)
        _CubeSize("Cube Size", range(0,1)) = 0.5
        _EadgeThickness("Cube Edge", range(0,1)) = 0.5
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

            fixed4 _Color, _ColorB;
            fixed _CubeSize, _EadgeThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 wPos : POSITION1;
                fixed3 vDir : POSITION2;
                fixed3 normal : NORMAL; 
            };

            struct BoxHitData{
                fixed color : TEXCOORD4;
                fixed3 norl : NORMAL1;
                fixed3 v : POSITION2;
                int f : POSITION3;
            };

            struct planeData{
                fixed2 uv : TEXCOORD0;
                fixed facing : TEXCOORD1;
                fixed color : TEXCOORD2;
                fixed3 norl : NORMAL0;
                fixed3 v : POSITION3;
                fixed3 iF : POSITION4;
            };


            float SqrDistToSphere(fixed3 c, fixed3 o, fixed3 d){

                fixed3 co = c - o;
                fixed coProj = dot(co, d);
                fixed3 distVector = (d * coProj) - co;
                return distVector.x * distVector.x + distVector.y * distVector.y + distVector.z *distVector.z;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vDir = UnityWorldSpaceViewDir(o.wPos);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }



            planeData PointOnPlane(fixed3 oV, fixed3 dV, fixed3 oP, fixed3 nP, fixed3 x, fixed3 y){

                planeData p;
                fixed3 dirVP = oV - oP;
                fixed perpVP = dot(nP, dirVP);
                
                fixed projVD = dot(dV, -nP);
                fixed minStep = perpVP/projVD;
                p.v = oV + dV * minStep * 1.0009;
                fixed3 ptVec = p.v - oP;
                p.uv = fixed2(dot(ptVec, x), dot(ptVec, y));
                p.facing = sign(perpVP);
                int fState = (step(abs(p.uv.x), _CubeSize - _EadgeThickness) * step(abs(p.uv.y), _CubeSize - _EadgeThickness));
                p.iF = fState * step(0, p.facing);
                p.norl = nP;
                p.color = (1 - fState) * step(abs(p.uv.x), _CubeSize) * step(abs(p.uv.y), _CubeSize) * step(0, p.facing);
                return p;
            }

            

            BoxHitData DrawCube(fixed3 vDir, fixed3 eye){

                BoxHitData bD;
                planeData pData;

                pData = PointOnPlane(eye, normalize(vDir), fixed3(0,_CubeSize,0), fixed3(0,1,0), fixed3(1,0,0), fixed3(0,0,1));
                bD.color = pData.color;
                bD.norl = pData.norl * pData.iF;
                bD.v = pData.v * pData.iF;
                bD.f = pData.iF;

                pData = PointOnPlane(eye, normalize(vDir), fixed3(0,-_CubeSize,0), fixed3(0,-1,0), fixed3(1,0,0), fixed3(0,0,1));
                bD.color += pData.color;
                bD.norl += pData.norl * pData.iF;
                bD.v += pData.v * pData.iF;
                bD.f += pData.iF;
                
                pData = PointOnPlane(eye, normalize(vDir), fixed3(_CubeSize,0,0), fixed3(1,0,0), fixed3(0,0,1), fixed3(0,1,0));
                bD.color += pData.color;
                bD.norl += pData.norl * pData.iF;
                bD.v += pData.v * pData.iF;
                bD.f += pData.iF;
                
                pData = PointOnPlane(eye, normalize(vDir), fixed3(-_CubeSize,0,0), fixed3(-1,0,0), fixed3(0,0,1), fixed3(0,1,0));
                bD.color += pData.color;
                bD.norl += pData.norl * pData.iF;
                bD.v += pData.v * pData.iF;
                bD.f += pData.iF;
                
                pData = PointOnPlane(eye, normalize(vDir), fixed3(0,0,_CubeSize), fixed3(0,0,1), fixed3(1,0,0), fixed3(0,1,0));
                bD.color += pData.color;
                bD.norl += pData.norl * pData.iF;
                bD.v += pData.v * pData.iF;
                bD.f += pData.iF;

                pData = PointOnPlane(eye, normalize(vDir), fixed3(0,0,-_CubeSize), fixed3(0,0,-1), fixed3(1,0,0), fixed3(0,1,0));
                bD.color += pData.color;
                bD.norl += pData.norl * pData.iF;
                bD.v += pData.v * pData.iF;
                bD.f += pData.iF;

                return bD;
            }

            int Al_Step(fixed a, fixed b){
                return a-b == 0 ? -1 : 1;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed3 s = i.wPos, o = fixed3(0,0,0), nml = fixed3(0,0,0), vPos = fixed3(0,0,0);
                fixed3 vVec = -i.vDir;
                fixed4 col = 0;
                BoxHitData bD;

                for(int n = 0; n < 25; n++){
                    //   col += pow(saturate(step(SqrDistToSphere(fixed3(0,0,0), s, normalize(i.vDir)) - 0.05, 0) * (1 - (0.05 * n))), 3);

                    bD = DrawCube(vVec, s);
                    float fadeValue = 1 - (0.03 * n);
                    col += lerp(_ColorB, _Color, fadeValue) * pow(saturate(bD.color * fadeValue), 2);
                    nml = bD.norl;
                    vPos = bD.v;
                    
                    
                    float wallDist_X = o.x - s.x;
                    float wallDist_Y = o.y - s.y;
                    float wallDist_Z = o.z - s.z;


                    float rayFactX = vVec.x;
                    float rayFactY = vVec.y;
                    float rayFactZ = vVec.z;

                    float closestWallX = wallDist_X + 0.5f * sign(rayFactX);
                    float closestWallY = wallDist_Y + 0.5f * sign(rayFactY);
                    float closestWallZ = wallDist_Z + 0.5f * sign(rayFactZ);

                    float xStepFac = closestWallX / rayFactX;
                    float yStepFac = closestWallY / rayFactY;
                    float zStepFac = closestWallZ / rayFactZ;

                    float shortestStep = xStepFac < yStepFac ? xStepFac < zStepFac ? xStepFac : zStepFac < yStepFac ? zStepFac : yStepFac : yStepFac < zStepFac ? yStepFac : zStepFac < xStepFac ? zStepFac : xStepFac;

                    
                    
                    fixed3 hitPoint = vVec * shortestStep + s;
                    s = fixed3(
                    hitPoint.x * Al_Step(shortestStep, xStepFac), 
                    hitPoint.y * Al_Step(shortestStep, yStepFac), 
                    hitPoint.z * Al_Step(shortestStep, zStepFac)) * (1 - bD.f) + vPos * bD.f;
                    
                    vVec = vVec * (1 - bD.f) + reflect(vVec, nml) * bD.f;
                }

                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(i.vDir, i.normal));
                // decode cubemap data into actual color
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);

                col.xyz += pow(skyColor, 3);
                //  return col * fixed4(0,0.5,0,1);
                //    return col;
                //return bD.f;
                return col;
            }
            ENDCG
        }
    }
}
