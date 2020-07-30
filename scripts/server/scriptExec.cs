//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load up all scripts.  This function is called when
// a server is constructed.
exec("./camera.cs");
exec("./triggers.cs");
exec("./inventory.cs");
exec("./shapeBase.cs");
exec("./item.cs");
exec("./health.cs");
exec("./projectile.cs");
exec("./radiusDamage.cs");

// Load our supporting weapon script, it contains methods used by all weapons.
exec("./weapon.cs");

// Exec of the Flag
exec("./flag.cs");

// Load our weapon scripts
// We only need weapon scripts for those weapons that work differently from the
// "generic" methods defined in weapon.cs
exec("./rocketLauncher.cs");

// Load our default player script
exec("./player.cs");

// Load our player scripts
exec("./aiPlayer.cs");

exec("./vehicle.cs");
exec("./vehicleWheeled.cs");

// Load our gametypes
exec("./gameCore.cs"); // This is the 'core' of the gametype functionality.
exec("./gameDM.cs"); // Overrides GameCore with DeathMatch functionality.
