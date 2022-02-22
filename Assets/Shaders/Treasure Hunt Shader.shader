Shader "xSite/Treasure Hunt"
{
    Properties
    {
        _BaseMap ("Albedo (RGB)", 2D) = "white" {}
        _Scale("Scale", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex Vertex
            #pragma fragment Fragment

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float _Scale;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 projection : TEXCOORD1;
            };
            
            Varyings Vertex(Attributes input) 
            {
                Varyings output = (Varyings) 0;
                output.uv = input.uv;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                float3 a = float3(input.uv.x, input.uv.y, 0);
                a.xy = a.xy * 2 * _Scale - _Scale;
                output.projection = ComputeScreenPos(TransformObjectToHClip(a));
                return output;
            }
            
            half4 Fragment(Varyings input) : SV_Target
            {
                float2 screenUV = input.projection.xy / input.projection.w;
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, screenUV);
                //return half4(screenUV, 0, 1);
                return color;
            }
        
        
            ENDHLSL
    
        }
    }
}
