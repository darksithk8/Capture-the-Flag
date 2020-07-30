//-----------------------------------------------------------------------------
// Torque Game Engine Advanced
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// First we execute the core default preferences.
exec( "core/scripts/server/defaults.cs" );


// Now add your own game specific server preferences as
// well as any overloaded core defaults here.




// Finally load the preferences saved from the last
// game execution if they exist.
if ( isFile( "./prefs.cs" ) )
   exec( "./prefs.cs" );
