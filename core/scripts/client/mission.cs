//-----------------------------------------------------------------------------
// Torque Shader Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//----------------------------------------------------------------------------
// Mission start / end events sent from the server
//----------------------------------------------------------------------------

function clientCmdMissionStart(%seq)
{
   // The client recieves a mission start right before
   // being dropped into the game.
   
   physicsStartSimulation( "client" );
      
   new SimGroup( ClientMissionCleanup );
}

function clientCmdMissionEnd(%seq)
{
   // Stop all the simulation sounds.
   sfxStopAll( $SimAudioType );
   
   // TODO: this client command is never sent in singleplayer or maybe even
   // not to the local client! Also need to revisit the way decal data
   // is loaded/reloaded between missions.
   
   // Delete all the decals.
   decalManagerClear();

   // Disable mission lighting if it's going, this is here
   // in case the mission ends while we are in the process
   // of loading it.
   $lightingMission = false;
   $sceneLighting::terminateLighting = true;
   
   if( isObject(ClientMissionCleanup) )
   {
      ClientMissionCleanup.delete();
   }
   clearClientPaths();
}
