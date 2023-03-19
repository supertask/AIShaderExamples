Shader "Custom/MixedShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.5
        _DissolveRamp ("Dissolve Ramp", 2D) = "white" {}
        _DissolveColor ("Dissolve Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _FresnelPower ("Fresnel Power", Range(1, 10)) = 3
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 1)
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DissolveRamp;
        float _DissolveAmount;
        float4 _DissolveColor;
        float _OutlineWidth;
        float4 _OutlineColor;
        float _FresnelPower;
        float4 _FresnelColor;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            float4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = mainTex.rgb;
            o.Alpha = mainTex.a;

            // Dissolve effect
            float dissolveValue = tex2D(_DissolveRamp, IN.uv_MainTex).r;
            if (dissolveValue < _DissolveAmount) {
                o.Albedo = _DissolveColor.rgb;
                o.Alpha = 0;
            }

            // Fresnel effect
            float fresnel = pow(1.0 - dot(IN.viewDir, o.Normal), _FresnelPower);
            o.Emission = _FresnelColor.rgb * fresnel;

            // Outline effect
            float outline = tex2D(_MainTex, IN.uv_MainTex + _OutlineWidth).a;
            outline -= mainTex.a;
            outline = 1.0 - saturate(outline);
            o.Albedo = lerp(_OutlineColor.rgb, o.Albedo, outline);
        }
        ENDCG
    }
    FallBack "Diffuse"
}