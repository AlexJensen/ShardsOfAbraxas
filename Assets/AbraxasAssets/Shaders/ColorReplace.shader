Shader "Custom/ColorReplace2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _Color1 ("Color to Replace 1", Color) = (1,0,0,1)
        _ReplaceColor1 ("Replacement Color 1", Color) = (0,1,0,1)
        _Threshold1 ("Threshold 1", Range(0, 1)) = 0.1

        _Color2 ("Color to Replace 2", Color) = (1,0,0.5,1)
        _ReplaceColor2 ("Replacement Color 2", Color) = (0,0.5,1,1)
        _Threshold2 ("Threshold 2", Range(0, 1)) = 0.1

        _Color3 ("Color to Replace 3", Color) = (0.5,0,0,1)
        _ReplaceColor3 ("Replacement Color 3", Color) = (1,0,1,1)
        _Threshold3 ("Threshold 3", Range(0, 1)) = 0.1

        _Color4 ("Color to Replace 4", Color) = (0,0.5,0,1)
        _ReplaceColor4 ("Replacement Color 4", Color) = (0.5,0,1,1)
        _Threshold4 ("Threshold 4", Range(0, 1)) = 0.1

        _Color5 ("Color to Replace 5", Color) = (0,0,0.5,1)
        _ReplaceColor5 ("Replacement Color 5", Color) = (1,0.5,0,1)
        _Threshold5 ("Threshold 5", Range(0, 1)) = 0.1

        _Color6 ("Color to Replace 6", Color) = (0.1,0.2,0.3,1)
        _ReplaceColor6 ("Replacement Color 6", Color) = (0.6,0.7,0.8,1)
        _Threshold6 ("Threshold 6", Range(0, 1)) = 0.1

        _Color7 ("Color to Replace 7", Color) = (0.2,0.3,0.4,1)
        _ReplaceColor7 ("Replacement Color 7", Color) = (0.7,0.8,0.9,1)
        _Threshold7 ("Threshold 7", Range(0, 1)) = 0.1

        _Color8 ("Color to Replace 8", Color) = (0.3,0.4,0.5,1)
        _ReplaceColor8 ("Replacement Color 8", Color) = (0.8,0.9,1,1)
        _Threshold8 ("Threshold 8", Range(0, 1)) = 0.1

        _Color9 ("Color to Replace 9", Color) = (0.4,0.5,0.6,1)
        _ReplaceColor9 ("Replacement Color 9", Color) = (0.9,1,0.7,1)
        _Threshold9 ("Threshold 9", Range(0, 1)) = 0.1

        _Color10 ("Color to Replace 10", Color) = (0.5,0.6,0.7,1)
        _ReplaceColor10 ("Replacement Color 10", Color) = (1,0.9,0.8,1)
        _Threshold10 ("Threshold 10", Range(0, 1)) = 0.1

        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };
            
            fixed4 _Color;
            fixed4 _Color1, _ReplaceColor1, _Color2, _ReplaceColor2, _Color3, _ReplaceColor3, _Color4, _ReplaceColor4, _Color5, _ReplaceColor5;
            fixed4 _Color6, _ReplaceColor6, _Color7, _ReplaceColor7, _Color8, _ReplaceColor8, _Color9, _ReplaceColor9, _Color10, _ReplaceColor10;
            float _Threshold1, _Threshold2, _Threshold3, _Threshold4, _Threshold5, _Threshold6, _Threshold7, _Threshold8, _Threshold9, _Threshold10;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

                if (_AlphaSplitEnabled)
                    color.a = tex2D (_AlphaTex, uv).r;

                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
                fixed4 originalColor = c;

                // Color replacement logic with individual thresholds
                if (distance(c.rgb, _Color1.rgb) < _Threshold1)
                    c.rgb = _ReplaceColor1.rgb;
                else if (distance(c.rgb, _Color2.rgb) < _Threshold2)
                    c.rgb = _ReplaceColor2.rgb;
                else if (distance(c.rgb, _Color3.rgb) < _Threshold3)
                    c.rgb = _ReplaceColor3.rgb;
                else if (distance(c.rgb, _Color4.rgb) < _Threshold4)
                    c.rgb = _ReplaceColor4.rgb;
                else if (distance(c.rgb, _Color5.rgb) < _Threshold5)
                    c.rgb = _ReplaceColor5.rgb;
                else if (distance(c.rgb, _Color6.rgb) < _Threshold6)
                    c.rgb = _ReplaceColor6.rgb;
                else if (distance(c.rgb, _Color7.rgb) < _Threshold7)
                    c.rgb = _ReplaceColor7.rgb;
                else if (distance(c.rgb, _Color8.rgb) < _Threshold8)
                    c.rgb = _ReplaceColor8.rgb;
                else if (distance(c.rgb, _Color9.rgb) < _Threshold9)
                    c.rgb = _ReplaceColor9.rgb;
                else if (distance(c.rgb, _Color10.rgb) < _Threshold10)
                    c.rgb = _ReplaceColor10.rgb;

                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}