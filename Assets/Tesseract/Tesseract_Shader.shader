Shader "MyTest/Tesseract_Shader"
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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed col = 0;



                /*
                for(int i=0; i<36; i++)
                {

                }
                */
                col += step(SqrDistToSphere(fixed3(0,0,0), i.wPos, normalize(i.vDir)) - 0.1, 0);
                return col;
            }
            ENDCG
        }
    }
}
