Shader "Dojo/Test/DepthNormalVisualizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanShift ("Shift", Range(0,1)) = 0.01
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
                
                fixed4 screenPosition : NORMAL3;
            };

            float4x4 _VWMatrix;
            sampler2D _MainTex;
            fixed _ScanShift;
            float4 _MainTex_ST;
            sampler2D _CameraDepthNormalsTexture;   

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float normpdf(float x, float sigma)
            {
                return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;

                
                float3 normal, normalSum = 0;
                float depth;

                
                const int mSize = 11;
                const int iter = (mSize - 1) / 2;

                for (int i = -iter; i <= iter; ++i) {
                    for (int j = -iter; j <= iter; ++j) {
                        float4 depthnormal = tex2D(_CameraDepthNormalsTexture, float2(textureCoordinate.x + i * _ScanShift, textureCoordinate.y + j * _ScanShift));
                        DecodeDepthNormal(depthnormal, depth, normal);
                        normalSum += normal * normpdf(float(i), 7);
                    }
                }
            
                normalSum /= mSize;
                normalSum = mul((float3x3)_VWMatrix, normalSum);
                float blendLight = dot(normalSum, fixed4(1,1,-1,0));
                

                return blendLight;
            }
            ENDCG
        }
    }
}
