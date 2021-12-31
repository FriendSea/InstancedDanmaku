Shader "Hidden/Bullets/CollisionPreview"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        LOD 100

        Pass
        {
            ZTest Off

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv*2-1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                clip(1.0-length(i.uv));
                clip(length(i.uv) - (1 - ddx(i.uv.x) * 5));
                return fixed4(0,1,0,1);
            }
            ENDCG
        }
    }
}
