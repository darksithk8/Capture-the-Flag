//-----------------------------------------------------------------------------
// Copyright (C) Sickhead Games, LLC
//-----------------------------------------------------------------------------

function initializeDatablockEditor()
{
   echo( " - Initializing Datablock Editor" );
   
   exec("./datablockEditor.cs");
   exec("./datablockEditorUndo.cs");
   exec("./DatablockEditorTreeWindow.ed.gui");
   exec("./DatablockEditorInspectorWindow.ed.gui");
   exec("./DatablockEditorCreatePrompt.ed.gui");
   
   // Add ourselves to EditorGui, where all the other tools reside
   DatablockEditorInspectorWindow.setVisible( false );
   DatablockEditorTreeWindow.setVisible( false );
   
   EditorGui.add( DatablockEditorInspectorWindow );
   EditorGui.add( DatablockEditorTreeWindow );
   
   new ScriptObject( DatablockEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   new ArrayObject( UnlistedDatablocks );
   
   // create our persistence manager
   DatablockEditorPlugin.PM = new PersistenceManager();
   
   %map = new ActionMap();
   %map.bindCmd( keyboard, "backspace", "DatablockEditorPlugin.onDeleteKey();", "" );
   %map.bindCmd( keyboard, "delete", "DatablockEditorPlugin.onDeleteKey();", "" );  
   DatablockEditorPlugin.map = %map;
   
   DatablockEditorPlugin.initSettings();
}

function destroyDatablockEditor()
{
   
}

function DatablockEditorPlugin::onActivated( %this )
{  
   DatablockEditorPlugin.readSettings();
   
   WorldEditorPlugin.onActivated();
   EditorGui-->WorldEditorToolbar.setVisible(false);
   EditorGui.bringToFront( DatablockEditorPlugin );
   
   DatablockEditorTreeWindow.setVisible( true );
   DatablockEditorInspectorWindow.setVisible( true );
   DatablockEditorInspectorWindow.makeFirstResponder( true );
   
   %this.map.push();

   // Set the status bar here until all tool have been hooked up
   EditorGuiStatusBar.setInfo("Datablock editor.");

   EditorGuiStatusBar.setSelection("");
   
   %this.init();
   
   Parent::onActivated(%this);
}

function DatablockEditorPlugin::onDeactivated( %this )
{
   DatablockEditorPlugin.writeSettings();
   
   WorldEditorPlugin.onDeactivated();
   DatablockEditorInspectorWindow.setVisible( false );
   DatablockEditorTreeWindow.setVisible( false );
   %this.map.pop();
   
   Parent::onDeactivated(%this);
}

function DatablockEditorPlugin::handleEscape( %this )
{
   return DatablockEditorPlugin.onEscapePressed();  
}

function DatablockEditorPlugin::setEditorFunction( %this )
{
   //%terrainExists = parseMissionGroup( "TerrainBlock" );

   //if( %terrainExists == false )
      //MessageBoxYesNoCancel("No Terrain","Would you like to create a New Terrain?", "Canvas.pushDialog(CreateNewTerrainGui);");
   
   //return %terrainExists;
   return true;
}