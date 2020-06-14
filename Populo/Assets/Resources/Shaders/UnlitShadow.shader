Shader "Custom/UnlitShadow"
{
	Properties{
	  _Color("Color", Color) = (0, 0, 0, 1.0)
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha
		fixed4 _Color;

		struct Input {
			float4 color : COLOR;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
		}
	FallBack "Diffuse"
}