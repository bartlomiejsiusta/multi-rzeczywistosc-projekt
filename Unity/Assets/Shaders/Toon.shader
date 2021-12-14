Shader "Custom/Toon"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Gloss ("Gloss", Range(0, 1)) = 0.5
		_Specular ("Specular", float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}

		CGPROGRAM
		#pragma surface Surf Toon fullforwardshadows

		#pragma target 3.0

		#include "ToonLighting.cginc"

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		fixed _Gloss;
		half _Specular;
		fixed4 _Color;

		void Surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Specular = _Specular;
			o.Gloss = _Gloss;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
