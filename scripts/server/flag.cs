//$flagslot = 2;
datablock ShapeBaseImageData(Flag1Image)
{
   shapeFile = "art/shapes/JediFlags/Flag1.DAE";
   item = Flag1;
   mountPoint = 2;
   offset = "0 0 0";

   lightType = "PulsingLight";
   lightColor = "0.5 0.5 0.5 1.0";
   lightTime = "1000";
   lightRadius = "3";
   cloakable = false;
};

datablock StaticShapeData(Flag1)
{
   category = "Flag";
   className = "Flag";
   image = Flag1Image;
   shapefile = "art/shapes/JediFlags/Flag1.DAE";
   mass = 55;
   elasticity = 0.2;
   friction = 0.6;
   pickupRadius = 3;
   pickUpName = "a flag";
   computeCRC = true;

   lightType = "PulsingLight";
   lightColor = "0.5 0.5 0.5 1.0";
   lightTime = "1000";
   lightRadius = "3";

   isInvincible = true;
   
};

datablock ShapeBaseImageData(Flag2Image)
{
   shapeFile = "art/shapes/JediFlags/Flag2.DAE";
   item = Flag2;
   mountPoint = 2;
   offset = "0 0 0";

   lightType = "PulsingLight";
   lightColor = "0.5 0.5 0.5 1.0";
   lightTime = "1000";
   lightRadius = "3";
   cloakable = false;
};

datablock StaticShapeData(Flag2)
{
   category = "Flag";
   className = "Flag";
   image = Flag2Image;
   shapefile = "art/shapes/JediFlags/Flag2.DAE";
   mass = 55;
   elasticity = 0.2;
   friction = 0.6;
   pickupRadius = 3;
   pickUpName = "a flag";
   computeCRC = true;

   lightType = "PulsingLight";
   lightColor = "0.5 0.5 0.5 1.0";
   lightTime = "1000";
   lightRadius = "3";

   isInvincible = true;
};