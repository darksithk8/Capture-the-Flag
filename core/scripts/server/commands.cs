//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Misc. server commands avialable to clients
//-----------------------------------------------------------------------------

//----------------------------------------------------------------------------
// Camera commands
//----------------------------------------------------------------------------

function serverCmdToggleCamera(%client)
{
   if (%client.getControlObject() == %client.player)
   {
      %client.camera.setVelocity("0 0 0");
      %control = %client.camera;
   }
   else
   {
      %client.player.setVelocity("0 0 0");
      %control = %client.player;
   }
   %client.setControlObject(%control);
   clientCmdSyncEditorGui();
}

function serverCmdSetEditorCameraPlayer(%client)
{
   // Switch to Player Mode
   %client.player.setVelocity("0 0 0");
   %client.setControlObject(%client.player);
   ServerConnection.setFirstPerson(1);
   $isFirstPersonVar = 1;

   clientCmdSyncEditorGui();
}

function serverCmdSetEditorCameraPlayerThird(%client)
{
   // Swith to Player Mode
   %client.player.setVelocity("0 0 0");
   %client.setControlObject(%client.player);
   ServerConnection.setFirstPerson(0);
   $isFirstPersonVar = 0;

   clientCmdSyncEditorGui();
}

function serverCmdDropPlayerAtCamera(%client)
{
   %client.player.setTransform(%client.camera.getTransform());
   %client.player.setVelocity("0 0 0");
   %client.setControlObject(%client.player);
   clientCmdSyncEditorGui();
}

function serverCmdDropCameraAtPlayer(%client)
{
   %client.camera.setTransform(%client.player.getEyeTransform());
   %client.camera.setVelocity("0 0 0");
   %client.setControlObject(%client.camera);
   clientCmdSyncEditorGui();
}

function serverCmdCycleCameraFlyType(%client)
{
   if(%client.camera.getMode() $= "Fly")
	{
		if(%client.camera.newtonMode == false) // Fly Camera
		{
			// Switch to Newton Fly Mode without rotation damping
			%client.camera.newtonMode = "1";
			%client.camera.newtonRotation = "0";
			%client.camera.setVelocity("0 0 0");
		}
		else if(%client.camera.newtonRotation == false) // Newton Camera without rotation damping
		{
			// Switch to Newton Fly Mode with damped rotation
			%client.camera.newtonMode = "1";
			%client.camera.newtonRotation = "1";
			%client.camera.setAngularVelocity("0 0 0");
		}
		else // Newton Camera with rotation damping
		{
			// Switch to Fly Mode
			%client.camera.newtonMode = "0";
			%client.camera.newtonRotation = "0";
		}
		%client.setControlObject(%client.camera);
		clientCmdSyncEditorGui();
	}
}

function serverCmdSetEditorCameraStandard(%client)
{
   // Switch to Fly Mode
   %client.camera.setFlyMode();
   %client.camera.newtonMode = "0";
   %client.camera.newtonRotation = "0";
   %client.setControlObject(%client.camera);
   clientCmdSyncEditorGui();
}

function serverCmdSetEditorCameraNewton(%client)
{
   // Switch to Newton Fly Mode without rotation damping
   %client.camera.setFlyMode();
   %client.camera.newtonMode = "1";
   %client.camera.newtonRotation = "0";
   %client.camera.setVelocity("0 0 0");
   %client.setControlObject(%client.camera);
   clientCmdSyncEditorGui();
}

function serverCmdSetEditorCameraNewtonDamped(%client)
{
   // Switch to Newton Fly Mode with damped rotation
   %client.camera.setFlyMode();
   %client.camera.newtonMode = "1";
   %client.camera.newtonRotation = "1";
   %client.camera.setAngularVelocity("0 0 0");
   %client.setControlObject(%client.camera);
   clientCmdSyncEditorGui();
}

function serverCmdSetEditorOrbitCamera(%client)
{
   %client.camera.setEditOrbitMode();
   %client.setControlObject(%client.camera);
   clientCmdSyncEditorGui();
}

function serverCmdSetEditorFlyCamera(%client)
{
   %client.camera.setFlyMode();
   %client.setControlObject(%client.camera);
   clientCmdSyncEditorGui();
}

function serverCmdEditorOrbitCameraSelectChange(%client, %size, %center)
{
   if(%size > 0)
   {
      %client.camera.setValidEditOrbitPoint(true);
      %client.camera.setEditOrbitPoint(%center);
   }
   else
   {
      %client.camera.setValidEditOrbitPoint(false);
   }
}

function serverCmdEditorCameraAutoFit(%client, %radius)
{
   %client.camera.autoFitRadius(%radius);
   %client.setControlObject(%client.camera);
  clientCmdSyncEditorGui();
}

//----------------------------------------------------------------------------
// Server admin
//----------------------------------------------------------------------------

function serverCmdSAD( %client, %password )
{
   if( %password !$= "" && %password $= $Pref::Server::AdminPassword)
   {
      %client.isAdmin = true;
      %client.isSuperAdmin = true;
      %name = getTaggedString( %client.playerName );
      MessageAll( 'MsgAdminForce', "\c2" @ %name @ " has become Admin by force.", %client );   
   }
}

function serverCmdSADSetPassword(%client, %password)
{
   if(%client.isSuperAdmin)
      $Pref::Server::AdminPassword = %password;
}


//----------------------------------------------------------------------------
// Server chat message handlers
//----------------------------------------------------------------------------

function serverCmdTeamMessageSent(%client, %text)
{
   if(strlen(%text) >= $Pref::Server::MaxChatLen)
      %text = getSubStr(%text, 0, $Pref::Server::MaxChatLen);
   chatMessageTeam(%client, %client.team, '\c3%1: %2', %client.playerName, %text);
}

function serverCmdMessageSent(%client, %text)
{
   if(strlen(%text) >= $Pref::Server::MaxChatLen)
      %text = getSubStr(%text, 0, $Pref::Server::MaxChatLen);
   chatMessageAll(%client, '\c4%1: %2', %client.playerName, %text);
}



function serverCmdpickTeam(%client, %team, %spawnPoint)
{
     if (%team == 1)
   {
      
  %player = new player()
   {
    datablock = DefaultPlayerData; 
   };

   %client.player = %player;
   %client.setControlObject(%player);
   
   %player.team = 1;      
   
   %player.client = %client;
   %groupID = nameToID("MissionGroup/PlayerDropPoints/teamOneSpawnPoints");  
   %pos = %groupID.getObject(0).getTransform();
   %player.setTransform(%pos);
   
    %teamGroupID = nameToID("MissionGroup/AllPlayers/teamOnePlayers");
   %teamGroupID.add(%player);
   }
   
   
      //%player.setInventory(RocketLauncher, 1);
   //%player.setInventory(RocketLauncherAmmo, %player.maxInventory(RocketLauncherAmmo));
   //%player.mountImage(RocketLauncherImage, 0);
   else if (%team == 2)
   {
      
   %player = new player()
   {
    datablock = DefaultPlayerData; 
   };

   %client.player = %player;
   %client.setControlObject(%player);
   
   %player.team = 2;
   
   %player.client = %client;
   %groupID = nameToID("MissionGroup/PlayerDropPoints/teamTwoSpawnPoints");  
   %pos = %groupID.getObject(0).getTransform();
   %player.setTransform(%pos);
   
    %teamGroupID = nameToID("MissionGroup/AllPlayers/teamTwoPlayers");
   %teamGroupID.add(%player); 
   }
}
