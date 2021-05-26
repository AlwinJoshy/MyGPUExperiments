Shader "Unlit/VelocitySimulatorObstacleCurl"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _FlowMap ("Flowap", 2D) = "black" {}
        _FlowAmount ("Flow Amount", Range(0, 1)) = 0.1
        _Divergence ("Divergence Ctrl", Range(0, 2)) = 1
        _Curl ("Curl Ctrl", Range(0, 10)) = 1
        _CurlRes ("Curl Vec Resp", Range(-1, 1)) = 0
        _SurfaceResp ("Surface Resp", Range(0, 2)) = 1
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

            sampler2D _MainTex, _FlowMap;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed _FlowAmount, _Divergence, _Curl, _CurlRes, _SurfaceResp;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 myBit = tex2D(_MainTex, i.uv);
                myBit.xy = (myBit.xy * 2) - 1;

                // sample all the neibouring pixels
                fixed4 pc = tex2D(_MainTex, i.uv  + fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y));
                pc.xy = (pc.xy * 2) - 1;
                fixed4 pf = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0));
                pf.xy = (pf.xy * 2) - 1;
                fixed4 pi = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y));
                pi.xy = (pi.xy * 2) - 1;
                fixed4 ph = tex2D(_MainTex, i.uv + fixed2(0, -_MainTex_TexelSize.y));
                ph.xy = (ph.xy * 2) - 1;
                fixed4 pg = tex2D(_MainTex, i.uv + fixed2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y));
                pg.xy = (pg.xy * 2) - 1;
                fixed4 pd = tex2D(_MainTex, i.uv + fixed2(-_MainTex_TexelSize.x, 0));
                pd.xy = (pd.xy * 2) - 1;
                fixed4 pa = tex2D(_MainTex, i.uv + fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y));
                pa.xy = (pa.xy * 2) - 1;
                fixed4 pb = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y));
                pb.xy = (pb.xy * 2) - 1;

                float totalActiveNeibours = pc.z + pf.z + pi.z + ph.z + pg.z + pd.z + pa.z + pb.z;

                float vc = dot(normalize(pc.xy), -normalize(fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)));
                float vf = dot(normalize(pf.xy), -normalize(fixed2(_MainTex_TexelSize.x, 0)));
                float vi = dot(normalize(pi.xy), -normalize(fixed2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y)));
                float vh = dot(normalize(ph.xy), -normalize(fixed2(0, -_MainTex_TexelSize.y)));
                float vg = dot(normalize(pg.xy), -normalize(fixed2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y)));
                float vd = dot(normalize(pd.xy), -normalize(fixed2(-_MainTex_TexelSize.x,0)));
                float va = dot(normalize(pa.xy), -normalize(fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)));
                float vb = dot(normalize(pb.xy), -normalize(fixed2(0, _MainTex_TexelSize.y)));



                // divergence values
                float2 dc = pc.xy * pow(max(vc, 0), _Divergence) * pc.z;
                float2 df = pf.xy * pow(max(vf, 0), _Divergence) * pf.z;
                float2 di = pi.xy * pow(max(vi, 0), _Divergence) * pi.z;
                float2 dh = ph.xy * pow(max(vh, 0), _Divergence) * ph.z;
                float2 dg = pg.xy * pow(max(vg, 0), _Divergence) * pg.z;
                float2 dd = pd.xy * pow(max(vd, 0), _Divergence) * pd.z;
                float2 da = pa.xy * pow(max(va, 0), _Divergence) * pa.z;
                float2 db = pb.xy * pow(max(vb, 0), _Divergence) * pb.z;
                float4 col = 0;
                col.xy = dc + df + di + dh + dg + dd + da + db + myBit.xy;
                col.xy *= pow(1/totalActiveNeibours, _SurfaceResp) * 2.95 * _FlowAmount;

                // curl values
                

                // dencity curl 
                fixed2 curl = 
                -normalize(fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)) * length(pc.xy) * (1 - abs(vc) * step(_CurlRes, vc))+
                -normalize(fixed2(_MainTex_TexelSize.x, 0)) * length(pf.xy) * (1 - abs(vf)) * step(_CurlRes, vf) +
                -normalize(fixed2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y)) * length(pi.xy) * (1 - abs(vi)) * step(_CurlRes, vi)+
                -normalize(fixed2(0, -_MainTex_TexelSize.y)) * length(ph.xy) * (1 - abs(vh)) * step(_CurlRes, vh) +
                -normalize(fixed2(-_MainTex_TexelSize.x, -_MainTex_TexelSize.y)) * length(pg.xy) * (1 - abs(vg)) * step(_CurlRes, vg) +
                -normalize(fixed2(-_MainTex_TexelSize.x, 0)) * length(pd.xy) * (1 - abs(vd)) * step(_CurlRes, vd) +
                -normalize(fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)) * length(pa.xy) * (1 - abs(va)) * step(_CurlRes, va) +
                -normalize(fixed2(0, _MainTex_TexelSize.y)) * length(pb.xy) * (1 - abs(vb)) * step(_CurlRes, vb);
                col.xy += curl * _Curl;

                col.xy = ((col.xy * myBit.z * 0.5) + 0.5);
                col.zw = myBit.zw;

                return col;
            }
            ENDCG
        }
    }
}
