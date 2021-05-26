Shader "Volumetric/VolumeTest1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StepSize("Step Size", Range(0, 0.3)) = 0.2
        _Bounds("Box Size", vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Cull back
		Zwrite Off
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 pos : TEXCOORD1;
                float3 worldViewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Bounds;
            fixed _StepSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                o.worldViewDir = normalize(UnityWorldSpaceViewDir(o.pos));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                float fogDencity = 0;

                float3 rayPoint = i.pos;
           //     fogDencity *= ;

                for(int n = 0; n < 512; n++)
                {
                    fogDencity += step(abs(rayPoint.x), _Bounds.x) * step(abs(rayPoint.y), _Bounds.y) * step(abs(rayPoint.z), _Bounds.z) * 0.03;
                    rayPoint += -i.worldViewDir * _StepSize;
                }
                

                return fogDencity;
            }
            ENDCG
        }
    }
}
