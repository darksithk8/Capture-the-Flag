$Team1Scores = 0;
$Team1PointsForCapture = 5;
//-----------------------------------------------------------
//NEW FLAG CAPTURE TRIGGER
//-----------------------------------------------------------
// Trigger datablock
//-----------------------------------------------------------
datablock TriggerData(CapFlagTrigger1)
{
    tickPeriodMS = 100;
    team = -1;
    teamEmitter = 0;
};

function CapFlagTrigger1::onEnterTrigger( %this, %trigger, %obj )
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
   
   if (%obj.team == 2)
   {
      %obj.mountobject (Flag1SS, 1);
      $team1flagcaptured = true;
      $team2flagcarrier = %obj;
   }
   if ((%obj.team == 1) && (isObject(%obj.getMountedObject(0))))
    { 
       if (%obj.getMountedObject(0).getName() == (Flag2SS))
       {
         %obj.unmountobject (Flag2SS);
         Flag2SS.setTransform (TriggerOfTeam2.getPosition());
         
         messageAll( 'MsgObjective', '\c2Team %1 captured a flag! %2 points!', %obj.team, $Team1PointsForCapture );
         $Team1Scores = ($Team1PointsForCapture + $Team1Scores);
         messageAll( 'MsgObjective', '\c2Team %1 Have points = %2', %obj.team, $Team1Scores );
         $team2flagcaptured = false;
       }
   }
   if ($Team1Scores == 5)
   loadTeamWon1();
}