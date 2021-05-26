Shader "PixelToWorldSpace/TransmissionAndDepth"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", color) = (1,1,1,1)
        [Toggle] _isTransmission("is Transmission", Float) = 0
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
        LOD 1
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed3 color : COLOR0;
                float depth : DEPTH;
            };
            fixed _isTransmission;
            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;
            uniform float4 SHADOW_CAMERA_PROJPARAMS;
            uniform fixed3 _ClearColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * SHADOW_CAMERA_PROJPARAMS.w;
                o.color = _Color.xyz * _isTransmission +  _ClearColor * (1 - _isTransmission);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {   
                fixed3 col = tex2D(_MainTex, i.uv).xyz;
                return float4(i.color * col, i.depth);
            }
            ENDCG
        }
    }

        SubShader
    {

        Tags { "RenderType"="Active" }
        Cull OFF
        LOD 1
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
                float depth : DEPTH;
            };
            fixed _isTransmission;
            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;
            uniform float4 SHADOW_CAMERA_PROJPARAMS;
            uniform fixed3 _ClearColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * SHADOW_CAMERA_PROJPARAMS.w;
                o.color = v.color;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {   
                fixed3 col = tex2D(_MainTex, i.uv).xyz;
                clip((col.r) - (1 - i.color.a));
                return float4(_ClearColor, i.depth);
            }
            ENDCG
        }
    }

        SubShader
    {

        Tags { "RenderType"="Bird" }
        Cull OFF
        LOD 1
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float depth : DEPTH;
            };
            fixed _isTransmission;
            sampler2D _MainTex;
            float4 _MainTex_ST, _Color;
            uniform float4 SHADOW_CAMERA_PROJPARAMS;
            uniform fixed3 _ClearColor;
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
                o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * SHADOW_CAMERA_PROJPARAMS.w;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {   
                return float4(fixed3(0,0,0), i.depth);
            }
            ENDCG
        }
    }

}
