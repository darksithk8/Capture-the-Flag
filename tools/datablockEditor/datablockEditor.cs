function DatablockEditorPlugin::init(%this)
{
   // first lets populate our datablock tree
   %this.populateTrees();
}

// Material Editor
function DatablockEditorPlugin::onWorldEditorStartup( %this )
{
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Datablock Editor", "", DatablockEditorPlugin );
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Datablock Editor (" @ %accel @ ")"; 
   EditorGui.addToToolsToolbar( "DatablockEditorPlugin", "DatablockEditorPalette", expandFilename("tools/worldEditor/images/toolbar/datablock-editor"), %tooltip );

   //connect editor windows
   AttachWindows( DatablockEditorInspectorWindow, DatablockEditorTreeWindow);
}

function DatablockEditorPlugin::isDirty( %this )
{
   %dirty = %this.PM.hasDirty();
   return %dirty;
}

//--------------------------------------------
// Settings
//
// Here we init, read, and write our settings
// for this editor
//--------------------------------------------
function DatablockEditorPlugin::initSettings(%this)
{
   EditorSettings.beginGroup("DatablockEditor", true);
   
   EditorSettings.setDefaultValue("libraryTab", "0");
   
   EditorSettings.endGroup();
}

function DatablockEditorPlugin::readSettings(%this)
{
   EditorSettings.beginGroup("DatablockEditor", true);
   
   DatablockEditorTreeTabBook.selectPage(EditorSettings.value("libraryTab"));
   %this.selectDatablock(EditorSettings.value("selectedDatablock"));
   
   EditorSettings.endGroup();  
}

function DatablockEditorPlugin::writeSettings(%this)
{
   EditorSettings.beginGroup("DatablockEditor", true);
   
   EditorSettings.setValue("libraryTab", DatablockEditorTreeTabBook.getSelectedPage());
   if(isObject(%this.currentDatablock))
      EditorSettings.setValue("selectedDatablock", %this.currentDatablock.getName());

   EditorSettings.endGroup();
}

function DatablockEditorPlugin::populateTrees(%this)
{
   // Populate datablock tree.
   
   if(isObject(DataBlockGroup))
   {
      DatablockEditorTree.clear();
      for(%i=0; %i<DataBlockGroup.getCount(); %i++)
      {
         %datablock = DataBlockGroup.getObject(%i);

         for(%k=0; %k< UnlistedDatablocks.count(); %k++ )
         {
            %unlistedFound = 0;
            if( UnlistedDatablocks.getValue(%k) $= %datablock.getName() )
            {
               %unlistedFound = 1;
               break;
            }
         }
      
         if( %unlistedFound )
            continue;
            
         %this.addExistingItem( %datablock, true );
      }
      
      DatablockEditorTree.sort( 0, true, false, false );
   }
   
   // Populate datablock type tree.
   
   %this.datablockTypeList = enumerateConsoleClasses(SimDatablock);
   DatablockEditorTypeTree.clear();
   for(%i=0; %i<getWordCount(%this.datablockTypeList); %i++)
   {
      %datablock = getWord(%this.datablockTypeList, %i);
      
      if(%datablock !$= "SimDatablock" && DatablockEditorTypeTree.findItemByName(%datablock) == 0)
         DatablockEditorTypeTree.insertItem(0, %datablock);
   }
   DatablockEditorTypeTree.sort();
   
   if(isObject(%this.currentDatablock))
   {
      %this.selectDatablock(%this.currentDatablock);
      DatablockEditorTree.scrollVisible(DatablockEditorTree.findItemByName(%this.currentDatablock.getName()));
   }
}

function DatablockEditorPlugin::addExistingItem( %this, %datablock, %dontSort )
{
   %class = %datablock.getClassName();
   %parentID = DatablockEditorTree.findItemByName(%class);
        
   if(%parentID == 0)
      %parentID = DatablockEditorTree.insertItem(0, %class);
         
   %name = %datablock.getName();
         
   if(%name $= "")
      %name = "unnamed";
   
   %id = DatablockEditorTree.findItemByName(%name);
   if(%id == 0)
      %id = DatablockEditorTree.findItemByName(%name @ " *");
      
   // it doesn't exist so lets add it
   if(%id == 0)
   {
      if(%this.PM.isDirty(%datablock))
         %name = %name @ " *";
         
      %id = DatablockEditorTree.insertItem(%parentID, %name, %datablock);
      if( !%dontSort )
         DatablockEditorTree.sort( %parentID, false, false, false );
   }
         
   return %id;   
}

function DatablockEditorPlugin::flagInspectorAsDirty(%this, %dirty)
{
   if(%dirty)
   {
      DatablockEditorInspectorWindow.text = "Datablock *";
   } else
   {
      DatablockEditorInspectorWindow.text = "Datablock";
   }
   
   %this.flagItemAsDirty(%dirty, %this.currentDatablock);
}

function DatablockEditorPlugin::flagItemAsDirty(%this, %dirty, %datablock)
{
   if(%dirty)
   {
      %id = DatablockEditorTree.findItemByName(%datablock.getName());
      if(%id != 0)
         DatablockEditorTree.editItem(%id, %datablock.getName() @ " *", %datablock);
   } else
   {
      %id = DatablockEditorTree.findItemByName(%datablock.getName() @ " *");
      if(%id != 0)
         DatablockEditorTree.editItem(%id, %datablock.getName(), %datablock); 
   }
}

function DatablockEditorPlugin::resetSelectedDatablock(%this)
{
   %this.lastDatablock = "";
   %this.currentDatablock = "";
   DatablockEditorInspector.inspect(0);   
   DatablockEditorInspectorWindow-->DatablockFile.setText("");     
}

function DatablockEditorPlugin::selectDatablockCheck(%this, %datablock)
{
   if(!isObject(%datablock))
      return;
   
   if(isObject(%this.currentDatablock) && %this.PM.isDirty(%this.currentDatablock) && %this.currentDatablock != %datablock)
      %this.showSaveDialog(%datablock);
   else
      %this.selectDatablock(%datablock);
}

function DatablockEditorPlugin::selectDatablock(%this, %datablock)
{
   if(!isObject(%datablock))
      return;
   
   if(isObject(%this.currentDatablock) && %this.currentDatablock != %this.lastDatablock)
      %this.lastDatablock = %this.currentDatablock;
      
   %this.currentDatablock = %datablock;    
   
   DatablockEditorInspectorWindow-->DatablockFile.setText(%datablock.getFilename());
   DatablockEditorInspector.inspect(%datablock);  
   
   %id = DatablockEditorTree.findItemByName(%datablock.getName());
   if(%id == 0)
      %id = DatablockEditorTree.findItemByName(%datablock.getName() @ " *");
   
   if( DatablockEditorTree.getSelectedItem() != %id && %id != 0 )
   {
      DatablockEditorTree.selectItem(%id);
      DatablockEditorTree.scrollVisible(%id);
   }
   
   %this.flagInspectorAsDirty(%this.PM.isDirty(%this.currentDatablock));
}

function DatablockEditorPlugin::onSelect( %this, %obj )
{
   //DatablockEditorTree.clearSelection();
   //DatablockEditorTree.addSelection( %obj );         
   DatablockInspector.inspect( %obj );
}

function DatablockEditorPlugin::onUnSelect( %this, %obj )
{
   if ( DatablockInspector.getInspectObject() == %obj )
      DatablockInspector.inspect( 0 );
            
   DatablockEditorTree.removeSelection(%obj);
}

function DatablockEditorPlugin::showSaveNewFileDialog(%this)
{
   %currentFile = %this.currentDatablock.getFilename();
   getSaveFilename(%currentFile, %this @ ".saveNewFileFinish", %currentFile, false);
}

function DatablockEditorPlugin::saveNewFileFinish(%this, %file)
{
   %filename = %file;
   %search = "/game";
   %pos = strstr(%file, %search);
   if(%pos != -1)
   {
      %startPos = %pos + strlen(%search) + 1;
      %filename = getSubStr(%file, %startPos, strlen(%file) - %startPos);
   }
   
   %this.PM.removeObjectFromFile(%this.currentDatablock, %this.currentDatablock.getFilename());
   save();
   //%this.currentDatablock.setFilename(%filename);
   %this.PM.setDirty(%this.currentDatablock, %filename);
   %this.save();
   %this.selectDatablock(%this.currentDatablock);
}

function DatablockEditorPlugin::showSaveDialog(%this, %datablock)
{
   if(%datablock $= "")
   {
      MessageBoxYesNo("Save Datablock?", 
         "This datablock has unsaved changes. <br>Do you want to save?", 
         "DatablockEditorPlugin.saveDialogSave();", 
         "DatablockEditorPlugin.saveDialogDontSave();");
   }
   else
   {
      MessageBoxYesNo("Save Datablock Changes?", 
         "Do you wish to save the changes made to the <br>current datablock before changing the datablock?",
         "DatablockEditorPlugin.saveDialogSave(" @ %datablock @ ");", 
         "DatablockEditorPlugin.saveDialogSave(" @ %datablock @ ", true);");
   }
}

function DatablockEditorPlugin::saveDialogDontSave(%this)
{
   DatablockEditorTree.clearSelection();
   %id = DatablockEditorTree.findItemByName(%this.currentDatablock.getName());
   if(%id == 0)
      %id = DatablockEditorTree.findItemByName(%this.currentDatablock.getName() @ " *");
      
   if(%id != 0)
      DatablockEditorTree.selectItem(%id);
}

function DatablockEditorPlugin::saveDialogSave(%this, %datablock, %dontSave)
{
   if( !%dontSave )
      %this.save();
   
   if( %datablock !$= "" )
   {
      %this.selectDatablock( %datablock );
   }
}

function DatablockEditorPlugin::save(%this)
{
   if(isObject(%this.currentDatablock) && %this.PM.isDirty(%this.currentDatablock))
   {
      %this.PM.saveDirtyObject(%this.currentDatablock);
      %this.flagInspectorAsDirty(false);
   }
}

function DatablockEditorInspector::onInspectorFieldModified( %this, %object, %fieldName, %arrayIndex, %oldValue, %newValue )
{
   // Same work to do as for the regular WorldEditor Inspector.
   Inspector::onInspectorFieldModified( %this, %object, %fieldName, %arrayIndex, %oldValue, %newValue );   
   
   DatablockEditorPlugin.PM.setDirty( %object );
   DatablockEditorPlugin.flagInspectorAsDirty(true);
}

function DatablockEditorInspector::onFieldSelected( %this, %fieldName, %fieldTypeStr, %fieldDoc )
{
   DatablockFieldInfoControl.setText( "<font:ArialBold:14>" @ %fieldName @ "<font:ArialItalic:14> (" @ %fieldTypeStr @ ") " NL "<font:Arial:14>" @ %fieldDoc );
}

function DatablockEditorInspector::inspect( %this, %obj )
{
   %name = "";
   if ( isObject( %obj ) )
      %name = %obj.getName();   
   else
      DatablockFieldInfoControl.setText( "" );
      
   Parent::inspect( %this, %obj );
}

//-----------------------------------------------------------------------------

function DatablockEditorTree::onDeleteSelection( %this )
{
   %this.undoDeleteList = "";   
}

function DatablockEditorTree::onDeleteObject( %this, %object )
{
   // Append it to our list.
   %this.undoDeleteList = %this.undoDeleteList TAB %object;
              
   // We're gonna delete this ourselves in the
   // completion callback.
   return true;
}

function DatablockEditorTree::onObjectDeleteCompleted( %this )
{
   //MEDeleteUndoAction::submit( %this.undoDeleteList );
   
   // Let the world editor know to 
   // clear its selection.
   //EWorldEditor.clearSelection();
   //EWorldEditor.isDirty = true;
}

function DatablockEditorTree::onClearSelected(%this)
{
   //WorldEditor.clearSelection();
}

function DatablockEditorTree::init(%this)
{
   //%this.open(MissionGroup);

  
}

function DatablockEditorTree::onInspect(%this, %obj)
{
   %text = %this.getItemValue(%obj);
   DatablockEditorPlugin.selectDatablockCheck(%text);
}

function DatablockEditorTree::onAddSelection(%this, %obj)
{/*
   EWorldEditor.selectObject(%obj);
   
   %selSize = EWorldEditor.getSelectionSize();
   %lockCount = EWorldEditor.getSelectionLockCount();
   
   if( %lockCount < %selSize )
   {
      EWTreeWindow-->LockSelection.setStateOn(0);
      EWTreeWindow-->LockSelection.command = "EWorldEditor.lockSelection(true); EditorTree.toggleLock();";
   }
   else if ( %lockCount > 0 )
   {
      EWTreeWindow-->LockSelection.setStateOn(1);
      EWTreeWindow-->LockSelection.command = "EWorldEditor.lockSelection(false); EditorTree.toggleLock();";
   }
   
   if( %selSize > 0 && %lockCount == 0 )
      EWTreeWindow-->DeleteSelection.command = "EditorMenuEditDelete();";
   else
      EWTreeWindow-->DeleteSelection.command = "";*/
      
}
function DatablockEditorTree::onRemoveSelection(%this, %obj)
{
   //EWorldEditor.unselectObject(%obj);
}
function DatablockEditorTree::onSelect(%this, %obj)
{
   //EWorldEditor.selectObject(%obj);
}

function DatablockEditorTree::onUnselect(%this, %obj)
{
   //EWorldEditor.unselectObject(%obj);
}

function DatablockEditorTreeTabBook::onTabSelected(%this, %text, %id)
{
   switch(%id)
   {
      case 0:
         DatablockEditorTreeWindow-->DeleteSelection.visible = true;
         DatablockEditorTreeWindow-->CreateSelection.visible = false;
      
      case 1:
         DatablockEditorTreeWindow-->DeleteSelection.visible = false;
         DatablockEditorTreeWindow-->CreateSelection.visible = true;               
   }
}

function DatablockEditorPlugin::deleteDatablock(%this)
{
   if( DatablockEditorTree.findItemByName(%this.currentDatablock.getName()) )
      %id = DatablockEditorTree.findItemByName(%this.currentDatablock.getName());
   else
      %id = DatablockEditorTree.findItemByName(%this.currentDatablock.getName() @ " *");
      
   DatablockEditorTree.removeItem(%id);
   //DatablockEditorTree.deleteSelection();
   %file = %this.currentDatablock.getFilename();
   
   %action = %this.createUndo(ActionDeleteDatablock, "Delete Datablock"); 
   %action.db = %this.currentDatablock;
   %action.fname = %file;
   %this.submitUndo( %action );

   %this.PM.removeObjectFromFile(%this.currentDatablock);
   MessageBoxOk("Object Deleted", "The datablock (" @ %this.currentDatablock.getName() @ ") has been removed from " @
                "it's file (" @ %file @ ") and upon restart will cease to exist");
   
   UnlistedDatablocks.add( "unlistedDatablocks", %this.currentDatablock.getName() );
   DatablockEditorPlugin.resetSelectedDatablock();
   /*
   if(isObject(%this.lastDatablock))
   {
      %this.selectDatablock(%this.lastDatablock);
   } else
   {
      %this.resetSelectedDatablock(); 
   }*/
}

function DatablockEditorPlugin::createDatablock(%this)
{
   %class = DatablockEditorTypeTree.getItemText(DatablockEditorTypeTree.getSelectedItem());
   if(%class !$= "")
   {  
      // need to prompt for a name
      
      DatablockEditorCreatePrompt-->CreateDatablockName.setText("Name");
      DatablockEditorCreatePrompt-->CreateDatablockName.selectAllText();
      
      canvas.pushDialog(DatablockEditorCreatePrompt);
   }
}

function DatablockEditorPlugin::createPromptNameCheck(%this)
{
   %name = DatablockEditorCreatePrompt-->CreateDatablockName.getText();
   if(isObject(%name))
   {
      MessageBoxOK("Invalid Name", "An object with this name already exists, please choose another.");
   } else
   {
      canvas.popDialog(DatablockEditorCreatePrompt);
      %this.createDatablockFinish(%name);
   }
}

function DatablockEditorPlugin::createDatablockFinish(%this, %name)
{
   %class = DatablockEditorTypeTree.getItemText(DatablockEditorTypeTree.getSelectedItem());
   if(%class !$= "")
   {  
      %action = %this.createUndo(ActionCreateDatablock, "Create Datablock");     
      
      %eval = "datablock " @ %class @ "(" @ %name @ ") { canSaveDynamicFields = \"1\"; };";
      %res = eval(%eval);
      
      %action.db = %name.getId();
      
      %file = "art/datablocks/managedDatablocks.cs";
      %name.setFilename(%file);
      %this.PM.setDirty(%name, %file);
      
      %action.fname = %file;
      %this.submitUndo( %action );
      
      %this.addExistingItem(%name, true);
      %this.selectDatablock(%name);
      %this.flagInspectorAsDirty(true);
   }
}