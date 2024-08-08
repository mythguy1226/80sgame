Shader "Hidden/BlackAndWhite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("ColorOverlay", Color) = (1, 1, 1, 1)
        _Amount ("PreserveBase", Range(0, 1)) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
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
             
            sampler2D _MainTex;
            fixed4 _Color;
            float _Amount;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float val = (col.r + col.g + col.b) / 3;
                // just invert the colors
                col = ((col * _Amount) + (fixed4(val, val, val, col.a) * (1-_Amount))) * fixed4(_Color.r, _Color.g, _Color.b, 1); 
                return col;
            }
            ENDCG
        }
    }
}
