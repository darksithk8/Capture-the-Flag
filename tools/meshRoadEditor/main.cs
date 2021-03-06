//-----------------------------------------------------------------------------
// Copyright (C) Sickhead Games, LLC
//-----------------------------------------------------------------------------

function initializeMeshRoadEditor()
{
   echo(" % - Initializing Mesh Road Editor");
     
   exec( "./meshRoadEditor.cs" );
   exec( "./meshRoadEditorGui.gui" );
   exec( "./meshRoadEditorToolbar.gui");
   exec( "./meshRoadEditorGui.cs" );
   
   MeshRoadEditorGui.setVisible( false );  
   MeshRoadEditorOptionsWindow.setVisible( false );  
   MeshRoadEditorToolbar.setVisible( false ); 
   MeshRoadEditorTreeWindow.setVisible( false ); 
   
   EditorGui.add( MeshRoadEditorGui );
   EditorGui.add( MeshRoadEditorOptionsWindow );
   EditorGui.add( MeshRoadEditorToolbar );
   EditorGui.add( MeshRoadEditorTreeWindow );
      
   new ScriptObject( MeshRoadEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   %map = new ActionMap();
   %map.bindCmd( keyboard, "backspace", "MeshRoadEditorGui.deleteNode();", "" );
   %map.bindCmd( keyboard, "delete", "MeshRoadEditorGui.deleteNode();", "" );  
   %map.bindCmd( keyboard, "1", "MeshRoadEditorGui.prepSelectionMode();", "" );  
   %map.bindCmd( keyboard, "2", "ToolsPaletteArray->MeshRoadEditorMoveMode.performClick();", "" );  
   %map.bindCmd( keyboard, "3", "ToolsPaletteArray->MeshRoadEditorRotateMode.performClick();", "" );  
   %map.bindCmd( keyboard, "4", "ToolsPaletteArray->MeshRoadEditorScaleMode.performClick();", "" );  
   %map.bindCmd( keyboard, "5", "ToolsPaletteArray->MeshRoadEditorAddRoadMode.performClick();", "" );  
   %map.bindCmd( keyboard, "-", "ToolsPaletteArray->MeshRoadEditorInsertPointMode.performClick();", "" );  
   %map.bindCmd( keyboard, "=", "ToolsPaletteArray->MeshRoadEditorRemovePointMode.performClick();", "" );  
   %map.bindCmd( keyboard, "z", "MeshRoadEditorShowSplineBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "x", "MeshRoadEditorWireframeBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "c", "MeshRoadEditorShowRoadBtn.performClick();", "" );  
   MeshRoadEditorPlugin.map = %map;
   
   MeshRoadEditorPlugin.initSettings();
}

function destroyMeshRoadEditor()
{
}

function MeshRoadEditorPlugin::onWorldEditorStartup( %this )
{     
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Mesh Road Editor", "", MeshRoadEditorPlugin );
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Mesh Road Editor (" @ %accel @ ")";   
   EditorGui.addToToolsToolbar( "MeshRoadEditorPlugin", "MeshRoadEditorPalette", expandFilename("tools/worldEditor/images/toolbar/mesh-road-editor"), %tooltip );

   //connect editor windows
   AttachWindows( MeshRoadEditorOptionsWindow, MeshRoadEditorTreeWindow);
   
   // Add ourselves to the Editor Settings window
   exec( "./meshRoadEditorSettingsTab.gui" );
   ESettingsWindow.addTabPage( EMeshRoadEditorSettingsPage );
}

function MeshRoadEditorPlugin::onActivated( %this )
{
   %this.readSettings();
   
   ToolsPaletteArray->MeshRoadEditorAddRoadMode.performClick();
   EditorGui.bringToFront( MeshRoadEditorGui );
   MeshRoadEditorGui.setVisible( true );
   MeshRoadEditorGui.makeFirstResponder( true );
   MeshRoadEditorOptionsWindow.setVisible( true );
   MeshRoadEditorToolbar.setVisible( true );  
   MeshRoadEditorTreeWindow.setVisible( true );
   MeshRoadTreeView.open(ServerMeshRoadSet,true);
   %this.map.push();
   
   // Store this on a dynamic field
   // in order to restore whatever setting
   // the user had before.
   %this.prevGizmoAlignment = GlobalGizmoProfile.alignment;
   
   // The DecalEditor always uses Object alignment.
   GlobalGizmoProfile.alignment = "Object";
   
   // Set the status bar here until all tool have been hooked up
   EditorGuiStatusBar.setInfo("Mesh road editor.");
   EditorGuiStatusBar.setSelection("");
   
   Parent::onActivated(%this);
}

function MeshRoadEditorPlugin::onDeactivated( %this )
{   
   %this.writeSettings();
   
   MeshRoadEditorGui.setVisible( false );
   MeshRoadEditorOptionsWindow.setVisible( false );
   MeshRoadEditorToolbar.setVisible( false );  
   MeshRoadEditorTreeWindow.setVisible( false );
   %this.map.pop();
   
   // Restore the previous Gizmo
   // alignment settings.
   GlobalGizmoProfile.alignment = %this.prevGizmoAlignment; 
   
   Parent::onDeactivated(%this);  
}

function MeshRoadEditorPlugin::handleEscape( %this )
{
   return MeshRoadEditorGui.onEscapePressed();  
}

function MeshRoadEditorPlugin::isDirty( %this )
{
   return MeshRoadEditorGui.isDirty;
}

function MeshRoadEditorPlugin::onSaveMission( %this, %missionFile )
{
   if( MeshRoadEditorGui.isDirty )
   {
      MissionGroup.save( %missionFile );
      MeshRoadEditorGui.isDirty = false;
   }
}

//-----------------------------------------------------------------------------
// Settings
//-----------------------------------------------------------------------------

function MeshRoadEditorPlugin::initSettings( %this )
{
   EditorSettings.beginGroup( "MeshRoadEditor", true );
   
   EditorSettings.setDefaultValue(  "DefaultWidth",         "10" );
   EditorSettings.setDefaultValue(  "DefaultDepth",         "5" );
   EditorSettings.setDefaultValue(  "DefaultNormal",        "0 0 1" );
   EditorSettings.setDefaultValue(  "HoverSplineColor",     "255 0 0 255" );
   EditorSettings.setDefaultValue(  "SelectedSplineColor",  "0 255 0 255" );
   EditorSettings.setDefaultValue(  "HoverNodeColor",       "255 255 255 255" ); //<-- Not currently used
   
   EditorSettings.endGroup();
}

function MeshRoadEditorPlugin::readSettings( %this )
{
   EditorSettings.beginGroup( "MeshRoadEditor", true );
   
   MeshRoadEditorGui.DefaultWidth         = EditorSettings.value("DefaultWidth");
   MeshRoadEditorGui.DefaultDepth         = EditorSettings.value("DefaultDepth");
   MeshRoadEditorGui.DefaultNormal        = EditorSettings.value("DefaultNormal");
   MeshRoadEditorGui.HoverSplineColor     = EditorSettings.value("HoverSplineColor");
   MeshRoadEditorGui.SelectedSplineColor  = EditorSettings.value("SelectedSplineColor");
   MeshRoadEditorGui.HoverNodeColor       = EditorSettings.value("HoverNodeColor");
   
   EditorSettings.endGroup();  
}

function MeshRoadEditorPlugin::writeSettings( %this )
{
   EditorSettings.beginGroup( "MeshRoadEditor", true );
   
   EditorSettings.setValue( "DefaultWidth",           MeshRoadEditorGui.DefaultWidth );
   EditorSettings.setValue( "DefaultDepth",           MeshRoadEditorGui.DefaultDepth );
   EditorSettings.setValue( "DefaultNormal",          MeshRoadEditorGui.DefaultNormal );
   EditorSettings.setValue( "HoverSplineColor",       MeshRoadEditorGui.HoverSplineColor );
   EditorSettings.setValue( "SelectedSplineColor",    MeshRoadEditorGui.SelectedSplineColor );
   EditorSettings.setValue( "HoverNodeColor",         MeshRoadEditorGui.HoverNodeColor );

   EditorSettings.endGroup();
}
