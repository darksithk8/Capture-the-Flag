//------------------------------------------------------------------------------
// Shape Editor
//------------------------------------------------------------------------------

function initializeShapeEditor()
{
   echo(" % - Initializing Shape Editor");

   exec("./gui/Profiles.ed.cs");

   exec("./gui/shapeEdPreviewWindow.gui");
   exec("./gui/shapeEditorToolbar.ed.gui");
   exec("./scripts/shapeEditorGui.cs");

   exec("./gui/shapeEdSelectWindow.gui");
   exec("./gui/shapeEdPropWindow.gui");
   exec("./scripts/shapeEditorHints.cs");
   exec("./scripts/shapeEditorActions.cs");

   // Add windows to editor gui
   ShapeEdPreviewGui.setVisible(false);
   ShapeEdPreviewWindow.setVisible(false);

   ShapeEditorToolbar.setVisible(false);
   ShapeEdSelectWindow.setVisible(false);
   ShapeEdPropWindow.setVisible(false);

   EditorGui.add(ShapeEdPreviewGui);
   EditorGui.add(ShapeEdPreviewWindow);

   EditorGui.add(ShapeEditorToolbar);
   EditorGui.add(ShapeEdSelectWindow);
   EditorGui.add(ShapeEdPropWindow);

   new ScriptObject(ShapeEditorPlugin)
   {
      superClass = "EditorPlugin";
   };
   %map = new ActionMap();
   %map.bindCmd( keyboard, "escape", "ToolsToolbarArray->WorldEditorInspectorPalette.performClick();", "" );
   %map.bindCmd( keyboard, "1", "ShapeEditorNoneModeBtn.performClick();", "" );
   %map.bindCmd( keyboard, "2", "ShapeEditorMoveModeBtn.performClick();", "" );
   %map.bindCmd( keyboard, "3", "ShapeEditorRotateModeBtn.performClick();", "" );
   //%map.bindCmd( keyboard, "4", "ShapeEditorScaleModeBtn.performClick();", "" ); // not needed for the shape editor
   %map.bindCmd( keyboard, "n", "ShapeEditorToolbar->showNodes.performClick();", "" );
   %map.bindCmd( keyboard, "t", "ShapeEditorToolbar->ghostMode.performClick();", "" );
   %map.bindCmd( keyboard, "r", "ShapeEditorToolbar->wireframeMode.performClick();", "" );
   %map.bindCmd( keyboard, "f", "ShapeEditorToolbar->fitToShapeBtn.performClick();", "" );
   %map.bindCmd( keyboard, "g", "ShapeEditorToolbar->showGridBtn.performClick();", "" );
   %map.bindCmd( keyboard, "h", "ShapeEdSelectWindow->tabBook.selectPage( 2 );", "" ); // Load help tab
   %map.bindCmd( keyboard, "l", "ShapeEdSelectWindow->tabBook.selectPage( 1 );", "" ); // load Library Tab
   %map.bindCmd( keyboard, "j", "ShapeEdSelectWindow->tabBook.selectPage( 0 );", "" ); // load scene object Tab
   %map.bindCmd( keyboard, "SPACE", "ShapeEdPreviewWindow.togglePlayMode();", "" );
   %map.bindCmd( keyboard, "i", "ShapeEdSequences.onEditSeqInOut(\"in\", ShapeEdSeqSlider.getValue());", "" );
   %map.bindCmd( keyboard, "o", "ShapeEdSequences.onEditSeqInOut(\"out\", ShapeEdSeqSlider.getValue());", "" );
   %map.bindCmd( keyboard, "shift -", "ShapeEdSeqSlider.setValue(ShapeEdPreviewWindow-->seqIn.getText());", "" );
   %map.bindCmd( keyboard, "shift =", "ShapeEdSeqSlider.setValue(ShapeEdPreviewWindow-->seqOut.getText());", "" );
   %map.bindCmd( keyboard, "=", "ShapeEdSeqSlider.setValue(mFloor(ShapeEdSeqSlider.getValue() + 1));", "" );
   %map.bindCmd( keyboard, "-", "ShapeEdSeqSlider.setValue(mCeil(ShapeEdSeqSlider.getValue() - 1));", "" );

   ShapeEditorPlugin.map = %map;
}

function destroyShapeEditor()
{
}

function ShapeEditorPlugin::onWorldEditorStartup(%this)
{
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu("Shape Editor", "", ShapeEditorPlugin);

   // Add ourselves to the ToolsToolbar
   %tooltip = "Shape Editor (" @ %accel @ ")";
   EditorGui.addToToolsToolbar( "ShapeEditorPlugin", "ShapeEditorPalette", expandFilename("tools/worldEditor/images/toolbar/shape-editor"), %tooltip );

   AttachWindows(ShapeEdPropWindow, ShapeEdSelectWindow);
   ShapeEdPreviewWindow.resize( -1, 526, 593, 53 );
   
   // Initialise gui
   ShapeEdSeqNodeTabBook.selectPage(0);
   ShapeEdSelectWindow-->tabBook.selectPage(0);
   ShapeEdSelectWindow.navigate("");
   ShapeEdNodes-->objectTransform.setStateOn(1);

   ShapeEditorToolbar-->showGridBtn.setValue(1);
   ShapeEditorToolbar-->showNodes.setValue(1);
   ShapeEditorToolbar-->showBounds.setValue(0);
   //ShapeEditorToolbar-->showPreview.setValue(1);
   ShapeEdShapeView.setTimeScale(1.0);
   ShapeEdShapeView.setSliderCtrl(ShapeEdSeqSlider);

   // Initialise hints menu
   ShapeEdHintMenu.clear();
   %count = ShapeHintGroup.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %hint = ShapeHintGroup.getObject(%i);
      ShapeEdHintMenu.add(%hint.objectType, %hint);
   }
}

function ShapeEditorPlugin::onActivated(%this)
{
   $wasInWireFrameMode = $gfx::wireframe;
   ShapeEditorToolbar-->wireframeMode.setStateOn($gfx::wireframe);

   ShapeEditorToolbar.setVisible(true);

   // Initialise and show the shape editor
   ShapeEdShapeTreeView.open(MissionGroup);
   ShapeEdShapeTreeView.buildVisibleTree(true);

   ShapeEdPreviewGui.setVisible(true);
   ShapeEdSelectWindow.setVisible(true);
   ShapeEdPropWindow.setVisible(true);
   ShapeEdPreviewWindow.setVisible(true);

   EditorGuiStatusBar.setInfo("Shape editor ( Shift Click ) to speed up camera.");
   EditorGuiStatusBar.setSelection("");
   EditorGuiStatusBar.setCamera("Orbit Camera");

   if (ShapeEditor.shape <= 0)
      ShapeEditor.setDirty(false);

   // Try to start with the shape selected in the world editor
   %count = EWorldEditor.getSelectionSize();
   for (%i = 0; %i < %count; %i++)
   {
      %obj = EWorldEditor.getSelectedObject(%i);
      %shapeFile = ShapeEditor.getObjectShapeFile(%obj);
      if (%shapeFile !$= "")
      {
         // Call the 'onSelect' method directly if the object is not in the
         // MissionGroup tree (such as a Player or Projectile object).
         ShapeEdShapeTreeView.clearSelection();
         if (!ShapeEdShapeTreeView.selectItem(%obj))
            ShapeEdShapeTreeView.onSelect(%obj);

         // 'fitToShape' only works after the GUI has been rendered, so force a repaint first
         Canvas.repaint();
         ShapeEdShapeView.fitToShape();
         return;
      }
   }
   ToolsPaletteArray->WorldEditorMove.performClick();
   %this.map.push();
   
   Parent::onActivated(%this);
}

function ShapeEditorPlugin::onDeactivated(%this)
{
   $gfx::wireframe = $wasInWireFrameMode;

   ShapeEditorToolbar.setVisible(false);
   EditorGui.syncCameraGui();

   ShapeEdPreviewGui.setVisible(false);
   ShapeEdSelectWindow.setVisible(false);
   ShapeEdPropWindow.setVisible(false);
   ShapeEdPreviewWindow.setVisible(false);
   %this.map.pop();
   
   Parent::onDeactivated(%this);
}

function shapeEditorWireframeMode()
{
   $gfx::wireframe = !$gfx::wireframe;
   ShapeEditorToolbar-->wireframeMode.setStateOn($gfx::wireframe);
}
