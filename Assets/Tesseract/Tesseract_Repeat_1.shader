Shader "MyTest/Tesseract_Repeat_1"
{
    Properties
    {

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
                return o;
            }

            int Al_Step(fixed a, fixed b){
                return a-b == 0 ? -1 : 1;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed3 s = i.wPos, o = fixed3(0,0,0);
                i.vDir *= -1;
                fixed col = 0;

                for(int n = 0; n < 256; n++){
                    col += pow(saturate(step(SqrDistToSphere(fixed3(0,0,0), s, normalize(i.vDir)) - 0.05, 0) * (1 - (0.05 * n))), 3);

                    float wallDist_X = o.x - s.x;
                    float wallDist_Y = o.y - s.y;
                    float wallDist_Z = o.z - s.z;


                    float rayFactX = i.vDir.x;
                    float rayFactY = i.vDir.y;
                    float rayFactZ = i.vDir.z;

                    float closestWallX = wallDist_X + 0.5f * sign(rayFactX);
                    float closestWallY = wallDist_Y + 0.5f * sign(rayFactY);
                    float closestWallZ = wallDist_Z + 0.5f * sign(rayFactZ);

                    float xStepFac = closestWallX / rayFactX;
                    float yStepFac = closestWallY / rayFactY;
                    float zStepFac = closestWallZ / rayFactZ;

                    float shortestStep = xStepFac < yStepFac ? xStepFac < zStepFac ? xStepFac : zStepFac < yStepFac ? zStepFac : yStepFac : yStepFac < zStepFac ? yStepFac : zStepFac < xStepFac ? zStepFac : xStepFac;

                    //   Debug.Log(xStepFac + " " + yStepFac + " " + zStepFac + " " + shortestStep);
                    
                    fixed3 hitPoint = i.vDir * shortestStep + s;
                    s = fixed3(
                    hitPoint.x * Al_Step(shortestStep, xStepFac), 
                    hitPoint.y * Al_Step(shortestStep, yStepFac), 
                    hitPoint.z * Al_Step(shortestStep, zStepFac));
                }
                return col * fixed4(0,0.5,0,1);
            }
            ENDCG
        }
    }
}
