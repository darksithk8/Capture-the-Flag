//-----------------------------------------------------------------------------
// Copyright (C) Sickhead Games, LLC
//-----------------------------------------------------------------------------

function initializeRoadEditor()
{
   echo( " - Initializing Road and Path Editor" );
   
   exec( "./roadEditor.cs" );
   exec( "./roadEditorGui.gui" );
   exec( "./roadEditorToolbar.gui");
   exec( "./roadEditorGui.cs" );
   
   // Add ourselves to EditorGui, where all the other tools reside
   RoadEditorGui.setVisible( false ); 
   RoadEditorToolbar.setVisible( false );
   RoadEditorOptionsWindow.setVisible( false );
   RoadEditorTreeWindow.setVisible( false );
   
   EditorGui.add( RoadEditorGui );
   EditorGui.add( RoadEditorToolbar );
   EditorGui.add( RoadEditorOptionsWindow );
   EditorGui.add( RoadEditorTreeWindow );
   
   new ScriptObject( RoadEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   %map = new ActionMap();
   %map.bindCmd( keyboard, "backspace", "RoadEditorGui.onDeleteKey();", "" );
   %map.bindCmd( keyboard, "delete", "RoadEditorGui.onDeleteKey();", "" );  
   %map.bindCmd( keyboard, "1", "RoadEditorGui.prepSelectionMode();", "" );  
   %map.bindCmd( keyboard, "2", "ToolsPaletteArray->RoadEditorMoveMode.performClick();", "" );  
   %map.bindCmd( keyboard, "4", "ToolsPaletteArray->RoadEditorScaleMode.performClick();", "" );  
   %map.bindCmd( keyboard, "5", "ToolsPaletteArray->RoadEditorAddRoadMode.performClick();", "" );  
   %map.bindCmd( keyboard, "-", "ToolsPaletteArray->RoadEditorInsertPointMode.performClick();", "" );  
   %map.bindCmd( keyboard, "=", "ToolsPaletteArray->RoadEditorRemovePointMode.performClick();", "" );  
   %map.bindCmd( keyboard, "z", "RoadEditorShowSplineBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "x", "RoadEditorWireframeBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "c", "RoadEditorShowRoadBtn.performClick();", "" ); 
   RoadEditorPlugin.map = %map;
   
   RoadEditorPlugin.initSettings();
}

function destroyRoadEditor()
{
}

function RoadEditorPlugin::onWorldEditorStartup( %this )
{  
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Road and Path Editor", "", RoadEditorPlugin );      
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Road Editor (" @ %accel @ ")";   
   EditorGui.addToToolsToolbar( "RoadEditorPlugin", "RoadEditorPalette", expandFilename("tools/worldEditor/images/toolbar/road-path-editor"), %tooltip );
   
   //connect editor windows
   AttachWindows( RoadEditorOptionsWindow, RoadEditorTreeWindow);
   
   // Add ourselves to the Editor Settings window
   exec( "./RoadEditorSettingsTab.gui" );
   ESettingsWindow.addTabPage( ERoadEditorSettingsPage );
}

function RoadEditorPlugin::onActivated( %this )
{
   %this.readSettings();
   
   ToolsPaletteArray->RoadEditorAddRoadMode.performClick();
   EditorGui.bringToFront( RoadEditorGui );
   
   RoadEditorGui.setVisible( true );
   RoadEditorGui.makeFirstResponder( true );
   RoadEditorToolbar.setVisible( true );   
   
   RoadEditorOptionsWindow.setVisible( true );
   RoadEditorTreeWindow.setVisible( true );
   
   RoadTreeView.open(ServerDecalRoadSet,true);
   
   %this.map.push();

   // Set the status bar here until all tool have been hooked up
   EditorGuiStatusBar.setInfo("Road editor.");
   EditorGuiStatusBar.setSelection("");
   
   Parent::onActivated(%this);
}

function RoadEditorPlugin::onDeactivated( %this )
{
   %this.writeSettings();
   
   RoadEditorGui.setVisible( false );
   RoadEditorToolbar.setVisible( false );   
   RoadEditorOptionsWindow.setVisible( false );
   RoadEditorTreeWindow.setVisible( false );
   %this.map.pop();
   
   Parent::onDeactivated(%this);
}

function RoadEditorPlugin::handleEscape( %this )
{
   return RoadEditorGui.onEscapePressed();  
}

function RoadEditorPlugin::isDirty( %this )
{
   return RoadEditorGui.isDirty;
}

function RoadEditorPlugin::onSaveMission( %this, %missionFile )
{
   if( RoadEditorGui.isDirty )
   {
      MissionGroup.save( %missionFile );
      RoadEditorGui.isDirty = false;
   }
}

function RoadEditorPlugin::setEditorFunction( %this )
{
   %terrainExists = parseMissionGroup( "TerrainBlock" );

   if( %terrainExists == false )
      MessageBoxYesNoCancel("No Terrain","Would you like to create a New Terrain?", "Canvas.pushDialog(CreateNewTerrainGui);");
   
   return %terrainExists;
}

//-----------------------------------------------------------------------------
// Settings
//-----------------------------------------------------------------------------

function RoadEditorPlugin::initSettings( %this )
{
   EditorSettings.beginGroup( "RoadEditor", true );
   
   EditorSettings.setDefaultValue(  "DefaultWidth",         "10" );
   EditorSettings.setDefaultValue(  "HoverSplineColor",     "255 0 0 255" );
   EditorSettings.setDefaultValue(  "SelectedSplineColor",  "0 255 0 255" );
   EditorSettings.setDefaultValue(  "HoverNodeColor",       "255 255 255 255" ); //<-- Not currently used
   
   EditorSettings.endGroup();
}

function RoadEditorPlugin::readSettings( %this )
{
   EditorSettings.beginGroup( "RoadEditor", true );
   
   RoadEditorGui.DefaultWidth         = EditorSettings.value("DefaultWidth");
   RoadEditorGui.HoverSplineColor     = EditorSettings.value("HoverSplineColor");
   RoadEditorGui.SelectedSplineColor  = EditorSettings.value("SelectedSplineColor");
   RoadEditorGui.HoverNodeColor       = EditorSettings.value("HoverNodeColor");
   
   EditorSettings.endGroup();  
}

function RoadEditorPlugin::writeSettings( %this )
{
   EditorSettings.beginGroup( "RoadEditor", true );
   
   EditorSettings.setValue( "DefaultWidth",           RoadEditorGui.DefaultWidth );
   EditorSettings.setValue( "HoverSplineColor",       RoadEditorGui.HoverSplineColor );
   EditorSettings.setValue( "SelectedSplineColor",    RoadEditorGui.SelectedSplineColor );
   EditorSettings.setValue( "HoverNodeColor",         RoadEditorGui.HoverNodeColor );

   EditorSettings.endGroup();
}
