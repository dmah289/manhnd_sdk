Shader "manhnd_sdk/radial_gradient"
{
    // Using fixed UV in screen space instead of uv data of mesh
    Properties
    {
        _OuterColor("Gradient Color 1", Color) = (0,0,0,1)
        _InnerColor("Gradient Color 2", Color) = (1,1,1,1)
        _Scale("Scale", Vector) = (1,1,0,0)
        _Pow("Pow", Float) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                fixed3 scr : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed3 _OuterColor, _InnerColor;
            fixed4 _Scale;
            fixed _Pow;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scr = ComputeScreenPos(o.vertex).xyw;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed2 uv_scr = i.scr.xy / i.scr.z;
                uv_scr -= 0.5;
                
                uv_scr.x *= _ScreenParams.x / _ScreenParams.y;
                uv_scr = uv_scr * _Scale.xy + _Scale.zw;
                
                uv_scr += 0.5;
                
                fixed4 col = 1;
                col.rgb = lerp(_OuterColor, _InnerColor, pow(saturate(distance(0.5, uv_scr)), _Pow));
                return col;
            }
            ENDCG
        }
    }
}