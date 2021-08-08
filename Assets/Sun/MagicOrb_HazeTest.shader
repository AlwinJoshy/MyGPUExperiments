Shader "MagicOrb/MagicOrb_Haze"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowTex ("Glow Texture", 2D) = "white" {}
        _SwaySample ("SwaySample", 2D) = "bump" {}
        _CentralSphereSize("Sphere Size", range(0,2)) = 1
        _SampleStepDist("Sample Step Length", range(0,1)) = 1
        _SampleStrength("Sample Strength", range(0,1)) = 1
        _ExtraLength("Extra Length", range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend One One

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

            sampler2D _MainTex, _GlowTex, _SwaySample;
            float4 _MainTex_ST;
            fixed _CentralSphereSize, _SampleStepDist, _SampleStrength, _ExtraLength;

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

            #define STEP_SIZE 0.01;

            fixed4 frag (v2f i) : SV_Target
            {
                bool onSurface = false;
                fixed3 surfaceColor = 0;
                fixed3 glowHazeColor = 0;

                fixed3 pos;
                for(int n = 0; n < 256; n++)
                {
                    pos = i.worldPos - (i.camDir * _SampleStepDist * (n + 1));
                    fixed sqrDist = SqrMag(pos);
                    onSurface = step(sqrDist, _CentralSphereSize) > 0;
                    fixed3 sample = tex2D(_MainTex, GetUV(pos));
                    fixed lengthVar = SqrMag(sample)/3;
                    surfaceColor = sample * onSurface;
                    glowHazeColor += pow(tex2D(_GlowTex, GetUV(pos)) * (1 - onSurface) * _SampleStrength * saturate(0.1 + (lengthVar * _ExtraLength) - max(sqrDist - _CentralSphereSize,0)), 2);
                    if(onSurface == 1)break;
                }
                return fixed4(surfaceColor + glowHazeColor,0);
            }
            ENDCG
        }
    }
}
