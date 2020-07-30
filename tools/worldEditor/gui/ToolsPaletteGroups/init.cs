//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function EWToolsPaletteWindow::loadToolsPalettes()
{
   %filespec = "tools/worldEditor/gui/ToolsPaletteGroups/*.ed.gui";
   
   // were executing each gui file and adding them to the ToolsPaletteArray
   for( %file = findFirstFile(%filespec); %file !$= ""; %file = findNextFile(%filespec))
   {
      exec( %file );
      %paletteGroup = 0;      
      
      %i = %paletteId.getCount();
      for( ; %i != 0; %i--)
      {
         %paletteId.getObject(0).visible = 0;
         %paletteId.getObject(0).groupNum = %paletteGroup;
         %paletteId.getObject(0).paletteName = %paletteId.getName();
         ToolsPaletteArray.addGuiControl(%paletteId.getObject(0));
      }
      %paletteGroup++;
      //%paletteId.delete(); //delete gui storage container we no longer need
   }

   %filespec = "tools/worldEditor/gui/ToolsPaletteGroups/*.ed.gui.edso";
   
   // were executing each gui file and adding them to the ToolsPaletteArray
   for( %file = findFirstFile(%filespec); %file !$= ""; %file = findNextFile(%filespec))
   {
      exec( %file );
      %paletteGroup = 0;      
      
      %i = %paletteId.getCount();
      for( ; %i != 0; %i--)
      {
         %paletteId.getObject(0).visible = 0;
         %paletteId.getObject(0).groupNum = %paletteGroup;
         %paletteId.getObject(0).paletteName = %paletteId.getName();
         ToolsPaletteArray.addGuiControl(%paletteId.getObject(0));
      }
      %paletteGroup++;
      //%paletteId.delete(); //delete gui storage container we no longer need
   }
}

function EWToolsPaletteWindow::init()
{
   EWToolsPaletteWindow.loadToolsPalettes();
}

function EWToolsPaletteWindow::togglePalette(%this, %paletteName)
{
   // since the palette window ctrl auto adjusts to child ctrls being visible,
   // loop through the array and pick out the children that belong to a certain tool
   // and label them visible or not visible
   // EDIT: Sloppy that were doing this twice right now. It's now being done this way
   // due to some palettes have more than one set of guis
   
   for( %i = 0; %i < ToolsPaletteArray.getCount(); %i++ )
      ToolsPaletteArray.getObject(%i).visible = 0;
   
   %windowMultiplier = 0;
   %paletteNameWordCount = getWordCount( %paletteName );
   for(%pallateNum = 0; %pallateNum < %paletteNameWordCount; %pallateNum++)
   {
      %currentPalette = getWord(%paletteName, %pallateNum);
      for( %i = 0; %i < ToolsPaletteArray.getCount(); %i++ )
      {
         if( ToolsPaletteArray.getObject(%i).paletteName $= %currentPalette)
         {
            ToolsPaletteArray.getObject(%i).visible = 1;
            %windowMultiplier++;
         }
      }
   }
   
   // auto adjust the palette window extent according to how many 
   // children controls we found; if none found, the palette window becomes invisible
   if( %windowMultiplier == 0 || %paletteName $= "")
      EWToolsPaletteWindow.visible = 0;
   else
   {
      EWToolsPaletteWindow.visible = 1;
      EWToolsPaletteWindow.extent = getWord(EWToolsPaletteWindow.extent, 0) SPC (16 + 26 * %windowMultiplier);
   }
}

/*
// Counterpart to the function above; this crazy loop allows us to specifically order the palette if need be.
// It accepts a ordered string(seperated by spaces) and reorders the palette accoring to it
// For this function to work properly, it also requires us to specifically name and call out to 
// each button in the palette via internalNames.
function EWToolsPaletteWindow::reorderManually(%this, %specialOrder)
{
   for( %i = 0; %i < ToolsPaletteArray.getCount(); %i++ )
      ToolsPaletteArray.getObject(%i).visible = 0;
   
   %windowMultiplier = 0;

   for(%i = getWordCount( %specialOrder ); %i != 0; %i--)
   {
      for(%j = 0; %j < ToolsPaletteArray.getCount(); %j++)
      {
         if( getWord( %specialOrder, %i - 1 ) $= ToolsPaletteArray.getObject(%j).internalName )
         {
            ToolsPaletteArray.getObject(%j).visible = 1;
            ToolsPaletteArray.reorderChild(ToolsPaletteArray.findObjectByInternalName( getWord( %specialOrder, %i - 2 ) ), ToolsPaletteArray.getObject(%j));

            %windowMultiplier++;
            break;
         }
      }
   }
   
   // auto adjust the palette window extent according to how many 
   // children controls we found; if none found, the palette window becomes invisible
   if( %windowMultiplier == 0)
      EWToolsPaletteWindow.visible = 0;
   else
   {
      EWToolsPaletteWindow.visible = 1;
      EWToolsPaletteWindow.extent = getWord(EWToolsPaletteWindow.extent, 0) SPC (16 + 26 * %windowMultiplier);
   }
}
*/