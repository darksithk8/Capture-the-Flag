//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function initializeGuiEditor()
{
   echo( " % - Initializing Gui Builder" );
   
   exec( "./gui/guiEditor.ed.gui" );
   exec( "./gui/guiEditor.ed.cs" );
   exec( "./gui/guiEditorGroup.ed.cs" );
   exec( "./gui/newGuiDialog.ed.gui" );
   exec( "./gui/fileDialogs.ed.cs" );
   exec( "./gui/guiEditorPrefsDlg.ed.gui" );
   exec( "./gui/guiEditorPrefsDlg.ed.cs" );
   exec( "./gui/guiEditorPalette.ed.gui" );
   exec( "./gui/guiEditorPalette.ed.cs" );
   exec( "./gui/guiEditorUndo.ed.cs" );
   exec( "./scripts/guiEditorCanvas.ed.cs" );
   exec( "./gui/EditorChooseGUI.ed.gui" );
   exec( "./scripts/EditorChooseGUI.ed.cs" );
}

function destroyGuiEditor()
{
}
