//-----------------------------------------------------------------------------
// Torque 3D
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------



//-----------------------------------------------------------------------------
// Water
//-----------------------------------------------------------------------------

singleton ShaderData( WaterShader )
{
   DXVertexShaderFile 	= "shaders/common/water/waterV.hlsl";
   DXPixelShaderFile 	= "shaders/common/water/waterP.hlsl";
   
   OGLVertexShaderFile = "shaders/common/water/gl/waterV.glsl";
   OGLPixelShaderFile = "shaders/common/water/gl/waterP.glsl";
   
   samplerNames[0] = "bumpMap";
   samplerNames[1] = "prepassTex";
   samplerNames[2] = "reflectMap";
   samplerNames[3] = "refractBuff";
   samplerNames[4] = "skyMap";
   samplerNames[5] = "foamMap";
   
   pixVersion = 3.0;
};

singleton GFXStateBlockData( WaterStateBlock )
{
   samplersDefined = true;
   samplerStates[0] = SamplerWrapLinear;  // noise
   samplerStates[1] = SamplerClampPoint;  // #prepass
   samplerStates[2] = SamplerClampPoint;  // $reflectbuff
   samplerStates[3] = SamplerClampPoint;  // $backbuff
   samplerStates[4] = SamplerWrapLinear;  // $cubemap   
   samplerStates[5] = SamplerWrapLinear;  // foam      
   cullDefined = true;
   cullMode = "GFXCullNone";
};

singleton CustomMaterial( Water )
{
   // These samplers are set in code not here.
   // This is to allow different WaterObject instances
   // to use this same material but override these textures
   // per instance.   
   //texture[0] = "core/art/water/noise02";
   //texture[5] = "core/art/water/foam";
   //texture[4] = "$cubemap";   
   
   texture[1] = "#prepass";   
   texture[2] = "$reflectbuff";
   texture[3] = "$backbuff";   
   
   cubemap = NewLevelSkyCubemap;
   shader = WaterShader;
   stateBlock = WaterStateBlock;
   specular = "0.75 0.75 0.75 1.0";
   specularPower = 48.0;
   version = 3.0;
};

//-----------------------------------------------------------------------------
// Underwater
//-----------------------------------------------------------------------------

singleton ShaderData( UnderWaterShader )
{
   DXVertexShaderFile 	= "shaders/common/water/waterV.hlsl";
   DXPixelShaderFile 	= "shaders/common/water/waterP.hlsl";   
   
   OGLVertexShaderFile 	= "shaders/common/water/gl/waterV.glsl";
   OGLPixelShaderFile 	= "shaders/common/water/gl/waterP.glsl"; 
   
   samplerNames[0] = "bumpMap";
   samplerNames[1] = "prepassTex";
   samplerNames[2] = "reflectMap";
   samplerNames[3] = "refractBuff";
   samplerNames[4] = "skyMap";
   samplerNames[5] = "foamMap";  
   
   defines = "UNDERWATER";   
   pixVersion = 3.0;
};

singleton CustomMaterial( Underwater )
{  
   // These samplers are set in code not here.
   // This is to allow different WaterObject instances
   // to use this same material but override these textures
   // per instance.   
   //texture[0] = "core/art/water/noise02";
   //texture[4] = "$cubemap";  

   texture[1] = "#prepass";
   texture[3] = "$backbuff";   
   
   shader = UnderWaterShader;
   stateBlock = WaterStateBlock;
   specular = "0.75 0.75 0.75 1.0";
   specularPower = 48.0;
   version = 3.0;
};

//-----------------------------------------------------------------------------
// Basic Water
//-----------------------------------------------------------------------------

singleton ShaderData( WaterBasicShader )
{
   DXVertexShaderFile 	= "shaders/common/water/waterBasicV.hlsl";
   DXPixelShaderFile 	= "shaders/common/water/waterBasicP.hlsl";
   
   OGLVertexShaderFile 	= "shaders/common/water/gl/waterBasicV.glsl";
   OGLPixelShaderFile 	= "shaders/common/water/gl/waterBasicP.glsl"; 
   
   samplerNames[0] = "$bumpMap";
   //samplerNames[1] = "$prepassTex";
   samplerNames[2] = "$reflectMap";
   samplerNames[3] = "$refractBuff";
   samplerNames[4] = "$skyMap";
   //samplerNames[5] = "$foamMap";
   
   pixVersion = 2.0;
};

singleton GFXStateBlockData( WaterBasicStateBlock )
{
   samplersDefined = true;
   samplerStates[0] = SamplerWrapLinear;  // noise
   samplerStates[2] = SamplerClampPoint;  // $reflectbuff
   samplerStates[3] = SamplerClampPoint;  // $backbuff
   samplerStates[4] = SamplerWrapLinear;  // $cubemap
   cullDefined = true;
   cullMode = "GFXCullNone";
};

singleton CustomMaterial( WaterBasic )
{
   // These samplers are set in code not here.
   // This is to allow different WaterObject instances
   // to use this same material but override these textures
   // per instance.     
   //texture[0] = "core/art/water/noise02";
   //texture[5] = "core/art/water/foam";
   //texture[4] = "$cubemap";   
   
   //texture[1] = "#prepass";   
   texture[2] = "$reflectbuff";
   texture[3] = "$backbuff";   
   texture[4] = "$cubemap";
    
   cubemap = NewLevelSkyCubemap;
   shader = WaterBasicShader;
   stateBlock = WaterBasicStateBlock;
   version = 2.0;
};

//-----------------------------------------------------------------------------
// Basic UnderWater
//-----------------------------------------------------------------------------

singleton ShaderData( UnderWaterBasicShader )
{
   DXVertexShaderFile 	= "shaders/common/water/waterBasicV.hlsl";
   DXPixelShaderFile 	= "shaders/common/water/waterBasicP.hlsl";   
   
   OGLVertexShaderFile 	= "shaders/common/water/gl/waterBasicV.glsl";
   OGLPixelShaderFile 	= "shaders/common/water/gl/waterBasicP.glsl";
   
   samplerNames[0] = "$bumpMap";
   //samplerNames[1] = "$prepassTex";
   samplerNames[2] = "$reflectMap";
   samplerNames[3] = "$refractBuff";
   samplerNames[4] = "$skyMap";
   //samplerNames[5] = "$foamMap"; 
   
   defines = "UNDERWATER";   
   pixVersion = 2.0;
};

singleton CustomMaterial( UnderwaterBasic )
{
   // These samplers are set in code not here.
   // This is to allow different WaterObject instances
   // to use this same material but override these textures
   // per instance.  
   //texture[0] = "core/art/water/noise02";
   //texture[4] = "$cubemap";  

   //texture[1] = "#prepass";
   texture[3] = "$backbuff";
   
   shader = UnderWaterBasicShader;
   stateBlock = WaterBasicStateBlock;
   version = 2.0;
};