// Trigger count

$Game::TeamObjectives[1] = 0;
$Game::TeamObjectives[2] = 0;
$Game::TeamScores[1] = 0;
$Game::TeamScores[2] = 0;
$Game::TeamFlagCaptures[1] = 0;
$Game::TeamFlagCaptures[2] = 0;
$Game::PointsForCapture = 50;

//-----------------------------------------------------------
//NEW FLAG CAPTURE TRIGGER
//--------------------------------------------------------------
// Trigger datablock
//--------------------------------------------------------------
datablock TriggerData(CapFlagTrigger)
{
    tickPeriodMS = 100;
    team = -1;
    teamEmitter = 0;
    pointsforcapture = 50;
};

//--------------------------------------------------------------
// Events
//--------------------------------------------------------------
function CapFlagTrigger::onEnterTrigger( %this, %trigger, %obj )
{
   // This method is called whenever an object enters the %trigger
   // area, the object is passed as %obj.  The default onEnterTrigger
   // method (in the C++ code) invokes the ::onTrigger(%trigger,1) method on
   // every object (whatever it's type) in the same group as the trigger.
   Parent::onEnterTrigger( %this, %trigger, %obj );
   //SO the trigger requires 3 dynamic variable when placing
   //invcheckitem  = Flag1 OR Flag2 OR whatever it is looking for in the players
   //inv and
   //team = 1 or 2 or whatever you labled the team property
   // and
   //respawnat = the name of a marker object
   echo("ENTERING A CapFLAG TRIGGER 1");
   // check to see if the lad who is in this trigger is on the our team
  if( %obj.team == %trigger.team )
   {
    echo("This is your team");
		// ok, its one of our lads.. see if he has the enemy flag?
		echo(%trigger.invcheckitem);
		if (%obj.getInventory(%trigger.invcheckitem) > 0)
		{
            echo("You Scored");
			// cool.. we got a capture!
			$Game::TeamFlagCaptures[%obj.client.team.teamID]++;
			// make a hoot about capping.
			messageAll( 'MsgObjective', '\c2Team %1 captured a flag! %2 points!', %obj.team,$Game::PointsForCapture );
			$Game::TeamScores[%obj.team] += $Game::PointsForCapture;
			// take the flag out the guys inventory.
			%obj.setInventory(%trigger.invcheckitem,0);
			%obj.unmountImage($FlagSlot);
			// now return the flag..
            echo("ENTERING A CapFLAG TRIGGER return 2");
            newflag(%trigger.invcheckitem,%trigger.respawnat);
		}
	}
}