//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// This file contains 'generic' datablocks usable by all weapon class items.

// ----------------------------------------------------------------------------
// Audio profiles
// ----------------------------------------------------------------------------

datablock SFXProfile(WeaponUseSound)
{
   filename = "art/sound/weapons/weapon_switch";
   description = AudioClose3d;
   preload = true;
};

datablock SFXProfile(WeaponPickupSound)
{
   filename = "art/sound/weapons/Weapon_pickup";
   description = AudioClose3d;
   preload = true;
};

datablock SFXProfile(AmmoPickupSound)
{
   filename = "art/sound/weapons/Ammo_pickup";
   description = AudioClose3d;
   preload = true;
};
