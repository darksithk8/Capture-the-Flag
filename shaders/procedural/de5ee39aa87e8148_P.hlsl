//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Dependencies:
#include "shaders/common/lighting.hlsl"
//------------------------------------------------------------------------------
// Autogenerated 'Light Buffer Conditioner [RGB]' Uncondition Method
//------------------------------------------------------------------------------
inline void autogenUncondition_bde4cbab(in float4 bufferSample, out float3 lightColor, out float NL_att, out float specular)
{
   lightColor = bufferSample.rgb;
   NL_att = dot(bufferSample.rgb, float3(0.3576, 0.7152, 0.1192));
   specular = max(bufferSample.a / NL_att, 0.00001f);
}



// Features:
// Vert Position
// Diffuse Color
// Deferred RT Lighting
// Visibility

struct ConnectData
{
   float4 screenspacePos  : TEXCOORD0;
};


struct Fragout
{
   float4 col : COLOR0;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
Fragout main( ConnectData IN,
              uniform float4    diffuseMaterialColor : register(C0),
              uniform float4    renderTargetParams : register(C1),
              uniform sampler2D lightInfoBuffer : register(S0),
              uniform float     visibility      : register(C2)
)
{
   Fragout OUT;

   // Vert Position
   
   // Diffuse Color
   OUT.col = diffuseMaterialColor;
   
   // Deferred RT Lighting
   float2 uvScene = IN.screenspacePos.xy / IN.screenspacePos.w;
   uvScene = ( uvScene + 1.0 ) / 2.0;
   uvScene.y = 1.0 - uvScene.y;
   uvScene = ( uvScene * renderTargetParams.zw ) + renderTargetParams.xy;
   float3 d_lightcolor;
   float d_NL_Att;
   float d_specular;
   lightinfoUncondition(tex2D(lightInfoBuffer, uvScene), d_lightcolor, d_NL_Att, d_specular);
   OUT.col *= float4(d_lightcolor, 1.0);
   
   // Visibility
   OUT.col.w *= visibility;
   

   return OUT;
}
