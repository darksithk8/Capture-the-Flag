//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

$Pref::WorldEditor::FileSpec = "Torque Mission Files (*.mis)|*.mis|All Files (*.*)|*.*|";

//////////////////////////////////////////////////////////////////////////
// File Menu Handlers
//////////////////////////////////////////////////////////////////////////

function EditorFileMenu::onMenuSelect(%this)
{
   // don't do this since it won't exist if this is a "demo"
   if(!isWebDemo())
      %this.enableItem(2, EditorIsDirty());
}

//////////////////////////////////////////////////////////////////////////

// Package that gets temporarily activated to toggle editor after mission loading.
// Deactivates itself.
package BootEditor {

function GameConnection::initialControlSet( %this )
{
   Parent::initialControlSet( %this );
   
   toggleEditor( true );
   deactivatePackage( "BootEditor" );
}

};

//////////////////////////////////////////////////////////////////////////

function EditorIsActive()
{
   return ( Canvas.getContent() == EditorGui.getId() );
}

/// Checks the various dirty flags and returns true if the 
/// mission or other related resources need to be saved.  
function EditorIsDirty()
{
   // We kept a hard coded test here, but we could break these
   // into the registered tools if we wanted to.
   %isDirty =  ( isObject( "ETerrainEditor" ) && ( ETerrainEditor.isMissionDirty || ETerrainEditor.isDirty ) )
               || ( isObject( "EWorldEditor" ) && EWorldEditor.isDirty )
               || ( isObject( "ETerrainPersistMan" ) && ETerrainPersistMan.hasDirty() );
   
   // Give the editor plugins a chance to set the dirty flag.
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject(%i);
      %isDirty |= %obj.isDirty(); 
   }
   
   return %isDirty;
}

/// Clears all the dirty state without saving.
function EditorClearDirty()
{
   EWorldEditor.isDirty = false;
   ETerrainEditor.isDirty = false;
   ETerrainEditor.isMissionDirty = false;
   ETerrainPersistMan.clearAll();
   
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject(%i);
      %obj.clearDirty();      
   }
}

function EditorQuitGame()
{
   if( EditorIsDirty() && !isWebDemo())
   {
      MessageBoxYesNoCancel("Level Modified", "Would you like to save your changes before quitting?", "EditorSaveMissionMenu(); quit();", "quit();", "" );
   }
   else
      quit();
}

function EditorExitMission()
{  
   if( EditorIsDirty() && !isWebDemo() )
   {
      MessageBoxYesNoCancel("Level Modified", "Would you like to save your changes before exiting?", "EditorDoExitMission(true);", "EditorDoExitMission(false);", "");
   }
   else
      EditorDoExitMission(false);
}

function EditorDoExitMission(%saveFirst)
{
   if(%saveFirst && !isWebDemo())
   {
      EditorSaveMissionMenu();
   }
   else
   {
      EditorClearDirty();
   }
   if (isObject( MainMenuGui ))
      Editor.close("MainMenuGui");
   else if (isObject( UnifiedMainMenuGui ))
      Editor.close("UnifiedMainMenuGui");
   disconnect();
}

function EditorNewLevel( %file )
{
   if(isWebDemo())
      return;
      
   %saveFirst = false;
   if ( EditorIsDirty() )
   {
      error(knob);
      %saveFirst = MessageBox("Mission Modified", "Would you like to save changes to the current mission \"" @
         $Server::MissionFile @ "\" before creating a new mission?", "SaveDontSave", "Question") == $MROk;
   }
      
   if(%saveFirst)
      EditorSaveMission();

   // Clear dirty flags first to avoid duplicate dialog box from EditorOpenMission()
   EditorClearDirty();
   EUndoManager.clearAll();

   if( %file $= "" )
      %file = "tools/levels/BlankRoom.mis";

   if( !$missionRunning )
   {
      activatePackage( "BootEditor" );
      StartLevel( %file );
   }
   else
      EditorOpenMission(%file);

   //EWorldEditor.isDirty = true;
   //ETerrainEditor.isDirty = true;
   EditorGui.saveAs = true;
}

function EditorSaveMissionMenu()
{
   if(!$Pref::disableSaving && !isWebDemo())
   {
      if(EditorGui.saveAs)
         EditorSaveMissionAs();
      else
         EditorSaveMission();
   }
   else
   {
      EditorSaveMissionMenuDisableSave();
   }
}

function EditorSaveMission()
{
   // just save the mission without renaming it
   if(isFunction("getObjectLimit") && MissionGroup.getFullCount() >= getObjectLimit())
   {
      MessageBoxOKBuy( "Object Limit Reached", "You have exceeded the object limit of " @ getObjectLimit() @ " for this demo. You can remove objects if you would like to add more.", "", "Canvas.showPurchaseScreen(\"objectlimit\");" );
      return;
   }
   
   // first check for dirty and read-only files:
   if((EWorldEditor.isDirty || ETerrainEditor.isMissionDirty) && !isWriteableFileName($Server::MissionFile))
   {
      MessageBox("Error", "Mission file \""@ $Server::MissionFile @ "\" is read-only.  Continue?", "Ok", "Stop");
      return false;
   }
   if(ETerrainEditor.isDirty)
   {
      // Find all of the terrain files
      initContainerTypeSearch($TypeMasks::TerrainObjectType);

      while ((%terrainObject = containerSearchNext()) != 0)
      {
         if (!isWriteableFileName(%terrainObject.terrainFile))
         {
            if (MessageBox("Error", "Terrain file \""@ %terrainObject.terrainFile @ "\" is read-only.  Continue?", "Ok", "Stop") == $MROk)
               continue;
            else
               return false;
         }
      }
   }
  
   // now write the terrain and mission files out:

   if(EWorldEditor.isDirty || ETerrainEditor.isMissionDirty)
      MissionGroup.save($Server::MissionFile);
   if(ETerrainEditor.isDirty)
   {
      // Find all of the terrain files
      initContainerTypeSearch($TypeMasks::TerrainObjectType);

      while ((%terrainObject = containerSearchNext()) != 0)
         %terrainObject.save(%terrainObject.terrainFile);
   }

   ETerrainPersistMan.saveDirty();
      
   // Give EditorPlugins a chance to save.
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject(%i);
      if ( %obj.isDirty() )
         %obj.onSaveMission( $Server::MissionFile );      
   } 
   
   EditorClearDirty();
   
   EditorGui.saveAs = false;
   
   return true;
}

function EditorSaveMissionMenuDisableSave()
{
   GenericPromptDialog-->GenericPromptWindow.text = "Warning";
   GenericPromptDialog-->GenericPromptText.setText("Saving disabled in demo mode."); 
   Canvas.pushDialog( GenericPromptDialog ); 
}

function EditorSaveMissionAs()
{
   if(isFunction("getObjectLimit") && MissionGroup.getFullCount() >= getObjectLimit())
   {
      MessageBoxOKBuy( "Object Limit Reached", "You have exceeded the object limit of " @ getObjectLimit() @ " for this demo. You can remove objects if you would like to add more.", "", "Canvas.showPurchaseScreen(\"objectlimit\");" );
      return;
   }
   
   if(!$Pref::disableSaving && !isWebDemo())
   {
      %defaultFileName = $Server::MissionFile;
      if( %defaultFileName $= "" )
         %defaultFileName = "levels/untitled.mis";

      %dlg = new SaveFileDialog()
      {
         Filters        = $Pref::WorldEditor::FileSpec;
         DefaultPath    = $Pref::WorldEditor::LastPath;
         DefaultFile    = %defaultFileName;
         ChangePath     = false;
         OverwritePrompt   = true;
      };
            
      %ret = %dlg.Execute();
      if(%ret)
      {
         $Pref::WorldEditor::LastPath = filePath( %dlg.FileName );
         %missionName = %dlg.FileName;
      }
      
      %dlg.delete();
      
      if(! %ret)
         return;
         
      if( fileExt( %missionName ) $= "" )
         %missionName = %missionName @ ".mis";

      EWorldEditor.isDirty = true;
      %saveMissionFile = $Server::MissionFile;

      $Server::MissionFile = %missionName;

      %copyTerrainsFailed = false;
      if( ETerrainEditor.isDirty )
      {
         // Rename all the terrain files.  Save all previous names so we can
         // reset them if saving fails.
         %newMissionName = fileBase(%missionName);
         %oldMissionName = fileBase(%saveMissionFile);

         initContainerTypeSearch( $TypeMasks::TerrainObjectType );
         %savedTerrNames = new ScriptObject();
         for( %i = 0;; %i ++ )
         {
            %terrainObject = containerSearchNext();
            if( !%terrainObject )
               break;

            %savedTerrNames.array[ %i ] = %terrainObject.terrainFile;
            
            %terrainFilePath = filePath( %terrainObject.terrainFile );
            %terrainFileName = fileName( %terrainObject.terrainFile );

            // Try and follow the existing naming convention.
            // If we can't, use systematic terrain file names.
            if( strstr( %terrainFileName, %oldMissionName ) >= 0 )
               %terrainFileName = strreplace( %terrainFileName, %oldMissionName, %newMissionName );
            else
               %terrainFileName = %newMissionName @ "_" @ %i @ ".ter";

            %newTerrainFile = %terrainFilePath @ "/" @ %terrainFileName;

            if (!isWriteableFileName(%newTerrainFile))
            {
               if (MessageBox("Error", "Terrain file \""@ %newTerrainFile @ "\" is read-only.  Continue?", "Ok", "Stop") == $MROk)
                  continue;
               else
               {
                  %copyTerrainsFailed = true;
                  break;
               }
            }
            
            %terrainObject.save( %newTerrainFile );
            %terrainObject.terrainFile = %newTerrainFile;
         }
         ETerrainEditor.isDirty = false;
      }
      
      // Save the mission.
      if(%copyTerrainsFailed || !EditorSaveMission())
      {
         // It failed, so restore the mission and terrain filenames.
         
         $Server::MissionFile = %saveMissionFile;

         initContainerTypeSearch( $TypeMasks::TerrainObjectType );
         for( %i = 0;; %i ++ )
         {
            %terrainObject = containerSearchNext();
            if( !%terrainObject )
               break;
               
            %terrainObject.terrainFile = %savedTerrNames.array[ %i ];
         }
      }
      
      %savedTerrNames.delete();
   }
   else
   {
      EditorSaveMissionMenuDisableSave();
   }
   
}

function EditorOpenMission(%filename)
{
   if( EditorIsDirty() && !isWebDemo() )
   {
      // "EditorSaveBeforeLoad();", "getLoadFilename(\"*.mis\", \"EditorDoLoadMission\");"
      if(MessageBox("Mission Modified", "Would you like to save changes to the current mission \"" @
         $Server::MissionFile @ "\" before opening a new mission?", SaveDontSave, Question) == $MROk)
      {
         if(! EditorSaveMission())
            return;
      }
   }

   if(%filename $= "")
   {
      %defaultFileName = $Server::MissionFile;
      if( %defaultFileName $= "" )
         %defaultFileName = "levels/untitled.mis";

      %dlg = new OpenFileDialog()
      {
         Filters        = $Pref::WorldEditor::FileSpec;
         DefaultPath    = $Pref::WorldEditor::LastPath;
         DefaultFile    = %defaultFileName;
         ChangePath     = false;
         MustExist      = true;
      };
            
      %ret = %dlg.Execute();
      if(%ret)
      {
         $Pref::WorldEditor::LastPath = filePath( %dlg.FileName );
         %filename = %dlg.FileName;
      }
      
      %dlg.delete();
      
      if(! %ret)
         return;
   }
      
   // close the current editor, it will get cleaned up by MissionCleanup
   if( isObject( "Editor" ) )
      Editor.close();

   // If we haven't yet connnected, create a server now.
   // Otherwise just load the mission.

   if( !$missionRunning )
   {
      activatePackage( "BootEditor" );
      StartLevel( %filename );
   }
   else
   {
      loadMission( %filename, true ) ;
   
      pushInstantGroup();

      // recreate and open the editor
      Editor::create();
      MissionCleanup.add( Editor );
      MissionCleanup.add( EUndoManager );
      EditorGui.loadingMission = true;
      Editor.open();
   
      popInstantGroup();
   }
}

//////////////////////////////////////////////////////////////////////////
// Edit Menu Handlers
//////////////////////////////////////////////////////////////////////////

function EditorEditMenu::onMenuSelect( %this )
{
   %selSize = EWorldEditor.getSelectionSize();
   %lockCount = EWorldEditor.getSelectionLockCount();
   %hideCount = EWorldEditor.getSelectionHiddenCount();
   
   %this.enableItem( 6, %selSize > 0 && %lockCount != %selSize ); // Delete Selection
   
   // Let the undo manager do its magic.
   EUndoManager.updateUndoMenu( %this );
   // SICKHEAD: It a perfect world we would abstract 
   // cut/copy/paste with a generic selection object 
   // which would know how to process itself.   
   
   // Give the active editor a chance at fixing up
   // the state of the edit menu.
   // JCF: do we need this check here?
   if ( isObject( EditorGui.currentEditor ) )
      EditorGui.currentEditor.onEditMenuSelect( %this );   
}

//////////////////////////////////////////////////////////////////////////

function EditorMenuEditDelete()
{
   // The tree handles deletion and notifies the
   // world editor to clear its selection.  
   //
   // This is because non-SceneObject elements like
   // SimGroups also need to be destroyed.
   //
   // See EditorTree::onObjectDeleteCompleted().
   %selSize = EWorldEditor.getSelectionSize();
   if( %selSize > 0 )
   {
      EditorTree.deleteSelection();
   }
}

// Note: The original editor didn't implement these either, but leaving stubs since
// we will want to implement them at some point in the future.
function EditorMenuEditSelectAll()
{
}

function EditorMenuEditSelectNone()
{
}

//////////////////////////////////////////////////////////////////////////
// Window Menu Handler
//////////////////////////////////////////////////////////////////////////

function EditorToolsMenu::onSelectItem(%this, %id)
{
   %toolName = getField( %this.item[%id], 2 );  

   EditorGui.setEditor(%toolName, %paletteName  );
   
   %this.checkRadioItem(0, %this.getItemCount(), %id);
}

function EditorToolsMenu::setupDefaultState(%this)
{
   Parent::setupDefaultState(%this);
}

//////////////////////////////////////////////////////////////////////////
// Camera Menu Handler
//////////////////////////////////////////////////////////////////////////

function EditorCameraMenu::onSelectItem(%this, %id, %text)
{
   if(%id == 0 || %id == 1)
   {
      // Handle the Free Camera/Orbit Camera toggle
      %this.checkRadioItem(0, 1, %id);
   }

   Parent::onSelectItem(%this, %id, %text);
}

function EditorCameraMenu::setupDefaultState(%this)
{
   // Set the Free Camera/Orbit Camera check marks
   %this.checkRadioItem(0, 1, 0);
   Parent::setupDefaultState(%this);
}

function EditorFreeCameraTypeMenu::onSelectItem(%this, %id, %text)
{
   // Handle the camera type radio
   %this.checkRadioItem(0, 2, %id);

   Parent::onSelectItem(%this, %id, %text);
}

function EditorFreeCameraTypeMenu::setupDefaultState(%this)
{
   // Set the camera type check marks
   %this.checkRadioItem(0, 2, 0);
   Parent::setupDefaultState(%this);
}

function EditorCameraSpeedMenu::onSelectItem(%this, %id, %text)
{
   // CodeReview - Seriously, comment your magic numbers... -JDD
   // This math is located in two places(here and in EditorGui.ed.cs). Going 
   // to be moved to one function in the camera scritps
   //$Camera::movementSpeed = (%id / 6.0) * 195 + 5;
   
   $Camera::movementSpeed = getField( %this.item[%id], 2 );  
   // Update Editor
   %this.checkRadioItem(0, 6, %id);
   
   // Update toolbar
   CameraSpeedDropdownContainer-->textEdit.setText( $Camera::movementSpeed );
}
function EditorCameraSpeedMenu::setupDefaultState(%this)
{
   %this.onSelectItem(3, getField(%this.item[3], 0));
   Parent::setupDefaultState(%this);
}

//////////////////////////////////////////////////////////////////////////
// World Menu Handler Object Menu
//////////////////////////////////////////////////////////////////////////

function EditorWorldMenu::onMenuSelect(%this)
{
   %selSize = EWorldEditor.getSelectionSize();
   %lockCount = EWorldEditor.getSelectionLockCount();
   %hideCount = EWorldEditor.getSelectionHiddenCount();
   
   %this.enableItem(0, %lockCount < %selSize);  // Lock Selection
   %this.enableItem(1, %lockCount > 0);  // Unlock Selection
   %this.enableItem(3, %hideCount < %selSize);  // Hide Selection
   %this.enableItem(4, %hideCount > 0);  // Show Selection
   %this.enableItem(6, %selSize > 1 && %lockCount == 0);  // Align bounds
   %this.enableItem(7, %selSize > 1 && %lockCount == 0);  // Align center
   %this.enableItem(9, %selSize > 0 && %lockCount == 0);  // Reset Transforms
   %this.enableItem(10, %selSize > 0 && %lockCount == 0);  // Reset Selected Rotation
   %this.enableItem(11, %selSize > 0 && %lockCount == 0);  // Reset Selected Scale
   %this.enableItem(13, %selSize > 0);  // Camera To Selection
   %this.enableItem(14, %selSize > 0);  // Add to instant group
   %this.enableItem(15, %selSize > 0 && %lockCount == 0);  // Drop Selection
   //%this.enableItem(18, %lockCount == 0);  // Delete Selection
   
}

//////////////////////////////////////////////////////////////////////////

function EditorDropTypeMenu::onSelectItem(%this, %id, %text)
{
   // This sets up which drop script function to use when
   // a drop type is selected in the menu.
   EWorldEditor.dropType = getField(%this.item[%id], 2);
   
   %this.checkRadioItem(0, (%this.getItemCount() - 1), %id);
}

function EditorDropTypeMenu::setupDefaultState(%this)
{
   // Check the radio item for the currently set drop type.
   
   %numItems = %this.getItemCount();
   
   %dropTypeIndex = 0;
   for( ; %dropTypeIndex < %numItems; %dropTypeIndex ++ )
      if( getField( %this.item[ %dropTypeIndex ], 2 ) $= EWorldEditor.dropType )
         break;
 
   // Default to screenCenter if we didn't match anything.        
   if( %dropTypeIndex > (%numItems - 1) )
      %dropTypeIndex = 4;
   
   %this.checkRadioItem( 0, (%numItems - 1), %dropTypeIndex );
      
   Parent::setupDefaultState(%this);
}

//////////////////////////////////////////////////////////////////////////

function EditorAlignBoundsMenu::onSelectItem(%this, %id, %text)
{
   // Have the editor align all selected objects by the selected bounds.
   EWorldEditor.alignByBounds(getField(%this.item[%id], 2));
}

function EditorAlignBoundsMenu::setupDefaultState(%this)
{
   // Allow the parent to set the menu's default state
   Parent::setupDefaultState(%this);
}

//////////////////////////////////////////////////////////////////////////

function EditorAlignCenterMenu::onSelectItem(%this, %id, %text)
{
   // Have the editor align all selected objects by the selected axis.
   EWorldEditor.alignByAxis(getField(%this.item[%id], 2));
}

function EditorAlignCenterMenu::setupDefaultState(%this)
{
   // Allow the parent to set the menu's default state
   Parent::setupDefaultState(%this);
}
