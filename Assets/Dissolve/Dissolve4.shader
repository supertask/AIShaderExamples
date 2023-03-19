Shader "Custom/Dissolve4" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveRamp ("Dissolve Ramp", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1, 0, 0, 1)
        _EdgeThickness ("Edge Thickness", Range(0, 1)) = 0.1
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _DissolveRamp;
        float _DissolveAmount;
        float _EdgeThickness;
        float4 _EdgeColor;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            float dissolveValue = tex2D(_DissolveRamp, IN.uv_MainTex).r;
            float edge = _DissolveAmount - _EdgeThickness;
            clip(dissolveValue - edge);

            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Alpha = step(edge, dissolveValue);
            o.Emission = _EdgeColor.rgb * (1 - smoothstep(edge, _DissolveAmount, dissolveValue));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
