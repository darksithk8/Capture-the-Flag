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

struct ConnectData
{
   float tcEmpty         : TEXCOORD0;
   float2 texCoord        : TEXCOORD1;
   float4 wsEyeVec        : TEXCOORD2;
   float3 gbNormal        : TEXCOORD3;
};


struct Fragout
{
   float4 col : COLOR0;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
Fragout main( ConnectData IN,
              uniform float3    vEye            : register(C0)
)
{
   Fragout OUT;

   // Vert Position
   
   // Terrain Empty
   clip( IN.tcEmpty + 0.999 );
   
   // Terrain Base Texture
   
   // Terrain Detail Texture 0
   
   // Eye Space Depth (Out)
   float eyeSpaceDepth = dot(vEye, (IN.wsEyeVec.xyz / IN.wsEyeVec.w));
   
   // GBuffer Conditioner
   float4 normal_depth = float4(normalize(IN.gbNormal), eyeSpaceDepth);

   // output buffer format: GFXFormatR16G16B16A16F
   // g-buffer conditioner: float4(normal.theta, normal.phi, depth Hi, depth Lo)
   float4 _gbConditionedOutput = float4(float2(atan2(normal_depth.y, normal_depth.x) / 3.14159265358979323846f, normal_depth.z), 0.0, normal_depth.a);
   if ( abs( dot( normal_depth.xyz, float3( 0.0, 0.0, 1.0 ) ) ) > 0.999f ) _gbConditionedOutput = float4( 0, 1 * sign( normal_depth.z ), 0, normal_depth.a );
   
   // Encode depth into hi/lo
   float3 _tempDepth = frac(normal_depth.a * float3(1.0, 65535.0, 4294836225.0));
   _gbConditionedOutput.zw = _tempDepth.xy - _tempDepth.yz * float2(1.0/65535.0, 1.0/65535.0);

   OUT.col = _gbConditionedOutput;
   

   return OUT;
}
