Shader "LightSaber/LightSaberTest"
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
                fixed3 distToCentre : TEXCOORD2;
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
                o.distToCentre = -UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, fixed4(0,0,0,1)));
                //    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);

                fixed col = 0;
                
                fixed vecShadow = dot(i.distToCentre, i.viewDir);
                fixed3 perpPoint = vecShadow * i.viewDir;


                return tex2D(_MainTex , fixed2( saturate(1 - length(i.distToCentre - perpPoint) * 2), 0.5));
            }
            ENDCG
        }
    }
}
