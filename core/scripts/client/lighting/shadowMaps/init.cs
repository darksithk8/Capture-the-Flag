//---------------------------------------------------------------------------------------------
// Torque 3D
// Copyright (C) GarageGames.com, Inc.
//---------------------------------------------------------------------------------------------


new ShaderData(BlurDepthShader)
{
   DXVertexShaderFile = "shaders/common/lighting/shadowMap/boxFilterV.hlsl";
   DXPixelShaderFile  = "shaders/common/lighting/shadowMap/boxFilterP.hlsl";
   
   OGLVertexShaderFile = "shaders/common/lighting/shadowMap/gl/boxFilterV.glsl";
   OGLPixelShaderFile = "shaders/common/lighting/shadowMap/gl/boxFilterP.glsl";
   pixVersion = 2.0;
};
