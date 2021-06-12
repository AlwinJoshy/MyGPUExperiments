Shader "MyShader/DepthDecal_Object"
{
    Properties
    {
        _DecalTex("Decal Texture", 2D) = "white"{}
        _ScanShift ("Shift", Range(0,3)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        ZWrite Off
        ZTest greater
        Cull front
        //Blend SrcAlpha OneMinusSrcAlpha
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
            };

            struct v2f
            {
                fixed4 vertex : SV_POSITION;
                fixed4 screenPosition : NORMAL0;
                fixed3 viewDir : NORMAL1;
                fixed3 vDir : NORMAL2;
                fixed3 NvDir : NORMAL3;
            };

            sampler2D _CameraDepthTexture, _DecalTex; 
            fixed _ScanShift;

            fixed SqrMag(fixed3 v){
                return v.x * v.x + v.y * v.y + v.z * v.z;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                fixed3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz); // get normalized view dir
                o.viewDir /= o.viewDir.z; // rescale vector so z is 1.0
                o.vDir = UnityWorldSpaceViewDir(wPos);
                o.NvDir = -normalize(o.vDir);
                return o;
            }



            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
                fixed depth = LinearEyeDepth(tex2D(_CameraDepthTexture, textureCoordinate).r);
                fixed3 wDPos = _WorldSpaceCameraPos + i.NvDir * depth * length(i.viewDir);

                fixed3 oPos = mul(unity_WorldToObject, fixed4(wDPos, 1)) * _ScanShift;
                clip(step(abs(oPos.x), 0.5) * step(abs(oPos.y), 0.5) *step(SqrMag(oPos) * _ScanShift, 0.5) - 0.01);
                return tex2D(_DecalTex, fixed2(oPos.x, oPos.y) + 0.5); 
            }
            ENDCG
        }
    }
}
