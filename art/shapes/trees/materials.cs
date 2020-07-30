new Material(shrub)
{
   diffuseMap[0] = "shrub";

   translucent = true;
   translucentBlendOp = LerpAlpha;
   translucentZWrite = true;
   alphaRef = 20;
};

//--- oak1.dae MATERIALS BEGIN ---
singleton Material(oak1_oak_bark)
{
	diffuseMap[0] = "oak_bark";
	mapTo = "oak_bark";

	diffuseColor[0] = "1 1 1 1";
	specular[0] = "0 0 0 1";
	specularPower[0] = 1.07177;
	pixelSpecular[0] = false;
	emissive[0] = false;

	doubleSided = false;
	translucent = false;
	translucentBlendOp = "None";
};

singleton Material(oak1_oak_branch)
{
	diffuseMap[0] = "oak_branch";
	mapTo = "oak_branch";

	diffuseColor[0] = "1 1 1 1";

	doubleSided = false;
	translucent = true;
   translucentZWrite = true;
   alphaRef = 20;
};

//--- oak1.dae MATERIALS END ---

