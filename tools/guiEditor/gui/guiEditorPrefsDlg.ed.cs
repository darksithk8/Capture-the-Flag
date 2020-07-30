//---------------------------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------------------

$GuiEditor::defaultGridSize = 8;
$GuiEditor::minGridSize = 3;

//-----------------------------------------------------------------------------------------
// Buttons
//-----------------------------------------------------------------------------------------

function GuiEditorPrefsDlgOkBtn::onAction(%this)
{
   $pref::guiEditor::snap2gridsize = GuiEditorPrefsDlgGridEdit.getValue();
   if( $pref::guiEditor::snap2grid )
      GuiEditor.setSnapToGrid( $pref::guiEditor::snap2gridsize );
      
   Canvas.popDialog( GuiEditorPrefsDlg );
}

function GuiEditorPrefsDlgCancelBtn::onAction(%this)
{
   Canvas.popDialog( GuiEditorPrefsDlg );
}

function GuiEditorPrefsDlgDefaultsBtn::onAction(%this)
{
   GuiEditorPrefsDlgGridSlider.setValue( $GuiEditor::defaultGridSize );
}

//-----------------------------------------------------------------------------------------
// Grid
//-----------------------------------------------------------------------------------------

function GuiEditorPrefsDlgGridEdit::onWake(%this)
{
   %this.setValue($pref::guiEditor::snap2gridsize);
}

function GuiEditorPrefsDlgGridEdit::onAction( %this )
{
   %value = %this.getValue();
   if( %value < $GuiEditor::minGridSize )
   {
         %value = $GuiEditor::minGridSize;
         %this.setValue( %value );
   }
   
   GuiEditorPrefsDlgGridSlider.setValue( %value );
}

function GuiEditorPrefsDlgGridSlider::onWake(%this)
{
   %this.setValue( $pref::guiEditor::snap2gridsize );
}

function GuiEditorPrefsDlgGridSlider::onAction(%this)
{
   %value = %this.value;
   if( %value < $GuiEditor::minGridSize )
   {
      %value = $GuiEditor::minGridSize;
      %this.setValue( %value );
   }
      
   GuiEditorPrefsDlgGridEdit.setvalue( mCeil( %value ) );
}
