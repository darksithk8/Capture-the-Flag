//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load up all datablocks.  This function is called when
// a server is constructed.

// Do the sounds first -- later scripts/datablocks may need them
exec("./audioProfiles.cs");

// Do the various effects next -- later scripts/datablocks may need them
exec("./particles.cs");

exec("./environment.cs");
exec("./triggers.cs");

// Add a rigid example
exec("./rigidShape.cs");

exec("./health.cs");

// Load our supporting weapon datablocks
exec("./weapon.cs");

// Load the weapon datablocks
exec("./weapons/swarmgun.cs");

// Load the default player datablocks
exec("./player.cs");

// Load our other player datablocks
exec("./aiPlayer.cs");

// Exec of teams datablocks:

exec("./team1.cs");

exec("./team2.cs");

exec("./team1Objectives.cs");

exec("./team2Objectives.cs");

// Load the vehicle datablocks
exec("./vehicles/defaultCar.cs");
