//-----------------------------------------------------------------------------
// Torque Game Engine Advanced
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Set the name of our application
$appName = "AI";

// The directory it is run from
$defaultGame = "scripts";

// Set profile directory
$Pref::Video::ProfilePath = "core/profile";

function createCanvas(%windowTitle)
{
   if ($isDedicated)
   {
      GFXInit::createNullDevice();
      return true;
   }

   // Create the Canvas
   %foo = new GuiCanvas(Canvas);
   
   // Set the window title
   if (isObject(Canvas))
      Canvas.setWindowTitle(getEngineName() SPC getVersionString() @ " - " @ $appName);
   
   return true;
}

// Display the optional commandline arguements
$displayHelp = false;

// Use these to record and play back crashes
//saveJournal("editorOnFileQuitCrash.jrn");
//playJournal("editorOnFileQuitCrash.jrn", false);

//-----------------------------------------------------------------------------
// Support functions used to manage the mod string

function pushFront(%list, %token, %delim)
{
   if (%list !$= "")
      return %token @ %delim @ %list;
   return %token;
}

function pushBack(%list, %token, %delim)
{
   if (%list !$= "")
      return %list @ %delim @ %token;
   return %token;
}

function popFront(%list, %delim)
{
   return nextToken(%list, unused, %delim);
}


//------------------------------------------------------------------------------
// Process command line arguments

// Run the Torque Creator mod by default, it's needed for editors.


$isDedicated = false;
$dirCount = 2;
$userDirs = $defaultGame @ ";art;levels";

// load tools scripts if we're a tool build
if (isToolBuild())
    $userDirs = "tools;" @ $userDirs;
    
for ($i = 1; $i < $Game::argc ; $i++)
{
   $arg = $Game::argv[$i];
   $nextArg = $Game::argv[$i+1];
   $hasNextArg = $Game::argc - $i > 1;
   $logModeSpecified = false;

   // Check for dedicated run
   if( stricmp($arg,"-dedicated") == 0  )
   {
      $userDirs = $defaultGame;
      $dirCount = 1;
      $isDedicated = true;
   }

   switch$ ($arg)
   {
      //--------------------
      case "-log":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            // Turn on console logging
            if ($nextArg != 0)
            {
               // Dump existing console to logfile first.
               $nextArg += 4;
            }
            setLogMode($nextArg);
            $logModeSpecified = true;
            $argUsed[$i+1]++;
            $i++;
         }
         else
            error("Error: Missing Command Line argument. Usage: -log <Mode: 0,1,2>");

      //--------------------
      case "-dir":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            // Append the mod to the end of the current list
            $userDirs = strreplace($userDirs, $nextArg, "");
            $userDirs = pushFront($userDirs, $nextArg, ";");
            $argUsed[$i+1]++;
            $i++;
            $dirCount++;
         }
         else
            error("Error: Missing Command Line argument. Usage: -dir <dir_name>");

      //--------------------
      // changed the default behavior of this command line arg. It now
      // defaults to ONLY loading the game, not tools 
      // default auto-run already loads in tools --SRZ 11/29/07
      case "-game":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            // Set the selected dir --NOTE: we no longer allow tools with this argument
            /* 
            if( $isDedicated )
            {
               $userDirs = $nextArg;
               $dirCount = 1;
            }
            else
            {
               $userDirs = "tools;" @ $nextArg;
               $dirCount = 2;
            }
            */
            $userDirs = $nextArg;
            $dirCount = 1;
            $argUsed[$i+1]++;
            $i++;
            error($userDirs);
         }
         else
            error("Error: Missing Command Line argument. Usage: -game <game_name>");

/* deprecated SRZ 11/29/07
      //--------------------
      case "-show":
         // A useful shortcut for -mod show
         $userMods = strreplace($userMods, "show", "");
         $userMods = pushFront($userMods, "show", ";");
         $argUsed[$i]++;
         $modcount++;
*/
      //--------------------
      case "-console":
         enableWinConsole(true);
         $argUsed[$i]++;

      //--------------------
      case "-jSave":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            echo("Saving event log to journal: " @ $nextArg);
            saveJournal($nextArg);
            $argUsed[$i+1]++;
            $i++;
         }
         else
            error("Error: Missing Command Line argument. Usage: -jSave <journal_name>");

      //--------------------
      case "-jPlay":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            playJournal($nextArg,false);
            $argUsed[$i+1]++;
            $i++;
         }
         else
            error("Error: Missing Command Line argument. Usage: -jPlay <journal_name>");

      //--------------------
      case "-jDebug":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            playJournal($nextArg,true);
            $argUsed[$i+1]++;
            $i++;
         }
         else
            error("Error: Missing Command Line argument. Usage: -jDebug <journal_name>");

      //--------------------
      case "-level":
         $argUsed[$i]++;
         if ($hasNextArg)
         {
            %hasExt = strpos($nextArg, ".mis");
            if(%hasExt == -1)
            {
               $levelToLoad = $nextArg @ " ";
               
               for(%i = $i + 2; %i < $Game::argc; %i++)
               {
                  %arg = $Game::argv[%i];
                  %hasExt = strpos(%arg, ".mis");
                  
                  if(%hasExt == -1)
                  {
                     $levelToLoad = $levelToLoad @ %arg @ " ";
                  } else
                  {
                     $levelToLoad = $levelToLoad @ %arg;
                     break;
                  }
               }
            } else
            $levelToLoad = $nextArg;
            $argUsed[$i+1]++;
            $i++;
         }
         else
            error("Error: Missing Command Line argument. Usage: -level <level file name (no path), with or without extension>");

      //-------------------
      case "-worldeditor":
         $startWorldEditor = true;
         $argUsed[$i]++;
         
      //-------------------
      case "-guieditor":
         $startGUIEditor = true;
         $argUsed[$i]++;

      //-------------------
      case "-help":
         $displayHelp = true;
         $argUsed[$i]++;

      //-------------------
      case "-compileAll":
         $compileAll = true;
         $argUsed[$i]++;
         
      //-------------------
      case "-compileTools":
         $compileTools = true;
         $argUsed[$i]++;
         
      //-------------------
      default:
         $argUsed[$i]++;
         if($userDirs $= "")
            $userDirs = $arg;
   }
}

if($dirCount == 0) {
      $userDirs = $defaultGame;
      $dirCount = 1;
}

//-----------------------------------------------------------------------------
// Display a splash window immediately to improve app responsiveness before
// engine is initialized and main window created
if (!$isDedicated)
   displaySplashWindow();

//-----------------------------------------------------------------------------
// The displayHelp, onStart, onExit and parseArgs function are overriden
// by mod packages to get hooked into initialization and cleanup.

function onStart()
{
   // Default startup function
}

function onExit()
{
   // OnExit is called directly from C++ code, whereas onStart is
   // invoked at the end of this file.
}

function parseArgs()
{
   // Here for mod override, the arguments have already
   // been parsed.
}

function compileFiles(%pattern)
{  
   %path = filePath(%pattern);

   %saveDSO    = $Scripts::OverrideDSOPath;
   %saveIgnore = $Scripts::ignoreDSOs;
   
   $Scripts::OverrideDSOPath  = %path;
   $Scripts::ignoreDSOs       = false;
   %mainCsFile = makeFullPath("main.cs");

   for (%file = findFirstFileMultiExpr(%pattern); %file !$= ""; %file = findNextFileMultiExpr(%pattern))
   {
      // we don't want to try and compile the primary main.cs
      if(%mainCsFile !$= %file)      
         compile(%file, true);
   }
   
   $Scripts::OverrideDSOPath  = %saveDSO;
   $Scripts::ignoreDSOs       = %saveIgnore;
   
}

if($compileAll)
{
   echo(" --- Compiling all files ---");
   compileFiles("/*.cs");
   compileFiles("/*.gui");
   compileFiles("/*.ts");  
   echo(" --- Exiting after compile ---");
   quit();
}

if($compileTools)
{
   echo(" --- Compiling tools scritps ---");
   compileFiles("tools/*.cs");
   compileFiles("tools/*.gui");
   compileFiles("tools/*.ts");  
   echo(" --- Exiting after compile ---");
   quit();
}

package Help {
   function onExit() {
      // Override onExit when displaying help
   }
};

function displayHelp() {
   activatePackage(Help);

      // Notes on logmode: console logging is written to console.log.
      // -log 0 disables console logging.
      // -log 1 appends to existing logfile; it also closes the file
      // (flushing the write buffer) after every write.
      // -log 2 overwrites any existing logfile; it also only closes
      // the logfile when the application shuts down.  (default)

   error(
      "Torque Demo command line options:\n"@
      "  -log <logmode>         Logging behavior; see main.cs comments for details\n"@
      "  -game <game_name>      Reset list of mods to only contain <game_name>\n"@
      "  <game_name>            Works like the -game argument\n"@
      "  -dir <dir_name>        Add <dir_name> to list of directories\n"@
      "  -console               Open a separate console\n"@
      "  -show <shape>          Deprecated\n"@
      "  -jSave  <file_name>    Record a journal\n"@
      "  -jPlay  <file_name>    Play back a journal\n"@
      "  -jDebug <file_name>    Play back a journal and issue an int3 at the end\n"@
      "  -help                  Display this help message\n"
   );
}


//--------------------------------------------------------------------------

// Default to a new logfile each session.
if( !$logModeSpecified )
{
   if( $platform !$= "xbox" && $platform !$= "xenon" )
      setLogMode(6);
}

// Get the first dir on the list, which will be the last to be applied... this
// does not modify the list.
nextToken($userDirs, currentMod, ";");

// Execute startup scripts for each mod, starting at base and working up
function loadDir(%dir)
{
   pushback($userDirs, %dir, ";");

   if (isFile(%dir @ "/main.cs"))
   exec(%dir @ "/main.cs");
}

echo("--------- Loading DIRS ---------");
function loadDirs(%dirPath)
{
   %dirPath = nextToken(%dirPath, token, ";");
   if (%dirPath !$= "")
      loadDirs(%dirPath);

   if(exec(%token @ "/main.cs") != true)
   {
      error("Error: Unable to find specified directory: " @ %token );
      $dirCount--;
   }
}
loadDirs($userDirs);
echo("");

if($dirCount == 0) {
   enableWinConsole(true);
   error("Error: Unable to load any specified directories");
   quit();
}
// Parse the command line arguments
echo("--------- Parsing Arguments ---------");
parseArgs();

// Either display the help message or startup the app.
if ($displayHelp) {
   enableWinConsole(true);
   displayHelp();
   quit();
}
else {
   onStart();
   echo("Engine initialized...");
}

// Display an error message for unused arguments
for ($i = 1; $i < $Game::argc; $i++)  {
   if (!$argUsed[$i])
      error("Error: Unknown command line argument: " @ $Game::argv[$i]);
}

// Automatically start up the appropriate eidtor, if any
if ($startWorldEditor) {
   Canvas.setCursor("DefaultCursor");
   Canvas.setContent(EditorChooseLevelGui);
} else if ($startGUIEditor) {
   Canvas.setCursor("DefaultCursor");
   Canvas.setContent(EditorChooseGUI);
}
