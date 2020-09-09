#ifndef DC_WIREFRAME_CORE_MOBILE_INCLUDED
#define DC_WIREFRAME_CORE_MOBILE_INCLUDED

// Struct

struct appdata_full_t
{
     float4 vertex    : POSITION;  // The vertex position in model space.
     float3 normal    : NORMAL;    // The vertex normal in model space.
     float4 texcoord  : TEXCOORD0; // The first UV coordinate.
     float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
     float4 texcoord2 : TEXCOORD2; // The second UV coordinate.
     float2 uv4 	  : TEXCOORD3; // The second UV coordinate.
     float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
     float4 color     : COLOR;     // Per-vertex color
};

struct SurfaceOutput_t
{
     half3 Albedo;
     half3 Base;
     half3 Normal;
     half3 Emission;
     half Specular;
     half Gloss;
     half Alpha;
     float w;
};

// Marcos

#define DC_WIREFRAME_COORDS_MOBILE fixed4 mass;float2 uv_MainTex;float2 uv2_MainTex2;
#define DC_WIREFRAME_TRANSFER_COORDS_MOBILE(o) if(v.uv4.y==1.0)o.mass=float4(1,0,0,0);else if(v.uv4.y==2.0)o.mass=float4(0,1,0,0);else if(v.uv4.y==4.0)o.mass=float4(0,0,1,0);
#define DC_WIREFRAME_TRANSFER_TEX float2 wireframe_tex = TRANSFORM_TEX(((_WireframeUV==0)?i.uv_MainTex.xy:i.uv2_MainTex2.xy), _WireframeTex)+half2(_WireframeTexAniSpeedX,_WireframeTexAniSpeedY)*_Time.y;\
								float2 wireframeMask_tex = TRANSFORM_TEX(i.uv_MainTex.xy, _WireframeMaskTex);

#if _WIREFRAME_ALPHA_TEX_ALPHA
	#define DC_APPLY_WIREFRAME_MOBILE(col,alpha,i,w) DC_WIREFRAME_TRANSFER_TEX float w=DC_APPLY_WIREFRAME_COLOR_TEX(col,i.mass,wireframe_tex);
#elif _WIREFRAME_ALPHA_TEX_ALPHA_INVERT
	#define DC_APPLY_WIREFRAME_MOBILE(col,alpha,i,w) DC_WIREFRAME_TRANSFER_TEX float w=DC_APPLY_WIREFRAME_COLOR_TEX_ALPHA(col,i.mass,1-alpha,wireframe_tex);alpha+=w;
#elif _WIREFRAME_ALPHA_MASK
	#define DC_APPLY_WIREFRAME_MOBILE(col,alpha,i,w) DC_WIREFRAME_TRANSFER_TEX float w=DC_APPLY_WIREFRAME_COLOR_TEX_MASK(col,i.mass,wireframe_tex,wireframeMask_tex);alpha+=w;
#else	
	#define DC_APPLY_WIREFRAME_MOBILE(col,alpha,i,w) DC_WIREFRAME_TRANSFER_TEX float w=DC_APPLY_WIREFRAME_COLOR_TEX(col,i.mass,wireframe_tex);alpha+=w;
#endif

#endif //DC_WIREFRAME_CORE_INCLUDED