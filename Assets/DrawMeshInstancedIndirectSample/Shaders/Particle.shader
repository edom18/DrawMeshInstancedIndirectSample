Shader "Unlit/Particle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
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
                float3 position;
                float3 normal;
                float4 color;
                float scale;
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            StructuredBuffer<Particle> _ParticleBuffer;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v, uint instanceId : SV_InstanceID)
            {
                Particle p = _ParticleBuffer[instanceId];

                v2f o;

                float3 pos = (v.vertex.xyz * p.scale) + p.position;
                o.vertex = mul(UNITY_MATRIX_VP, float4(pos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
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
