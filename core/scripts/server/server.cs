//-----------------------------------------------------------------------------
// Torque Shader Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function initBaseServer()
{
   // Base server functionality
   exec("./audio.cs");
   exec("./message.cs");
   exec("./commands.cs");
   exec("./levelInfo.cs");
   exec("./missionLoad.cs");
   exec("./missionDownload.cs");
   exec("./clientConnection.cs");
   exec("./kickban.cs");
   exec("./game.cs");
   exec("./spawn.cs");
   exec("./camera.cs");
   exec("./centerPrint.cs");
}

//-----------------------------------------------------------------------------
// Attempt to find an open port to initialize the server with
//-----------------------------------------------------------------------------
function portInit(%port)
{
   %failCount = 0;
   while(%failCount < 10 && !setNetPort(%port))
   {
      echo("Port init failed on port " @ %port @ " trying next port.");
      %port++; %failCount++;
   }
}

//-----------------------------------------------------------------------------
// Create a server with either a "SinglePlayer" or "MultiPlayer" type
// Specify the level to load on the server
//-----------------------------------------------------------------------------
function createServer(%serverType, %level)
{
   if (%level $= "")
   {
      error("createServer(): level name unspecified");
      return;
   }
   
   // Make sure our level name is relative so that it can send
   // across the network correctly
   %level = makeRelativePath(%level, getWorkingDirectory());

   destroyServer();

   $missionSequence = 0;
   $Server::PlayerCount = 0;
   $Server::ServerType = %serverType;

   // Setup for multi-player, the network must have been
   // initialized before now.
   if (%serverType $= "MultiPlayer")
   {
      echo("Starting multiplayer mode");

      // Make sure the network port is set to the correct pref.
      portInit($Pref::Server::Port);
      allowConnections(true);

      if ($pref::Net::DisplayOnMaster !$= "Never" )
         schedule(0,0,startHeartbeat);
   }

   // Load the level
   $ServerGroup = new SimGroup(ServerGroup);

   // Load up any core datablocks
   exec("core/art/datablocks/datablockExec.cs");

   // Let the game initialize some things now that the
   // the server has been created
   onServerCreated();

   loadMission(%level, true);
}

//-----------------------------------------------------------------------------
// Shut down the server
//-----------------------------------------------------------------------------
function destroyServer()
{
   $Server::ServerType = "";
   allowConnections(false);
   stopHeartbeat();
   $missionRunning = false;
   
   // End any running levels
   endMission();
   onServerDestroyed();

   // Delete all the server objects
   if (isObject(MissionGroup))
      MissionGroup.delete();
   if (isObject(MissionCleanup))
      MissionCleanup.delete();
   if (isObject($ServerGroup))
      $ServerGroup.delete();

   // Delete all the connections:
   while (ClientGroup.getCount())
   {
      %client = ClientGroup.getObject(0);
      %client.delete();
   }

   $Server::GuidList = "";

   // Delete all the data blocks...
   deleteDataBlocks();
   
   // Save any server settings
   echo( "Exporting server prefs..." );
   export( "$Pref::Server::*", "~/prefs.cs", false );
}

//--------------------------------------------------------------------------
// Reset the server's default prefs
//--------------------------------------------------------------------------
function resetServerDefaults()
{
   echo( "Resetting server defaults..." );
   
   exec( "~/defaults.cs" );
   exec( "~/prefs.cs" );

   // Reload the current level
   loadMission( $Server::MissionFile );
}

//------------------------------------------------------------------------------
// Guid list maintenance functions
//------------------------------------------------------------------------------
function addToServerGuidList( %guid )
{
   %count = getFieldCount( $Server::GuidList );
   for ( %i = 0; %i < %count; %i++ )
   {
      if ( getField( $Server::GuidList, %i ) == %guid )
         return;
   }

   $Server::GuidList = $Server::GuidList $= "" ? %guid : $Server::GuidList TAB %guid;
}

function removeFromServerGuidList( %guid )
{
   %count = getFieldCount( $Server::GuidList );
   for ( %i = 0; %i < %count; %i++ )
   {
      if ( getField( $Server::GuidList, %i ) == %guid )
      {
         $Server::GuidList = removeField( $Server::GuidList, %i );
         return;
      }
   }
}


//-----------------------------------------------------------------------------
// When the server is queried for information, the value of this function is
// returned as the status field of the query packet.  This information is
// accessible as the ServerInfo::State variable.
//-----------------------------------------------------------------------------
function onServerInfoQuery()
{
   return "Doing Ok";
}
