//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function EditorGui::buildMenus(%this)
{
   if(isObject(%this.menuBar))
      return;
   
   //set up %cmdctrl variable so that it matches OS standards
   if( $platform $= "macos" )
   {   
      %cmdCtrl = "Cmd";
      %quitShortcut = "Cmd Q";
      %redoShortcut = "Cmd-Shift Z";
   }
   else
   {
      %cmdCtrl = "Ctrl";
      %quitShortcut = "Alt F4";
      %redoShortcut = "Ctrl Y";
   }

   // Sub menus (temporary, until MenuBuilder gets updated)
   %this.cameraSpeedMenu = new PopupMenu(EditorCameraSpeedOptions)
   {
      superClass = "MenuBuilder";
      class = "EditorCameraSpeedMenu";
      
      item[0] = "Slowest" TAB %cmdCtrl @ "-Shift 1" TAB "5";
      item[1] = "Slow" TAB %cmdCtrl @ "-Shift 2" TAB "35";
      item[2] = "Slower" TAB %cmdCtrl @ "-Shift 3" TAB "70";
      item[3] = "Normal" TAB %cmdCtrl @ "-Shift 4" TAB "100";
      item[4] = "Faster" TAB %cmdCtrl @ "-Shift 5" TAB "130";
      item[5] = "Fast" TAB %cmdCtrl @ "-Shift 6" TAB "165";
      item[6] = "Fastest" TAB %cmdCtrl @ "-Shift 7" TAB "200";
   };
   %this.freeCameraTypeMenu = new PopupMenu(EditorFreeCameraTypeOptions)
   {
      superClass = "MenuBuilder";
      class = "EditorFreeCameraTypeMenu";
      
      item[0] = "Standard" TAB "Ctrl 1" TAB "commandToServer('SetEditorCameraStandard');";
      item[1] = "Orbit Camera" TAB "Ctrl 2" TAB "commandToServer('SetEditorOrbitCamera');";
      Item[2] = "-";
      item[3] = "Smoothed" TAB "" TAB "commandToServer('SetEditorCameraNewton');";
      item[4] = "Smoothed Rotate" TAB "" TAB "commandToServer('SetEditorCameraNewtonDamped');";
   };
   %this.playerCameraTypeMenu = new PopupMenu(EditorPlayerCameraTypeOptions)
   {
      superClass = "MenuBuilder";
      class = "EditorPlayerCameraTypeMenu";
      
      Item[0] = "First Person" TAB "" TAB "commandToServer('SetEditorCameraPlayer');";
      Item[1] = "Third Person" TAB "" TAB "commandToServer('SetEditorCameraPlayerThird');";
   };
   %this.cameraBookmarksMenu = new PopupMenu(EditorCameraBookmarks)
   {
      superClass = "MenuBuilder";
      class = "EditorCameraBookmarksMenu";
      
      //item[0] = "None";
   };
      
   // Menu bar
   %this.menuBar = new MenuBar()
   {
      dynamicItemInsertPos = 3;
   };
   
   // File Menu
   %fileMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorFileMenu";

      barTitle = "File";
   };
   
   if(!isWebDemo())
   {
      %fileMenu.appendItem("New Level" TAB "" TAB "EditorNewLevel();");
      %fileMenu.appendItem("Open Level ..." TAB %cmdCtrl SPC "O" TAB "EditorOpenMission();");
      %fileMenu.appendItem("Save Level" TAB %cmdCtrl SPC "S" TAB "EditorSaveMissionMenu();");
      %fileMenu.appendItem("Save Level As ..." TAB "" TAB "EditorSaveMissionAs();");
      %fileMenu.appendItem("-");
   }
   
   %fileMenu.appendItem("Create Blank Terrain" TAB "" TAB "Canvas.pushDialog( CreateNewTerrainGui );");        
   %fileMenu.appendItem("Import Terrain Heightmap" TAB "" TAB "Canvas.pushDialog( TerrainImportGui );");
   
   if(!isWebDemo())
   {
      %fileMenu.appendItem("Export Terrain Heightmap" TAB "" TAB "Canvas.pushDialog( TerrainExportGui );");
      %fileMenu.appendItem("-");
      %fileMenu.appendItem("Export Interiors to Collada" TAB "" TAB "Canvas.pushDialog( InteriorExportGui );");
         //item[5] = "Import Terraform Data ..." TAB "" TAB "Heightfield::import();";
         //item[6] = "Import Texture Data ..." TAB "" TAB "Texture::import();";
         //item[7] = "-";
         //item[8] = "Export Terraform Data ..." TAB "" TAB "Heightfield::saveBitmap(\"\");";
   }
   
   %fileMenu.appendItem("-");
   %fileMenu.appendItem("Play Level" TAB "F11" TAB "Editor.close(\"PlayGui\");");
      
   if(!isWebDemo())
   {
      %fileMenu.appendItem("Exit Level" TAB "" TAB "EditorExitMission();");
      %fileMenu.appendItem("Quit" TAB %quitShortcut TAB "EditorQuitGame();");
   }
   %this.menuBar.insert(%fileMenu, %this.menuBar.getCount());
   
   // Edit Menu
   %editMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorEditMenu";
      internalName = "EditMenu";
         
      barTitle = "Edit";
         
      item[0] = "Undo" TAB %cmdCtrl SPC "Z" TAB "EUndoManager.undo();";
      item[1] = "Redo" TAB %redoShortcut TAB "EUndoManager.redo();";
      item[2] = "-";
      item[3] = "Cut" TAB %cmdCtrl SPC "X" TAB "";
      item[4] = "Copy" TAB %cmdCtrl SPC "C" TAB "";
      item[5] = "Paste" TAB %cmdCtrl SPC "V" TAB "";
      item[6] = "Delete" TAB "Delete" TAB "EditorMenuEditDelete();";
      item[7] = "-";
      //item[7] = "Select All" TAB "Ctrl A" TAB "";
      item[8] = "Deselect" TAB "X" TAB "eworldeditor.clearselection();";
      item[9] = "-";
      item[10] = "Editor Settings" TAB "" TAB "ESettingsWindow.ToggleVisibility();";
      item[11] = "Snap Options" TAB "" TAB "ESnapOptions.ToggleVisibility();";
      //item[9] = "Mission Area Editor" TAB "" TAB "Canvas.pushDialog(MissionAreaEditorContainer);";
      item[12] = "-";
      item[13] = "Game Options" TAB "" TAB "Canvas.pushDialog(optionsDlg);";
   };
   %this.menuBar.insert(%editMenu, %this.menuBar.getCount());
      
   // View Menu
   %viewMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorViewMenu";

      barTitle = "View";
         
      item[0] = "Visibility Layers" TAB "Alt V" TAB "VisibilityDropdownToggle();";
   };
   %this.menuBar.insert(%viewMenu, %this.menuBar.getCount());
      
   // Camera Menu
   %cameraMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorCameraMenu";

      barTitle = "Camera";
         
      item[0] = "World Camera" TAB %this.freeCameraTypeMenu;
      //Item[1] = "-";
      //item[2] = "Player Camera" TAB "" TAB "commandToServer('SetEditorCameraPlayer');";
      item[1] = "Player Camera" TAB %this.playerCameraTypeMenu;
      item[2] = "-";
      Item[3] = "Toggle Camera" TAB "Alt C" TAB "commandToServer('ToggleCamera');";
      item[4] = "Place Camera at Selection" TAB "Ctrl Q" TAB "EWorldEditor.dropCameraToSelection();";
      item[5] = "Place Camera at Player" TAB "Alt Q" TAB "commandToServer('dropCameraAtPlayer');";
      item[6] = "Place Player at Camera" TAB "Alt W" TAB "commandToServer('DropPlayerAtCamera');";
      item[7] = "-";
      item[8] = "Fit View to Selection" TAB "F" TAB "commandToServer('EditorCameraAutoFit', EWorldEditor.getSelectionRadius()+1);";
      item[9] = "Fit View To Selection and Orbit" TAB "Alt F" TAB "commandToServer('SetEditorOrbitCamera'); commandToServer('EditorCameraAutoFit', EWorldEditor.getSelectionRadius()+1);";
      item[10] = "-";
      item[11] = "Speed" TAB %this.cameraSpeedMenu;
      item[12] = "-";
      Item[13] = "Add Bookmark..." TAB "Ctrl B" TAB "EditorGui.addCameraBookmarkByGui();";
      Item[14] = "Manage Bookmarks..." TAB "Ctrl-Shift B" TAB "EditorGui.toggleCameraBookmarkWindow();";
      item[15] = "Jump to Bookmark" TAB %this.cameraBookmarksMenu;
   };
   %this.menuBar.insert(%cameraMenu, %this.menuBar.getCount());
      
   // Editors Menu
   %editorsMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorToolsMenu";

      barTitle = "Editors";
         
         //item[0] = "Object Editor" TAB "F1" TAB WorldEditorInspectorPlugin;
         //item[1] = "Material Editor" TAB "F2" TAB MaterialEditorPlugin;
         //item[2] = "-";
         //item[3] = "Terrain Editor" TAB "F3" TAB TerrainEditorPlugin;
         //item[4] = "Terrain Painter" TAB "F4" TAB TerrainPainterPlugin;
         //item[5] = "-";
   };
   %this.menuBar.insert(%editorsMenu, %this.menuBar.getCount());
      
   // Lighting Menu
   %lightingMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorLightingMenu";

      barTitle = "Lighting";
         
      item[0] = "Full Relight" TAB "Alt L" TAB "Editor.lightScene(\"\", forceAlways);";
      item[1] = "Toggle ShadowViz" TAB "" TAB "toggleShadowViz();";
      item[2] = "-";
         
         // NOTE: The light managers will be inserted as the
         // last menu items in EditorLightingMenu::onAdd().
   };
   %this.menuBar.insert(%lightingMenu, %this.menuBar.getCount());
      
   // Help Menu
   %helpMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";
      class = "EditorHelpMenu";

      barTitle = "Help";

      item[0] = "Online Documentation ..." TAB "Alt F1" TAB "gotoWebPage($pref::WorldEditor::documentationURL);";
      item[1] = "Offline Documentation ..." TAB "" TAB "gotoWebPage($pref::WorldEditor::documentationLocal);";
      item[2] = "Torque 3D Forums ..." TAB "" TAB "gotoWebPage($pref::WorldEditor::forumURL);";
   };
   %this.menuBar.insert(%helpMenu, %this.menuBar.getCount());
   
   // Menus that are added/removed dynamically (temporary)
   
   // World Menu
   if(! isObject(%this.worldMenu))
   {
      %this.dropTypeMenu = new PopupMenu()
      {
         superClass = "MenuBuilder";
         class = "EditorDropTypeMenu";

         // The onSelectItem() callback for this menu re-purposes the command field
         // as the MenuBuilder version is not used.
         item[0] = "at Origin" TAB "" TAB "atOrigin";
         item[1] = "at Camera" TAB "" TAB "atCamera";
         item[2] = "at Camera w/Rotation" TAB "" TAB "atCameraRot";
         item[3] = "Below Camera" TAB "" TAB "belowCamera";
         item[4] = "Screen Center" TAB "" TAB "screenCenter";
         item[5] = "at Centroid" TAB "" TAB "atCentroid";
         item[6] = "to Terrain" TAB "" TAB "toTerrain";
         item[7] = "Below Selection" TAB "" TAB "belowSelection";
      };
      
      %this.alignBoundsMenu = new PopupMenu()
      {
         superClass = "MenuBuilder";
         class = "EditorAlignBoundsMenu";

         // The onSelectItem() callback for this menu re-purposes the command field
         // as the MenuBuilder version is not used.
         item[0] = "+X Axis" TAB "" TAB "0";
         item[1] = "+Y Axis" TAB "" TAB "1";
         item[2] = "+Z Axis" TAB "" TAB "2";
         item[3] = "-X Axis" TAB "" TAB "3";
         item[4] = "-Y Axis" TAB "" TAB "4";
         item[5] = "-Z Axis" TAB "" TAB "5";
      };
      
      %this.alignCenterMenu = new PopupMenu()
      {
         superClass = "MenuBuilder";
         class = "EditorAlignCenterMenu";

         // The onSelectItem() callback for this menu re-purposes the command field
         // as the MenuBuilder version is not used.
         item[0] = "X Axis" TAB "" TAB "0";
         item[1] = "Y Axis" TAB "" TAB "1";
         item[2] = "Z Axis" TAB "" TAB "2";
      };
      
      %this.worldMenu = new PopupMenu()
      {
         superClass = "MenuBuilder";
         class = "EditorWorldMenu";

         barTitle = "Object";
         
         item[0] = "Lock Selection" TAB %cmdCtrl @ " L" TAB "EWorldEditor.lockSelection(true);";
         item[1] = "Unlock Selection" TAB %cmdCtrl @ "-Shift L" TAB "EWorldEditor.lockSelection(false);";
         item[2] = "-";
         item[3] = "Hide Selection" TAB %cmdCtrl @ " H" TAB "EWorldEditor.hideSelection(true);";
         item[4] = "Show Selection" TAB %cmdCtrl @ "-Shift H" TAB "EWorldEditor.hideSelection(false);";
         item[5] = "-";
         item[6] = "Align Bounds" TAB %this.alignBoundsMenu;
         item[7] = "Align Center" TAB %this.alignCenterMenu;
         item[8] = "-";
         item[9] = "Reset Transforms" TAB "Ctrl R" TAB "EWorldEditor.resetTransforms();";
         item[10] = "Reset Selected Rotation" TAB "" TAB "EWorldEditor.resetSelectedRotation();";
         item[11] = "Reset Selected Scale" TAB "" TAB "EWorldEditor.resetSelectedScale();";
         item[12] = "Transform Selection..." TAB "Ctrl T" TAB "ETransformSelection.ToggleVisibility();";
         item[13] = "-";
         //item[13] = "Drop Camera to Selection" TAB "Ctrl Q" TAB "EWorldEditor.dropCameraToSelection();";
         //item[14] = "Add Selection to Instant Group" TAB "" TAB "EWorldEditor.addSelectionToAddGroup();";
         item[14] = "Drop Selection" TAB "Ctrl D" TAB "EWorldEditor.dropSelection();";
         //item[15] = "-";
         item[15] = "Drop Location" TAB %this.dropTypeMenu;
      };
   }
}

//////////////////////////////////////////////////////////////////////////

function EditorGui::attachMenus(%this)
{
   %this.menuBar.attachToCanvas(Canvas, 0);
}

function EditorGui::detachMenus(%this)
{
   %this.menuBar.removeFromCanvas();
}

function EditorGui::setMenuDefaultState(%this)
{  
   if(! isObject(%this.menuBar))
      return 0;
      
   for(%i = 0;%i < %this.menuBar.getCount();%i++)
   {
      %menu = %this.menuBar.getObject(%i);
      %menu.setupDefaultState();
   }
   
   %this.worldMenu.setupDefaultState();
}

//////////////////////////////////////////////////////////////////////////

function EditorGui::findMenu(%this, %name)
{
   if(! isObject(%this.menuBar))
      return 0;
      
   for(%i = 0;%i < %this.menuBar.getCount();%i++)
   {
      %menu = %this.menuBar.getObject(%i);
      
      if(%name $= %menu.barTitle)
         return %menu;
   }
   
   return 0;
}
