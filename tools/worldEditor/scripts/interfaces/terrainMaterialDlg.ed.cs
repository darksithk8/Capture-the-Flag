//-----------------------------------------------------------------------------
// Torque Game Engine Advanced
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function TerrainMaterialDlg::_selectTextureFileDialog( %this, %defaultFileName )
{
   if( $Pref::TerrainEditor::LastPath $= "" )
      $Pref::TerrainEditor::LastPath = "art/terrains";

   %dlg = new OpenFileDialog()
   {
      Filters        = $TerrainEditor::TextureFileSpec;
      DefaultPath    = $Pref::TerrainEditor::LastPath;
      DefaultFile    = %defaultFileName;
      ChangePath     = false;
      MustExist      = true;
   };
            
   %ret = %dlg.Execute();
   if ( %ret )
   {
      $Pref::TerrainEditor::LastPath = filePath( %dlg.FileName );
      %file = %dlg.FileName;
   }
      
   %dlg.delete();
   
   if ( !%ret )
      return; 
      
   %file = filePath(%file) @ "/" @ fileBase(%file);
      
   return %file;
}

function TerrainMaterialDlg::show( %this, %matIndex, %terrMat, %onApplyCallback )
{
   Canvas.pushDialog( %this );
   
   %this.matIndex = %matIndex; 
   %this.onApplyCallback = %onApplyCallback;

   %matLibTree = %this-->matLibTree;
   %item = %matLibTree.findItemByObjectId( %terrMat );
   if ( %item != -1 )
   {
      %matLibTree.selectItem( %item );
      %matLibTree.scrollVisible( %item );
   }
   else
   {
      for( %i = 1; %i < %matLibTree.getItemCount(); %i++ )
      {
         %terrMat = TerrainMaterialDlg-->matLibTree.getItemValue(%i);
         if( %terrMat.getClassName() $= "TerrainMaterial" )
         {
            %matLibTree.selectItem( %i, true );
            %matLibTree.scrollVisible( %i );
            break;
         }
      }
   }
}

function TerrainMaterialDlg::showByObjectId( %this, %matObjectId, %onApplyCallback )
{
   Canvas.pushDialog( %this );
     
   %this.matIndex = -1;
   %this.onApplyCallback = %onApplyCallback;
                 
   %matLibTree = %this-->matLibTree;
   %matLibTree.clearSelection();   
   %item = %matLibTree.findItemByObjectId( %matObjectId );
   if ( %item != -1 )
   {
      %matLibTree.selectItem( %item );
      %matLibTree.scrollVisible( %item );
   }
}

function TerrainMaterialDlg::onWake( %this )
{
   // Refresh the material list.
   %matLibTree = %this-->matLibTree;
   %matLibTree.clear();
   
   %this.activeMat = 0;
   %matLibTree.open( TerrainMaterialSet, false );  
   
   %matLibTree.buildVisibleTree( false );   
   %item = %matLibTree.getFirstRootItem();
   %matLibTree.expandItem( %item );
   
   %this.activateMaterialCtrls( false );      
}

function TerrainMaterialDlg::dialogApply( %this )
{
   // Make sure we save any changes to the current selection.
   %this.saveDirtyMaterial( %this.activeMat, true );

   // Remove ourselves from the canvas.
   Canvas.popDialog( TerrainMaterialDlg ); 
                            
   call( %this.onApplyCallback, %this.activeMat, %this.matIndex );
}

function TerrainMaterialDlg::dialogCancel( %this )
{
   // TODO: Restore any changes we made!   
   
   Canvas.popDialog( TerrainMaterialDlg );  
}

function TerrainMaterialDlg::changeBase( %this )
{   
   %ctrl = %this-->baseTexCtrl;
   %file = %ctrl.bitmap;
   if( getSubStr( %file, 0 , 6 ) $= "tools/" )
      %file = "";
      
   %file = TerrainMaterialDlg._selectTextureFileDialog( %file );      
   if( %file $= "" )
   {
      if( %ctrl.bitmap !$= "" )
         %file = %ctrl.bitmap;
      else
         %file = "tools/materialeditor/gui/unknownImage";
   }
   
   %file = makeRelativePath( %file, getMainDotCsDir() );
   %ctrl.setBitmap( %file );  
}

function TerrainMaterialDlg::changeDetail( %this )
{
   %ctrl = %this-->detailTexCtrl;
   %file = %ctrl.bitmap;
   if( getSubStr( %file, 0 , 6 ) $= "tools/" )
      %file = "";

   %file = TerrainMaterialDlg._selectTextureFileDialog( %file );  
   if( %file $= "" )
   {
      if( %ctrl.bitmap !$= "" )
         %file = %ctrl.bitmap;
      else
         %file = "tools/materialeditor/gui/unknownImage";
   }
   
   %file = makeRelativePath( %file, getMainDotCsDir() );
   %ctrl.setBitmap( %file );  
}

function TerrainMaterialDlg::changeNormal( %this )
{   
   %ctrl = %this-->normTexCtrl;
   %file = %ctrl.bitmap;
   if( getSubStr( %file, 0 , 6 ) $= "tools/" )
      %file = "";

   %file = TerrainMaterialDlg._selectTextureFileDialog( %file );  
   if( %file $= "" )
   {
      if( %ctrl.bitmap !$= "" )
         %file = %ctrl.bitmap;
      else
         %file = "tools/materialeditor/gui/unknownImage";
   }

   %file = makeRelativePath( %file, getMainDotCsDir() );
   %ctrl.setBitmap( %file );   
}

function TerrainMaterialDlg::newMat( %this )
{   
   // The instant group will try to stick us into
   // some other group if we don't disable it.   
   pushInstantGroup();
   
   // Create the new material.
   %newMat = new TerrainMaterial()
   {
      internalName = "newMaterial";
   };
   
   // Mark it as dirty and to be saved in the default location.
   ETerrainPersistMan.setDirty( %newMat, "art/terrains/materials.cs" );
         
   // Restore the instant group.
   popInstantGroup();
   
   %matLibTree = %this-->matLibTree;
   %matLibTree.buildVisibleTree( false );
   %item = %matLibTree.findItemByObjectId( %newMat );
   %matLibTree.selectItem( %item );   
}

function TerrainMaterialDlg::deleteMat( %this )
{
   // TODO: Fix me once ETerrainPersistMan can 
   // safely delete object from disk!
   
   /*
   if ( !isObject( %this.activeMat ) )
      return;
      
   %this.activeMat.delete();
   %this.activeMat = 0;
   
   %matLibTree = %this-->matLibTree;
   %matLibTree.buildVisibleTree( false );         
   */
}

function TerrainMaterialDlg::activateMaterialCtrls( %this, %active )
{  
   %parent = %this-->matSettingsParent;
   %count = %parent.getCount();
   for ( %i = 0; %i < %count; %i++ )
      %parent.getObject( %i ).setActive( %active );      
}

function TerrainMaterialTreeCtrl::onSelect( %this, %item )
{
   TerrainMaterialDlg.setActiveMaterial( %item );
}

function TerrainMaterialTreeCtrl::onUnSelect( %this, %item )
{
   TerrainMaterialDlg.saveDirtyMaterial( %item );   
   TerrainMaterialDlg.setActiveMaterial( 0 );   
}

function TerrainMaterialDlg::setActiveMaterial( %this, %mat )
{  
   if (  isObject( %mat ) && 
         %mat.isMemberOfClass( TerrainMaterial ) )
   {
      %this.activeMat = %mat;
      
      %this-->matNameCtrl.setText( %mat.internalName );
      if (%mat.diffuseMap $= ""){
         %this-->baseTexCtrl.setBitmap( "tools/materialeditor/gui/unknownImage" );
      }else{
         %this-->baseTexCtrl.setBitmap( %mat.diffuseMap ); 
      }
      if (%mat.detailMap $= ""){
         %this-->detailTexCtrl.setBitmap( "tools/materialeditor/gui/unknownImage" );
      }else{
         %this-->detailTexCtrl.setBitmap( %mat.detailMap );
      }
      if (%mat.normalMap $= ""){
         %this-->normTexCtrl.setBitmap( "tools/materialeditor/gui/unknownImage" );
      }else{
         %this-->normTexCtrl.setBitmap( %mat.normalMap ); 
      }
      %this-->detSizeCtrl.setText( %mat.detailSize );
      %this-->baseSizeCtrl.setText( %mat.diffuseSize );
      %this-->detStrengthCtrl.setText( %mat.detailStrength );
      %this-->detDistanceCtrl.setText( %mat.detailDistance );      
      %this-->sideProjectionCtrl.setValue( %mat.useSideProjection );
      %this-->parallaxScaleCtrl.setText( %mat.parallaxScale );
            
      %this.activateMaterialCtrls( true );      
   }
   else
   {
      %this.activeMat = 0;
      %this.activateMaterialCtrls( false );        
   }
}

function TerrainMaterialDlg::saveDirtyMaterial( %this, %mat, %skipAskMessage )
{
   // Skip over obviously bad cases.
   if (  !isObject( %mat ) || 
         !%mat.isMemberOfClass( TerrainMaterial ) )
      return;
            
   %isDirty = false;
   
   %newName = %this-->matNameCtrl.getText(); 
   
   if (%this-->baseTexCtrl.bitmap $= "tools/materialeditor/gui/unknownImage"){
      %newDiffuse = "";
   }else{
      %newDiffuse = %this-->baseTexCtrl.bitmap;  
   }
   if (%this-->normTexCtrl.bitmap $= "tools/materialeditor/gui/unknownImage"){
      %newNormal = "";
   }else{
      %newNormal = %this-->normTexCtrl.bitmap;  
   }
   if (%this-->detailTexCtrl.bitmap $= "tools/materialeditor/gui/unknownImage"){
      %newDetail = "";
   }else{
      %newDetail = %this-->detailTexCtrl.bitmap;  
   }
   %detailSize = %this-->detSizeCtrl.getText();      
   %diffuseSize = %this-->baseSizeCtrl.getText();     
   %detailStrength = %this-->detStrengthCtrl.getText();
   %detailDistance = %this-->detDistanceCtrl.getText();   
   %useSideProjection = %this-->sideProjectionCtrl.getValue();   
   %parallaxScale = %this-->parallaxScaleCtrl.getText();

   // TODO: Validate the new name... make sure its unique!
   
   if (  %mat.internalName !$= %newName ||
         %mat.diffuseMap !$= %newDiffuse ||
         %mat.normalMap !$= %newNormal ||
         %mat.detailMap !$= %newDetail ||
         %mat.detailSize != %detailSize ||
         %mat.diffuseSize != %diffuseSize ||
         %mat.detailStrength != %detailStrength ||
         %mat.detailDistance != %detailDistance ||         
         %mat.useSideProjection != %useSideProjection ||
         %mat.parallaxScale != %parallaxScale )               
      %isDirty = true;

   if ( !%isDirty )
      return;
      
   if ( !%skipAskMessage )
   {
      // Ask first... it may have be inadvertent.
      %result = MessageBox( "Terrain Materials", "Do you want to save your changes to this material?", "Yes", "No" );   
      if ( %result != $MROk )
         return;  
   }
      
   %mat.setInternalName( %newName );    
   %mat.diffuseMap = %newDiffuse;    
   %mat.normalMap = %newNormal;    
   %mat.detailMap = %newDetail;    
   %mat.detailSize = %detailSize;  
   %mat.diffuseSize = %diffuseSize;
   %mat.detailStrength = %detailStrength;    
   %mat.detailDistance = %detailDistance;    
   %mat.useSideProjection = %useSideProjection;
   %mat.parallaxScale = %parallaxScale;
   
   // Mark the material as dirty and needing saving.
   //
   // NOTE: We're currently forcing all the materials to save
   // into the same location.  This needs to be corrected to only
   // set the default location if the material doesn't already
   // have an existing file path.
   //
   ETerrainPersistMan.setDirty( %mat, "art/terrains/materials.cs" ); 
}
