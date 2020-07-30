//-----------------------------------------------------------------------------
// Copyright (C) GarageGames
//-----------------------------------------------------------------------------

function ParticleEditor::createUndo(%this, %class, %desc)
{
   pushInstantGroup();
   %action = new UndoScriptAction()
   {
      class = %class;
      superClass = BaseParticleEdAction;
      actionName = %desc;
   };
   popInstantGroup();
   return %action;
}

function ParticleEditor::submitUndo(%this, %action)
{
   if (%action.isMethod("redo"))
      %action.addToManager(EUndoManager);
}

function BaseParticleEdAction::redo(%this)
{
   %this.redo();
}

function BaseParticleEdAction::undo(%this)
{
}

// Create New Emitter

function ActionCreateNewEmitter::redo(%this)
{
   %id = UnlistedEmitters.getIndexFromValue( %this.newEmitter );
   UnlistedEmitters.erase( %id );
   
   // Drop it in the dropdown
   PEE_EmitterSelector_Control-->PopUpMenu.add( %this.newEmitter.getName());
   PEE_EmitterSelector_Control-->PopUpMenu.sort();
   
   // Select and Load it
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %this.newEmitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id );
   
   if( PE_Window.isVisible() )
      PE_EmitterEditor.loadNewEmitter();
}

function ActionCreateNewEmitter::undo(%this)
{
   UnlistedEmitters.add( "unlistedEmitters", %this.newEmitter );
   
   // Remove it from in the dropdown
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %this.newEmitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.clearEntry( %id );
   PEE_EmitterSelector_Control-->PopUpMenu.sort();
   
   // Select and Load it the old emitter
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %this.oldEmitter );
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id );
   
   if( PE_Window.isVisible() )
      PE_EmitterEditor.loadNewEmitter();
}

// Delete Emitter

function ActionDeleteEmitter::redo(%this)
{
   
   UnlistedEmitters.add( "unlistedEmitters", %this.oldEmitter );
   // check to see if its valid (not already been deleted and not editor used)
   if( %this.oldEmitterFname !$= "" && 
         %this.oldEmitterFname !$= "tools/particleEditor/particleEditor.ed.gui" )
      ParticleEditor_Emitter.removeObjectFromFile( %this.oldEmitter );
   
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %this.oldEmitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.clearEntry( %id );
   PEE_EmitterSelector_Control-->PopUpMenu.sort();
   
   // Select and Load it the old emitter
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %this.newEmitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id );
   
   if( PE_Window.isVisible() )
      PE_EmitterEditor.loadNewEmitter();
}

function ActionDeleteEmitter::undo(%this)
{
   %idx = UnlistedEmitters.getIndexFromValue( %this.oldEmitter.getName() );
   UnlistedEmitters.erase( %idx );
   
   if( %this.oldEmitterFname !$= "" && 
         %this.oldEmitterFname !$= "tools/particleEditor/particleEditor.ed.gui" )
   {
      ParticleEditor_Emitter.setDirty(%this.oldEmitter, %this.oldEmitterFname);
      ParticleEditor_Emitter.saveDirty();
      ParticleEditor_Emitter.removeDirty(%this.oldEmitter);
   }

   // Drop it in the dropdown
   PEE_EmitterSelector_Control-->PopUpMenu.add( %this.oldEmitter.getName());
   PEE_EmitterSelector_Control-->PopUpMenu.sort();
   
   // Select and Load it
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText(%this.oldEmitter.getName());
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id );
   
   if( PE_Window.isVisible() )
      PE_EmitterEditor.loadNewEmitter();
}

// Generic updateEmitter redo/undo

function ActionUpdateActiveEmitter::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      eval("PE_EmitterEditor.currEmitter." @ %this.field @ " = " @ %this.newValue @ ";");
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      eval("%this.emitter." @ %this.field @ " = " @ %this.newValue @ ";");
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitter::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      eval("PE_EmitterEditor.currEmitter." @ %this.field @ " = " @ %this.oldValue @ ";");
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();      
   }
   else
   {
      eval("%this.emitter." @ %this.field @ " = " @ %this.oldValue @ ";");
      %this.emitter.reload();
   }
}

// Special case updateEmitter redo/undo

function ActionUpdateActiveEmitterLifeFields::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.lifetimeMS = %this.newValueLifetimeMS;
      PE_EmitterEditor.currEmitter.lifetimeVarianceMS = %this.newValueLifetimeVarianceMS;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.lifetimeMS = %this.newValueLifetimeMS;
      %this.emitter.lifetimeVarianceMS = %this.newValueLifetimeVarianceMS;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterLifeFields::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.lifetimeMS = %this.oldValueLifetimeMS;
      PE_EmitterEditor.currEmitter.lifetimeVarianceMS = %this.oldValueLifetimeVarianceMS;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.lifetimeMS = %this.oldValueLifetimeMS;
      %this.emitter.lifetimeVarianceMS = %this.oldValueLifetimeVarianceMS;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterLifeFieldsInfiniteLoop::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.lifetimeMS = %this.newValueLifetimeMS;
      PE_EmitterEditor.currEmitter.lifetimeVarianceMS = %this.newValueLifetimeVarianceMS;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.lifetimeMS = %this.newValueLifetimeMS;
      %this.emitter.lifetimeVarianceMS = %this.newValueLifetimeVarianceMS;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterLifeFieldsInfiniteLoop::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.lifetimeMS = %this.oldValueLifetimeMS;
      PE_EmitterEditor.currEmitter.lifetimeVarianceMS = %this.oldValueLifetimeVarianceMS;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.lifetimeMS = %this.oldValueLifetimeMS;
      %this.emitter.lifetimeVarianceMS = %this.oldValueLifetimeVarianceMS;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterAmountFields::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.ejectionPeriodMS = %this.newValueEjectionPeriodMS;
      PE_EmitterEditor.currEmitter.periodVarianceMS = %this.newValuePeriodVarianceMS;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.ejectionPeriodMS = %this.newValueEjectionPeriodMS;
      %this.emitter.periodVarianceMS = %this.newValuePeriodVarianceMS;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterAmountFields::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.ejectionPeriodMS = %this.oldValueEjectionPeriodMS;
      PE_EmitterEditor.currEmitter.periodVarianceMS = %this.oldValuePeriodVarianceMS;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.ejectionPeriodMS = %this.oldValueEjectionPeriodMS;
      %this.emitter.periodVarianceMS = %this.oldValuePeriodVarianceMS;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterSpeedFields::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.ejectionVelocity = %this.newValueEjectionVelocity;
      PE_EmitterEditor.currEmitter.velocityVariance = %this.newValueVelocityVariance;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.ejectionVelocity = %this.newValueEjectionVelocity;
      %this.emitter.velocityVariance = %this.newValueVelocityVariance;
      %this.emitter.reload();
   }
}

function ActionUpdateActiveEmitterSpeedFields::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_EmitterEditor.currEmitter.ejectionVelocity = %this.oldValueEjectionVelocity;
      PE_EmitterEditor.currEmitter.velocityVariance = %this.oldValueVelocityVariance;
      PE_EmitterEditor.currEmitter.reload();
      PE_EmitterEditor.guiSync();
      PE_EmitterEditor.setEmitterDirty();
   }
   else
   {
      %this.emitter.ejectionVelocity = %this.oldValueEjectionVelocity;
      %this.emitter.velocityVariance = %this.oldValueVelocityVariance;
      %this.emitter.reload();
   }
}

// Create New Particle

function ActionCreateNewParticle::redo(%this)
{
   %id = UnlistedParticles.getIndexFromValue( %this.newParticle );
   UnlistedParticles.erase( %id );

   // Update the emitter the old fashion way
   PE_EmitterEditor.currEmitter.particles = PE_EmitterEditor.currEmitter.particles @ "\t" @ %this.newParticle;
   PE_EmitterEditor.currEmitter.reload();
   
   // Drop it in the dropdown
   PEP_ParticleSelector_Control-->PopUpMenu.add( %this.newParticle);
   PEP_ParticleSelector_Control-->PopUpMenu.sort();
   
   // going to have to change this as well, at the the emitter set
   // Select and Load it
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText(%this.newParticle.getName());
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id );
   
   // Add particle to dropdowns in the emitter editor
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      %emitterParticle-->PopUpMenu.add( %this.newParticle);
      %emitterParticle-->PopUpMenu.sort();
   }
   
   if( PE_Window.isVisible() )
      PE_ParticleEditor.loadNewParticle();
}

function ActionCreateNewParticle::undo(%this)
{
   UnlistedParticles.add( "unlistedParticles", %this.newParticle );
   
   // Removing the particle from the emitter
   for( %i = 0; %i < getWordCount( PE_EmitterEditor.currEmitter.particles ); %i++ )
   {
      if( getWord( PE_EmitterEditor.currEmitter.particles, %i ) $= %this.newParticle.getName() )
         break;
   }
   
   // Update the old fashion way
   PE_EmitterEditor.currEmitter.particles = removeWord( PE_EmitterEditor.currEmitter.particles, %i );
   PE_EmitterEditor.currEmitter.reload();
   
   // Remove it from in the dropdown
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %this.newParticle.getName() );
   PEP_ParticleSelector_Control-->PopUpMenu.clearEntry( %id );
   PEP_ParticleSelector_Control-->PopUpMenu.sort();
   
   // Select and Load it the old particle
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %this.oldParticle.getName() );
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id );
   
   // Add particle to dropdowns in the emitter editor
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      %id = %emitterParticle-->PopUpMenu.findText( %this.newParticle.getName() );
      %emitterParticle-->PopUpMenu.clearEntry( %id );
      %emitterParticle-->PopUpMenu.sort();
      
      %id = %emitterParticle-->PopUpMenu.findText( %this.oldParticle.getName() );
      %emitterParticle-->PopUpMenu.setSelected( %id );
   }
   
   if( PE_Window.isVisible() )
      PE_ParticleEditor.loadNewParticle();
}

// Delete Particle

function ActionDeleteParticle::redo(%this)
{
   UnlistedParticles.add( "unlistedParticles", %this.oldParticle );
   
   // Check to see if its valid (not already been deleted and not editor used)
   if( %this.oldParticle.getFileName() !$= "" && %this.oldParticle.getFilename() !$= "tools/particleEditor/particleEditor.ed.cs")
      ParticleEditor_Particle.removeObjectFromFile( %this.oldParticle );
   
   // Removing the particle from the popupMenu
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %this.oldParticle.getName() );
   PEP_ParticleSelector_Control-->PopUpMenu.clearEntry( %id );
   PEP_ParticleSelector_Control-->PopUpMenu.sort();
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected(0);
   
   // Removing the particle from the popupMenu in the emitterEditor
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      
      %id = %emitterParticle-->PopUpMenu.findText( %this.oldParticle.getName() );
      %emitterParticle-->PopUpMenu.clearEntry( %id );
   }
   
   // Removing the particle from the emitter
   for( %i = 0; %i < getWordCount( PE_EmitterEditor.currEmitter.particles ); %i++ )
   {
      if( getWord( PE_EmitterEditor.currEmitter.particles, %i ) $= %this.oldParticle.getName() )
         break;
   }
   PE_EmitterEditor.currEmitter.particles = %this.newParticleSet;
   PE_EmitterEditor.currEmitter.reload();
   
   // Setting up the popup menu with and loading a new particle 
   if( PE_Window.isVisible() )
      PE_ParticleEditor.loadNewParticle();
}

function ActionDeleteParticle::undo(%this)
{
   %idx = UnlistedParticles.getIndexFromValue( %this.oldParticle.getName() );
   UnlistedParticles.erase( %idx );
   
   if( %this.oldParticle.getFilename() !$= "" && 
         %this.oldParticle.getFilename() !$= "tools/particleEditor/particleEditor.ed.gui" )
   {
      ParticleEditor_Particle.setDirty(%this.oldParticle, %this.oldParticle.getFilename());
      ParticleEditor_Particle.saveDirty();
      ParticleEditor_Particle.removeDirty(%this.oldParticle.getFilename());
   }
   
   PE_EmitterEditor.currEmitter.particles = %this.oldParticleSet;
   PE_EmitterEditor.currEmitter.reload();
   
   // Drop it in the dropdown
   PEP_ParticleSelector_Control-->PopUpMenu.add( %this.oldParticle);
   PEP_ParticleSelector_Control-->PopUpMenu.sort();
   
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %this.oldParticle.getName() );
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id );
   
   // Add particle to dropdowns in the emitter editor
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      %emitterParticle-->PopUpMenu.add( %this.oldParticle);
      %emitterParticle-->PopUpMenu.sort();
   }
   
   if( PE_Window.isVisible() )
      PE_ParticleEditor.loadNewParticle();
}

// Generic updateParticle redo/undo

function ActionUpdateActiveParticle::redo(%this)
{
   if( PE_Window.isVisible() )
   {      
      eval("PE_ParticleEditor.currParticle." @ %this.field @ " = " @ %this.newValue @ ";");
      PE_ParticleEditor.currParticle.reload();
      PE_ParticleEditor.guiSync();
      PE_ParticleEditor.setParticleDirty();      
   }
   else
   {
      eval("%this.particle." @ %this.field @ " = " @ %this.newValue @ ";");
      %this.particle.reload();
   }
}

function ActionUpdateActiveParticle::undo(%this)
{
   if( PE_Window.isVisible() )
   {      
      eval("PE_ParticleEditor.currParticle." @ %this.field @ " = " @ %this.oldValue @ ";");
      PE_ParticleEditor.currParticle.reload();
      PE_ParticleEditor.guiSync();
      PE_ParticleEditor.setParticleDirty();      
   }
   else
   {
      eval("%this.particle." @ %this.field @ " = " @ %this.oldValue @ ";");
      %this.particle.reload();
   }
}

// Special case updateParticle redo/undo

function ActionUpdateActiveParticleLifeFields::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_ParticleEditor.currParticle.lifetimeMS = %this.newValueLifetimeMS;
      PE_ParticleEditor.currParticle.lifetimeVarianceMS = %this.newValueLifetimeVarianceMS;
      PE_ParticleEditor.currParticle.reload();
      PE_ParticleEditor.guiSync();
      PE_ParticleEditor.setParticleDirty();
   }
   else
   {
      %this.particle.lifetimeMS = %this.newValueLifetimeMS;
      %this.particle.lifetimeVarianceMS = %this.newValueLifetimeVarianceMS;
      %this.particle.reload();
   }
}

function ActionUpdateActiveParticleLifeFields::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_ParticleEditor.currParticle.lifetimeMS = %this.oldValueLifetimeMS;
      PE_ParticleEditor.currParticle.lifetimeVarianceMS = %this.oldValueLifetimeVarianceMS;
      PE_ParticleEditor.currParticle.reload();
      PE_ParticleEditor.guiSync();
      PE_ParticleEditor.setParticleDirty();
   }
   else
   {
      %this.particle.lifetimeMS = %this.oldValueLifetimeMS;
      %this.particle.lifetimeVarianceMS = %this.oldValueLifetimeVarianceMS;
      %this.particle.reload();
   }
}

function ActionUpdateActiveParticleSpinFields::redo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_ParticleEditor.currParticle.spinRandomMax = %this.newValueSpinRandomMax;
      PE_ParticleEditor.currParticle.spinRandomMin = %this.newValueSpinRandomMin;
      PE_ParticleEditor.currParticle.reload();
      PE_ParticleEditor.guiSync();
      PE_ParticleEditor.setParticleDirty();
   }
   else
   {
      %this.particle.spinRandomMax = %this.newValueSpinRandomMax;
      %this.particle.spinRandomMin = %this.newValueSpinRandomMin;
      %this.particle.reload();
   }
}

function ActionUpdateActiveParticleSpinFields::undo(%this)
{
   if( PE_Window.isVisible() )
   {
      PE_ParticleEditor.currParticle.spinRandomMax = %this.oldValueSpinRandomMax;
      PE_ParticleEditor.currParticle.spinRandomMin = %this.oldValueSpinRandomMin;
      PE_ParticleEditor.currParticle.reload();
      PE_ParticleEditor.guiSync();
      PE_ParticleEditor.setParticleDirty();
   }
   else
   {
      %this.particle.spinRandomMax = %this.oldValueSpinRandomMax;
      %this.particle.spinRandomMin = %this.oldValueSpinRandomMin;
      %this.particle.reload();
   }
}
