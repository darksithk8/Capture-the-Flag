//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Console onEditorRender functions:
//------------------------------------------------------------------------------
// Functions:
//   - renderSphere([pos], [radius], <sphereLevel>);
//   - renderCircle([pos], [normal], [radius], <segments>);
//   - renderTriangle([pnt], [pnt], [pnt]);
//   - renderLine([start], [end], <thickness>);
//
// Variables:
//   - consoleFrameColor - line prims are rendered with this
//   - consoleFillColor
//   - consoleSphereLevel - level of polyhedron subdivision
//   - consoleCircleSegments
//   - consoleLineWidth
//------------------------------------------------------------------------------

function SpawnSphere::onEditorRender(%this, %editor, %selected, %expanded)
{
   if(%selected $= "true")
   {
      %editor.consoleFrameColor = "255 0 0";
      %editor.consoleFillColor = "0 160 0 95";
      %editor.renderSphere(%this.getWorldBoxCenter(), %this.radius, 1);
   }
}

//TODO: add sound cone visualization
function SFXEmitter::onEditorRender( %this, %editor, %selected, %expanded )
{
   // Check to see if the emitter is in range and playing
   // and assign a proper color depending on this.
   
   if( %this.getPlaybackStatus() $= "playing" )
   {
      if( %this.isInRange() )
         %editor.consoleFillColor = "50 255 50 255";
      else
         %editor.consoleFillColor = "50 128 50 255";
   }
   else
   {
      if( %this.isInRange() )
         %editor.consoleFillColor = "0 0 0 255";
      else
         %editor.consoleFillColor = "128 128 128 255";
   }
   
   // Draw the cube.
   
   %editor.renderBox( %this.getWorldBoxCenter(), "1.0 1.0 1.0" );
   
   // Draw the range sphere if the emitter is selected.
   
   if( %selected $= "true" )
   {
      %renderRange = false;
      %range = 0;
      
      if( isObject( %this.profile ) && %this.profile.description.is3D )
      {
         %renderRange = true;
         %range = %this.profile.description.maxDistance;
      }
      else
      {
         %renderRange = %this.is3D;
         %range = %this.maxDistance;
      }
         
      if( %renderRange )
      {
         %editor.consoleFillColor = "255 50 50 64";
         %editor.renderSphere( %this.getWorldBoxCenter(), %range, 1 );
      }
   }
}

//function Item::onEditorRender(%this, %editor, %selected, %expanded)
//{
//   if(%this.getDataBlock().getName() $= "MineDeployed")
//   {
//      %editor.consoleFillColor = "0 0 0 0";
//      %editor.consoleFrameColor = "255 0 0";
//      %editor.renderSphere(%this.getWorldBoxCenter(), 6, 1);
//   }
//}