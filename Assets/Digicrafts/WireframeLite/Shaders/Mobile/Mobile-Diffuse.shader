// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Digicrafts/Wireframe/Mobile/Diffuse" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}

	// Wireframe Properties
	[HDR]_WireframeColor ("Color", Color) = (1,1,1,1)
	_WireframeTex ("Texture", 2D) = "white" {}
	[Enum(UV0,0,UV1,1)] _WireframeUV ("UV Set for Texture", Float) = 0
	_WireframeSize ("Size", Range(0.0, 10.0)) = 1
	[Toggle(_WIREFRAME_LIGHTING)]_WireframeLighting ("Color affect by Light", Float) = 0
	[Toggle(_WIREFRAME_AA)]_WireframeAA ("Anti Aliasing", Float) = 0
	[Toggle]_WireframeDoubleSided ("2 Sided", Float) = 0
	_WireframeMaskTex ("Mask Texture", 2D) = "white" {}
	_WireframeTexAniSpeedX ("Speed X", Float) = 0
	_WireframeTexAniSpeedY ("Speed Y", Float) = 0
	[HDR]_WireframeEmissionColor("Color", Color) = (0,0,0)

	[HideInInspector] _WireframeAlphaMode ("__WireframeAlphaMode", Float) = 0
	[HideInInspector] _WireframeCull ("__WireframeCull", Float) = 2

}
SubShader {

Tags { "RenderType"="Opaque" }
LOD 150
Cull [_WireframeCull]
CGPROGRAM
#pragma surface surf SimpleLambert noforwardadd vertex:vert //alpha
#pragma target 3.0
#pragma shader_feature _WIREFRAME_LIGHTING
#pragma shader_feature _WIREFRAME_AA
#pragma shader_feature _WIREFRAME_ALPHA_NORMAL _WIREFRAME_ALPHA_TEX_ALPHA _WIREFRAME_ALPHA_TEX_ALPHA_INVERT _WIREFRAME_ALPHA_MASK

#include "../Core/WireframeCore.cginc"
#include "../Core/WireframeCoreMobile.cginc"

uniform sampler2D _MainTex;

struct Input {	
	DC_WIREFRAME_COORDS_MOBILE
};

half4 LightingSimpleLambert (SurfaceOutput_t s, half3 lightDir, half atten) {
  	half NdotL = dot (s.Normal, lightDir);
  	half4 c;  
	#if _WIREFRAME_LIGHTING
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
	#else
		c.rgb = lerp(s.Albedo * _LightColor0.rgb * (NdotL * atten),s.Base,s.w);	
	#endif
  	c.a = s.Alpha;
  	return c;
}

void vert (inout appdata_full_t v, out Input o) {	
	UNITY_INITIALIZE_OUTPUT(Input,o);
	DC_WIREFRAME_TRANSFER_COORDS_MOBILE(o)
}

void surf (Input i, inout SurfaceOutput_t o) {
	fixed4 c = tex2D(_MainTex, i.uv_MainTex);
	DC_APPLY_WIREFRAME_MOBILE(c.rgb,c.a,i,w);
	#if _WIREFRAME_LIGHTING
		o.Albedo = c.rgb;
	#else
		o.Base=c.rgb;
		o.w = w;
		o.Albedo = lerp(c.rgb,fixed3(0,0,0),w);
	#endif
	o.Emission = _WireframeEmissionColor*w;
	o.Alpha = c.a;	
}
ENDCG

}
Fallback "Mobile/VertexLit"
CustomEditor "WireframeGeneralShaderGUI"
}
