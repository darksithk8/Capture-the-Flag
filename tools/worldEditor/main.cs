//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function initializeWorldEditor()
{
   echo(" % - Initializing World Editor");
   
   // Load GUI
   exec("./gui/profiles.ed.cs");
   exec("./scripts/cursors.ed.cs");

   exec("./gui/guiCreateNewTerrainGui.gui" );
   exec("./gui/genericPromptDialog.ed.gui" );
   exec("./gui/guiTerrainImportGui.gui" );
   exec("./gui/guiTerrainExportGui.gui" );
   exec("./gui/guiInteriorExportGui.gui" );
   exec("./gui/EditorGui.ed.gui");
   exec("./gui/objectBuilderGui.ed.gui");
   exec("./gui/TerrainEditorVSettingsGui.ed.gui");
   exec("./gui/WorldEditorSettingsDlg.ed.gui");
   exec("./gui/guiMissionAreaEditorContainer.ed.gui");
   exec("./gui/EditorChooseLevelGui.ed.gui");
   exec("./gui/VisibilityLayerWindow.ed.gui");
   exec("./gui/ManageBookmarksWindow.ed.gui");

   // Load Scripts.
   exec("./scripts/menus.ed.cs");
   exec("./scripts/menuHandlers.ed.cs");
   exec("./scripts/editor.ed.cs");
   exec("./scripts/editor.bind.ed.cs");
   exec("./scripts/undoManager.ed.cs");
   exec("./scripts/lighting.ed.cs");
   exec("./scripts/EditorGui.ed.cs");
   exec("./scripts/editorPrefs.ed.cs");
   exec("./scripts/editorRender.ed.cs");
   exec("./scripts/editorPlugin.ed.cs");
   exec("./scripts/EditorChooseLevelGui.ed.cs");
   exec("./scripts/visibilityLayer.ed.cs");
   exec("./scripts/cameraBookmarks.ed.cs");

   // Load Custom Editors
   loadDirectory(expandFilename("./scripts/editors"));
   loadDirectory(expandFilename("./scripts/interfaces"));
   
   // Create the default editor plugins before calling buildMenus.
      
   new ScriptObject( WorldEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   new ScriptObject( WorldEditorInspectorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   new ScriptObject( WorldEditorCreatorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   new ScriptObject( MissionAreaEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   new ScriptObject( TerrainEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   new ScriptObject( TerrainPainterPlugin )
   {
      superClass = "EditorPlugin";
   };
 
   new ScriptObject( MaterialEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   // Expose stock visibility/debug options.
   EVisibility.addOption( "Render Zones", "$Zone::renderZones", "" );
   EVisibility.addOption( "Render Portals", "$Portal::renderPortals", "" );
   EVisibility.addOption( "Render Triggers", "$Trigger::renderTriggers", "" );
   EVisibility.addOption( "Render PhysicalZones", "$PhysicalZone::renderZones", "" );
   EVisibility.addOption( "Wireframe Mode", "$gfx::wireframe", "" );
   EVisibility.addOption( "Player Render Collision", "$Player::renderCollision", "" );   
   EVisibility.addOption( "Terrain Debug Render", "TerrainBlock::debugRender", "" );
   EVisibility.addOption( "Disable Shadows", "$ShadowMap::disableShadows", "" );   
   EVisibility.addOption( "Toggle Light Color Viz", "$AL_LightColorVisualizeVar", "toggleLightColorViz" );
   EVisibility.addOption( "Toggle Light Specular Viz", "$AL_LightSpecularVisualizeVar", "toggleLightSpecularViz" );
   EVisibility.addOption( "Toggle Normals Viz", "$AL_NormalsVisualizeVar", "toggleNormalsViz" );
   EVisibility.addOption( "Toggle Depth Viz", "$AL_DepthVisualizeVar", "toggleDepthViz" );
}

function destroyWorldEditor()
{
}
