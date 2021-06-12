Shader "MyTest/PlaneTest"
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

            fixed3 PointOnPlane(fixed3 oV, fixed3 dV, fixed3 oP, fixed3 nP){

                fixed3 dirVP = oV - oP;
                fixed perpVP = dot(nP, dirVP);
  
                fixed projVD = dot(dV, -nP);
                fixed minStep = perpVP/projVD;
                return oV + dV * minStep;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 1;

                fixed3 colissionPoint = PointOnPlane(_WorldSpaceCameraPos, normalize(-i.vDir), fixed3(0,0,0), normalize(fixed3(0,1,0)));


                return length(colissionPoint) * 0.1;
            }
            ENDCG
        }
    }
}
