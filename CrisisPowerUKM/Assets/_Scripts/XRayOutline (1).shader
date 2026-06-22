Shader "Custom/XRayOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 0.005
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Overlay+1" }

        Pass
        {
            Name "XRayOutlinePass"

            Cull Front
            ZTest Always
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float  _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 expanded = v.vertex.xyz + v.normal * _OutlineWidth;
                o.pos = UnityObjectToClipPos(expanded);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
