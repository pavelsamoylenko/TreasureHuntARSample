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
            float4 _BaseMap_TexelSize;
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
                float4 positionOS : TEXCOORD2;
            };
            
            Varyings Vertex(Attributes input) 
            {
                Varyings output = (Varyings) 0;
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv * 2 - 1;
                float3 fakePositionOS = float3(output.uv.x, 0, output.uv.y);
                fakePositionOS.x *= _BaseMap_TexelSize.z / _BaseMap_TexelSize.w;
                // output.positionOS.xyz = fakePositionOS.xyz;
                output.positionOS.xyz = input.positionOS.xyz;
                output.projection = ComputeScreenPos(TransformObjectToHClip(fakePositionOS));

                return output;
            }
            
            half4 Fragment(Varyings input) : SV_Target
            {
                // return half4(input.positionOS.xz, 0, 1);
                float2 screenUV = input.projection.xy / input.projection.w;
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, screenUV);
                //return half4(screenUV, 0, 1);
                return color;
            }
        
        
            ENDHLSL
    
        }
    }
}
