Shader "Unlit/Advanced/SmokeSimulator"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" {}
		_FlowMap("Flowap", 2D) = "black" {}
		_FlowAmount("Flow Amount", Range(0, 1)) = 0.1
		_FadeStrength("Fade Strength", Range(0, 2)) = 0.1
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

				sampler2D _MainTex, _FlowMap;
				float4 _MainTex_ST;
				float4 _MainTex_TexelSize;
				fixed _FlowAmount, _FadeStrength;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					fixed4 value = tex2D(_FlowMap, i.uv);

					fixed2 shift = -((value.xy * 2) - 1) * _FlowAmount;
					//fixed2 shift = fixed2(sin(i.uv.x * 100), sin(i.uv.y * 100)) * _FlowAmount;
					//fixed2 shift = fixed2(0, 1) * _FlowAmount;
					// sample all the neibouring pixels

					float3 col = tex2D(_MainTex, i.uv + shift + fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)) +
					tex2D(_MainTex, i.uv + shift + fixed2(_MainTex_TexelSize.x, 0)) +
					tex2D(_MainTex, i.uv + shift + fixed2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y)) +
					tex2D(_MainTex, i.uv + shift + fixed2(0, -_MainTex_TexelSize.y)) +
					tex2D(_MainTex, i.uv + shift + fixed2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y)) +
					tex2D(_MainTex, i.uv + shift + fixed2(-_MainTex_TexelSize.x, 0)) +
					tex2D(_MainTex, i.uv + shift + fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)) +
					tex2D(_MainTex, i.uv + shift + fixed2(0, _MainTex_TexelSize.y));

					col *= 0.125 * _FadeStrength * value.z;
					return fixed4(col, 1);
				}
				ENDCG
			}
		}
}
