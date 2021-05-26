Shader "Lightsaber/LightsaberVolume1"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _Strength("Strength", range(0, 2)) = 0.3
        _Control("Control", range(0, 3)) = 0.3
        _ScanLength("Scan Length", range(0, 0.1)) = 0.3
        _ScanValue("Scan Value", range(0, 3)) = 0.3
        _Cutoff("Cutoff Value", range(0, 0.5)) = 0.3

        _ColorCore("Core Color", color) = (0,0,0,0)
        _ColorOuter("Outer Color", color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Blend One One
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
            };

            struct v2f
            {
                fixed3 camDir : TEXCOORD1;
                fixed4 vertex : SV_POSITION;
                fixed3 worldPos : TEXCOORD2;
                fixed3 cent : POSITION1;
                fixed4 screenPosition : TEXCOORD4;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST, _ColorCore, _ColorOuter;
            sampler2D _DitherPattern;
            fixed4 _DitherPattern_TexelSize;
            fixed _Strength, _Control, _ScanValue, _Cutoff, _ScanLength;

            fixed SqrDist(fixed3 pos){
                return pos.x * pos.x + pos.y * pos.y + pos.z * pos.z;
            }

            fixed SqrDist2D(fixed2 pos){
                return pos.x * pos.x + pos.y * pos.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.cent = mul ( unity_ObjectToWorld, float4(0,0,0,1) ).xyz;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.camDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = (tex2D(_DitherPattern, ditherCoordinate + fixed2(_Time.x, _Time.y)).r - 0.5 ) * 2;
                //   return col;
                fixed col = 0;
                fixed dist = 0;
                fixed3 vecPoint = 0;

                for(int n = 0; n < 32; n++)
                {
                    vecPoint = (i.worldPos + ditherValue * i.camDir * _ScanValue + (n * -i.camDir * _ScanLength)) - i.cent;

                    /*
                    col += (step(SqrDist2D(((vecPoint.xz) + fixed2(sin(vecPoint.y + _Time.x * 100),sin(vecPoint.y + _Time.x * 200)) * 
                    (1 - flameWidth) * 0.02) * 
                    lerp(5, 15, 1 - sin(flameWidth * 1.6 * 1.3))) , 0.2) * 
                    step(0, vecPoint.y)) * 
                    _Strength * 
                    flameWidth;
                    */
                    col += step(SqrDist2D(vecPoint.xz), 0.5) * _Strength;

                }

                clip(col - _Cutoff);

                // sample the texture
                //    fixed4 col = tex2D(_MainTex, i.uv);
                //    return col;
                //  return fixed4(i.cent, 1);
                return tex2D(_MainTex, fixed2(pow(col, _Control), 0.5));
                //    return lerp(_ColorOuter, _ColorCore, pow(col, _Control));
            }
            ENDCG
        }
    }
}
