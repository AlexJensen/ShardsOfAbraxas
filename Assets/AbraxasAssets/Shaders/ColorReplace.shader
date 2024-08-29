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

        _Color4 ("Color to Replace 4", Color) = (0,0.5,0,1)
        _ReplaceColor4 ("Replacement Color 4", Color) = (0.5,0,1,1)

        _Color5 ("Color to Replace 5", Color) = (0,0,0.5,1)
        _ReplaceColor5 ("Replacement Color 5", Color) = (1,0.5,0,1)

        _Threshold1 ("Threshold 1", Range(0, 1)) = 0.00
        _Threshold2 ("Threshold 2", Range(0, 1)) = 0.00
        _Threshold3 ("Threshold 3", Range(0, 1)) = 0.00
        _Threshold4 ("Threshold 4", Range(0, 1)) = 0.00
        _Threshold5 ("Threshold 5", Range(0, 1)) = 0.00
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
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
            float4 _Color4;
            float4 _ReplaceColor4;
            float4 _Color5;
            float4 _ReplaceColor5;
            float _Threshold1, _Threshold2, _Threshold3, _Threshold4, _Threshold5;

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

                // Replace colors if they match within the threshold
                if (_Color1.a != 0 && all(abs(col.rgb - _Color1.rgb) < _Threshold1))
                    col.rgb = _ReplaceColor1.rgb;
                else if (_Color2.a != 0 && all(abs(col.rgb - _Color2.rgb) < _Threshold2))
                    col.rgb = _ReplaceColor2.rgb;
                else if (_Color3.a != 0 && all(abs(col.rgb - _Color3.rgb) < _Threshold3))
                    col.rgb = _ReplaceColor3.rgb;
                else if (_Color4.a != 0 && all(abs(col.rgb - _Color4.rgb) < _Threshold4))
                    col.rgb = _ReplaceColor4.rgb;
                else if (_Color5.a != 0 && all(abs(col.rgb - _Color5.rgb) < _Threshold5))
                    col.rgb = _ReplaceColor5.rgb;

                col.a = alpha;

                return col;
            }
            ENDCG
        }
    }
}
