Shader "Custom/ColorReplace"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color1 ("Color to Replace 1", Color) = (1,0,0,1)
        _ReplaceColor1 ("Replacement Color 1", Color) = (0,1,0,1)
        _Color2 ("Color to Replace 2", Color) = (1,0,0.5,1)
        _ReplaceColor2 ("Replacement Color 2", Color) = (0,0.5,1,1)
        _Color3 ("Color to Replace 3", Color) = (0.5,0,0,1)
        _ReplaceColor3 ("Replacement Color 3", Color) = (1,0,1,1)
        _Threshold ("Threshold", Range(0, 1)) = 0.01
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color1;
            float4 _ReplaceColor1;
            float4 _Color2;
            float4 _ReplaceColor2;
            float4 _Color3;
            float4 _ReplaceColor3;
            float _Threshold;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                float alpha = col.a;

                // Check the RGB channels only, preserving the alpha channel
                if (all(abs(col.rgb - _Color1.rgb) < _Threshold))
                    col.rgb = _ReplaceColor1.rgb;
                else if (all(abs(col.rgb - _Color2.rgb) < _Threshold))
                    col.rgb = _ReplaceColor2.rgb;
                else if (all(abs(col.rgb - _Color3.rgb) < _Threshold))
                    col.rgb = _ReplaceColor3.rgb;

                col.a = alpha;

                // Return the modified color while preserving the original alpha
                return col;
            }
            ENDCG
        }
    }
}
