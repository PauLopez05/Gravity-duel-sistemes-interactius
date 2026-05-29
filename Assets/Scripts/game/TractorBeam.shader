Shader "Custom/TractorBeam"
{
    Properties
    {
        [HDR] _Color ("Color del Rayo", Color) = (0, 1, 0, 1)
        _MainTex ("Textura Base (Opcional)", 2D) = "white" {}
        _ScrollSpeed ("Velocidad de Ondas", Float) = 2.0
        _WaveFrequency ("Cantidad de Ondas", Float) = 5.0
        _WaveExp ("Grosor/Nitidez de Ondas", Range(1.0, 20.0)) = 4.0
        _RimPower ("Brillo del Borde (Fresnel)", Range(0.5, 8.0)) = 3.0
    }
    SubShader
    {
        Tags 
        { 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
        }
        
        LOD 100
        Blend SrcAlpha One // Mezcla aditiva para que brille intensamente
        ZWrite Off
        Cull Off            // Renderiza por dentro y fuera del cono

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 viewDirWS    : TEXCOORD1;
                float3 normalWS     : NORMAL;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _ScrollSpeed;
                float _WaveFrequency;
                float _WaveExp;
                float _RimPower;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                
                output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // 1. Efecto Fresnel básico para silueta suave
                float rim = 1.0 - saturate(dot(normalize(input.normalWS), normalize(input.viewDirWS)));
                rim = pow(rim, _RimPower);

                // 2. Generación de Ondas Estilo Tu Diagrama (Arco/Anillos en el eje Y del cono)
                // Usamos frac() para repetir los anillos verticalmente a lo largo del cono
                float wavePattern = frac(input.uv.x * _WaveFrequency - _Time.y * _ScrollSpeed);
                
                // Exponencial para hacer las líneas más finas y estilizadas, como pulsos de energía
                float rings = pow(wavePattern, _WaveExp);

                // Opcional: Muestreo de textura por si quieres meter ruido de fondo
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                // 3. Combinación final: El fondo translúcido del cono + los anillos brillantes flotando
                float finalIntensity = rim * 0.4 + rings * 2.0;
                
                half4 finalColor = _Color * finalIntensity * texColor;
                
                // Aseguramos que mantenga transparencia alfa coherente
                finalColor.a = saturate(finalIntensity * _Color.a);
                
                return finalColor;
            }
            ENDHLSL
        }
    }
}