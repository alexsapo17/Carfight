Shader "Custom/CircleExpandShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.5,0.5,0.5,1)
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", float) = 0.5
        _EdgeSoftness ("Edge Softness", float) = 0.1
        _RingWidth ("Ring Width", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BaseColor;
            fixed4 _Color;
            float _Radius;
            float _EdgeSoftness;
            float _RingWidth;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 originalColor = tex2D(_MainTex, i.uv);
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float ringEffect = smoothstep(_Radius - _EdgeSoftness - _RingWidth, _Radius - _RingWidth, dist) - 
                                   smoothstep(_Radius, _Radius + _EdgeSoftness, dist);
                                   
                // Applica l'effetto di colore mantenendo la trasparenza originale
                fixed4 colorEffect = lerp(_BaseColor, _Color, ringEffect) * originalColor.a;
                fixed4 resultColor = lerp(originalColor * colorEffect.a, colorEffect, colorEffect.a);
                
                // Mantiene la trasparenza originale dell'immagine
                resultColor.a = originalColor.a;
                
                return resultColor;
            }
            ENDCG
        }
    }
}

