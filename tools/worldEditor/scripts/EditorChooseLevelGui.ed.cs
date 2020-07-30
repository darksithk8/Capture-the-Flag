//-----------------------------------------------------------------------------
// Torque Game Engine
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Canvas.setContent(EditorChooseLevelGui);

function WE_EditLevel(%levelFile)
{
   %serverType = "SinglePlayer";

   createServer(%serverType, %levelFile);
   %conn = new GameConnection(ServerConnection);
   RootGroup.add(ServerConnection);
   %conn.setConnectArgs($pref::Player::Name);
   %conn.setJoinPassword($Client::Password);
   %conn.connectLocal();

   // recreate and open the editor
   Editor::create();
   Editor.open();
   MissionCleanup.add(Editor);
   EditorGui.loadingMission = true;
}

function WE_ReturnToMainMenu()
{
   loadMainMenu();
}

function WE_LevelList::onURL(%this, %url)
{
   // Remove 'gamelink:' from front
   %levelFile = getSubStr(%url, 9, 1024);
   WE_EditLevel(%levelFile);
}

function WE_TemplateList::onURL(%this, %url)
{
   // Remove 'gamelink:' from front
   %levelFile = getSubStr(%url, 9, 1024);
   WE_EditLevel(%levelFile);
   EditorGui.saveAs = true;
}

function EditorChooseLevelGui::onWake()
{
   // first check if we have a level file to load, then we'll bypass this
   if ($levelToLoad !$= "")
   {
      %levelFile = "levels/";
      %ext = getSubStr($levelToLoad, strlen($levelToLoad) - 3, 3);
      if(%ext !$= "mis")
         %levelFile = %levelFile @ $levelToLoad @ ".mis";
      else
         %levelFile = %levelFile @ $levelToLoad;
         
      // let's make sure the file exists
      %file = findFirstFile(%levelFile);
         
      if(%file !$= "")
      {
         WE_EditLevel(%file);
         return;      
      }
   }
   
   // Build the text lists
   WE_LevelList.clear();
   WE_TemplateList.clear();

   %leveltext = "<linkcolor:0000FF><linkcolorhl:FF0000>";
   %templatetext = "<linkcolor:0000FF><linkcolorhl:FF0000>";
   for(%file = findFirstFile($Server::MissionFileSpec); %file !$= ""; %file = findNextFile($Server::MissionFileSpec))
   {
      %name = getLevelDisplayName(%file);
      %n = strlwr(%name);
      if(strstr(%n, "template") == -1)
      {
         %leveltext = %leveltext @ "<a:gamelink:" @ %file @ ">" @ %name @ "</a><br>";
      }
      else
      {
         %templatetext = %templatetext @ "<a:gamelink:" @ %file @ ">" @ %name @ "</a><br>";
      }
   }

   WE_LevelList.setText(%leveltext);
   WE_LevelList.forceReflow();
   WE_LevelList.scrollToTop();

   WE_TemplateList.setText(%templatetext);
   WE_TemplateList.forceReflow();
   WE_TemplateList.scrollToTop();
}   

function getLevelDisplayName( %levelFile ) 
{
   %file = new FileObject();
   
   %MissionInfoObject = "";
   
   if ( %file.openForRead( %levelFile ) ) {
		%inInfoBlock = false;
		
		while ( !%file.isEOF() ) {
			%line = %file.readLine();
			%line = trim( %line );
			
			if( %line $= "new ScriptObject(MissionInfo) {" )
				%inInfoBlock = true;
         else if( %line $= "new LevelInfo(theLevelInfo) {" )
				%inInfoBlock = true;
			else if( %inInfoBlock && %line $= "};" ) {
				%inInfoBlock = false;
				%MissionInfoObject = %MissionInfoObject @ %line; 
				break;
			}
			
			if( %inInfoBlock )
			   %MissionInfoObject = %MissionInfoObject @ %line @ " "; 	
		}
		
		%file.close();
	}
	%MissionInfoObject = "%MissionInfoObject = " @ %MissionInfoObject;
	eval( %MissionInfoObject );
	
   %file.delete();

   if( %MissionInfoObject.name !$= "" )
      return %MissionInfoObject.name;
   else
      return fileBase(%levelFile); 
}
