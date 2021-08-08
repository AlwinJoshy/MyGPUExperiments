Shader "VolumetricOcean/OceanCube"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _GlowTex ("Glow Texture", 2D) = "black" {}
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _WaveTexture ("Wave Texture", 2D) = "black" {}
        _SampleStepDist("Sample Step Length", range(0,1)) = 1
        _SampleStrength("Sample Strength", range(0,10)) = 1
        _WaterColor("Water Color", color) = (0,0,0,0)
        _GroundColor("Ground Color", color) = (0,0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        //Blend One One

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
                fixed4 screenPosition : TEXCOORD4;
            };

            sampler2D _MainTex, _GlowTex, _WaveTexture, _DitherPattern;
            fixed4 _DitherPattern_TexelSize;
            float4 _MainTex_ST;
            fixed _SampleStepDist, _SampleStrength;
            fixed3 _WaterColor, _GroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.camDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                o.screenPosition = ComputeScreenPos(o.vertex);
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

            inline fixed WaveHeight(fixed3 pos){
                pos.xz *= 5;
                pos.xz += fixed2(_Time.y, _Time.y * 3);
                return sin(pos.x) + sin(pos.z);
            }

            inline fixed2 GetUV(fixed3 pos){
                fixed3 dir = normalize(pos);
                fixed YValue = dot(dir, fixed3(0,1,0)) * 0.5 + 0.5;
                fixed2 rotVec = normalize(dir.xz);
                fixed flip = step(dot(rotVec, fixed2(0,1)), 0);
                fixed xRange = dot(rotVec, fixed2(1,0));
                //   fixed XValue = (lerp(1 - xRange, xRange, flip) + flip) * 0.5;
                fixed XValue = lerp(xRange, xRange * -1, flip);
                return fixed2( (acos(XValue)/ (3.14 * 2)) + flip * 0.5, YValue);
            }

            inline bool InWater(fixed3 pos){
                //return step(max(1 - pos.x * pos.x - -1, 0) * max(1 - pos.y * pos.y - -1, 0) * max(1 - pos.z * pos.z - -1, 0), 0);
                return step(0.5, max((1 - pos.x) * (pos.x - -1), 0) * max( ((2 + WaveHeight(pos) * 0.05) - pos.y) * (pos.y - 0), 0) * max((1 - pos.z) * (pos.z - -1), 0) * 1000);
            }

            inline fixed3 CausticCalculation(fixed3 pos){
                fixed waterLevel = (2 + WaveHeight(pos) * 0.1);
                fixed3 lightDir = normalize(fixed3(0,1,0.1));
                fixed d = waterLevel - pos.y;
                return tex2D(_WaveTexture, (pos.xz + fixed2(_Time.y, 15)) * 0.3 + lightDir.xz * d)
                    * 0.02 * max(pow(tex2D(_WaveTexture, (pos.xz + fixed2(_Time.y, _Time.x * 3)) * 0.2 + lightDir.xz * d).r, 2) * 3 - d, 0);
            }

            fixed4 GroundSurface(fixed3 pos){
                fixed3 lightDir = normalize(fixed3(0,1,0.1));
                fixed d = (2 + WaveHeight(pos) * 0.1) - pos.y;
                return fixed4( _GroundColor + pow(tex2D(_WaveTexture, (pos.xz + fixed2(_Time.y, 15)) * 0.3 + lightDir.xz * d).rgb * 0.8, 2) * 0.2,
                 step(pos.y, 0.05) * max((1 - pos.x) * (pos.x - -1), 0) * max((1 - pos.z) * (pos.z - -1) * 1000, 0));
            }

            #define STEP_SIZE 0.01;

            fixed4 frag (v2f i) : SV_Target
            {
                bool onSurface = false;
                fixed4 surfaceColor = 0;
                fixed3 glowHazeColor = 0;
                fixed lastMedium = 0; // 0 = air, 1 =
                float val = 0, touchSurface;
                fixed4 waterColor = 0;

                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = (tex2D(_DitherPattern, ditherCoordinate + fixed2(_Time.x, _Time.y)).r - 0.5 ) * 2;
                fixed3 dir = i.camDir * _SampleStepDist;
                fixed3 pos = i.worldPos - (dir * ditherValue);

                for(int n = 0; n < 120; n++)
                {
                    val = InWater(pos);
                    surfaceColor = GroundSurface(pos) * val;
                    waterColor.xyz += val * _WaterColor * 0.1 + val * CausticCalculation(pos);
                    if(surfaceColor.a > 0.1f)break;
                    pos -= dir;
                 //   if(val > 0.1f)break;
                }
                return waterColor + surfaceColor;
                //return val;
            }
            ENDCG
        }
    }
}
/*

Shader "VolumetricOcean/OceanCube"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _GlowTex ("Glow Texture", 2D) = "black" {}
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _WaveTexture ("Wave Texture", 2D) = "black" {}
        _SampleStepDist("Sample Step Length", range(0,1)) = 1
        _SampleStrength("Sample Strength", range(0,10)) = 1
        _WaterColor("Water Color", color) = (0,0,0,0)
        _GroundColor("Ground Color", color) = (0,0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        //Blend One One

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
                fixed4 screenPosition : TEXCOORD4;
            };

            struct WaterDate
            {
                fixed inWater : COLOR1;
                fixed3 dir : NORMAL0;
            };

            sampler2D _MainTex, _GlowTex, _WaveTexture, _DitherPattern;
            fixed4 _DitherPattern_TexelSize;
            float4 _MainTex_ST;
            fixed _SampleStepDist, _SampleStrength;
            fixed3 _WaterColor, _GroundColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.camDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                o.screenPosition = ComputeScreenPos(o.vertex);
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

            inline fixed WaveHeight(fixed3 pos){
                pos.xz *= 5;
                pos.xz += fixed2(_Time.y, _Time.y * 3);
                return sin(pos.x) + sin(pos.z);
            }

            inline fixed2 GetUV(fixed3 pos){
                fixed3 dir = normalize(pos);
                fixed YValue = dot(dir, fixed3(0,1,0)) * 0.5 + 0.5;
                fixed2 rotVec = normalize(dir.xz);
                fixed flip = step(dot(rotVec, fixed2(0,1)), 0);
                fixed xRange = dot(rotVec, fixed2(1,0));
                //   fixed XValue = (lerp(1 - xRange, xRange, flip) + flip) * 0.5;
                fixed XValue = lerp(xRange, xRange * -1, flip);
                return fixed2( (acos(XValue)/ (3.14 * 2)) + flip * 0.5, YValue);
            }

            inline fixed InWater(fixed3 pos){
                return step(0.5, max((1 - pos.x) * (pos.x - -1), 0) * max( ((2 + WaveHeight(pos) * 0.05) - pos.y) * (pos.y - 0), 0) * max((1 - pos.z) * (pos.z - -1), 0) * 10000);
            }
        
            inline WaterDate WaterCalculate(fixed3 pos, fixed3 dir, fixed lastMedium){
                WaterDate o;
                //return step(max(1 - pos.x * pos.x - -1, 0) * max(1 - pos.y * pos.y - -1, 0) * max(1 - pos.z * pos.z - -1, 0), 0);
                //return ;
                o.inWater = InWater(pos);
                fixed3 surfaceNormal = 
                normalize(
                    fixed3(1,0,0) * step(0.9, abs(pos.x)) * sign(pos.x) +
                    fixed3(0,0,1) * step(0.9, abs(pos.z)) * sign(pos.z) +
                    fixed3(0,1,0) * step(1.9 + WaveHeight(pos) * 0.05, abs(pos.y))
                );
                //o.dir = lerp(dir,  refract( dir, surfaceNormal, 0.5), abs(o.inWater - lastMedium)) * 10;
                    o.dir = lerp(dir, -refract( -dir, surfaceNormal, 0.5), abs(o.inWater - lastMedium));
              //  o.dir = surfaceNormal;
                return o;
            }

            inline fixed3 CausticCalculation(fixed3 pos){
                fixed waterLevel = (2 + WaveHeight(pos) * 0.1);
                fixed3 lightDir = normalize(fixed3(0,1,0.1));
                fixed d = waterLevel - pos.y;
                return tex2D(_WaveTexture, (pos.xz + fixed2(_Time.y, 15)) * 0.3 + lightDir.xz * d)
                    * 0.02 * max(pow(tex2D(_WaveTexture, (pos.xz + fixed2(_Time.y, _Time.x * 3)) * 0.2 + lightDir.xz * d).r, 2) * 3 - d, 0);
            }

            fixed4 GroundSurface(fixed3 pos){
                fixed3 lightDir = normalize(fixed3(0,1,0.1));
                fixed d = (2 + WaveHeight(pos) * 0.1) - pos.y;
                return fixed4( _GroundColor + pow(tex2D(_WaveTexture, (pos.xz + fixed2(_Time.y, 15)) * 0.3 + lightDir.xz * d).rgb * 0.8, 2) * 0.2,
                 step(pos.y, 0.05) * max((1 - pos.x) * (pos.x - -1), 0) * max((1 - pos.z) * (pos.z - -1) * 1000, 0));
            }

            #define STEP_SIZE 0.01;

            fixed4 frag (v2f i) : SV_Target
            {
                bool onSurface = false;
                fixed4 surfaceColor = 0;
                fixed3 glowHazeColor = 0;
                fixed lastMedium = 0; // 0 = air, 1 = water
                float val = 0, touchSurface;
                fixed4 waterColor = 0;
                 WaterDate wData;

                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = (tex2D(_DitherPattern, ditherCoordinate + fixed2(_Time.x, _Time.y)).r - 0.5 ) * 2;
                fixed3 dir = i.camDir;
                fixed3 pos = i.worldPos - (dir * ditherValue);

                for(int n = 0; n < 120; n++)
                {
                    wData = WaterCalculate(pos, dir, lastMedium);
                    val = wData.inWater;
                    surfaceColor = GroundSurface(pos) * val;
                    waterColor.xyz += val * _WaterColor * 0.1 + val * CausticCalculation(pos);
                    lastMedium = wData.inWater;
                    dir = wData.dir;
                    pos -= dir * _SampleStepDist;
                  //  if(wData.inWater > 0.1f)break;
                    if(surfaceColor.a > 0.1f)break;
                 //   if(val > 0.1f)break;
                }
                //return fixed4((wData.dir * 0.5) + 0.5, 0);
                return waterColor + surfaceColor;
             //  return fixed4(wData.dir, 1);
                //return val;
            }
            ENDCG
        }
    }
}


*/