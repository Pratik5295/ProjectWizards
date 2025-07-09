Shader "Custom/OutlineFill"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 20)) = 5
    }

    SubShader
    {
        Tags { 
            "Queue" = "Transparent+100"
            "RenderType" = "Transparent"
            "ForceNoShadowCasting" = "True"
            "DisableBatching" = "True"  // 禁用批处理确保轮廓稳定
        }
        
        Pass
        {
            Name "OUTLINE"
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            
            Stencil {
                Ref 1
                Comp NotEqual
                Pass Keep
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 smoothNormal : TEXCOORD3;  
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _OutlineWidth)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _OutlineColor)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                float outlineWidth = UNITY_ACCESS_INSTANCED_PROP(Props, _OutlineWidth);
                fixed4 outlineColor = UNITY_ACCESS_INSTANCED_PROP(Props, _OutlineColor);
                
                
                float3 normal = length(v.smoothNormal) > 0 ? v.smoothNormal : v.normal;
                
                
                float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, normal));
                
                
                float4 pos = UnityObjectToClipPos(v.vertex);
                float2 offset = TransformViewToProjection(viewNormal.xy);
                
                
                float perspectiveScale = 1.0 / max(1.0, pos.w);
                pos.xy += offset * outlineWidth * 0.001 * perspectiveScale;
                
                o.pos = pos;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _OutlineColor);
            }
            ENDCG
        }
    }
    Fallback Off
}