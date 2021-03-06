//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Game duration in secs, no limit if the duration is set to 0
$Game::Duration = 20 * 60;

// When a client score reaches this value, the game is ended.
$Game::EndGameScore = 30;

// Pause while looking over the end game screen (in secs)
$Game::EndGamePause = 10;

//-----------------------------------------------------------------------------

function onServerCreated()
{
   // Server::GameType is sent to the master server.
   // This variable should uniquely identify your game and/or mod.
   $Server::GameType = $appName;

   // Server::MissionType sent to the master server.  Clients can
   // filter servers based on mission type.
   $Server::MissionType = "Deathmatch";

   // GameStartTime is the sim time the game started. Used to calculated
   // game elapsed time.
   $Game::StartTime = 0;

   // Create the server physics world.
   physicsInitWorld( "server" );

   // Load up any objects or datablocks saved to the editor managed scripts
   if (isFile("art/shapes/particles/managedParticleData.cs"))
      exec("art/shapes/particles/managedParticleData.cs");
   if (isFile("art/decals/managedDecalData.cs"))
      exec("art/decals/managedDecalData.cs");
   if (isFile("art/datablocks/managedDatablocks.cs"))
      exec("art/datablocks/managedDatablocks.cs");

   // Load up user specified data and object declarations
   exec("art/datablocks/datablockExec.cs");

   // Run the other gameplay scripts in this folder
   exec("./scriptExec.cs");

   // Keep track of when the game started
   $Game::StartTime = $Sim::Time;
}

function onServerDestroyed()
{
   // This function is called as part of a server shutdown.

   physicsDestroyWorld( "server" );

   if (isObject(game))
   {
      game.deactivatepackages();
      game.delete();
   }
}

//-----------------------------------------------------------------------------

function onGameDurationEnd()
{
   // This "redirect" is here so that we can abort the game cycle if
   // the $Game::Duration variable has been cleared, without having
   // to have a function to cancel the schedule.

   if ($Game::Duration && !isObject(EditorGui))
      game.onGameDurationEnd();
}

//-----------------------------------------------------------------------------

function cycleGame()
{
   // This is setup as a schedule so that this function can be called
   // directly from object callbacks.  Object callbacks have to be
   // carefull about invoking server functions that could cause
   // their object to be deleted.

   if (!$Game::Cycling)
   {
      $Game::Cycling = true;
      $Game::Schedule = schedule(0, 0, "onCycleExec");
   }
}

function onCycleExec()
{
   // End the current game and start another one, we'll pause for a little
   // so the end game victory screen can be examined by the clients.

   endGame();
   $Game::Schedule = schedule($Game::EndGamePause * 1000, 0, "onCyclePauseEnd");
}

function onCyclePauseEnd()
{
   $Game::Cycling = false;

   // Just cycle through the missions for now.
   %search = $Server::MissionFileSpec;
   for (%file = findFirstFile(%search); %file !$= ""; %file = findNextFile(%search))
   {
      if (%file $= $Server::MissionFile)
      {
         // Get the next one, back to the first if there is no next.
         %file = findNextFile(%search);
         if (%file $= "")
            %file = findFirstFile(%search);
         break;
      }
   }
   loadMission(%file);
}

//-----------------------------------------------------------------------------
// GameConnection Methods
// These methods are extensions to the GameConnection class. Extending
// GameConnection makes it easier to deal with some of this functionality,
// but these could also be implemented as stand-alone functions.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

function GameConnection::onLeaveMissionArea(%this)
{
   // The control objects invoke this method when they
   // move out of the mission area.

   messageClient(%this, 'MsgClientJoin', '\c2Now leaving the mission area!');
}

function GameConnection::onEnterMissionArea(%this)
{
   // The control objects invoke this method when they
   // move back into the mission area.

   messageClient(%this, 'MsgClientJoin', '\c2Now entering the mission area.');
}

//-----------------------------------------------------------------------------

function GameConnection::onDeath(%this, %sourceObject, %sourceClient, %damageType, %damLoc)
{
   game.onDeath(%this, %sourceObject, %sourceClient, %damageType, %damLoc);
}

// ----------------------------------------------------------------------------
// weapon HUD
// ----------------------------------------------------------------------------
function GameConnection::setAmmoAmountHud(%client, %amount)
{
   commandToClient(%client, 'SetAmmoAmountHud', %amount);
}

function GameConnection::RefreshWeaponHud(%client, %amount, %preview, %ret)
{
   commandToClient(%client, 'RefreshWeaponHud', %amount, %preview, %ret);
}
