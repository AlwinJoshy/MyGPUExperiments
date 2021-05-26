// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MiniGame/Unlit/LightSphere"
{
    Properties
    {
        _DitherPattern ("Texture", 2D) = "white" {}
        _ColorCore("Core Color", color) = (1,1,1,1)
        _ColorRim("Rim Color", color) = (1,1,1,1)
        _FlixkerContol("Flicker", range(0,1)) = 1
        _ContolDither("Dither Tightness", range(0,5)) = 1
        _Contol("Tightness", range(0,3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque + 1" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed4 screenPosition : TEXCOORD1;
                fixed col : COLOR0;
            };

            sampler2D _DitherPattern;
            fixed _Contol, _ContolDither, _FlixkerContol;
            fixed4 _DitherPattern_TexelSize, _ColorCore, _ColorRim;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.col = dot(normalize(UnityWorldSpaceViewDir(worldPos)), UnityObjectToWorldDir(v.normal));
                return o;
            }

            fixed LightFlicker(){
                return (((sin(_Time.x + 50) + sin(_Time.x * 100) + sin(_Time.x * 200))/3 * 0.5) + 0.5) * _FlixkerContol;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                fixed2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                fixed ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;
                clip((pow(i.col, _ContolDither) + LightFlicker()) - ditherValue);
                return lerp(_ColorRim, _ColorCore, saturate(pow(i.col, _Contol) + LightFlicker()));
            }
            ENDCG
        }
    }
}
