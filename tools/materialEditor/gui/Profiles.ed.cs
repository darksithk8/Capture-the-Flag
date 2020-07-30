//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
// 
// Material Editor Written by Dave Calabrese of Gaslight Studios
//-----------------------------------------------------------------------------

if(!isObject(GuiMatEdSliderProfile)) new GuiControlProfile (GuiMatEdSliderProfile)
{
   bitmap = "./matEdSlider";
};

if(!isObject(GuiMatEdRightJustifyProfile)) new GuiControlProfile (GuiMatEdRightJustifyProfile)
{
   // font
   fontType = "Arial";
   fontSize = 14;
   fontCharset = ANSI;

   fontColor = "0 0 0";
   
   justify = "right";
};

new GuiControlProfile(GuiMatEdPopUpMenuProfile)
{
   opaque = false;
   mouseOverSelected = true;
   textOffset = "3 3";
   border = 1;
   /*borderThickness = 1;*/
   fixedExtent = true;
   //bitmap = "./images/scrollbar";
   bitmap = "tools/editorClasses/gui/images/scroll";
   hasBitmapArray = true;
   //profileForChildren = GuiPopupMenuItemBorder;
   profileForChildren = GuiControlListPopupProfile;
   fillColor = "255 0 0 255";
   fontColor = "255 255 255 255";
   fillColorHL = "50 50 50";
   fontColorHL = "220 220 220";
   borderColor = "100 100 108";
};

new GuiControlProfile (MatEdCenteredTextProfile)
{
   fontColor = "0 0 0";
   justify = "center";
};