//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

exec("./fileDialogBase.ed.cs");
exec("./openFileDialog.ed.cs");
exec("./saveFileDialog.ed.cs");
exec("./saveChangesMBDlg.ed.gui");
exec("./simViewDlg.ed.gui");
exec("./colorPicker.ed.gui");
exec("./materialSelector.ed.gui");
exec("./scriptEditorDlg.ed.gui");
exec("./textDocumentController.ed.cs");
exec("./textDocumentEditor.ed.cs");
exec("./colladaImport.ed.gui");
exec("./EditorLoadingGui.gui");

if (isDemo())
   exec("./messageBoxOKBuy.ed.gui");
