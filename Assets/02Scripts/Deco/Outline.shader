Shader "Custom/Outline"
{
    SubShader
    {
        Tags { "Queue" = "Overlay" }

        // 아웃라인 렌더링 Pass
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }

            ZWrite On
            ZTest LEqual
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            // 아웃라인을 그리기 위한 처리
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex + v.normal * _OutlineWidth);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // 원본 스프라이트 렌더링 Pass
        Pass
        {
            Tags { "LightMode" = "Always" }

            ZWrite On
            ZTest LEqual
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            // 스프라이트 렌더링 처리
            CGPROGRAM
            #include "UnityCG.cginc"
            ENDCG
        }
    }

    Fallback "Diffuse"
}
