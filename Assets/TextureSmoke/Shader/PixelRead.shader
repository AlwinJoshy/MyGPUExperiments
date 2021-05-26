Shader "Unlit/Simple/PixelRead"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Shift("Shift", vector) = (0,0,0,0)
	//	_Point("_Point", float) = 0.5
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainTex_TexelSize, _Shift;
			uniform float _Point;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				//fixed2 i.uv
			//	fixed2 readUV = fixed2();
            //    fixed4 col = tex2D(_MainTex, readUV);

				fixed pixelX = _MainTex_TexelSize.x;
				fixed pixelY = _MainTex_TexelSize.y;

				fixed2 readUV = fixed2((pixelX * _Shift.x) - pixelX * 0.5, (pixelY * _Shift.y) - pixelY * 0.5);

				fixed4 pointCol = tex2D(_MainTex, readUV);
				_Point = Luminance(pointCol);
                return pointCol;
            }
            ENDCG
        }
    }
}
