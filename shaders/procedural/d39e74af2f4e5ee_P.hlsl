//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Features:
// Paraboloid Vert Transform
// Depth (Out)

struct ConnectData
{
   float2 posXY           : TEXCOORD0;
   float depth           : TEXCOORD1;
};


struct Fragout
{
   float4 col : COLOR0;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
Fragout main( ConnectData IN
)
{
   Fragout OUT;

   // Paraboloid Vert Transform
   clip( 1.0 - length( IN.posXY ) );
   
   // Depth (Out)
   OUT.col = float4( IN.depth, 0, 0, 1 );
   

   return OUT;
}
