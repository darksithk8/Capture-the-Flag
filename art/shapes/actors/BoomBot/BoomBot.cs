//-----------------------------------------------------------------------------
// Torque Game Engine Advanced
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//--- OBJECT WRITE BEGIN ---
function BoomBotDts::onLoad(%this)
{
   %this.addSequence("art/shapes/players/animations/player_diehead.dsq", "death1");
   %this.addSequence("art/shapes/players/animations/player_diechest.dsq", "death2");
   %this.addSequence("art/shapes/players/animations/player_dieback.dsq", "death3");
   %this.addSequence("art/shapes/players/animations/player_diesidelf.dsq", "death4");
   %this.addSequence("art/shapes/players/animations/player_diesidert.dsq", "death5");
   %this.addSequence("art/shapes/players/animations/player_dieleglf.dsq", "death6");
   %this.addSequence("art/shapes/players/animations/player_dielegrt.dsq", "death7");
   %this.addSequence("art/shapes/players/animations/player_dieslump.dsq", "death8");
   %this.addSequence("art/shapes/players/animations/player_dieknees.dsq", "death9");
   %this.addSequence("art/shapes/players/animations/player_dieforward.dsq", "death10");
   %this.addSequence("art/shapes/players/animations/player_diespin.dsq", "death11");
   %this.addSequence("art/shapes/players/animations/player_looksn.dsq", "looksn");
   %this.addSequence("art/shapes/players/animations/player_lookms.dsq", "lookms");
   %this.addSequence("art/shapes/players/animations/player_scoutroot.dsq", "scoutroot");
   %this.addSequence("art/shapes/players/animations/player_sitting.dsq", "sitting");
   %this.addSequence("art/shapes/players/animations/player_celsalute.dsq", "celsalute");
   %this.addSequence("art/shapes/players/animations/player_celwave.dsq", "celwave");
   %this.addSequence("art/shapes/players/animations/player_looknw.dsq", "looknw");
   %this.addSequence("art/shapes/players/animations/player_dance.dsq", "dance");
   %this.addSequence("art/shapes/players/animations/player_range.dsq", "range");
   %this.addNode("myNode1", "Spine2", "0 0 0 0 0 1 0");
   %this.renameNode("myNode1", "mount1");
   %this.setNodeTransform("mount1", "-0.347754 -0.316493 1.87151 0.73453 0.583784 0.345921 2.69067", "1");
   %this.renameNode("mount1", "mount2");
   %this.addNode("myNode1", "LeftHandEnd", "0 0 0 0 0 1 0");
   %this.setNodeTransform("myNode1", "0.249854 0.632574 1.24725 -0.199984 0.744945 0.636446 2.36842", "1");
   %this.renameNode("myNode1", "mount1");
   %this.setNodeTransform("mount2", "-0.347754 -0.316493 1.87151 -0.192967 0.540733 -0.818762 0.448972", "1");
}

singleton TSShapeConstructor(BoomBotDts)
{
   baseShape = "art/shapes/players/BoomBot/BoomBot.dts";
   upAxis = "DEFAULT";
   unit = "-1";
   canSaveDynamicFields = "1";
};
//--- OBJECT WRITE END ---
