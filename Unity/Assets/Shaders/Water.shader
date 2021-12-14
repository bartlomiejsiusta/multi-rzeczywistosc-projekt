Shader "Custom/Water"
{
	Properties
	{
		_WaveHeight ("Wave height", float) = 0.1
		_WaveSpeed ("Wave speed", float) = 1.0
		_WavePeriod ("Wave period", float) = 1.0
		_WaterColor ("Water Color", Color) = (0, 0, 1, 1)
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface Surf Standard fullforwardshadows vertex:Vert

		#pragma target 3.0

		struct Input
		{
			float3 worldPos;
		};

		fixed _WaveHeight;
		fixed _WaveSpeed;
		fixed _WavePeriod;
		fixed4 _WaterColor;

		inline float2 Hash22(float2 uv, float offset)
		{
			float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
			uv = frac(sin(mul(uv, m)) * 46839.32);
			return float2(sin(uv.y * offset) * 0.5 + 0.5, cos(uv.x * offset) * 0.5 + 0.5);
		}

		float Voronoi(float2 uv, float angleOffset)
		{
			float2 g = floor(uv);
			float2 f = frac(uv);

			float ret = 8.0;

			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					float2 lattice = float2(x, y);
					float2 offset = Hash22(lattice + g, angleOffset);
					float d = distance(lattice + offset, f);
					if (d < ret)
					{
						ret = d;
					}
				}
			}

			return ret;
		}

		/* struct appdata_full
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		}; */

		void Vert(inout appdata_full v)
		{
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			v.vertex.y += _WaveHeight * sin(worldPos.x * _WaveSpeed + _Time.y * _WavePeriod);
		}

		/* struct SurfaceOutputStandard
		{
			fixed3 Albedo;
			fixed3 Normal; // tangent space
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
		}; */

		void Surf(Input IN, inout SurfaceOutputStandard o)
		{
			float v = pow(Voronoi(IN.worldPos.xz * 0.5 + float2(_Time.y * 0.1, 0.0), _Time.x) * 0.6, 2.0);

			o.Albedo = _WaterColor.rgb + float3(v, v, v);
			//o.Albedo = round(o.Albedo * 32.0) / 32.0;
			o.Metallic = 0.0;
			o.Smoothness = 0.5;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
