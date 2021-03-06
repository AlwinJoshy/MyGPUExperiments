Shader "Util/Unlit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NightTex ("Night (RGB)", 2D) = "white" {}
        [Toggle] _isTransmission("is Transmission", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf SimpleLambert
        
        half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb;
            c.a = s.Alpha;
            return c;
        }

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        fixed _TimeOfDay;
        uniform fixed _DayTime;
        sampler2D _MainTex, _NightTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = lerp(tex2D (_MainTex, IN.uv_MainTex), tex2D (_NightTex, IN.uv_MainTex), _DayTime) * _Color;
            o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
