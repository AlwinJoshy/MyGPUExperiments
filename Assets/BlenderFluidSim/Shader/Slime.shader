Shader "FluidSim/Slime_1"
{
    Properties
    {
        _BaseColor("Base Color", color) = (1,1,1,1)
        _SideColor("Side Color", color) = (1,1,1,1)
        _RimGloss("Rim Gloss", color) = (1,1,1,1)
        _RimCtrl("Rim Ctrl", range(0, 5)) = 1
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
            // make fog work
            #pragma shader_feature_local

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 viewDir : NORMAL;
                fixed3 normal : NORMAL1;
                fixed3 hVec : NORMAL2;
            };

            float4 _BaseColor, _SideColor, _RimGloss;
            fixed _RimCtrl;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                fixed3 wPos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = normalize(UnityWorldSpaceViewDir(wPos));
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.hVec = normalize(o.viewDir + normalize(_WorldSpaceLightPos0.xyz));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
              //  return fixed4(i.hVec, 0);
                return  pow(lerp(1, 0, saturate(1 - dot(i.hVec, i.normal))), 20) + lerp(_SideColor, _BaseColor, dot(i.normal, i.viewDir)) + lerp(_RimGloss, 0, saturate(dot(i.normal, i.viewDir) * _RimCtrl));
            }
            ENDCG
        }
    }
}
