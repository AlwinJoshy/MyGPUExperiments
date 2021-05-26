Shader "Unlit/CapsuleDistanceFieldTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radi ("Thickness", range(0, 10)) = 1
        _BladeLength ("Blade Length", range(0, 100)) = 1
        _GlowSpread ("Glow Spread", range(0, 100)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend One One
        ZWrite OFF

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
                fixed3 camDir : TEXCOORD1;
                fixed3 worldPos : TEXCOORD2;
                fixed3 cent : POSITION1;
                fixed3 bladeDir : POSITION2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _BladeLength, _Radi, _GlowSpread;

            float sdCapsule( fixed3 p, fixed3 a, fixed3 b, fixed r )
            {
                fixed3 pa = p - a, ba = b - a;
                float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
                return length( pa - ba * h ) - r;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.camDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                o.cent = mul ( unity_ObjectToWorld, float4(0,-0.5,0,1) ).xyz;
                o.bladeDir = UnityObjectToWorldDir(fixed3(0,1,0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //   fixed4 col = tex2D(_MainTex, i.uv);
                fixed col = 0;
                fixed3 viewVector = 0;
                fixed stepD = 0;
                fixed lastDist = 1000000;

                for(int n = 0; n < 32; n++){
                    fixed dist = sdCapsule(i.worldPos - i.camDir * stepD, i.cent, i.cent + i.bladeDir * _BladeLength, _Radi);
                    if(lastDist < dist)break;
                    lastDist = dist;
                    stepD += dist;
                }

                //return pow(saturate(1 - lastDist * 0.5), 2);
                return tex2D(_MainTex, fixed2(pow(saturate(1 - lastDist * 0.5), _GlowSpread), 0.5));
            }
            ENDCG
        }
    }
}

