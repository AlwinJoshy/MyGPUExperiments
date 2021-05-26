Shader "Unlit/WaterMask" {
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry +1"}
        ZTest less
            ColorMask 0
             Stencil {
                Ref 3
                Comp greater
                Pass replace

            }
        Pass {
               ZWrite OFF
        }
    } 
}