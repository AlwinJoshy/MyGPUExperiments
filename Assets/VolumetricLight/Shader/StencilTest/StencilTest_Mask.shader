Shader "StencilTest/StencilTest_Mask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite On
        ColorMask 0
        Stencil{
            Ref 2
            Comp always
            pass replace
        }
        Pass
        {
       
        }
    }
}
