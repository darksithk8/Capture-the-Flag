//-----------------------------------------------------------------------------
// Data
//-----------------------------------------------------------------------------
uniform mat4 modelview;

varying vec2 texCoord, lmCoord;

//-----------------------------------------------------------------------------
// Main                                                                        
//-----------------------------------------------------------------------------
void main()
{
   gl_Position = modelview * gl_Vertex;

   texCoord   = gl_MultiTexCoord0.st;
   lmCoord    = gl_MultiTexCoord1.st;
}
