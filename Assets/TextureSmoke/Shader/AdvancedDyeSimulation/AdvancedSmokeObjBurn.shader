Shader "Unlit/Advanced/SmokeColorClear"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		_BadBlocks("Destructable Blocks", 2D) = "black" {}
		_Outline("Outline", range(0.0001, 0.1)) = 0.01
			_Color("Color", color) = (1,1,1,1)
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

            sampler2D _MainTex, _BadBlocks;
            float4 _MainTex_ST, _Color;
			fixed _Outline;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 colBlocks = tex2D(_BadBlocks, i.uv);

	
				float val = 1 - step(colBlocks.a, 0.2);

				// edge Detection;
				fixed check_top = tex2D(_BadBlocks, i.uv + fixed2(0, -_Outline)).a;
				fixed check_down = tex2D(_BadBlocks, i.uv + fixed2(0, _Outline)).a;
				fixed check_left = tex2D(_BadBlocks, i.uv + fixed2(_Outline, 0)).a;
				fixed check_right = tex2D(_BadBlocks, i.uv + fixed2(-_Outline, 0)).a;

				float outLine = max(check_top - val, 0) + max(check_down - val, 0) + max(check_right - val, 0) + max(check_left - val, 0);
				

				return lerp(col, colBlocks, 1 - step(colBlocks.a, step(colBlocks.a, 0.2))) + (1 - step(_Color * outLine, 0.3));
            }
            ENDCG
        }
    }
}
