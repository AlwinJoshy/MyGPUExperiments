// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PixelToWorldSpace/DepthToWorld"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            };

            uniform fixed4x4 _InversionMatrix;

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv).r;

                fixed3 camOrigine = mul(unity_CameraToWorld, fixed4(0,0,0,1)).xyz;
                fixed4 dirVector = mul(unity_CameraInvProjection, fixed4(i.uv * 2 - 1, 0, 1));
              
                dirVector.z *= -1;
                dirVector.xyz = mul(unity_CameraToWorld, dirVector.xyz).xyz;

                fixed vectorScale = LinearEyeDepth(depth);
                fixed3 wPos = (dirVector.xyz * vectorScale) + camOrigine;

                clip(50 - vectorScale);

                return fixed4(frac(wPos), 1);
            }
            ENDCG
        }
    }
}
