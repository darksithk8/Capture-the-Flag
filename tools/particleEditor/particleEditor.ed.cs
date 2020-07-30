//-----------------------------------------------------------------------------
// Copyright (C) GarageGames
//-----------------------------------------------------------------------------

// Open the particle editor to spawn a test emitter in front of the player.
// Edit the sliders, check boxes, and text fields and see the results in
// realtime.  Switch between emitters and particles with the buttons in the
// top left corner.  When in particle mode, the only particles available will
// be those assigned to the current emitter to avoid confusion.  In the top
// right corner, there is a button marked "Drop Emitter", which will spawn the
// test emitter in front of the player again, and a button marked "Restart
// Emitter", which will play the particle animation again.

$ParticleEditorInitialized = false;

function toggleParticleEditor()
{
   if ($ParticleEditor::isOpen)
   {
      
      $ParticleEditor::isOpen = false;
      return;
   }

   if (!$ParticleEditorInitialized)
   {
	   $ParticleEditorInitialized = true;
      ParticleEditor.initEditor();
   }
   
   ParticleEditor.startup();
   
   $ParticleEditor::isOpen = true;
}

function ParticleEditor::startup(%this)
{
   if (!isObject($ParticleEditor::emitterNode))
      %this.resetEmitterNode();
   
   PE_EmitterEditor.guiSync();
   PE_EmitterEditor.setEmitterNotDirty();
   
   // this needs to be better done with the currently selected emitter datablock
   PE_EmitterEditor.copyEmitters( PE_EmitterEditor.currEmitter, PE_EmitterEditor_NotDirtyEmitter );
}

function ParticleEditor::initEditor(%this)
{
   echo("Initializing ParticleEmitterData and ParticleData DataBlocks...");
   new PersistenceManager(ParticleEditor_Emitter);
   new PersistenceManager(ParticleEditor_Particle);

   datablock ParticleEmitterData(PE_EmitterEditor_NotDirtyEmitter)
   {
      particles = "RocketProjSmokeTrail";
   };
   datablock ParticleData(PE_ParticleEditor_NotDirtyParticle)
   {
      textureName = "art/shapes/particles/smoke";
   };
   
   new ArrayObject(UnlistedEmitters);
   UnlistedEmitters.add( "unlistedEmitters", PE_EmitterEditor_NotDirtyEmitter );
   
   new ArrayObject(UnlistedParticles);
   UnlistedEmitters.add( "unlistedParticles", PE_ParticleEditor_NotDirtyParticle );
   
   PEE_EmitterSelector.clear();
   PEE_EmitterParticleSelector1.clear();
   PEE_EmitterParticleSelector2.clear();
   PEE_EmitterParticleSelector3.clear();
   PEE_EmitterParticleSelector4.clear();
   
   PEP_ParticleSelector.clear();
   
   %emitterCount = 0;
   %particleCount = 0;
   
   %count = DatablockGroup.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %obj = DatablockGroup.getObject(%i);
      if (%obj.getClassName() $= "ParticleEmitterData")
      {
         for( %k = 0; %k < UnlistedEmitters.count(); %k++ )
         {
            %unlistedFound = 0;
            if( UnlistedEmitters.getValue(%k) $= %obj.getName )
            {
               %unlistedFound = 1;
               break;
            }
         }
      
         if( %unlistedFound )
            continue;
         
         PEE_EmitterSelector.add(%obj.getName());
         %emitterCount++;
      }
      if (%obj.getClassName() $= "ParticleData")
      {
         PEE_EmitterParticleSelector1.add(%obj.getName());
         PEE_EmitterParticleSelector2.add(%obj.getName());
         PEE_EmitterParticleSelector3.add(%obj.getName());
         PEE_EmitterParticleSelector4.add(%obj.getName());
      }
   }

   echo("Found" SPC %emitterCount SPC "emitters and" SPC %particleCount SPC "particles.");
   PEE_EmitterSelector.sort();
   
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText("DefaultEmitter");
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id );
   
   PE_EmitterEditor.loadNewEmitter();
   
   PE_EmitterEditor.copyEmitters( PE_EmitterEditor.currEmitter, PE_EmitterEditor_NotDirtyEmitter );
   PE_EmitterEditor_NotDirtyEmitter.originalName = PE_EmitterEditor.currEmitter.getName();
   
   PEE_EmitterParticleSelector2.add("None");
   PEE_EmitterParticleSelector3.add("None");
   PEE_EmitterParticleSelector4.add("None");
   
   PEE_EmitterParticleSelector1.sort();
   PEE_EmitterParticleSelector2.sort();
   PEE_EmitterParticleSelector3.sort();
   PEE_EmitterParticleSelector4.sort();
         
   PE_EmitterEditor-->PEE_blendType.clear();
   PE_EmitterEditor-->PEE_blendType.add("NORMAL",0);
   PE_EmitterEditor-->PEE_blendType.add("ADDITIVE",1);
   PE_EmitterEditor-->PEE_blendType.add("SUBTRACTIVE",2);
   PE_EmitterEditor-->PEE_blendType.add("PREMULTALPHA",3);
   
   PE_Window-->EditorTabBook.selectPage( 0 );
 }

function ParticleEditor::openEmitterPane(%this)
{
   PE_Window.text = "Particle Editor - Emitters";
   PE_EmitterEditor.guiSync();
   ParticleEditor.activeEditor = PE_EmitterEditor;
   
   if( !PE_EmitterEditor.dirty )
      PE_EmitterEditor.setEmitterNotDirty();
}

function ParticleEditor::openParticlePane(%this)
{
   PE_Window.text = "Particle Editor - Particles";
   
   PE_ParticleEditor.guiSync();
   ParticleEditor.activeEditor = PE_ParticleEditor;
   
   if( !PE_ParticleEditor.dirty )
      PE_ParticleEditor.setParticleNotDirty();
}

function ParticleEditor::resetEmitterNode(%this)
{
   %tform = ServerConnection.getControlObject().getEyeTransform();
   %vec = VectorNormalize(ServerConnection.getControlObject().getForwardVector());
   %vec = VectorScale(%vec, 4);
   %tform = setWord(%tform, 0, getWord(%tform, 0) + getWord(%vec, 0));
   %tform = setWord(%tform, 1, getWord(%tform, 1) + getWord(%vec, 1));
   %tform = setWord(%tform, 2, getWord(%tform, 2) + getWord(%vec, 2));

   if (!isObject($ParticleEditor::emitterNode))
   {
      if (!isObject(TestEmitterNodeData))
      {
         datablock ParticleEmitterNodeData(TestEmitterNodeData)
         {
            timeMultiple = 1;
         };
      }

      $ParticleEditor::emitterNode = new ParticleEmitterNode()
      {
         emitter = PEE_EmitterSelector.getText();
         velocity = 1;
         position = getWords(%tform, 0, 2);
         rotation = getWords(%tform, 3, 6);
         datablock = TestEmitterNodeData;
         parentGroup = MissionGroup;
      };
      //grab the client-side emitter node so we can reload the emitter datablock
      $ParticleEditor::clientEmitterNode = $ParticleEditor::emitterNode+1;
   }
   else
   {
      $ParticleEditor::emitterNode.setTransform(%tform);
      $ParticleEditor::clientEmitterNode.setTransform(%tform);
      ParticleEditor.updateEmitterNode();
   }
}

function ParticleEditor::updateEmitterNode()
{
   %id = PEE_EmitterSelector_Control-->PopUpMenu.getSelected();
   %emitter = PEE_EmitterSelector_Control-->PopUpMenu.getTextById( %id );
   if( isObject($ParticleEditor::clientEmitterNode) )
   {
      $ParticleEditor::clientEmitterNode.setEmitterDataBlock(%emitter.getId());
   }
}

function PE_TabBook::onTabSelected( %this, %text, %idx )
{
   if( %idx == 0)
      ParticleEditor.openEmitterPane();
   else
      ParticleEditor.openParticlePane();
}

//------------------------------------------------------------------------------
// Particle Emitter functionality

function PE_EmitterEditor::guiSync( %this )
{
   %data = PE_EmitterEditor.currEmitter;
   
   //temporary name 
   PEE_EmitterSelector_Control-->textEdit.setText( %data );
   
   if( %data.lifetimeMS == 0 )
   {
      PE_EmitterEditor-->PEE_infiniteLoop.setValue( true );
      PE_EmitterEditor-->PEE_lifetimeMS_slider.setValue( 0 );
      PE_EmitterEditor-->PEE_lifetimeMS_textEdit.setText( 0 );
   }
   else
   {
      PE_EmitterEditor-->PEE_infiniteLoop.setValue( false );
      PE_EmitterEditor-->PEE_lifetimeMS_slider.setValue( %data.lifetimeMS );
      PE_EmitterEditor-->PEE_lifetimeMS_textEdit.setText( %data.lifetimeMS );
   }
   
   PE_EmitterEditor-->PEE_lifetimeVarianceMS_slider.setValue( %data.lifetimeVarianceMS );
   PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.setText( %data.lifetimeVarianceMS );
      
   PE_EmitterEditor-->PEE_ejectionPeriodMS_slider.setValue( %data.ejectionPeriodMS );
   PE_EmitterEditor-->PEE_ejectionPeriodMS_textEdit.setText( %data.ejectionPeriodMS );

   PE_EmitterEditor-->PEE_periodVarianceMS_slider.setValue( %data.periodVarianceMS );
   PE_EmitterEditor-->PEE_periodVarianceMS_textEdit.setText( %data.periodVarianceMS );
   
   PE_EmitterEditor-->PEE_ejectionVelocity_slider.setValue( %data.ejectionVelocity );
   PE_EmitterEditor-->PEE_ejectionVelocity_textEdit.setText( %data.ejectionVelocity );
   
   PE_EmitterEditor-->PEE_velocityVariance_slider.setValue( %data.velocityVariance );
   PE_EmitterEditor-->PEE_velocityVariance_textEdit.setText( %data.velocityVariance );
   
   PE_EmitterEditor-->PEE_orientParticles.setValue( %data.orientParticles );
   PE_EmitterEditor-->PEE_alignParticles.setValue( %data.alignParticles );
   PE_EmitterEditor-->PEE_alignDirection.setText( %data.alignDirection );
   
   PE_EmitterEditor-->PEE_thetaMin_slider.setValue( %data.thetaMin );
   PE_EmitterEditor-->PEE_thetaMin_textEdit.setText( %data.thetaMin );
   
   PE_EmitterEditor-->PEE_thetaMax_slider.setValue( %data.thetaMax );
   PE_EmitterEditor-->PEE_thetaMax_textEdit.setText( %data.thetaMax );
   
   PE_EmitterEditor-->PEE_phiVariance_slider.setValue( %data.phiVariance );
   PE_EmitterEditor-->PEE_phiVariance_textEdit.setText( %data.phiVariance );
   
   PE_EmitterEditor-->PEE_ejectionOffset_slider.setValue( %data.ejectionOffset );
   PE_EmitterEditor-->PEE_ejectionOffset_textEdit.setText( %data.ejectionOffset );
   
   // turn this dirty thing into a loop
   %particle = "";
   %bitmap = "";
   
   %particle = getWord(%data.particles, 0);
   if( isObject( %particle ) )
   {
      //set dropdown      
      %particleId = PEE_EmitterParticle1-->PopUpMenu.findText( %particle.getName() );
      PEE_EmitterParticle1-->PopUpMenu.setSelected( %particleId );
            
      %particle = "";
      %bitmap = "";
   }
   else
   {
      %particleId = PEE_EmitterParticle1-->PopUpMenu.findText( "None" );
      PEE_EmitterParticle1-->PopUpMenu.setSelected( %particleId );
   }
   
   %particle = getWord(%data.particles, 1);
   if( isObject( %particle ) )
   {
      %particleId = PEE_EmitterParticle2-->PopUpMenu.findText( %particle.getName() );
      PEE_EmitterParticle2-->PopUpMenu.setSelected( %particleId );
      
      %particle = "";
      %bitmap = "";
   }
   else
   {
      %particleId = PEE_EmitterParticle2-->PopUpMenu.findText( "None" );
      PEE_EmitterParticle2-->PopUpMenu.setSelected( %particleId );
   }
   
   %particle = getWord(%data.particles, 2);
   if( isObject( %particle ) )
   {
      %particleId = PEE_EmitterParticle3-->PopUpMenu.findText( %particle.getName() );
      PEE_EmitterParticle3-->PopUpMenu.setSelected( %particleId );
      
      %particle = "";
      %bitmap = "";
   }
   else
   {
      %particleId = PEE_EmitterParticle3-->PopUpMenu.findText( "None" );
      PEE_EmitterParticle3-->PopUpMenu.setSelected( %particleId );
   }
   
   %particle = getWord(%data.particles, 3);
   if( isObject( %particle ) )
   {
      %particleId = PEE_EmitterParticle4-->PopUpMenu.findText( %particle.getName() );
      PEE_EmitterParticle4-->PopUpMenu.setSelected( %particleId );
      
      %particle = "";
      %bitmap = "";
   }
   else
   {
      %particleId = PEE_EmitterParticle4-->PopUpMenu.findText( "None" );
      PEE_EmitterParticle4-->PopUpMenu.setSelected( %particleId );
   }
   
   %blendTypeId = PE_EmitterEditor-->PEE_blendType.findText( %data.blendStyle );
   PE_EmitterEditor-->PEE_blendType.setSelected( %blendTypeId );
   
   PE_EmitterEditor-->PEE_softnessDistance_slider.setValue( %data.softnessDistance );
   PE_EmitterEditor-->PEE_softnessDistance_textEdit.setText( %data.softnessDistance );
   
   PE_EmitterEditor-->PEE_ambientFactor_slider.setValue( %data.ambientFactor );
   PE_EmitterEditor-->PEE_ambientFactor_textEdit.setText( %data.ambientFactor );
   
   PE_EmitterEditor-->PEE_softParticles.setValue( %data.softParticles );
   PE_EmitterEditor-->PEE_reverseOrder.setValue( %data.reverseOrder );
   PE_EmitterEditor-->PEE_useEmitterSizes.setValue( %data.useEmitterSizes );
   PE_EmitterEditor-->PEE_useEmitterColors.setValue( %data.useEmitterColors );
}

// Generic updateEmitter method
function PE_EmitterEditor::updateEmitter(%this, %propertyField, %value, %isSlider, %onMouseUp)
{
   PE_EmitterEditor.setEmitterDirty();
   
   if(%value $= "")
      %value = "\"\"";
      
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.field = %propertyField;
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValue = %value;
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveEmitter, "Update Active Emitter");
      %action.emitter = PE_EmitterEditor.currEmitter;
      %action.field = %propertyField;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      %action.newValue = %value;
      eval( "%action.oldValue = " @ PE_EmitterEditor.currEmitter @ "." @ %propertyField @ ";");
      %action.oldValue = "\"" @ %action.oldValue @ "\"";
      
      ParticleEditor.submitUndo( %action );
   }
   
	eval("PE_EmitterEditor.currEmitter." @ %propertyField @ " = " @ %value @ ";");
   PE_EmitterEditor.currEmitter.reload();
}

// Special case updateEmitter methods
function PE_EmitterEditor::updateLifeFields( %this, %isRandom, %value, %isSlider, %onMouseUp )
{
   PE_EmitterEditor.setEmitterDirty();
   
   if(%value $= "")
      %value = "\"\"";
   
   // Transfer values over to gui controls
   if( %isRandom )
   {
      if( %value > 0 )
         %value++;
          
      if( %value > PE_EmitterEditor-->PEE_lifetimeMS_slider.getValue() )
      {
         PE_EmitterEditor-->PEE_lifetimeMS_textEdit.setText( %value );
         PE_EmitterEditor-->PEE_lifetimeMS_slider.setValue( %value );
      }
   }
   else
   {
      if( %value > 0 )
         %value--;
         
      if( %value < PE_EmitterEditor-->PEE_lifetimeVarianceMS_slider.getValue() )
      {
         PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.setText( %value );
         PE_EmitterEditor-->PEE_lifetimeVarianceMS_slider.setValue( %value );
      }
   }
   
   // Logic for storing undo values
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValueLifetimeMS = PE_EmitterEditor-->PEE_lifetimeMS_textEdit.getText();
      %last.newValueLifetimeVarianceMS = PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.getText();
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveEmitterLifeFields, "Update Active Emitter");
      %action.emitter = PE_EmitterEditor.currEmitter;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      
      %action.newValueLifetimeMS = PE_EmitterEditor-->PEE_lifetimeMS_textEdit.getText();
      %action.oldValueLifetimeMS = PE_EmitterEditor.currEmitter.lifetimeMS;
      
      %action.newValueLifetimeVarianceMS = PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.getText();
      %action.oldValueLifetimeVarianceMS = PE_EmitterEditor.currEmitter.lifetimeVarianceMS;
      
      ParticleEditor.submitUndo( %action );
   }
   
   // Set the values on the current emitter
   PE_EmitterEditor.currEmitter.lifetimeMS = PE_EmitterEditor-->PEE_lifetimeMS_textEdit.getText();
   PE_EmitterEditor.currEmitter.lifetimeVarianceMS = PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.getText();
   PE_EmitterEditor.currEmitter.reload();
   
   // Keep the infiniteLoop checkbox up to date
   if( PE_EmitterEditor.currEmitter.getFieldValue("lifetimeMS") == 0 &&
         PE_EmitterEditor.currEmitter.getFieldValue("lifetimeVarianceMS") == 0)
   {
      PE_EmitterEditor-->PEE_infiniteLoop.setStateOn(1);
   }
   else
   {
      PE_EmitterEditor-->PEE_infiniteLoop.setStateOn(0);
   }
}

function PE_EmitterEditor::updateLifeFieldsInfiniteLoop( %this )
{
   if( PE_EmitterEditor-->PEE_infiniteLoop.getValue() != 1 )
      return;
      
   PE_EmitterEditor-->PEE_lifetimeMS_textEdit.setText( 0 );
   PE_EmitterEditor-->PEE_lifetimeMS_slider.setValue( 0 );
   PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.setText( 0 );
   PE_EmitterEditor-->PEE_lifetimeVarianceMS_slider.setValue( 0 );
   
   
   %action = ParticleEditor.createUndo(ActionUpdateActiveEmitterLifeFieldsInfiniteLoop, "Update Active Emitter");
   %action.emitter = PE_EmitterEditor.currEmitter;
      
   %action.newValueLifetimeMS = PE_EmitterEditor-->PEE_lifetimeMS_textEdit.getText();
   %action.oldValueLifetimeMS = PE_EmitterEditor.currEmitter.lifetimeMS;
   
   %action.newValueLifetimeVarianceMS = PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.getText();
   %action.oldValueLifetimeVarianceMS = PE_EmitterEditor.currEmitter.lifetimeVarianceMS;
      
   ParticleEditor.submitUndo( %action );

   // Set the values on the current emitter
   PE_EmitterEditor.currEmitter.lifetimeMS = PE_EmitterEditor-->PEE_lifetimeMS_textEdit.getText();
   PE_EmitterEditor.currEmitter.lifetimeVarianceMS = PE_EmitterEditor-->PEE_lifetimeVarianceMS_textEdit.getText();
   PE_EmitterEditor.currEmitter.reload();         
}

function PE_EmitterEditor::updateAmountFields( %this, %isRandom, %value, %isSlider, %onMouseUp )
{
   PE_EmitterEditor.setEmitterDirty();
   
   if(%value $= "")
      %value = "\"\"";
   
   // Transfer values over to gui controls
   if( %isRandom )
   {
      %value++;
      if( %value > PE_EmitterEditor-->PEE_ejectionPeriodMS_slider.getValue() )
      {
         PE_EmitterEditor-->PEE_ejectionPeriodMS_textEdit.setText( %value );
         PE_EmitterEditor-->PEE_ejectionPeriodMS_slider.setValue( %value );
      }
   }
   else
   {
      
      %value--;
      if( %value < PE_EmitterEditor-->PEE_periodVarianceMS_slider.getValue() )
      {
         PE_EmitterEditor-->PEE_periodVarianceMS_textEdit.setText( %value );
         PE_EmitterEditor-->PEE_periodVarianceMS_slider.setValue( %value );
      }
   }
   
   // Logic for storing undo values
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValueEjectionPeriodMS = PE_EmitterEditor-->PEE_ejectionPeriodMS_textEdit.getText();
      %last.newValuePeriodVarianceMS = PE_EmitterEditor-->PEE_periodVarianceMS_textEdit.getText();
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveEmitterAmountFields, "Update Active Emitter");
      %action.emitter = PE_EmitterEditor.currEmitter;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      
      %action.newValueEjectionPeriodMS = PE_EmitterEditor-->PEE_ejectionPeriodMS_textEdit.getText();
      %action.oldValueEjectionPeriodMS = PE_EmitterEditor.currEmitter.ejectionPeriodMS;
      
      %action.newValuePeriodVarianceMS = PE_EmitterEditor-->PEE_periodVarianceMS_textEdit.getText();
      %action.oldValuePeriodVarianceMS = PE_EmitterEditor.currEmitter.periodVarianceMS;
      
      ParticleEditor.submitUndo( %action );
   }
   
   // Set the values on the current emitter
   PE_EmitterEditor.currEmitter.ejectionPeriodMS = PE_EmitterEditor-->PEE_ejectionPeriodMS_textEdit.getText();
   PE_EmitterEditor.currEmitter.periodVarianceMS = PE_EmitterEditor-->PEE_periodVarianceMS_textEdit.getText();
   PE_EmitterEditor.currEmitter.reload();
}

function PE_EmitterEditor::updateSpeedFields( %this, %isRandom, %value, %isSlider, %onMouseUp )
{
   PE_EmitterEditor.setEmitterDirty();
   
   if(%value $= "")
      %value = "\"\"";
   
   // Transfer values over to gui controls
   if( %isRandom )
   {
      %value++;
      if( %value > PE_EmitterEditor-->PEE_ejectionVelocity_slider.getValue() )
      {
         PE_EmitterEditor-->PEE_ejectionVelocity_textEdit.setText( %value );
         PE_EmitterEditor-->PEE_ejectionVelocity_slider.setValue( %value );
      }
   }
   else
   {
      %value--;
      if( %value < PE_EmitterEditor-->PEE_velocityVariance_slider.getValue() )
      {
         PE_EmitterEditor-->PEE_velocityVariance_textEdit.setText( %value );
         PE_EmitterEditor-->PEE_velocityVariance_slider.setValue( %value );
      }
   }
   
   // Logic for storing undo values
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValueEjectionVelocity = PE_EmitterEditor-->PEE_ejectionVelocity_textEdit.getText();
      %last.newValueVelocityVariance = PE_EmitterEditor-->PEE_velocityVariance_textEdit.getText();
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveEmitterSpeedFields, "Update Active Emitter");
      %action.emitter = PE_EmitterEditor.currEmitter;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      
      %action.newValueEjectionVelocity = PE_EmitterEditor-->PEE_ejectionVelocity_textEdit.getText();
      %action.oldValueEjectionVelocity = PE_EmitterEditor.currEmitter.ejectionVelocity;
      
      %action.newValueVelocityVariance = PE_EmitterEditor-->PEE_velocityVariance_textEdit.getText();
      %action.oldValueVelocityVariance = PE_EmitterEditor.currEmitter.velocityVariance;
      
      ParticleEditor.submitUndo( %action );
   }
   
   // Set the values on the current emitter
   PE_EmitterEditor.currEmitter.ejectionVelocity = PE_EmitterEditor-->PEE_ejectionVelocity_textEdit.getText();
   PE_EmitterEditor.currEmitter.velocityVariance = PE_EmitterEditor-->PEE_velocityVariance_textEdit.getText();
   PE_EmitterEditor.currEmitter.reload();
}

function PE_EmitterEditor::updateParticlesFields( %this )
{
   %particles = "";
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      if( %emitterParticle-->PopUpMenu.getText() $= "" || %emitterParticle-->PopUpMenu.getText() $= "None")
         continue;
      
      if( %particles $= "" )
         %particles = %emitterParticle-->PopUpMenu.getText();
      else
         %particles = %particles @ " TAB " @ %emitterParticle-->PopUpMenu.getText();
   }
   
   %changedEditParticle = 1;
   %count = getWordCount(%particles);
   for(%i = 0; %i < %count; %i++) 
   {
      %particleName = getWord(%particles, %i);
      if( %particleName $= PE_ParticleEditor.currParticle.getName() && PE_ParticleEditor.currParticle.getName() !$= "" )
      {
         %changedEditParticle = 0; 
         break;
      }
   }
   
   // True only if the currently edited particle has not been found and the 
   // ParticleEditor is dirty
   if( %changedEditParticle && PE_ParticleEditor.dirty )
   {
      MessageBoxYesNoCancel("Save Particle Changes?", 
         "Do you wish to save the changes made to the <br>current particle before changing the particle?", 
         "PE_ParticleEditor.saveParticle( " @ PE_ParticleEditor.currParticle.getName() @ " ); PE_EmitterEditor.updateEmitter( \"particles\"," @ %particles @ ");", 
         "PE_ParticleEditor.saveParticleDialogDontSave( " @ PE_ParticleEditor.currParticle.getName() @ " ); PE_EmitterEditor.updateEmitter( \"particles\"," @ %particles @ ");", 
         "PE_EmitterEditor.guiSync();" );
   }
   else
   {
      PE_EmitterEditor.updateEmitter( "particles", %particles);
   }
}

// Load Emitter Functionality
function PE_EmitterEditor::onNewEmitter(%this)
{
   if( PE_EmitterEditor.currEmitter $= PEE_EmitterSelector.getText() )
      return;
   
   if( PE_EmitterEditor.dirty )
   {         
      %savedEmitter = PE_EmitterEditor.currEmitter;
      MessageBoxYesNoCancel("Save Existing Emitter?", 
      "Do you want to save changes to <br><br>" @ %savedEmitter.getName(), 
      "PE_EmitterEditor.saveEmitter(" @ %savedEmitter@ "); PE_EmitterEditor.loadNewEmitter();", 
      "PE_EmitterEditor.saveEmitterDialogDontSave(" @ %savedEmitter @ "); PE_EmitterEditor.loadNewEmitter();",
      "PE_EmitterEditor.saveEmitterDialogCancel();" );
   }
   else
   {
     PE_EmitterEditor.loadNewEmitter();
   }
}

function PE_EmitterEditor::loadNewEmitter(%this)
{
   %id = PEE_EmitterSelector.getSelected();
   PE_EmitterEditor.currEmitter = PEE_EmitterSelector.getTextById(%id);
   PE_EmitterEditor.copyEmitters( PE_EmitterEditor.currEmitter, PE_EmitterEditor_NotDirtyEmitter );
   PE_EmitterEditor_NotDirtyEmitter.originalName = PE_EmitterEditor.currEmitter.getName();
   
   PE_EmitterEditor.guiSync();
   PE_EmitterEditor.setEmitterNotDirty();
   
   ParticleEditor.updateEmitterNode();
   PE_ParticleEditor.currParticle = getWord(PE_EmitterEditor.currEmitter.particles, 0);
}
   
function PE_EmitterEditor::copyEmitters(%this, %copyFrom, %copyTo) 
{
   (%copyTo).lifetimeMS = (%copyFrom).lifetimeMS;
   (%copyTo).lifetimeVarianceMS = (%copyFrom).lifetimeVarianceMS;
   (%copyTo).ejectionPeriodMS = (%copyFrom).ejectionPeriodMS;
   (%copyTo).periodVarianceMS = (%copyFrom).periodVarianceMS;
   (%copyTo).ejectionVelocity = (%copyFrom).ejectionVelocity;
   (%copyTo).velocityVariance = (%copyFrom).velocityVariance;
   (%copyTo).orientParticles = (%copyFrom).orientParticles;
   (%copyTo).alignParticles = (%copyFrom).alignParticles;
   (%copyTo).alignDirection = (%copyFrom).alignDirection; 
   (%copyTo).thetaMin = (%copyFrom).thetaMin;
   (%copyTo).thetaMax = (%copyFrom).thetaMax;
   (%copyTo).phiVariance = (%copyFrom).phiVariance;
   (%copyTo).ejectionOffset = (%copyFrom).ejectionOffset;
   (%copyTo).particles = (%copyFrom).particles;
   
   (%copyTo).blendStyle = (%copyFrom).blendStyle;
   (%copyTo).softnessDistance = (%copyFrom).softnessDistance;
   (%copyTo).ambientFactor = (%copyFrom).ambientFactor;
   
   (%copyTo).softParticles = (%copyFrom).softParticles;
   (%copyTo).reverseOrder = (%copyFrom).reverseOrder;
   (%copyTo).useEmitterSizes = (%copyFrom).useEmitterSizes;
   (%copyTo).useEmitterColors = (%copyFrom).useEmitterColors;
}

function PE_EmitterEditor::setEmitterDirty(%this)
{
   %propertyText = "Emitter *";
   PE_EmitterEditor.text = %propertyText;
   PE_EmitterEditor.dirty = true;
   
   %emitter = PE_EmitterEditor.currEmitter;
   
   // emitters created in the particleEditor are given that as its filename, so we run another check
   if( PE_EmitterEditor.currEmitter.getFilename() $= "" || PE_EmitterEditor.currEmitter.getFilename() $= "tools/particleEditor/particleEditor.ed.cs" )
      ParticleEditor_Emitter.setDirty(%emitter, "art/shapes/particles/managedParticleData.cs");
   else
      ParticleEditor_Emitter.setDirty(%emitter);
}

function PE_EmitterEditor::setEmitterNotDirty( %this )
{
   
   %propertyText = strreplace("Emitter" , "*" , "");
   PE_EmitterEditor.text = %propertyText;
   PE_EmitterEditor.dirty = false;
   
   %emitter = PE_EmitterEditor.currEmitter;
   ParticleEditor_Emitter.removeDirty(%emitter);
}

// Create Functionality
function PE_EmitterEditor::showNewDialog( %this )
{
   // Open a dialog if the current emitter is dirty
   if ( PE_EmitterEditor.dirty ) 
   {
      MessageBoxYesNoCancel("Save Emitter Changes?", 
         "Do you wish to save the changes made to the <br>current emitter before changing the emitter?", 
         "PE_EmitterEditor.saveEmitter( " @ PE_EmitterEditor.currEmitter.getName() @ " ); PE_EmitterEditor.createEmitter();", 
         "PE_EmitterEditor.saveEmitterDialogDontSave( " @ PE_EmitterEditor.currEmitter.getName() @ " ); PE_EmitterEditor.createEmitter();", 
         "" );
   }
   else
   {
      PE_EmitterEditor.createEmitter();
   }
}

function PE_EmitterEditor::createEmitter(%this)
{
   // Create an actual emitter
   %emitter = getUniqueName("newEmitter");   
   eval("datablock ParticleEmitterData" @ "(" @ %emitter @ ") { particles = \"DefaultParticle\"; parentGroup = \"DataBlockGroup\"; };" );
   %action = ParticleEditor.createUndo(ActionCreateNewEmitter, "Create New Emitter");
   %action.oldEmitter = PE_EmitterEditor.currEmitter;
   %action.newEmitter = %emitter;
   ParticleEditor.submitUndo( %action );
      
   // Drop it in the dropdown
   PEE_EmitterSelector_Control-->PopUpMenu.add( %emitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.sort();
   
   // Select and Load it
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %emitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id );
   
   PE_EmitterEditor.loadNewEmitter();
   
   // Sort the dropdown
   //PEE_EmitterSelector_Control-->PopUpMenu.sort();
}

// Delete Functionality
function PE_EmitterEditor::showDeleteDialog( %this )
{
   if( PE_EmitterEditor.currEmitter.getName() $= "DefaultEmitter" )
   {
      MessageBoxOK( "Error", "Cannot delete DefaultEmitter");
      return;
   }
   
   if( isObject( PE_EmitterEditor.currEmitter ) )
   {
      MessageBoxYesNoCancel("Delete Emitter?", 
         "Are you sure you want to delete<br><br>" @ PE_EmitterEditor.currEmitter.getName() @ "<br><br> Emitter deletion won't take affect until the engine is quit.", 
         "PE_EmitterEditor.saveEmitterDialogDontSave( " @ PE_EmitterEditor.currEmitter.getName() @ " ); PE_EmitterEditor.deleteEmitter();", 
         "", 
         "" );
   }
}

function PE_EmitterEditor::deleteEmitter( %this )
{  
   // Create an actual emitter
   if( PEE_EmitterSelector_Control-->PopUpMenu.size() == 0 )
   {
      %emitter = getUniqueName("newEmitter");   
      eval("datablock ParticleEmitterData" @ "(" @ %emitter @ ") { particles = \"DefaultParticle\"; parentGroup = \"DataBlockGroup\"; };" );
      
      PEE_EmitterSelector_Control-->PopUpMenu.add( %emitter.getName() );   
   }
   else
   {
      %id = PEE_EmitterSelector_Control-->PopUpMenu.findText("DefaultEmitter");
      if(%id >= 0)
         %emitter = "DefaultEmitter";
      else
         %emitter = PEE_EmitterSelector_Control-->PopUpMenu.getTextById( 0 );
   }
   
   %action = ParticleEditor.createUndo(ActionDeleteEmitter, "Delete Emitter");
   %action.oldEmitter = PE_EmitterEditor.currEmitter;
   %action.oldEmitterFname = PE_EmitterEditor.currEmitter.getFilename();
   %action.newEmitter = %emitter;
   
   
   
   UnlistedEmitters.add( "unlistedEmitters", PE_EmitterEditor.currEmitter );
   // check to see if its valid (not already been deleted and not editor used)
   if( PE_EmitterEditor.currEmitter.getFileName() !$= "" && 
         PE_EmitterEditor.currEmitter.getFilename() !$= "tools/particleEditor/particleEditor.ed.gui" )
      ParticleEditor_Emitter.removeObjectFromFile( PE_EmitterEditor.currEmitter );
   
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( PE_EmitterEditor.currEmitter.getName() );
   
   %action.oldEmitterId = %id;
   ParticleEditor.submitUndo( %action );
   
   PEE_EmitterSelector_Control-->PopUpMenu.clearEntry( %id );
   PEE_EmitterSelector_Control-->PopUpMenu.sort();
   
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %emitter.getName() );
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id  );
   PE_EmitterEditor.loadNewEmitter();
}

function PE_EmitterEditor::saveEmitter( %this, %emitter )
{   
   PE_EmitterEditor.copyEmitters( %emitter, PE_EmitterEditor_NotDirtyEmitter );
   PE_EmitterEditor.currEmitter.setName( PE_EmitterEditor_NotDirtyEmitter.originalName );
   
   ParticleEditor_Emitter.saveDirty(); 
   
   PE_EmitterEditor.setEmitterNotDirty();  
}

function PE_EmitterEditor::saveEmitterDialogDontSave( %this, %emitter)
{  
   /* redo the name change 
   %idx = matEd_cubemapEd_availableCubemapList.findItemText( %oldCubemap.getName() );
   matEd_cubemapEd_availableCubemapList.setItemText( %idx, notDirtyCubemap.originalName );      
   */  
   %emitter.setName( PE_EmitterEditor_NotDirtyEmitter.originalName );
   PE_EmitterEditor.copyEmitters( PE_EmitterEditor_NotDirtyEmitter, %emitter);
   PE_EmitterEditor.setEmitterNotDirty();
   
   //update to new emitter
   
}

function PE_EmitterEditor::saveEmitterDialogCancel( %this )
{ 
   %emitter = PE_EmitterEditor.currEmitter;
   
   %id = PEE_EmitterSelector_Control-->PopUpMenu.findText( %emitter );
   PEE_EmitterSelector_Control-->PopUpMenu.setSelected( %id, true );
}

//------------------------------------------------------------------------------
// Particle Functionality

function PE_ParticleEditor::guiSync( %this )
{
   // needs to be rewritten
   %containsCurrParticle = false;
   PEP_ParticleSelector_Control-->PopUpMenu.clear();
   %count = getWordCount(PE_EmitterEditor.currEmitter.particles);
   for (%i = 0; %i < %count; %i++) 
   {
      %particleName = getWord(PE_EmitterEditor.currEmitter.particles, %i);
      
      if( %particleName $= PE_ParticleEditor.currParticle.getName() )
         %containsCurrParticle = true;      
      
      PEP_ParticleSelector_Control-->PopUpMenu.add( %particleName );
   }
   
   // Just in case the particle doesn't exist, fallback gracefully
   if( !%containsCurrParticle )
      PE_ParticleEditor.currParticle = getWord(PE_EmitterEditor.currEmitter.particles, 0);
   
   %data = PE_ParticleEditor.currParticle;
   
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %data.getName() );
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id );
   
   PEP_ParticleSelector_Control-->TextEdit.setText( %data.getName() );
   
   PE_ParticleEditor-->PEP_previewImage.setBitmap( %data );
   %bitmap = MaterialEditorGui.searchForTexture( %data.getName(), %data.textureName );
   if( %bitmap !$= "" )
   {
      PE_ParticleEditor-->PEP_previewImage.setBitmap(%bitmap);
      PE_ParticleEditor-->PEP_previewImageName.setText( %bitmap );
      PE_ParticleEditor-->PEP_previewImageName.tooltip = %bitmap;
   }
   else
   {
      PE_ParticleEditor-->PEP_previewImage.setBitmap( "" );
      PE_ParticleEditor-->PEP_previewImageName.setText("None");
      PE_ParticleEditor-->PEP_previewImageName.tooltip = "None";
   }
   
   PE_ParticleEditor-->PEP_inverseAlpha.setValue( %data.useInvAlpha );
   
   PE_ParticleEditor-->PEP_lifetimeMS_slider.setValue( %data.lifetimeMS );
   PE_ParticleEditor-->PEP_lifetimeMS_textEdit.setText( %data.lifetimeMS );
   
   PE_ParticleEditor-->PEP_lifetimeVarianceMS_slider.setValue( %data.lifetimeVarianceMS );
   PE_ParticleEditor-->PEP_lifetimeVarianceMS_textEdit.setText( %data.lifetimeVarianceMS );
   
   PE_ParticleEditor-->PEP_inheritedVelFactor_slider.setValue( %data.inheritedVelFactor );
   PE_ParticleEditor-->PEP_inheritedVelFactor_textEdit.setText( %data.inheritedVelFactor );
   
   PE_ParticleEditor-->PEP_constantAcceleration_slider.setValue( %data.constantAcceleration );
   PE_ParticleEditor-->PEP_constantAcceleration_textEdit.setText( %data.constantAcceleration );
   
   PE_ParticleEditor-->PEP_gravityCoefficient_slider.setValue( %data.gravityCoefficient );
   PE_ParticleEditor-->PEP_gravityCoefficient_textEdit.setText( %data.gravityCoefficient );
   
   PE_ParticleEditor-->PEP_dragCoefficient_slider.setValue( %data.dragCoefficient );
   PE_ParticleEditor-->PEP_dragCoefficient_textEdit.setText( %data.dragCoefficient );
   
   PE_ParticleEditor-->PEP_spinRandomMin_slider.setValue( %data.spinRandomMin );
   PE_ParticleEditor-->PEP_spinRandomMin_textEdit.setText( %data.spinRandomMin );
   
   PE_ParticleEditor-->PEP_spinRandomMax_slider.setValue( %data.spinRandomMax );
   PE_ParticleEditor-->PEP_spinRandomMax_textEdit.setText( %data.spinRandomMax  );
   
   PE_ParticleEditor-->PEP_spinRandomMax_slider.setValue( %data.spinRandomMax );
   PE_ParticleEditor-->PEP_spinRandomMax_textEdit.setText( %data.spinRandomMax  );
   
   PE_ParticleEditor-->PEP_spinSpeed_slider.setValue( %data.spinSpeed );
   PE_ParticleEditor-->PEP_spinSpeed_textEdit.setText( %data.spinSpeed );
   
   //PE_ParticleEditor-->PEP_useTextureAnimation.setValue( %data.animateTexture );
   
   //PE_ParticleEditor-->PEP_framesPerSec_slider.setValue( %data.framesPerSec );
   //PE_ParticleEditor-->PEP_framesPerSec_textEdit.setText( %data.framesPerSec );
   PE_ColorTintSwatch0.color = %data.colors[0];
   PE_ColorTintSwatch1.color = %data.colors[1];
   PE_ColorTintSwatch2.color = %data.colors[2];
   PE_ColorTintSwatch3.color = %data.colors[3];
   
   PE_ParticleEditor-->PEP_pointSize_slider0.setValue( %data.sizes[0] );
   PE_ParticleEditor-->PEP_pointSize_textEdit0.setText( %data.sizes[0] );
   
   PE_ParticleEditor-->PEP_pointSize_slider1.setValue( %data.sizes[1] );
   PE_ParticleEditor-->PEP_pointSize_textEdit1.setText( %data.sizes[1] );
   
   PE_ParticleEditor-->PEP_pointSize_slider2.setValue( %data.sizes[2] );
   PE_ParticleEditor-->PEP_pointSize_textEdit2.setText( %data.sizes[2] );
   
   PE_ParticleEditor-->PEP_pointSize_slider3.setValue( %data.sizes[3] );
   PE_ParticleEditor-->PEP_pointSize_textEdit3.setText( %data.sizes[3] );
   
   PE_ParticleEditor-->PEP_pointTime_slider0.setValue( %data.times[0] );
   PE_ParticleEditor-->PEP_pointTime_textEdit0.setText( %data.times[0] );
   
   PE_ParticleEditor-->PEP_pointTime_slider1.setValue( %data.times[1] );
   PE_ParticleEditor-->PEP_pointTime_textEdit1.setText( %data.times[1] );
   
   PE_ParticleEditor-->PEP_pointTime_slider2.setValue( %data.times[2] );
   PE_ParticleEditor-->PEP_pointTime_textEdit2.setText( %data.times[2] );
   
   PE_ParticleEditor-->PEP_pointTime_slider3.setValue( %data.times[3] );
   PE_ParticleEditor-->PEP_pointTime_textEdit3.setText( %data.times[3] );
   /*
   PE_ParticleEditor-->PEP_columns_slider.setValue( %data.columns );
   PE_ParticleEditor-->PEP_columns_slider.setText( %data.columns );
   
   PE_ParticleEditor-->PEP_rows_slider.setValue( %data.rows );
   PE_ParticleEditor-->PEP_rows_slider.setText( %data.rows );
   
   PE_ParticleEditor-->PEP_pointSize_slider.setValue( %data.spinSpeed );
   PE_ParticleEditor-->PEP_pointSize_textEdit.setText( %data.spinSpeed );
   
   PEP_windCoefficient.setValue(     %data.windCoefficient);
   PEP_times0.setText(               %data.times[0]);
   PEP_times1.setText(               %data.times[1]);
   PEP_times2.setText(               %data.times[2]);
   PEP_times3.setText(               %data.times[3]);
   PEP_sizes0.setText(               %data.sizes[0]);
   PEP_sizes1.setText(               %data.sizes[1]);
   PEP_sizes2.setText(               %data.sizes[2]);
   PEP_sizes3.setText(               %data.sizes[3]);
   PEP_colors0.setText(              %data.colors[0]);
   PEP_colors1.setText(              %data.colors[1]);
   PEP_colors2.setText(              %data.colors[2]);
   PEP_colors3.setText(              %data.colors[3]);
   */
}

// Generic updateParticle method
function PE_ParticleEditor::updateParticle(%this, %propertyField, %value, %isSlider, %onMouseUp)
{
   PE_ParticleEditor.setParticleDirty();
   
   if(%value $= "")
      %value = "\"\"";
   
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.field = %propertyField;
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValue = %value;
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveParticle, "Update Active Particle");
      %action.particle = PE_ParticleEditor.currParticle;
      %action.field = %propertyField;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      %action.newValue = %value;
      eval( "%action.oldValue = " @ PE_ParticleEditor.currParticle @ "." @ %propertyField @ ";");
      %action.oldValue = "\"" @ %action.oldValue @ "\"";
      
      ParticleEditor.submitUndo( %action );
   }
   
	eval("PE_ParticleEditor.currParticle." @ %propertyField @ " = " @ %value @ ";");
   PE_ParticleEditor.currParticle.reload();
   
   /*
   PE_ParticleEditor.currParticle.times[0]             = PEP_times0.getValue();
   PE_ParticleEditor.currParticle.times[1]             = PEP_times1.getValue();
   PE_ParticleEditor.currParticle.times[2]             = PEP_times2.getValue();
   PE_ParticleEditor.currParticle.times[3]             = PEP_times3.getValue();
   PE_ParticleEditor.currParticle.sizes[0]             = PEP_sizes0.getValue();
   PE_ParticleEditor.currParticle.sizes[1]             = PEP_sizes1.getValue();
   PE_ParticleEditor.currParticle.sizes[2]             = PEP_sizes2.getValue();
   PE_ParticleEditor.currParticle.sizes[3]             = PEP_sizes3.getValue();
   PE_ParticleEditor.currParticle.colors[0]            = PEP_colors0.getValue();
   PE_ParticleEditor.currParticle.colors[1]            = PEP_colors1.getValue();
   PE_ParticleEditor.currParticle.colors[2]            = PEP_colors2.getValue();
   PE_ParticleEditor.currParticle.colors[3]            = PEP_colors3.getValue();
   */                    
}

// Special case updateEmitter methods
function PE_ParticleEditor::updateParticleTexture( %this, %action )
{
   if( %action )
   {
      %texture = MaterialEditorGui.openFile("texture");
      if( %texture !$= "" )
      {
         PE_ParticleEditor-->PEP_previewImage.setBitmap(%texture);
         PE_ParticleEditor-->PEP_previewImageName.setText(%texture);
         PE_ParticleEditor-->PEP_previewImageName.tooltip = %texture;
         
         PE_ParticleEditor.updateParticle("textureName","\"" @ %texture @ "\"");
      }
   }
   else
   {
      PE_ParticleEditor-->PEP_previewImage.setBitmap("");
      PE_ParticleEditor-->PEP_previewImageName.setText("");
      PE_ParticleEditor-->PEP_previewImageName.tooltip = "";
      
      PE_ParticleEditor.updateParticle("textureName", "");
   }
}

function PE_ParticleEditor::updateLifeFields( %this, %isRandom, %value, %isSlider, %onMouseUp )
{
   PE_ParticleEditor.setParticleDirty();
   
   if(%value $= "")
      %value = "\"\"";
   
   //Transfer values over to gui controls
   if( %isRandom )
   {
      %value++;
      if( %value > PE_ParticleEditor-->PEP_lifetimeMS_slider.getValue() )
      {
         PE_ParticleEditor-->PEP_lifetimeMS_textEdit.setText( %value );
         PE_ParticleEditor-->PEP_lifetimeMS_slider.setValue( %value );
      }
   }
   else
   {
      
      %value--;
      if( %value < PE_ParticleEditor-->PEP_lifetimeVarianceMS_slider.getValue() )
      {
         PE_ParticleEditor-->PEP_lifetimeVarianceMS_textEdit.setText( %value );
         PE_ParticleEditor-->PEP_lifetimeVarianceMS_slider.setValue( %value );
      }
   }
   
   // Logic for storing undo values
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValueLifetimeMS = PE_ParticleEditor-->PEP_lifetimeMS_textEdit.getText();
      %last.newValueLifetimeVarianceMS = PE_ParticleEditor-->PEP_lifetimeVarianceMS_textEdit.getText();
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveParticleLifeFields, "Update Active Particle");
      %action.particle = PE_ParticleEditor.currParticle;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      
      %action.newValueLifetimeMS = PE_ParticleEditor-->PEP_lifetimeMS_textEdit.getText();
      %action.oldValueLifetimeMS = PE_ParticleEditor.currParticle.lifetimeMS;
      
      %action.newValueLifetimeVarianceMS = PE_ParticleEditor-->PEP_lifetimeVarianceMS_textEdit.getText();
      %action.oldValueLifetimeVarianceMS = PE_ParticleEditor.currParticle.lifetimeVarianceMS;
      
      ParticleEditor.submitUndo( %action );
   }
   
   PE_ParticleEditor.currParticle.lifetimeMS = PE_ParticleEditor-->PEP_lifetimeMS_textEdit.getText();
   PE_ParticleEditor.currParticle.lifetimeVarianceMS = PE_ParticleEditor-->PEP_lifetimeVarianceMS_textEdit.getText();
   PE_ParticleEditor.currParticle.reload();   
}

function PE_ParticleEditor::updateSpinFields( %this, %isMax, %value, %isSlider, %onMouseUp )
{
   PE_ParticleEditor.setParticleDirty();
   
   if(%value $= "")
      %value = "\"\"";
   
   //Transfer values over to gui controls
   if( %isMax )
   {
      %value++;
      if( %value > PE_ParticleEditor-->PEP_spinRandomMax_slider.getValue() )
      {
         PE_ParticleEditor-->PEP_spinRandomMax_textEdit.setText( %value );
         PE_ParticleEditor-->PEP_spinRandomMax_slider.setValue( %value );
      }
   }
   else
   {
      
      %value--;
      if( %value < PE_ParticleEditor-->PEP_spinRandomMin_slider.getValue() )
      {
         PE_ParticleEditor-->PEP_spinRandomMin_textEdit.setText( %value );
         PE_ParticleEditor-->PEP_spinRandomMin_slider.setValue( %value );
      }
   }
   
   // Logic for storing undo values
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if( (%isSlider) && (%last.isSlider) && (!%last.onMouseUp) )
   {
      %last.isSlider = %isSlider;
      %last.onMouseUp = %onMouseUp;
      %last.newValueSpinRandomMax = PE_ParticleEditor-->PEP_spinRandomMax_textEdit.getText();
      %last.newValueSpinRandomMin = PE_ParticleEditor-->PEP_spinRandomMin_textEdit.getText();
   }
   else
   {
      %action = ParticleEditor.createUndo(ActionUpdateActiveParticleSpinFields, "Update Active Particle");
      %action.particle = PE_ParticleEditor.currParticle;
      %action.isSlider = %isSlider;
      %action.onMouseUp = %onMouseUp;
      
      %action.newValueSpinRandomMax = PE_ParticleEditor-->PEP_spinRandomMax_textEdit.getText();
      %action.oldValueSpinRandomMax = PE_ParticleEditor.currParticle.spinRandomMax;
      
      %action.newValueSpinRandomMin = PE_ParticleEditor-->PEP_spinRandomMin_textEdit.getText();
      %action.oldValueSpinRandomMin = PE_ParticleEditor.currParticle.spinRandomMin;
      
      ParticleEditor.submitUndo( %action );
   }
   
   PE_ParticleEditor.currParticle.spinRandomMax = PE_ParticleEditor-->PEP_spinRandomMax_textEdit.getText();
   PE_ParticleEditor.currParticle.spinRandomMin = PE_ParticleEditor-->PEP_spinRandomMin_textEdit.getText();
   
   PE_ParticleEditor.currParticle.reload();   
}

function PE_ColorTintSwatch0::updateParticleColor( %this, %color )
{
   %arrayNum = %this.arrayNum;
   
   %r = getWord(%color,0);
   %g = getWord(%color,1);
   %b = getWord(%color,2);
   %a = getWord(%color,3);
   
   %colorSwatch = (%r SPC %g SPC %b SPC %a);
   %color = "\"" @ %r SPC %g SPC %b SPC %a @ "\"";
   
   %this.color = %colorSwatch;
	PE_ParticleEditor.updateParticle("colors[" @ %arrayNum @ "]", %color);
}

function PE_ColorTintSwatch1::updateParticleColor( %this, %color )
{
   %arrayNum = %this.arrayNum;
   
   %r = getWord(%color,0);
   %g = getWord(%color,1);
   %b = getWord(%color,2);
   %a = getWord(%color,3);
   
   %colorSwatch = (%r SPC %g SPC %b SPC %a);
   %color = "\"" @ %r SPC %g SPC %b SPC %a @ "\"";
   
   %this.color = %colorSwatch;
	PE_ParticleEditor.updateParticle("colors[" @ %arrayNum @ "]", %color);
}

function PE_ColorTintSwatch2::updateParticleColor( %this, %color )
{
   %arrayNum = %this.arrayNum;
   
   %r = getWord(%color,0);
   %g = getWord(%color,1);
   %b = getWord(%color,2);
   %a = getWord(%color,3);
   
   %colorSwatch = (%r SPC %g SPC %b SPC %a);
   %color = "\"" @ %r SPC %g SPC %b SPC %a @ "\"";
   
   %this.color = %colorSwatch;
	PE_ParticleEditor.updateParticle("colors[" @ %arrayNum @ "]", %color);
}

function PE_ColorTintSwatch3::updateParticleColor( %this, %color )
{
   %arrayNum = %this.arrayNum;
   
   %r = getWord(%color,0);
   %g = getWord(%color,1);
   %b = getWord(%color,2);
   %a = getWord(%color,3);
   
   %colorSwatch = (%r SPC %g SPC %b SPC %a);
   %color = "\"" @ %r SPC %g SPC %b SPC %a @ "\"";
   
   %this.color = %colorSwatch;
	PE_ParticleEditor.updateParticle("colors[" @ %arrayNum @ "]", %color);
}

// Loads the selected particle
function PE_ParticleEditor::onNewParticle(%this)
{
   // Bail if the user selected the same particle
   %id = PEP_ParticleSelector.getSelected();
   if( PEP_ParticleSelector.getTextById(%id) $= PE_ParticleEditor.currParticle ||
      PEP_ParticleSelector.getTextById(%id) $= "" )
   {
      // Temporary cover up for guiPopupCtrl bug. Ensures that the name of the
      // current particle remains visible
      %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( PE_ParticleEditor.currParticle );
      PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id );
      PEP_ParticleSelector_Control-->TextEdit.setText( PE_ParticleEditor.currParticle );
      return;
   }
   
   // Load new particle if we're not in a dirty state
   if( PE_ParticleEditor.dirty )
   {         
      MessageBoxYesNoCancel("Save Existing Particle?", 
      "Do you want to save changes to <br><br>" @ PE_ParticleEditor.currParticle.getName(), 
      "PE_ParticleEditor.saveParticle(" @ PE_ParticleEditor.currParticle @ ");", 
      "PE_ParticleEditor.saveParticleDialogDontSave(" @ PE_ParticleEditor.currParticle @ "); PE_ParticleEditor.loadNewParticle();",
      "PE_ParticleEditor.saveParticleDialogCancel();" );
   }
   else
   {
      PE_ParticleEditor.loadNewParticle();
   }

}

function PE_ParticleEditor::loadNewParticle(%this)
{
   // Set the text of the selected particle
   %id = PEP_ParticleSelector.getSelected();
   PE_ParticleEditor.currParticle = PEP_ParticleSelector.getTextById(%id);
   
   PE_ParticleEditor.currParticle.reload();
   
   // Do the dance
   PE_ParticleEditor_NotDirtyParticle.originalName = PE_ParticleEditor.currParticle.getName();
   PE_ParticleEditor.copyParticles( PE_ParticleEditor.currParticle, PE_ParticleEditor_NotDirtyParticle );
   PE_ParticleEditor.guiSync();
   PE_ParticleEditor.setParticleNotDirty();
}

function PE_ParticleEditor::copyParticles(%this, %copyFrom, %copyTo) 
{
   (%copyTo).lifetimeMS = (%copyFrom).lifetimeMS;
   (%copyTo).lifetimeVarianceMS = (%copyFrom).lifetimeVarianceMS;
   (%copyTo).constantAcceleration = (%copyFrom).constantAcceleration;
   (%copyTo).inheritedVelFactor = (%copyFrom).inheritedVelFactor;
   (%copyTo).gravityCoefficient = (%copyFrom).gravityCoefficient;
   (%copyTo).dragCoefficient = (%copyFrom).dragCoefficient;
   (%copyTo).spinSpeed = (%copyFrom).spinSpeed;
   (%copyTo).spinRandomMin = (%copyFrom).spinRandomMin;
   (%copyTo).spinRandomMax = (%copyFrom).spinRandomMax; 
   (%copyTo).useInvAlpha = (%copyFrom).useInvAlpha;
   (%copyTo).animateTexture = (%copyFrom).animateTexture;
   (%copyTo).framesPerSec = (%copyFrom).framesPerSec;
   (%copyTo).textureCoords[0] = (%copyFrom).textureCoords[0];
   (%copyTo).textureCoords[1] = (%copyFrom).textureCoords[1];
   (%copyTo).textureCoords[2] = (%copyFrom).textureCoords[2];
   (%copyTo).textureCoords[3] = (%copyFrom).textureCoords[3];
   
   (%copyTo).colors[0] = (%copyFrom).colors[0];
   (%copyTo).colors[1] = (%copyFrom).colors[1];
   (%copyTo).colors[2] = (%copyFrom).colors[2];
   (%copyTo).colors[3] = (%copyFrom).colors[3];
   
   (%copyTo).sizes[0] = (%copyFrom).sizes[0];
   (%copyTo).sizes[1] = (%copyFrom).sizes[1];
   (%copyTo).sizes[2] = (%copyFrom).sizes[2];
   (%copyTo).sizes[3] = (%copyFrom).sizes[3];
   
   (%copyTo).times[0] = (%copyFrom).times[0];
   (%copyTo).times[1] = (%copyFrom).times[1];
   (%copyTo).times[2] = (%copyFrom).times[2];
   (%copyTo).times[3] = (%copyFrom).times[3];
   (%copyTo).animTexTiling = (%copyFrom).animTexTiling;
   
   (%copyTo).animTexFrames = (%copyFrom).animTexFrames;
   (%copyTo).textureName = (%copyFrom).textureName;
   (%copyTo).animTexName = (%copyFrom).animTexName;
}

function PE_ParticleEditor::setParticleDirty(%this)
{
   %propertyText = "Particle *";
   PE_ParticleEditor.text = %propertyText;
   PE_ParticleEditor.dirty = true;
   
   %particle = PE_ParticleEditor.currParticle;
   
   // particles created in the particleEditor are given that as its filename, so we run another check
   if( PE_ParticleEditor.currParticle.getFilename() $= "" || PE_ParticleEditor.currParticle.getFilename() $= "tools/particleEditor/particleEditor.ed.cs" )
      ParticleEditor_Particle.setDirty(%particle, "art/shapes/particles/managedParticleData.cs");
   else
      ParticleEditor_Particle.setDirty(%particle);
}

function PE_ParticleEditor::setParticleNotDirty( %this )
{
   
   %propertyText = strreplace("Particle" , "*" , "");
   PE_ParticleEditor.text = %propertyText;
   PE_ParticleEditor.dirty = false;
   
   %particle = PE_ParticleEditor.currParticle;
   ParticleEditor_Particle.removeDirty(%particle);
}

// Create Functionality
function PE_ParticleEditor::showNewDialog( %this )
{
   // Open a dialog if the current Particle is dirty
   if ( PE_ParticleEditor.dirty ) 
   {
      MessageBoxYesNoCancel("Save Particle Changes?", 
         "Do you wish to save the changes made to the <br>current particle before changing the particle?", 
         "PE_ParticleEditor.saveParticle( " @ PE_ParticleEditor.currParticle.getName() @ " ); PE_ParticleEditor.createParticle();", 
         "PE_ParticleEditor.saveParticleDialogDontSave( " @ PE_ParticleEditor.currParticle.getName() @ " ); PE_ParticleEditor.createParticle();", 
         "" );
   }
   else
   {
      PE_ParticleEditor.createParticle();
   }
}

function PE_ParticleEditor::createParticle(%this)
{
   %maxParticleCheck = getWord( PE_EmitterEditor.currEmitter.particles, 3 );
   if( %maxParticleCheck !$= "" )
   {
      MessageBoxOK( "Error", "Max number of particles per emitter has been reached.");
      return;
   }
   
   %newParticle = getUniqueName("newParticle");
   %firstParticle = getWord( PE_EmitterEditor.currEmitter.particles, 0 );
   %texture = %firstParticle.textureName;
   eval("datablock ParticleData" @ "(" @ %newParticle @ ") { textureName = %texture ; parentGroup = \"DataBlockGroup\"; };" );
   %action = ParticleEditor.createUndo(ActionCreateNewParticle, "Create New Particle");
   %action.oldParticle = PE_ParticleEditor.currParticle;
   %action.newParticle = %newParticle;
   ParticleEditor.submitUndo( %action );
   
   %particles = "";
   for(%i = 0; %i < 4; %i++)
   {
      %currParticles = getWord( PE_EmitterEditor.currEmitter.particles, %i );
      
      if( %currParticles !$= "" )
      {
         if( %particles $= "" )
            %particles = %currParticles;
         else
            %particles = %particles @ "\t" @ %currParticles;
      }
      else
      {
         %particles = %particles @ "\t" @ %newParticle;
         break;
      }
   }
   
   // Update the emitter the old fashion way 
   PE_EmitterEditor.setEmitterDirty();
   PE_EmitterEditor.currEmitter.particles = %particles;
   PE_EmitterEditor.currEmitter.reload();
   
   // Drop it in the dropdown
   PEP_ParticleSelector_Control-->PopUpMenu.add( %newParticle );
   PEP_ParticleSelector_Control-->PopUpMenu.sort();
   
   // going to have to change this as well, at the the emitter set
   // Select and Load it
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %newParticle );
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id );
   
   PE_ParticleEditor.loadNewParticle();
   // Add particle to dropdowns in the emitter editor
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      %emitterParticle-->PopUpMenu.add( %newParticle );
      %emitterParticle-->PopUpMenu.sort();
   }
}

// Delete Functionality
function PE_ParticleEditor::showDeleteDialog( %this )
{
   if( PE_ParticleEditor.currParticle.getName() $= "DefaultParticle" )
   {
      MessageBoxOK( "Error", "Cannot delete DefaultParticle");
      return;
   }
   
   // check to see if the particle emitter has more than 1 particle on it
   %secondaryParticle = getWord( PE_EmitterEditor.currEmitter.particles, 1 );
   if( !isObject( %secondaryParticle ) )
   {
      MessageBoxOK( "Error", "At least one particle must remain on the particle emitter.");
      return;
   }
   
   if( isObject( PE_ParticleEditor.currParticle ) )
   {
      MessageBoxYesNoCancel("Delete Particle?", 
         "Are you sure you want to delete<br><br>" @ PE_ParticleEditor.currParticle.getName() @ "<br><br> Particle deletion won't take affect until the engine is quit.", 
         "PE_ParticleEditor.saveParticleDialogDontSave( " @ PE_ParticleEditor.currParticle.getName() @ " ); PE_ParticleEditor.deleteParticle();", 
         "", 
         "" );
   }
}

function PE_ParticleEditor::deleteParticle( %this )
{      
   %action = ParticleEditor.createUndo(ActionDeleteParticle, "Delete Particle");
   %action.oldParticle = PE_ParticleEditor.currParticle;
   %action.newParticle = PEE_EmitterSelector_Control-->PopUpMenu.getTextById( 0 );
      
   UnlistedParticles.add( "unlistedParticles", PE_ParticleEditor.currParticle );
   
   // Check to see if its valid (not already been deleted and not editor used)
   if( PE_ParticleEditor.currParticle.getFileName() !$= "" && PE_ParticleEditor.currParticle.getFilename() !$= "tools/particleEditor/particleEditor.ed.cs")
      ParticleEditor_Particle.removeObjectFromFile( PE_ParticleEditor.currParticle );
   
   // Removing the particle from the popupMenu
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( PE_ParticleEditor.currParticle.getName() );
   PEP_ParticleSelector_Control-->PopUpMenu.clearEntry( %id );
   
   // Removing the particle from the popupMenu in the emitterEditor
   for(%i = 1; %i < 5; %i++)
   {
      %emitterParticle = "PEE_EmitterParticle" @ %i;
      
      %id = %emitterParticle-->PopUpMenu.findText( PE_ParticleEditor.currParticle.getName() );
      %emitterParticle-->PopUpMenu.clearEntry( %id );
   }
   
   // Removing the particle from the emitter
   for( %i = 0; %i < getWordCount( PE_EmitterEditor.currEmitter.particles ); %i++ )
   {
      if( getWord( PE_EmitterEditor.currEmitter.particles, %i ) $= PE_ParticleEditor.currParticle.getName() )
         break;
   }
   %action.oldParticleSet = PE_EmitterEditor.currEmitter.particles;
   PE_EmitterEditor.setEmitterDirty();
   PE_EmitterEditor.currEmitter.particles = removeWord( PE_EmitterEditor.currEmitter.particles, %i );
   PE_EmitterEditor.currEmitter.reload();
   %action.newParticleSet = PE_EmitterEditor.currEmitter.particles;
   
   ParticleEditor.submitUndo( %action );
   
   // Setting up the popup menu with and loading a new particle 
   PEP_ParticleSelector_Control-->PopUpMenu.sort();
   PEP_ParticleSelector_Control-->PopUpMenu.setFirstSelected();
}

// Save Functionality

function PE_ParticleEditor::saveParticle( %this, %particle )
{
   PE_ParticleEditor.copyParticles( %particle, PE_ParticleEditor_NotDirtyParticle );
   PE_ParticleEditor_NotDirtyParticle.originalName = PE_ParticleEditor.currParticle.getName();
   ParticleEditor_Particle.saveDirty(); 
   PE_ParticleEditor.setParticleNotDirty();  
}

function PE_ParticleEditor::saveParticleDialogDontSave( %this, %particle)
{  
   //deal with old emitter first
   %oldParticle = PE_ParticleEditor.currParticle.getName();
   /* redo the name change 
   %idx = matEd_cubemapEd_availableCubemapList.findItemText( %oldCubemap.getName() );
   matEd_cubemapEd_availableCubemapList.setItemText( %idx, notDirtyCubemap.originalName );      
   */   
   //%oldParticle.setName( PE_ParticleEditor_NotDirtyParticle.originalName );
   PE_ParticleEditor.copyParticles( PE_ParticleEditor_NotDirtyParticle, %oldParticle);
   PE_ParticleEditor.setParticleNotDirty();
}

function PE_ParticleEditor::saveParticleDialogCancel( %this )
{ 
   %particle = PE_ParticleEditor.currParticle;
   
   %id = PEP_ParticleSelector_Control-->PopUpMenu.findText( %particle );
   PEP_ParticleSelector_Control-->PopUpMenu.setSelected( %id, true );
}
