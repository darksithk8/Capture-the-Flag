//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------
// Define the field types for objects that link to the namespace LevelInfo
function LevelInfo::onDefineFieldTypes( %this )
{
   %this.setFieldType("Desc", "TypeString");
   %this.setFieldType("DescLines", "TypeS32");
}
