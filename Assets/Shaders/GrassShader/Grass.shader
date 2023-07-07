Shader "Custom/Grass" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.5
        _BendAmount ("Bend Amount", Range(0, 1)) = 0.5
        _MaxDistance ("Max Distance", Range(0, 100)) = 10
        _CameraPos ("Camera Position", Vector) = (0, 0, 0, 0)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float dist : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Cutoff;
            float _WindSpeed;
            float _WindStrength;
            float _BendAmount;
            float _MaxDistance;
            float3 _CameraPos;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Calculate the squared distance from the camera
                float3 viewPos = mul(unity_ObjectToWorld, v.vertex);
                float dist = length(viewPos - _CameraPos);
                o.dist = smoothstep(_MaxDistance, _MaxDistance * 0.9, dist);

                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float4 texColor = tex2D(_MainTex, i.uv) * _Color;
                float alpha = texColor.a;

                // Calculate wind effect
                float2 wind = _Time.y * _WindSpeed * float2(1, 0);
                float2 bend = _Time.y * _BendAmount * float2(0, 1);
                float2 offset = lerp(wind, bend, _WindStrength);
                float2 uv = i.uv + offset;

                // Apply alpha cutoff
                if (alpha < _Cutoff) discard;

                // Apply distance fade
                texColor.a *= i.dist;

                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
