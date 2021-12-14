/* struct SurfaceOutput
{
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
};

struct UnityLight
{
	half3 color;
	half3 dir;
	half ndotl; // Deprecated
};

struct UnityIndirect
{
	half3 diffuse;
	half3 specular;
};

struct UnityGI
{
	UnityLight light;
	UnityIndirect indirect;
}; */

inline half4 LightingToon(SurfaceOutput s, half3 viewDir, UnityGI gi)
{
	half3 h = normalize(gi.light.dir + viewDir);

	fixed diff = max(0, dot(s.Normal, gi.light.dir));
	diff = round(diff * 4.0) / 4.0;

	float nh = max(0, dot(s.Normal, h));
	float spec = pow(nh, s.Specular * 128.0) * s.Gloss;

	fixed4 c;
	c.rgb = s.Albedo * gi.light.color * diff + gi.light.color * _SpecColor.rgb * spec;
	c.a = s.Alpha;

	#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
	c.rgb += s.Albedo * gi.indirect.diffuse;
	#endif
	return c;
}

inline void LightingToon_GI(SurfaceOutput s, UnityGIInput data, inout UnityGI gi)
{
	gi = UnityGlobalIllumination(data, 1.0, s.Normal);
}
