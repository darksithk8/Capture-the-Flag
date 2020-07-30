// ----------------------------------------------------------------------------
// GameCore
// ----------------------------------------------------------------------------
// This is the core of the gametype functionality. The "Default Game". All of
// the gametypes share or over-ride the scripted controls for the default game.
//
// The desired Game Type must be added to each mission's LevelInfo object.
//   - gameType = "";
//   - gameType = "Deathmatch";
// If this information is missing then the GameCore will default to Deathmatch.
// ----------------------------------------------------------------------------

function GameCore::activatePackages(%game)
{
   echo (%game @"\c4 -> activatePackages");

   activatePackage(GameCore);
   if (isPackage(%game.class) && %game.class !$= GameCore)
      activatePackage(%game.class);
}

function GameCore::deactivatePackages(%game)
{
   //echo (%game @"\c4 -> deactivatePackages");

   if (isPackage(%game.class) && %game.class !$= GameCore)
      deactivatePackage(%game.class);
   deactivatePackage(GameCore);
}

// ----------------------------------------------------------------------------
// Package
// ----------------------------------------------------------------------------

// The GameCore package overides functions loadMissionStage2(), endMission(),
// and function resetMission() from "core/scripts/server/missionLoad.cs" in
// order to create our Game object, which allows our gameType functionality to
// be initiated.

package GameCore
{
   function loadMissionStage2()
   {
      //echo("\c4 -> loadMissionStage2() override success");

      // Create the mission group off the ServerGroup
      echo("*** Stage 2 load");
      $instantGroup = ServerGroup;

      // Make sure the mission exists
      %file = $Server::MissionFile;

      if (!isFile(%file))
      {
         error("Could not find mission "@ %file);
         return;
      }

      // Calculate the mission CRC.  The CRC is used by the clients
      // to caching mission lighting.
      $missionCRC = getFileCRC(%file);

      // Exec the mission, objects are added to the ServerGroup
      exec(%file);

      // If there was a problem with the load, let's try another mission
      if (!isObject(MissionGroup))
      {
         error("No 'MissionGroup' found in mission \""@ $missionName @"\".");
         schedule(3000, ServerGroup, CycleMissions);
         return;
      }

      // Mission cleanup group
      new SimGroup(MissionCleanup);
      $instantGroup = MissionCleanup;

      // ================================
      // Create Game Objects
      // Here begins our gametype functionality
      $Server::MissionType = theLevelInfo.gameType;  //MissionInfo.gametype;
      //echo("\c4 -> Parsed mission Gametype: "@ theLevelInfo.gameType); //MissionInfo.gametype);
      if ($Server::MissionType $= "")
         $Server::MissionType = "Deathmatch"; //Default gametype, just in case
      new ScriptObject(Game)
      {
         class = $Server::MissionType @"Game";
         superClass = GameCore;
      };
      Game.activatePackages();
      // ================================

      // Construct MOD paths
      pathOnMissionLoadDone();

      // Mission loading done...
      echo("*** Mission loaded");

      // Start all the clients in the mission
      $missionRunning = true;
      for (%clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++)
         ClientGroup.getObject(%clientIndex).loadMission();

      // Go ahead and launch the game
      Game.onMissionLoaded();
   }

   function endMission()
   {
      //echo("\c4 -> endMission() override success");
      if (!isObject(MissionGroup))
         return;

      echo("*** ENDING MISSION");

      // Inform the game code we're done.
      Game.onMissionEnded();

      // Inform the clients
      for (%clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++)
      {
         // clear ghosts and paths from all clients
         %cl = ClientGroup.getObject(%clientIndex);
         %cl.endMission();
         %cl.resetGhosting();
         %cl.clearPaths();
      }

      // Delete everything
      MissionGroup.delete();
      MissionCleanup.delete();

      $ServerGroup.delete();
      $ServerGroup = new SimGroup(ServerGroup);

      clearServerpaths();

      if (isObject(Game))
      {
         Game.deactivatePackages();
         Game.delete();
      }
   }

   function resetMission()
   {
      //echo("\c4 -> resetMission() override success");
      echo("*** MISSION RESET");

      // Remove any temporary mission objects
      MissionCleanup.delete();
      $instantGroup = ServerGroup;
      new SimGroup(MissionCleanup);
      $instantGroup = MissionCleanup;

      if (isObject(game))
      {
         game.deactivatePackages();
         game.delete();
         game.onMissionReset();
         game.onMissionEnded();
      }

      clearServerpaths();
      Game.onMissionReset();
   }

   // We also need to override function GameConnection::onConnect() from
   // "core/scripts/server/clientConnection.cs" in order to initialize, reset,
   // and pass some client scoring variables to playerList.gui -- the scoreHUD.

   function GameConnection::onConnect(%client, %name)
   {
      // Send down the connection error info, the client is responsible for
      // displaying this message if a connection error occurs.
      messageClient(%client, 'MsgConnectionError',"",$Pref::Server::ConnectionError);

      // Send mission information to the client
      sendLoadInfoToClient(%client);

      // Simulated client lag for testing...
      // %client.setSimulatedNetParams(0.1, 30);

      // Get the client's unique id:
      // %authInfo = %client.getAuthInfo();
      // %client.guid = getField(%authInfo, 3);
      %client.guid = 0;
      addToServerGuidList(%client.guid);

      // Set admin status
      if (%client.getAddress() $= "local")
      {
         %client.isAdmin = true;
         %client.isSuperAdmin = true;
      }
      else
      {
         %client.isAdmin = false;
         %client.isSuperAdmin = false;
      }

      // Save client preferences on the connection object for later use.
      %client.gender = "Male";
      %client.armor = "Light";
      %client.race = "Human";
      %client.skin = addTaggedString("base");
      %client.setPlayerName(%name);
      %client.score = 0;
      %client.kills = 0;
      %client.deaths = 0;

      //
      $instantGroup = ServerGroup;
      $instantGroup = MissionCleanup;
      echo("CADD: "@ %client @" "@ %client.getAddress());

      // Inform the client of all the other clients
      %count = ClientGroup.getCount();
      for (%cl = 0; %cl < %count; %cl++)
      {
         %other = ClientGroup.getObject(%cl);
         if ((%other != %client))
         {
            // These should be "silent" versions of these messages...
            messageClient(%client, 'MsgClientJoin', "",
               %other.playerName,
               %other,
               %other.sendGuid,
               %other.score,
               %other.kills,
               %other.deaths,
               %other.isAIControlled(),
               %other.isAdmin,
               %other.isSuperAdmin);
         }
      }

      // Inform the client we've joined up
      messageClient(%client,
         'MsgClientJoin', '\c2Welcome to the Torque demo app %1.',
         %client.playerName,
         %client,
         %client.sendGuid,
         %client.score,
         %client.kills,
         %client.deaths,
         %client.isAiControlled(),
         %client.isAdmin,
         %client.isSuperAdmin);

      // Inform all the other clients of the new guy
      messageAllExcept(%client, -1, 'MsgClientJoin', '\c1%1 joined the game.',
         %client.playerName,
         %client,
         %client.sendGuid,
         %client.score,
         %client.kills,
         %client.deaths,
         %client.isAiControlled(),
         %client.isAdmin,
         %client.isSuperAdmin);

      // If the mission is running, go ahead download it to the client
      if ($missionRunning)
         %client.loadMission();
      $Server::PlayerCount++;
   }

   function GameConnection::onClientEnterGame(%this)
   {
      Game.onClientEnterGame(%this);
   }
   function GameConnection::onClientLeaveGame(%this)
   {
      Game.onClientLeaveGame(%this);
   }

   // Need to supersede this "core" function in order to properly re-spawn a
   // player after he/she is killed.
   // This will also allow the differing gametypes to more easily have a unique
   // method for spawn handling without needless duplication of code.
   function GameConnection::spawnPlayer(%this, %spawnPoint)
   {
      Game.spawnPlayer(%this, %spawnPoint);
   }
};
// end of our package... now activate it!
activatePackage(GameCore);

// ----------------------------------------------------------------------------
//  Game Control Functions
// ----------------------------------------------------------------------------

function GameCore::onMissionLoaded(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onMissionLoaded");

   //set up the game and game variables
   %game.initGameVars(%game);

   $Game::Duration = %game.duration;
   $Game::EndGameScore = %game.endgameScore;
   $Game::EndGamePause = %game.endgamePause;

   physicsStartSimulation("server");
   %game.startGame();
}

function GameCore::onMissionEnded(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onMissionEnded");

   // Called by endMission(), right before the mission is destroyed

   // Normally the game should be ended first before the next
   // mission is loaded, this is here in case loadMission has been
   // called directly.  The mission will be ended if the server
   // is destroyed, so we only need to cleanup here.

   physicsStopSimulation("server");
   %game.endGame();

   cancel($Game::Schedule);
   $Game::Running = false;
   $Game::Cycling = false;
}

function GameCore::onMissionReset(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onMissionReset");
   %game.startGame();
}

function GameCore::startGame(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onStartGame");
   if ($Game::Running)
   {
      error("startGame: End the game first!");
      return;
   }

   // Inform the client we're starting up
   for (%clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++)
   {
      %cl = ClientGroup.getObject(%clientIndex);
      commandToClient(%cl, 'GameStart');

      // Other client specific setup..
      %cl.score = 0;
   }

   // Start the game timer
   if ($Game::Duration)
      $Game::Schedule = schedule($Game::Duration * 1000, 0, "onGameDurationEnd");
   $Game::Running = true;

//    // Start the AIManager
//    new ScriptObject(AIManager) {};
//    MissionCleanup.add(AIManager);
//    AIManager.think();


}

function GameCore::endGame(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::endGame");

   if (!$Game::Running)
   {
      error("endGame: No game running!");
      return;
   }

//    // Stop the AIManager
//    AIManager.delete();

   // Stop any game timers
   cancel($Game::Schedule);

   for (%clientIndex = 0; %clientIndex < ClientGroup.getCount(); %clientIndex++)
   {
      %cl = ClientGroup.getObject(%clientIndex);
      commandToClient(%cl, 'GameEnd');
   }

   // Delete all the temporary mission objects
   resetMission();
   $Game::Running = false;
}

function GameCore::onGameDurationEnd()
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onGameDurationEnd");

   if ($Game::Duration && !isObject(EditorGui))
      cycleGame();
}

// ----------------------------------------------------------------------------
//  Game Setup
// ----------------------------------------------------------------------------

function GameCore::initGameVars(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::initGameVars");

   //-----------------------------------------------------------------------------
   // What kind of "player" is spawned is either controlled directly by the
   // SpawnSphere or it defaults back to the values set here. This also controls
   // which SimGroups to attempt to select the spawn sphere's from by walking down
   // the list of SpawnGroups till it finds a valid spawn object.
   //-----------------------------------------------------------------------------
   $Game::DefaultPlayerClass = "Player";
   $Game::DefaultPlayerDataBlock = "DefaultPlayerData";
   $Game::DefaultPlayerSpawnGroups = "PlayerSpawnPoints PlayerDropPoints";

   //-----------------------------------------------------------------------------
   // What kind of "camera" is spawned is either controlled directly by the
   // SpawnSphere or it defaults back to the values set here. This also controls
   // which SimGroups to attempt to select the spawn sphere's from by walking down
   // the list of SpawnGroups till it finds a valid spawn object.
   //-----------------------------------------------------------------------------
   $Game::DefaultCameraClass = "Camera";
   $Game::DefaultCameraDataBlock = "Observer";
   $Game::DefaultCameraSpawnGroups = "CameraSpawnPoints PlayerSpawnPoints PlayerDropPoints";
}

// ----------------------------------------------------------------------------
//  Client Management
// ----------------------------------------------------------------------------

function GameCore::onClientEnterGame(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onClientEntergame");

   // Sync the client's clocks to the server's
   commandToClient(%client, 'SyncClock', $Sim::Time - $Game::StartTime);

   // Find a spawn point for the camera
   // This function currently relies on some helper functions defined in
   // core/scripts/server/spawn.cs. For custom spawn behaviors one can either
   // override the properties on the SpawnSphere's or directly override the
   // functions themselves.
   %cameraSpawnPoint = pickCameraSpawnPoint($Game::DefaultCameraSpawnGroups);
   // Spawn a camera for this client using the found %spawnPoint
   %client.spawnCamera(%cameraSpawnPoint);

   // Setup game parameters, the onConnect method currently starts
   // everyone with a 0 score.
   %client.score = 0;
   %client.kills = 0;
   %client.deaths = 0;

   // weaponHUD
   %client.RefreshWeaponHud(0, "", "");

   // Prepare the player object.
   %game.preparePlayer(%client);
}

function GameCore::onClientLeaveGame(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::onClientLeaveGame");

   // Cleanup the camera
   if (isObject(%this.camera))
      %this.camera.delete();
   // Cleanup the player
   if (isObject(%this.player))
      %this.player.delete();
}

// Added this stage to creating a player so game types can override it easily.
// This is a good place to initiate team selection.
function GameCore::preparePlayer(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::preparePlayer");

   // Find a spawn point for the player
   // This function currently relies on some helper functions defined in
   // core/scripts/spawn.cs. For custom spawn behaviors one can either
   // override the properties on the SpawnSphere's or directly override the
   // functions themselves.
   %playerSpawnPoint = pickPlayerSpawnPoint($Game::DefaultPlayerSpawnGroups);
   // Spawn a camera for this client using the found %spawnPoint
   //%client.spawnPlayer(%playerSpawnPoint);
   %game.spawnPlayer(%client, %playerSpawnPoint);


}

// jedi player

function GameCore::onDeath(%game, %client, %sourceObject, %sourceClient, %damageType, %damLoc)
{
   if(isObject(%client))
   {  
     if (%client.player == $team1flagcarrier)
      {
         %client.player.unmountobject (Flag2SS);
         Flag2SS.setTransform (TriggerOfTeam2.getPosition());
         $team1flagcarrier = false;
      }
   else if (%client.player == $team2flagcarrier)
      {
         %client.player.unmountobject (Flag1SS);
         Flag1SS.setTransform (TriggerOfTeam1.getPosition());
         $team2flagcarrier = false;
      }
    // Switch the client over to the death cam and unhook the player object.
   if (isObject(%client.camera) && isObject(%client.player))
   {
      %client.camera.setMode("Corpse", %client.player);
      %client.setControlObject(%client.camera);
   }
   %client.player = 0;
      // Dole out points and display an appropriate message
   if (%damageType $= "Suicide" || %sourceClient == %this)
   {
      game.incScore(%client, -1);
      game.incDeaths(%client, 1);
      messageAll('MsgClientKilled', '%1 takes his own life!', %client.playerName);
   }
   else
   {
      
      
      game.incScore(%sourceClient, 1);
      game.incKills(%sourceClient, 1);
      game.incDeaths(%client, 1);
      messageAll('MsgClientKilled', '%1 gets nailed by %2!', %client.playerName, %sourceClient.playerName);
      if (%sourceClient.score >= $Game::EndGameScore)
         cycleGame();
   }
   messageAll('MsgClientScoreChanged', "", %client.score, %client.kills, %client.deaths, %client);
   }
}

// ----------------------------------------------------------------------------
// Scoring
// ----------------------------------------------------------------------------

function GameCore::incKills(%game, %client, %kill)
{
   %client.kills += %kill;
   messageAll('MsgClientScoreChanged', "", %client.score, %client.kills, %client.deaths, %client);
}

function GameCore::incDeaths(%game, %client, %death)
{
   %client.deaths += %death;
   messageAll('MsgClientScoreChanged', "", %client.score, %client.kills, %client.deaths, %client);
}

function GameCore::incScore(%game, %client, %score)
{
   %client.score += %score;
   messageAll('MsgClientScoreChanged', "", %client.score, %client.kills, %client.deaths, %client);
}

function GameCore::getScore(%client) { return %client.score; }
function GameCore::getKills(%client) { return %client.kills; }
function GameCore::getDeaths(%client) { return %client.deaths; }

// ----------------------------------------------------------------------------
// Spawning
// ----------------------------------------------------------------------------

function GameCore::spawnPlayer(%game, %this, %spawnPoint, %noControl)
{
   // Something doing with teams, %j< Number:
   for(%j=0; %j<2; %j++)
   {
      %teamNumber = %j + 1;
      if(%teamNumber == 1)
      {
         %teamWord = "One";
      }
      else
      {
         %teamWord = "Two";
      }
      
      // Amount of bots, %i< Number:
      %NumberOfBots = 8;
      for(%i=0; %i<%NumberOfBots/2; %i++)
      {
         %action = 1;
                createBot(%teamNumber, %teamWord, %action);
      }
      for(%i=0; %i<%NumberOfBots/2; %i++)
      {
         %action = 2;
                createBot(%teamNumber, %teamWord, %action);
      }
   }
}

function createBot(%teamNumber, %teamWord, %action)
{
   if(%teamNumber == 1)
   {
      %botID = new AIPlayer()
         {
            datablock = botOfteam1;
         };
         %botID.team = 1;
         //botAttackFlag(%botID);
   }
   else if(%teamNumber == 2)
   {
      %botID = new AIPlayer()
         {
            datablock = botOfteam2;
         };
         %botID.team = 2;
         //botAttackFlag(%botID);
   }
   %groupID = nameToID("MissionGroup/PlayerDropPoints/team" @ %teamWord @ "SpawnPoints");  
   %pos = %groupID.getObject(0).getTransform();

   %x =  getRandom(-20, 20);
   %y =  getRandom(-20, 20);   

   %newX = getWord(%pos, 0) + %x;
   %newY = getWord(%pos, 1) + %y;
   %newZ = getWord(%pos, 2);

   %botID.setTransform(%newX SPC %newY SPC %newZ);
   %teamGroupID = nameToID("MissionGroup/AllPlayers/team" @ %teamWord @ "Players");
   %teamGroupID.add(%botID);

   // Equipment for bots:
   %botID.mountImage(RocketLauncherImage, 0);
   %botID.setInventory(RocketLauncherAmmo, 1000);
   
   %botID.team = %teamNumber;
   %botID.action = %action;
   %botID.teamWord = %teamWord; // Temporary
   
   schedule(3000, %botID, botThink, %botID, %action);
}

// ----------------------------------------------------------------------------
// Observer
// ----------------------------------------------------------------------------

function GameCore::spawnObserver(%game, %client)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::spawnObserver");

   // Position the camera on one of our observer spawn points
   %client.camera.setTransform(%game.pickObserverSpawnPoint());

   // Set control to the camera
   %client.setControlObject(%client.camera);
}

function GameCore::pickObserverSpawnPoint(%game)
{
   //echo (%game @"\c4 -> "@ %game.class @" -> GameCore::pickObserverSpawnPoint");

   %groupName = "MissionGroup/ObserverSpawnPoints";
   %group = nameToID(%groupName);

   if (%group != -1)
   {
      %count = %group.getCount();
      if (%count != 0)
      {
         %index = getRandom(%count-1);
         %spawn = %group.getObject(%index);
         return %spawn.getTransform();
      }
      else
         error("No spawn points found in "@ %groupName);
   }
   else
      error("Missing spawn points group "@ %groupName);

   // Could be no spawn points, in which case we'll stick the
   // player at the center of the world.
   return "0 0 300 1 0 0 0";
}