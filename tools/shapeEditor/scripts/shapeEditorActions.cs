// The TSShapeConstructor object allows you to apply a set of transformations
// to a 3space shape after it is loaded by Torque, but _before_ the shape is used
// by any other object (eg. Player, StaticShape etc). The sort of transformations
// available include adding, renaming and removing nodes and sequences. This GUI
// is a visual wrapper around TSShapeConstructor which allows you to build up the
// transformation set without having to get your hands dirty with TorqueScript.
//
// There are a few interesting situations to handle:
//
// 1. Undoing a 'delete' operation: Since nodes and sequences may have a lot of
//    animation data associated with them, it would be a lot of work to try to
//    save this data somewhere on the off chance the user decides to undo the
//    delete operation. Instead, when we delete a node or sequence in the GUI, we
//    just rename the node to something like __deleted__<name>. These names are
//    automatically filtered out by the gui, so as far as the user can tell,
//    they really have been deleted. To un-delete, we just rename the node or
//    sequence back again!
//
// 2. (1) above introduces a minor issue => what if we delete a node, then add a
//    new one with the same name, then delete that? We'd try to rename it to
//    __deleted_<name> which already exists! The simple workaround is to append
//    a number to the deleted name which increments for each delete operation,
//    thus ensuring that all deleted names are unique.
//
// 3. When renaming a node/sequence in the GUI, there tends to be a lot of
//    consecutive renames as the user types each character (same with editing
//    transforms). To avoid flooding the undo stack, we check if the new operation
//    is the same as the top of the stack, and if so, we merge the operations
//    instead of adding a new one. So doing a series of renames on the same node
//    will only result in a single action on the stack.
//
// This file implements all of the actions that can be applied by the GUI. Each
// action has 3 methods:
//
//    doit: called the first time the action is performed
//    undo: called to undo the action
//    redo: called to redo the action (usually the same as doit)
//
// In each case, the appropriate change is made to the shape, and the GUI updated.
//
// TSShapeConstructor keeps track of all the changes made and provides a simple
// way to save the modifications back out to a script file.


//------------------------------------------------------------------------------
// Helper functions for creating and applying GUI operations

function ShapeEditor::createAction(%this, %class, %desc)
{
   %action = new UndoScriptAction()
   {
      class = %class;
      superClass = BaseShapeEdAction;
      actionName = %desc;
      done = 0;
      tree = ShapeEdNodeTreeView;
   };
   return %action;
}

function ShapeEditor::doAction(%this, %action)
{
   if (%action.doit())
   {
      ShapeEditor.setDirty(true);
      %action.addToManager(EUndoManager);
   }
}

function BaseShapeEdAction::redo(%this)
{
   // Default redo action is the same as the doit action
   ShapeEditor.setDirty(true);
   %this.doit();
}

function BaseShapeEdAction::undo(%this)
{
   ShapeEditor.setDirty(true);
}

//------------------------------------------------------------------------------
// Add node
function ShapeEditor::doAddNode(%this, %nodeName, %parentName, %transform)
{
   %action = %this.createAction(ActionAddNode, "Add node");
   %action.nodeName = %nodeName;
   %action.parentName = %parentName;
   %action.transform = %transform;

   %this.doAction(%action);
}

function ActionAddNode::doit(%this)
{
   if (ShapeEditor.shape.addNode(%this.nodeName, %this.parentName, %this.transform))
   {
      ShapeEdShapeView.updateNodeTransforms();

      // add the node to the tree and make it the current selection
      %parentName = %this.parentName;
      if (%this.parentName $= "")
         %parentName = "<root>";

      %parentId = %this.tree.findItemByName(%parentName);
      %id = %this.tree.insertItem(%parentId, %this.nodeName, 0, "");
      if (%id > 0)
      {
         %this.tree.clearSelection();
         %this.tree.selectItem(%id);
      }

      // Update object type hints
      ShapeEdSelectWindow.updateHints();

      return true;
   }
   return false;
}

function ActionAddNode::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.removeNode(%this.nodeName))
   {
      ShapeEdShapeView.updateNodeTransforms();

      // delete the node
      %this.tree.removeItem(%this.tree.findItemByName(%this.nodeName));
      if (%this.tree.getSelectedItem() <= 0)
         ShapeEdPropWindow.syncNodeDetails(-1);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();
   }
}

//------------------------------------------------------------------------------
// Remove node
function ShapeEditor::doRemoveNode(%this, %nodeName)
{
   %action = %this.createAction(ActionRemoveNode, "Remove node");
   %action.nodeName =%nodeName;
   %action.nodeChildIndex = %action.tree.getChildIndexByName(%nodeName);

   // Need to delete all child nodes of this node as well, so recursively collect
   // all of the names.
   %names = %this.getNodeNames(%nodeName, "");
   %action.nameCount = getFieldCount(%names);
   for (%i = 0; %i < %action.nameCount; %i++)
   {
      %action.names[%i] = getField(%names, %i);
      %action.deletedNames[%i] = %this.generateDeletedName(%action.names[%i]);
   }

   %this.doAction(%action);
}

function ActionRemoveNode::doit(%this)
{
   for (%i = 0; %i < %this.nameCount; %i++)
      ShapeEditor.shape.renameNode(%this.names[%i], %this.deletedNames[%i]);

   // remove tree items
   %id = %this.tree.findItemByName(%this.nodeName);
   if (%this.tree.getSelectedItem() == %id)
      ShapeEdPropWindow.syncNodeDetails(-1);
   if (%id != -1)
      %this.tree.removeItem(%id);

   ShapeEdShapeView.updateNodeTransforms();

   // Update object type hints
   ShapeEdSelectWindow.updateHints();

   return true;
}

function ActionRemoveNode::undo(%this)
{
   Parent::undo(%this);

   // 'Un-delete' the nodes in reverse order
   for (%i = %this.nameCount-1; %i >= 0; %i--)
      ShapeEditor.shape.renameNode(%this.deletedNames[%i], %this.names[%i]);

   // Restore the deleted items
   %this.tree.addNodeTree(%this.nodeName);

   // Inserting a new item puts it at the end of the siblings, but we want to
   // restore the original order as if the item was never deleted, so move it up
   // as required.
   %id = %this.tree.findItemByName(%this.nodeName);
   %childIndex = %this.tree.getChildIndexByName(%this.nodeName);
   while (%childIndex > %this.nodeChildIndex)
   {
      %this.tree.moveItemUp(%id);
      %childIndex--;
   }

   ShapeEdShapeView.updateNodeTransforms();

   // Update object type hints
   ShapeEdSelectWindow.updateHints();
}

//------------------------------------------------------------------------------
// Rename node
function ShapeEditor::doRenameNode(%this, %oldName, %newName)
{
   // First check if the last action was a rename to this node => if so, modify
   // that action instead of adding a new one
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if ((%last != -1) && (%last.class $= ActionRenameNode) && (%last.newName $= %oldName))
   {
      // Use the last action to do the rename, and modify it to merge the 2 renames
      %savedOldName = %last.oldName;

      %last.oldName = %oldName;
      %last.newName = %newName;
      %last.doit();
      ShapeEditor.setDirty(true);

      %last.oldName = %savedOldName;
   }
   else
   {
      %action = %this.createAction(ActionRenameNode, "Rename node");
      %action.oldName = %oldName;
      %action.newName = %newName;

      %this.doAction(%action);
   }
}

function ActionRenameNode::doit(%this)
{
   if (ShapeEditor.shape.renameNode(%this.oldName, %this.newName))
   {
      // rename the node in the tree
      %id = %this.tree.findItemByName(%this.oldName);
      %this.tree.editItem(%id, %this.newName, 0);
      if (%this.tree.getSelectedItem() == %id)
         ShapeEdNodes-->nodeName.setText(%this.newName);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();

      return true;
   }
   return false;
}

function ActionRenameNode::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.renameNode(%this.newName, %this.oldName))
   {
      // rename the node in the tree
      %id = %this.tree.findItemByName(%this.newName);
      %this.tree.editItem(%id, %this.oldName, 0);
      if (%this.tree.getSelectedItem() == %id)
         ShapeEdNodes-->nodeName.setText(%this.oldName);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();
   }
}

//------------------------------------------------------------------------------
// Set node parent
function ShapeEditor::doSetNodeParent(%this, %name, %parent)
{
   if (%parent $= "<root>")
      %parent = "";

   %action = %this.createAction(ActionSetNodeParent, "Set parent node");
   %action.nodeName = %name;
   %action.parentName = %parent;
   %action.oldParentName = ShapeEditor.shape.getNodeParentName(%name);

   %this.doAction(%action);
}

function ActionSetNodeParent::doit(%this)
{
   if (ShapeEditor.shape.setNodeParent(%this.nodeName, %this.parentName))
   {
      // update the node tree
      %id = %this.tree.findItemByName(%this.nodeName);
      if (%id != -1)
         %this.tree.removeItem(%id);
      %this.tree.addNodeTree(%this.nodeName);
      %this.tree.selectItem(%this.tree.findItemByName(%this.nodeName));

      ShapeEdShapeView.updateNodeTransforms();

      return true;
   }
   return false;
}

function ActionSetNodeParent::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.setNodeParent(%this.nodeName, %this.oldParentName))
   {
      // remove the collapsed do+undo set parent commands from the change set
      ShapeEditor.shape.popChangeCommands(1);

      // update the node tree
      %id = %this.tree.findItemByName(%this.nodeName);
      if (%id != -1)
         %this.tree.removeItem(%id);
      %this.tree.addNodeTree(%this.nodeName);
      %this.tree.selectItem(%this.tree.findItemByName(%this.nodeName));

      ShapeEdShapeView.updateNodeTransforms();
   }
}

//------------------------------------------------------------------------------
// Edit node transform
function ShapeEditor::doEditNodeTransform(%this, %nodeName, %newTransform, %isWorld)
{
   // First check if the last action was an edit transform for this node => if so,
   // modify that action instead of adding a new one
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if ((%last != -1) && (%last.class $= ActionEditNodeTransform) && (%last.nodeName $= %nodeName))
   {
      // Use the last action to do the edit, and modify it so it only applies
      // the latest transform
      %last.newTransform = %newTransform;
      %last.isWorld = %isWorld;
      %last.doit();
      ShapeEditor.setDirty(true);
   }
   else
   {
      %action = %this.createAction(ActionEditNodeTransform, "Edit node transform");
      %action.nodeName = %nodeName;
      %action.newTransform = %newTransform;
      %action.isWorld = %isWorld;
      %action.oldTransform = %this.shape.getNodeTransform(%nodeName, %isWorld);

      %this.doAction(%action);
   }
}

function ActionEditNodeTransform::doit(%this)
{
   ShapeEditor.shape.setNodeTransform(%this.nodeName, %this.newTransform, %this.isWorld);
   ShapeEdShapeView.updateNodeTransforms();

   // Update the node transform fields if necessary
   %id = %this.tree.findItemByName(%this.nodeName);
   if (%this.tree.getSelectedItem() == %id)
   {
      %pos = getWords(%this.newTransform, 0, 2);
      %rot = getWords(%this.newTransform, 3, 6);
      if (ShapeEdNodes-->nodePosition.getText() !$= %pos)
         ShapeEdNodes-->nodePosition.setText(%pos);
      if (ShapeEdNodes-->nodeRotation.getText() !$= %rot)
         ShapeEdNodes-->nodeRotation.setText(%rot);
   }

   return true;
}

function ActionEditNodeTransform::undo(%this)
{
   Parent::undo(%this);

   ShapeEditor.shape.setNodeTransform(%this.nodeName, %this.oldTransform, %this.isWorld);
   ShapeEdShapeView.updateNodeTransforms();

   // remove the collapsed do+undo set transform commands from the change set
   ShapeEditor.shape.popChangeCommands(1);

   %id = %this.tree.findItemByName(%this.nodeName);
   if (%this.tree.getSelectedItem() == %id)
      ShapeEdPropWindow.syncNodeDetails(%id);
}

//------------------------------------------------------------------------------
// Add sequence
function ShapeEditor::doAddSequence(%this, %seqName, %from, %start, %end)
{
   %action = %this.createAction(ActionAddSequence, "Add sequence");
   %action.seqName = %seqName;
   %action.from = %from;
   %action.start = %start;
   %action.end = %end;

   %this.doAction(%action);
}

function ActionAddSequence::doit(%this)
{
   // If adding this sequence from an existing sequence, make a backup copy of
   // the existing sequence first, so we can edit the start/end frames later
   // without having to worry if the original source sequence has changed.
   %this.origFrom = %this.from;
   if (ShapeEditor.shape.getSequenceIndex(%this.from) >= 0)
   {
      %this.from = ShapeEditor.getUniqueName("sequence", "__backup__" @ %this.origFrom);
      ShapeEditor.shape.addSequence(%this.origFrom, %this.from);
   }

   if (ShapeEditor.shape.addSequence(%this.from, %this.seqName, %this.start, %this.end))
   {
      // Add the new sequence to the list, and make it the current selection
      %row = ShapeEdSequenceList.addItem(%this.seqName);
      ShapeEdSequenceList.setSelectedRow(%row);
      ShapeEdSequenceList.scrollVisible(%row);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();

      return true;
   }
   return false;
}

function ActionAddSequence::undo(%this)
{
   Parent::undo(%this);

   // Remove the backup sequence if one was created
   if (%this.origFrom !$= %this.from)
   {
      ShapeEditor.shape.removeSequence(%this.from);
      %this.from = %this.origFrom;
   }

   // Remove the actual sequence
   if (ShapeEditor.shape.removeSequence(%this.seqName))
   {
      ShapeEdSequenceList.removeItem(%this.seqName);
      ShapeEdPropWindow.syncSequenceDetails();

      // Update object type hints
      ShapeEdSelectWindow.updateHints();
   }
}

//------------------------------------------------------------------------------
// Remove sequence

function ShapeEditor::doRemoveSequence(%this, %seqName)
{
   %action = %this.createAction(ActionRemoveSequence, "Remove sequence");
   %action.seqName = %seqName;
   %action.deletedName = %this.generateDeletedName(%seqName);
   %action.seqIndex = ShapeEdSequenceList.getItemIndex(%seqName);

   %this.doAction(%action);
}

function ActionRemoveSequence::doit(%this)
{
   if (ShapeEditor.shape.renameSequence(%this.seqName, %this.deletedName))
   {
      ShapeEdSequenceList.removeItem(%this.seqName);
      ShapeEdPropWindow.syncSequenceDetails();

      // Update object type hints
      ShapeEdSelectWindow.updateHints();

      return true;
   }
   return false;
}

function ActionRemoveSequence::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.renameSequence(%this.deletedName, %this.seqName))
   {
      ShapeEdSequenceList.insertItem(%this.seqName, %this.seqIndex);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();
   }
}

//------------------------------------------------------------------------------
// Rename sequence
function ShapeEditor::doRenameSequence(%this, %oldName, %newName)
{
   // First check if the last action was a rename to this sequence => if so, modify
   // that action instead of adding a new one
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if ((%last != -1) && (%last.class $= ActionRenameSequence) && (%last.newName $= %oldName))
   {
      // Use the last action to do the rename, and modify it to merge the 2 renames
      %savedOldName = %last.oldName;

      %last.oldName = %oldName;
      %last.newName = %newName;
      %last.doit();
      ShapeEditor.setDirty(true);

      %last.oldName = %savedOldName;
   }
   else
   {
      %action = %this.createAction(ActionRenameSequence, "Rename sequence");
      %action.oldName = %oldName;
      %action.newName = %newName;

      %this.doAction(%action);
   }
}

function ActionRenameSequence::doit(%this)
{
   if (ShapeEditor.shape.renameSequence(%this.oldName, %this.newName))
   {
      // rename the sequence in the list
      ShapeEdSequenceList.editColumn(%this.oldName, 0, %this.newName);
      if (ShapeEdSequenceList.getSelectedName() $= %this.newName)
         ShapeEdSequences-->seqName.setText(%this.newName);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();

      return true;
   }
   return false;
}

function ActionRenameSequence::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.renameSequence(%this.newName, %this.oldName))
   {
      // rename the sequence in the list
      ShapeEdSequenceList.editColumn(%this.newName, 0, %this.oldName);
      if (ShapeEdSequenceList.getSelectedName() $= %this.oldName)
         ShapeEdSequences-->seqName.setText(%this.oldName);

      // Update object type hints
      ShapeEdSelectWindow.updateHints();
   }
}

//------------------------------------------------------------------------------
// Edit sequence source data (parent, start or end)
function ShapeEditor::doEditSeqSource(%this, %seqName, %from, %start, %end)
{
   %usingProxy = 0;
   if (%from $= %seqName)
   {
      // Attempting to modify the source of an internal sequence => need to
      // keep it around and use it as the 'from'
      %from = "__backup__" @ %seqName;
      %usingProxy = 1;
   }

   // First check if the last action was a source edit to this sequence => if so,
   // modify that action instead of adding a new one
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if ((%last != -1) && (%last.class $= ActionEditSeqSource) && (%last.seqName $= %seqName))
   {
      // Undo the last action, then redo it with modified parameters
      %last.undo();
      %last.from = %from;
      %last.start = %start;
      %last.end = %end;
      %last.redo();
      ShapeEditor.setDirty(true);
   }
   else
   {
      %action = %this.createAction(ActionEditSeqSource, "Edit sequence source data");
      %action.seqName = %seqName;
      %action.from = %from;
      %action.start = %start;
      %action.end = %end;
      if (%usingProxy)
         %action.deletedName = %action.from;
      else
         %action.deletedName = %this.generateDeletedName(%seqName);

      %this.doAction(%action);
   }
}

function ActionEditSeqSource::doit(%this)
{
   %this.origFrom = %this.from;

   // If the sequence parent is changing to an existing sequence, make a backup
   // copy of that sequence first, so we can edit the start/end frames later
   // without having to worry if the original source sequence has changed.
   %oldParent = getFields(ShapeEditor.shape.getSequenceSource(%this.seqName), 0, 1);
   if (%this.from !$= %oldParent)
   {
      if ((strstr(%this.from, "__backup__") != 0) && 
       (ShapeEditor.shape.getSequenceIndex(%this.from) >= 0))
      {
         %this.from = ShapeEditor.getUniqueName("sequence", "__backup__" @ %this.origFrom);
         ShapeEditor.shape.addSequence(%this.origFrom, %this.from);
      }
   }

   if (ShapeEditor.shape.renameSequence(%this.seqName, %this.deletedName) &&
      ShapeEditor.shape.addSequence(%this.from, %this.seqName, %this.start, %this.end))
   {
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         ShapeEdSequenceList.editColumn(%this.seqName, 3, %this.end - %this.start + 1);
         ShapeEdPropWindow.syncPlaybackDetails();
      }

      return true;
   }
   return false;
}

function ActionEditSeqSource::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.removeSequence(%this.seqName) &&
      ShapeEditor.shape.renameSequence(%this.deletedName, %this.seqName))
   {
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         ShapeEdSequenceList.editColumn(%this.seqName, 3, %this.end - %this.start + 1);
         ShapeEdPropWindow.syncPlaybackDetails();
      }
   }

   // Remove the backup sequence if required
   if (%this.from !$= %this.origFrom)
   {
      ShapeEditor.shape.removeSequence(%this.from);
      %this.from = %this.origFrom;
   }
}

//------------------------------------------------------------------------------
// Edit cyclic flag
function ShapeEditor::doEditCyclic(%this, %seqName, %cyclic)
{
   %action = %this.createAction(ActionEditCyclic, "Toggle cyclic flag");
   %action.seqName = %seqName;
   %action.cyclic = %cyclic;

   %this.doAction(%action);
}

function ActionEditCyclic::doit(%this)
{
   if (ShapeEditor.shape.setSequenceCyclic(%this.seqName, %this.cyclic))
   {
      // update the cyclic setting
      ShapeEdSequenceList.editColumn(%this.seqName, 1, %this.cyclic ? "yes" : "no");
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         ShapeEdSequences-->cyclicFlag.setStateOn(%this.cyclic);

         // Apply the same transformation to the proxy animation if necessary
         if (ShapeEdShapeView.usingProxySequence)
            ShapeEditor.shape.setSequenceCyclic("__proxy__", %this.cyclic);
      }
      return true;
   }
   return false;
}

function ActionEditCyclic::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.setSequenceCyclic(%this.seqName, !%this.cyclic))
   {
      // update the cyclic setting
      ShapeEdSequenceList.editColumn(%this.seqName, 1, !%this.cyclic ? "yes" : "no");
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         ShapeEdSequences-->cyclicFlag.setStateOn(!%this.cyclic);

         // Apply the same transformation to the proxy animation if necessary
         if (ShapeEdShapeView.usingProxySequence)
            ShapeEditor.shape.setSequenceCyclic("__proxy__", !%this.cyclic);
      }
   }
}

//------------------------------------------------------------------------------
// Edit blend properties
function ShapeEditor::doEditBlend(%this, %seqName, %blend, %blendSeq, %blendFrame)
{
   %action = %this.createAction(ActionEditBlend, "Edit blend properties");
   %action.seqName = %seqName;
   %action.blend = %blend;
   %action.blendSeq = %blendSeq;
   %action.blendFrame = %blendFrame;

   // Store the current blend settings
   %oldBlend = ShapeEditor.shape.getSequenceBlend(%seqName);
   %action.oldBlend = getField(%oldBlend, 0);
   %action.oldBlendSeq = getField(%oldBlend, 1);
   %action.oldBlendFrame = getField(%oldBlend, 2);

   // Use new values if the old ones do not exist (for blend sequences embedded
   // in the DTS/DSQ file)
   if (%action.oldBlendSeq $= "")
      %action.oldBlendSeq = %action.blendSeq;
   if (%action.oldBlendFrame $= "")
      %action.oldBlendFrame = %action.blendFrame;

   %this.doAction(%action);
}

function ActionEditBlend::doit(%this)
{
   // If we are changing the blend reference (rather than just toggling the flag)
   // we need to undo the current blend first.
   if (%this.blend && %this.oldBlend)
   {
      if (!ShapeEditor.shape.setSequenceBlend(%this.seqName, false, %this.oldBlendSeq, %this.oldBlendFrame))
         return false;
   }

   if (ShapeEditor.shape.setSequenceBlend(%this.seqName, %this.blend, %this.blendSeq, %this.blendFrame))
   {
      // update the blend setting
      ShapeEdSequenceList.editColumn(%this.seqName, 2, %this.blend ? "yes" : "no");
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         ShapeEdSequences-->blendFlag.setStateOn(%this.blend);
         ShapeEdSequences-->blendSeq.setText(%this.blendSeq);
         ShapeEdSequences-->blendFrame.setText(%this.blendFrame);

         // Apply the same transformation to the proxy animation if necessary
         if (ShapeEdShapeView.usingProxySequence)
         {
            if (%this.blend && %this.oldBlend)
               ShapeEditor.shape.setSequenceBlend("__proxy__", false, %this.oldBlendSeq, %this.oldBlendFrame);
            ShapeEditor.shape.setSequenceBlend("__proxy__", %this.blend, %this.blendSeq, %this.blendFrame);
         }

         ShapeEdShapeView.updateNodeTransforms();
      }

      return true;
   }
   return false;
}

function ActionEditBlend::undo(%this)
{
   Parent::undo(%this);

   // If we are changing the blend reference (rather than just toggling the flag)
   // we need to undo the current blend first.
   if (%this.blend && %this.oldBlend)
   {
      if (!ShapeEditor.shape.setSequenceBlend(%this.seqName, false, %this.blendSeq, %this.blendFrame))
         return;
   }

   if (ShapeEditor.shape.setSequenceBlend(%this.seqName, %this.oldBlend, %this.oldBlendSeq, %this.oldBlendFrame))
   {
      // update the blend setting
      ShapeEdSequenceList.editColumn(%this.seqName, 2, %this.oldBlend ? "yes" : "no");
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         ShapeEdSequences-->blendFlag.setStateOn(%this.oldBlend);
         ShapeEdSequences-->blendSeq.setText(%this.oldBlendSeq);
         ShapeEdSequences-->blendFrame.setText(%this.oldBlendFrame);

         // Apply the same transformation to the proxy animation if necessary
         if (ShapeEdShapeView.usingProxySequence)
         {
            if (%this.blend && %this.oldBlend)
               ShapeEditor.shape.setSequenceBlend("__proxy__", false, %this.blendSeq, %this.blendFrame);
            ShapeEditor.shape.setSequenceBlend("__proxy__", %this.oldBlend, %this.oldBlendSeq, %this.oldBlendFrame);
         }

         ShapeEdShapeView.updateNodeTransforms();
      }
   }
}

//------------------------------------------------------------------------------
// Edit sequence priority
function ShapeEditor::doEditSequencePriority(%this, %seqName, %newPriority)
{
   // First check if the last action was an edit to this sequence priority => if
   // so, modify that action instead of adding a new one
   %last = EUndoManager.getUndoAction(EUndoManager.getUndoCount() - 1);
   if ((%last != -1) && (%last.class $= ActionEditSequencePriority) && (%last.seqName $= %seqName))
   {
      // Use the last action to do the edit, and modify it so it only applies
      // the latest priority
      %last.newPriority = %newPriority;
      %last.doit();
      ShapeEditor.setDirty(true);
   }
   else
   {
      %action = %this.createAction(ActionEditSequencePriority, "Edit sequence priority");
      %action.seqName = %seqName;
      %action.newPriority = %newPriority;
      %action.oldPriority = %this.shape.getSequencePriority(%seqName);

      %this.doAction(%action);
   }
}

function ActionEditSequencePriority::doit(%this)
{
   if (ShapeEditor.shape.setSequencePriority(%this.seqName, %this.newPriority))
   {
      // Update priority in GUI
      ShapeEdSequenceList.editColumn(%this.seqName, 4, %this.newPriority);
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
         ShapeEdSequences-->priority.setText(%this.newPriority);
      return true;
   }
   return false;
}

function ActionEditSequencePriority::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.setSequencePriority(%this.seqName, %this.oldPriority))
   {
      // remove the collapsed do+undo set priority commands from the change set
      ShapeEditor.shape.popChangeCommands(1);

      // Update priority in GUI
      ShapeEdSequenceList.editColumn(%this.seqName, 4, %this.oldPriority);
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
         ShapeEdSequences-->priority.setText(%this.oldPriority);
   }
}

//------------------------------------------------------------------------------
// Edit sequence ground speed
function ShapeEditor::doEditSequenceGroundSpeed(%this, %seqName, %newSpeed)
{
   %action = %this.createAction(ActionEditSequenceGroundSpeed, "Edit sequence ground speed");
   %action.seqName = %seqName;
   %action.newSpeed = %newSpeed;
   %action.oldSpeed = %this.shape.getSequenceGroundSpeed(%seqName);

   %this.doAction(%action);
}

function ActionEditSequenceGroundSpeed::doit(%this)
{
   if (ShapeEditor.shape.setSequenceGroundSpeed(%this.seqName, %this.newSpeed))
   {
      // Update ground speed text if this sequence is selected
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         // @todo
         //ShapeEdSequences-->seqGroundSpeed.setText(%this.newSpeed);
      }
      return true;
   }
   return false;
}

function ActionEditSequenceGroundSpeed::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.setSequenceGroundSpeed(%this.seqName, %this.oldSpeed))
   {
      // remove the collapsed do+undo set ground speed commands from the change set
      ShapeEditor.shape.popChangeCommands(1);

      // Update ground speed text if this sequence is selected
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
      {
         // @todo
         //ShapeEdSequences-->seqGroundSpeed.setText(%this.oldSpeed);
      }
   }
}

//------------------------------------------------------------------------------
// Add trigger
function ShapeEditor::doAddTrigger(%this, %seqName, %frame, %state)
{
   %action = %this.createAction(ActionAddTrigger, "Add trigger");
   %action.seqName = %seqName;
   %action.frame = %frame;
   %action.state = %state;

   %this.doAction(%action);
}

function ActionAddTrigger::doit(%this)
{
   if (ShapeEditor.shape.addTrigger(%this.seqName, %this.frame, %this.state))
   {
      // Add trigger to list if this sequence is selected
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
         ShapeEdTriggerList.addItem(%this.frame, %this.state);
      return true;
   }
   return false;
}

function ActionAddTrigger::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.removeTrigger(%this.seqName, %this.frame, %this.state))
   {
      // Remove trigger from list if this sequence is selected
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
         ShapeEdTriggerList.removeItem(%this.frame, %this.state);
   }
}

//------------------------------------------------------------------------------
// Remove trigger
function ShapeEditor::doRemoveTrigger(%this, %seqName, %frame, %state)
{
   %action = %this.createAction(ActionRemoveTrigger, "Remove trigger");
   %action.seqName = %seqName;
   %action.frame = %frame;
   %action.state = %state;

   %this.doAction(%action);
}

function ActionRemoveTrigger::doit(%this)
{
   if (ShapeEditor.shape.removeTrigger(%this.seqName, %this.frame, %this.state))
   {
      // Remove trigger from list if this sequence is selected
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
         ShapeEdTriggerList.removeItem(%this.frame, %this.state);
      return true;
   }
   return false;
}

function ActionRemoveTrigger::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.addTrigger(%this.seqName, %this.frame, %this.state))
   {
      // Add trigger to list if this sequence is selected
      if (ShapeEdSequenceList.getSelectedName() $= %this.seqName)
         ShapeEdTriggerList.addItem(%this.frame, %this.state);
   }
}

//------------------------------------------------------------------------------
// Edit trigger
function ShapeEditor::doEditTrigger(%this, %seqName, %oldFrame, %oldState, %frame, %state)
{
   %action = %this.createAction(ActionEditTrigger, "Edit trigger");
   %action.seqName = %seqName;
   %action.oldFrame = %oldFrame;
   %action.oldState = %oldState;
   %action.frame = %frame;
   %action.state = %state;

   %this.doAction(%action);
}

function ActionEditTrigger::doit(%this)
{
   if (ShapeEditor.shape.addTrigger(%this.seqName, %this.frame, %this.state) &&
      ShapeEditor.shape.removeTrigger(%this.seqName, %this.oldFrame, %this.oldState))
   {
      ShapeEdTriggerList.updateItem(%this.oldFrame, %this.oldState, %this.frame, %this.state);
      return true;
   }
   return false;
}

function ActionEditTrigger::undo(%this)
{
   Parent::undo(%this);

   if (ShapeEditor.shape.addTrigger(%this.seqName, %this.oldFrame, %this.oldState) &&
      ShapeEditor.shape.removeTrigger(%this.seqName, %this.frame, %this.state))
      ShapeEdTriggerList.updateItem(%this.frame, %this.state, %this.oldFrame, %this.oldState);
}
