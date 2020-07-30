//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// DefaultTrigger is used by the mission editor.  This is also an example
// of trigger methods and callbacks.

function DefaultTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   // This method is called whenever an object enters the %trigger
   // area, the object is passed as %obj.  The default onEnterTrigger
   // method (in the C++ code) invokes the ::onTrigger(%trigger,1) method on
   // every object (whatever it's type) in the same group as the trigger.
   Parent::onEnterTrigger(%this,%trigger,%obj);
}

function DefaultTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   // This method is called whenever an object leaves the %trigger
   // area, the object is passed as %obj.  The default onLeaveTrigger
   // method (in the C++ code) invokes the ::onTrigger(%trigger,0) method on
   // every object (whatever it's type) in the same group as the trigger.
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}