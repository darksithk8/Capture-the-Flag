//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load up all datablocks.  This function is called when
// a server is constructed.

// Set up some useful AudioDescription's
exec("./audioDescriptions.cs");

// Set up the Camera's
exec("./camera.cs");

// Common Marker's
exec("./markers.cs");

exec("./defaultparticle.cs");

// LightFlareData and LightAnimData(s)
exec("./lights.cs");