Shader "Unlit/Advanced/DrawDyeSpot"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FlowTex("Flow Tex", 2D) = "black" {}
		_Color("Color", color) = (1,1,1,1)
		_Range("Radi", range(100, 1)) = 20
		_Amount("Amount", range(0, 1)) = 20
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
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

				sampler2D _MainTex, _FlowTex;
				float4 _MainTex_ST;
				float2 _DrawPoint;
				fixed4 _Color;
				fixed _Range, _Amount;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_FlowTex, i.uv);
					fixed2 dir = i.uv - _DrawPoint;
					fixed dye = 1 / pow(max(length(dir * _Range),1), 2);
					return tex2D(_MainTex, i.uv) + (_Color * _Amount * dye * col.z);
				}
				ENDCG
			}
		}
}
