// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VertexColorIndexer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "green" {}
		_Height ("Height", Int) = 1
		_Width ("Width", Int) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vertexPass
			#pragma fragment fragmentPass
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			int _Height;
			int _Width;

			struct VertexIn {
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct VertexToFragment {
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			VertexToFragment vertexPass(VertexIn v) {
				VertexToFragment o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				return o;
			}

			fixed4 fragmentPass(VertexToFragment i) : COLOR{
				float2 uv = float2((i.color.x + 1) / _Width, (i.color.y + 1) / _Height);
				return tex2D(_MainTex, uv);
			}
            ENDCG
        }
    }
}
