//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------


//------------------------------------------------------------------------------
// Hard coded images referenced from C++ code
//------------------------------------------------------------------------------

//   editor/SelectHandle.png
//   editor/DefaultHandle.png
//   editor/LockedHandle.png


//------------------------------------------------------------------------------
// Functions
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Mission Editor 
//------------------------------------------------------------------------------

function Editor::create()
{
   // Not much to do here, build it and they will come...
   // Only one thing... the editor is a gui control which
   // expect the Canvas to exist, so it must be constructed
   // before the editor.
   new EditManager(Editor)
   {
      profile = "GuiContentProfile";
      horizSizing = "right";
      vertSizing = "top";
      position = "0 0";
      extent = "640 480";
      minExtent = "8 8";
      visible = "1";
      setFirstResponder = "0";
      modal = "1";
      helpTag = "0";
      open = false;
   };
   
   /// This is the global undo manager used by all
   /// of the mission editor sub-editors.
   new UndoManager( EUndoManager )
   {
      numLevels = 200;         
   };
}


function Editor::onAdd(%this)
{
   // Ignore Replicated fxStatic Instances.
   EWorldEditor.ignoreObjClass("fxShapeReplicatedStatic");

   // do gui initialization...
   EditorGui.init();

   //
}

function Editor::checkActiveLoadDone()
{
   if(isObject(EditorGui) && EditorGui.loadingMission)
   {
      Canvas.setContent(EditorGui);
      EditorGui.loadingMission = false;
      return true;
   }
   return false;
}

//------------------------------------------------------------------------------
function toggleEditor(%make)
{
   if (Canvas.isFullscreen())
   {
      MessageBoxOKOld("Windowed Mode Required", "Please switch to windowed mode to access the Mission Editor.");
      return;
   }
   
   if (%make)
   {      
      %timerId = startPrecisionTimer();
      
      if( $InGuiEditor )
         GuiEdit();
         
      if( !$missionRunning )
         EditorNewLevel();
      else
      {
         pushInstantGroup();
         
         if ( !isObject( Editor ) )
         {
            Editor::create();
            MissionCleanup.add( Editor );
            MissionCleanup.add( EUndoManager );
         }
         
         if( EditorIsActive() )
         {
            if (theLevelInfo.type $= "DemoScene") 
            {
               commandToServer('dropPlayerAtCamera');
               Editor.close("SceneGui");   
            } 
            else 
            {
               Editor.close("PlayGui");
            }
         }
         else 
         {
            canvas.pushDialog(EditorLoadingGui);
            canvas.repaint();
            if (theLevelInfo.type $= "DemoScene")
               commandToServer('dropCameraAtPlayer', true);
               
            Editor.open();
            canvas.popDialog(EditorLoadingGui);
         }
         
         popInstantGroup();
      }
      
      %elapsed = stopPrecisionTimer( %timerId );
      warn( "Time spent in toggleEditor() : " @ %elapsed / 1000.0 @ " s" );
   }
}

//------------------------------------------------------------------------------
//  The editor action maps are defined in editor.bind.cs
GlobalActionMap.bind(keyboard, "f11", toggleEditor);
