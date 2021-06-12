Shader "Modifier/SpringShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IridTex ("Irid Tex", 2D) = "white" {}
        _Color ("Spring Color", color) = (1,1,1,1)
        _Align("Align", range(0, 10)) = 1
        _Twist("Rotation", range(0, 10)) = 1
        _Width("Width", range(0, 10)) = 1
        _Height("Height", range(0, 10)) = 1
        _Thickness("Thickness", range(0, 50)) = 1
        _SpecularColor ("Specular Color", color) = (1,1,1,1)
        _Specular("Gloss", range(0, 100)) = 1
        _SpecStreng("Gloss Power", range(0, 20)) = 1
        _IridEffect("Irid Effect", range(0, 1)) = 1
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
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed3 nornal : NORMAL0;
                fixed4 vertex : SV_POSITION;
                fixed3 viewDir : NORMAL1;
                fixed3 worldRefl : NORMAL2;
            };

            sampler2D _MainTex, _IridTex;
            fixed4 _MainTex_ST, _Color, _SpecularColor;
            fixed _Twist, _Width, _Height, _Align, _Thickness, _Specular, _IridEffect, _SpecStreng;

            fixed2 RotateVector(fixed theta, fixed2 vec)
            {
                return fixed2(vec.x * cos(theta) - vec.y * sin(theta), vec.x * sin(theta) + vec.y * cos(theta));
            }

            v2f vert (appdata v)
            {
                v2f o;

                fixed3 localVert = v.vertex.xyz;

                localVert.y *= _Height;

                fixed2 shiftVector = fixed2(_Width,0);

                fixed heightTemp = localVert.y;

                fixed3 circleCenter = fixed3(0,heightTemp,0);

                fixed2 circlePoint = localVert.zy - circleCenter.zy;

                localVert.yz = RotateVector(6.28319 * _Align, circlePoint);

                localVert.xy *= _Thickness;

                localVert.y += heightTemp;
                localVert.xz += shiftVector;
                circleCenter.xy += shiftVector;

                localVert.xz = RotateVector(v.vertex.y * _Twist, localVert.xz);
                circleCenter.xz = RotateVector(v.vertex.y * _Twist, circleCenter.xz);
                o.nornal = UnityObjectToWorldNormal(normalize(localVert - circleCenter));


                v.vertex.xyz = localVert;
                o.vertex = UnityObjectToClipPos(v.vertex);

                fixed3 wPos = mul(unity_ObjectToWorld, v.vertex.xyz).xyz;

                o.viewDir = normalize(UnityWorldSpaceViewDir(wPos));

                o.worldRefl = reflect(-o.viewDir, o.nornal);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
                half4 skyColor = 1;
                skyColor.xyz = DecodeHDR (skyData, unity_SpecCube0_HDR) * lerp(0, tex2D(_IridTex, fixed2(saturate(dot(i.nornal, i.viewDir)), 0.5)).xyz, _IridEffect);



                fixed spec = max(dot(normalize(i.viewDir + _WorldSpaceLightPos0.xyz), i.nornal),0);
                return (max(saturate(dot(i.nornal, _WorldSpaceLightPos0.xyz)), 0.3) * _Color) + _SpecularColor * saturate(pow(spec,_Specular)) * _SpecStreng + skyColor;
            }
            ENDCG
        }
    }
}
