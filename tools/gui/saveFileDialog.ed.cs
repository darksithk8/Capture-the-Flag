//------------------------------------------------------------------------------
// ex: getSaveFilenameEx("~/stuff/*.*", saveStuff);
//     -- calls 'saveStuff(%filename)' on ok
//------------------------------------------------------------------------------

function getSaveFilename(%filespec, %callback, %currentFile, %overwrite)
{
   if(%overwrite $= "")
      %overwrite = true;
   
   %dlg = new SaveFileDialog()
   {
      Filters = %filespec;
      DefaultFile = %currentFile;
      ChangePath = false;
      OverwritePrompt = %overwrite;
   };
   
   if(filePath( %currentFile ) !$= "")
      %dlg.DefaultPath = filePath(%currentFile);
   else
      %dlg.DefaultPath = getMainDotCSDir();
      
   if(%dlg.Execute())
   {
      %ext = strstr( %filespec, "." );
      %extEnd = strstr( %filespec, ")" );

      if( %extEnd == -1 )
         %extEnd = strstr( %filespec, "|");

      if( %extEnd == -1 )
         %extEnd = strlen(%filespec);

      %ext = getSubStr( %filespec, %ext, %extEnd - %ext );

      %filename = %dlg.FileName;
      
      eval(%callback @ "(\"" @ %filename @ "\");");
   }
   
   %dlg.delete();
}
