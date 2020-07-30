//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Dependencies:
#include "shaders/common/terrain/terrain.hlsl"

// Features:
// Vert Position
// Terrain Empty
// Terrain Base Texture
// Terrain Detail Texture 0
// Eye Space Depth (Out)
// GBuffer Conditioner

struct VertData
{
   float3 position        : POSITION;
   float3 normal          : NORMAL;
   float3 T               : TANGENT;
   float2 texCoord        : TEXCOORD0;
   float tcEmpty         : TEXCOORD1;
};


struct ConnectData
{
   float4 hpos            : POSITION;
   float outEmpty        : TEXCOORD0;
   float2 outTexCoord     : TEXCOORD1;
   float4 wsEyeVec        : TEXCOORD2;
   float3 gbNormal        : TEXCOORD3;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
ConnectData main( VertData IN,
                  uniform float4x4 modelview       : register(C0),
                  uniform float4x4 objTrans        : register(C4),
                  uniform float3   eyePosWorld     : register(C8)
)
{
   ConnectData OUT;

   // Vert Position
   OUT.hpos = mul(modelview, float4(IN.position.xyz,1));
   
   // Terrain Empty
   OUT.outEmpty = IN.tcEmpty;
   
   // Terrain Base Texture
   OUT.outTexCoord = IN.texCoord;
   
   // Terrain Detail Texture 0
   
   // Eye Space Depth (Out)
   OUT.wsEyeVec = mul(objTrans, float4(IN.position.xyz,1)) - float4(eyePosWorld, 0.0);
   
   // GBuffer Conditioner
   OUT.gbNormal = mul(objTrans, normalize(IN.normal));
   
   return OUT;
}
