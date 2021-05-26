Shader "Unlit/ScreenTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", color) = (1,1,1,1)
        _CheckerShut("Checker Shut", range(0, 1)) = 0
        _DiamondCover("Diamond Creap", range(0, 1)) = 0
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;
            fixed _CheckerShut, _DiamondCover;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed CkeckerCover(fixed2 uvCorrectAspect, fixed value, fixed size){
                 fixed2 scaledUV = uvCorrectAspect * size;
                fixed2 unShifted = frac(scaledUV);

                 float yShift = ceil(scaledUV.y) % 2;
                float mod = (ceil(scaledUV.x) + yShift) % 2 * step(unShifted.x, (value * 2));
                mod += (ceil(scaledUV.x + 1) + yShift) % 2 * step(1 - unShifted.x, (value - 0.5) * 2);
              // * step(unShifted.x, _ScreenCover)

                return step(0.5, mod);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect = _ScreenParams.x /_ScreenParams.y;
                fixed2 uvCorrectAspect = i.uv * fixed2(aspect, 1);
                fixed2 scaledUV = uvCorrectAspect * 10;
                fixed2 unShifted = frac(scaledUV);
                fixed2 newUV = (unShifted - 0.5) * 2;
               // fixed geometryShape = step(length(newUV), (1 - (i.uv.x + _ScreenCover) * 0.5) * 2);
             //  fixed geometryShape = step(abs(newUV.x) + abs(newUV.y), (1 - (i.uv.x + _ScreenCover) * 0.5) * 2);
                

           //     fixed geometryShape = step(abs(newUV.x) + abs(newUV.y), (1 - (i.uv.y + _ScreenCover) * 0.5) * 2);
          
              // * step(unShifted.x, _ScreenCover)

            //    fixed geometryShape = step(0.5, mod) ;

                fixed geometryShape = step(abs(newUV.x) + abs(newUV.y), (2 - (i.uv.y + (1 - _DiamondCover) * 2)) * 2);

            //    clip(step(length(newUV), (1 - (i.uv.x + _ScreenCover) * 0.5) * 2) - 0.01);
                clip(geometryShape - 0.001);    
                return _Color * CkeckerCover(uvCorrectAspect, 1 - _CheckerShut, 20);
                /*
                fixed2 uvCorrectAspect2 = fixed2( 1 -i.uv.x, i.uv.y) * fixed2(aspect, 1);
                fixed2 newUV2 = (frac(uvCorrectAspect2 * 10) - 0.5) * 2;
                float seconColorArea = step(length(newUV2), (1 - (1 - i.uv.x + _ScreenCover2) * 0.5) * 2);
                return lerp(_Color, 0, seconColorArea);
                */
            }
            ENDCG
        }
    }
}
