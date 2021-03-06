//-----------------------------------------------------------------------------
// Copyright (C) Sickhead Games, LLC
//-----------------------------------------------------------------------------

singleton GuiControlProfile( RoadEditorProfile )
{
   canKeyFocus = true;
   opaque = true;
   fillColor = "192 192 192 192";
};

singleton GuiControlProfile (GuiSimpleBorderProfile)
{
   opaque = false;   
   border = 1;   
};

singleton GuiCursor(RoadEditorMoveCursor)
{
   hotSpot = "4 4";
   renderOffset = "0 0";
   bitmapName = "~/gui/images/macCursor";
};  

singleton GuiCursor( RoadEditorMoveNodeCursor )
{
   hotSpot = "1 1";
   renderOffset = "0 0";
   bitmapName = "./Cursors/outline/drag_node_outline";
};

singleton GuiCursor( RoadEditorAddNodeCursor )
{
   hotSpot = "1 1";
   renderOffset = "0 0";
   bitmapName = "./Cursors/outline/add_to_end_outline";
};

singleton GuiCursor( RoadEditorInsertNodeCursor )
{
   hotSpot = "1 1";
   renderOffset = "0 0";
   bitmapName = "./Cursors/outline/insert_in_middle_outline";
};

singleton GuiCursor( RoadEditorResizeNodeCursor )
{
   hotSpot = "1 1";
   renderOffset = "0 0";
   bitmapName = "./Cursors/outline/widen_path_outline";
};