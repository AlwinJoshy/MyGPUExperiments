Shader "Util/Clipper"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="NULL" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
            };

            struct v2f
            {
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                clip(-1);
                return 0;
            }
            ENDCG
        }
    }
}
