// @todo:
//
// - split node transform editboxes into X Y Z and rot X Y Z with spin controls
//   to allow easier manual editing
// - add groundspeed editing (use same format as node transform editing)
//
// Known bugs/limitations:
//
// - resizing the GuiTextListCtrl should resize the columns as well
// - modifying the from/in/out properties of a sequence will change the sequence
//   order in the shape (since it results in remove/add sequence commands)
// - deleting a node should not delete its children as well?
//

//------------------------------------------------------------------------------
// Utility Methods
//------------------------------------------------------------------------------

if (!isObject(ShapeEditor)) new ScriptObject(ShapeEditor)
{
   shape = -1;
   deletedCount = 0;
};

function ShapeEditor::getObjectShapeFile(%this, %obj)
{
   // Get the path to the shape file used by the given object (not perfect, but
   // works for the vast majority of object types)
   %path = "";
   if (%obj.isMemberOfClass("TSStatic"))
      %path = %obj.shapeName;
   else if (%obj.isMemberOfClass("GameBase"))
      %path = %obj.getDataBlock().shapeFile;
   return %path;
}

// Check if the given name already exists
function ShapeEditor::nameExists(%this, %type, %name)
{
   if (ShapeEditor.shape == -1)
      return false;

   if (%type $= "node")
      return (ShapeEditor.shape.getNodeIndex(%name) >= 0);
   else if (%type $= "sequence")
      return (ShapeEditor.shape.getSequenceIndex(%name) >= 0);
}

function ShapeEditor::hintNameExists(%this, %type, %name)
{
   if (ShapeEditor.nameExists(%type, %name))
      return true;

   // If the name contains spaces, try replacing with underscores
   %name = strreplace(%name, " ", "_");
   if (ShapeEditor.nameExists(%type, %name))
      return true;

   return false;
}

// Generate a unique name from a given base by appending an integer
function ShapeEditor::getUniqueName(%this, %type, %name)
{
   for (%idx = 1; %idx < 100; %idx++)
   {
      %uniqueName = %name @ %idx;
      if (!%this.nameExists(%type, %uniqueName))
         break;
   }

   return %uniqueName;
}

function ShapeEditor::generateDeletedName(%this, %name)
{
   // Generate a unique name to use when deleting a node or sequence in the editor
   ShapeEditor.deletedCount++;
   return "__deleted_" @ %name @ "_" @ ShapeEditor.deletedCount;
}

function ShapeEditor::isDeletedName(%this, %name)
{
   return (strstr(%name, "__deleted_") == 0);
}

// Recursively get names for a node and its children
function ShapeEditor::getNodeNames(%this, %nodeName, %names)
{
   // Ignore deleted nodes
   if (!%this.isDeletedName(%nodeName))
   {
      %count = %this.shape.getNodeChildCount(%nodeName);
      for (%i = 0; %i < %count; %i++)
      {
         %childName = %this.shape.getNodeChildName(%nodeName, %i);
         %names = %this.getNodeNames(%childName, %names);
      }

      %names = trim(%names TAB %nodeName);
   }
   return %names;
}

function ShapeEditor::isDirty(%this)
{
   return ((%this.shape > 0) && ShapeEdPropWindow-->saveBtn.isActive());
}

function ShapeEditor::setDirty(%this, %dirty)
{
   if (%dirty)
      ShapeEdSelectWindow.text = "Shapes *";
   else
      ShapeEdSelectWindow.text = "Shapes";

   ShapeEdPropWindow-->saveBtn.setActive(%dirty);
}

function ShapeEditor::saveChanges(%this)
{
   if (ShapeEditor.shape > 0)
   {
      %savepath = ShapeEditor.shape.baseShape;
      ShapeEditor.shape.save(filePath(%savepath) @ "/" @ fileBase(%savepath) @ ".cs");
      ShapeEditor.setDirty(false);
   }
}

// Capitalise the first letter of the input string
function strcapitalise(%str)
{
   %len = strlen(%str);
   return strupr(getSubStr(%str,0,1)) @ getSubStr(%str,1,%len-1);
}

//------------------------------------------------------------------------------
// Shape Selection
//------------------------------------------------------------------------------

function ShapeEditor::findConstructor(%this, %path)
{
   %count = TSShapeConstructorGroup.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %obj = TSShapeConstructorGroup.getObject(%i);
      if (%obj.baseShape $= %path)
         return %obj;
   }
   return -1;
}

function ShapeEditor::createConstructor(%this, %path)
{
   %name = strcapitalise(fileBase(%path)) @ strcapitalise(getSubStr(fileExt(%path), 1, 3));
   %name = strreplace(%name, "-", "_");
   %name = strreplace(%name, ".", "_");
   %name = getUniqueName(%name);
   return new TSShapeConstructor(%name) { baseShape = %path; };
}

// Handle a selection in the shape selector list
function ShapeEdSelectWindow::onSelect(%this, %path)
{
   // Prompt user to save the old shape if it is dirty
   if (ShapeEditor.isDirty())
   {
      %cmd = "ColladaImportDlg.showDialog(\"" @ %path @ "\", \"ShapeEditor.selectShape(\\\"" @ %path @ "\\\", ";
      MessageBoxYesNoCancel("Shape Modified", "Would you like to save your changes?", %cmd @ "true);\");", %cmd @ "false);\");");
   }
   else
   {
      %cmd = "ShapeEditor.selectShape(\"" @ %path @ "\", false);";
      ColladaImportDlg.showDialog(%path, %cmd);
   }
}

function ShapeEditor::selectShape(%this, %path, %saveOld)
{
   if (%saveOld)
   {
      // Save changes to a TSShapeConstructor datablock
      %this.saveChanges();
   }
   else if (ShapeEditor.shape > 0)
   {
      // Undo all changes
      while (EUndoManager.getUndoCount() > 0)
         EUndoManager.undo();
   }

   // Initialise the shape preview window
   ShapeEdShapeView.setModel("");
   ShapeEdShapeView.setModel(%path);
   ShapeEdShapeView.fitToShape();

   EUndoManager.clearAll();
   ShapeEditor.setDirty(false);

   // Get (or create) the TSShapeConstructor object for this shape
   ShapeEditor.shape = ShapeEditor.findConstructor(%path);
   if (ShapeEditor.shape <= 0)
   {
      ShapeEditor.shape = %this.createConstructor(%path);
      if (ShapeEditor.shape <= 0)
      {
         error("ShapeEditor: Error - could not select " @ %path);
         return;
      }
   }

   // Initialise the node tree
   ShapeEdNodeTreeView.removeItem(0);
   %rootId = ShapeEdNodeTreeView.insertItem(0, "<root>", 0, "");
   %count = ShapeEditor.shape.getNodeCount();
   for (%i = 0; %i < %count; %i++)
   {
      %name = ShapeEditor.shape.getNodeName(%i);
      if (ShapeEditor.shape.getNodeParentName(%name) $= "")
         ShapeEdNodeTreeView.addNodeTree(%name);
   }
   ShapeEdPropWindow.syncNodeDetails(-1);

   // Initialise the sequence list
   ShapeEdSequenceList.clear();
   ShapeEdSequenceList.addRow(-1, "Name" TAB "Cyclic" TAB "Blend" TAB "Frames" TAB "Priority");
   ShapeEdSequenceList.setRowActive(-1, false);
   ShapeEdSequenceList.addRow(0, "<rootpose>" TAB "" TAB "" TAB "" TAB "");
   %count = ShapeEditor.shape.getSequenceCount();
   for (%i = 0; %i < %count; %i++)
   {
      %name = ShapeEditor.shape.getSequenceName(%i);

      // Ignore __backup__ sequences (only used by editor)
      if (strstr(%name, "__backup__") == 0)
         continue;

      ShapeEdSequenceList.addItem(%name);
   }
   ShapeEdSequenceList.setSelectedRow(1); // select the <rootpose>

   // Update object type hints
   ShapeEdSelectWindow.updateHints();

   // Update editor status bar
   EditorGuiStatusBar.setSelection(%path);
}

// Handle a selection in the MissionGroup shape selector
function ShapeEdShapeTreeView::onSelect(%this, %obj)
{
   %path = ShapeEditor.getObjectShapeFile(%obj);
   if (%path !$= "")
      ShapeEdSelectWindow.onSelect(%path);

   // Set the object type (for required nodes and sequences display)
   %objClass = %obj.getClassName();
   %hintId = -1;

   %count = ShapeHintGroup.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %hint = ShapeHintGroup.getObject(%i);
      if (%objClass $= %hint.objectType)
      {
         %hintId = %hint;
         break;
      }
      else if (isMemberOfClass(%objClass, %hint.objectType))
      {
         %hintId = %hint;
      }
   }
   ShapeEdHintMenu.setSelected(%hintId);
}

// Find all DTS or Collada models. Note: most of this section was shamelessly
// stolen from creater.ed.cs => great work whoever did the original!
function ShapeEdSelectWindow::navigate(%this, %address)
{
   // Freeze the icon array so it doesn't update until we've added all of the
   // icons
   %this-->shapeLibrary.frozen = true;
   %this-->shapeLibrary.clear();
   ShapeEdSelectMenu.clear();

   %filePatterns = "*.dts" TAB "*.dae" TAB "*.kmz";
   %fullPath = findFirstFileMultiExpr(%filePatterns);

   while (%fullPath !$= "")
   {
      // Ignore cached DTS files
      if (strstr(%fullPath, "cached.dts") != -1)
      {
         %fullPath = findNextFileMultiExpr(%filePatterns);
         continue;
      }

      // Ignore assets in the tools folder
      %fullPath = makeRelativePath(%fullPath, getMainDotCSDir());
      %splitPath = strreplace(%fullPath, "/", " ");
      if (getWord(%splitPath, 0) $= "tools")
      {
         %fullPath = findNextFileMultiExpr(%filePatterns);
         continue;
      }

      %dirCount = getWordCount(%splitPath) - 1;
      %pathFolders = getWords(%splitPath, 0, %dirCount - 1);

      // Add this file's path (parent folders) to the
      // popup menu if it isn't there yet.
      %temp = strreplace(%pathFolders, " ", "/");
      %r = ShapeEdSelectMenu.findText(%temp);
      if (%r == -1)
         ShapeEdSelectMenu.add(%temp, 0);

      // Is this file in the current folder?
      if (stricmp(%pathFolders, %address) == 0)
      {
         %this.addShapeIcon(%fullPath);
      }
      // Then is this file in a subfolder we need to add
      // a folder icon for?
      else
      {
         %wordIdx = 0;
         %add = false;

         if (%address $= "")
         {
            %add = true;
            %wordIdx = 0;
         }
         else
         {
            for (; %wordIdx < %dirCount; %wordIdx++)
            {
               %temp = getWords(%splitPath, 0, %wordIdx);
               if (stricmp(%temp, %address) == 0)
               {
                  %add = true;
                  %wordIdx++;
                  break;
               }
            }
         }

         if (%add == true)
         {
            %folder = getWord(%splitPath, %wordIdx);

            // Add folder icon if not already present
            %ctrl = %this.findIconCtrl(%folder);
            if (%ctrl == -1)
               %this.addFolderIcon(%folder);
         }
      }

      %fullPath = findNextFileMultiExpr(%filePatterns);
   }

   %this-->shapeLibrary.sort("alphaIconCompare");
   for (%i = 0; %i < %this-->shapeLibrary.getCount(); %i++)
      %this-->shapeLibrary.getObject(%i).autoSize = false;

   %this-->shapeLibrary.frozen = false;
   %this-->shapeLibrary.refresh();

   %this.address = %address;

   %str = strreplace(%address, " ", "/");
   ShapeEdSelectMenu.setText(%str);
   ShapeEdSelectMenu.sort();
}

function ShapeEdSelectWindow::navigateDown(%this, %folder)
{
   if (%this.address $= "")
      %address = %folder;
   else
      %address = %this.address SPC %folder;

   // Because this is called from an IconButton::onClick command
   // we have to wait a tick before actually calling navigate, else
   // we would delete the button out from under itself.
   %this.schedule(1, "navigate", %address);
}

function ShapeEdSelectWindow::navigateUp(%this)
{
   %count = getWordCount(%this.address);

   if (%count == 0)
      return;

   if (%count == 1)
      %address = "";
   else
      %address = getWords(%this.address, 0, %count - 2);

   %this.navigate(%address);
}

function ShapeEdSelectWindow::findIconCtrl(%this, %name)
{
   for (%i = 0; %i < %this-->shapeLibrary.getCount(); %i++)
   {
      %ctrl = %this-->shapeLibrary.getObject(%i);
      if (%ctrl.text $= %name)
         return %ctrl;
   }
   return -1;
}

function ShapeEdSelectWindow::createIcon(%this)
{
   %ctrl = new GuiIconButtonCtrl()
   {
      profile = "GuiCreatorIconButtonProfile";
      iconLocation = "Left";
      textLocation = "Right";
      extent = "348 19";
      textMargin = 8;
      buttonMargin = "2 2";
      autoSize = true;
      buttonType = "radioButton";
      groupNum = "-1";   
   };

   return %ctrl;
}

function ShapeEdSelectWindow::addFolderIcon(%this, %text)
{
   %ctrl = %this.createIcon();

   %ctrl.altCommand = "ShapeEdSelectWindow.navigateDown(\"" @ %text @ "\");";
   %ctrl.iconBitmap = "core/art/gui/images/folder.png";
   %ctrl.text = %text;
   %ctrl.tooltip = %text;
   %ctrl.class = "CreatorFolderIconBtn";
   
   %ctrl.buttonType = "radioButton";
   %ctrl.groupNum = "-1";
   
   %this-->shapeLibrary.addGuiControl(%ctrl);
}

function ShapeEdSelectWindow::addShapeIcon(%this, %fullPath)
{
   %ctrl = %this.createIcon();

   %ext = fileExt(%fullPath);
   %file = fileBase(%fullPath);
   %fileLong = %file @ %ext;
   %tip = %fileLong NL
          "Size: " @ fileSize(%fullPath) / 1000.0 SPC "KB" NL
          "Date Created: " @ fileCreatedTime(%fullPath) NL
          "Last Modified: " @ fileModifiedTime(%fullPath);

   %ctrl.altCommand = "ShapeEdSelectWindow.onSelect(\"" @ %fullPath @ "\");";
   %ctrl.iconBitmap = ((%ext $= ".dts") ? EditorIconRegistry::findIconByClassName("TSStatic") : "tools/gui/images/iconCollada");
   %ctrl.text = %file;
   %ctrl.class = "CreatorStaticIconBtn";
   %ctrl.tooltip = %tip;
   
   %ctrl.buttonType = "radioButton";
   %ctrl.groupNum = "-1";
   
   %this-->shapeLibrary.addGuiControl(%ctrl);
}

function ShapeEdSelectMenu::onSelect(%this, %id, %text)
{
   %split = strreplace(%text, "/", " ");
   ShapeEdSelectWindow.navigate(%split);
}

//------------------------------------------------------------------------------
// Shape Hints
//------------------------------------------------------------------------------

function ShapeEdHintMenu::onSelect(%this, %id, %text)
{
   ShapeEdSelectWindow.updateHints();
}

function ShapeEdSelectWindow::updateHints(%this)
{
   %objectType = ShapeEdHintMenu.getText();

   ShapeEdSelectWindow-->nodeHints.freeze(true);
   ShapeEdSelectWindow-->sequenceHints.freeze(true);

   // Move all current hint controls to a holder SimGroup
   for (%i = ShapeEdSelectWindow-->nodeHints.getCount()-1; %i >= 0; %i--)
      ShapeHintControls.add(ShapeEdSelectWindow-->nodeHints.getObject(%i));
   for (%i = ShapeEdSelectWindow-->sequenceHints.getCount()-1; %i >= 0; %i--)
      ShapeHintControls.add(ShapeEdSelectWindow-->sequenceHints.getObject(%i));

   // Update node and sequence hints, modifying and/or creating gui controls as needed
   for (%i = 0; %i < ShapeHintGroup.getCount(); %i++)
   {
      %hint = ShapeHintGroup.getObject(%i);
      if ((%objectType $= %hint.objectType) || isMemberOfClass(%objectType, %hint.objectType))
      {
         for (%idx = 0; %hint.node[%idx] !$= ""; %idx++)
            ShapeEdHintMenu.processHint("node", %hint.node[%idx]);

         for (%idx = 0; %hint.sequence[%idx] !$= ""; %idx++)
            ShapeEdHintMenu.processHint("sequence", %hint.sequence[%idx]);
      }
   }

   ShapeEdSelectWindow-->nodeHints.freeze(false);
   ShapeEdSelectWindow-->nodeHints.updateStack();
   ShapeEdSelectWindow-->sequenceHints.freeze(false);
   ShapeEdSelectWindow-->sequenceHints.updateStack();

}

function ShapeEdHintMenu::processHint(%this, %type, %hint)
{
   %name = getField(%hint, 0);
   %desc = getField(%hint, 1);

   // check for arrayed names (ending in 0-N or 1-N)
   %pos = strstr(%name, "0-");
   if (%pos == -1)
      %pos = strstr(%name, "1-");

   if (%pos > 0)
   {
      // arrayed name => add controls for each name in the array, but collapse
      // consecutive indices where possible. eg.  if the model only has nodes
      // mount1-3, we should create: mount0 (red), mount1-3 (green), mount4-31 (red)
      %base = getSubStr(%name, 0, %pos);      // array name
      %first = getSubStr(%name, %pos, 1);     // first index
      %last = getSubStr(%name, %pos+2, 3);    // last index

      // get the state of the first element
      %arrayStart = %first;
      %prevPresent = ShapeEditor.hintNameExists(%type, %base @ %first);

      for (%j = %first + 1; %j <= %last; %j++)
      {
         // if the state of this element is different to the previous one, we
         // need to add a hint
         %present = ShapeEditor.hintNameExists(%type, %base @ %j);
         if (%present != %prevPresent)
         {
            ShapeEdSelectWindow.addObjectHint(%type, %base, %desc, %prevPresent, %arrayStart, %j-1);
            %arrayStart = %j;
            %prevPresent = %present;
         }
      }

      // add hint for the last group
      ShapeEdSelectWindow.addObjectHint(%type, %base, %desc, %prevPresent, %arrayStart, %last);
   }
   else
   {
      // non-arrayed name
      %present = ShapeEditor.hintNameExists(%type, %name);
      ShapeEdSelectWindow.addObjectHint(%type, %name, %desc, %present);
   }
}

function ShapeEdSelectWindow::addObjectHint(%this, %type, %name, %desc, %present, %start, %end)
{
   // Get a hint gui control (create one if needed)
   if (ShapeHintControls.getCount() == 0)
   {
      // Create a new hint gui control
      %ctrl = new GuiIconButtonCtrl()
      {
         profile = "GuiCreatorIconButtonProfile";
         iconLocation = "Left";
         textLocation = "Right";
         extent = "348 19";
         textMargin = 8;
         buttonMargin = "2 2";
         autoSize = true;
         buttonType = "radioButton";
         groupNum = "-1";
         iconBitmap = "tools/editorClasses/gui/images/iconCancel";
         text = "hint";
         tooltip = "";
      };

      ShapeHintControls.add(%ctrl);
   }
   %ctrl = ShapeHintControls.getObject(0);

   // Initialise the control, then add it to the appropriate list
   %name = %name @ %start;
   if (%end !$= %start)
      %ctrl.text = %name @ "-" @ %end;
   else
      %ctrl.text = %name;

   %ctrl.tooltip = %desc;
   %ctrl.setBitmap("tools/editorClasses/gui/images/" @ (%present ? "iconAccept" : "iconCancel"));
   %ctrl.setStateOn(false);
   %ctrl.resetState();

   switch$ (%type)
   {
      case "node":
         %ctrl.altCommand = %present ? "" : "ShapeEdNodes.onAddNode(\"" @ %name @ "\");";
         ShapeEdSelectWindow-->nodeHints.addGuiControl(%ctrl);
      case "sequence":
         %ctrl.altCommand = %present ? "" : "ShapeEdSequences.onAddSequence(\"" @ %name @ "\");";
         ShapeEdSelectWindow-->sequenceHints.addGuiControl(%ctrl);
   }
}

//------------------------------------------------------------------------------

function ShapeEdSeqNodeTabBook::onTabSelected(%this, %name, %index)
{
   switch$ (%name)
   {
      case "Sequences":
         ShapeEdPropWindow-->newBtn.ToolTip = "Add new sequence";
         ShapeEdPropWindow-->newBtn.Command = "ShapeEdSequences.onAddSequence();";
         ShapeEdPropWindow-->deleteBtn.ToolTip = "Delete selected sequence";
         ShapeEdPropWindow-->deleteBtn.Command = "ShapeEdSequenceList.onDeleteSelection();";

      case "Nodes":
         ShapeEdPropWindow-->newBtn.ToolTip = "Add new node";
         ShapeEdPropWindow-->newBtn.Command = "ShapeEdNodes.onAddNode();";
         ShapeEdPropWindow-->deleteBtn.ToolTip = "Delete selected node";
         ShapeEdPropWindow-->deleteBtn.Command = "ShapeEdNodeTreeView.deleteSelection();";
   }
}

//------------------------------------------------------------------------------
// Node Editing
//------------------------------------------------------------------------------

function ShapeEdPropWindow::syncNodeDetails(%this, %id)
{
   if (%id > 0)
   {
      // Enable delete button and edit boxes
      ShapeEdPropWindow-->deleteBtn.setActive(true);
      ShapeEdNodes-->nodeName.setActive(true);
      ShapeEdNodes-->nodePosition.setActive(true);
      ShapeEdNodes-->nodeRotation.setActive(true);

      // Update the node inspection data
      %name = ShapeEdNodeTreeView.getItemText(%id);

      ShapeEdNodes-->nodeName.setText(%name);

      ShapeEdNodeParentMenu.clear();
      ShapeEdNodeParentMenu.add("<root>", 0);
      %count = ShapeEditor.shape.getNodeCount();
      for (%i = 0; %i < %count; %i++)
      {
         %pname = ShapeEditor.shape.getNodeName(%i);
         if (%pname !$= $name)
            ShapeEdNodeParentMenu.add(%pname, %i+1);
      }
      %pName = ShapeEditor.shape.getNodeParentName(%name);
      if (%pName $= "")
         %pName = "<root>";
      ShapeEdNodeParentMenu.setText(%pName);

      if (ShapeEdNodes-->worldTransform.getValue())
      {
         // Global transform
         %txfm = ShapeEditor.shape.getNodeTransform(%name, 1);
         ShapeEdNodes-->nodePosition.setText(getWords(%txfm, 0, 2));
         ShapeEdNodes-->nodeRotation.setText(getWords(%txfm, 3, 6));
      }
      else
      {
         // Local transform (relative to parent)
         %txfm = ShapeEditor.shape.getNodeTransform(%name, 0);
         ShapeEdNodes-->nodePosition.setText(getWords(%txfm, 0, 2));
         ShapeEdNodes-->nodeRotation.setText(getWords(%txfm, 3, 6));
      }

      ShapeEdShapeView.selectedNode = ShapeEditor.shape.getNodeIndex(%name);
   }
   else
   {
      // Disable delete button and edit boxes
      ShapeEdPropWindow-->deleteBtn.setActive(false);
      ShapeEdNodes-->nodeName.setActive(false);
      ShapeEdNodes-->nodePosition.setActive(false);
      ShapeEdNodes-->nodeRotation.setActive(false);

      ShapeEdNodes-->nodeName.setText("");
      ShapeEdNodes-->nodePosition.setText("");
      ShapeEdNodes-->nodeRotation.setText("");

      ShapeEdShapeView.selectedNode = -1;
   }
}

function ShapeEdNodeTreeView::onClearSelection(%this)
{
   ShapeEdPropWindow.syncNodeDetails(-1);
}

function ShapeEdNodeTreeView::onSelect(%this, %id)
{
   // Update the node name and transform controls
   ShapeEdPropWindow.syncNodeDetails(%id);
}

function ShapeEdShapeView::onNodeSelected(%this, %index)
{
   ShapeEdNodeTreeView.clearSelection();
   if (%index != -1)
   {
      %name = ShapeEditor.shape.getNodeName(%index);
      %id = ShapeEdNodeTreeView.findItemByName(%name);
      if (%id > 0)
         ShapeEdNodeTreeView.selectItem(%id);
   }
}

function ShapeEdNodes::onAddNode(%this, %name)
{
   // Add a new node, using the currently selected node as the initial parent
   if (%name $= "")
      %name = ShapeEditor.getUniqueName("node", "myNode");

   %id = ShapeEdNodeTreeView.getSelectedItem();
   if (%id <= 0)
      %parent = "";
   else
      %parent = ShapeEdNodeTreeView.getItemText(%id);

   ShapeEditor.doAddNode(%name, %parent, "0 0 0 0 0 1 0");
}

function ShapeEdNodeTreeView::onDeleteSelection(%this)
{
   // Remove the node and all its children from the shape
   %id = %this.getSelectedItem();
   if (%id > 0)
   {
      %name = %this.getItemText(%id);
      ShapeEditor.doRemoveNode(%name);
   }
}

// Determine the index of a node in the tree relative to its parent
function ShapeEdNodeTreeView::getChildIndexByName(%this, %name)
{
   %id = %this.findItemByName(%name);
   %parentId = %this.getParent(%id);
   %childId = %this.getChild(%parentId);

   %index = 0;
   while (%childId != %id)
   {
      %childId = %this.getNextSibling(%childId);
      %index++;
   }

   return %index;
}

// Add a node and its children to the node tree view
function ShapeEdNodeTreeView::addNodeTree(%this, %nodeName)
{
   // Ignore deleted nodes
   if (!ShapeEditor.isDeletedName(%nodeName))
   {
      // Abort if already added => something dodgy has happened and we'd end up
      // recursing indefinitely
      if (%this.findItemByName(%nodeName))
      {
         error("Recursion error in ShapeEdNodeTreeView::addNodeTree");
         return;
      }

      // Find parent and add me to it
      %parentName = ShapeEditor.shape.getNodeParentName(%nodeName);
      if (%parentName $= "")
         %parentName = "<root>";

      %parentId = %this.findItemByName(%parentName);
      %id = %this.insertItem(%parentId, %nodeName, 0, "");

      // Add children
      %count = ShapeEditor.shape.getNodeChildCount(%nodeName);
      for (%i = 0; %i < %count; %i++)
         %this.addNodeTree(ShapeEditor.shape.getNodeChildName(%nodeName, %i));
   }
}

function ShapeEdNodes::onEditName(%this)
{
   %id = ShapeEdNodeTreeView.getSelectedItem();
   if (%id > 0)
   {
      %oldName = ShapeEdNodeTreeView.getItemText(%id);
      %newName = %this-->nodeName.getText();
      if (%newName !$= "")
         ShapeEditor.doRenameNode(%oldName, %newName);
   }
}

function ShapeEdNodeParentMenu::onSelect(%this, %id, %text)
{
   %id = ShapeEdNodeTreeView.getSelectedItem();
   if (%id > 0)
   {
      %name = ShapeEdNodeTreeView.getItemText(%id);
      ShapeEditor.doSetNodeParent(%name, %text);
   }
}

function ShapeEdNodes::onEditTransform(%this)
{
   %id = ShapeEdNodeTreeView.getSelectedItem();
   if (%id > 0)
   {
      %name = ShapeEdNodeTreeView.getItemText(%id);

      // Get the node transform from the gui
      %pos = %this-->nodePosition.getText();
      %rot = %this-->nodeRotation.getText();
      %txfm = %pos SPC %rot;
      %isWorld = ShapeEdNodes-->worldTransform.getValue();

      // Do a quick sanity check to avoid setting wildly invalid transforms
      for (%i = 0; %i < 7; %i++)    // "x y z aa.x aa.y aa.z aa.angle"
      {
         if (getWord(%txfm, %i) $= "")
            return;
      }

      ShapeEditor.doEditNodeTransform(%name, %txfm, %isWorld);
   }
}

//------------------------------------------------------------------------------
// Sequence Editing
//------------------------------------------------------------------------------

function ShapeEdPropWindow::syncSequenceDetails(%this)
{
   ShapeEdSeqFromMenu.clear();
   ShapeEdSequences-->blendSeq.clear();

   // Clear the trigger list
   ShapeEdTriggerList.clear();
   ShapeEdTriggerList.addRow(-1, "-1" TAB "Frame" TAB "Trigger" TAB "State");
   ShapeEdTriggerList.setRowActive(-1, false);

   // Update the active sequence data
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      // Disable delete button and edit boxes
      ShapeEdPropWindow-->deleteBtn.setActive(true);
      ShapeEdSequences-->seqName.setActive(true);
      ShapeEdSequences-->blendFlag.setActive(true);
      ShapeEdSequences-->cyclicFlag.setActive(true);
      ShapeEdSequences-->priority.setActive(true);
      ShapeEdSequences-->addTriggerBtn.setActive(true);
      ShapeEdSequences-->deleteTriggerBtn.setActive(true);

      // Initialise the sequence properties
      %blendData = ShapeEditor.shape.getSequenceBlend(%seqName);
      ShapeEdSequences-->seqName.setText(%seqName);
      ShapeEdSequences-->cyclicFlag.setValue(ShapeEditor.shape.getSequenceCyclic(%seqName));
      ShapeEdSequences-->blendFlag.setValue(getField(%blendData, 0));
      ShapeEdSequences-->priority.setText(ShapeEditor.shape.getSequencePriority(%seqName));

      // 'From' and 'Blend' sequence menus
      ShapeEdSeqFromMenu.add("Browse...", 0);
      %count = ShapeEdSequenceList.rowCount();
      for (%i = 2; %i < %count; %i++)  // skip header row and <rootpose>
      {
         %name = ShapeEdSequenceList.getItemName(%i);
         if (%name !$= %seqName)
         {
            ShapeEdSeqFromMenu.add(%name, %i+1);
            ShapeEdSequences-->blendSeq.add(%name, %i+1);
         }
      }
      ShapeEdSequences-->blendSeq.setText(getField(%blendData, 1));
      ShapeEdSequences-->blendFrame.setText(getField(%blendData, 2));

      // Triggers
      %count = ShapeEditor.shape.getTriggerCount(%seqName);
      for (%i = 0; %i < %count; %i++)
      {
         %trigger = ShapeEditor.shape.getTrigger(%seqName, %i);
         ShapeEdTriggerList.addItem(getWord(%trigger, 0), getWord(%trigger, 1));
      }
   }
   else
   {
      // Disable delete button and edit boxes
      ShapeEdPropWindow-->deleteBtn.setActive(false);
      ShapeEdSequences-->seqName.setActive(false);
      ShapeEdSequences-->blendFlag.setActive(false);
      ShapeEdSequences-->cyclicFlag.setActive(false);
      ShapeEdSequences-->priority.setActive(false);
      ShapeEdSequences-->addTriggerBtn.setActive(false);
      ShapeEdSequences-->deleteTriggerBtn.setActive(false);

      // Clear sequence properties
      ShapeEdSequences-->seqName.setText("");
      ShapeEdSequences-->cyclicFlag.setValue(0);
      ShapeEdSequences-->blendSeq.setText("");
      ShapeEdSequences-->blendFlag.setValue(0);
      ShapeEdSequences-->priority.setText(0);
   }

   %this.syncPlaybackDetails();
   %this.syncTriggerDetails();

   // Reset current frame
   ShapeEdSeqSlider.setValue(ShapeEdPreviewWindow-->seqIn.getText());
}

function ShapeEdPropWindow::syncPlaybackDetails(%this)
{
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      // Show sequence in/out bars
      ShapeEdPreviewWindow-->seqInBar.setVisible(true);
      ShapeEdPreviewWindow-->seqOutBar.setVisible(true);

      // Sync playback controls
      %sourceData = ShapeEditor.shape.getSequenceSource(%seqName);
      %seqFrom = rtrim(getFields(%sourceData, 0, 1));
      %seqStart = getField(%sourceData, 2);
      %seqEnd = getField(%sourceData, 3);
      %seqFromTotal = getField(%sourceData, 4);

      // Display the original source for edited sequences
      if (strstr(%seqFrom, "__backup__") == 0)
      {
         %backupData = ShapeEditor.shape.getSequenceSource(getField(%seqFrom, 0));
         %seqFrom = rtrim(getFields(%backupData, 0, 1));
      }

      ShapeEdSeqFromMenu.setText(%seqFrom);
      ShapeEdSeqFromMenu.tooltip = ShapeEdSeqFromMenu.getText();   // use tooltip to show long names
      ShapeEdSequences-->startFrame.setText(%seqStart);
      ShapeEdSequences-->endFrame.setText(%seqEnd);

      %val = ShapeEdSeqSlider.getValue() / getWord(ShapeEdSeqSlider.range, 1);
      ShapeEdSeqSlider.range = "0" SPC (%seqFromTotal-1);
      ShapeEdSeqSlider.setValue(%val * getWord(ShapeEdSeqSlider.range, 1));

      ShapeEdPreviewWindow.setSequence(%seqName);
      ShapeEdPreviewWindow.setPlaybackLimit("in", %seqStart);
      ShapeEdPreviewWindow.setPlaybackLimit("out", %seqEnd);
   }
   else
   {
      // Hide sequence in/out bars
      ShapeEdPreviewWindow-->seqInBar.setVisible(false);
      ShapeEdPreviewWindow-->seqOutBar.setVisible(false);

      ShapeEdSeqFromMenu.setText("");
      ShapeEdSeqFromMenu.tooltip = "";
      ShapeEdSequences-->startFrame.setText(0);
      ShapeEdSequences-->endFrame.setText(0);

      ShapeEdSeqSlider.range = "0 1";
      ShapeEdSeqSlider.setValue(0);
      ShapeEdPreviewWindow.setPlaybackLimit("in", 0);
      ShapeEdPreviewWindow.setPlaybackLimit("out", 1);
      ShapeEdPreviewWindow.setSequence("");
   }
}

function ShapeEdPropWindow::syncTriggerDetails(%this)
{
   %row = ShapeEdTriggerList.getSelectedRow();
   if (%row > 0)  // skip header row
   {
      %text = ShapeEdTriggerList.getRowText(%row);

      ShapeEdSequences-->triggerFrame.setActive(true);
      ShapeEdSequences-->triggerNum.setActive(true);
      ShapeEdSequences-->triggerOnOff.setActive(true);

      ShapeEdSequences-->triggerFrame.setText(getField(%text, 1));
      ShapeEdSequences-->triggerNum.setText(getField(%text, 2));
      ShapeEdSequences-->triggerOnOff.setValue(getField(%text, 3) $= "on");
   }
   else
   {
      // No trigger selected
      ShapeEdSequences-->triggerFrame.setActive(false);
      ShapeEdSequences-->triggerNum.setActive(false);
      ShapeEdSequences-->triggerOnOff.setActive(false);

      ShapeEdSequences-->triggerFrame.setText("");
      ShapeEdSequences-->triggerNum.setText("");
      ShapeEdSequences-->triggerOnOff.setValue(0);
   }
}

function ShapeEdSequences::onEditName(%this)
{
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      %newName = %this-->seqName.getText();
      if (%newName !$= "")
         ShapeEditor.doRenameSequence(%seqName, %newName);
   }
}

function ShapeEdSequences::onEditSeqInOut(%this, %type, %val)
{
   %frameCount = getWord(ShapeEdSeqSlider.range, 1);

   // Force value to a frame index within the slider range
   %val = mRound(%val);
   if (%val < 0)
      %val = 0;
   if (%val > %frameCount)
      %val = %frameCount;

   // Enforce 'in' value must be < 'out' value
   if (%type $= "in")
   {
      if (%val >= %this-->endFrame.getText())
         %val = %this-->endFrame.getText() - 1;
      %this-->startFrame.setText(%val);
   }
   else
   {
      if (%val <= %this-->startFrame.getText())
         %val = %this-->startFrame.getText() + 1;
      %this-->endFrame.setText(%val);
   }

   %this.onEditSequenceSource("");
}

function ShapeEdSequences::onEditSequenceSource(%this, %from)
{
   %start = %this-->startFrame.getText();
   %end = %this-->endFrame.getText();

   if ((%start !$= "") && (%end !$= ""))
   {
      %seqName = ShapeEdSequenceList.getSelectedName();
      %oldSource = ShapeEditor.shape.getSequenceSource(%seqName);
      if (%from $= "")
         %from = rtrim(getFields(%oldSource, 0, 1));

      if (getFields(%oldSource, 0, 3) !$= (%from TAB "" TAB %start TAB %end))
         ShapeEditor.doEditSeqSource(%seqName, %from, %start, %end);
   }
}

function ShapeEdSequences::onToggleCyclic(%this)
{
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      %cyclic = %this-->cyclicFlag.getValue();
      ShapeEditor.doEditCyclic(%seqName, %cyclic);
   }
}

function ShapeEdSequences::onEditPriority(%this)
{
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      %newPriority = %this-->priority.getText();
      if (%newPriority !$= "")
         ShapeEditor.doEditSequencePriority(%seqName, %newPriority);
   }
}

function ShapeEdSequences::onEditBlend(%this)
{
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      // Get the blend flags (current and new)
      %oldBlendData = ShapeEditor.shape.getSequenceBlend(%seqName);
      %oldBlend = getField(%oldBlendData, 0);
      %blend = %this-->blendFlag.getValue();

      // Ignore changes to the blend reference for non-blend sequences
      if (!%oldBlend && !%blend)
         return;

      // OK - we're trying to change the blend properties of this sequence. The
      // new reference sequence and frame must be set.
      %blendSeq = %this-->blendSeq.getText();
      %blendFrame = %this-->blendFrame.getText();
      if ((%blendSeq $= "") || (%blendFrame $= ""))
      {
         MessageBoxOK("Blend reference not set", "The blend reference sequence and " @
            "frame must be set before changing the blend flag or frame.");
         ShapeEdSequences-->blendFlag.setStateOn(%oldBlend);
         return;
      }

      // Get the current blend properties (use new values if not specified)
      %oldBlendSeq = getField(%oldBlendData, 1);
      if (%oldBlendSeq $= "")
         %oldBlendSeq = %blendSeq;
      %oldBlendFrame = getField(%oldBlendData, 2);
      if (%oldBlendFrame $= "")
         %oldBlendFrame = %blendFrame;

      // Check if there is anything to do
      if ((%oldBlend TAB %oldBlendSeq TAB %oldBlendFrame) !$= (%blend TAB %blendSeq TAB %blendFrame))
         ShapeEditor.doEditBlend(%seqName, %blend, %blendSeq, %blendFrame);
   }
}

function ShapeEdSequences::onAddSequence(%this, %name)
{
   if (%name $= "")
      %name = ShapeEditor.getUniqueName("sequence", "mySequence");

   // Use the currently selected sequence as the base
   %from = ShapeEdSequenceList.getSelectedName();
   %row = ShapeEdSequenceList.getSelectedRow();
   if ((%row < 2) && (ShapeEdSequenceList.rowCount() > 2))
      %row = 2;
   if (%from $= "")
   {
      // No sequence selected => open dialog to browse for one
      getLoadFilename("DSQ Files|*.dsq|COLLADA Files|*.dae|Sketchup Files|*.kmz", %this @ ".onAddSequenceFromBrowse", ShapeEdFromMenu.lastPath);
      return;
   }
   else
   {
      // Add the new sequence
      %start = ShapeEdSequences-->startFrame.getText();
      %end = ShapeEdSequences-->endFrame.getText();
      ShapeEditor.doAddSequence(%name, %from, %start, %end);
   }
}

function ShapeEdSequences::onAddSequenceFromBrowse(%this, %path)
{
   // Add a new sequence from the browse path
   %path = makeRelativePath(%path, getWorkingDirectory());
   ShapeEdFromMenu.lastPath = %path;

   %name = ShapeEditor.getUniqueName("sequence", "mySequence");
   ShapeEditor.doAddSequence(%name, %path, 0, -1);
}

// Delete the selected sequence
function ShapeEdSequenceList::onDeleteSelection(%this)
{
   %row = %this.getSelectedRow();
   if (%row != -1)
   {
      %seqName = %this.getItemName(%row);
      ShapeEditor.doRemoveSequence(%seqName);
   }
}

// Get the name of the currently selected sequence
function ShapeEdSequenceList::getSelectedName(%this)
{
   %row = %this.getSelectedRow();
   return (%row > 1) ? %this.getItemName(%row) : "";    // ignore header row
}

// Get the sequence name from the indexed row
function ShapeEdSequenceList::getItemName(%this, %row)
{
   return getField(%this.getRowText(%row), 0);
}

// Get the index in the list of the sequence with the given name
function ShapeEdSequenceList::getItemIndex(%this, %name)
{
   for (%i = 0; %i < %this.rowCount(); %i++)
   {
      if (%this.getItemName(%i) $= %name)
         return %i;
   }
   return -1;
}

// Change one of the fields in the sequence list
function ShapeEdSequenceList::editColumn(%this, %name, %col, %text)
{
   %row = %this.getItemIndex(%name);
   %rowText = setField(%this.getRowText(%row), %col, %text);
   %this.setRowById(%this.getRowId(%row), %rowText);
}

function ShapeEdSequenceList::addItem(%this, %name)
{
   return %this.insertItem(%name, %this.rowCount());
}

function ShapeEdSequenceList::insertItem(%this, %name, %index)
{
   %cyclic = ShapeEditor.shape.getSequenceCyclic(%name) ? "yes" : "no";
   %blend = getField(ShapeEditor.shape.getSequenceBlend(%name), 0) ? "yes" : "no";
   %frameCount = ShapeEditor.shape.getSequenceFrameCount(%name);
   %priority = ShapeEditor.shape.getSequencePriority(%name);

   return %this.addRow(%this.seqId++, %name TAB %cyclic TAB %blend TAB %frameCount TAB %priority, %index);
}

function ShapeEdSequenceList::removeItem(%this, %name)
{
   %index = %this.getItemIndex(%name);
   if (%index >= 0)
      %this.removeRow(%index);
}

function ShapeEdSeqFromMenu::onSelect(%this, %id, %text)
{
   if (%text $= "Browse...")
   {
      // Reset menu text
      %seqName = ShapeEdSequenceList.getSelectedName();
      %sourceData = ShapeEditor.shape.getSequenceSource(%seqName);
      %this.setText(rtrim(getFields(%sourceData, 0, 1)));

      // Allow the user to browse for an external source of animation data
      getLoadFilename("DSQ Files|*.dsq|COLLADA Files|*.dae|Sketchup Files|*.kmz", %this @ ".onBrowseSelect", %this.lastPath);
   }
   else
   {
      ShapeEdSequences.onEditSequenceSource(%text);
   }
}

function ShapeEdSeqFromMenu::onBrowseSelect(%this, %path)
{
   %path = makeRelativePath(%path, getWorkingDirectory());
   %this.lastPath = %path;
   %this.setText(%path);
   ShapeEdSequences.onEditSequenceSource(%path);
}

//------------------------------------------------------------------------------
// Trigger Editing
//------------------------------------------------------------------------------

function ShapeEdTriggerList::getTriggerText(%this, %frame, %state)
{
   // First column is invisible and used only for sorting
   %sortKey = (%frame * 1000) + (mAbs(%state) * 10) + ((%state > 0) ? 1 : 0);
   return %sortKey TAB %frame TAB mAbs(%state) TAB ((%state > 0) ? "on" : "off");
}

function ShapeEdTriggerList::removeItem(%this, %frame, %state)
{
   %row = %this.findTextIndex(%this.getTriggerText(%frame, %state));
   if (%row > 0)
      %this.removeRow(%row);
}

function ShapeEdTriggerList::addItem(%this, %frame, %state)
{
   %this.addRow(%this.id++, %this.getTriggerText(%frame, %state));
   %this.sortNumerical(0, true);
}

function ShapeEdTriggerList::updateItem(%this, %oldFrame, %oldState, %frame, %state)
{
   %oldText = %this.getTriggerText(%oldFrame, %oldState);
   %row = %this.getSelectedRow();
   if ((%row <= 0) || (%this.getRowText(%row) !$= %oldText))
      %row = %this.findTextIndex(%oldText);
   if (%row > 0)
   {
      %newText = %this.getTriggerText(%frame, %state);
      %this.setRowById(%this.getRowId(%row), %newText);

      // keep selected row the same
      %id = %this.getSelectedId();
      %this.sortNumerical(0, true);
      %this.setSelectedById(%id);
   }
}

function ShapeEdSequences::onAddTrigger(%this)
{
   // Can only add triggers if a sequence is selected
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      // Add a new trigger at the current frame
      %frame = mRound(ShapeEdSeqSlider.getValue());
      %state = ShapeEdTriggerList.rowCount() % 30;
      ShapeEditor.doAddTrigger(%seqName, %frame, %state);
   }
}

function ShapeEdTriggerList::onDeleteSelection(%this)
{
   // Can only delete a trigger if a sequence and trigger are selected
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      %row = %this.getSelectedRow();
      if (%row > 0)
      {
         %text = %this.getRowText(%row);
         %frame = getWord(%text, 1);
         %state = getWord(%text, 2);
         %state *= (getWord(%text, 3) $= "on") ? 1 : -1;
         ShapeEditor.doRemoveTrigger(%seqName, %frame, %state);
      }
   }
}

function ShapeEdTriggerList::onEditSelection(%this)
{
   // Can only edit triggers if a sequence and trigger are selected
   %seqName = ShapeEdSequenceList.getSelectedName();
   if (%seqName !$= "")
   {
      %row = ShapeEdTriggerList.getSelectedRow();
      if (%row > 0)
      {
         %text = %this.getRowText(%row);
         %oldFrame = getWord(%text, 1);
         %oldState = getWord(%text, 2);
         %oldState *= (getWord(%text, 3) $= "on") ? 1 : -1;

         %frame = mRound(ShapeEdSequences-->triggerFrame.getText());
         %state = mRound(mAbs(ShapeEdSequences-->triggerNum.getText()));
         %state *= ShapeEdSequences-->triggerOnOff.getValue() ? 1 : -1;

         if ((%frame >= 0) && (%state != 0))
            ShapeEditor.doEditTrigger(%seqName, %oldFrame, %oldState, %frame, %state);
      }
   }
}

//------------------------------------------------------------------------------
// Shape Preview
//------------------------------------------------------------------------------

function ShapeEdSeqSlider::onMouseDragged(%this)
{
   // Stop animation playback when the slider is dragged
   if (ShapeEdShapeView.isPlaying)
      ShapeEdPreviewWindow.togglePlayMode();
}

// Toggle between paused and continuous play modes
function ShapeEdPreviewWindow::togglePlayMode(%this)
{
   if (ShapeEdShapeView.isPlaying)
   {
      ShapeEdShapeView.isPlaying = false;
      ShapeEdPreviewWindow-->playBtn.setBitmap("tools/shapeEditor/images/play_btn");
   }
   else
   {
      ShapeEdShapeView.isPlaying = true;
      ShapeEdPreviewWindow-->playBtn.setBitmap("tools/shapeEditor/images/pause_btn");
   }
}

// Set the sequence to play
function ShapeEdPreviewWindow::setSequence(%this, %seqName)
{
   ShapeEdShapeView.usingProxySequence = 0;
   if (%seqName !$= "")
   {
      // To be able to effectively scrub through the animation, we need to have all
      // frames available, even if it was added with only a subset. If that is the
      // case, then use a proxy sequence that has all the frames instead.
      %sourceData = ShapeEditor.shape.getSequenceSource(%seqName);
      %from = rtrim(getFields(%sourceData, 0, 1));
      %startFrame = getField(%sourceData, 2);
      %endFrame = getField(%sourceData, 3);
      %frameCount = getField(%sourceData, 4);

      if ((%startFrame != 0) || (%endFrame != (%frameCount-1)))
      {
         %seqName = "__proxy__";
         if (ShapeEditor.shape.getSequenceIndex(%seqName) != -1)
            ShapeEditor.shape.removeSequence(%seqName);
         ShapeEditor.shape.addSequence(%from, %seqName);
         ShapeEdShapeView.usingProxySequence = 1;
      }
   }

   ShapeEdShapeView.setSequence(%seqName);
}

// Set the in or out sequence limit
function ShapeEdPreviewWindow::setPlaybackLimit(%this, %limit, %val)
{
   %frameCount = getWord(ShapeEdSeqSlider.range, 1);

   // Determine where to place the in/out bar on the slider
   %thumbWidth = 8;    // width of the thumb bitmap
   %pos_x = getWord(ShapeEdSeqSlider.getPosition(), 0);
   %len_x = getWord(ShapeEdSeqSlider.getExtent(), 0) - %thumbWidth;
   %pos_x += ((%len_x * %val / %frameCount) );

   if (%limit $= "in")
   {
      %this-->seqIn.setText(%val);
      %this-->seqInBar.setPosition(%pos_x, 0);
      ShapeEdShapeView.seqIn = %val / %frameCount;
   }
   else
   {
      %this-->seqOut.setText(%val);
      %this-->seqOutBar.setPosition(%pos_x, 0);
      ShapeEdShapeView.seqOut = %val / %frameCount;
   }
}

function ShapeEdPreviewGui::updatePreviewBackground(%color)
{
   ShapeEdPreviewGui-->previewBackground.color = %color;
   ShapeEditorToolbar-->previewBackgroundPicker.color = %color;
}

function showShapeEditorPreview()
{
   %visible = ShapeEditorToolbar-->showPreview.getValue();
   ShapeEdPreviewGui.setVisible(%visible);
}

function ShapeEdShapeView::onEditNodeTransform(%this, %node, %txfm)
{
   ShapeEditor.doEditNodeTransform(%node, %txfm, 1);
}
