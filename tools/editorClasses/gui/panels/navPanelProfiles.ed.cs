//---------------------------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//---------------------------------------------------------------------------------------------


singleton GuiControlProfile (NavPanelProfile) 
{
   opaque = false;
   border = -2;
};


singleton GuiControlProfile (NavPanel : NavPanelProfile) 
{
   bitmap = "./navPanel";
};

singleton GuiControlProfile (NavPanelBlue : NavPanelProfile) 
{
   bitmap = "./navPanel_blue";
};

singleton GuiControlProfile (NavPanelGreen : NavPanelProfile) 
{
   bitmap = "./navPanel_green";
};

singleton GuiControlProfile (NavPanelRed : NavPanelProfile) 
{
   bitmap = "./navPanel_red";
};

singleton GuiControlProfile (NavPanelWhite : NavPanelProfile) 
{
   bitmap = "./navPanel_white";
};

singleton GuiControlProfile (NavPanelYellow : NavPanelProfile) 
{
   bitmap = "./navPanel_yellow";
};
singleton GuiControlProfile (menubarProfile : NavPanelProfile) 
{
   bitmap = "./menubar";
};
singleton GuiControlProfile (editorMenubarProfile : NavPanelProfile) 
{
   bitmap = "./editor-menubar";
};
singleton GuiControlProfile (inspectorStyleRolloutProfile : NavPanelProfile) 
{
   bitmap = "./inspector-style-rollout";
};
singleton GuiControlProfile (inspectorStyleRolloutListProfile : NavPanelProfile) 
{
   bitmap = "./inspector-style-rollout-list";
};
singleton GuiControlProfile (inspectorStyleRolloutDarkProfile : NavPanelProfile) 
{
   bitmap = "./inspector-style-rollout-dark";
};
singleton GuiControlProfile (inspectorStyleRolloutInnerProfile : NavPanelProfile) 
{
   bitmap = "./inspector-style-rollout_inner";
};
singleton GuiControlProfile (IconDropdownProfile : NavPanelProfile) 
{
   bitmap = "./icon-dropdownbar";
};
