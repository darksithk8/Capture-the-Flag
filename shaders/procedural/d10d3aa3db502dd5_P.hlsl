//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Dependencies:
#include "shaders/common/torque.hlsl"

// Features:
// Vert Position
// Reflect Cube
// Visibility
// HDR Output

struct ConnectData
{
   float3 reflectVec      : TEXCOORD0;
};


struct Fragout
{
   float4 col : COLOR0;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
Fragout main( ConnectData IN,
              uniform samplerCUBE cubeMap         : register(S0),
              uniform float     visibility      : register(C0)
)
{
   Fragout OUT;

   // Vert Position
   
   // Reflect Cube
   OUT.col = texCUBE( cubeMap, IN.reflectVec );
   
   // Visibility
   OUT.col.w *= visibility;
   
   // HDR Output
   hdrEncode( OUT.col );
   

   return OUT;
}
