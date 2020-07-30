//-----------------------------------------------------------------------------
// Torque Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

$InGuiEditor = false;

function GuiEdit(%val)
{

   if (Canvas.isFullscreen())
   {
      MessageBoxOKOld("Windowed Mode Required", "Please switch to windowed mode to access the GUI Editor.");
      return;
   }

   if(%val != 0)
      return;

   if (!$InGuiEditor)
   {
      GuiEditContent(Canvas.getContent());
   }
   else
   {
      GuiEditCanvas.quit();
   }

}

function GuiEditContent(%content)
{
   if( !isObject( GuiEditCanvas ) )
      new GuiControl( GuiEditCanvas, EditorGuiGroup );

   GuiEditorOpen(%content);
   
   $InGuiEditor = true;
}

function GuiEditCanvas::onAdd( %this )
{
   // %this.setWindowTitle("Torque Gui Editor");

   %this.onCreateMenu();
}

function GuiEditCanvas::onRemove( %this )
{
   if( isObject( GuiEditorGui.menuGroup ) )
      GuiEditorGui.delete();

   // cleanup
   %this.onDestroyMenu();
}

function GuiEditCanvas::onCreateMenu(%this)
{
   
   if(isObject(%this.menuBar))
      return;
   
   //set up %cmdctrl variable so that it matches OS standards
   if( $platform $= "macos" )
   {
      %cmdCtrl = "cmd";
      %redoShortcut = "Cmd-Shift Z";
   }
   else
   {
      %cmdCtrl = "Ctrl";
      %redoShort = "Ctrl Y";
   }
   
   // Menu bar
   %this.menuBar = new MenuBar()
   {
      dynamicItemInsertPos = 3;
      
      new PopupMenu()
      {
         superClass = "MenuBuilder";
         barTitle = "File";
         internalName = "FileMenu";
         
         item[0] = "New Gui..." TAB %cmdCtrl SPC "N" TAB %this @ ".create();";
         item[1] = "Open From File..." TAB %cmdCtrl SPC "O" TAB %this @ ".open();";
         item[2] = "Save To File..." TAB %cmdCtrl SPC "S" TAB %this @ ".save( false );";
         item[3] = "Save Selected To File..." TAB %cmdCtrl @ "-Shift S" TAB %this @ ".save( true );";
         item[4] = "-";
         item[5] = "Revert Gui" TAB "" TAB %this @ ".revert();";
         item[6] = "Add Gui From File..." TAB "" TAB %this @ ".append();";
         //item[7] = "Close Gui" TAB %cmdCtrl SPC "W" TAB %this @ ".close();";
         item[7] = "-";
         item[8] = "Close Editor" TAB "F10" TAB %this @ ".quit();";
         item[9] = "Quit" TAB %cmdCtrl SPC "Q" TAB "quit();";
      };

      new PopupMenu()
      {
         superClass = "MenuBuilder";
         barTitle = "Edit";
         internalName = "EditMenu";
         
         item[0] = "Undo" TAB %cmdCtrl SPC "Z" TAB "GuiEditor.undo();";
         item[1] = "Redo" TAB %redoShortcut TAB "GuiEditor.redo();";
         item[2] = "-";
         item[3] = "Cut" TAB %cmdCtrl SPC "X" TAB "GuiEditor.saveSelection($Gui::clipboardFile); GuiEditor.deleteSelection();";
         item[4] = "Copy" TAB %cmdCtrl SPC "C" TAB "GuiEditor.saveSelection($Gui::clipboardFile);";
         item[5] = "Paste" TAB %cmdCtrl SPC "V" TAB "GuiEditor.loadSelection($Gui::clipboardFile);";
         item[6] = "Select All" TAB %cmdCtrl SPC "A" TAB "GuiEditor.selectAll();";
         item[7] = "Deselect All" TAB %cmdCtrl SPC "D" TAB "GuiEditor.clearSelection();";
         item[8] = "-";
         item[9] = "Lock Selection" TAB %cmdCtrl SPC "L" TAB "GuiEditorTreeView.lockSelection(true);";
         item[10] = "Unlock Selection" TAB %cmdCtrl @ "-Shift L" TAB "GuiEditorTreeView.lockSelection(false);";
         item[11] = "-";
         item[12] = "Hide Selection" TAB %cmdCtrl SPC "H" TAB "GuiEditorTreeView.hideSelection(true);";
         item[13] = "Unhide Selection" TAB %cmdCtrl @ "-Shift H" TAB "GuiEditorTreeView.hideSelection(false);";
         item[14] = "-";
         item[15] = "Group Selection" TAB %cmdCtrl SPC "G" TAB "GuiEditor.groupSelected();";
         item[16] = "Ungroup Selection" TAB %cmdCtrl @ "-Shift G" TAB "GuiEditor.ungroupSelected();";
         item[17] = "-";
         item[18] = "Full Box Selection" TAB "" TAB "GuiEditor.toggleFullBoxSelection();";
         item[19] = "-";
         item[20] = "Grid Size" TAB %cmdCtrl SPC "," TAB "GuiEditor.showPrefsDialog();";
      };
      
      new PopupMenu()
      {
         superClass = "MenuBuilder";
         barTitle = "Layout";
         internalName = "LayoutMenu";
         
         item[0] = "Align Left" TAB %cmdCtrl SPC "Left" TAB "GuiEditor.Justify(0);";
         item[1] = "Center Horizontally" TAB "" TAB "GuiEditor.Justify(1);";
         item[2] = "Align Right" TAB %cmdCtrl SPC "Right" TAB "GuiEditor.Justify(2);";
         item[3] = "-";
         item[4] = "Align Top" TAB %cmdCtrl SPC "Up" TAB "GuiEditor.Justify(3);";
         item[5] = "Center Vertically" TAB "" TAB "GuiEditor.Justify(7);";
         item[6] = "Align Bottom" TAB %cmdCtrl SPC "Down" TAB "GuiEditor.Justify(4);";
         item[7] = "-";
         item[8] = "Space Vertically" TAB "" TAB "GuiEditor.Justify(5);";
         item[9] = "Space Horizontally" TAB "" TAB "GuiEditor.Justify(6);";
         item[10] = "-";
         item[11] = "Bring to Front" TAB "" TAB "GuiEditor.BringToFront();";
         item[12] = "Send to Back" TAB "" TAB "GuiEditor.PushToBack();";
      };
      
      new PopupMenu()
      {
         superClass = "MenuBuilder";
         barTitle = "Move";
         internalName = "MoveMenu";
            
         item[0] = "Nudge Left" TAB "Left" TAB "GuiEditor.moveSelection( -1, 0);";
         item[1] = "Nudge Right" TAB "Right" TAB "GuiEditor.moveSelection( 1, 0);";
         item[2] = "Nudge Up" TAB "Up" TAB "GuiEditor.moveSelection( 0, -1);";
         item[3] = "Nudge Down" TAB "Down" TAB "GuiEditor.moveSelection( 0, 1 );";
         item[4] = "-";
         item[5] = "Big Nudge Left" TAB "Shift Left" TAB "GuiEditor.moveSelection( - $pref::guiEditor::snap2gridsize, 0 );";
         item[6] = "Big Nudge Right" TAB "Shift Right" TAB "GuiEditor.moveSelection( $pref::guiEditor::snap2gridsize, 0 );";
         item[7] = "Big Nudge Up" TAB "Shift Up" TAB "GuiEditor.moveSelection( 0, - $pref::guiEditor::snap2gridsize );";
         item[8] = "Big Nudge Down" TAB "Shift Down" TAB "GuiEditor.moveSelection( 0, $pref::guiEditor::snap2gridsize );";
      };

      new PopupMenu()
      {
         superClass = "MenuBuilder";
         barTitle = "Snap";
         internalName = "SnapMenu";

         item[0] = "Snap Edges" TAB "Alt-Shift E" TAB "GuiEditor.toggleEdgeSnap();";
         item[1] = "Snap Centers" TAB "Alt-Shift C" TAB "GuiEditor.toggleCenterSnap();";
         item[2] = "-";
         item[3] = "Snap to Guides" TAB "Alt-Shift G" TAB "GuiEditor.toggleGuideSnap();";
         item[4] = "Snap to Controls" TAB "Alt-Shift T" TAB "GuiEditor.toggleControlSnap();";
         item[5] = "-";
         item[6] = "Show Guides" TAB "" TAB "GuiEditor.toggleDrawGuides();";
         item[7] = "Clear Guides" TAB "" TAB "GuiEditor.clearGuides();";
      };

      new PopupMenu()
      {
         superClass = "MenuBuilder";
         internalName = "HelpMenu";

         barTitle = "Help";

         item[0] = "Online Documentation..." TAB "Alt F1" TAB "gotoWebPage( EditorSettings.value(\"documentationURL\") );";
         item[1] = "Offline Documentation..." TAB "" TAB "gotoWebPage( EditorSettings.value(\"documentationLocal\") );";
         item[2] = "Torque 3D Forums..." TAB "" TAB "gotoWebPage( EditorSettings.value(\"forumURL\") );";
      };
   };
   %this.menuBar.attachToCanvas( Canvas, 0 );
}

$GUI_EDITOR_MENU_EDGESNAP_INDEX = 0;
$GUI_EDITOR_MENU_CENTERSNAP_INDEX = 1;
$GUI_EDITOR_MENU_GUIDESNAP_INDEX = 3;
$GUI_EDITOR_MENU_CONTROLSNAP_INDEX = 4;
$GUI_EDITOR_MENU_DRAWGUIDES_INDEX = 6;
$GUI_EDITOR_MENU_FULLBOXSELECT_INDEX = 18;

// Called before onSleep when the canvas content is changed
function GuiEditCanvas::onDestroyMenu(%this)
{
   if( !isObject( %this.menuBar ) )
      return;

   // Destroy menus      
   while( %this.menuBar.getCount() != 0 )
      %this.menuBar.getObject( 0 ).delete();
   
   %this.menuBar.removeFromCanvas();
   %this.menuBar.delete();
}

//
// Menu Operations
//
function GuiEditCanvas::create( %this )
{
   GuiEditorStartCreate();
}

function GuiEditCanvas::load( %this, %filename )
{
   %newRedefineBehavior = "replaceExisting";
   if( isDefined( "$GuiEditor::loadRedefineBehavior" ) )
   {
      // This trick allows to choose different redefineBehaviors when loading
      // GUIs.  This is use, for example, when loading GUIs that would lead to
      // problems when loading with their correct names because script behavior
      // would immediately attach.
      //
      // This allows to also edit the GUI editor's own GUI inside itself.
      
      %newRedefineBehavior = $GuiEditor::loadRedefineBehavior;
   }
   
   // Allow stomping objects while exec'ing the GUI file as we want to
   // pull the file's objects even if we have another version of the GUI
   // already loaded.
   
   %oldRedefineBehavior = $Con::redefineBehavior;
   $Con::redefineBehavior = %newRedefineBehavior;
   
   // Load up the gui.
   exec( %fileName );
   
   $Con::redefineBehavior = %oldRedefineBehavior;
   
   // The GUI file should have contained a GUIControl which should now be in the instant
   // group. And, it should be the only thing in the group.
   if( !isObject( %guiContent ) )
   {
      MessageBox( getEngineName(),
         "You have loaded a Gui file that was created before this version.  It has been loaded but you must open it manually from the content list dropdown",
         "Ok", "Information" );   
      return 0;
   }

   GuiEditorOpen( %guiContent );      
}

function GuiEditCanvas::open( %this )
{
   %openFileName = GuiBuilder::getOpenName();
   if( %openFileName $= "" )
      return;

   // Make sure the file is valid.
   if ((!isFile(%openFileName)) && (!isFile(%openFileName @ ".dso")))
      return;

   %this.load( %openFileName );
}

function GuiEditCanvas::save( %this, %selectedOnly )
{
   // Get the control we should save.
   
   if( %selectedOnly )
   {
      %selected = GuiEditor.getSelected();
      if( !%selected.getCount() )
         return;
      else if( %selected.getCount() > 1 )
      {
         MessageBox( "Invalid selection", "Only a single control hierarchy can be saved to a file.  Make sure you have selected only one control in the tree view." );
         return;
      }
         
      %currentObject = %selected.getObject( 0 );
   }
   else if( GuiEditorContent.getCount() > 0 )
      %currentObject = GuiEditorContent.getObject( 0 );
   else
      return;
      
   // Store the current guide set on the control.
   
   GuiEditor.writeGuides( %currentObject );
   %currentObject.canSaveDynamicFields = true; // Make sure the guides get saved out.
   
   // Construct a base filename.
   
   if( %currentObject.getName() !$= "" )
      %name =  %currentObject.getName() @ ".gui";
   else
      %name = "Untitled.gui";
      
   // Construct a path.
   
   if( %selectedOnly
       && %currentObject != GuiEditorContent.getObject( 0 )
       && %currentObject.getScriptFile() $= GuiEditorContent.getObject( 0 ).getScriptFile() )
   {
      // Selected child control that hasn't been yet saved to its own file.
      
      %currentFile = %Pref::GuiEditor::LastPath @ "/" @ %name;
      %currentFile = makeRelativePath( %currentFile, getMainDotCsDir() );
   }
   else
   {
      %currentFile = %currentObject.getScriptFile();
      if( %currentFile $= "")
      {
         if( $Pref::GuiEditor::LastPath !$= "" )
         {
            %currentFile = $Pref::GuiEditor::LastPath @ "/" @ %name;
            %currentFile = makeRelativePath( %currentFile, getMainDotCsDir() );
         }
         else
            %currentFile = expandFileName( %name );
      }
      else
         %currentFile = expandFileName(%currentFile);
   }
   
   // get the filename
   %filename = GuiBuilder::getSaveName(%currentFile);
   
   if(%filename $= "")
      return;
      
   // Save the Gui
   if( isWriteableFileName( %filename ) )
   {
      //
      // Extract any existent TorqueScript before writing out to disk
      //
      %fileObject = new FileObject();
      %fileObject.openForRead( %filename );      
      %skipLines = true;
      %beforeObject = true;
      // %var++ does not post-increment %var, in torquescript, it pre-increments it,
      // because ++%var is illegal. 
      %lines = -1;
      %beforeLines = -1;
      %skipLines = false;
      while( !%fileObject.isEOF() )
      {
         %line = %fileObject.readLine();
         if( %line $= "//--- OBJECT WRITE BEGIN ---" )
            %skipLines = true;
         else if( %line $= "//--- OBJECT WRITE END ---" )
         {
            %skipLines = false;
            %beforeObject = false;
         }
         else if( %skipLines == false )
         {
            if(%beforeObject)
               %beforeNewFileLines[ %beforeLines++ ] = %line;
            else
               %newFileLines[ %lines++ ] = %line;
         }
      }      
      %fileObject.close();
      %fileObject.delete();
     
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      
      // Write out the captured TorqueScript that was before the object before the object
      for( %i = 0; %i <= %beforeLines; %i++)
         %fo.writeLine( %beforeNewFileLines[ %i ] );
         
      %fo.writeLine("//--- OBJECT WRITE BEGIN ---");
      %fo.writeObject(%currentObject, "%guiContent = ");
      %fo.writeLine("//--- OBJECT WRITE END ---");
      
      // Write out captured TorqueScript below Gui object
      for( %i = 0; %i <= %lines; %i++ )
         %fo.writeLine( %newFileLines[ %i ] );
               
      %fo.close();
      %fo.delete();
      
      %currentObject.setScriptFile( makeRelativePath( %filename, getMainDotCsDir() ) );
   }
   else
      MessageBox( "Error writing to file", "There was an error writing to file '" @ %currentFile @ "'. The file may be read-only.", "Ok", "Error" );   
}

function GuiEditCanvas::append( %this )
{
   // Get filename.
   
   %openFileName = GuiBuilder::getOpenName();
   if( %openFileName $= ""
       || ( !isFile( %openFileName )
            && !isFile( %openFileName @ ".dso" ) ) )
      return;
   
   // Exec file.

   %oldRedefineBehavior = $Con::redefineBehavior;
   $Con::redefineBehavior = "renameNew";
   exec( %openFileName );
   $Con::redefineBehavior = %oldRedefineBehavior;
   
   // Find guiContent.
   
   if( !isObject( %guiContent ) )
   {
      MessageBox( "Error loading GUI file", "The GUI content controls could not be found.  This function can only be used with files saved by the GUI editor.", "Ok", "Error" );
      return;
   }
   
   if( !GuiEditorContent.getCount() )
      GuiEditorOpen( %guiContent );
   else
   {
      GuiEditor.getCurrentAddSet().add( %guiContent );
      GuiEditor.readGuides( %guiContent );
      GuiEditor.onAddNewCtrl( %guiContent );
      GuiEditor.onHierarchyChanged();
   }
}

function GuiEditCanvas::revert( %this )
{
   if( !GuiEditorContent.getCount() )
      return;
      
   %gui = GuiEditorContent.getObject( 0 );
   %filename = %gui.getScriptFile();
   if( %filename $= "" )
      return;
      
   if( MessageBox( "Revert Gui", "Really revert the current Gui?  This cannot be undone.", "OkCancel", "Question" ) == $MROk )
      %this.load( %filename );
}

function GuiEditCanvas::close( %this )
{
}

function GuiEditCanvas::onWindowClose(%this)
{
   %this.quit();
}

function GuiEditCanvas::quit( %this )
{
   %this.close();
   GuiGroup.add(GuiEditorGui);
   // we must not delete a window while in its event handler, or we foul the event dispatch mechanism
   %this.schedule(10, delete);
   
   Canvas.setContent(GuiEditor.lastContent);
   $InGuiEditor = false;
}
