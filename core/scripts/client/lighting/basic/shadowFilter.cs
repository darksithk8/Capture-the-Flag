//-----------------------------------------------------------------------------
// Torque 3D
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Projected Shadow Filter
//------------------------------------------------------------------------------

singleton ShaderData( BL_ShadowFilterShader )
{   
   DXVertexShaderFile 	= "core/scripts/client/lighting/basic/shadowFilterV.hlsl";
   DXPixelShaderFile 	= "core/scripts/client/lighting/basic/shadowFilterP.hlsl";
   
   OGLVertexShaderFile 	= "core/scripts/client/lighting/basic/shadowFilterV.glsl";
   OGLPixelShaderFile 	= "core/scripts/client/lighting/basic/shadowFilterP.glsl";

   samplerNames[0] = "$diffuseMap";
   
   pixVersion = 2.0;     
};

singleton GFXStateBlockData( BL_ShadowFilterSB : PFX_DefaultStateBlock )
{
   blendDefined = true;
   blendEnable = true;
};

// NOTE: This is ONLY used in Basic Lighting, and 
// only directly by the ProjectedShadow.  It is not 
// meant to be manually enabled like other PostEffects.
singleton PostEffect( BL_ShadowFilterPostFx )
{
   requirements = "";

   shader = BL_ShadowFilterShader;
   stateBlock = PFX_DefaultStateBlock;
   targetClear = "PFXTargetClear_OnDraw";
   targetClearColor = "0 0 0 0";
   texture[0] = "$inTex";
   target = "$outTex";   
};
