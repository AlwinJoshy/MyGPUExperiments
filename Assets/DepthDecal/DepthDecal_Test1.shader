Shader "MyShader/DepthDecal_Test"
{
    Properties
    {
        _ScanShift ("Shift", Range(0,3)) = 0.01
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
                float4 vertex : SV_POSITION;
                fixed4 screenPosition : NORMAL0;
                fixed3 viewDir : NORMAL1;
                fixed3 vDir : NORMAL2;
            };

            sampler2D _CameraDepthTexture; 
            fixed _ScanShift;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                fixed3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz); // get normalized view dir
                o.viewDir /= o.viewDir.z; // rescale vector so z is 1.0
                o.vDir = UnityWorldSpaceViewDir(wPos);
                return o;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;

                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, textureCoordinate).r);

                depth *= length(i.viewDir);

                return frac(fixed4(_WorldSpaceCameraPos + normalize(-i.vDir) * depth, 1)); 
            }
            ENDCG
        }
    }
}
