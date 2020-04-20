Shader "Hidden/Fiery"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gradient ("Texture", 2D) = "white" {}
        _FireAlpha ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            sampler2D _Gradient;
            sampler2D _FireAlpha;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
				fixed4 fireAlpha = tex2D(_FireAlpha, i.uv);
				if(fireAlpha.r > 0.01){
					fixed4 gradient = tex2D(_Gradient, float2(1-fireAlpha.r, 0));
					mainTex = fixed4(fireAlpha.r * gradient.rgb + (1-fireAlpha.r) * mainTex.rgb, 1);
					//mainTex = fireAlpha;
					//mainTex = fixed4(fireAlpha.a, fireAlpha.a, fireAlpha.a, 1);
				}
				
				return mainTex;
            }
            ENDCG
        }
    }
}
