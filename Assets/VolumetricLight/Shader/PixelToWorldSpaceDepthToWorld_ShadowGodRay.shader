// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PixelToWorldSpace/DepthShadow&GodRay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dither Texture", 2D) = "white" {}
        _BlueNoice ("Blue Noice Texture", 2D) = "white" {}
        _DitherCtrl("Dither Strength", range(0,2)) = 0
        _CorrectionOffset("Correction Offset", range(0,0.1)) = 0
        _ShadowColor("Shadow Color", color) = (1,1,1,1)
        _FogCtrl("Fog Strength", range(0,1)) = 0
        _FogColor("Fog Color", color) = (1,1,1,1)

        _TestCtrl("Test Control", range(-5,20)) = 0
    }

     CGINCLUDE
    #pragma vertex vert
    #pragma fragment frag

    #include "UnityCG.cginc"

    #define STRIDE_LENGTH 3

      struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

               struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 screenPosition : TEXCOORD1;
            };

             uniform fixed4x4 SHADOW_CAMERA_MATRIX;
            uniform sampler2D SHADOW_CAMERA_DEPTH;
            uniform fixed4 SHADOW_CAMERA_PROJPARAMS, SHADOW_CAMERA_FORWARD, SHADOW_CAMERA_POSITION,_ShadowColor;

            
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture, _DitherPattern, _BlueNoice;
            float4 _MainTex_ST;

          v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

    ENDCG

    SubShader
    {

        Pass
        {
            Tags { "RenderType"="Opaque" }
            LOD 100
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            fixed _CorrectionOffset, _DitherCtrl;
            float4 _DitherPattern_TexelSize;

          
            float OrthoZ2EyeDepth(float z) {
                return (abs(SHADOW_CAMERA_PROJPARAMS.z) - abs(SHADOW_CAMERA_PROJPARAMS.y)) * z + abs(SHADOW_CAMERA_PROJPARAMS.y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv).r;

                fixed3 camOrigine = mul(unity_CameraToWorld, fixed4(0,0,0,1)).xyz;
                fixed4 dirVector = mul(unity_CameraInvProjection, fixed4(i.uv * 2 - 1, 0, 1));
              
                dirVector.z *= -1;
                dirVector.xyz = mul(unity_CameraToWorld, dirVector.xyz).xyz;

                // Z depth in units
                fixed vectorScale = LinearEyeDepth(depth);
                // world position
                fixed3 wPos = (dirVector.xyz * vectorScale) + camOrigine;

                clip(50 - vectorScale);

                // transform world position to shadow camera position
                fixed4 cameraPos = mul(SHADOW_CAMERA_MATRIX, fixed4(wPos, 1));

                // applaying the presprective
                cameraPos.xyz /= cameraPos.w;
                // converting into depth texture space (UV space)
                cameraPos.xy = (cameraPos.xy * 0.5) + 0.5;
                // sample depth at world position
                fixed depthColorSample = tex2D(SHADOW_CAMERA_DEPTH, cameraPos.xy).r;

                depthColorSample = OrthoZ2EyeDepth(1 - depthColorSample);
                
                // direction from world position to camera
                fixed3 wPosToShadowCamera = wPos - SHADOW_CAMERA_POSITION.xyz;
                // z depth of point from camera
                fixed zToShadowCamera = dot(wPosToShadowCamera, SHADOW_CAMERA_FORWARD.xyz);
                //dithering section
                float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r; 

                clip(ditherValue * _DitherCtrl - 0.1);
                clip(step(depthColorSample + _CorrectionOffset, zToShadowCamera) - 0.1);
                return _ShadowColor;
              //  return fixed4(depthColorSample, 0, 0, 1);
            }
            ENDCG
        }
        
        Pass
        {
            Tags { "RenderType"="Opaque" }
            LOD 100
            ZWrite OFF
            ZTest OFF
            Blend OneMinusDstColor One // Soft Additive
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            fixed _CorrectionOffset, _DitherCtrl, _FogCtrl, _TestCtrl;
            float4 _BlueNoice_TexelSize, _FogColor;

            float OrthoZ2EyeDepth(float z) {
                return (abs(SHADOW_CAMERA_PROJPARAMS.z) - abs(SHADOW_CAMERA_PROJPARAMS.y)) * z + abs(SHADOW_CAMERA_PROJPARAMS.y);
            }

            fixed SquareMagnitude(fixed2 inVal){
                return inVal.x * inVal.x + inVal.y * inVal.y;
            }

            fixed SquareMagnitude(fixed3 inVal){
                return inVal.x * inVal.x + inVal.y * inVal.y + inVal.z * inVal.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv).r;

                fixed3 camOrigine = mul(unity_CameraToWorld, fixed4(0,0,0,1)).xyz;
                fixed4 dirVector = mul(unity_CameraInvProjection, fixed4(i.uv * 2 - 1, 0, 1));
              
                 // screen ray offset control
                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 screenNoiceCoordinate = screenPos * _ScreenParams.xy * _BlueNoice_TexelSize.xy;
                fixed noiceValue = (tex2D(_BlueNoice, screenNoiceCoordinate).r * 2 - 1); 

                fixed noiceVectorLength = STRIDE_LENGTH * noiceValue;


                dirVector.z *= -1;
                dirVector.xyz = mul(unity_CameraToWorld, dirVector.xyz).xyz;

                camOrigine += dirVector.xyz * noiceVectorLength;

                // Z depth in units
                fixed vectorScale = LinearEyeDepth(depth);
                // is the vector hitting an object
                int lineType = step(0,50 - vectorScale);
                vectorScale = ((vectorScale * lineType) + 50 * (1 - lineType)) + -noiceVectorLength;
                // world position
                fixed3 finalWPos = (dirVector.xyz * vectorScale) + camOrigine;



                // transform final hit world position to shadow camera position
                fixed4 finalHItPosInDepthCamera = mul(SHADOW_CAMERA_MATRIX, fixed4(finalWPos, 1));

                // applaying the presprective
                finalHItPosInDepthCamera.xyz /= finalHItPosInDepthCamera.w;
                // converting into depth texture space (UV space)
                fixed2 finalUVPoint = (finalHItPosInDepthCamera.xy * 0.5) + 0.5;
                fixed2 initialUVPoint = (mul(SHADOW_CAMERA_MATRIX, fixed4(camOrigine, 1)).xy * 0.5) + 0.5;

                float strideCount = vectorScale/ STRIDE_LENGTH;
                fixed2 uvStrideVec = (finalUVPoint - initialUVPoint)/strideCount;
                
               

                fixed3 noiceOffsetVector = dirVector * STRIDE_LENGTH * noiceValue;
                fixed2 noiceUVOffset = uvStrideVec * noiceValue;

                // fog accumulation strendth
                fixed sigmaFog = 0;
                int maxStrides = floor(strideCount);
                float strideFraction = frac(strideCount);

                for(int i = 1; i < 64; i++){
                    int stridePhase = step(0, maxStrides - i);
                    if(i > maxStrides + 1) break;
                    fixed sampleStep = (i * stridePhase) + (strideCount - _CorrectionOffset) * (1 - stridePhase);
                    fixed2 testPtUV = initialUVPoint + (uvStrideVec * sampleStep);
                    fixed depthColorSample = tex2D(SHADOW_CAMERA_DEPTH, testPtUV).r;
                    depthColorSample = OrthoZ2EyeDepth(1 - depthColorSample);
                    // lerping from camera origin to git point
                    fixed3 wLerpPos = lerp(camOrigine, finalWPos, sampleStep/strideCount);
    
                    fixed3 wPosToShadowCamera = wLerpPos - SHADOW_CAMERA_POSITION.xyz;
                // z depth of point from camera
                    fixed zToShadowCamera = dot(wPosToShadowCamera, SHADOW_CAMERA_FORWARD.xyz);
                    fixed strideFogdencity = (STRIDE_LENGTH * stridePhase) + STRIDE_LENGTH * strideFraction * (1 - stridePhase);
                    if(SquareMagnitude(wLerpPos.xyz) < pow(20,2 ))sigmaFog += step(zToShadowCamera, depthColorSample) * _FogCtrl * strideFogdencity;
                }
                return _FogColor * clamp(pow(sigmaFog, 2), 0, 0.95);
            }
            ENDCG
        }
    }
}
