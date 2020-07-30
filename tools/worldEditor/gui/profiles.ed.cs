//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

singleton GuiControlProfile (EditorDefaultProfile)
{
   opaque = true;
};

singleton GuiControlProfile (EditorToolButtonProfile)
{
   opaque = true;
   border = 2;
};

singleton GuiControlProfile (EditorTextProfile)
{
   fontType = "Arial Bold";
   fontColor = "0 0 0";
   autoSizeWidth = true;
   autoSizeHeight = true;
};

singleton GuiControlProfile (EditorTextProfileWhite)
{
   fontType = "Arial Bold";
   fontColor = "255 255 255";
   autoSizeWidth = true;
   autoSizeHeight = true;
};

singleton GuiControlProfile (WorldEditorProfile)
{
   canKeyFocus = true;
};

singleton GuiControlProfile (EditorScrollProfile)
{
   opaque = true;
   fillColor = "192 192 192 192";
   border = 3;
   borderThickness = 2;
   borderColor = "0 0 0";
   bitmap = "core/art/gui/images/scrollBar";
   hasBitmapArray = true;
};

singleton GuiControlProfile (GuiEditorClassProfile)
{
   opaque = true;
   fillColor = "232 232 232";
   border = true;
   borderColor   = "0 0 0";
   borderColorHL = "127 127 127";
   fontColor = "0 0 0";
   fontColorHL = "50 50 50";
   fixedExtent = true;
   justify = "center";
   bitmap = "core/art/gui/images/scrollBar";
   hasBitmapArray = true;
};

singleton GuiControlProfile( EPainterBitmapProfile )
{
   opaque = false;
   border = false;
   borderColor ="243 242 241";
   Color ="230 230 230";
};

singleton GuiControlProfile( EPainterBorderButtonProfile : GuiDefaultProfile )
{
   border = true;
   borderColor = "0 0 0";
   borderThickness = 2;
   
   fontColorHL = "255 0 0";
   fontColorSEL = "0 0 255";
};

singleton GizmoProfile( GlobalGizmoProfile )
{
   // JCF: this isnt a GuiControlProfile but fits in well here...
   // JCF: don't really have to initialize this now
   // because that will be done later based on the saved editor prefs.
   screenLength = 100;
};