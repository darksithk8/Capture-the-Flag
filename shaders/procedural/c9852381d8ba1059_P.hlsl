//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Features:
// Vert Position
// Eye Space Depth (Out)

struct ConnectData
{
   float4 wsEyeVec        : TEXCOORD0;
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
   
   // Eye Space Depth (Out)
   float eyeSpaceDepth = dot(vEye, (IN.wsEyeVec.xyz / IN.wsEyeVec.w));
   OUT.col = eyeSpaceDepth;
   

   return OUT;
}
