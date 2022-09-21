Shader "GParticles/Simple"
{
    Properties
    {
        [HDR]_Color ("Color", COLOR) = (1,1,1,1)
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _Size ("Size", FLOAT) = 0.01
        [Space]
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc ("Blend Src", FLOAT) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendDst ("Blend Dst", FLOAT) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        Blend [_BlendSrc] [_BlendDst]
        ZWrite off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            // #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : TEXCOORD1;
            };

            sampler2D _MainTex;
            half4 _Color;
            float _Size;
            
            float deltaTime;
            struct InstanceData
            {
                float3 position;
                float3 velocity;
                float4 color;
            };
            StructuredBuffer<InstanceData> _Particles;

            v2f vert (appdata v, uint instanceID : SV_InstanceID)
            {
                v2f o;
                InstanceData pt = _Particles[instanceID];
                float4 pos = mul(UNITY_MATRIX_V, float4(pt.position, 1));
                pos.xy += (v.uv-0.5)*_Size;

                o.vertex = mul(UNITY_MATRIX_P, pos);
                o.color = _Color * pt.color;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = i.color * tex2D(_MainTex, i.uv);
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
                //return float4(0.1,0.1,0.1, 1);
            }
            ENDCG
        }
    }
}
