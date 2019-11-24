Shader "Unlit/Particle"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Particle
            {
                float3 basePosition;
                float3 position;
                float4 color;
                float scale;
            };

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            StructuredBuffer<Particle> _ParticleBuffer;

            v2f vert (appdata v, uint instanceId : SV_InstanceID)
            {
                Particle p = _ParticleBuffer[instanceId];

                v2f o;

                float3 pos = (v.vertex.xyz * p.scale) + p.position;
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0));
                o.color = p.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }
    }
}
