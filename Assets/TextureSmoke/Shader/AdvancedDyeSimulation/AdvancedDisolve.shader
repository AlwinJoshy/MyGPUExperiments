Shader "Unlit/Advanced/Disolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
		_ObjectTex("Obj Texture", 2D) = "black" {}
		_Shift("Shift", range(0.0001, 0.1)) = 0.01
		_SmokeAmount("Disolve amount", range(0.0001, 1)) = 0.01
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

            sampler2D _MainTex, _ObjectTex;
            float4 _MainTex_ST;
			float _Shift, _SmokeAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // dye texture
                fixed4 col = tex2D(_MainTex, i.uv);
				// object block texture
				fixed4 check_top = tex2D(_ObjectTex, i.uv +  fixed2(0, _Shift));
				fixed4 check_top_left = tex2D(_ObjectTex, i.uv + fixed2(_Shift, _Shift) / 1.5);
				fixed4 check_top_right = tex2D(_ObjectTex, i.uv + fixed2(-_Shift, _Shift) / 1.5);

				fixed4 check_down = tex2D(_ObjectTex, i.uv + fixed2(0, -_Shift));
				fixed4 check_down_left = tex2D(_ObjectTex, i.uv + fixed2(_Shift, -_Shift) / 1.5);
				fixed4 check_down_right = tex2D(_ObjectTex, i.uv + fixed2(-_Shift, -_Shift)/ 1.5);


				fixed4 check_left = tex2D(_ObjectTex, i.uv + fixed2(_Shift, 0));
				fixed4 check_right = tex2D(_ObjectTex, i.uv + fixed2(-_Shift, 0));

				float val = 1- Luminance(col);

				float outLine = max(check_down_right.a - val, 0) + max(check_down_left.a - val, 0) + max(check_top_right.a - val, 0) + max(check_top_left.a - val, 0) + max(check_top.a - val, 0) + max(check_down.a - val, 0) + max(check_right.a - val, 0) + max(check_left.a - val, 0);
		
				//	col = lerp(col, block, (1 - step(block.a, 0.2)) * disolveAmount);
			//	float outLine = max(check_top.a - val, 0);
				fixed4 leakColor = (
					check_top * check_top.a + 
					check_down * check_down.a + 
					check_left * check_left.a + 
					check_right * check_right.a + 
					check_top_left * check_top_left.a + 
					check_top_right * check_top_right.a +
					check_down_left * check_down_left.a +
					check_down_right * check_down_right.a);
				leakColor.a = 1;
			//	leakColor.rgb = leakColor.a;
                return lerp(col, saturate(leakColor), saturate(outLine) * _SmokeAmount);
            }
            ENDCG
        }
    }
}
