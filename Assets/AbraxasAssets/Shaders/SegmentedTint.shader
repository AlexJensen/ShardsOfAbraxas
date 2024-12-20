Shader "Custom/TwelveSegmentTintShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            // Use uniform arrays to pass colors and segment weights
            fixed4 _TintColors[12];
            float _Segments[12];

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
                float2 uv = i.uv;
                float4 texColor = tex2D(_MainTex, uv);

                // Calculate cumulative segments
                float cumulativeSegment = 0.0;

                // Determine which segment the current pixel falls into
                float cumulativePosition = uv.x;

                for (int k = 0; k < 12; k++)
                {
                    cumulativeSegment += _Segments[k];
                    if (cumulativePosition <= cumulativeSegment)
                    {
                        texColor.rgb *= _TintColors[k].rgb; // Apply tint based on segment
                        break;
                    }
                }

                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
