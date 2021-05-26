Shader "Util/UnlitWater"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DitherPattern ("Dither Texture", 2D) = "white" {}
        _HighColor ("High Color", Color) = (1,1,1,1)
        _LowColor ("Low Color", Color) = (1,1,1,1)
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _DangerRingColor ("DangerRing Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BoxBlur("Blur", Range(0, 1)) = 0.2
        _RingRadius("Ring Radius", Range(0, 1)) = 0.2
        _RingThickness("Ring Thickness", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+2"}
        ZTest less
        LOD 100

       Stencil {
                Ref 2
                Comp greater
            }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
         #pragma surface surf SimpleLambert exclude_path:prepass noforwardadd vertex:vert
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3
        uniform sampler2D _FoamTex;
        uniform fixed4 _WaveParams;
        sampler2D _MainTex, _DitherPattern;
        fixed4 _Color, _HighColor, _LowColor, _DangerRingColor, _FoamColor;
        float4 _DitherPattern_TexelSize;
        fixed _BoxBlur, _RingThickness, _RingRadius;

        uniform fixed4 _DangerRingArray[50];
        uniform fixed _DangerRingCount = 50;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 screenPosition;
            fixed4 value;
        };

            fixed SquareMag(fixed3 input){
                return input.x * input.x + input.y * input.y + input.z * input.z;
            }
            fixed SquareMag(fixed2 input){
                return input.x * input.x + input.y * input.y;
            }

            void vert (inout appdata_full v, out Input o) {
                UNITY_INITIALIZE_OUTPUT(Input,o);
                fixed3 wVert = mul(unity_ObjectToWorld, v.vertex);
                fixed3 rotOffset = 0;
                rotOffset.y = sin(wVert.z * _WaveParams.z + _Time.y * _WaveParams.x);
                rotOffset.z = cos(wVert.z * _WaveParams.z + _Time.y * _WaveParams.x);
                v.vertex.xyz += mul(unity_WorldToObject, rotOffset * _WaveParams.y);
                o.value.x = rotOffset.y; // height of water
                o.screenPosition = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
                o.value.y = 1;
                for(int i = 0; i < _DangerRingCount; i++){
                  //  o.value.y = step(SquareMag(wVert.xz - _DangerRingArray[i].xz), _DangerRingArray[i].w);
                  //  o.value.yz += (wVert.xz - _DangerRingArray[i].xz) * step(SquareMag(wVert.xz - _DangerRingArray[i].xz), _DangerRingArray[i].w + 5);
                  o.value.y *= saturate(SquareMag(wVert.xz - _DangerRingArray[i].xz));
                //    o.value.w += 0.3 * step(SquareMag(wVert.xz - _DangerRingArray[i].xz), _DangerRingArray[i].w + 1);
                }
                
            }

          half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
              half4 c;
              c.rgb = s.Albedo * _LightColor0.rgb;
              c.a = s.Alpha;
              return c;
          }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed2 screenPos = IN.screenPosition.xy / IN.screenPosition.w;
            fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
            fixed ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;
            fixed seamFoam = 
            tex2D(_FoamTex, screenPos + fixed2(_BoxBlur, 0)).r + 
            tex2D(_FoamTex, screenPos + fixed2(-_BoxBlur, 0)).r +
            tex2D(_FoamTex, screenPos + fixed2(0, _BoxBlur)).r +
            tex2D(_FoamTex, screenPos + fixed2(0, -_BoxBlur)).r +
            tex2D(_FoamTex, screenPos + fixed2(_BoxBlur, _BoxBlur) * 0.707).r + 
            tex2D(_FoamTex, screenPos + fixed2(-_BoxBlur, _BoxBlur) * 0.707).r +
            tex2D(_FoamTex, screenPos + fixed2(-_BoxBlur, -_BoxBlur) * 0.707).r +
            tex2D(_FoamTex, screenPos + fixed2(-_BoxBlur, -_BoxBlur) * 0.707).r +
            tex2D(_FoamTex, screenPos).r;
           // fixed dangerRing = (step(SquareMag(IN.value.yz), IN.value.w) * (1 - step(SquareMag(IN.value.yz), IN.value.w - _RingShift))) * 0.5;
           fixed dangerRing = step(IN.value.y, _RingRadius) * step(_RingRadius - _RingThickness, IN.value.y);
           // 
            o.Albedo = saturate(lerp(0, _DangerRingColor, step(ditherValue, dangerRing)) + (lerp(_LowColor, _HighColor, step(ditherValue, IN.value.x * 0.5 + 0.5)) + lerp(0.000000001, _FoamColor, saturate(step(ditherValue, seamFoam * 0.11112)))));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
