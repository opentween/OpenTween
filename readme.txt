Dll for XML Serializer is created in the post build event.
It isn't must needed for test using. When you want to create dll with the compiling time, you must locate 'sgen.exe' as below.
"$(DevEnvDir)..\..\SDK\v2.0\bin\sgen.exe"

$(DevEnvDir):VS macro value. 
 ex) Point the dir... C:\Program Files\Microsoft Visual Studio 8\Common7\IDE\

sgen.exe is contained in .NET Framework SDK.