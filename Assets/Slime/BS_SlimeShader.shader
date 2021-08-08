// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BS_Slime"
{
    Properties {
        // three textures we'll use in the material
        _Color("Base Color", color) = (1,1,1,1)
        _LightColor("Light Color", color) = (1,1,1,1)
        _DarkColor("Dark Color", color) = (1,1,1,1)
        _RimColor("Rim Color", color) = (1,1,1,1)
        _RimPower("Rim Power", range(0,20)) = 0.01
        _BumpMap("Normal Map", 2D) = "bump" {}
        _FlowMap("Flow Map", 2D) = "black" {}
        _FlowSpeed("Flow Speed", range(0,2)) = 0.01
        _DistortionMap("Distortion Map", 2D) = "black" {}
        _WobbleSpeed("Wobble Speed", range(0,2)) = 0.01
        _WobbleStrength("Wobble Strength", range(0,2)) = 0.01
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // exactly the same as in previous shader
            struct v2f {
                float3 worldPos : TEXCOORD0;
                half3 tspace0 : TEXCOORD1;
                half3 tspace1 : TEXCOORD2;
                half3 tspace2 : TEXCOORD3;
                float2 uv : TEXCOORD4;
                float4 pos : SV_POSITION;
                float3 wNormal : NORMAL;
                
            };

            sampler2D _DistortionMap, _FlowMap;
            fixed _WobbleSpeed, _WobbleStrength;

            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0)
            {
                v2f o;
                vertex.xyz += normal * (tex2Dlod(_DistortionMap, fixed4(uv + fixed2(_Time.z * _WobbleSpeed, 0), 0,0) * 0.1).r * 2 - 1) * _WobbleStrength;
                o.pos = UnityObjectToClipPos(vertex);
                o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
                o.wNormal = UnityObjectToWorldNormal(normal);
                half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
                half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(o.wNormal, wTangent) * tangentSign;
                o.tspace0 = half3(wTangent.x, wBitangent.x, o.wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, o.wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, o.wNormal.z);
                o.uv = uv;
                return o;
            }

            // textures from shader properties
            sampler2D _MainTex;
            sampler2D _OcclusionMap;
            sampler2D _BumpMap;
            fixed4 _Color, _LightColor, _DarkColor, _RimColor;
            fixed _FlowSpeed, _RimPower;
            
            fixed4 frag (v2f i) : SV_Target
            {
                // same as from previous shader...

                half3 flowVal = (tex2D(_FlowMap, i.uv) * 2 - 1) * _FlowSpeed;
                
                float dif1 = frac(_Time.y * 0.25 + 0.5);
                float dif2 = frac(_Time.y * 0.25);
                
                half lerpVal = abs((0.5 - dif1)/0.5);
                
                half3 col1 = UnpackNormal(tex2D(_BumpMap, i.uv - flowVal.xy * dif1));
                half3 col2 = UnpackNormal(tex2D(_BumpMap, i.uv - flowVal.xy * dif2));
                
                half3 tnormal = lerp(col1, col2, lerpVal);
                
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 worldRefl = reflect(-worldViewDir, worldNormal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);                
                fixed4 c = 1;
                //c.rgb = skyColor;

                // modulate sky color with the base texture, and the occlusion map
                fixed3 baseColor = _Color;
                fixed occlusion = tex2D(_OcclusionMap, i.uv).r;
                fixed3 hVec = (normalize(worldViewDir) + _WorldSpaceLightPos0.xyz) * 0.5;

                c.rgb = _Color * lerp(_DarkColor, _LightColor, dot(worldNormal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5); 
                c.rgb += _RimColor * pow(saturate(1 - dot(worldNormal, normalize(worldViewDir))), _RimPower) + skyColor * 0.2 + pow(saturate(dot(hVec, worldNormal)),10) * 1.5;
                //  c.rgb *= occlusion;

                return c;
            }
            ENDCG
        }
    }
}