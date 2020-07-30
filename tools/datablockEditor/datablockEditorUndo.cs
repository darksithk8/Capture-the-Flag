function DatablockEditorPlugin::createUndo(%this, %class, %desc)
{
   pushInstantGroup();
   %action = new UndoScriptAction()
   {
      class = %class;
      superClass = BaseDatablockEdAction;
      actionName = %desc;
      editor = DatablockEditorPlugin;
      treeview = DatablockEditorTree;
      inspector = DatablockEditorInspector;
   };
   popInstantGroup();
   return %action;
}

function DatablockEditorPlugin::submitUndo(%this, %action)
{
   if (%action.isMethod("redo"))
      %action.addToManager(EUndoManager);
}

function BaseDatablockEdAction::redo(%this)
{
   %this.redo();
}

function BaseDatablockEdAction::undo(%this)
{
}

// Create Datablock

function ActionCreateDatablock::redo(%this)
{
   %this.editor.PM.setDirty(%this.db.getName(), %this.fname);
   %this.editor.addExistingItem(%this.db.getName(), true);
   %this.editor.selectDatablock(%this.db.getName());
   %this.editor.flagInspectorAsDirty(true);
   
   %id = UnlistedDatablocks.getIndexFromValue( %this.db.getName() );
   UnlistedDatablocks.erase( %id );
}

function ActionCreateDatablock::undo(%this)
{
   if( %this.treeview.findItemByName(%this.db.getName()) )
      %id = %this.treeview.findItemByName(%this.db.getName());
   else
      %id = %this.treeview.findItemByName(%this.db.getName() @ " *");
   
   %this.treeview.removeItem(%id);
   %this.editor.resetSelectedDatablock();
   %this.editor.PM.removeDirty(%this.db.getName());
   
   UnlistedDatablocks.add( "unlistedDatablocks", %this.db.getName() );
}

// Delete Datablock

function ActionDeleteDatablock::redo(%this)
{
   if( %this.treeview.findItemByName(%this.db.getName()) )
      %id = %this.treeview.findItemByName(%this.db.getName());
   else
      %id = %this.treeview.findItemByName(%this.db.getName() @ " *");
      
   %this.treeview.removeItem(%id);
   %this.editor.resetSelectedDatablock();
   %this.editor.PM.removeObjectFromFile(%this.db.getName());
   
   UnlistedDatablocks.add( "unlistedDatablocks", %this.db.getName() );
}

function ActionDeleteDatablock::undo(%this)
{    
   %this.editor.addExistingItem(%this.db.getName(), true);
   %this.editor.selectDatablock(%this.db.getName());
   %this.editor.flagInspectorAsDirty(true);
   
   %this.editor.PM.setDirty(%this.db.getName(), %this.fname);
   %this.editor.PM.saveDirtyObject( %this.db.getName() );
   
   %id = UnlistedDatablocks.getIndexFromValue( %this.db.getName() );
   UnlistedDatablocks.erase( %id );
}