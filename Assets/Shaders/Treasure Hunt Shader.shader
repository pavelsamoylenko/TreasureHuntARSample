Shader "xSite/Treasure Hunt"
{
    Properties
    {
        _BaseMap ("Albedo (RGB)", 2D) = "white" {}
        _Scale("Scale", float) = 0.5
        _UVCoefficient("UV Coeff", float) = 10
        _YCoefficient("Y Coeff", float) = 10
        _XZCoefficient("XZ Coeff", float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex Vertex
            #pragma fragment Fragment

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseMap_TexelSize;
            float _Scale;
            float _UVCoefficient;
            float _XZCoefficient;


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
                float4 positionOS : TEXCOORD2;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv * 2 - 1;
                float3 fakePositionOS = float3(output.uv.x, 0, output.uv.y);
                output.positionOS.xyz = input.positionOS.xyz;
                output.projection = ComputeScreenPos(TransformObjectToHClip(fakePositionOS));
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float scale_x = _XZCoefficient * _BaseMap_TexelSize.w /
                    _BaseMap_TexelSize.z;
                float scale_y = _UVCoefficient;
                float2 screenUV = input.projection.yx / input.projection.w;
                screenUV.x = (screenUV.x - 0.5) * scale_x + 0.5;
                screenUV.y = (screenUV.y - 0.5) * scale_y + 0.5;
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, screenUV);
                return color;
            }
            ENDHLSL

        }
    }
}