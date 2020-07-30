//-----------------------------------------------------------------------------
// Copyright (C) GarageGames
//-----------------------------------------------------------------------------

function initializeParticleEditor()
{
   echo(" % - Initializing Particle Editor");
     
   exec("./ParticleEditor.ed.gui");
   exec("./particleEditor.ed.cs");
   exec("./particleEditorUndo.ed.cs");   
   
   ParticleEditor.setVisible( false );
   PE_Window.setVisible( false );
   
   EditorGui.add( ParticleEditor );
   EditorGui.add( PE_Window );
   
   new ScriptObject( ParticleEditorPlugin )
   {
      superClass = "EditorPlugin";
   };
   
   %map = new ActionMap();
   ParticleEditorPlugin.map = %map;
   
}

function destroyParticleEditor()
{
}

function ParticleEditorPlugin::onWorldEditorStartup( %this )
{     
   // Add ourselves to the window menu.
   %accel = EditorGui.addToEditorsMenu( "Particle Editor", "", ParticleEditorPlugin );
   
   // Add ourselves to the ToolsToolbar
   %tooltip = "Particle Editor (" @ %accel @ ")";   
   EditorGui.addToToolsToolbar( "ParticleEditorPlugin", "ParticleEditorPalette", expandFilename("./particleeditor"), %tooltip );
}

function ParticleEditorPlugin::onActivated( %this )
{
   WorldEditorPlugin.onActivated();
   
   toggleParticleEditor();
   
   EditorGui.bringToFront( ParticleEditor );
   EditorGui-->WorldEditorToolbar.setVisible( true );
   ParticleEditor.setVisible( true );
   PE_Window.setVisible( true );
   ParticleEditor.makeFirstResponder( true );
   %this.map.push();
   
   
   // Set the status bar here
   EditorGuiStatusBar.setInfo("Particle editor.");
   EditorGuiStatusBar.setSelection("");
   
   Parent::onActivated(%this);
}

function ParticleEditorPlugin::onDeactivated( %this )
{   
   WorldEditorPlugin.onDeactivated();
   EditorGui-->WorldEditorToolbar.setVisible( false );
   ParticleEditor.setVisible( false );
   PE_Window.setVisible( false );
   
   toggleParticleEditor();
   $ParticleEditor::emitterNode.delete();   
   %this.map.pop(); 
   
   Parent::onDeactivated(%this);
}