Shader "Custom/HPBarWithImageColor"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {} // Main texture of the sprite
        _HP ("Current HP", Float) = 1.0              // Current HP
        _MaxHP ("Max HP", Float) = 1.0              // Max HP
        _EmptyColor ("Empty Color", Color) = (1,0,0,1) // Color used for the empty part
        _FillColor ("Fill Color from Image", Color) = (1,1,1,1) // Color passed from the Image component
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

            sampler2D _MainTex;
            float _HP;
            float _MaxHP;
            float4 _EmptyColor;
            float4 _FillColor; // This will be passed in from the Image component's color

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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float hpPercent = clamp(_HP / _MaxHP, 0.0, 1.0); // Calculate percentage of HP

                // Sample the texture color
                float4 texColor = tex2D(_MainTex, i.uv);

                // Determine whether the current pixel should show fill or empty based on HP
                float lerpValue = step(i.uv.y, hpPercent);

                // Lerp between the empty color and the color from the Image component (_FillColor)
                float4 finalColor = lerp(_EmptyColor, _FillColor * texColor, lerpValue);

                // Maintain the original alpha
                return float4(finalColor.rgb, texColor.a);
            }
            ENDCG
        }
    }
}
