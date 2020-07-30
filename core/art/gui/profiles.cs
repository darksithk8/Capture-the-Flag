//---------------------------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//---------------------------------------------------------------------------------------------

$Gui::fontCacheDirectory = getPrefsPath("fonts");

// If we got back no prefs path modification
if( $Gui::fontCacheDirectory $= "fonts" )
{
   $Gui::fontCacheDirectory = expandFilename( "~/fonts" );
}

//---------------------------------------------------------------------------------------------
// GuiDefaultProfile is a special profile that all other profiles inherit defaults from. It
// must exist.
//---------------------------------------------------------------------------------------------
singleton GuiControlProfile (GuiDefaultProfile)
{
   tab = false;
   canKeyFocus = false;
   hasBitmapArray = false;
   mouseOverSelected = false;

   // fill color
   opaque = false;
   fillColor = "242 241 240";
   fillColorHL ="228 228 235";
   fillColorSEL = "98 100 137";
   fillColorNA = "255 255 255 ";

   // border color
   border = 0;
   borderColor   = "100 100 100"; 
   borderColorHL = "50 50 50 50";
   borderColorNA = "75 75 75"; 

   // font
   fontType = "Arial";
   fontSize = 14;
   fontCharset = ANSI;

   fontColor = "0 0 0";
   fontColorHL = "0 0 0";
   fontColorNA = "0 0 0";
   fontColorSEL= "255 255 255";

   // bitmap information
   bitmap = "";
   bitmapBase = "";
   textOffset = "0 0";

   // used by guiTextControl
   modal = true;
   justify = "left";
   autoSizeWidth = false;
   autoSizeHeight = false;
   returnTab = false;
   numbersOnly = false;
   cursorColor = "0 0 0 255";

   // sounds
   //soundButtonDown = "";
   //soundButtonOver = "";
};

singleton GuiControlProfile (GuiSolidDefaultProfile)
{
   opaque = true;
   border = true;
};

singleton GuiControlProfile (GuiTransparentProfile)
{
   opaque = false;
   border = false;
};
singleton GuiControlProfile (GuiTransparentwbProfile)
{
   opaque = false;
   border = true;
   //fillcolor ="255 255 255";
   borderColor   = "100 100 100";
};
singleton GuiControlProfile( GuiGroupBorderProfile )
{
   border = false;
   opaque = false;
   hasBitmapArray = true;
   bitmap = "./images/group-border";
};
singleton GuiControlProfile( GuiTabBorderProfile )
{
   border = false;
   opaque = false;
   hasBitmapArray = true;
   bitmap = "./images/tab-border";
};
singleton GuiControlProfile( GuiGroupTitleProfile )
{
   fillColor = "242 241 240";
   fillColorHL ="242 241 240";
   fillColorNA = "242 241 240";
   fontColor = "0 0 0";
   opaque = true;
};


singleton GuiControlProfile (GuiToolTipProfile)
{
   // fill color
   fillColor = "239 237 222";

   // border color
   borderColor   = "138 134 122";

   // font
   fontType = "Arial";
   fontSize = 14;
   fontColor = "0 0 0";

};

singleton GuiControlProfile("GuiModelessDialogProfile")
{
   modal = false;
};

singleton GuiControlProfile (GuiFrameSetProfile)
{
   fillcolor = "255 255 255";//GuiDefaultProfile.fillColor;
   borderColor = "246 245 244";
   border = 1;
   //fillColor = "240 239 238";
   //borderColor = "50 50 50";//"204 203 202";
   //fillColor = GuiDefaultProfile.fillColorNA;
   //borderColor   = GuiDefaultProfile.borderColorNA;
   opaque = true;
   border = true;
};


if ($platform $= "macos")
{

   singleton GuiControlProfile (GuiWindowProfile)
   {
      	opaque = false;
   		border = 2;
   		fillColor = "242 241 240";
   		fillColorHL = "221 221 221";
   		fillColorNA = "200 200 200";
   		fontColor = "50 50 50";
   		fontColorHL = "0 0 0";
         bevelColorHL = "255 255 255";
         bevelColorLL = "0 0 0";
   		text = "untitled";
   		bitmap = "./images/window";
   		textOffset = "8 4";
   		hasBitmapArray = true;
   		justify = "center";
   		yPositionOffset = "21";
   };
}
else {

   singleton GuiControlProfile (GuiWindowProfile)
   {
   		opaque = false;
   		border = 2;
   		fillColor = "242 241 240";
   		fillColorHL = "221 221 221";
   		fillColorNA = "200 200 200";
   		fontColor = "50 50 50";
   		fontColorHL = "0 0 0";
         bevelColorHL = "255 255 255";
         bevelColorLL = "0 0 0";
   		text = "untitled";
   		bitmap = "./images/window";
   		textOffset = "8 4";
   		hasBitmapArray = true;
   		justify = "left";
   		yPositionOffset = "21";
	};
}
 singleton GuiControlProfile (GuiToolbarWindowProfile : GuiWindowProfile)
{
      bitmap = "./images/toolbar-window";
      text = "";
};  
singleton GuiControlProfile (GuiHToolbarWindowProfile : GuiWindowProfile)
{
      bitmap = "./images/htoolbar-window";
      text = "";
};  
singleton GuiControlProfile (GuiMenubarWindowProfile : GuiWindowProfile)
{
      bitmap = "./images/menubar-window";
      text = "";
}; 
	
singleton GuiControlProfile (GuiWindowCollapseProfile : GuiWindowProfile)
{
};

if(!isObject(chathudWindowProfile)) new GuiControlProfile (chathudWindowProfile)
{
   opaque = true;
   border = 2;
   fillColor = "158 158 158";
   fillColorHL = "221 221 221";
   fillColorNA = "200 200 200";
   fontColor = "255 255 255";
   fontColorHL = "200 200 200";
   text = "untitled";
   bitmap = "./images/chatHudBorderNEW";
   textOffset = "6 6";
   hasBitmapArray = true;
   justify = "center";
};
if(!isObject(numberIncProfile)) new GuiControlProfile (numberIncProfile)
{
   opaque = true;
   //fillColor = ($platform $= "macos") ? "211 211 211" : "192 192 192";
   fillColor ="200 200 200 50";
   fillColorHL = "0 0 96";
   border = 1;
   borderColor = "50 50 50";
   fontColor = "0 0 0";
   fontColorHL = "255 255 255";
   fontColorNA = "50 50 50";
   fontSize = 12;
   fixedExtent = true;
   justify = "left";
   canKeyFocus = false;
   mouseOverSelected = true;
   //hasBitmapArray = true;
};

singleton GuiControlProfile (GuiContentProfile)
{
   opaque = true;
   fillColor = "255 255 255";
};

singleton GuiControlProfile (GuiBlackContentProfile)
{
   opaque = true;
   fillColor = "0 0 0";
};

singleton GuiControlProfile( GuiInputCtrlProfile )
{
   tab = true;
   canKeyFocus = true;
};

singleton GuiControlProfile (GuiTextProfile)
{
   justify = "left";
   fontColor = "20 20 20";
};
singleton GuiControlProfile (GuiTextBoldProfile : GuiTextProfile)
{
   fontType = "Arial Bold";
   fontSize = 16;
};
singleton GuiControlProfile (GuiTextRightProfile : GuiTextProfile)
{
   justify = "right";
};
singleton GuiControlProfile (GuiTextCenterProfile : GuiTextProfile)
{
   justify = "center";
};
singleton GuiControlProfile (GuiTextSolidProfile : GuiTextProfile)
{
   opaque = true;
   border = 5;
   borderColor = GuiDefaultProfile.fillColor;
};

singleton GuiControlProfile (InspectorTitleTextProfile)
{
   fontColor = "100 100 100";
};

singleton GuiControlProfile (GuiTextProfileLight)
{
   fontColor = "220 220 220";
};

singleton GuiControlProfile (GuiAutoSizeTextProfile)
{
   fontColor = "255 255 255";
   autoSizeWidth = true;
   autoSizeHeight = true;
   border = 10;
};

singleton GuiControlProfile (GuiTextRightProfile : GuiTextProfile)
{
   justify = "right";
};

singleton GuiControlProfile( GuiMediumTextProfile : GuiTextProfile )
{
   fontSize = 24;
};

singleton GuiControlProfile( GuiBigTextProfile : GuiTextProfile )
{
   fontSize = 36;
};

singleton GuiControlProfile( GuiMLTextProfile )
{
   fontColorLink = "100 100 100";
   fontColorLinkHL = "255 255 255";
   autoSizeWidth = true;
   autoSizeHeight = true;  
   border = false;
};

singleton GuiControlProfile( GuiTextArrayProfile : GuiTextProfile )
{
   fontColor = "50 50 50";
   fontColorHL = " 0 0 0";
   fontColorSEL = "0 0 0";
   fillColor ="200 200 200";
   fillColorHL = "228 228 235";
   fillColorSEL = "200 200 200";
   border = false;
};

singleton GuiControlProfile( GuiTextListProfile : GuiTextProfile ) 
{
   tab = true;
   canKeyFocus = true;
};

singleton GuiControlProfile( GuiTextEditProfile )
{
   opaque = true;
   bitmap = "./images/textEdit";
   hasBitmapArray = true; 
   border = -2; // fix to display textEdit img
   //borderWidth = "1";  // fix to display textEdit img
   //borderColor = "100 100 100";
   fillColor = "242 241 240 0";
   fillColorHL = "255 255 255";
   fontColor = "0 0 0";
   fontColorHL = "255 255 255";
   fontColorSEL = "98 100 137";
   fontColorNA = "200 200 200";
   textOffset = "4 2";
   autoSizeWidth = false;
   autoSizeHeight = true;
   justify = "left";
   tab = true;
   canKeyFocus = true;
   
};

singleton GuiControlProfile( GuiTextEditProfileNumbersOnly : GuiTextEditProfile )
{
   numbersOnly = true;
};
singleton GuiControlProfile( GuiTextEditNumericProfile : GuiTextEditProfile )
{
   numbersOnly = true;
};
singleton GuiControlProfile( GuiNumericDropSliderTextProfile : GuiTextEditProfile )
{
   bitmap = "./images/textEditSliderBox";
};
singleton GuiControlProfile( GuiTextEditDropSliderNumbersOnly :  GuiTextEditProfile )
{
   numbersOnly = true;
   bitmap = "./images/textEditSliderBox";
};
singleton GuiControlProfile( GuiProgressProfile )
{
   opaque = false;
   fillColor = "0 162 255 200";
   border = true;
   borderColor   = "50 50 50 200";
};

singleton GuiControlProfile( GuiProgressBitmapProfile )
{
   border = false;
   hasBitmapArray = true;
   bitmap = "./images/loadingbar";
};
singleton GuiControlProfile( GuiRLProgressBitmapProfile )
{
   border = false;
   hasBitmapArray = true;
   bitmap = "./images/rl-loadingbar";
};

singleton GuiControlProfile( GuiProgressTextProfile )
{
   fontSize = "14";
	fontType = "Arial";
   fontColor = "0 0 0";
   justify = "center";
   
};

singleton GuiControlProfile( GuiButtonProfile )
{
   opaque = true;
   border = true;
	 
   fontColor = "50 50 50";
   fontColorHL = "0 0 0";
	 fontColorNA = "200 200 200";
	 //fontColorSEL ="0 0 0";
   fixedExtent = false;
   justify = "center";
   canKeyFocus = false;
	bitmap = "./images/button";
   hasBitmapArray = false;
};

singleton GuiControlProfile( GuiThumbHighlightButtonProfile : GuiButtonProfile )
{
   bitmap = "./images/thumbHightlightButton";
};

singleton GuiControlProfile( InspectorDynamicFieldButton : GuiButtonProfile )
{
   canKeyFocus = true;
};

singleton GuiControlProfile( GuiMenuButtonProfile )
{
   opaque = true;
   border = false;
   fontSize = 18;
   fontType = "Arial Bold";
   fontColor = "50 50 50";
   fontColorHL = "0 0 0";
	 fontColorNA = "200 200 200";
	 //fontColorSEL ="0 0 0";
   fixedExtent = false;
   justify = "center";
   canKeyFocus = false;
	bitmap = "./images/selector-button";
   hasBitmapArray = false;
};
singleton GuiControlProfile( GuiIconButtonProfile )
{
   opaque = true;
   border = true;
	 
   fontColor = "50 50 50";
   fontColorHL = "0 0 0";
	 fontColorNA = "200 200 200";
	 //fontColorSEL ="0 0 0";
   fixedExtent = false;
   justify = "center";
   canKeyFocus = false;
	bitmap = "./images/iconbutton";
   hasBitmapArray = true;
};
singleton GuiControlProfile( GuiIconButtonSolidProfile : GuiIconButtonProfile )
{
   bitmap = "./images/iconbuttonsolid";
};
singleton GuiControlProfile( GuiIconButtonSmallProfile : GuiIconButtonProfile )
{
   bitmap = "./images/iconbuttonsmall";
};
singleton GuiControlProfile( GuiButtonTabProfile )
{
   opaque = true;
   border = true;
	 
   fontColor = "50 50 50";
   fontColorHL = "0 0 0";
   fontColorNA = "0 0 0";
	 //fontColorSEL ="0 0 0";
   fixedExtent = false;
   justify = "center";
   canKeyFocus = false;
   bitmap = "./images/buttontab";
  // hasBitmapArray = false;
};

singleton GuiControlProfile(EditorTabPage)
{
   opaque = true;
   border = false;
   //fontSize = 18;
   //fontType = "Arial";
   fontColor = "0 0 0";
   fontColorHL = "0 0 0";
   fixedExtent = false;
   justify = "center";
   canKeyFocus = false;
   bitmap = "./images/tab";
   hasBitmapArray = true; //false;
};

singleton GuiControlProfile( GuiCheckBoxProfile )
{
   opaque = false;
   fillColor = "232 232 232";
   border = false;
   borderColor = "100 100 100";
   fontSize = 14;
   fontColor = "20 20 20";
   fontColorHL = "80 80 80";
	fontColorNA = "200 200 200";
   fixedExtent = true;
   justify = "left";
   bitmap = "./images/checkbox";
   hasBitmapArray = true;
};
singleton GuiControlProfile( GuiCheckBoxListProfile : GuiCheckBoxProfile)
{
   bitmap = "./images/checkbox-list";
};
singleton GuiControlProfile( GuiCheckBoxListFlipedProfile : GuiCheckBoxProfile)
{
   bitmap = "./images/checkbox-list_fliped";
};

singleton GuiControlProfile( InspectorCheckBoxTitleProfile : GuiCheckBoxProfile ){
   fontColor = "100 100 100";
};
singleton GuiControlProfile( GuiRadioProfile )
{
   fontSize = 14;
   fillColor = "232 232 232";
	/*fontColor = "200 200 200";
   fontColorHL = "255 255 255";*/
   fontColor = "20 20 20";
   fontColorHL = "80 80 80";
   fixedExtent = true;
   bitmap = "./images/radioButton";
   hasBitmapArray = true;
};

singleton GuiControlProfile( GuiScrollProfile )
{
   opaque = true;
   fillcolor = "255 255 255";
   fontColor = "0 0 0";
   fontColorHL = "150 150 150";
   //borderColor = GuiDefaultProfile.borderColor;
   border = true;
   bitmap = "./images/scrollBar";
   hasBitmapArray = true;
};

singleton GuiControlProfile( GuiOverlayProfile )
{
   opaque = true;
   fillcolor = "255 255 255";
	 fontColor = "0 0 0";
	 fontColorHL = "255 255 255";
	fillColor = "0 0 0 100";
};

singleton GuiControlProfile( GuiTransparentScrollProfile )
{
   opaque = false;
   fillColor = "255 255 255";
	 fontColor = "0 0 0";
   border = false;
   borderThickness = 2;
   borderColor = "100 100 100";
   bitmap = "./images/scrollBar";
   hasBitmapArray = true;
};
 
singleton GuiControlProfile( GuiSliderProfile )
{
   bitmap = "./images/slider";
};
singleton GuiControlProfile( GuiSliderBoxProfile )
{
   bitmap = "./images/slider-w-box";
};

singleton GuiControlProfile( GuiPaneProfile )
{
   bitmap = "./images/popupMenu";
   hasBitmapArray = true;
};



singleton GuiControlProfile( GuiPopupMenuItemBorder : GuiButtonProfile )
{
  // borderThickness = 1;
   //borderColor = "100 100 100 220"; //200
   //borderColorHL = "51 51 51 220"; //200
   
    opaque = true;
   border = true;
	 
   fontColor = "0 0 0";
   fontColorHL = "0 0 0";
   fontColorNA = "255 255 255";
   fixedExtent = false;
   justify = "center";
   canKeyFocus = false;
	 bitmap = "./images/button";
  // hasBitmapArray = false;

};

singleton GuiControlProfile( GuiPopUpMenuDefault : GuiDefaultProfile )
{
   opaque = true;
   mouseOverSelected = true;
   textOffset = "3 3";
   border = 0;
   borderThickness = 0;
   fixedExtent = true;
   bitmap = "./images/scrollbar";
   hasBitmapArray = true;
   profileForChildren = GuiPopupMenuItemBorder;
   fillColor = "242 241 240 ";//"255 255 255";//100
   fillColorHL = "228 228 235 ";//"204 203 202";
   fillColorSEL = "98 100 137 ";//"204 203 202";
   // font color is black
   fontColorHL = "0 0 0 ";//"0 0 0";
   fontColorSEL = "255 255 255";//"0 0 0";
   borderColor = "100 100 100";
};

singleton GuiControlProfile( GuiPopupBackgroundProfile )
{
   modal = true;
};


singleton GuiControlProfile( GuiPopUpMenuProfile : GuiPopUpMenuDefault )
{
   textOffset         = "6 4";
   bitmap             = "./images/dropDown";
   hasBitmapArray     = true;
   border             = 1;
   profileForChildren = GuiPopUpMenuDefault;
};
singleton GuiControlProfile( GuiPopUpMenuTabProfile : GuiPopUpMenuDefault )
{
   bitmap             = "./images/dropDown-tab";
   textOffset         = "6 4";
   canKeyFocus        = true;
   hasBitmapArray     = true;
   border             = 1;
   profileForChildren = GuiPopUpMenuDefault;
};
singleton GuiControlProfile( GuiPopUpMenuEditProfile : GuiPopUpMenuDefault )
{
   textOffset         = "6 4";
   canKeyFocus        = true;
   bitmap             = "./images/dropDown";
   hasBitmapArray     = true;
   border             = 1;
   profileForChildren = GuiPopUpMenuDefault;
};

singleton GuiControlProfile( GuiListBoxProfile )
{
   tab = true;
   canKeyFocus = true;
};

singleton GuiControlProfile( GuiTabBookProfile )
{
   fillColorHL = "100 100 100";
   fillColorNA = "150 150 150";
   fontColor = "30 30 30";
   fontColorHL = "0 0 0";
   fontColorNA = "50 50 50";
   fontType = "Arial";
   fontSize = 14;
   justify = "center";
   bitmap = "./images/tab";
   tabWidth = 64;
   tabHeight = 24;
   tabPosition = "Top";
   tabRotation = "Horizontal";
   textOffset = "0 -3";
   tab = true;
   cankeyfocus = true;
   //border = false;
   //opaque = false;
};

singleton GuiControlProfile( GuiTabBookNoBitmapProfile : GuiTabBookProfile )
{
   bitmap = "";
};

singleton GuiControlProfile( GuiTabPageProfile : GuiDefaultProfile )
{
		fontType = "Arial";
   fontSize = 10;
   justify = "center";
   bitmap = "./images/tab";
   opaque = false;
   fillColor = "240 239 238";
};

singleton GuiControlProfile( GuiMenuBarProfile )
{
   fontType = "Arial";
   fontSize = 14;
   opaque = true;
   fillColor = "240 239 238";
   fillColorHL = "202 201 200";
   fillColorSEL = "202 0 0";
   
   borderColorNA = "202 201 200";
   borderColorHL = "50 50 50";
   border = 0;
   fontColor = "20 20 20";
   fontColorHL = "0 0 0";
   fontColorNA = "255 255 255";
   //fixedExtent = true;
   justify = "center";
   canKeyFocus = false;
   mouseOverSelected = true;
   bitmap = "./images/menu";
   hasBitmapArray = true;
};

singleton GuiControlProfile( GuiConsoleProfile )
{
   fontType = ($platform $= "macos") ? "Monaco" : "Lucida Console";
   fontSize = ($platform $= "macos") ? 13 : 12;
    fontColor = "255 255 255";
    fontColorHL = "0 255 255";
    fontColorNA = "255 0 0";
    fontColors[6] = "100 100 100";
    fontColors[7] = "100 100 0";
    fontColors[8] = "0 0 100";
    fontColors[9] = "0 100 0";
};

singleton GuiControlProfile( GuiConsoleTextEditProfile : GuiTextEditProfile )
{
   fontType = ($platform $= "macos") ? "Monaco" : "Lucida Console";
   fontSize = ($platform $= "macos") ? 13 : 12;
};

singleton GuiControlProfile( GuiConsoleTextProfile )
{   
   fontColor = "0 0 0";
   autoSizeWidth = true;
   autoSizeHeight = true;   

   textOffset = "2 2";
   opaque = true;   
   fillColor = "255 255 255";
   border = true;
   borderThickness = 1;
   borderColor = "0 0 0";
};

singleton GuiControlProfile( GuiTreeViewProfile )
{  
   bitmap            = "./images/treeView";
   autoSizeHeight    = true;
   canKeyFocus       = true;
   
   fillColor = "255 255 255"; //GuiDefaultProfile.fillColor;
   fillColorHL = "228 228 235";//GuiDefaultProfile.fillColorHL;
   fillColorSEL = "98 100 137";
   fillColorNA = "255 255 255";//GuiDefaultProfile.fillColorNA;
   fontColor = "0 0 0";//GuiDefaultProfile.fontColor;
   fontColorHL = "0 0 0";//GuiDefaultProfile.fontColorHL;   
   fontColorSEL= "255 255 255";//GuiDefaultProfile.fontColorSEL;
   fontColorNA = "200 200 200";//GuiDefaultProfile.fontColorNA;
   borderColor = "128 000 000";
   borderColorHL = "255 228 235";

   fontSize = 14;
   
   opaque = false;
   border = false;
};

singleton GuiControlProfile( GuiSimpleTreeProfile : GuiTreeViewProfile )
{
   opaque = true;
   fillColor = "255 255 255 255";
   border = true;
};

//*** DAW:
singleton GuiControlProfile( GuiText24Profile : GuiTextProfile )
{
   fontSize = 24;
};

singleton GuiControlProfile( GuiRSSFeedMLTextProfile )
{
   fontColorLink = "55 55 255";
   fontColorLinkHL = "255 55 55";
};

$ConsoleDefaultFillColor = "0 0 0 175";

singleton GuiControlProfile( ConsoleScrollProfile : GuiScrollProfile )
{
	opaque = true;
	fillColor = $ConsoleDefaultFillColor;
	border = 1;
	//borderThickness = 0;
	borderColor = "0 0 0";
};

singleton GuiControlProfile( ConsoleTextEditProfile : GuiTextEditProfile )
{
   fillColor = "242 241 240 255";
   fillColorHL = "255 255 255";   
};

singleton GuiControlProfile( GuiTextPadProfile )
{
   fontType = ($platform $= "macos") ? "Monaco" : "Lucida Console";
   fontSize = ($platform $= "macos") ? 13 : 12;
   tab = true;
   canKeyFocus = true;
   
   // Deviate from the Default
   opaque=true;  
   fillColor = "255 255 255";
   
   border = 0;
};

singleton GuiControlProfile( GuiTransparentProfileModeless : GuiTransparentProfile )  
{
   modal = false;
};

singleton GuiControlProfile( GuiParticleListBoxProfile : GuiListBoxProfile )
{
   tab = true;
   canKeyFocus = true;
   fontColor = "200 200 200";
   fontColorHL = "25 25 25 220";
   fontColorNA = "204 203 202";
   fontColor = "0 0 0 150";
};

singleton GuiControlProfile( GuiFormProfile : GuiTextProfile )
{
   opaque = false;
   border = 5;
   
   bitmap = "./images/form";
   hasBitmapArray = true;

   justify = "center";
   
   profileForChildren = GuiButtonProfile;
   
   // border color
	 opaque = false;
   //border = 5;
	 bitmap = "./images/button";
  // borderColor   = "153 153 153"; 
 //  borderColorHL = "230 230 230";
 //  borderColorNA = "126 79 37";
};

singleton GuiControlProfile( GuiBreadcrumbsMenuProfile )
{
   fontColor = "0 0 0";
   fontType = "Arial";
   fontSize = 14;
   
   bitmap = "./images/breadcrumbs";
   hasBitmapArray = true;
};

singleton GuiControlProfile( GuiNumericTextEditSliderProfile )
{
   // Transparent Background
   opaque = true;
   fillColor = "0 0 0 0";
   fillColorHL = "255 255 255";
   
   border = true;   

   tab = false;
   canKeyFocus = true;

   // font
   fontType = "Arial";
   fontSize = 14;

   fontColor = "0 0 0";
   fontColorSEL = "43 107 206";
   fontColorHL = "244 244 244";
   fontColorNA = "100 100 100";
   
   numbersOnly = true;
};

singleton GuiControlProfile( GuiNumericTextEditSliderBitmapProfile )
{
   // Transparent Background
   opaque = true;
   
   border = true;   
   borderColor = "100 100 100";
   
   tab = false;
   canKeyFocus = true;

   // font
   fontType = "Arial";
   fontSize = 14;

   fillColor = "242 241 240";//"255 255 255";
   fillColorHL = "255 255 255";//"222 222 222";
   fontColor = "0 0 0";//"0 0 0";
   fontColorHL = "255 255 255";//"0 0 0";
   fontColorSEL = "98 100 137";//"230 230 230";
   fontColorNA = "200 200 200";//"0 0 0";
   
   numbersOnly = true;
   
   hasBitmapArray = true;
   bitmap = "./images/numericslider";
};

singleton GuiControlProfile( GuiMultiFieldTextEditProfile : GuiTextEditProfile )
{
};

singleton GuiControlProfile( GuiModalDialogBackgroundProfile )
{
   opaque = true;
   fillColor = "221 221 221 150";
};

//-----------------------------------------------------------------------------
// Center and bottom print
//-----------------------------------------------------------------------------

singleton GuiControlProfile ("CenterPrintProfile")
{
   opaque = false;
   fillColor = "128 128 128";
   fontColor = "0 255 0";
   border = true;
   borderColor = "0 255 0";
};

singleton GuiControlProfile ("CenterPrintTextProfile")
{
   opaque = false;
   fontType = "Arial";
   fontSize = 12;
   fontColor = "0 255 0";
};