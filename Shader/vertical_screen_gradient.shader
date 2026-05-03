Shader "manhnd_sdk/vertical_screen_gradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1,1,1,1)
        _BottomColor ("Bottom Color", Color) = (0,0,0,1)
        _Scale ("Scale", Vector) = (1,1,0,0)
        _Pow ("Power (Blend Smoothness)", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Background"
            "IgnoreProjector"="True"
            "RenderType"="Background"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
            };

            struct v2f
            {
                fixed3 scr : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
            };

            fixed4 _TopColor, _BottomColor;
            fixed4 _Scale;
            fixed _Pow;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scr = ComputeScreenPos(o.vertex).xyw;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uvScreen = i.scr.xy / i.scr.z;
                uvScreen = uvScreen * _Scale.xy + _Scale.zw;
                fixed4 col = 1;
                col.rgb = lerp(_BottomColor.rgb, _TopColor.rgb, pow(saturate(uvScreen.y), _Pow));
                
                return col;
            }
            ENDCG
        }
    }
}