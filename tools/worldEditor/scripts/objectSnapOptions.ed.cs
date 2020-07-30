
function ESnapOptions::onWake( %this )
{
   ESnapOptionsTabTerrain-->NoAlignment.setStateOn(1);
   
   ESnapOptionsTabSoft-->NoAlignment.setStateOn(1);
   ESnapOptionsTabSoft-->RenderSnapBounds.setStateOn(1);
   ESnapOptionsTabSoft-->SnapBackfaceTolerance.setText(EWorldEditor.getSoftSnapBackfaceTolerance());
}

function ESnapOptions::hideDialog( %this )
{
   %this.setVisible(false);
}

function ESnapOptions::ToggleVisibility()
{
   if ( ESnapOptions.visible  )
   {
      ESnapOptions.setVisible(false);
   }
   else
   {
      ESnapOptions.setVisible(true);
      ESnapOptions.selectWindow();
      ESnapOptions.setCollapseGroup(false);
   }
}

function ESnapOptions::setTerrainSnapAlignment( %this, %val )
{
   EWorldEditor.setTerrainSnapAlignment(%val);
}

function ESnapOptions::setSoftSnapAlignment( %this, %val )
{
   EWorldEditor.setSoftSnapAlignment(%val);
}

function ESnapOptions::setSoftSnapSize( %this )
{
   %val = ESnapOptions-->SnapSize.getText();
   
   EWorldEditor.setSoftSnapSize(%val);
   EWorldEditor.syncGui();
}

function ESnapOptions::toggleRenderSnapBounds( %this )
{
   EWorldEditor.softSnapRender( ESnapOptionsTabSoft-->RenderSnapBounds.getValue() );
}

function ESnapOptions::toggleRenderSnappedTriangle( %this )
{
   EWorldEditor.softSnapRenderTriangle( ESnapOptionsTabSoft-->RenderSnappedTriangle.getValue() );
}

function ESnapOptions::getSoftSnapBackfaceTolerance( %this )
{
   %val = ESnapOptions-->SnapBackfaceTolerance.getText();
   
   EWorldEditor.setSoftSnapBackfaceTolerance(%val);
}
