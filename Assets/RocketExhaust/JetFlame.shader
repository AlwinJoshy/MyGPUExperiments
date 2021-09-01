Shader "Alwin_BS/JetFlame"
{
    Properties
    {
        _FlameOuterColor("Outer Color", color) = (1,1,1,1)
        _FlameOuterTaper("Flame Outer Taper", range(-5, 5)) = 3
        _FlameOuterTerbulance("Flame Outer Terbulance", range(0, 5)) = 3
        _FlameOuterFade("Flame Outer Fade", range(0, 0.5)) = 3


        _SpeedDiamondColor("Inner Color", color) = (1,1,1,1)
        _FlameColumTaper("Flame Colum Taper", range(-5, 5)) = 3
        _FlameColumFade("Flame Colum Fade", range(0, 0.5)) = 3
        _FlameColumSize("Flame Colum Size", range(0, 1)) = 3
        _FlameColumLength("Flame Colum Length", range(0, 3)) = 3
        _FlameColumDisplaceRange("Flame Colum Displace Range", range(0, 1)) = 3
        _ForecColumCount("Flame Colum Count", int) = 1

        _SpeedForceTex("Speed Force Texture", 2D) = "white" {}
        _SpeedForceColor("Speed Color", color) = (1,1,1,1)
        _SpeedForceTaper("Speed Colum Taper", range(-5, 5)) = 3
        _SpeedForceSpeed("Speed Force Speed", range(0, 100)) = 3
        _SpeedForceTerbulance("Speed Outer Terbulance", range(0, 5)) = 3
        _SpeedForceSize("Speed Force Size", range(0, 1)) = 3
        _SpeedForceFade("Speed Colum Fade", range(0, 1)) = 3
        _SpeedForceScale("Speed Texture Scale", Vector) = (0,0,0,0)
    }

    CGINCLUDE
    #pragma fragment frag
    #include "UnityCG.cginc"


    struct appdata
    {
        fixed4 vertex : POSITION;
        fixed3 normal : NORMAL;
        fixed2 uv : TEXCOORD0;
    };

    struct v2f
    {
        fixed2 uv : TEXCOORD0;
        fixed4 vertex : SV_POSITION;
        fixed3 oPos : TEXCOORD1;
    };

    sampler2D _SpeedForceTex;
    fixed4 _FlameOuterColor, 
    _SpeedForceColor,
    _SpeedDiamondColor,
    _SpeedForceScale;
    
    fixed 
    _FlameOuterTaper, 
    _FlameColumTaper, 
    _FlameOuterFade, 
    _FlameColumSize, 
    _FlameColumLength,
    _FlameColumFade,
    _SpeedForceSize,
    _SpeedForceFade,
    _FlameOuterTerbulance,
    _SpeedForceTerbulance,
    _SpeedForceSpeed,
    _SpeedForceTaper,
    _ForecColumCount,
    _FlameColumDisplaceRange;


    ENDCG

    SubShader
    {

       

        // speed layer

        Pass
        {

            Tags { "RenderType"="Transparent" }
            LOD 100
            ZWrite Off
            Blend OneMinusDstColor One // Soft additive
            Cull OFF


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xy += v.normal.xy * tex2Dlod(_SpeedForceTex, fixed4(v.uv.x * _SpeedForceScale.x + (_Time.y * _SpeedForceSpeed), v.uv.y * _SpeedForceScale.y + _Time.y, 0, 0)) * v.vertex.z * _SpeedForceTerbulance;
                v.vertex.xyz += v.normal * v.vertex.z * _SpeedForceTaper;
                v.vertex.xy *= _SpeedForceSize;
                o.oPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _SpeedForceColor * tex2D(_SpeedForceTex, fixed2((i.uv.x + (_Time.y * _SpeedForceSpeed)), i.uv.y)).r * saturate(1 - (i.oPos.z * _SpeedForceFade));
            }
            ENDCG
        }

        Pass
        {

            Tags { "RenderType"="Transparent" }
            LOD 100
            ZWrite Off
            Blend One One // Premultiplied transparency
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed zShift;

            v2f vert (appdata v)
            {
                v2f o;
                zShift = v.vertex.z + sin(_Time.z * 100) * _FlameColumDisplaceRange;
                v.vertex.z *= _FlameColumLength;
                v.vertex.xy *= _FlameColumSize * (sin(zShift * _ForecColumCount) * 0.5 + 0.5);
                
                v.vertex.xyz += v.normal * zShift * _FlameColumTaper;
             //   v.vertex.x += tex2Dlod(_SpeedForceTex, fixed4(v.uv.x + _Time.y * _SpeedForceSpeed, v.uv.y + _Time.y, 0, 0)).x * 2 - 1;
            //    v.vertex.y += tex2Dlod(_SpeedForceTex, fixed4(v.uv.x + _Time.y * _SpeedForceSpeed, v.uv.y + _Time.y + 0.1, 0, 0)).x * 2 - 1;
               
                o.oPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _SpeedDiamondColor * saturate((sin(zShift * _ForecColumCount) * 0.5 + 0.5) * saturate(1 - (i.oPos.z * _FlameColumFade)));
            }
            ENDCG
        }

         Pass
        {

            Tags { "RenderType"="Transparent" }
            LOD 100
            ZWrite Off
            Blend One One // Soft additive
            Cull OFF


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.xy += v.normal.xy * tex2Dlod(_SpeedForceTex, fixed4(v.uv.x * _SpeedForceScale.x + (_Time.y * _SpeedForceSpeed), v.uv.y * _SpeedForceScale.y + _Time.y, 0, 0)) * v.vertex.z * _FlameOuterTerbulance;
                v.vertex.xyz += v.normal * v.vertex.z * _FlameOuterTaper;
                o.oPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _FlameOuterColor * saturate(1 - (i.oPos.z * _FlameOuterFade));
            }
            ENDCG
        }


    }
}
