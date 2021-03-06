//*****************************************************************************
// Torque -- HLSL procedural shader
//*****************************************************************************

// Dependencies:
#include "shaders/common/terrain/terrain.hlsl"
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
// Terrain Empty
// Terrain Base Texture
// Terrain Detail Texture 0
// Deferred RT Lighting

struct ConnectData
{
   float tcEmpty         : TEXCOORD0;
   float2 texCoord        : TEXCOORD1;
   float3 detCoord0       : TEXCOORD2;
   float4 screenspacePos  : TEXCOORD3;
};


struct Fragout
{
   float4 col : COLOR0;
};


//-----------------------------------------------------------------------------
// Main
//-----------------------------------------------------------------------------
Fragout main( ConnectData IN,
              uniform sampler2D baseTexMap      : register(S0),
              uniform sampler2D layerTex        : register(S1),
              uniform float     layerSize       : register(C0),
              uniform float3    detailIdStrengthParallax0 : register(C1),
              uniform sampler2D detailMap0      : register(S2),
              uniform float4    renderTargetParams : register(C2),
              uniform sampler2D lightInfoBuffer : register(S3)
)
{
   Fragout OUT;

   // Vert Position
   
   // Terrain Empty
   clip( IN.tcEmpty + 0.999 );
   
   // Terrain Base Texture
   float4 baseColor = tex2D( baseTexMap, IN.texCoord );
   OUT.col = baseColor;
   
   // Terrain Detail Texture 0
   float4 layerSample = round( tex2D( layerTex, IN.texCoord ) * 255.0f );
   float detailBlend0 = calcBlend( detailIdStrengthParallax0.x, IN.texCoord.xy, layerSize, layerSample );
   float blendTotal = 0;
   blendTotal += detailBlend0;
   float4 detailColor;
   {
      detailColor = ( tex2D( detailMap0, IN.detCoord0.xy ) * 2.0 ) - 1.0;
      detailColor *= detailIdStrengthParallax0.y * IN.detCoord0.z;
      OUT.col = lerp( OUT.col, baseColor + detailColor, detailBlend0 );
   }
   
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
   

   return OUT;
}
