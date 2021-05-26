Shader "Volumetric/VolumeTest_CameraShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CameraShadowTexture ("Camera Shadow", 2D) = "black" {}
        _Color ("Color", color) = (1,1,1,1)
        _DitherPattern ("Dithering Pattern", 2D) = "black" {}
        _UnitValue("Unit Amount", Range(0, 5)) = 0.2
        _StepSize("Step Size", Range(0, 1)) = 0.2
        _Bounds("Box Size", vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Cull back
		Zwrite Off
		Blend OneMinusDstColor One
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
                float3 pos : TEXCOORD1;
                float3 worldViewDir : TEXCOORD2;
                float4 screenPosition : TEXCOORD3;
            };

            sampler2D _MainTex, _CameraShadowTexture;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _Bounds;
            fixed _StepSize;
            fixed _UnitValue;
            float4x4 _WorldToVolumeCamera;

            sampler2D _DitherPattern;
            float4 _DitherPattern_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                o.worldViewDir = normalize(UnityWorldSpaceViewDir(o.pos));
                o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }


            float IsObscured(fixed3 wPos)
            {
                fixed4 volumeCameraView = mul(_WorldToVolumeCamera, fixed4(wPos,1));
                fixed depthValue = LinearEyeDepth(tex2D( _CameraShadowTexture, saturate(volumeCameraView.xy * 0.5 + 0.5)));
                return step(1 - depthValue, 0);
            }


            fixed4 frag (v2f i) : SV_Target
            {
                
                float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
              
            //    return ditherValue;

                float fogDencity = 0;

                float3 rayPoint = i.pos;
           //     fogDencity *= ;

                for(int n = 0; n < 512; n++)
                {

                    float2 ditherCoordinate = (screenPos + fixed2(n * 2, n * 3)) * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                    float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r ;
                    fixed3 castVector = -i.worldViewDir * _StepSize * ditherValue;
                    rayPoint += castVector;
                    fogDencity += step(abs(rayPoint.x), _Bounds.x) * step(abs(rayPoint.y), _Bounds.y) * step(abs(rayPoint.z), _Bounds.z) * IsObscured(rayPoint) * length(castVector)/_UnitValue;
                }
                
        //        clip(ditherValue - 0.5);

                return _Color * fogDencity;
            }
            ENDCG
        }
    }
}
