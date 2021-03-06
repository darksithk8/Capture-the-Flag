//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// The texture filename filter used with OpenFileDialog.
$TerrainEditor::TextureFileSpec = "Image Files (*.png, *.jpg, *.dds)|*.png;*.jpg;*.dds|All Files (*.*)|*.*|";

function TerrainEditor::init( %this )
{
   %this.attachTerrain();
   %this.setBrushSize( 9, 9 );
      
   new PersistenceManager( ETerrainPersistMan );   
}

///
function EPainter_TerrainMaterialUpdateCallback( %mat, %matIndex )
{           
   // Skip over a bad selection.
   if ( %matIndex == -1 || !isObject( %mat ) )   
      return;
                        
   // Update the material and the UI.
   ETerrainEditor.updateMaterial( %matIndex, %mat.getInternalName() );
   EPainter.setup( %matIndex );            
}

function EPainter_TerrainMaterialAddCallback( %mat, %matIndex )
{           
   // Ignore bad materials.
   if ( !isObject( %mat ) )
      return;

   // Add it and update the UI.                              
   %matIndex = ETerrainEditor.addMaterial( %mat.getInternalName() );
   EPainter.setup( %matIndex );            
}

function TerrainEditor::setPaintMaterial( %this, %matIndex, %terrainMat )
{
   assert( isObject( %terrainMat ), "TerrainEditor::setPaintMaterial - Got bad material!" );
   
   ETerrainEditor.paintIndex = %matIndex;
   ETerrainMaterialSelected.selectedMatIndex = %matIndex;
   ETerrainMaterialSelected.selectedMat = %terrainMat;
   ETerrainMaterialSelected.bitmap = %terrainMat.diffuseMap;
   ETerrainMaterialSelectedEdit.Visible = isObject(%terrainMat);
   TerrainTextureText.text = %terrainMat.getInternalName();      
}

function TerrainEditor::setup( %this )
{   
   %action = %this.savedAction;
   %desc = %this.savedActionDesc;
   if ( %this.savedAction $= "" )
   {
      %action = brushAdjustHeight;
      %desc = "Adjust terrain height";
   }
      
   %this.switchAction( %action, %desc );
}

function EPainter::updateLayers( %this, %matIndex )
{
   // Default to whatever was selected before.
   if ( %matIndex $= "" )
      %matIndex = ETerrainEditor.paintIndex;
      
   // The material string is a newline seperated string of 
   // TerrainMaterial internal names which we can use to find
   // the actual material data in TerrainMaterialSet.
   
   %mats = ETerrainEditor.getMaterials();
   
   %matList = %this-->theMaterialList;
   %matList.deleteAllObjects();
   %listWidth = getWord( %matList.getExtent(), 0 );
   
   for( %i = 0; %i < getRecordCount( %mats ); %i++ )
   {
      %matInternalName = getRecord( %mats, %i );      
      %mat = TerrainMaterialSet.findObjectByInternalName( %matInternalName );
      
      // Is there no material info for this slot?
      if ( !isObject( %mat ) )
         continue;

      %index = %matList.getCount();
      %command = "ETerrainEditor.setPaintMaterial( " @ %index @ ", " @ %mat @ " );";      
      %altCommand = "TerrainMaterialDlg.show( " @ %index @ ", " @ %mat @ ", EPainter_TerrainMaterialUpdateCallback );";
      
      %ctrl = new GuiIconButtonCtrl()
      {            
         internalName = "EPainterMaterialButton" @ %i;
         profile = "GuiCreatorIconButtonProfile";         
         iconLocation = "Left";
         textLocation = "Right";
         extent = %listWidth SPC "46";
         textMargin = 5;
         buttonMargin = "4 4";
         buttonType = "RadioButton";      
         sizeIconToButton = true;
         makeIconSquare = true;   
         tooltipprofile = "GuiToolTipProfile";
         command = %command;
         altCommand = %altCommand;
         
         // TODO: Add a nested button for delete?
      };
      
      %ctrl.setText( %matInternalName );
      %ctrl.setBitmap( %mat.diffuseMap );

      %tooltip = %matInternalName;
      if(%i < 9)
         %tooltip = %tooltip @ " (" @ (%i+1) @ ")";
      else if(%i == 9)
         %tooltip = %tooltip @ " (0)";
      %ctrl.tooltip = %tooltip;

      %matList.add( %ctrl );
   }
   
   %matCount = %matList.getCount();
   
   // Add one more layer as the 'add new' layer.
   %ctrl = new GuiIconButtonCtrl()
   {            
      profile = "GuiCreatorIconButtonProfile";         
      iconBitmap = "~/worldEditor/images/terrainpainter/new_layer_icon";
      iconLocation = "Left";
      textLocation = "Right";
      extent = %listWidth SPC "46";
      textMargin = 5;
      buttonMargin = "4 4";
      buttonType = "PushButton";
      sizeIconToButton = true;
      makeIconSquare = true;   
      tooltipprofile = "GuiToolTipProfile";
      text = "New Layer";
      tooltip = "New Layer";
      command = "TerrainMaterialDlg.show( " @ %matCount @ ", 0, EPainter_TerrainMaterialAddCallback );";
   };
   %matList.add( %ctrl );
               
   // Make sure our selection is valid and that we're
   // not selecting the 'New Layer' button.
   %matIndex = %matCount < %matIndex ? %matCount - 1 : %matIndex;
   if ( %matIndex < 1 )
      return;

   // To make things simple... click the paint material button to
   // active it and initialize other state.   
   %ctrl = %matList.getObject( %matIndex );
   %ctrl.performClick();
}

function EPainter::setup( %this, %matIndex )
{
   // Update the layer listing.
   %this.updateLayers( %matIndex );
   
   // Automagically put us into material paint mode.
   ETerrainEditor.currentMode = "paint";
   ETerrainEditor.selectionHidden = true;
   ETerrainEditor.currentAction = paintMaterial;
   ETerrainEditor.currentActionDesc = "Paint material on terrain";
   ETerrainEditor.setAction( ETerrainEditor.currentAction );
   EditorGuiStatusBar.setInfo(ETerrainEditor.currentActionDesc);
   ETerrainEditor.renderVertexSelection = true;
}

function onNeedRelight()
{
   if( RelightMessage.visible == false )
      RelightMessage.visible = true;
}

function TerrainEditor::onGuiUpdate(%this, %text)
{
   %minHeight = getWord(%text, 1);
   %avgHeight = getWord(%text, 2);
   %maxHeight = getWord(%text, 3);

   %mouseBrushInfo = " (Mouse) #: " @ getWord(%text, 0) @ "  avg: " @ %avgHeight @ " " @ ETerrainEditor.currentAction;
   %selectionInfo = "     (Selected) #: " @ getWord(%text, 4) @ "  avg: " @ getWord(%text, 5);

   TEMouseBrushInfo.setValue(%mouseBrushInfo);
   TEMouseBrushInfo1.setValue(%mouseBrushInfo);
   TESelectionInfo.setValue(%selectionInfo);
   TESelectionInfo1.setValue(%selectionInfo);

   EditorGuiStatusBar.setSelection("min: " @ %minHeight @ "  avg: " @ %avgHeight @ "  max: " @ %maxHeight);
}

function TerrainEditor::onBrushChanged( %this )
{
   if ( EditorGui.currentEditor.getId() == TerrainPainterPlugin.getId() )
      %parent = EditorGui-->TerrainPainter;
   else if ( EditorGui.currentEditor.getId() == TerrainEditorPlugin.getId() )
      %parent = EditorGui-->TerrainEdit;
}

function TerrainEditor::toggleBrushType( %this, %brush )
{
   %this.setBrushType( %brush.internalName );                        
}

function TerrainEditor::offsetBrush(%this, %x, %y)
{
   %curPos = %this.getBrushPos();
   %this.setBrushPos(getWord(%curPos, 0) + %x, getWord(%curPos, 1) + %y);
}

function TerrainEditor::onActiveTerrainChange(%this, %newTerrain)
{
   // Need to refresh the terrain painter.
   if ( EditorGui.currentEditor.getId() == TerrainPainterPlugin.getId() )
      EPainter.setup(ETerrainEditor.paintIndex);
}

/// This is only ment for terrain editing actions and not
/// processed actions or the terrain material painting action.
function TerrainEditor::switchAction( %this, %action, %actionDesc )
{
   %this.currentMode = "paint";
   %this.selectionHidden = true;
   %this.currentAction = %action;
   %this.currentActionDesc = %actionDesc;
   %this.savedAction = %action;
   %this.savedActionDesc = %actionDesc;
   
   if (  %action $= "setEmpty" || 
         %action $= "clearEmpty" ||
          %action $= "setHeight" )         
      %this.renderSolidBrush = true;
   else
      %this.renderSolidBrush = false;
      
   EditorGuiStatusBar.setInfo(%this.currentActionDesc);

   %this.setAction( %this.currentAction );      
}

function TerrainEditor::onSmoothHeightmap( %this )
{
   if ( !%this.getActiveTerrain() )
      return;
         
   // Show the dialog first and let the user
   // set the smoothing parameters.
   
   
   
   // Now create the terrain smoothing action to
   // get the work done and perform later undos.
   %action = new TerrainSmoothAction();
   %action.smooth( %this.getActiveTerrain(), 1.0, 1 );
   %action.addToManager( EUndoManager );   
}

//------------------------------------------------------------------------------
// Functions
//------------------------------------------------------------------------------

function TerrainEditorSettingsGui::onWake(%this)
{
   TESoftSelectFilter.setValue(ETerrainEditor.softSelectFilter);
}

function TerrainEditorSettingsGui::onSleep(%this)
{
   ETerrainEditor.softSelectFilter = TESoftSelectFilter.getValue();
}

function TESettingsApplyButton::onAction(%this)
{
   ETerrainEditor.softSelectFilter = TESoftSelectFilter.getValue();
   ETerrainEditor.resetSelWeights(true);
   ETerrainEditor.processAction("softSelect");
}

function getPrefSetting(%pref, %default)
{
   //
   if(%pref $= "")
      return(%default);
   else
      return(%pref);
}

function TerrainEditorPlugin::setEditorFunction(%this)
{
   %terrainExists = parseMissionGroup( "TerrainBlock" );

   if( %terrainExists == false )
      MessageBoxYesNoCancel("No Terrain","Would you like to create a New Terrain?", "Canvas.pushDialog(CreateNewTerrainGui);");
   
   return %terrainExists;
}

function TerrainPainterPlugin::setEditorFunction(%this, %overrideGroup)
{ 
   %terrainExists = parseMissionGroup( "TerrainBlock" );

   if( %terrainExists == false )
      MessageBoxYesNoCancel("No Terrain","Would you like to create a New Terrain?", "Canvas.pushDialog(CreateNewTerrainGui);");
   
   return %terrainExists;
}