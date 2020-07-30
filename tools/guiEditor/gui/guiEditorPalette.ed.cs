//---------------------------------------------------------------------------------------------
// Torque 3D
// Copyright (C) GarageGames.com, Inc.
//---------------------------------------------------------------------------------------------


//---------------------------------------------------------------------------------------------

function GuiEditorPaletteGui::onWake( %this )
{
   GuiEditorStealPalette( %this );
}

//---------------------------------------------------------------------------------------------

function GuiEditorStealPalette( %thief )
{
   %pal = %thief-->palette;
   if( !isObject( %pal ) )
      %thief.add( GuiEditorPalette );
}

//---------------------------------------------------------------------------------------------

function GuiEditorPalette::createControl( %this, %className )
{
   %ctrl = eval( "return new " @ %className @ "();" );
   if( !isObject( %ctrl ) )
      return;
      
   // Add the control.
   
   GuiEditor.addNewCtrl( %ctrl );
}

//---------------------------------------------------------------------------------------------

function GuiEditorPalette::isFilteredClass( %this, %className )
{
   switch$( %className )
   {
      case "GuiCanvas":             return true;
      case "GuiAviBitmapCtrl":      return true; // For now.  Probably removed altogether.
      case "GuiArrayCtrl":          return true; // Abstract base class really.
      case "GuiScintillaTextCtrl":  return true; // Internal class.
      case "GuiNoMouseCtrl":        return true; // Too odd.
   }
   
   return false;
}

//---------------------------------------------------------------------------------------------

function GuiEditorPalette::addClassToCommonPage( %this, %className )
{
   %list = %this-->listboxCommon;
   %id = %list.addItem( %className );
   %list.setItemTooltip( %id, getDescriptionOfClass( %className ) );
}

//---------------------------------------------------------------------------------------------

function GuiEditorPalette::onWake( %this )
{
   if( !%this.isInitialized )
      %this.initialize();
}

//---------------------------------------------------------------------------------------------

function GuiEditorPalette::initialize( %this )
{
   // Populate the list containing all GuiControls.
   
   %controls = enumerateConsoleClasses( "GuiControl" );
   %listboxAll = %this-->listboxAll;
   %listboxAll.clearItems();
   for( %i = 0; %i < getFieldCount( %controls ); %i ++)
   {
      %className = getField( %controls, %i );
      if( %this.isFilteredClass( %className ) )
         continue;
         
      %id = %listboxAll.addItem( %className );
      %listboxAll.setItemTooltip( %id, getDescriptionOfClass( %className ) );
   }
   
   // Populate the categorized view of controls.
   
   %treeView = %this-->treeView;
   %treeView.clear();
   %controls = enumerateConsoleClassesByCategory( "Gui" );
   for( %i = 0; %i < getFieldCount( %controls ); %i ++)
   {
      %className = getField( %controls, %i );
      if( %this.isFilteredClass( %className )
          || !isMemberOfClass( %className, "GuiControl" ) )
         continue;
         
      // Find the parent node for the class category.
      
      %parent = 0;
      %category = getCategoryOfClass( %className );
      %numSubCategories = getWordCount( %category );
      
      if( %numSubCategories >= 2 )
      {
         for( %n = 1; %n < %numSubCategories; %n ++ )
         {
            %subCategory = getWord( %category, %n );
            %newParent = %treeView.findChildItemByName( %parent, %subCategory );
            if( !%newParent )
               %parent = %treeView.insertItem( %parent, %subCategory );
            else
               %parent = %newParent;
         }
      }
   
      // Insert the item.
         
      %id = %treeView.insertItem( %parent, %className );
      %treeView.setItemTooltip( %id, getDescriptionOfClass( %className ) );
   }
   
   %treeView.sort( 0, true, true, false );
   
   // Populate the common page with some often used controls.

   %this-->listboxCommon.clearItems();
   %this.addClassToCommonPage( "GuiControl" );
   %this.addClassToCommonPage( "GuiBitmapButtonCtrl" );
   %this.addClassToCommonPage( "GuiBitmapButtonTextCtrl" );
   %this.addClassToCommonPage( "GuiButtonCtrl" );
   %this.addClassToCommonPage( "GuiCheckBoxCtrl" );
   %this.addClassToCommonPage( "GuiConsole" );
   %this.addClassToCommonPage( "GuiFadeinBitmapCtrl" );
   %this.addClassToCommonPage( "GuiFrameSetCtrl" );
   %this.addClassToCommonPage( "GuiListBoxCtrl" );
   %this.addClassToCommonPage( "GuiPopUpMenuCtrl" );
   %this.addClassToCommonPage( "GuiRadioCtrl" );
   %this.addClassToCommonPage( "GuiRolloutCtrl" );
   %this.addClassToCommonPage( "GuiScrollCtrl" );
   %this.addClassToCommonPage( "GuiSeparatorCtrl" );
   %this.addClassToCommonPage( "GuiSliderCtrl" );
   %this.addClassToCommonPage( "GuiTabBookCtrl" );
   %this.addClassToCommonPage( "GuiTabPageCtrl" );
   %this.addClassToCommonPage( "GuiTextCtrl" );
   %this.addClassToCommonPage( "GuiTextEditCtrl" );
   %this.addClassToCommonPage( "GuiWindowCtrl" );
   
   // Select default page.
   
   %defaultPage = 0;
   if( isDefined( "$pref::GuiEditor::defaultEditorPalettePage" ) )
      %defaultPage = $pref::GuiEditor::defaultEditorPalettePage;
   
   %this-->paletteBook.selectPage( %defaultPage );
   %this.isInitialized = true;
}

//---------------------------------------------------------------------------------------------

function GuiEditorPalette::startGuiControlDrag( %this, %class )
{
   // Create a new control of the given class.
   
   %payload = eval( "return new " @ %class @ "();" );
   if( !isObject( %payload ) )
      return;
   
   // this offset puts the cursor in the middle of the dragged object.
   %xOffset = getWord( %payload.extent, 0 ) / 2;
   %yOffset = getWord( %payload.extent, 1 ) / 2;  
   
   // position where the drag will start, to prevent visible jumping.
   %cursorpos = Canvas.getCursorPos();
   %xPos = getWord( %cursorpos, 0 ) - %xOffset;
   %yPos = getWord( %cursorpos, 1 ) - %yOffset;
   
   // Create drag&drop control.
   
   %dragCtrl = new GuiDragAndDropControl()
   {
      canSaveDynamicFields    = "0";
      Profile                 = "GuiSolidDefaultProfile";
      HorizSizing             = "right";
      VertSizing              = "bottom";
      Position                = %xPos SPC %yPos;
      extent                  = %payload.extent;
      MinExtent               = "32 32";
      canSave                 = "1";
      Visible                 = "1";
      hovertime               = "1000";
      deleteOnMouseUp         = true;
   };
   
   %dragCtrl.add( %payload );
   Canvas.getContent().add( %dragCtrl );
   
   // Start drag.

   %dragCtrl.startDragging( %xOffset, %yOffset );
}

//---------------------------------------------------------------------------------------------

function GuiEditorPaletteDragList::onMouseDragged( %this )
{
   %class = %this.getItemText( %this.getSelectedItem() );
   GuiEditorPalette.startGuiControlDrag( %class );
}

//---------------------------------------------------------------------------------------------

function GuiEditorPaletteDragList::onMouseUp( %this, %id )
{
   %class = %this.getItemText( %id );
   GuiEditorPalette.createControl( %class );
}

//---------------------------------------------------------------------------------------------

function GuiEditorPaletteDragView::onMouseDragged( %this )
{
   %selectedItem = %this.getSelectedItem();
   if( !%this.isParentItem( %selectedItem ) )
   {
      %class = %this.getItemText( %this.getSelectedItem() );
      GuiEditorPalette.startGuiControlDrag( %class );
   }
}

//---------------------------------------------------------------------------------------------

/// Callback that will be triggered in the class tree view when a class
/// name is single-clicked.
function GuiEditorPaletteDragView::onMouseUp( %this, %id )
{
   if( %id == -1 || %this.isParentItem( %id ) )
      return;
      
   // Get the class name under the cursor and create
   // a new control.
      
   %class = %this.getItemText( %id );
   GuiEditorPalette.createControl( %class );
}
