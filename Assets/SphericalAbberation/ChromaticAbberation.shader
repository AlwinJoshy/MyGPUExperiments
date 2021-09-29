Shader "Spherical Lense/ChromaticAbberation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LenseColor ("Base Color", color) = (1,1,1,1)
        _LenseDarkColor ("Dark Color", color) = (1,1,1,1)
        _LenseDist ("Lense Distortion", range(0, 30)) = 1
        _UVOffset ("UV Offset", Vector) = (0,0,0,0)
        _CurvePower ("Curve Power", range(0, 30)) = 1
        _ColorShift ("Color Offset", Vector) = (0,0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex, _BackgroundTexture;
            float _LenseDist, _CurvePower;
            float4 
            _MainTex_ST, 
            _UVOffset, 
            _ColorShift,
            _LenseColor,
            _LenseDarkColor;
            
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
                fixed2 newUV = (i.grabPos.xy/i.grabPos.w - 0.5) * 2;
                fixed2 ptUV = (i.uv - 0.5) * 2;
                fixed pointScale = length(ptUV);
                float lenseScale = length(newUV);
                half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);

                fixed2 dir = normalize(newUV);

                fixed4 col = 
                fixed4(
                tex2D(_BackgroundTexture, (newUV + newUV * pow(pointScale, _LenseDist) * _ColorShift.r) * 0.25 + 0.5).r,
                tex2D(_BackgroundTexture, (newUV + newUV * pow(pointScale, _LenseDist) * _ColorShift.g) * 0.25 + 0.5).g,
                tex2D(_BackgroundTexture, (newUV + newUV * pow(pointScale, _LenseDist) * _ColorShift.b) * 0.25 + 0.5).b,
                1);

                fixed cutOff = step(pointScale, 1);
                clip(cutOff - 0.01);
                //return pow(pointScale, _LenseDist);
                return col * lerp(_LenseColor, _LenseDarkColor, pow(saturate(1.5 - pointScale), 2));
                //return pow((pointScale + 0.5)/1.5, _LenseDist);
            }
            ENDCG
        }
    }
}