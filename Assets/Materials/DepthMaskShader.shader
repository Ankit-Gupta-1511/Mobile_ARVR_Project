Shader "Custom/DepthOnlyOccluder"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-1" }
        Lighting Off
        ZWrite On
        ZTest LEqual
        ColorMask 0
        Cull Off

        Pass { 
              
        }
    }

    FallBack Off
}
