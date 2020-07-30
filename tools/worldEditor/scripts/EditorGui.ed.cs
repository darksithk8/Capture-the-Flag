//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function EditorGui::init(%this)
{
   %this.readWorldEditorSettings();

   $SelectedOperation = -1;
   $NextOperationId   = 1;
   $HeightfieldDirtyRow = -1;
         
   %this.buildMenus();
   
   if( !isObject( %this-->ToolsPaletteWindow ) )
   {
      // Load Creator/Inspector GUI
      exec("~/worldEditor/gui/ToolsPaletteGroups/init.cs");
      exec("~/worldEditor/gui/ToolsPaletteWindow.ed.gui");
      
      if( isObject( EWToolsPaletteWindow ) )
      {
         %this.add( EWToolsPaletteWindow );
         EWToolsPaletteWindow.init();
         EWToolsPaletteWindow.setVisible( false );
      }
   }
   
   if( !isObject( %this-->TreeWindow ) )
   {
      // Load Creator/Inspector GUI
      exec("~/worldEditor/gui/WorldEditorTreeWindow.ed.gui");
      if( isObject( EWTreeWindow ) )
      {
         %this.add( EWTreeWindow );
         EWTreeWindow-->EditorTree.selectPage( 0 );
         EWTreeWindow.setVisible( false );
      }
   }
   
   if( !isObject( %this-->InspectorWindow ) )
   {
      // Load Creator/Inspector GUI
      exec("~/worldEditor/gui/WorldEditorInspectorWindow.ed.gui");
      //EWInspectorWindow.resize(getWord(EWInspectorWindow.Position, 0), getWord(EWInspectorWindow.Position, 1), getWord(EWInspectorWindow.extent, 0), getWord(EWInspectorWindow.extent, 1));
      if( isObject( EWInspectorWindow ) )
      {
         %this.add( EWInspectorWindow );
         EWInspectorWindow.setVisible( false );
      }
   }   
   
   if( !isObject( %this-->WorldEditorToolbar ) )
   {
      // Load Creator/Inspector GUI
      exec("~/worldEditor/gui/WorldEditorToolbar.ed.gui");
      if( isObject( EWorldEditorToolbar ) )
      {
         %this.add( EWorldEditorToolbar );
         EWorldEditorToolbar.setVisible( false );
      }
   }  
   
   if ( !isObject( %this-->TerrainEditToolbar ) )
   {
      // Load Terrain Edit GUI
      exec("~/worldEditor/gui/TerrainEditToolbar.ed.gui");
      if( isObject( EWTerrainEditToolbar ) )
      {
         %this.add( EWTerrainEditToolbar );
         EWTerrainEditToolbar.setVisible( false );
      }
   }
   
   if( !isObject( %this-->TerrainPainter ) )
   {
      // Load Terrain Painter GUI
      exec("~/worldEditor/gui/TerrainPainterWindow.ed.gui");
      if( isObject( %guiContent ) ){
         %this.add( %guiContent->TerrainPainter );
         %this.add( %guiContent->TerrainPainterPreview );
      }
         
      exec("~/worldEditor/gui/guiTerrainMaterialDlg.ed.gui"); 
      exec("~/worldEditor/gui/TerrainBrushSoftnessCurveDlg.ed.gui");        
   }
   if ( !isObject( %this-->TerrainPainterToolbar) )
   {
      // Load Terrain Edit GUI
      exec("~/worldEditor/gui/TerrainPainterToolbar.ed.gui");
      if( isObject( EWTerrainPainterToolbar ) )
      {
         %this.add( EWTerrainPainterToolbar );
         EWTerrainPainterToolbar.setVisible( false );
      }
   }

   if( !isObject( %this-->ToolsToolbar ) )
   {
      // Load Creator/Inspector GUI
      exec("~/worldEditor/gui/ToolsToolbar.ed.gui");
      if( isObject( EWToolsToolbar ) )
      {
         %this.add( EWToolsToolbar );
         EWToolsToolbar.setVisible( true );
         
      }
   }
   
   // Visibility Layer Window
   if( !isObject( %this-->VisibilityLayerWindow ) )
   {
      %this.add( EVisibility );
      EVisibility.setVisible(false);
      EVisibilityTabBook.selectPage(0);
   }
      
   // Editor Settings Window
   if( !isObject( %this-->EditorSettingsWindow ) )
   {
      exec("~/worldEditor/gui/EditorSettingsWindow.ed.gui");
      exec("~/worldEditor/scripts/editorSettingsWindow.ed.cs");
      %this.add( ESettingsWindow );
      ESettingsWindow.setVisible(false);
      
      // Start the standard settings tabs pages
      exec("~/worldEditor/gui/ObjectEditorSettingsTab.ed.gui");
      ESettingsWindow.addTabPage( EObjectEditorSettingsPage );
      exec("~/worldEditor/gui/AxisGizmoSettingsTab.ed.gui");
      ESettingsWindow.addTabPage( EAxisGizmoSettingsPage );
      exec("~/worldEditor/gui/TerrainEditorSettingsTab.ed.gui");
      ESettingsWindow.addTabPage( ETerrainEditorSettingsPage );
   }

   // Object Snap Options Window
   if( !isObject( %this-->SnapOptionsWindow ) )
   {
      exec("~/worldEditor/gui/ObjectSnapOptionsWindow.ed.gui");
      exec("~/worldEditor/scripts/objectSnapOptions.ed.cs");
      %this.add( ESnapOptions );
      ESnapOptions.setVisible(false);
      ESnapOptionsTabBook.selectPage(0);
   }
   
   // Transform Selection Window
   if( !isObject( %this-->TransformSelectionWindow ) )
   {
      exec("~/worldEditor/gui/TransformSelectionWindow.ed.gui");
      exec("~/worldEditor/scripts/transformSelection.ed.cs");
      %this.add( ETransformSelection );
      ETransformSelection.setVisible(false);
   }
   
   // Manage Bookmarks Window
   if( !isObject( %this-->ManageBookmarksWindow ) )
   {
      //exec("~/worldEditor/gui/ManageBookmarksWindow.ed.gui");
      %this.add( EManageBookmarks );
      EManageBookmarks.setVisible(false);
   }
   
   EWorldEditor.init();
   ETerrainEditor.init();

   //Creator.init();
   EWCreatorWindow.init();
   EditorTree.init();
   ObjectBuilderGui.init();

   %this.setMenuDefaultState();

   EWorldEditor.isDirty = false;
   ETerrainEditor.isDirty = false;
   ETerrainEditor.isMissionDirty = false;
   EditorGui.saveAs = false;
   
   EWorldEditorToggleCamera.setBitmap("tools/worldEditor/images/toolbar/player");
   
   /*
   EWorldEditorCameraSpeed.clear();
   EWorldEditorCameraSpeed.add("Slowest - Camera 1",0);
   EWorldEditorCameraSpeed.add("Slow - Camera 2",1);
   EWorldEditorCameraSpeed.add("Slower - Camera 3",2);
   EWorldEditorCameraSpeed.add("Normal - Camera 4",3);
   EWorldEditorCameraSpeed.add("Faster - Camera 5",4);
   EWorldEditorCameraSpeed.add("Fast - Camera 6",5);
   EWorldEditorCameraSpeed.add("Fastest - Camera 7",6);
   EWorldEditorCameraSpeed.setSelected(3);
   */
   
   EWorldEditorAlignPopup.clear();
   EWorldEditorAlignPopup.add("World",0);
   EWorldEditorAlignPopup.add("Object",1);
   EWorldEditorAlignPopup.setSelected(0);
   
   
   // sync camera gui
   EditorGui.syncCameraGui();
   
   // this will brind CameraTypesDropdown to front so that it goes over the menubar
   EditorGui.pushToBack(CameraTypesDropdown); 
   EditorGui.pushToBack(VisibilityDropdown); 
   
   // dropdowns out so that they display correctly in editor gui
   objectTransformDropdown.parentGroup = editorGui; 
   objectCenterDropdown.parentGroup = editorGui; 
   objectSnapDropdown.parentGroup = editorGui; 
   
   // make sure to show the default world editor guis
   EditorGui.bringToFront( EWorldEditor );
   EWorldEditor.setVisible( false ); 
   
   //
   // Now initialize WorldEditor Sub-Editors
   //          
   
   // Call the startup callback on the editor plugins
   // so that they can add menus and setup their guis.
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject( %i );
      %obj.onWorldEditorStartup();      
   }

   // With everything loaded, start up the settings window
   ESettingsWindow.startup();
}

/// This is used to add an editor to the Editors menu which
/// will take over the default world editor window.
function EditorGui::addToEditorsMenu( %this, %displayName, %accel, %newPlugin )
{
   %windowMenu = %this.findMenu( "Editors" );   
   %count = %windowMenu.getItemCount();      
   
   
   %alreadyExists = false;
   for ( %i = 0; %i < %count; %i++ )
   {      
      %existingPlugins = getField(%windowMenu.Item[%i], 2);
      
      if(%newPlugin $= %existingPlugins)
         %alreadyExists = true;
   }
   
   if( %accel $= "" && %count < 9 )
      %accel = "F" @ %count + 1;
   else
      %accel = "";
         
   if(!%alreadyExists)
      %windowMenu.addItem( %count, %displayName TAB %accel TAB %newPlugin );
      
   return %accel;
}

function EditorGui::addToToolsToolbar( %this, %pluginName, %internalName, %bitmap, %tooltip )
{   
   %count = ToolsToolbarArray.getCount();      
   
   %alreadyExists = false;
   for ( %i = 0; %i < %count; %i++ )
   {      
      %existingInternalName = ToolsToolbarArray.getObject(%i).getFieldValue("internalName");
      
      if(%internalName $= %existingInternalName)
      {
         %alreadyExists = true;
         break;
      }
   }
      
   if(!%alreadyExists)
   {
      %button = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
         internalName = %internalName;
         Enabled = "1";
         isContainer = "0";
         Profile = "GuiButtonProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "180 0";
         Extent = "25 19";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         Command = "EditorGui.setEditor(" @ %pluginName @ ");";
         tooltipprofile = "GuiToolTipProfile";
         ToolTip = %tooltip;
         hovertime = "750";
         bitmap = %bitmap;
         buttonType = "RadioButton";
         groupNum = "0";
         useMouseEvents = "0";
      };
      ToolsToolbarArray.add(%button);
   }
}

//-----------------------------------------------------------------------------

/*
function EditorGui::setWorldEditorVisible(%this)
{
   EWorldEditor.setVisible(true);

   //ETerrainEditor.setVisible(false);
   
   %this.menuBar.insert(%this.worldMenu, %this.menuBar.dynamicItemInsertPos);
   
   EWorldEditor.makeFirstResponder(true);
   EditorTree.open(MissionGroup,true);
   WorldEditorMap.push();
}

function EditorGui::setTerrainEditorVisible(%this)
{
   EWorldEditor.setVisible(false);
   ETerrainEditor.setVisible(true);
   ETerrainEditor.attachTerrain();
   EHeightField.setVisible(false);
   %this.menuBar.remove(%this.worldMenu);
   ETerrainEditor.makeFirstResponder(true);

   WorldEditorMap.pop();
}
*/

function WorldEditorPlugin::onActivated( %this )
{
   EditorGui.readWorldEditorSettings();
   
   EditorGui.bringToFront( EWorldEditor );
   EWorldEditor.setVisible(true);
   EditorGui.menuBar.insert( EditorGui.worldMenu, EditorGui.menuBar.dynamicItemInsertPos );
   EWorldEditor.makeFirstResponder(true);
   EditorTree.open(MissionGroup,true);
   EWCreatorWindow.setNewObjectGroup(MissionGroup);
   WorldEditorMap.push();

   EWorldEditor.syncGui();

   EditorGuiStatusBar.setSelectionObjectsByCount(EWorldEditor.getSelectionSize());
   
   // Should the Transform Selection window open?
   if( EWorldEditor.ETransformSelectionDisplayed )
   {
      ETransformSelection.setVisible(true);
   }
   
   Parent::onActivated(%this);
}

function WorldEditorPlugin::onEditMenuSelect( %this, %editMenu )
{
   %canCutCopy = EWorldEditor.getSelectionSize() > 0;
   %editMenu.enableItem( 3, %canCutCopy ); // Cut
   %editMenu.enableItem( 4, %canCutCopy ); // Copy      
   %editMenu.enableItem( 5, EWorldEditor.canPasteSelection() ); // Paste
   %editMenu.enableItem( 8, %canCutCopy ); // Deselect  
   
   // TODO: These could move into onActivated.
   %editMenu.setItemCommand( 3, "EWorldEditor.cutSelection();" );
   %editMenu.setItemCommand( 4, "EWorldEditor.copySelection();" );
   %editMenu.setItemCommand( 5, "EWorldEditor.pasteSelection();" );
   %editMenu.setItemCommand( 8, "EWorldEditor.clearSelection();" );
}

function WorldEditorPlugin::onDeactivated( %this )
{
   EditorGui.writeWorldEditorSettings();
   
   // Hide the Transform Selection window from other editors
   ETransformSelection.setVisible(false);
   
   EWorldEditor.setVisible( false );            
   EditorGui.menuBar.remove( EditorGui.worldMenu );
   WorldEditorMap.pop();  
   
   Parent::onDeactivated(%this);    
}

function WorldEditorInspectorPlugin::onWorldEditorStartup( %this )
{
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Object Editor", "", WorldEditorInspectorPlugin );
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Object Editor (" @ %accel @ ")";   
   EditorGui.addToToolsToolbar( "WorldEditorInspectorPlugin", "WorldEditorInspectorPalette", expandFilename("tools/worldEditor/images/toolbar/transform-objects"), %tooltip );
   
   //connect editor windows
   AttachWindows( EWInspectorWindow, EWTreeWindow);
   
   %map = new ActionMap();   
   %map.bindCmd( keyboard, "1", "EWorldEditorNoneModeBtn.performClick();", "" );  // Select
   %map.bindCmd( keyboard, "2", "EWorldEditorMoveModeBtn.performClick();", "" );  // Move
   %map.bindCmd( keyboard, "3", "EWorldEditorRotateModeBtn.performClick();", "" );  // Rotate
   %map.bindCmd( keyboard, "4", "EWorldEditorScaleModeBtn.performClick();", "" );  // Scale
   %map.bindCmd( keyboard, "f", "FitToSelectionBtn.performClick();", "" );// Fit Camera to Selection
   %map.bindCmd( keyboard, "z", "commandToServer('SetEditorCameraStandard');", "" );// Fit Camera to Selection
   %map.bindCmd( keyboard, "n", "ToggleNodeBar->renderHandleBtn.performClick();", "" );// Render Node
   %map.bindCmd( keyboard, "shift n", "ToggleNodeBar->renderTextBtn.performClick();", "" );// Render Node Text
   %map.bindCmd( keyboard, "t", "SnapToBar->stickToGroundBtn.performClick();", "" );// Terrain Snaping
   %map.bindCmd( keyboard, "b", "SnapToBar->dropAtBoundsBtn.performClick();", "" );// Bounds Snaping
   %map.bindCmd( keyboard, "v", "EWorldEditorToolbar->boundingBoxColBtn.performClick();", "" );// Bounds Selection
   %map.bindCmd( keyboard, "o", "objectCenterDropdown->objectBoxBtn.performClick(); objectCenterDropdown.toggle();", "" );// Object Center
   %map.bindCmd( keyboard, "p", "objectCenterDropdown->objectBoundsBtn.performClick(); objectCenterDropdown.toggle();", "" );// Bounds Center
   %map.bindCmd( keyboard, "k", "objectTransformDropdown->objectTransformBtn.performClick(); objectTransformDropdown.toggle();", "" );// Object Transform
   %map.bindCmd( keyboard, "l", "objectTransformDropdown->worldTransformBtn.performClick(); objectTransformDropdown.toggle();", "" );// World Transform
   
   WorldEditorInspectorPlugin.map = %map; 
}

function WorldEditorInspectorPlugin::onActivated( %this )
{
   WorldEditorPlugin.onActivated();
   
   EditorGui-->InspectorWindow.setVisible( true );   
   EditorGui-->TreeWindow.setVisible( true );
   EditorGui-->WorldEditorToolbar.setVisible( true );
   %this.map.push();
}

function WorldEditorInspectorPlugin::onEditMenuSelect( %this, %editMenu )
{
   WorldEditorPlugin.onEditMenuSelect( %editMenu );
}

function WorldEditorInspectorPlugin::onDeactivated( %this )
{
   WorldEditorPlugin.onDeactivated();
   
   EditorGui-->InspectorWindow.setVisible( false );  
   EditorGui-->TreeWindow.setVisible( false ); 
   EditorGui-->WorldEditorToolbar.setVisible( false );
   %this.map.pop();
}

function WorldEditorCreatorPlugin::onEditMenuSelect( %this, %editMenu )
{
   WorldEditorPlugin.onEditMenuSelect( %editMenu );
}

function MissionAreaEditorPlugin::onActivated( %this )
{
   WorldEditorPlugin.onActivated();
   
   EWMissionArea.setVisible(true);
   EditorGui-->MissionAreaEditor.setVisible(true);   
   
   Parent::onActivated(%this);
}

function MissionAreaEditorPlugin::onDeactivated( %this )
{
   WorldEditorPlugin.onDeactivated();
   
   EWMissionArea.setVisible(false);   
   EditorGui-->MissionAreaEditor.setVisible(false); 
   
   Parent::onDeactivated(%this);  
}

function TerrainEditorPlugin::onWorldEditorStartup( %this )
{
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Terrain Editor", "", TerrainEditorPlugin );
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Terrain Editor (" @ %accel @ ")";   
   EditorGui.addToToolsToolbar( "TerrainEditorPlugin", "TerrainEditorPalette", expandFilename("tools/worldEditor/images/toolbar/sculpt-terrain"), %tooltip );
   
   %map = new ActionMap();   
   %map.bindCmd( keyboard, "1", "ToolsPaletteArray->TerrainEditorAdjustHeight.performClick();", "" );    //Grab Terrain
   %map.bindCmd( keyboard, "2", "ToolsPaletteArray->TerrainEditorRaiseHeight.performClick();", "" );     // Raise Height
   %map.bindCmd( keyboard, "3", "ToolsPaletteArray->TerrainEditorLowerHeight.performClick();", "" );     // Lower Height
   %map.bindCmd( keyboard, "4", "ToolsPaletteArray->TerrainEditorSmoothHeight.performClick();", "" );    // Smooth
   %map.bindCmd( keyboard, "5", "ToolsPaletteArray->TerrainEditorPaintNoise.performClick();", "" );      // Noise
   %map.bindCmd( keyboard, "6", "ToolsPaletteArray->TerrainEditorFlattenHeight.performClick();", "" );   // Flatten
//   %map.bindCmd( keyboard, "7", "ToolsPaletteArray->TerrainEditorSetHeight.performClick();", "" );       // Set Height
   %map.bindCmd( keyboard, "8", "ToolsPaletteArray->TerrainEditorClearTerrain.performClick();", "" );    // Clear Terrain
   %map.bindCmd( keyboard, "9", "ToolsPaletteArray->TerrainEditorRestoreTerrain.performClick();", "" );  // Restore Terrain
   %map.bindCmd( keyboard, "c", "EWTerrainEditToolbarBrushType->ellipse.performClick();", "" );// Circle Brush
   %map.bindCmd( keyboard, "b", "EWTerrainEditToolbarBrushType->box.performClick();", "" );// Box Brush
   %map.bindCmd( keyboard, "=", "TerrainEditorPlugin.keyboardModifyBrushSize(1);", "" );// +1 Brush Size
   %map.bindCmd( keyboard, "+", "TerrainEditorPlugin.keyboardModifyBrushSize(1);", "" );// +1 Brush Size
   %map.bindCmd( keyboard, "-", "TerrainEditorPlugin.keyboardModifyBrushSize(-1);", "" );// -1 Brush Size
   %map.bindCmd( keyboard, "[", "TerrainEditorPlugin.keyboardModifyBrushSize(-5);", "" );// -5 Brush Size
   %map.bindCmd( keyboard, "]", "TerrainEditorPlugin.keyboardModifyBrushSize(5);", "" );// +5 Brush Size
   /*%map.bindCmd( keyboard, "]", "TerrainBrushPressureTextEditContainer->textEdit.text += 5", "" );// +5 Pressure
   %map.bindCmd( keyboard, "[", "TerrainBrushPressureTextEditContainer->textEdit.text -= 5", "" );// -5 Pressure
   %map.bindCmd( keyboard, "'", "TerrainBrushSoftnessTextEditContainer->textEdit.text += 5", "" );// +5 Softness
   %map.bindCmd( keyboard, ";", "TerrainBrushSoftnessTextEditContainer->textEdit.text -= 5", "" );// -5 Softness*/
   
   TerrainEditorPlugin.map = %map;  
   
   %this.terrainMenu = new PopupMenu()
   {
      superClass = "MenuBuilder";

      barTitle = "Terrain";
               
      item[0] = "Smooth Heightmap" TAB "" TAB "ETerrainEditor.onSmoothHeightmap();";
   };   
}

function TerrainEditorPlugin::onActivated( %this )
{
   EditorGui.readTerrainEditorSettings();
   
   ToolsPaletteArray->TerrainEditorAdjustHeight.performClick(); //Grab Terrain
   EWTerrainEditToolbarBrushType->ellipse.performClick(); // Circle Brush
   
   EditorGui.menuBar.insert( %this.terrainMenu, EditorGui.menuBar.dynamicItemInsertPos );
         
   EditorGui.bringToFront( ETerrainEditor );
   ETerrainEditor.setVisible( true );
   ETerrainEditor.attachTerrain();
   ETerrainEditor.makeFirstResponder( true );
        
   EWTerrainEditToolbar.setVisible( true );
   ETerrainEditor.onBrushChanged();
   ETerrainEditor.setup();
   TerrainEditorPlugin.rememberBrushInfo(0);

   EditorGuiStatusBar.setSelection("");
   %this.map.push();
   
   Parent::onActivated(%this);
}

function TerrainEditorPlugin::onDeactivated( %this )
{
   endToolTime("TerrainEditor");
   EditorGui.writeTerrainEditorSettings();
   
   EWTerrainEditToolbar.setVisible( false );
   ETerrainEditor.setVisible( false );
   
   EditorGui.menuBar.remove( %this.terrainMenu );
      
   TerrainEditorPlugin.rememberBrushInfo(1);
   %this.map.pop();
   
   Parent::onDeactivated(%this);
}

function TerrainEditorPlugin::rememberBrushInfo( %this, %storeOrCheckout)
{
   // 0 = store
   // 1 = checkout 
   if(%storeOrCheckout == 1)
   {
      // Remember my brush info
      ETerrainEditor.terrainBrushInfo = "";
      ETerrainEditor.terrainBrushInfo = TerrainBrushSizeTextEditContainer-->textEdit.getText();
      ETerrainEditor.terrainBrushInfo = ETerrainEditor.terrainBrushInfo SPC TerrainBrushPressureTextEditContainer-->textEdit.getText();
      ETerrainEditor.terrainBrushInfo = ETerrainEditor.terrainBrushInfo SPC TerrainBrushSoftnessTextEditContainer-->textEdit.getText();
      ETerrainEditor.terrainBrushInfo = ETerrainEditor.terrainBrushInfo SPC ETerrainEditor.getBrushType();
   }
   else
   {
      //Input my brush info
      //The default vaules on this brush are 9 1 1; you can change the default values by changing
      //ETerrainEditor.terrainBrushInfo field in EditorGui.ed.cs. If you change the default values
      //you'll probably also want to change the default values on the text edit gui fields respectively
      
      %storedBrushInfo = ETerrainEditor.terrainBrushInfo;
      %brushType = GetWord(%storedBrushInfo, 3);
      
      TerrainBrushSizeTextEditContainer-->textEdit.text = GetWord(%storedBrushInfo, 0);
      ETerrainEditor.setBrushSize( GetWord(%storedBrushInfo, 0) );
      TerrainBrushPressureTextEditContainer-->textEdit.text = GetWord(%storedBrushInfo, 1);
      ETerrainEditor.setBrushPressure( GetWord(%storedBrushInfo, 1) );
      TerrainBrushSoftnessTextEditContainer-->textEdit.text = GetWord(%storedBrushInfo, 2);
      ETerrainEditor.setBrushSoftness( GetWord(%storedBrushInfo, 2) );
      eval( "EWTerrainEditToolbar-->" @ %brushType @ ".setStateOn(1);" );
      //EWTerrainEditToolbar-->(%brushType).setStateOn(1);
      ETerrainEditor.setBrushType( GetWord(%storedBrushInfo, 3) );
   }
}

function TerrainEditorPlugin::validateBrushSize( %this )
{
   %val = $ThisControl.getText();
   if(%val < 1)
      $ThisControl.setValue(1);
   else if(%val > 40)
      $ThisControl.setValue(40);
}

function TerrainEditorPlugin::keyboardModifyBrushSize( %this, %amt)
{
   %val = TerrainBrushSizeTextEditContainer-->textEdit.getText();
   %val += %amt;
   TerrainBrushSizeTextEditContainer-->textEdit.setValue(%val);
   TerrainBrushSizeTextEditContainer-->textEdit.forceValidateText();
   ETerrainEditor.setBrushSize( TerrainBrushSizeTextEditContainer-->textEdit.getText() );
}
function TerrainTextureEditorTool::onActivated( %this )
{
   EditorGui.bringToFront( ETerrainEditor );
   ETerrainEditor.setVisible( true );
   ETerrainEditor.attachTerrain();
   ETerrainEditor.makeFirstResponder( true );
   
   EditorGui-->TextureEditor.setVisible(true);

   EditorGuiStatusBar.setSelection("");
}

function TerrainTextureEditorTool::onDeactivated( %this )
{
   EditorGui-->TextureEditor.setVisible(false); 
       
   ETerrainEditor.setVisible( false );
}

function TerrainPainterPlugin::onWorldEditorStartup( %this )
{
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Terrain Painter", "", TerrainPainterPlugin );
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Terrain Painter (" @ %accel @ ")"; 
   EditorGui.addToToolsToolbar( "TerrainPainterPlugin", "TerrainPainterPalette", expandFilename("tools/worldEditor/images/toolbar/paint-terrain"), %tooltip );

   %map = new ActionMap();   
   %map.bindCmd( keyboard, "c", "EWTerrainPainterToolbarBrushType->ellipse.performClick();", "" );// Circle Brush
   %map.bindCmd( keyboard, "b", "EWTerrainPainterToolbarBrushType->box.performClick();", "" );// Box Brush
   %map.bindCmd( keyboard, "=", "TerrainPainterPlugin.keyboardModifyBrushSize(1);", "" );// +1 Brush Size
   %map.bindCmd( keyboard, "+", "TerrainPainterPlugin.keyboardModifyBrushSize(1);", "" );// +1 Brush Size
   %map.bindCmd( keyboard, "-", "TerrainPainterPlugin.keyboardModifyBrushSize(-1);", "" );// -1 Brush Size
   %map.bindCmd( keyboard, "[", "TerrainPainterPlugin.keyboardModifyBrushSize(-5);", "" );// -5 Brush Size
   %map.bindCmd( keyboard, "]", "TerrainPainterPlugin.keyboardModifyBrushSize(5);", "" );// +5 Brush Size
   /*%map.bindCmd( keyboard, "]", "PaintBrushSlopeControl->SlopeMinAngle.text += 5", "" );// +5 SlopeMinAngle
   %map.bindCmd( keyboard, "[", "PaintBrushSlopeControl->SlopeMinAngle.text -= 5", "" );// -5 SlopeMinAngle
   %map.bindCmd( keyboard, "'", "PaintBrushSlopeControl->SlopeMaxAngle.text += 5", "" );// +5 SlopeMaxAngle
   %map.bindCmd( keyboard, ";", "PaintBrushSlopeControl->SlopeMaxAngle.text -= 5", "" );// -5 Softness*/

   for(%i=1; %i<10; %i++)
   {
      %map.bindCmd( keyboard, %i, "TerrainPainterPlugin.keyboardSetMaterial(" @ (%i-1) @ ");", "" );
   }
   %map.bindCmd( keyboard, 0, "TerrainPainterPlugin.keyboardSetMaterial(10);", "" );
   
   TerrainPainterPlugin.map = %map;  
   AttachWindows( EPainter, EPainterPreview);
}

function TerrainPainterPlugin::onActivated( %this )
{
   EditorGui.readTerrainEditorSettings();
   
   EWTerrainPainterToolbarBrushType->ellipse.performClick();// Circle Brush
   %this.map.push();
   
   EditorGui.bringToFront( ETerrainEditor );
   ETerrainEditor.setVisible( true );
   ETerrainEditor.attachTerrain();
   ETerrainEditor.makeFirstResponder( true );
         
   EditorGui-->TerrainPainter.setVisible(true);
   EditorGui-->TerrainPainterPreview.setVisible(true);
   EWTerrainPainterToolbar.setVisible(true);
   ETerrainEditor.onBrushChanged();
   EPainter.setup();      
   TerrainPainterPlugin.rememberBrushInfo(0);

   EditorGuiStatusBar.setSelection("");
}

function TerrainPainterPlugin::onDeactivated( %this )
{
   EditorGui.writeTerrainEditorSettings();
   
   %this.map.pop();
   EditorGui-->TerrainPainter.setVisible(false);
   EditorGui-->TerrainPainterPreview.setVisible(false);
   EWTerrainPainterToolbar.setVisible(false);
   ETerrainEditor.setVisible( false );
   TerrainPainterPlugin.rememberBrushInfo(1);
   
}

function TerrainPainterPlugin::rememberBrushInfo( %this, %storeOrCheckout)
{
   // 0 = store
   // 1 = checkout 
   
   if(%storeOrCheckout == 1)
   {
      // Remember my brush info
      ETerrainEditor.paintBrushInfo = "";
      ETerrainEditor.paintBrushInfo = PaintBrushSizeTextEditContainer-->textEdit.getText();
      ETerrainEditor.paintBrushInfo = ETerrainEditor.paintBrushInfo SPC PaintBrushSlopeControl-->SlopeMinAngle.getText();
      ETerrainEditor.paintBrushInfo = ETerrainEditor.paintBrushInfo SPC PaintBrushSlopeControl-->SlopeMaxAngle.getText();
      ETerrainEditor.paintBrushInfo = ETerrainEditor.paintBrushInfo SPC ETerrainEditor.getBrushType();
   }
   else
   {
      //Input my brush info
      //The default vaules on this brush are 9 1 1; you can change the default values by changing
      //ETerrainEditor.paintBrushInfo field in EditorGui.ed.cs. If you change the default values
      //you'll probably also want to change the default values on the text edit gui fields respectively
      
      %storedBrushInfo = ETerrainEditor.paintBrushInfo;         
      %brushType = GetWord(%storedBrushInfo, 3);
      
      PaintBrushSizeTextEditContainer-->textEdit.text = GetWord(%storedBrushInfo, 0);
      ETerrainEditor.setBrushSize( GetWord(%storedBrushInfo, 0) );
      PaintBrushSlopeControl-->SlopeMinAngle.text = GetWord(%storedBrushInfo, 1);
      PaintBrushSlopeControl-->SlopeMaxAngle.text = GetWord(%storedBrushInfo, 2);
      ETerrainEditor.setBrushPressure( 1 );
      ETerrainEditor.setBrushSoftness( 0 );
      eval( "EWTerrainPainterToolbar-->" @ %brushType @ ".setStateOn(1);" );
      //EWTerrainPainterToolbar-->(%brushType).setStateOn(1);
      ETerrainEditor.setBrushType( GetWord(%storedBrushInfo, 3) );
   }
}

function TerrainPainterPlugin::validateBrushSize( %this )
{
   %val = $ThisControl.getText();
   if(%val < 1)
      $ThisControl.setValue(1);
   else if(%val > 40)
      $ThisControl.setValue(40);
}

function TerrainPainterPlugin::keyboardModifyBrushSize( %this, %amt)
{
   %val = PaintBrushSizeTextEditContainer-->textEdit.getText();
   %val += %amt;
   PaintBrushSizeTextEditContainer-->textEdit.setValue(%val);
   PaintBrushSizeTextEditContainer-->textEdit.forceValidateText();
   ETerrainEditor.setBrushSize( PaintBrushSizeTextEditContainer-->textEdit.getText() );
}

function TerrainPainterPlugin::keyboardSetMaterial( %this, %mat)
{
   %name = "EPainterMaterialButton" @ %mat;
   %ctrl = EPainter.findObjectByInternalName(%name, true);
   if(%ctrl)
   {
      %ctrl.performClick();
   }
}

function EditorGui::setEditor( %this, %newEditor)
{   
   if ( isObject( %this.currentEditor ) )
      %this.currentEditor.onDeactivated();
      
   // If we have a special set editor function, run that instead
   if (%newEditor.isMethod(setEditorFunction))
   {
      if( %newEditor.setEditorFunction() ) 
      {
         %this.syncEditor( %newEditor );
         %this.currentEditor = %newEditor; 
         %this.currentEditor.onActivated();
      }
      else
      {
         // if were falling back and were the same editor, why are we going to just shove ourself
         // into the editor position again? opt for a fallback
         if( !isObject( %this.currentEditor ) )
            %this.currentEditor = "WorldEditorInspectorPlugin";
         else if( %this.currentEditor.getId() == %newEditor.getId() )
            %this.currentEditor = "WorldEditorInspectorPlugin";
            
         %this.syncEditor( %this.currentEditor, true );
         %this.currentEditor.onActivated();
         return;
      }
   }
   
   %this.syncEditor( %newEditor );
   %this.currentEditor = %newEditor; 
   %this.currentEditor.onActivated();
}

function EditorGui::syncEditor( %this, %newEditor, %newEditorFailed )
{
   // Sync with menu bar
   %menu = %this.findMenu( "Editors" );
   %count = %menu.getItemCount();      
   for ( %i = 0; %i < %count; %i++ )
   {
      %pluginObj = getField( %menu.item[%i], 2 );
      if ( %pluginObj $= %newEditor )
      {
         %menu.checkRadioItem( 0, %count, %i );
         break;  
      }
   }   
   
   // In order to hook up a palette, the word Palette must be able to be
   // switched out in order to read correctly, if not, no palette will be used
   %paletteName = strreplace(%newEditor, "Plugin", "Palette");
   
   // Sync with ToolsToolbar
   for ( %i = 0; %i < ToolsToolbarArray.getCount(); %i++ )
   {
      %toolbarButton = ToolsToolbarArray.getObject(%i).internalName;
      if( %paletteName $= %toolbarButton )
      {
         ToolsToolbarArray.getObject(%i).setStateOn(1);
         break;
      }
   } 
   
   // Handles quit game and gui editor changes in wierd scenarios
   if( %newEditorFailed && EWToolsToolbar.isDynamic )
   {
      if( EWToolsToolbar.isClosed )
         EWToolsToolbar.reset();
      EWToolsToolbar.toggleSize();
   }
            
   // Toggle the editor specific palette; we define special cases here
   switch$ ( %paletteName )
   {
      case "MaterialEditorPalette":
         %paletteName = "WorldEditorInspectorPalette";
      case "DatablockEditorPalette":
         %paletteName = "WorldEditorInspectorPalette";
      case "ParticleEditorPalette":
         %paletteName = "WorldEditorInspectorPalette";
   }
      
   %this-->ToolsPaletteWindow.togglePalette(%paletteName);
}

function EditorGui::onWake( %this )
{
   // Notify the editor plugins that the editor has started.
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject( %i );
      %obj.onEditorWake();      
   }  
      
   MoveMap.push();
   EditorMap.push();
   
   EditorGui.setEditor($pref::WorldEditor::currentEditor);
   EHWorldEditor.setStateOn(1);
   
   if (isObject(DemoEditorAlert) && DemoEditorAlert.helpTag<2)
      Canvas.pushDialog(DemoEditorAlert);
}

function EditorGui::onSleep(%this)
{
   // Notify the editor plugins that the editor will be closing.   
   for ( %i = 0; %i < EditorPluginSet.getCount(); %i++ )
   {
      %obj = EditorPluginSet.getObject( %i );
      %obj.onEditorSleep();      
   }
         
   EditorGui.writeWorldEditorSettings();
   $pref::WorldEditor::currentEditor = EditorGui.currentEditor;

   EditorMap.pop();
   MoveMap.pop();
   
   if(isObject($Server::CurrentScene))
      $Server::CurrentScene.open();
}

//-----------------------------------------------------------------------------

// Called when we have been set as the content and onWake has been called
function EditorGui::onSetContent(%this, %oldContent)
{
   %this.attachMenus();
}

// Called before onSleep when the canvas content is changed
function EditorGui::onUnsetContent(%this, %newContent)
{
   %this.detachMenus();
}

//------------------------------------------------------------------------------

function EditorGui::addCameraBookmark( %this, %name )
{
   %obj = new CameraBookmark() {
      datablock = CameraBookmarkMarker;
      internalName = %name;
   };

   // Place into correct group
   if( !isObject(CameraBookmarks) )
   {
      %grp = new SimGroup(CameraBookmarks);
      MissionGroup.add(%grp);
   }
   CameraBookmarks.add( %obj );

   %cam = LocalClientConnection.camera.getTransform();
   %obj.setTransform( %cam );
   
   EWorldEditor.isDirty = true;
   EditorTree.buildVisibleTree(true);
}

function EditorGui::removeCameraBookmark( %this, %name )
{
   if( !isObject(CameraBookmarks) )
      return;

   %mark = CameraBookmarks.findObjectByInternalName( %name, true );
   if( %mark == 0 )
      return;

   MEDeleteUndoAction::submit( %mark );
   EWorldEditor.isDirty = true;
   EditorTree.buildVisibleTree(true);
}

function EditorGui::removeCameraBookmarkIndex( %this, %index )
{
   if( !isObject(CameraBookmarks) )
      return;

   if( %index < 0 || %index >= CameraBookmarks.getCount() )
      return;

   %obj = CameraBookmarks.getObject( %index );
   MEDeleteUndoAction::submit( %obj );
   EWorldEditor.isDirty = true;
   EditorTree.buildVisibleTree(true);
}

function EditorGui::jumpToBookmark( %this, %name )
{
   if( !isObject(CameraBookmarks) )
      return;

   %mark = CameraBookmarks.findObjectByInternalName( %name, true );
   if( %mark == 0 )
      return;
      
   LocalClientConnection.camera.setTransform( %mark.getTransform() );
   return;
}

function EditorGui::jumpToBookmarkIndex( %this, %index )
{
   if( !isObject(CameraBookmarks) )
      return;

   if( %index < 0 || %index >= CameraBookmarks.getCount() )
      return;

   %trans = CameraBookmarks.getObject( %index ).getTransform();
   LocalClientConnection.camera.setTransform( %trans );
}

function EditorGui::addCameraBookmarkByGui( %this )
{
   // look for a NewCamera name to grab
   for(%i = 0; ; %i++){
      %name = "NewCamera_" @ %i;
      if( !CameraBookmarks.findObjectByInternalName(%name) ){
         break;
      }
   }
   EditorGui.addCameraBookmark( %name );
}

function EditorGui::toggleCameraBookmarkWindow( %this )
{
   EManageBookmarks.ToggleVisibility();
}

//------------------------------------------------------------------------------

function EditorGui::syncCameraGui( %this )
{
   if( !EditorIsActive() )
      return;
      
   %flyModeRadioItem = -1;
   if(LocalClientConnection.getControlObject() != LocalClientConnection.player)
   {
      %mode = LocalClientConnection.camera.getMode();

      if(%mode $= "Fly" && LocalClientConnection.camera.newtonMode)
      {
         if(LocalClientConnection.camera.newtonRotation == true)
         {
            EditorGui-->NewtonianRotationCamera.setStateOn(true);
            EWorldEditorToggleCamera.setBitmap("tools/gui/images/menubar/smooth-cam-rot");
            %flyModeRadioItem = 4;
            EditorGuiStatusBar.setCamera("Smooth Rot Camera");
         }
         else
         {
            EditorGui-->NewtonianCamera.setStateOn(true);
            EWorldEditorToggleCamera.setBitmap("tools/gui/images/menubar/smooth-cam");
            %flyModeRadioItem = 3;
            EditorGuiStatusBar.setCamera("Smooth Camera");
         }
      }
      else if(%mode $= "EditOrbit")
      {
         EditorGui-->OrbitCamera.setStateOn(true);
         EWorldEditorToggleCamera.setBitmap("tools/gui/images/menubar/orbit-cam");
         %flyModeRadioItem = 1;
         EditorGuiStatusBar.setCamera("Orbit Camera");
      }
		else // default camera mode
      {
         EditorGui-->StandardCamera.setStateOn(true);
         EWorldEditorToggleCamera.setBitmap("tools/worldEditor/images/toolbar/camera");
         %flyModeRadioItem = 0;
         EditorGuiStatusBar.setCamera("Standard Camera");
      }
      
      //quick way select menu bar options
      %this.findMenu( "Camera" ).checkRadioItem( 0, 1, 0 );
      EditorFreeCameraTypeOptions.checkRadioItem( 0, 4, %flyModeRadioItem);
      EditorPlayerCameraTypeOptions.checkRadioItem( 0, 4, -1);
   }
   else if (!$isFirstPersonVar) // if 3rd person
   {
      EditorGui-->trdPersonCamera.setStateOn(true);
      EWorldEditorToggleCamera.setBitmap("tools/worldEditor/images/toolbar/3rd-person-camera");
      %flyModeRadioItem = 1;
      //quick way select menu bar options
      %this.findMenu( "Camera" ).checkRadioItem( 0, 1, 1 );
      EditorPlayerCameraTypeOptions.checkRadioItem( 0, 2, %flyModeRadioItem);
      EditorGuiStatusBar.setCamera("3rd Person Camera");
   }
   else if ($isFirstPersonVar) // if 1st Person
   {
      EditorGui-->PlayerCamera.setStateOn(true);
      EWorldEditorToggleCamera.setBitmap("tools/worldEditor/images/toolbar/player");
      %flyModeRadioItem = 0;
      //quick way select menu bar options
      %this.findMenu( "Camera" ).checkRadioItem( 0, 1, 1 );
      EditorPlayerCameraTypeOptions.checkRadioItem( 0, 2, %flyModeRadioItem);
      EditorFreeCameraTypeOptions.checkRadioItem( 0, 4, -1);
      EditorGuiStatusBar.setCamera("1st Person Camera");
   }
   
   
 }

function objectTransformDropdown::toggle()
{
   if ( objectTransformDropdown.visible  )
   {
      EWorldEditorToolbar-->objectTransform.setStateOn(false);
      objectTransformDropdownDecoy.setVisible(false);
      objectTransformDropdownDecoy.setEnabled(false);
      objectTransformDropdown.setVisible(false);
   }
   else
   {
      EWorldEditorToolbar-->objectTransform.setStateOn(true);
      objectTransformDropdown.setVisible(true);
      objectTransformDropdownDecoy.setEnabled(true);
      objectTransformDropdownDecoy.setVisible(true);
   }
}

function CameraTypesDropdownToggle()
{
   if ( CameraTypesDropdown.visible  )
   {
      EWorldEditorToggleCamera.setStateOn(0);
      CameraTypesDropdownDecoy.setVisible(false);
      CameraTypesDropdownDecoy.setEnabled(false);
      CameraTypesDropdown.setVisible(false);
   }
   else
   {
      CameraTypesDropdown.setVisible(true);
      CameraTypesDropdownDecoy.setVisible(true);
      CameraTypesDropdownDecoy.setEnabled(true);
      EWorldEditorToggleCamera.setStateOn(1);
   }
}

function VisibilityDropdownToggle()
{
   if ( EVisibility.visible  )
   {
      EVisibility.setVisible(false);
      visibilityToggleBtn.setStateOn(0);
   }
   else
   {
      EVisibility.setVisible(true);
      visibilityToggleBtn.setStateOn(1);
   }
}

function CameraTypesDropdownDecoy::onMouseLeave()
{
   CameraTypesDropdownToggle();
}

//-----------------------------------------------------------------------------

function WorldEditor::toggleSnapToGrid(%this)
{
   %this.snapToGrid = !(%this.snapToGrid);
}

//-----------------------------------------------------------------------------
function EWorldEditorToggleCamera::toggleBitmap(%this)
{
   %currentImage = %this.bitmap;

   if ( %currentImage $= "tools/worldEditor/images/toolbar/player" )
      %image = "tools/worldEditor/images/toolbar/camera";
   else
      %image = "tools/worldEditor/images/toolbar/player";

   %this.setBitmap( %image );
}

function EWorldEditorCameraSpeed::updateMenuBar(%this, %editorBarCtrl)
{
   // This math is located in two places(here and in menuHandlers.ed.cs). Going 
   // to be moved to one function in the camera scritps
   //$Camera::movementSpeed = (%id / 6.0) * 195 + 5;
   if( %editorBarCtrl != CameraSpeedDropdownContainer-->textEdit )
      CameraSpeedDropdownContainer-->textEdit.setValue((%id / 6.0) * 195 + 5); // - not working -

   // Update toolbar  
   //EditorGuiToolbar-->CameraSpeedDropdown.setText( %text );
   
   // Update Editor
   EditorCameraSpeedOptions.checkRadioItem(0, 6, -1);
}

//-----------------------------------------------------------------------------

function EWorldEditorAlignPopup::onSelect(%this, %id, %text)
{
   if ( GlobalGizmoProfile.mode $= "Scale" && %text $= "World" )
   {
      EWorldEditorAlignPopup.setSelected(1);
      return;
   }
   
   GlobalGizmoProfile.alignment = %text;   
}

//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------

function EWorldEditorNoneModeBtn::onClick(%this)
{
   GlobalGizmoProfile.mode = "None";
   
   EditorGuiStatusBar.setInfo("Selection arrow.");
}

function EWorldEditorMoveModeBtn::onClick(%this)
{
   GlobalGizmoProfile.mode = "Move";
   
   EditorGuiStatusBar.setInfo("Move selection.  SHIFT while dragging duplicates objects.  CTRL to toggle soft snap.");
}

function EWorldEditorRotateModeBtn::onClick(%this)
{
   GlobalGizmoProfile.mode = "Rotate";
   
   EditorGuiStatusBar.setInfo("Rotate selection.");
}

function EWorldEditorScaleModeBtn::onClick(%this)
{
   GlobalGizmoProfile.mode = "Scale";
   
   EditorGuiStatusBar.setInfo("Scale selection.");
}

//-----------------------------------------------------------------------------

function EditorTree::onDeleteSelection( %this )
{
   %this.undoDeleteList = "";   
}

function EditorTree::onDeleteObject( %this, %object )
{
   // Don't delete locked objects
   if( %object.locked  $= "true" )
      return true;
   
   if( %object == EWCreatorWindow.objectGroup )
      EWCreatorWindow.setNewObjectGroup( MissionGroup );

   // Append it to our list.
   %this.undoDeleteList = %this.undoDeleteList TAB %object;
              
   // We're gonna delete this ourselves in the
   // completion callback.
   return true;
}

function EditorTree::onObjectDeleteCompleted( %this )
{
   MEDeleteUndoAction::submit( %this.undoDeleteList );
   
   // Let the world editor know to 
   // clear its selection.
   EWorldEditor.clearSelection();
   EWorldEditor.isDirty = true;
}

function EditorTree::onClearSelected(%this)
{
   WorldEditor.clearSelection();
}

function EditorTree::init(%this)
{
   //%this.open(MissionGroup);

   // context menu
   new GuiControl(ETContextPopupDlg)
   {
    profile = "GuiModelessDialogProfile";
      horizSizing = "width";
      vertSizing = "height";
      position = "0 0";
      extent = "640 480";
      minExtent = "8 8";
      visible = "1";
      setFirstResponder = "0";
      modal = "1";

      new GuiPopUpMenuCtrl(ETContextPopup)
      {
         profile = "GuiScrollProfile";
         position = "0 0";
         extent = "0 0";
         minExtent = "0 0";
         maxPopupHeight = "200";
         command = "canvas.popDialog(ETContextPopupDlg);";
      };
   };
   ETContextPopup.setVisible(false);

   // SimGroup context menu
   new GuiControl(ETSimGroupContextPopupDlg)
   {
    profile = "GuiPopupBackgroundProfile";
      horizSizing = "width";
      vertSizing = "height";
      position = "0 0";
      extent = "640 480";
      minExtent = "8 8";
      visible = "1";
      setFirstResponder = "0";
      command = "canvas.popDialog(ETSimGroupContextPopupDlg);";

      new GuiTextListCtrl(ETSimGroupContextPopup) {
         canSaveDynamicFields = "0";
         Enabled = "1";
         isContainer = "1";
         Profile = "GuiScrollProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "1 1";
         Extent = "421 144";
         MinExtent = "8 8";
         canSave = "1";
         Visible = "0";
         tooltipprofile = "GuiToolTipProfile";
         hovertime = "1000";
         command = "canvas.popDialog(ETSimGroupContextPopupDlg);";
         enumerate = "0";
         resizeCell = "1";
         columns = "0";
         fitParentWidth = "0";
         clipColumnText = "0";
      };
   };
   ETSimGroupContextPopup.addRow(0, "Add New Objects Here");
   ETSimGroupContextPopup.addRow(1, "Add Children to Selection");
   ETSimGroupContextPopup.addRow(2, "Remove Children from Selection");
   ETSimGroupContextPopup.groupId = -1;
   ETSimGroupContextPopup.setVisible(false);

   // CameraBookmark context menu
   new GuiControl(ETCameraBookmarkContextPopupDlg)
   {
    profile = "GuiPopupBackgroundProfile";
      horizSizing = "width";
      vertSizing = "height";
      position = "0 0";
      extent = "640 480";
      minExtent = "8 8";
      visible = "1";
      setFirstResponder = "0";
      command = "canvas.popDialog(ETCameraBookmarkContextPopupDlg);";

      new GuiTextListCtrl(ETCameraBookmarkContextPopup) {
         canSaveDynamicFields = "0";
         Enabled = "1";
         isContainer = "1";
         Profile = "GuiScrollProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "1 1";
         Extent = "421 144";
         MinExtent = "8 8";
         canSave = "1";
         Visible = "0";
         tooltipprofile = "GuiToolTipProfile";
         hovertime = "1000";
         command = "canvas.popDialog(ETCameraBookmarkContextPopupDlg);";
         enumerate = "0";
         resizeCell = "1";
         columns = "0";
         fitParentWidth = "0";
         clipColumnText = "0";
      };
   };
   ETCameraBookmarkContextPopup.addRow(0, "Go To Bookmark");
   ETCameraBookmarkContextPopup.bookmark = -1;
   ETCameraBookmarkContextPopup.setVisible(false);
}

function EditorTree::onInspect(%this, %obj)
{
   Inspector.inspect(%obj);   
}

function EditorTree::toggleLock( %this )
{
   if( EWTreeWindow-->LockSelection.command $= "EWorldEditor.lockSelection(true); EditorTree.toggleLock();" )
   {
      EWTreeWindow-->LockSelection.command = "EWorldEditor.lockSelection(false); EditorTree.toggleLock();";
      EWTreeWindow-->DeleteSelection.command = "";
   }
   else
   {
      EWTreeWindow-->LockSelection.command = "EWorldEditor.lockSelection(true); EditorTree.toggleLock();";
      EWTreeWindow-->DeleteSelection.command = "EditorMenuEditDelete();";
   }
}

function EditorTree::onAddSelection(%this, %obj)
{
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
      EWTreeWindow-->DeleteSelection.command = "";
      
}
function EditorTree::onRemoveSelection(%this, %obj)
{
   EWorldEditor.unselectObject(%obj);
}
function EditorTree::onSelect(%this, %obj)
{
   EWorldEditor.selectObject(%obj);
}

function EditorTree::onUnselect(%this, %obj)
{
   EWorldEditor.unselectObject(%obj);
}

function EditorTree::onDragDropped(%this)
{
   EWorldEditor.isDirty = true;
}

function EditorTree::onAddGroupSelected(%this, %group)
{
   EWCreatorWindow.setNewObjectGroup(%group);
}

function EditorTree::onRightMouseDown( %this, %itemId, %mouse, %obj )
{
   // Open context menu if this is a SimGroup
   if( %obj.isMemberOfClass("SimGroup") )
   {
      canvas.pushDialog(ETSimGroupContextPopupDlg);
      ETSimGroupContextPopup.clearSelection();
      ETSimGroupContextPopup.position = %mouse;
      ETSimGroupContextPopup.groupId = %obj;
      
      %this.positionContextMenu( ETSimGroupContextPopup );
      ETSimGroupContextPopup.setVisible(true);
   }
   
   // Open context menu if this is a CameraBookmark
   if( %obj.isMemberOfClass("CameraBookmark") )
   {
      canvas.pushDialog(ETCameraBookmarkContextPopupDlg);
      ETCameraBookmarkContextPopup.clearSelection();
      ETCameraBookmarkContextPopup.position = %mouse;
      ETCameraBookmarkContextPopup.bookmark = %obj;
      
      %this.positionContextMenu( ETCameraBookmarkContextPopup );
      ETCameraBookmarkContextPopup.setVisible(true);
   }

}

function EditorTree::positionContextMenu( %this, %menu )
{
   if( (getWord(%menu.position, 0) + getWord(%menu.extent, 0)) > getWord(EWorldEditor.extent, 0) )
   {
      %posx = getWord(%menu.position, 0);
      %offset = getWord(EWorldEditor.extent, 0) - (%posx + getWord(%menu.extent, 0)) - 5;
      %posx += %offset;
      %menu.position = %posx @ " " @ getWord(%menu.position, 1);
   }
}

function EditorTreeTabBook::onTabSelected( %this )
{
   if( EditorTreeTabBook.getSelectedPage() == 0)
   {
      EWTreeWindow-->DeleteSelection.visible = true;
      EWTreeWindow-->LockSelection.visible = true;
   }
   else
   {
      EWTreeWindow-->DeleteSelection.visible = false;
      EWTreeWindow-->LockSelection.visible = false;
   }
}

function ETContextPopup::onSelect(%this, %index, %value)
{
   switch(%index)
   {
      case 0:
         EditorTree.contextObj.delete();
   }
}

function ETSimGroupContextPopup::onSelect( %this, %id, %text)
{
   if( %id == 0 )
   {
      // Add new objects here
      %group = ETSimGroupContextPopup.groupId;
      EWCreatorWindow.setNewObjectGroup( %group );
   }
   else if( %id == 1 )
   {
      // Selected
      %group = ETSimGroupContextPopup.groupId;
      %count = %group.getCount();
      for( %i=0; %i<%count; %i++)
      {
         %obj = %group.getObject(%i);
         EWorldEditor.selectObject(%obj);
      }
   }
   else if( %id == 2 )
   {
      // Deselect
      %group = ETSimGroupContextPopup.groupId;
      %count = %group.getCount();
      for( %i=0; %i<%count; %i++)
      {
         %obj = %group.getObject(%i);
         EWorldEditor.unselectObject(%obj);
      }
   }
}

function ETCameraBookmarkContextPopup::onSelect( %this, %id, %text)
{
   if( %id == 0 )
   {
      // Go to bookmark
      EditorGui.jumpToBookmark( ETCameraBookmarkContextPopup.bookmark.getInternalName() );
   }
}

//------------------------------------------------------------------------------

function Editor::open(%this)
{
   // prevent the mission editor from opening while the GuiEditor is open.
   if(Canvas.getContent() == GuiEditorGui.getId())
      return;

   Canvas.setContent(EditorGui);
   %this.editorEnabled();
   EditorGui.syncCameraGui();
}

function Editor::close(%this, %gui)
{
   Canvas.setContent(%gui);
   if(isObject(MessageHud))
      MessageHud.close();
   %this.editorDisabled();
}

$RelightCallback = "";

function EditorLightingComplete()
{
   $lightingMission = false;
   RelightStatus.visible = false;
   
   if ($RelightCallback !$= "")
   {
      eval($RelightCallback);
   }
   
   $RelightCallback = "";
}

function updateEditorLightingProgress()
{
   RelightProgress.setValue(($SceneLighting::lightingProgress));
   if ($lightingMission)
      $lightingProgressThread = schedule(1, 0, "updateEditorLightingProgress");
}

function Editor::lightScene(%this, %callback, %forceAlways)
{
   if ($lightingMission)
      return;
      
   $lightingMission = true;
   $RelightCallback = %callback;
   RelightStatus.visible = true;
   RelightProgress.setValue(0);
   Canvas.repaint();  
   lightScene("EditorLightingComplete", %forceAlways);
   updateEditorLightingProgress();
} 

//------------------------------------------------------------------------------

function EditorGui::handleEscape( %this )
{
   %result = false;
   if ( isObject( %this.currentEditor ) )
      %result = %this.currentEditor.handleEscape();
      
   if ( !%result )
   {
     Editor.close("PlayGui");
   }
}

function EditTSCtrl::updateGizmoMode( %this, %mode )
{
   // Called when the gizmo mode is changed from C++
   
   if ( %mode $= "None" )
      EditorGuiToolbar->NoneModeBtn.performClick();
   else if ( %mode $= "Move" )   
      EditorGuiToolbar->MoveModeBtn.performClick();
   else if ( %mode $= "Rotate" )
      EditorGuiToolbar->RotateModeBtn.performClick();
   else if ( %mode $= "Scale" )
      EditorGuiToolbar->ScaleModeBtn.performClick();
}

//------------------------------------------------------------------------------

function EWorldEditor::syncGui( %this )
{
   %this.syncToolPalette();
   
   EWorldEditorToolbar-->dropAtBoundsBtn.setStateOn( EWorldEditor.dropAtBounds );
   EWorldEditorToolbar-->boundingBoxColBtn.setStateOn( EWorldEditor.boundingBoxCollision );
      
   if( EWorldEditor.objectsUseBoxCenter )
   {
      EWorldEditorToolbar-->centerObject.setBitmap("tools/gui/images/menubar/bounds-center");
      objectCenterDropdown-->objectBoundsBtn.setStateOn( 1 );
   }
   else
   {
      EWorldEditorToolbar-->centerObject.setBitmap("tools/gui/images/menubar/object-center");
      objectCenterDropdown-->objectBoxBtn.setStateOn( 1 );
   }
   
   if( GlobalGizmoProfile.getFieldValue(alignment) $= "Object" )
   {
      EWorldEditorToolbar-->objectTransform.setBitmap("tools/gui/images/menubar/object-transform");
      objectTransformDropdown-->objectTransformBtn.setStateOn( 1 );
      
   }
   else
   {
      EWorldEditorToolbar-->objectTransform.setBitmap("tools/gui/images/menubar/world-transform");
      objectTransformDropdown-->worldTransformBtn.setStateOn( 1 );
   }
   
   EWorldEditorToolbar-->renderHandleBtn.setStateOn( EWorldEditor.renderObjHandle );
   EWorldEditorToolbar-->renderTextBtn.setStateOn( EWorldEditor.renderObjText );
   
   EWorldEditorToolbar-->softSnapSizeTextEdit.setText( EWorldEditor.getSoftSnapSize() );
   ESnapOptions-->SnapSize.setText( EWorldEditor.getSoftSnapSize() );
   
   if(SnapToBar-->objectSnapDownBtn.getValue())
   {
      ESnapOptions-->TerrainSnapButton.setStateOn(1);
   }
   else if(SnapToBar-->objectSnapBtn.getValue())
   {
      ESnapOptions-->SoftSnapButton.setStateOn(1);
   }
   else
   {
      ESnapOptions-->NoSnapButton.setStateOn(1);
   }
}

function EWorldEditor::syncToolPalette( %this )
{
   switch$ ( GlobalGizmoProfile.mode )
   {
      case "None":
         EWorldEditorNoneModeBtn.performClick();
      case "Move":
         EWorldEditorMoveModeBtn.performClick();
      case "Rotate":
         EWorldEditorRotateModeBtn.performClick();
      case "Scale":
         EWorldEditorScaleModeBtn.performClick();
   }
}

function toggleSnapingOptions(%var)
{
      if(SnapToBar->objectSnapDownBtn.getValue()&&SnapToBar->objectSnapBtn.getValue()){
         if(%var $= "terrain"){
            EWorldEditor.stickToGround = 1;
            EWorldEditor.setSoftSnap(false);
            SnapToBar->objectSnapBtn.setStateOn(0);
            EWorldEditor.syncGui();
         }else{ // soft snaping
            EWorldEditor.stickToGround = 0;
            EWorldEditor.setSoftSnap(true);
            SnapToBar->objectSnapDownBtn.setStateOn(0);
            EWorldEditor.syncGui();
         }
      }else if(%var $= "terrain" && EWorldEditor.stickToGround == 0){ // Terrain Snaping
         EWorldEditor.stickToGround = 1;
         EWorldEditor.setSoftSnap(false);
         SnapToBar->objectSnapDownBtn.setStateOn(1);
         SnapToBar->objectSnapBtn.setStateOn(0);
         EWorldEditor.syncGui();
         
      }else if(%var $= "soft" && EWorldEditor.getSoftSnap() == false){ // Object Snaping
         EWorldEditor.stickToGround = 0;
         EWorldEditor.setSoftSnap(true);
         SnapToBar->objectSnapBtn.setStateOn(1);
         SnapToBar->objectSnapDownBtn.setStateOn(0);
         EWorldEditor.syncGui();
         
      }else{ // No snaping
         EWorldEditor.stickToGround = 0;
         EWorldEditor.setSoftSnap(false);
         SnapToBar->objectSnapDownBtn.setStateOn(0);
         SnapToBar->objectSnapBtn.setStateOn(0);
         EWorldEditor.syncGui();
      }
}

function objectCenterDropdown::toggle()
{
   if ( objectCenterDropdown.visible  )
   {
      EWorldEditorToolbar-->centerObject.setStateOn(false);
      objectCenterDropdownDecoy.setVisible(false);
      objectCenterDropdownDecoy.setEnabled(false);
      objectCenterDropdown.setVisible(false);
   }
   else
   {
      EWorldEditorToolbar-->centerObject.setStateOn(true);
      objectCenterDropdown.setVisible(true);
      objectCenterDropdownDecoy.setEnabled(true);
      objectCenterDropdownDecoy.setVisible(true);
   }
}

function objectTransformDropdown::toggle()
{
   if ( objectTransformDropdown.visible  )
   {
      EWorldEditorToolbar-->objectTransform.setStateOn(false);
      objectTransformDropdownDecoy.setVisible(false);
      objectTransformDropdownDecoy.setEnabled(false);
      objectTransformDropdown.setVisible(false);
   }
   else
   {
      EWorldEditorToolbar-->objectTransform.setStateOn(true);
      objectTransformDropdown.setVisible(true);
      objectTransformDropdownDecoy.setEnabled(true);
      objectTransformDropdownDecoy.setVisible(true);
   }
}

function objectSnapDropdownDecoy::onMouseLeave()
{
   objectSnapDropdown.toggle();
}

function objectCenterDropdownDecoy::onMouseLeave()
{
   objectCenterDropdown.toggle();
}

function objectTransformDropdownDecoy::onMouseLeave()
{
   objectTransformDropdown.toggle();
}

//------------------------------------------------------------------------------

function EWToolsToolbar::reset( %this )
{
   %count = ToolsToolbarArray.getCount();
   for( %i = 0 ; %i < %count; %i++ )
      ToolsToolbarArray.getObject(%i).setVisible(true);

   %this.setExtent((29 + 4) * %count + 12, 33);
   %this.isClosed = 0;
   EWToolsToolbar.isDynamic = 0;
      
   EWToolsToolbarDecoy.setVisible(false);
   EWToolsToolbarDecoy.setExtent((29 + 4) * %count + 4, 31);

  %this-->resizeArrow.setBitmap( "core/art/gui/images/collapse-toolbar" );
}

function EWToolsToolbar::toggleSize( %this, %useDynamics )
{
   // toggles the size of the tooltoolbar. also goes through 
   // and hides each control not currently selected. we hide the controls
   // in a very neat, spiffy way

   if ( %this.isClosed == 0 )
   {
      %image = "core/art/gui/images/expand-toolbar";
      
      for( %i = 0 ; %i < ToolsToolbarArray.getCount(); %i++ )
      {
         if( ToolsToolbarArray.getObject(%i).getValue() != 1 )
            ToolsToolbarArray.getObject(%i).setVisible(false);
      }
         
      %this.setExtent(43, 33);
      %this.isClosed = 1;
      
      if(!%useDynamics)
      {
         EWToolsToolbarDecoy.setVisible(true);
         EWToolsToolbar.isDynamic = 1;
      }
         
      EWToolsToolbarDecoy.setExtent(35, 31);
   }
   else
   {
      %image = "core/art/gui/images/collapse-toolbar";

      %count = ToolsToolbarArray.getCount();
      for( %i = 0 ; %i < %count; %i++ )
         ToolsToolbarArray.getObject(%i).setVisible(true);
      
      %this.setExtent((29 + 4) * %count + 12, 33);
      %this.isClosed = 0;
      
      if(!%useDynamics)
      {
         EWToolsToolbarDecoy.setVisible(false);
         EWToolsToolbar.isDynamic = 0;
      }

      EWToolsToolbarDecoy.setExtent((29 + 4) * %count + 4, 32);
   }

  %this-->resizeArrow.setBitmap( %image );
  
}

function EWToolsToolbarDecoy::onMouseEnter( %this )
{
   EWToolsToolbar.toggleSize(true);
}

function EWToolsToolbarDecoy::onMouseLeave( %this )
{
   EWToolsToolbar.toggleSize(true);
}

//------------------------------------------------------------------------------

function EditorGuiStatusBar::reset( %this )
{
   EWorldEditorStatusBarInfo.clearInfo();
}

function EditorGuiStatusBar::getInfo( %this )
{
   return EWorldEditorStatusBarInfo.getValue();
}

function EditorGuiStatusBar::setInfo( %this, %text )
{
   EWorldEditorStatusBarInfo.setText(%text);
}

function EditorGuiStatusBar::clearInfo( %this )
{
   EWorldEditorStatusBarInfo.setText("");
}

function EditorGuiStatusBar::getSelection( %this )
{
   return EWorldEditorStatusBarSelection.getValue();
}

function EditorGuiStatusBar::setSelection( %this, %text )
{
   EWorldEditorStatusBarSelection.setText(%text);
}

function EditorGuiStatusBar::setSelectionObjectsByCount( %this, %count )
{
   %text = " objects selected";
   if(%count == 1)
      %text = " object selected";

   EWorldEditorStatusBarSelection.setText(%count @ %text);
}

function EditorGuiStatusBar::clearSelection( %this )
{
   EWorldEditorStatusBarSelection.setText("");
}

function EditorGuiStatusBar::getCamera( %this )
{
   return EWorldEditorStatusBarCamera.getValue();
}

function EditorGuiStatusBar::setCamera( %this, %text )
{
   EWorldEditorStatusBarCamera.setText(%text);
}

function EditorGuiStatusBar::clearCamera( %this )
{
   EWorldEditorStatusBarCamera.setText("");
}

//------------------------------------------------------------------------------------
// Each a gui slider bar is pushed on the editor gui, it maps itself with value
// located in its connected text control
//------------------------------------------------------------------------------------
function softSnapSizeSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(EWorldEditorToolbar-->softSnapSizeTextEdit.getValue());
}
function softSnapSizeSliderCtrlContainer::onSliderChanged(%this)
{
   EWorldEditor.setSoftSnapSize( %this-->slider.value );
   EWorldEditor.syncGui();
}
//------------------------------------------------------------------------------------
function PaintBrushSizeSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(PaintBrushSizeTextEditContainer-->textEdit.getValue());
}
/*
function PaintBrushPressureSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(PaintBrushPressureTextEditContainer-->textEdit.getValue());
}

function PaintBrushSoftnessSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(PaintBrushSoftnessTextEditContainer-->textEdit.getValue());
}*/
//------------------------------------------------------------------------------------
function TerrainBrushSizeSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(TerrainBrushSizeTextEditContainer-->textEdit.getValue());
}

function TerrainBrushPressureSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(TerrainBrushPressureTextEditContainer-->textEdit.getValue() / 100.0);
}

function TerrainBrushSoftnessSliderCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(TerrainBrushSoftnessTextEditContainer-->textEdit.getValue() / 100.0);
}
//------------------------------------------------------------------------------------
function CameraSpeedDropdownCtrlContainer::onWake(%this)
{
   %this-->slider.setValue(CameraSpeedDropdownContainer-->textEdit.getText());
}