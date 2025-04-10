Shader "Custom/DoubleSidedUnlitTexture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        Cull Off
        ZWrite On
        Lighting Off
        Pass {
            SetTexture [_MainTex] { combine texture }
        }
    }
}
