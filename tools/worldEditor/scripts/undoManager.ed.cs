//-----------------------------------------------------------------------------
// Copyright (C) Sickhead Games, LLC
//-----------------------------------------------------------------------------

function EUndoManager::onUndo( %this )
{
}

function EUndoManager::onRedo( %this )
{
}

function EUndoManager::onAddUndo( %this )
{
}

function EUndoManager::onRemoveUndo( %this )
{
}

function EUndoManager::onClear( %this )
{
}

function EUndoManager::updateUndoMenu( %this, %editMenu )
{
   // TODO: If we ever fix the TerrainEditor and WorldEditor
   // to have descriptive UndoAction names then we can change
   // the text as part of the menu update.
      
   %undoName = %this.getNextUndoName();
   %redoName = %this.getNextRedoName();
   
   %editMenu.setItemName( 0, "Undo " @ %undoName );
   %editMenu.setItemName( 1, "Redo " @ %redoName );
   
   %editMenu.enableItem( 0, %undoName !$= "" );
   %editMenu.enableItem( 1, %redoName !$= "" );
}


/// A helper for submitting a creation undo action.
function MECreateUndoAction::submit( %undoObject )
{
   // The instant group will try to add our
   // UndoAction if we don't disable it.   
   pushInstantGroup();
   
   // Create the undo action.     
   %action = new MECreateUndoAction()
   {
      actionName = "Create " @ %undoObject.getClassName();
   };
   
   // Restore the instant group.
   popInstantGroup();
   
   // Set the object to undo.
   %action.addObject( %undoObject );
   
   // Submit it.
   %action.addToManager( EUndoManager );
}


/// A helper for submitting a delete undo action.
function MEDeleteUndoAction::submit( %deleteObjects )
{
   // The instant group will try to add our
   // UndoAction if we don't disable it.   
   pushInstantGroup();
   
   // Create the undo action.     
   %action = new MEDeleteUndoAction()
   {
      actionName = "Delete";
   };

   // Restore the instant group.
   popInstantGroup();
   
   // Add the deletion objects to the action which
   // will take care of properly deleting them.
   %deleteObjects = trim( %deleteObjects );   
   %count = getFieldCount( %deleteObjects );
   for ( %i = 0; %i < %count; %i++ )
   {
      %object = getField( %deleteObjects, %i );
      %action.deleteObject( %object );
   }
   
   // Submit it.
   %action.addToManager( EUndoManager );
}


