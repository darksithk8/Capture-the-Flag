singleton Material( DefaultDecalRoadMaterial )
{
   diffuseMap[0] = "core/art/defaultRoadTextureTop.png";
   mapTo = "defaultpath";
};

singleton Material( BlankWhite )
{
   diffuseMap[0] = "core/art/white";
   mapTo = "white";
};

singleton Material( Empty )
{
};

singleton Material(DefaultRoadMaterialTop)
{
   mapTo = "unmapped_mat";
   diffuseMap[0] = "core/art/defaultRoadTextureTop.png";
};

singleton Material(DefaultRoadMaterialOther)
{
   mapTo = "unmapped_mat";
   diffuseMap[0] = "core/art/defaultRoadTextureOther.png";
};

