//-----------------------------------------------------------------------------
// Torque Game Engine Advanced
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------


///
/// This is used to register editor extensions and tools.
///
/// There are various callbacks you can overload to hook in your
/// own functionality without changing the core editor code.
///
/// At the moment this is primarily for the World/Mission 
/// Editor and the callbacks mostly make sense in that context.
///
/// Example:
///
///   %obj = new ScriptObject()
///   {
///      superclass = "EditorPlugin";
///      class = "RoadEditor";
///   };
///   
///   EditorPlugin::register( %obj );
///
/// For an a full example see: tools/roadEditor/main.cs
///                        or: tools/riverEditor/main.cs
///                        or: tools/decalEditor/main.cs
///

/// It is not intended for the user to overload this method.
/// If you do make sure you call the parent.
function EditorPlugin::onAdd( %this )
{
   EditorPluginSet.add( %this );   
}


/// Callback when the mission editor is first started.  It
/// is a good place to insert menus and menu items as well as 
/// preparing guis.
function EditorPlugin::onWorldEditorStartup( %this )
{
}

/// Callback right before the editor is opened.
function EditorPlugin::onEditorWake( %this )
{
}

/// Callback right before the editor is closed.
function EditorPlugin::onEditorSleep( %this )
{
}

/// Callback when the tool is 'activated' by the WorldEditor
/// Push Gui's, stuff like that
function EditorPlugin::onActivated( %this )
{
   if(isDemo())
      startToolTime(%this.getName());
}

/// Callback when the tool is 'deactivated' / closed by the WorldEditor
/// Pop Gui's, stuff like that
function EditorPlugin::onDeactivated( %this )
{
   if(isDemo())
      endToolTime(%this.getName());
}

/// Callback when tab is pressed.
/// Used by the WorldEditor to toggle between inspector/creator, for example.
function EditorPlugin::onToggleToolWindows( %this )
{
}

/// Callback when the edit menu is clicked.
/// This tools chance to fixing up the state of edit menu items. 
/// By default cut/copy/paste will be disabled. It is up to the active editor 
/// to determine if these are appropriate in the current state.
function EditorPlugin::onEditMenuSelect( %this, %editMenu )
{
   %editMenu.enableItem( 3, false ); // Cut
   %editMenu.enableItem( 4, false ); // Copy
   %editMenu.enableItem( 5, false ); // Paste  
   %editMenu.enableItem( 8, false ); // deselect     
}

/// If this tool keeps track of changes that necessitate resaving the mission
/// return true in that case.
function EditorPlugin::isDirty( %this )
{
   return false;  
}

/// This tools chance to clear whatever internal variables keep track of changes
/// since the last save.
function EditorPlugin::clearDirty( %this )
{
}

/// This tools chance to save data out when the mission is being saved.
/// This will only be called if the tool says it is dirty.
function EditorPlugin::onSaveMission( %this, %missionFile )
{
}

/// Callback when the escape key is pressed.
/// Return true if this tool has handled the key event in a custom way.
/// If false is returned the WorldEditor default behavior is to return
/// to the ObjectEditor.
function EditorPlugin::handleEscape( %this )
{
   return false;
}
