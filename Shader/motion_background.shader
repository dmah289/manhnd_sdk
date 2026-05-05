Shader "manhnd_sdk/motion_background"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorPattern("Color Pattern", Color) = (1,1,1,1)
        _Speed("Speed", float) = 1
        _Angle("Angle", Range(0,360)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Background"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct meshdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolator
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _Angle;
            fixed4 _ColorPattern;

            interpolator vert(meshdata v)
            {
                interpolator i;
                i.vertex = UnityObjectToClipPos(v.vertex);
                
                i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                i.uv.x *= _ScreenParams.x / _ScreenParams.y;
                
                i.uv += frac(float2(_Time.y * _Speed * sin(radians(_Angle)), _Time.y * _Speed * cos(radians(_Angle))));
                
                return i;
            }

            fixed4 frag(interpolator i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, frac(i.uv));
                col *= _ColorPattern;
                return col;
            }
            ENDCG
        }
    }
}