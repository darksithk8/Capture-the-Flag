//-----------------------------------------------------------------------------
// Torque 3D
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------


singleton ShaderData( TSImposterShaderData )
{
   DXVertexShaderFile   = "shaders/common/imposterV.hlsl";
   DXPixelShaderFile    = "shaders/common/imposterP.hlsl";
   pixVersion = 2.0;
};

singleton ShaderData( TSImposterPrePassShaderData )
{
   DXVertexShaderFile   = "shaders/common/imposterV.hlsl";
   DXPixelShaderFile    = "shaders/common/imposterP.hlsl";
        
   defines = "TORQUE_PREPASS";
   pixVersion = 2.0;
};
