Shader "MagicOrb/MagicOrb_1"
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
                fixed3 camDir : TEXCOORD1;
                fixed4 vertex : SV_POSITION;
                fixed3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.camDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }

            struct SphereSurface{
                fixed isSurface : TEXCOORD1;
                fixed3 sNormal : NORMAL;
            };

            inline fixed SqrMag(fixed3 val){
                return val.x * val.x + val.y * val.y + val.z * val.z;
            }

            SphereSurface CalculateSphereSurface(fixed3 pos)
            {
                SphereSurface o;
                fixed3 ptVec = pos - fixed3(0,0,0);
                o.isSurface = step(SqrMag(ptVec), 0.25);
                o.sNormal = normalize(ptVec);
                return o;
            } 

            fixed2 GetUV(fixed3 pos){
                fixed3 dir = normalize(pos);
                fixed YValue = dot(dir, fixed3(0,1,0)) * 0.5 + 0.5;
                fixed2 rotVec = normalize(dir.xz);
                fixed flip = step(dot(rotVec, fixed2(0,1)), 0);
                fixed xRange = dot(rotVec, fixed2(1,0));
             //   fixed XValue = (lerp(1 - xRange, xRange, flip) + flip) * 0.5;
                fixed XValue = lerp(xRange, xRange * -1, flip);
                return fixed2( (acos(XValue)/ (3.14 * 2)) + flip * 0.5, YValue);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed effect = 0;
              //  fixed3 
                fixed3 pos;
                for(int n = 0; n < 256; n++)
                {
                    pos = i.worldPos - (i.camDir * 0.01 * (n + 1));
                    effect += step(SqrMag(pos), 0.1);
                    if(effect > 0.5)break;
                }
                return tex2D(_MainTex, GetUV(pos)) * effect;
               // return GetUV(pos).x/(3.14 * 0.5);
            }
            ENDCG
        }
    }
}
