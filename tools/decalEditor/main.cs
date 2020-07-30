//-----------------------------------------------------------------------------
// Copyright (C) Sickhead Games, LLC
//-----------------------------------------------------------------------------

function initializeDecalEditor()
{
   echo(" % - Initializing Decal Editor");
   
   $decalDataFile = "art/decals/managedDecalData.cs";
     
   exec( "./decalEditor.cs" );
   exec( "./decalEditorGui.gui" );
   exec( "./decalEditorGui.cs" );
   exec( "./decalEditorActions.cs" );
   
   // Add ourselves to EditorGui, where all the other tools reside
   DecalEditorGui.setVisible( false ); 
   DecalPreviewWindow.setVisible( false );  
   DecalEditorWindow.setVisible( false );
   EditorGui.add( DecalEditorGui );
   EditorGui.add( DecalEditorWindow );
   EditorGui.add( DecalPreviewWindow );
   DecalEditorTabBook.selectPage( 0 );
   
   new ScriptObject( DecalEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   %map = new ActionMap();   
   %map.bindCmd( keyboard, "backspace", "DecalEditorGui.onDeleteKey();", "" );   
   %map.bindCmd( keyboard, "delete", "DecalEditorGui.onDeleteKey();", "" );  
   %map.bindCmd( keyboard, "5", "EDecalEditorAddDecalBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "1", "EDecalEditorSelectDecalBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "2", "EDecalEditorMoveDecalBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "3", "EDecalEditorRotateDecalBtn.performClick();", "" );  
   %map.bindCmd( keyboard, "4", "EDecalEditorScaleDecalBtn.performClick();", "" );
   
   DecalEditorPlugin.map = %map;
   
   new PersistenceManager( DecalPMan );  
    
}

function destroyDecalEditor()
{
}

// JCF: helper for during development
function reinitDecalEditor()
{
   exec( "./main.cs" );
   exec( "./decalEditor.cs" );
   exec( "./decalEditorGui.cs" );
}

function DecalEditorPlugin::onWorldEditorStartup( %this )
{      
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Decal Editor", "", DecalEditorPlugin );   
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Decal Editor (" @ %accel @ ")";   
   EditorGui.addToToolsToolbar( "DecalEditorPlugin", "DecalEditorPalette", expandFilename("tools/decalEditor/decal-editor"), %tooltip );

   //connect editor windows   
   AttachWindows( DecalPreviewWindow, DecalEditorWindow );
   
   //set initial palette setting
   %this.paletteSelection = "AddDecalMode";
}

function DecalEditorPlugin::onActivated( %this )
{   
   EditorGui.bringToFront( DecalEditorGui );
   DecalEditorGui.setVisible( true );
   DecalEditorGui.makeFirstResponder( true );
   DecalPreviewWindow.setVisible( true );
   DecalEditorWindow.setVisible( true );
   
   %this.map.push();
   
   //WORKAROUND: due to the gizmo mode being stored on its profile (which may be shared),
   //  we may end up with a mismatch between the editor mode and gizmo mode here.
   //  Reset mode explicitly here to work around this.
   DecalEditorGui.setMode( DecalEditorGui.getMode() );
   
   // Set the current palette selection
   DecalEditorGui.paletteSync( %this.paletteSelection );
   
   // Store this on a dynamic field
   // in order to restore whatever setting
   // the user had before.
   %this.prevGizmoAlignment = GlobalGizmoProfile.alignment;
   
   // The DecalEditor always uses Object alignment.
   GlobalGizmoProfile.alignment = "Object";
   
   // Initialize the instance tree when the tab is selected
   DecalEditorTreeView.removeItem(0);
   %rootId = DecalEditorTreeView.insertItem(0, "<root>", 0, "");
   %count = DecalEditorGui.getDecalCount();
   for (%i = 0; %i < %count; %i++)
   {
      %name = DecalEditorGui.getDecalLookupName(%i);
      if( %name $= "invalid" )
         continue;
         
      DecalEditorTreeView.addNodeTree(%i, %name);
   }
   
   // These could perhaps be the node details like the shape editor
   //ShapeEdPropWindow.syncNodeDetails(-1);
   
   Parent::onActivated(%this);
}

function DecalEditorPlugin::onDeactivated( %this )
{   
   DecalEditorGui.setVisible(false);
   DecalPreviewWindow.setVisible( false );
   DecalEditorWindow.setVisible( false );
   
   %this.map.pop();
   
   // Remember last palette selection
   %this.paletteSelection = DecalEditorGui.getMode();
   
   // Restore the previous Gizmo
   // alignment settings.
   GlobalGizmoProfile.alignment = %this.prevGizmoAlignment; 
   
   Parent::onDeactivated(%this);  
}

function DecalEditorPlugin::handleEscape( %this )
{
   // JCF: implement this.
   //return DecalEditorGui.onEscapePressed();  
}

function DecalEditorPlugin::isDirty( %this )
{
   %dirty = DecalPMan.hasDirty();
   
   %dirty |= decalManagerDirty();
      
   return %dirty;
}

function DecalEditorPlugin::onSaveMission( %this, %file )
{   
   DecalPMan.saveDirty();
   decalManagerSave( %file @ ".decals" );
}

