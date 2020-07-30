//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Always declare SFXDescription's (the type of sound) before SFXProfile's (the
// sound itself) when creating new ones

//-----------------------------------------------------------------------------
// 3D Sounds
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Single shot sounds
//-----------------------------------------------------------------------------

datablock SFXDescription( AudioDefault3D : AudioSim )
{
   is3D              = true;
   ReferenceDistance = 20.0;
   MaxDistance       = 100.0;
};

datablock SFXDescription( AudioClose3D : AudioSim )
{
   is3D              = true;
   ReferenceDistance = 10.0;
   MaxDistance       = 60.0;
};

datablock SFXDescription( AudioClosest3D : AudioSim )
{
   is3D              = true;
   ReferenceDistance = 5.0;
   MaxDistance       = 30.0;
};

//-----------------------------------------------------------------------------
// Looping sounds
//-----------------------------------------------------------------------------

datablock SFXDescription( AudioDefaultLoop3D : AudioSim )
{
   isLooping         = true;
   is3D              = true;
   ReferenceDistance = 20.0;
   MaxDistance       = 100.0;
};

datablock SFXDescription( AudioCloseLoop3D : AudioSim )
{
   isLooping         = true;
   is3D              = true;
   ReferenceDistance = 10.0;
   MaxDistance       = 50.0;
};

datablock SFXDescription( AudioClosestLoop3D : AudioSim )
{
   isLooping         = true;
   is3D              = true;
   ReferenceDistance = 5.0;
   MaxDistance       = 30.0;
};

//-----------------------------------------------------------------------------
// 2d sounds
//-----------------------------------------------------------------------------

// Used for non-looping environmental sounds (like power on, power off)
datablock SFXDescription( Audio2D : AudioSim )
{
   isLooping         = false;
};

// Used for Looping Environmental Sounds
datablock SFXDescription( AudioLoop2D : AudioSim )
{
   isLooping         = true;
};

datablock SFXDescription( AudioStream2D : AudioSim )
{
   isStreaming       = true;
};
datablock SFXDescription( AudioStreamLoop2D : AudioSim )
{
   isLooping         = true;
   isStreaming       = true;
};

//-----------------------------------------------------------------------------
// Music
//-----------------------------------------------------------------------------

datablock SFXDescription( AudioMusic2D : AudioMusic )
{
   isStreaming       = true;
};

datablock SFXDescription( AudioMusicLoop2D : AudioMusic )
{
   isLooping         = true;
   isStreaming       = true;
};

datablock SFXDescription( AudioMusic3D : AudioMusic )
{
   isStreaming       = true;
   is3D              = true;
};

datablock SFXDescription( AudioMusicLoop3D : AudioMusic )
{
   isStreaming       = true;
   is3D              = true;
   isLooping         = true;
};
