//-----------------------------------------------------------------------------
// Torque 3D
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------


//------------------------------------------------------------------------------
// LightAnimData
//------------------------------------------------------------------------------

datablock LightAnimData( NullLightAnim )
{   
   animEnabled = false;
};

datablock LightAnimData( PulseLightAnim )
{   
   minBrightness = 0.0;
   maxBrightness = 1.0;
   
   flicker = false;
   chanceTurnOn = 0.0;
   chanceTurnOff = 0.0;
};

datablock LightAnimData( SubtlePulseLightAnim )
{
   minBrightness = 0.5;
   maxBrightness = 1.0;
   
   flicker = false; 
};

datablock LightAnimData( FlickerLightAnim )
{
   flicker = true;
   chanceTurnOn = 0.1;
   chanceTurnOff = 0.15;
};

datablock LightAnimData( BlinkLightAnim )
{
   flicker = true;
   chanceTurnOn = 0.5;
   chanceTurnOff = 0.02;
};


//------------------------------------------------------------------------------
// LightFlareData
//------------------------------------------------------------------------------

datablock LightFlareData( NullLightFlare )
{
   flareEnabled = false;
};

datablock LightFlareData( SunFlareExample )
{      
   overallScale = 4.0;
   flareEnabled = true;
   flareTexture = "./../special/lensFlareSheet0";  

   elementRect[0] = "512 0 512 512";
   elementDist[0] = 0.0;
   elementScale[0] = 2.0;
   elementTint[0] = "0.6 0.6 0.6";
   elementRotate[0] = true;
   elementUseLightColor[0] = true;
   
   elementRect[1] = "1152 0 128 128";
   elementDist[1] = 0.3;
   elementScale[1] = 0.7;
   elementTint[1] = "1.0 1.0 1.0";
   elementRotate[1] = true;
   elementUseLightColor[1] = true;
   
   elementRect[2] = "1024 0 128 128";
   elementDist[2] = 0.5;
   elementScale[2] = 0.25;   
   elementTint[2] = "1.0 1.0 1.0";
   elementRotate[2] = true;
   elementUseLightColor[2] = true;
   
   elementRect[3] = "1024 128 128 128";
   elementDist[3] = 0.8;
   elementScale[3] = 0.7;   
   elementTint[3] = "1.0 1.0 1.0";
   elementRotate[3] = true;
   elementUseLightColor[3] = true;
   
   elementRect[4] = "1024 0 128 128";
   elementDist[4] = 1.18;
   elementScale[4] = 0.5;   
   elementTint[4] = "1.0 1.0 1.0";
   elementRotate[4] = true;
   elementUseLightColor[4] = true;
   
   elementRect[5] = "1152 128 128 128";
   elementDist[5] = 1.25;
   elementScale[5] = 0.25;   
   elementTint[5] = "1.0 1.0 1.0";
   elementRotate[5] = true;
   elementUseLightColor[5] = true;
   
   elementRect[6] = "1024 0 128 128";
   elementDist[6] = 2.0;
   elementScale[6] = 0.25;      
   elementTint[6] = "1.0 1.0 1.0";
   elementRotate[6] = true;
   elementUseLightColor[6] = true;
};

datablock LightFlareData( ScatterskyFlareExample )
{      
   overallScale = 2.0;
   flareEnabled = true;
   flareTexture = "./../special/lensFlareSheet0";  
   
   elementRect[0] = "1024 0 128 128";
   elementDist[0] = 0.5;
   elementScale[0] = 0.25;   
   elementTint[0] = "1.0 1.0 1.0";
   elementRotate[0] = true;
   elementUseLightColor[0] = true;
   
   elementRect[1] = "1024 128 128 128";
   elementDist[1] = 0.8;
   elementScale[1] = 0.7;   
   elementTint[1] = "1.0 1.0 1.0";
   elementRotate[1] = true;
   elementUseLightColor[1] = true;
   
   elementRect[2] = "1024 0 128 128";
   elementDist[2] = 1.18;
   elementScale[2] = 0.5;   
   elementTint[2] = "1.0 1.0 1.0";
   elementRotate[2] = true;
   elementUseLightColor[2] = true;
   
   elementRect[3] = "1152 128 128 128";
   elementDist[3] = 1.25;
   elementScale[3] = 0.25;   
   elementTint[3] = "1.0 1.0 1.0";
   elementRotate[3] = true;
   elementUseLightColor[3] = true;
   
   elementRect[4] = "1024 0 128 128";
   elementDist[4] = 2.0;
   elementScale[4] = 0.25;      
   elementTint[4] = "0.7 0.7 0.7";
   elementRotate[4] = true;
   elementUseLightColor[4] = true;
};

datablock LightFlareData( LightFlareExample0 )
{
   overallScale = 2.0;
   flareEnabled = true;
   flareTexture = "./../special/lensFlareSheet1";
   
   elementRect[0] = "0 512 512 512";
   elementDist[0] = 0.0;
   elementScale[0] = 0.5;
   elementTint[0] = "1.0 1.0 1.0";
   elementRotate[0] = false;
   elementUseLightColor[0] = false;
   
   elementRect[1] = "512 0 512 512";
   elementDist[1] = 0.0;
   elementScale[1] = 2.0;
   elementTint[1] = "0.5 0.5 0.5";
   elementRotate[1] = false;
   elementUseLightColor[1] = false;
};

datablock LightFlareData( LightFlareExample1 )
{
   overallScale = 2.0;
   flareEnabled = true;
   flareTexture = "./../special/lensFlareSheet1";
   
   elementRect[0] = "512 512 512 512";
   elementDist[0] = 0.0;
   elementScale[0] = 0.5;
   elementTint[0] = "1.0 1.0 1.0";
   elementRotate[0] = false;
   elementUseLightColor[0] = false;
   
   elementRect[1] = "512 0 512 512";
   elementDist[1] = 0.0;
   elementScale[1] = 2.0;
   elementTint[1] = "0.5 0.5 0.5";
   elementRotate[1] = false;
   elementUseLightColor[1] = false;
};

datablock LightFlareData( LightFlareExample2 )
{
   overallScale = 2.0;
   flareEnabled = true;
   flareTexture = "./../special/lensFlareSheet0";  

   elementRect[0] = "512 512 512 512";
   elementDist[0] = 0.0;
   elementScale[0] = 0.5;
   elementTint[0] = "1.0 1.0 1.0";
   elementRotate[0] = true;
   elementUseLightColor[0] = true;
   
   elementRect[1] = "512 0 512 512";
   elementDist[1] = 0.0;
   elementScale[1] = 2.0;
   elementTint[1] = "0.7 0.7 0.7";
   elementRotate[1] = true;
   elementUseLightColor[1] = true;
   
   elementRect[2] = "1152 0 128 128";
   elementDist[2] = 0.3;
   elementScale[2] = 0.5;
   elementTint[2] = "1.0 1.0 1.0";
   elementRotate[2] = true;
   elementUseLightColor[2] = true;
   
   elementRect[3] = "1024 0 128 128";
   elementDist[3] = 0.5;
   elementScale[3] = 0.25;   
   elementTint[3] = "1.0 1.0 1.0";
   elementRotate[3] = true;
   elementUseLightColor[3] = true;
   
   elementRect[4] = "1024 128 128 128";
   elementDist[4] = 0.8;
   elementScale[4] = 0.6;   
   elementTint[4] = "1.0 1.0 1.0";
   elementRotate[4] = true;
   elementUseLightColor[4] = true;
   
   elementRect[5] = "1024 0 128 128";
   elementDist[5] = 1.18;
   elementScale[5] = 0.5;   
   elementTint[5] = "0.7 0.7 0.7";
   elementRotate[5] = true;
   elementUseLightColor[5] = true;
   
   elementRect[6] = "1152 128 128 128";
   elementDist[6] = 1.25;
   elementScale[6] = 0.35;   
   elementTint[6] = "0.8 0.8 0.8";
   elementRotate[6] = true;
   elementUseLightColor[6] = true;
   
   elementRect[7] = "1024 0 128 128";
   elementDist[7] = 2.0;
   elementScale[7] = 0.25;      
   elementTint[7] = "1.0 1.0 1.0";
   elementRotate[7] = true;
   elementUseLightColor[7] = true;
};