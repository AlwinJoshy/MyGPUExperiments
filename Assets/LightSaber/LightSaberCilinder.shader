Shader "LightSaber/LightSaberCilinder"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend One One
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                
                //    float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 viewDir : TEXCOORD1;
                fixed3 centerPoint : TEXCOORD2;
                fixed3 upVector : TEXCOORD3;
                //  float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                fixed3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = -normalize(UnityWorldSpaceViewDir(wPos));
                o.centerPoint = mul(unity_ObjectToWorld, fixed4(0,0,0,1));
                o.upVector = UnityObjectToWorldDir(fixed4(0,1,0,1));
                //    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);

                fixed col = 0;
                
                //   fixed vecShadow = dot(i.distToCentre, i.viewDir);
                //     fixed3 perpPoint = vecShadow * i.viewDir;

                fixed3 nVec = cross(i.upVector, i.viewDir);
                fixed3 nVec2 = cross(i.viewDir, nVec);

                float distance = abs(dot(nVec, i.centerPoint - _WorldSpaceCameraPos)/ length(nVec));

                fixed3 p1 = i.centerPoint + (dot(_WorldSpaceCameraPos - i.centerPoint, nVec2)/ dot(i.upVector, nVec2)) * i.upVector;


                return tex2D(_MainTex , fixed2( saturate((1 - distance * 3)), 0.5)) * saturate(p1.y - 4);
            }
            ENDCG
        }
    }
}
