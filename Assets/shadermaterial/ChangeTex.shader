Shader "Unlit/ChangeTex"
{
    Properties
    {
        _Power("Power", Range(0,1.05)) = 0
         _Noise("Noise", 2D) = "white" {}
        _MainTex("After", 2D) = "white" {}
        _Color("After", Color) = (1,0.5,0.5,1)
        _OtherColor("Before", Color) = (1,0.5,0.5,1)
    }

    SubShader
    {
        Cull Off
        Tags { "Queue"="Transparent" }
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
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 wPos: TEXCOORD1;
                float wNormal : TEXTCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Noise;
            sampler2D _Other;
            float4 _MainTex_ST;
            float _Power;
            fixed4 _Color;
            fixed4 _OtherColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            float4 col = tex2D(_MainTex, i.uv)*_Color;
            float4 colother = tex2D(_MainTex, i.uv) * _OtherColor;

            float val = tex2D(_Noise, i.uv).r;
            float4 colour = lerp(col, colother, step(_Power, val));
            return colour;
            }
            ENDCG
        }
    }
}
