Shader "MiniGame/Unlit/MiniGameBirdFlap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MovementPower ("Movement Power", range(0, 20)) = 1
        _TimeScale ("Time Scale", range(0, 20)) = 1
        _OffserStrength ("Offset Effect", range(0, 20)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Bird" }
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
                float4 color : COLOR0;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _OffserStrength, _MovementPower, _TimeScale;

            v2f vert (appdata v)
            {
                v2f o;
                fixed3 newPos = v.vertex;
                fixed theta = sin((_Time.z * _TimeScale) + (v.color.g + _OffserStrength)) * v.color.r * _MovementPower * sign(v.vertex.x);
                fixed valX = newPos.x;
                fixed valY = newPos.y;
                newPos.xy = fixed2((cos(theta) * valX - sin(theta) * valY), (sin(theta) * valX + cos(theta) * valY));
                newPos.y += v.color.b * sin(_Time.z * _TimeScale) * 0.1;
                o.vertex = UnityObjectToClipPos(newPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }

                Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

              struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR0;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f { 
                V2F_SHADOW_CASTER;
            };

            fixed _OffserStrength, _MovementPower, _TimeScale;

            v2f vert(appdata v)
            {
                v2f o;
                fixed3 newPos = v.vertex;
                fixed theta = sin((_Time.z * _TimeScale) + (v.color.g + _OffserStrength)) * v.color.r * _MovementPower * sign(v.vertex.x);
                fixed valX = newPos.x;
                fixed valY = newPos.y;
                newPos.xy = fixed2((cos(theta) * valX - sin(theta) * valY), (sin(theta) * valX + cos(theta) * valY));
                newPos.y += v.color.b * sin(_Time.z * _TimeScale) * 0.1;
                v.vertex.xyz = newPos;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
