Shader "Unlit/Distortion"
{
    Properties
    {
        _Color ("Color", color) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
        _HeatSpeed("Speed", range(0, 3)) = 1
        _Scale("Scale", vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        //  ZTest OFF
        LOD 100
        Cull OFF

        GrabPass
        {
            "_BackgroundTexture"
        }

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
                float4 grabPos : TEXCOORD1;
            };

            sampler2D _MainTex, _BackgroundTexture;;
            float4 _MainTex_ST, _Scale, _Color;
            fixed _HeatSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float val = 1 - i.uv.y;
                fixed4 col = tex2D(_MainTex, fixed2(i.uv.x * _Scale.x, -val* val * _Scale.y) - fixed2(0, _Time.z * _HeatSpeed));



                fixed2 shiftValue = (col.rg - 0.5) * 2 * (1 - i.uv.y);
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos + fixed4(shiftValue.x,shiftValue.y, 0,0) * 0.01);
                return bgcolor + lerp(_Color, 0, saturate(length(shiftValue) + i.uv.y * 1.5));
                
                // return col;
            }
            ENDCG
        }
    }
}
