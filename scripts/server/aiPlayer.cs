//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// AIPlayer callbacks
// The AIPlayer class implements the following callbacks:
//
//    PlayerData::onStuck(%this,%obj)
//    PlayerData::onUnStuck(%this,%obj)
//    PlayerData::onStop(%this,%obj)
//    PlayerData::onMove(%this,%obj)
//    PlayerData::onReachDestination(%this,%obj)
//    PlayerData::onTargetEnterLOS(%this,%obj)
//    PlayerData::onTargetExitLOS(%this,%obj)
//    PlayerData::onAdd(%this,%obj)
//
// Since the AIPlayer doesn't implement it's own datablock, these callbacks
// all take place in the PlayerData namespace.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Demo Pathed AIPlayer.
//----------------------------------------------------------------------------


function DemoPlayer::onReachDestination(%this,%obj)
{
   // Moves to the next node on the path.
   // Override for all player.  Normally we'd override this for only
   // a specific player datablock or class of players.
   if (%obj.path !$= "")
   {
      if (%obj.currentNode == %obj.targetNode)
         %this.onEndOfPath(%obj,%obj.path);
      else
         %obj.moveToNextNode();
   }
}

function DemoPlayer::onEndOfPath(%this,%obj,%path)
{
   %obj.nextTask();
}

function DemoPlayer::onEndSequence(%this,%obj,%slot)
{
   echo("Sequence Done!");
   %obj.stopThread(%slot);
   %obj.nextTask();
}

//-----------------------------------------------------------------------------
// AIPlayer static functions
//-----------------------------------------------------------------------------

function AIPlayer::spawn(%name,%spawnPoint)
{
   // Create the demo player object
   %player = new AiPlayer()
   {
      dataBlock = DemoPlayer;
      path = "";
   };
   MissionCleanup.add(%player);
   %player.setShapeName(%name);
   %player.setTransform(%spawnPoint);
   return %player;
}

function AIPlayer::spawnOnPath(%name,%path)
{
   // Spawn a player and place him on the first node of the path
   if (!isObject(%path))
      return 0;
   %node = %path.getObject(0);
   %player = AIPlayer::spawn(%name, %node.getTransform());
   return %player;
}

//-----------------------------------------------------------------------------
// AIPlayer methods
//-----------------------------------------------------------------------------

function AIPlayer::followPath(%this,%path,%node)
{
   // Start the player following a path
   %this.stopThread(0);
   if (!isObject(%path))
   {
      %this.path = "";
      return;
   }

   if (%node > %path.getCount() - 1)
      %this.targetNode = %path.getCount() - 1;
   else
      %this.targetNode = %node;

   if (%this.path $= %path)
      %this.moveToNode(%this.currentNode);
   else
   {
      %this.path = %path;
      %this.moveToNode(0);
   }
}

function AIPlayer::moveToNextNode(%this)
{
   if (%this.targetNode < 0 || %this.currentNode < %this.targetNode)
   {
      if (%this.currentNode < %this.path.getCount() - 1)
         %this.moveToNode(%this.currentNode + 1);
      else
         %this.moveToNode(0);
   }
   else
      if (%this.currentNode == 0)
         %this.moveToNode(%this.path.getCount() - 1);
      else
         %this.moveToNode(%this.currentNode - 1);
}

function AIPlayer::moveToNode(%this,%index)
{
   // Move to the given path node index
   %this.currentNode = %index;
   %node = %this.path.getObject(%index);
   %this.setMoveDestination(%node.getTransform(), %index == %this.targetNode);
}

//-----------------------------------------------------------------------------
//
//-----------------------------------------------------------------------------

function AIPlayer::pushTask(%this,%method)
{
   if (%this.taskIndex $= "")
   {
      %this.taskIndex = 0;
      %this.taskCurrent = -1;
   }
   %this.task[%this.taskIndex] = %method;
   %this.taskIndex++;
   if (%this.taskCurrent == -1)
      %this.executeTask(%this.taskIndex - 1);
}

function AIPlayer::clearTasks(%this)
{
   %this.taskIndex = 0;
   %this.taskCurrent = -1;
}

function AIPlayer::nextTask(%this)
{
   if (%this.taskCurrent != -1)
      if (%this.taskCurrent < %this.taskIndex - 1)
         %this.executeTask(%this.taskCurrent++);
      else
         %this.taskCurrent = -1;
}

function AIPlayer::executeTask(%this,%index)
{
   %this.taskCurrent = %index;
   eval(%this.getId() @"."@ %this.task[%index] @";");
}

//-----------------------------------------------------------------------------

function AIPlayer::singleShot(%this)
{
   // The shooting delay is used to pulse the trigger
   %this.setImageTrigger(0, true);
   %this.setImageTrigger(0, false);
   %this.trigger = %this.schedule(%this.shootingDelay, singleShot);
}

//-----------------------------------------------------------------------------

function AIPlayer::wait(%this, %time)
{
   %this.schedule(%time * 1000, "nextTask");
}

function AIPlayer::done(%this,%time)
{
   %this.schedule(0, "delete");
}

function AIPlayer::fire(%this,%bool)
{
   if (%bool)
   {
      cancel(%this.trigger);
      %this.singleShot();
   }
   else
      cancel(%this.trigger);
   %this.nextTask();
}

function AIPlayer::aimAt(%this,%object)
{
   echo("Aim: "@ %object);
   %this.setAimObject(%object);
   %this.nextTask();
}

function AIPlayer::animate(%this,%seq)
{
   //%this.stopThread(0);
   //%this.playThread(0,%seq);
   %this.setActionThread(%seq);
}

// ----------------------------------------------------------------------------
// Some handy getDistance/nearestTarget functions for the AI to use
// ----------------------------------------------------------------------------

function AIPlayer::getTargetDistance(%this, %target)
{
   echo("\c4AIPlayer::getTargetDistance("@ %this @", "@ %target @")");
   $tgt = %target;
   %tgtPos = %target.getPosition();
   %eyePoint = %this.getWorldBoxCenter();
   %distance = VectorDist(%tgtPos, %eyePoint);
   echo("Distance to target = "@ %distance);
   return %distance;
}

function AIPlayer::getNearestPlayerTarget(%this)
{
   echo("\c4AIPlayer::getNearestPlayerTarget("@ %this @")");

   %index = -1;
   %botPos = %this.getPosition();
   %count = ClientGroup.getCount();
   for(%i = 0; %i < %count; %i++)
   {
      %client = ClientGroup.getObject(%i);
      if (%client.player $= "" || %client.player == 0)
         return -1;
      %playerPos = %client.player.getPosition();

      %tempDist = VectorDist(%playerPos, %botPos);
      if (%i == 0)
      {
         %dist = %tempDist;
         %index = %i;
      }
      else
      {
         if (%dist > %tempDist)
         {
            %dist = %tempDist;
            %index = %i;
         }
      }
   }
   return %index;
}

//-----------------------------------------------------------------------------

function AIManager::think(%this)
{
   // We could hook into the player's onDestroyed state instead of having to
   // "think", but thinking allows us to consider other things...
   if (!isObject(%this.player))
      %this.player = %this.spawn();
   %this.schedule(500, think);
}

function AIManager::spawn(%this)
{
   %player = AIPlayer::spawnOnPath("Shootme", "MissionGroup/Paths/Path1");

   if (isObject(%player))
   {
      %player.followPath("MissionGroup/Paths/Path1", -1);

      // slow this sucker down, I'm tired of chasing him!
      %player.setMoveSpeed(0.5);

      //%player.mountImage(xxxImage, 0);
      //%player.setInventory(xxxAmmo, 1000);

      return %player;
   }
   else
      return 0;
}

function botThink(%botID, %action)
{
   if(%botID.team == 1)
   {
      %enemyGroupID = nameToID("MissionGroup/AllPlayers/teamTwoPlayers");
   }
   else if(%botID.team == 2)
   {
      %enemyGroupID = nameToID("MissionGroup/AllPlayers/teamOnePlayers");
   }
   // Odleglosc z jakiej beda strzelac do siebie:
   %closestDistance = 1000; 
   for(%i=0; %i<%enemyGroupID.getCount(); %i++)
   {             
      %mask = $TypeMasks::PlayerObjectType;
      %scanTarg 	= containerRayCast(%botID.getTransform(), %enemyGroupID.getObject(%i).getTransform(), %mask, %botID);  
      if(getWord(%scanTarg, 0) == %enemyGroupID.getObject(%i))
      {      
         %distance = vectorDist(%enemyGroupID.getObject(%i).getTransform(), %botID.getTransform());
         if(%distance < %closestDistance)
         {
            %closestDistance = %distance;
            %closestBot = %enemyGroupID.getObject(%i);
         }
      }
   }
      // Aiming at enemy:
      %botID.setAimObject(%closestBot, "0 0 2");

//DECADING WHAT BOT DOES--------------------------------------------------------------------------------: 
//%botID.setMoveDestination(%closestBot.getTransform());
      if (%action == 1)
      {
         if (%botID.team == 1)
         {
            if ($team2flagcaptured == false)
            {
               botAttackFlag(%botID);
            }
            else if ($team2flagcaptured == true)
            {
               if (isObject($team1flagcarrier))
               {
                  botReturnFlag(%botID);
               }
               else
               {  
                  botAttackFlag(%botID);
               }
            }
         }
         else if (%botID.team == 2)
         {
            if ($team1flagcaptured == false)
            {
               botAttackFlag(%botID);
            }
            else if ($team1flagcaptured == true)
            {
               if (isObject($team2flagcarrier))
               {
                  botReturnFlag(%botID);
               }
               else
               {  
                  botAttackFlag(%botID);
               }
            }
         }
      }
      else if (%action == 2)
      {
         botDefendFlag(%botID);
      }
// How often my think function runs:
   %botID.thinkSchedule = schedule(500, %botID, botThink, %botID, %action);
}
// ATTACKING --------------------------------------------------------------------------------:
function botAttackFlag(%botID)
{
   if(%botID.team == 1)
   {
      %botID.setMoveDestination(Flag2SS.getPosition(), false);
   }
   
   else if (%botID.team == 2)
   {
      %botID.setMoveDestination(Flag1SS.getPosition(), false);
   }
   %botID.setShapeName("Attacking!");
}
// RETURNING --------------------------------------------------------------------------------:
// && %botID.getState() !$= "Dead" - that was inside of brackets of team == 1
function botReturnFlag(%botID)
   { 
      if(%botID.team == 1)
      {
            %botID.setMoveDestination(TriggerOfTeam1.getPosition(), false);
      }
      else if (%botID.team == 2)
      {
            %botID.setMoveDestination(TriggerOfTeam2.getPosition(), false);
      }
 %botID.setShapeName("Haha We got ur flag!");
   }
// DEFENDING --------------------------------------------------------------------------------:
function botDefendFlag(%botID, %closestBot)
  {
   if (%botID.team == 1)
   {
      if ($team1flagcaptured == false)
      {
         %x = getWord(TriggerOfTeam1.gettransform(),0);
         %y = getWord(TriggerOfTeam1.gettransform(),1);
         %rest = getwords(TriggerOfTeam1.gettransform(),2,5);
         
         %x = %x + getrandom(-40,40);
         %y = %y + getrandom(-40,40);
        
      // Should move somewhere within a 20X20 space around the Flag:
      %botID.setmovedestination(%x SPC %y SPC %rest, true);
      %botID.setShapeName("Defending 1!");
      }
      else if ($team1flagcaptured == true)
      {
         if (isObject($team2flagcarrier))
         {
            %botID.setMoveDestination($team2flagcarrier.getPosition(), false);
            %botID.setAimObject($team2flagcarrier, "0 0 2");
            %botID.setShapeName("Give my flag back!");
         }
         else
         {
            %x = getWord(TriggerOfTeam1.gettransform(),0);
            %y = getWord(TriggerOfTeam1.gettransform(),1);
            %rest = getwords(TriggerOfTeam1.gettransform(),2,5);
         
            %x = %x + getrandom(-40,40);
            %y = %y + getrandom(-40,40);
        
            // Should move somewhere within a 20X20 space around the Flag:
            %botID.setmovedestination(%x SPC %y SPC %rest, true);
            %botID.setShapeName("Defending 2!");
         }
      }
   }
   else if (%botID.team == 2)
   {
      if ($team2flagcaptured == false)
      {
         %x = getWord(TriggerOfTeam2.gettransform(),0);
         %y = getWord(TriggerOfTeam2.gettransform(),1);
         %rest = getwords(TriggerOfTeam2.gettransform(),2,5);
         
         %x = %x + getrandom(-40,40);
         %y = %y + getrandom(-40,40);
        
     // Should move somewhere within a 20X20 space around the Flag:
      %botID.setmovedestination(%x SPC %y SPC %rest, true);
      %botID.setShapeName("Defending 1!");
      }
      else if ($team2flagcaptured == true)
      {
          if (isObject($team1flagcarrier))
          {
            %botID.setMoveDestination($team1flagcarrier.getPosition(), false);
            %botID.setAimObject(%closestBot, "0 0 2");
            %botID.setShapeName("Give my flag back!");
          }
          else
          {
            %x = getWord(TriggerOfTeam2.gettransform(),0);
            %y = getWord(TriggerOfTeam2.gettransform(),1);
            %rest = getwords(TriggerOfTeam2.gettransform(),2,5);
            
            %x = %x + getrandom(-40,40);
            %y = %y + getrandom(-40,40);
        
            // Should move somewhere within a 20X20 space around the Flag:
            %botID.setmovedestination(%x SPC %y SPC %rest, true);
            %botID.setShapeName("Defending 2!");
          }
      }
   }
      %botID.setAimObject(%closestBot);
      %botID.thinkSchedule = schedule(500, %botID, botDefendFlag, %botID);
   }
// RESPAWNING --------------------------------------------------------------------------------:   
function botRespawn()
{
   %GroupID = nameToID("MissionGroup/AllPlayers/teamOnePlayers");
   %loopNum = 8 - %GroupID.getCount();
   for(%i=0; %i<%loopNum; %i++)
   {
      createBot(1, "One");
   }
   %GroupID = nameToID("MissionGroup/AllPlayers/teamTwoPlayers");
   %loopNum = 8 - %GroupID.getCount();
   for(%i=0; %i<%loopNum; %i++)
   {
      createBot(2, "Two");
   }     
}

function PlayerData::onTargetEnterLOS(%this, %obj)
   {
      %obj.setImageTrigger(0, true);
      %obj.setShapeName("Open Fire");
   }
function PlayerData::onTargetExitLOS(%this,%obj)
{
      %obj.setImageTrigger(0, false);
      %obj.setShapeName("No target");
}