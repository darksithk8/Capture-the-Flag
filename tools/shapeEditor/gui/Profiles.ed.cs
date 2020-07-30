//-----------------------------------------------------------------------------
// Shape Editor Profiles
//-----------------------------------------------------------------------------

singleton GuiControlProfile(GuiShapeEdScrollProfile : GuiEditorScrollProfile)
{
   // Don't clear the scroll area (since we need to be able to see the GuiContainer
   // underneath that provides the fill color for the header row)
   opaque = false;
};

singleton GuiControlProfile(GuiShapeEdTextListProfile : GuiTextListProfile)
{
   // Customise the not-active font used for the header row
   fontColorNA = "75 75 75";
};

singleton GuiControlProfile(GuiShapeEdRolloutProfile : GuiInspectorRolloutProfile0)
{
   bitmap = "tools/editorclasses/gui/images/rollout";
};
