Shader "Dojo/Test/HoneyShader"
{
    Properties
    {
        _ScanShift ("Shift", Range(0,1)) = 0.01
        _DepthCompTolerance ("DepthCompTolerance", Range(0,200)) = 50
        _BlendValue ("Blend Effect", Range(0,5)) = 50
        _ColorA("Color A", color) = (1,1,1,1)
        _ColorB("Color B", color) = (1,1,1,1)
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
                fixed4 vertex : SV_POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 screenPosition : NORMAL3;
                fixed3 vDir : TEXCOORD2;
            };

            fixed4x4 _VWMatrix;
            fixed _ScanShift, _DepthCompTolerance, _BlendValue;
            fixed4 _ColorA, _ColorB;
            sampler2D _CameraDepthNormalsTexture;   

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vDir = normalize(UnityWorldSpaceViewDir(worldPos));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed normpdf(fixed x, fixed sigma)
            {
                return 0.39894*exp(-0.5*x*x / (sigma*sigma)) / sigma;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
                fixed3 vDir = i.vDir;
                fixed2 nUV = i.uv;
                fixed3 normal, normalBase, normalSum = 0;
                fixed depth, depthBase, dVal;

                _ScanShift *= 1 - fwidth(nUV.x) * 40;
                const int mSize = 8;
                const int iter = (mSize - 1) / 2;

                fixed4 depthnormalBase = tex2D(_CameraDepthNormalsTexture, textureCoordinate);
                DecodeDepthNormal(depthnormalBase, depthBase, normalBase);
                //  depthBase = Linear01Depth(depthBase);

                for (int i = -iter; i <= iter; ++i) {
                    for (int j = -iter; j <= iter; ++j) {
                        fixed4 depthnormal = tex2D(_CameraDepthNormalsTexture, fixed2(textureCoordinate.x + i * _ScanShift, textureCoordinate.y + j * _ScanShift));
                        DecodeDepthNormal(depthnormal, depth, normal);
                        dVal += (1 - pow(1 - max(depth - depthBase, 0), _BlendValue)) * _DepthCompTolerance;
                        normalSum += lerp(normal * normpdf(fixed(i), 7) * 1.3, normalBase, clamp(dVal , 0, 1));
                        //  normalSum += lerp(normal * normpdf(fixed(i), 7) * 1.3, normalBase, pow(min(max(depth - depthBase, 0) * _DepthCompTolerance, _BlendValue) , 1));

                        //  normalSum += normal;
                    }
                }
                
                normalSum /= mSize;
                normalSum = normalize(mul((fixed3x3)_VWMatrix, normalSum));
                
                fixed3 shiftedNormal = normalSum + dot(vDir, normalSum) * -vDir;

                fixed lightAngle = dot(shiftedNormal, _WorldSpaceLightPos0);

                fixed4 col = 0;
                col.xyz = lerp(_ColorA.xyz, _ColorB.xyz, 1 - ((lightAngle + 1) * 0.5));
                col.xyz += pow(1 - dot(vDir, normalSum), 2) * 1;
                col.xyz += pow(max(dot(reflect(-normalize(_WorldSpaceLightPos0), normalSum), vDir), 0), 10) * 0.3;

                return col;
                //  return dVal;
                // return fixed4(normalSum,0);
            }
            ENDCG
        }
    }
}
