Shader "Skybox/CustomSkybox"
{
    Properties
    {
        [Header(Main)]
        _MainColor ("Color", Color) = (0.5, 0.5, 0.5, 1.0)
        
        [Header(Gradient)]
        _GradientStrength ("Strength", Range(0.0, 1.0)) = 0.0
        _GradientEdge0 ("Edge 0", Float) = 0.0
        _GradientEdge1 ("Edge 1", Float) = 1.0
        _GradientTopColor ("TopColor", Color) = (0.5, 0.5, 0.5, 1.0)
        _GradientBottomColor ("BottomColor", Color) = (0.5, 0.5, 0.5, 1.0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Background"
            "Queue" = "Background"
        }
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
            };

            float4 _MainColor;

            float _GradientStrength;
            float _GradientEdge0;
            float _GradientEdge1;
            float4 _GradientTopColor;
            float4 _GradientBottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // raw uv data, can use function for texture
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = _MainColor;

                float horizonMask = dot(normalize(i.worldPosition), float3(0, 1, 0));
                
                float gradientT = smoothstep(_GradientEdge0, _GradientEdge1, horizonMask);
                float3 gradientColor = lerp(_GradientBottomColor, _GradientTopColor, gradientT);
                col = lerp(col, gradientColor, _GradientStrength);
                
                return float4(col, 1.0);
            }
            ENDCG
        }
    }
}
