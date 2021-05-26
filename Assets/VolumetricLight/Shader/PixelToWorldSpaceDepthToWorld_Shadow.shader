// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PixelToWorldSpace/DepthShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dither Texture", 2D) = "white" {}
        _DitherCtrl("Dither Strength", range(0,1)) = 0
        _CorrectionOffset("Correction Offset", range(0,0.1)) = 0
        _ShadowColor("Shadow Color", color) = (1,1,1,1)
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

            fixed _CorrectionOffset, _DitherCtrl;

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture, _DitherPattern;
            float4 _MainTex_ST, _DitherPattern_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

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

                clip((step(depthColorSample + _CorrectionOffset, zToShadowCamera) * ditherValue * _DitherCtrl) - 0.1);
                return _ShadowColor;
              //  return fixed4(depthColorSample, 0, 0, 1);
            }
            ENDCG
        }
    }
}
