//---------------------------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//---------------------------------------------------------------------------------------------

$Pref::GuiEditor::GuiFilterList = "GuiEditorGui" TAB "MessageBoxOKDlg" TAB "MessageBoxOKCancelDlg" TAB "MessageBoxOKCancelDetailsDlg" TAB "MessageBoxYesNoDlg" TAB "MessageBoxYesNoCancelDlg" TAB "MessagePopupDlg";


//----------------------------------------
function GuiEditorStartCreate()
{
   NewGuiDialogClass.setText("GuiControl");
   NewGuiDialogClass.sort();
   NewGuiDialogName.setValue("NewGui");
   Canvas.pushDialog(NewGuiDialog);
}

//----------------------------------------
function GuiEditorCreate()
{
   %name = NewGuiDialogName.getValue();
   %class = NewGuiDialogClass.getText();
      
   // Make sure we don't clash with an existing object.
   // If there's an existing GUIControl with the name, ask to replace.
   // If there's an existing non-GUIControl with the name, refuse to create.
   
   %existingObject = nameToID( %name );
   if( %existingObject == -1 )
   {
      if( %name $= "RootGroup" )
         %existingObject = RootGroup;
      else
         %existingObject = "";
   }
   %replace = false;
      
   if( %existingObject !$= "" )
   {
      if( %existingObject.isMemberOfClass( "GuiControl" ) )
      {
         %replace = MessageBox( "Warning", "Replace the existing control '" @ %name @ "'?",
            "OkCancel", "Question" ) == $MROk;
      }
      else
         MessageBox( "Error", "An object called '" @ %name @ "' already exists.",
            "Ok", "Stop" );
   }
   
   if( %existingObject !$= "" && %replace )
   {
      %existingObject.delete();
      %existingObject = "";
   }
   
   if( %existingObject $= "" )
   {
      Canvas.popDialog(NewGuiDialog);
      %obj = eval("return new " @ %class @ "(" @ %name @ ");");
      GuiEditContent(%obj);
   }
}

package GuiEditor_BlockDialogs
{

function GuiCanvas::pushDialog()
{

}

function GuiCanvas::popDialog()
{

}

};

function GuiEditor::enableMenuItems(%this, %val)
{
   %menu = GuiEditCanvas.menuBar->EditMenu.getID();
   
   %menu.enableItem( 3, %val ); // cut
   %menu.enableItem( 4, %val ); // copy
   %menu.enableItem( 5, %val ); // paste
   %menu.enableItem( 6, %val ); // selectall
   %menu.enableItem( 7, %val ); // deselectall
   %menu.enableItem( 9, %val ); // lock
   %menu.enableItem( 10, %val ); // unlock
   %menu.enableItem( 12, %val ); // hide
   %menu.enableItem( 13, %val ); // unhide
   %menu.enableItem( 15, %val ); // group
   %menu.enableItem( 16, %val ); // ungroup
   
   GuiEditCanvas.menuBar->LayoutMenu.enableAllItems(%val);
   GuiEditCanvas.menuBar->MoveMenu.enableAllItems(%val);
}

function GuiEditorScanGroupForGuis( %group )
{
   %numObjects = %group.getCount();
   for( %i = 0; %i < %numObjects; %i ++ )
   {
      %obj = %group.getObject( %i );
      if( %obj.isMemberOfClass( "GuiControl" ) )
      {
         if(%obj.getClassName() $= "GuiCanvas")
         {
            GuiEditorScanGroupForGuis( %obj );
         }
         else 
         {
            if(%obj.getName() $= "")
               %name = "(unnamed) - " @ %obj;
            else
               %name = %obj.getName() @ " - " @ %obj;

            %skip = false;
            for(%j=0; %j<getWordCount($Pref::GuiEditor::GuiFilterList); %j++)
            {
               %guiEntry = getWord($Pref::GuiEditor::GuiFilterList, %j);
               if(%obj.getName() $= %guiEntry)
               {
                  %skip = true;
                  break;
               }
            }
      
            if(!%skip)
               GuiEditorContentList.add(%name, %obj);
         }
      }
      else if( %obj.isMemberOfClass( "SimGroup" )
               &&  ( %obj.internalName !$= "EditorGuiGroup"    // Don't put our editor's GUIs in the list
                     || $pref::GuiEditor::showEditorGuis ) )   // except if explicitly requested.
      {
         // Scan nested SimGroups for GuiControls.
         
         GuiEditorScanGroupForGuis( %obj );
      }
   }
}

//----------------------------------------
function GuiEditorOpen( %content )
{   
   Canvas.setContent( GuiEditorGui );
   while( GuiEditorContent.getCount() )
      GuiGroup.add( GuiEditorContent.getObject( 0 ) ); // get rid of anything being edited
      
   // Clear the current guide set and add the guides
   // from the control.
   
   GuiEditor.clearGuides();
   GuiEditor.readGuides( %content );
   
   // enumerate all Guis, and put them in a popup. Skip canvases, and the blank gui. 

   GuiEditorContentList.clear();
   GuiEditorScanGroupForGuis(GuiGroup);
   
   GuiEditorScroll.scrollToTop();
   activatePackage(GuiEditor_BlockDialogs);
   GuiEditorContent.add( %content );
   deactivatePackage(GuiEditor_BlockDialogs);
   GuiEditorContentList.sort();

   GuiEditorResList.clear();
   GuiEditorResList.add("640 x 480", 640);
   GuiEditorResList.add("800 x 600", 800);
   GuiEditorResList.add("1024 x 768", 1024);
   
   %ext = $Pref::GuiEditor::PreviewResolution;
   if( %ext $= "" )
   {
      %ext = GuiEditorRegion.getExtent();
      switch(getWord(%ext, 0))
      {
         case 640:
            GuiEditorResList.setText("640 x 480");
         case 800:
            GuiEditorResList.setText("800 x 600");
         case 1024:
            GuiEditorResList.setText("1024 x 768");
      }
   }
   else
   {
      GuiEditorResList.setText( getWord(%ext,0) @ " x " @ getWord(%ext, 1) );
   }
   
   if(%content.getName() $= "")
      %name = "(unnamed) - " @ %content;
   else
      %name = %content.getName() @ " - " @ %content;
   
   GuiEditorContentList.setText(%name);

   GuiEditor.setContentControl(%content);
   GuiEditorRegion.resize(0,0,getWord(%ext,0), getWord(%ext, 1));
   GuiEditorContent.getObject(0).resize(0,0,getWord(%ext,0), getWord(%ext, 1));

   //%content.resize(0,0,getWord(%ext,0), getWord(%ext, 1));

   GuiEditorTreeView.init();
   GuiEditorTreeView.open(%content);
   
   // clear the undo manager if we're switching controls.
   if(GuiEditor.lastContent != %content)
      GuiEditor.getUndoManager().clearAll();
      
   GuiEditor.setFirstResponder();
   
   GuiEditor.updateUndoMenu();
   GuiEditor.lastContent = %content;
   $pref::GuiEditor::lastContent = %content;
   GuiEditorEdgeSnapping_btn.setStateOn(GuiEditor.snapToEdges);
   GuiEditorCenterSnapping_btn.setStateOn(GuiEditor.snapToCenters);
}

//function GuiEditorMenuBar::onMenuItemSelect(%this, %menuId, %menu, %itemId, %item)
//{
//   if(%this.scriptCommand[%menu, %itemId] !$= "")
//      eval(%this.scriptCommand[%menu, %itemId]);
//   else
//      error("No script command defined for menu " @ %menu  @ " item " @ %item);
//}

//----------------------------------------
function GuiEditorContentList::onSelect(%this, %id)
{
   GuiEditorOpen(%id);
}

//----------------------------------------
function GuiEditorResList::onSelect(%this, %id)
{
   switch(%id)
   {
      case 640:
         GuiEditorRegion.resize(0,0,640,480);
         GuiEditorContent.getObject(0).resize(0,0,640,480);
         $Pref::GuiEditor::PreviewResolution = "640 480";
      case 800:
         GuiEditorRegion.resize(0,0,800,600);
         GuiEditorContent.getObject(0).resize(0,0,800,600);
         $Pref::GuiEditor::PreviewResolution = "800 600";
      case 1024:
         GuiEditorRegion.resize(0,0,1024,768);
         GuiEditorContent.getObject(0).resize(0,0,1024,768);
         $Pref::GuiEditor::PreviewResolution = "1024 768";
   }
}

//----------------------------------------

function GuiEditorTreeContextPopup::onSelect(%this, %id, %text)
{
   if( %id == 0 )
   {
      // Add new objects here
      GuiEditor.setCurrentAddSet( %this.groupId );
   }
   else if( %id == 1 )
   {
      // Selected
      %group = %this.groupId;
      %count = %group.getCount();
      for( %i=0; %i<%count; %i++)
      {
         %obj = %group.getObject(%i);
         GuiEditor.addSelection(%obj);
      }
   }
   else if( %id == 2 )
   {
      // Deselect
      %group = %this.groupId;
      %count = %group.getCount();
      for( %i=0; %i<%count; %i++)
      {
         %obj = %group.getObject(%i);
         GuiEditor.removeSelection(%obj);
      }
   }
}

//----------------------------------------

function GuiEditorTreeView::init(%this)
{
   if( !%this.contextMenu )
   {
      // SimGroup context menu
      %this.contextMenu = new GuiControl( GuiEditorTreeContextPopupDlg )
      {
         profile = "GuiPopupBackgroundProfile";
         horizSizing = "width";
         vertSizing = "height";
         position = "0 0";
         extent = "640 480";
         minExtent = "8 8";
         visible = "1";
         setFirstResponder = "0";
         command = "canvas.popDialog(GuiEditorTreeContextPopupDlg);";

         new GuiTextListCtrl( GuiEditorTreeContextPopup )
         {
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
            command = "canvas.popDialog(GuiEditorTreeContextPopupDlg);";
            enumerate = "0";
            resizeCell = "1";
            columns = "0";
            fitParentWidth = "0";
            clipColumnText = "0";
         };
      };
      
      GuiEditorTreeContextPopup.addRow( 0, "Add New Controls Here" );
      GuiEditorTreeContextPopup.addRow( 1, "Add Chil Controls to Selection" );
      GuiEditorTreeContextPopup.addRow( 2, "Remove Child Controls from Selection" );
      GuiEditorTreeContextPopup.groupId = -1;
      GuiEditorTreeContextPopup.setVisible( false );
   }
}

// defines the icons to be used in the tree view control
// provide the paths to each icon minus the file extension
// seperate them with : 
// the order of the icons must correspond to the bit array defined
// in the GuiTreeViewCtrl.h
function GuiEditorTreeView::onDefineIcons(%this)
{
   %icons = ":" @       // Default1
            ":" @       // SimGroup1
            ":" @       // SimGroup2
            ":" @       // SimGroup3
            ":" @       // SimGroup4
            "core/art/gui/images/treeview/hidden:" @
            "tools/worldEditor/images/lockedHandle";

   GuiEditorTreeView.buildIconTable( %icons );
}

function GuiEditorTreeView::update( %this )
{
   %obj = GuiEditorContent.getObject( 0 );
   
   if( !isObject( %obj ) )
      GuiEditorTreeView.clear();
   else
   {
      // Open inspector tree.
      
      GuiEditorTreeView.open( %obj );
      
      // Sync selection with GuiEditor.
      
      GuiEditorTreeView.clearSelection();
      
      %selection = GuiEditor.getSelected();
      %count = %selection.getCount();
      
      for( %i = 0; %i < %count; %i ++ )
         GuiEditorTreeView.addSelection( %selection.getObject( %i ) );
   }
}

function GuiEditorTreeView::onRightMouseDown(%this, %item, %pts, %obj)
{
   if( %obj )
   {
      canvas.pushDialog(GuiEditorTreeContextPopupDlg);
      GuiEditorTreeContextPopup.clearSelection();
      GuiEditorTreeContextPopup.position = %pts;
      GuiEditorTreeContextPopup.groupId = %obj;
      
      if( (getWord(GuiEditorTreeContextPopup.position, 0) + getWord(GuiEditorTreeContextPopup.extent, 0)) > getWord(GuiEditorGui.extent, 0) )
      {
         %posx = getWord(GuiEditorTreeContextPopup.position, 0);
         %offset = getWord(GuiEditorGui.extent, 0) - (%posx + getWord(GuiEditorTreeContextPopup.extent, 0)) - 5;
         %posx += %offset;
         GuiEditorTreeContextPopup.position = %posx @ " " @ getWord(GuiEditorTreeContextPopup.position, 1);
      }
      
      GuiEditorTreeContextPopup.setVisible(true);
   }
}

function GuiEditorTreeView::onAddSelection(%this,%ctrl)
{   
   GuiEditor.addSelection( %ctrl );  
   GuiEditor.setFirstResponder();
}

function GuiEditorTreeView::onRemoveSelection( %this, %ctrl )
{
   GuiEditor.removeSelection( %ctrl );
}

function GuiEditor::onClearSelected( %this )
{ 
   GuiEditorTreeView.clearSelection();
   GuiEditorInspectFields.update( 0 );
}

function GuiEditor::onRemoveSelected(%this,%ctrl)
{
   GuiEditorTreeView.removeSelection(%ctrl); 
}

function GuiEditor::onDelete(%this)
{
   GuiEditorTreeView.update();
   // clear out the gui inspector.
   GuiEditorInspectFields.update(0);
}

function GuiEditorTreeView::onDeleteSelection(%this)
{ 
   GuiEditor.clearSelection();
}

function GuiEditorTreeView::onSelect( %this, %obj )
{
   if( isObject( %obj ) )
   {
      GuiEditor.select( %obj );
      GuiEditorInspectFields.update( %obj );      
   }
}

//----------------------------------------
function GuiEditorInspectFields::update(%this, %inspectTarget)
{
   GuiEditorInspectFields.inspect( %inspectTarget );
}

function GuiEditorInspectFields::onInspectorFieldModified( %this, %object, %fieldName, %arrayIndex, %oldValue, %newValue )
{
   // The instant group will try to add our
   // UndoAction if we don't disable it.   
   pushInstantGroup();

   %nameOrClass = %object.getName();
   if ( %nameOrClass $= "" )
      %nameOrClass = %object.getClassname();
      
   %action = new InspectorFieldUndoAction()
   {
      actionName = %nameOrClass @ "." @ %fieldName @ " Change";
      
      objectId = %object.getId();
      fieldName = %fieldName;
      fieldValue = %oldValue;
      arrayIndex = %arrayIndex;
                  
      inspectorGui = %this;
   };
   
   // Restore the instant group.
   popInstantGroup();
         
   %action.addToManager( GuiEditor.getUndoManager() );
   
   GuiEditor.updateUndoMenu();
}

function GuiEditorInspectFields::onFieldSelected( %this, %fieldName, %fieldTypeStr, %fieldDoc )
{
   GuiEditorFieldInfo.setText( "<font:ArialBold:14>" @ %fieldName @ "<font:ArialItalic:14> (" @ %fieldTypeStr @ ") " @ "<font:Arial:14>" @ %fieldDoc );
}

//----------------------------------------
function GuiEditor::onSelect(%this, %ctrl)
{   
   GuiEditorTreeView.clearSelection();
   GuiEditorTreeView.addSelection( %ctrl );
   GuiEditorInspectFields.update( %ctrl );
}

function GuiEditor::onAddSelected( %this, %ctrl )
{
   GuiEditorTreeView.addSelection( %ctrl );
   GuiEditorTreeView.scrollVisibleByObjectId( %ctrl );
   
   // In multi-selection mode, don't show anything in the inspector for now.
   // Would be nice if we had same multi-object editing capability in the
   // inspector.
   
   GuiEditorInspectFields.update( 0 );
}

function GuiEditorSnapCheckBox::onWake(%this)
{
   %snap = $pref::guiEditor::snap2grid * $pref::guiEditor::snap2gridsize;
   %this.setValue(%snap);
   GuiEditor.setSnapToGrid(%snap);
   //error("snap = " @ %snap);   
}
function GuiEditorSnapCheckBox::onAction(%this)
{
   %snap = $pref::guiEditor::snap2gridsize * %this.getValue();
   $pref::guiEditor::snap2grid = %this.getValue();
   //error("snap = " @ %snap);
   GuiEditor.setSnapToGrid(%snap);
}

function GuiEditor::showPrefsDialog(%this)
{
   Canvas.pushDialog(GuiEditorPrefsDlg);
}

function GuiEditor::togglePalette(%this, %show)
{
   %vis = GuiEditorGui-->togglePaletteBtn.getValue();
   if(%show !$= "")
      %vis = %show;
   
   if(%vis)
      GuiEditorGui.add(GuiEditorPalette);
   else  
      GuiEditorPaletteGui.add(GuiEditorPalette);
      
   GuiEditorGui-->togglePaletteBtn.setValue(%vis);
}

function GuiEditor::onControlDragged(%this, %payload, %position)
{
   // use the position under the mouse cursor, not the payload position.
   %position = VectorSub(%position, GuiEditorContent.getGlobalPosition());
   %x = getWord(%position, 0);
   %y = getWord(%position, 1);
   %target = GuiEditorContent.findHitControl(%x, %y);
   
   while(! %target.isContainer )
      %target = %target.getParent();
   //echo(%target SPC %target.getName());
   if( %target != %this.getCurrentAddSet())
   %this.setCurrentAddSet(%target);
}

function GuiEditor::onControlDropped(%this, %payload, %position)
{  
   %pos = %payload.getGlobalPosition();
   %x = getWord(%pos, 0);
   %y = getWord(%pos, 1);

   %this.addNewCtrl(%payload);
   
   %payload.setPositionGlobal(%x, %y);
   %this.setFirstResponder();
}

function GuiEditor::onGainFirstResponder(%this)
{
   %this.enableMenuItems(true);
   
   // JCF: don't just turn them all on!
   // Undo/Redo is only enabled if those actions exist.
   %this.updateUndoMenu();
}

function GuiEditor::onLoseFirstResponder(%this)
{
   %this.enableMenuItems(false);
}

function GuiEditor::undo(%this)
{
   %this.getUndoManager().undo();
   %this.updateUndoMenu();
   %this.clearSelection();
}

function GuiEditor::redo(%this)
{
   %this.getUndoManager().redo();
   %this.updateUndoMenu();
   %this.clearSelection();
}

function GuiEditor::updateUndoMenu(%this)
{
   %uman = %this.getUndoManager();
   %nextUndo = %uman.getNextUndoName();
   %nextRedo = %uman.getNextRedoName();
   
   %editMenu = GuiEditCanvas.menuBar->editMenu;
   
   %editMenu.setItemName( 0, "Undo " @ %nextUndo );
   %editMenu.setItemName( 1, "Redo " @ %nextRedo );
   
   %editMenu.enableItem( 0, %nextUndo !$= "" );
   %editMenu.enableItem( 1, %nextRedo !$= "" );
}

function GuiEditor::onHierarchyChanged( %this )
{
   GuiEditorTreeView.update();
}

//------------------------------------------------------------------------------
// Grouping/ungrouping.

/// Group all GuiControls in the currenct selection set under a new GuiControl.
function GuiEditor::groupSelected( %this )
{
   %selection = %this.getSelected();
   if( %selection.getCount() < 2 )
      return;
         
   // Create action.
   
   %action = GuiEditorGroupAction::create( %selection, GuiEditor.getContentControl() );   
   %action.groupControls();

   // Update editor tree.
   
   GuiEditor.clearSelection();
   GuiEditor.addSelection( %action.group[ 0 ].groupObject );
   GuiEditorTreeView.update();
   
   // Update undo state.

   %action.addtoManager( %this.getUndoManager() );
   %this.updateUndoMenu();
}

/// Take all direct GuiControl instances in the selection set and reparent their child controls
/// to each of the group's parents.  The GuiControl group objects are deleted.
function GuiEditor::ungroupSelected( %this )
{
   %action = GuiEditorUngroupAction::create( %this.getSelected() );
   %action.ungroupControls();
   
   // Update editor tree.
   
   GuiEditor.clearSelection();
   GuiEditorTreeView.update();
   
   // Update undo state.
   
   %action.addToManager( %this.getUndoManager() );
   %this.updateUndoMenu();
}

//------------------------------------------------------------------------------
// Editor option toggles.

function GuiEditor::toggleEdgeSnap( %this )
{
   %this.snapToEdges = !%this.snapToEdges;
   $pref::GuiEditor::snapToEdges = %this.snapToEdges;
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_EDGESNAP_INDEX, %this.snapToEdges );
   GuiEditorEdgeSnapping_btn.setStateOn( %this.snapToEdges );
}

function GuiEditor::toggleCenterSnap( %this )
{
   %this.snapToCenters = !%this.snapToCenters;
   $pref::GuiEditor::snapToCenters = %this.snapToCenters;
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_CENTERSNAP_INDEX, %this.snapToCenters );
   GuiEditorCenterSnapping_btn.setStateOn( %this.snapToCenters );
}

function GuiEditor::toggleFullBoxSelection( %this )
{
   %this.fullBoxSelection = !%this.fullBoxSelection;
   $pref::GuiEditor::fullBoxSelection = %this.fullBoxSelection;
   GuiEditCanvas.menuBar->EditMenu.checkItem( $GUI_EDITOR_MENU_FULLBOXSELECT_INDEX, %this.fullBoxSelection );
}

function GuiEditor::toggleDrawGuides( %this )
{
   %this.drawGuides= !%this.drawGuides;
   $pref::GuiEditor::drawGuides = %this.drawGuides;
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_DRAWGUIDES_INDEX, %this.drawGuides );
}

function GuiEditor::toggleGuideSnap( %this )
{
   %this.snapToGuides = !%this.snapToGuides;
   $pref::GuiEditor::snapToGuides = %this.snapToGuides;
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_GUIDESNAP_INDEX, %this.snapToGuides );
}

function GuiEditor::toggleControlSnap( %this )
{
   %this.snapToControls = !%this.snapToControls;
   $pref::GuiEditor::snapToControls = %this.snapToControls;
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_CONTROLSNAP_INDEX, %this.snapToControls );
}

//------------------------------------------------------------------------------
// Gui Editor Menu activation
function GuiEditorGui::onWake( %this )
{
   GHGuiEditor.setStateOn( 1 );
      
   // Attach our menus.
   
   if( isObject( %this.menuGroup ) )
      for( %i = 0; %i < %this.menuGroup.getCount(); %i ++ )
         %this.menuGroup.getObject( %i ).attachToMenuBar();
         
   // Read our preferences.
     
   if( isDefined( "$pref::GuiEditor::snapToControls" ) )
      GuiEditor.snapToControls = $pref::GuiEditor::snapToControls;
   if( isDefined( "$pref::GuiEditor::snapToGuides" ) )
      GuiEditor.snapToGuides = $pref::GuiEditor::snapToGuides;
   if( isDefined( "$pref::GuiEditor::snapToEdges" ) )
      GuiEditor.snapToEdges = $pref::GuiEditor::snapToEdges;
   if( isDefined( "$pref::GuiEditor::snapToCenters" ) )
      GuiEditor.snapToCenters = $pref::GuiEditor::snapToCenters;
   if( isDefined( "$pref::GuiEditor::snapSensitiviy" ) )
      GuiEditor.snapSensitivity = $pref::GuiEditor::snapSensitivity;
   if( isDefined( "$pref::GuiEditor::fullBoxSelection" ) )
      GuiEditor.fullBoxSelection = $pref::GuiEditor::fullBoxSelection;
   if( isDefined( "$pref::GuiEditor::drawBorderLines" ) )
      GuiEditor.drawBorderLines = $pref::GuiEditor::drawBorderLines;
   if( isDefined( "$pref::GuiEditor::drawGuides" ) )
      GuiEditor.drawGuides = $pref::GuiEditor::drawGuides;
      
   //RDTODO: would be nice to have these point directly to the GUI Editor docs
   
   if( !isDefined( "$pref::GuiEditor::documentationURL" ) )
      $pref::GuiEditor::documentationURL = "http://docs.garagegames.com/t3d/official/";
   if( !isDefined( "$pref::GuiEditor::documentationLocal" ) )
      $pref::GuiEditor::documentationLocal = "../../../Documentation/Official Documentation.html"; //RDTODO: determine proper install path for release

   // Set up initial menu toggle states.
   
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_EDGESNAP_INDEX, GuiEditor.snapToEdges );
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_CENTERSNAP_INDEX, GuiEditor.snapToCenters );
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_GUIDESNAP_INDEX, GuiEditor.snapToGuides );
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_CONTROLSNAP_INDEX, GuiEditor.snapToControls );
   GuiEditCanvas.menuBar->SnapMenu.checkItem( $GUI_EDITOR_MENU_DRAWGUIDES_INDEX, GuiEditor.drawGuides );
   GuiEditCanvas.menuBar->EditMenu.checkItem( $GUI_EDITOR_MENU_FULLBOXSELECT_INDEX, GuiEditor.fullBoxSelection );
}

function GuiEditorGui::onSleep( %this)
{
   // If we are editing a control, store its guide state.
   
   %content = GuiEditor.getContentControl();
   if( isObject( %content ) )
      GuiEditor.writeGuides( %content );

   // Remove our menus.
   
   if( isObject( %this.menuGroup ) )
      for( %i = 0; %i < %this.menuGroup.getCount(); %i ++ )
         %this.menuGroup.getObject( %i ).removeFromMenuBar();
         
   // Store our preferences.
      
   $pref::GuiEditor::snapToGuides = GuiEditor.snapToGuides;
   $pref::GuiEditor::snapToEdges = GuiEditor.snapToEdges;
   $pref::GuiEditor::snapToCenters = GuiEditor.snapToCenters;
   $pref::GuiEditor::snapToControls = GuiEditor.snapToControls;
   $pref::GuiEditor::snapSensitivity = GuiEditor.snapSensitivity;
   $pref::GuiEditor::fullBoxSelection = GuiEditor.fullBoxSelection;
   $pref::GuiEditor::drawBorderLines = GuiEditor.drawBorderLines;
   $pref::GuiEditor::drawGuides = GuiEditor.drawGuides;
}

function GuiEditor::switchToWorldEditor( %this )
{
   %editingWorldEditor = false;
   if( GuiEditorContent.getObject( 0 ) == EditorGui.getId() )
      %editingWorldEditor = true;
      
   GuiEdit();
   
   if( !$missionRunning )
      EditorNewLevel();
   else if( !%editingWorldEditor )
      toggleEditor( true );
}

function toggleGuiEditor( %make )
{
   if( %make )
   {
      if( EditorIsActive() && !$pref::GuiEditor::toggleIntoEditorGui )
         toggleEditor( true );
         
      GuiEdit();
   }
}

GlobalActionMap.bind( keyboard, "f10", toggleGuiEditor );
