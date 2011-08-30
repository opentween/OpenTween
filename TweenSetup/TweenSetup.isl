<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<?xml-stylesheet type="text/xsl" href="is.xsl" ?>
<!DOCTYPE msi [
   <!ELEMENT msi   (summary,table*)>
   <!ATTLIST msi version    CDATA #REQUIRED>
   <!ATTLIST msi xmlns:dt   CDATA #IMPLIED
                 codepage   CDATA #IMPLIED
                 compression (MSZIP|LZX|none) "LZX">
   
   <!ELEMENT summary       (codepage?,title?,subject?,author?,keywords?,comments?,
                            template,lastauthor?,revnumber,lastprinted?,
                            createdtm?,lastsavedtm?,pagecount,wordcount,
                            charcount?,appname?,security?)>
                            
   <!ELEMENT codepage      (#PCDATA)>
   <!ELEMENT title         (#PCDATA)>
   <!ELEMENT subject       (#PCDATA)>
   <!ELEMENT author        (#PCDATA)>
   <!ELEMENT keywords      (#PCDATA)>
   <!ELEMENT comments      (#PCDATA)>
   <!ELEMENT template      (#PCDATA)>
   <!ELEMENT lastauthor    (#PCDATA)>
   <!ELEMENT revnumber     (#PCDATA)>
   <!ELEMENT lastprinted   (#PCDATA)>
   <!ELEMENT createdtm     (#PCDATA)>
   <!ELEMENT lastsavedtm   (#PCDATA)>
   <!ELEMENT pagecount     (#PCDATA)>
   <!ELEMENT wordcount     (#PCDATA)>
   <!ELEMENT charcount     (#PCDATA)>
   <!ELEMENT appname       (#PCDATA)>
   <!ELEMENT security      (#PCDATA)>                            
                                
   <!ELEMENT table         (col+,row*)>
   <!ATTLIST table
                name        CDATA #REQUIRED>

   <!ELEMENT col           (#PCDATA)>
   <!ATTLIST col
                 key       (yes|no) #IMPLIED
                 def       CDATA #IMPLIED>
                 
   <!ELEMENT row            (td+)>
   
   <!ELEMENT td             (#PCDATA)>
   <!ATTLIST td
                 href       CDATA #IMPLIED
                 dt:dt     (string|bin.base64) #IMPLIED
                 md5        CDATA #IMPLIED>
]>
<msi version="2.0" xmlns:dt="urn:schemas-microsoft-com:datatypes" codepage="65001">
	
	<summary>
		<codepage>1252</codepage>
		<title>Installation Database</title>
		<subject>Tween</subject>
		<author>##ID_STRING3##</author>
		<keywords>Installer,MSI,Database</keywords>
		<comments>Contact:  Your local administrator</comments>
		<template>Intel;1033</template>
		<lastauthor>Administrator</lastauthor>
		<revnumber>{C58BED49-0F30-4028-9F26-FD44CEB90DDA}</revnumber>
		<lastprinted/>
		<createdtm>06/21/1999 22:00</createdtm>
		<lastsavedtm>07/15/2000 01:50</lastsavedtm>
		<pagecount>200</pagecount>
		<wordcount>0</wordcount>
		<charcount/>
		<appname>InstallShield Express</appname>
		<security>1</security>
	</summary>
	
	<table name="ActionText">
		<col key="yes" def="s72">Action</col>
		<col def="L64">Description</col>
		<col def="L128">Template</col>
		<row><td>Advertise</td><td>##IDS_ACTIONTEXT_Advertising##</td><td/></row>
		<row><td>AllocateRegistrySpace</td><td>##IDS_ACTIONTEXT_AllocatingRegistry##</td><td>##IDS_ACTIONTEXT_FreeSpace##</td></row>
		<row><td>AppSearch</td><td>##IDS_ACTIONTEXT_SearchInstalled##</td><td>##IDS_ACTIONTEXT_PropertySignature##</td></row>
		<row><td>BindImage</td><td>##IDS_ACTIONTEXT_BindingExes##</td><td>##IDS_ACTIONTEXT_File##</td></row>
		<row><td>CCPSearch</td><td>##IDS_ACTIONTEXT_UnregisterModules##</td><td/></row>
		<row><td>CostFinalize</td><td>##IDS_ACTIONTEXT_ComputingSpace3##</td><td/></row>
		<row><td>CostInitialize</td><td>##IDS_ACTIONTEXT_ComputingSpace##</td><td/></row>
		<row><td>CreateFolders</td><td>##IDS_ACTIONTEXT_CreatingFolders##</td><td>##IDS_ACTIONTEXT_Folder##</td></row>
		<row><td>CreateShortcuts</td><td>##IDS_ACTIONTEXT_CreatingShortcuts##</td><td>##IDS_ACTIONTEXT_Shortcut##</td></row>
		<row><td>DeleteServices</td><td>##IDS_ACTIONTEXT_DeletingServices##</td><td>##IDS_ACTIONTEXT_Service##</td></row>
		<row><td>DuplicateFiles</td><td>##IDS_ACTIONTEXT_CreatingDuplicate##</td><td>##IDS_ACTIONTEXT_FileDirectorySize##</td></row>
		<row><td>FileCost</td><td>##IDS_ACTIONTEXT_ComputingSpace2##</td><td/></row>
		<row><td>FindRelatedProducts</td><td>##IDS_ACTIONTEXT_SearchForRelated##</td><td>##IDS_ACTIONTEXT_FoundApp##</td></row>
		<row><td>GenerateScript</td><td>##IDS_ACTIONTEXT_GeneratingScript##</td><td>##IDS_ACTIONTEXT_1##</td></row>
		<row><td>ISLockPermissionsCost</td><td>##IDS_ACTIONTEXT_ISLockPermissionsCost##</td><td/></row>
		<row><td>ISLockPermissionsInstall</td><td>##IDS_ACTIONTEXT_ISLockPermissionsInstall##</td><td/></row>
		<row><td>InstallAdminPackage</td><td>##IDS_ACTIONTEXT_CopyingNetworkFiles##</td><td>##IDS_ACTIONTEXT_FileDirSize##</td></row>
		<row><td>InstallFiles</td><td>##IDS_ACTIONTEXT_CopyingNewFiles##</td><td>##IDS_ACTIONTEXT_FileDirSize2##</td></row>
		<row><td>InstallODBC</td><td>##IDS_ACTIONTEXT_InstallODBC##</td><td/></row>
		<row><td>InstallSFPCatalogFile</td><td>##IDS_ACTIONTEXT_InstallingSystemCatalog##</td><td>##IDS_ACTIONTEXT_FileDependencies##</td></row>
		<row><td>InstallServices</td><td>##IDS_ACTIONTEXT_InstallServices##</td><td>##IDS_ACTIONTEXT_Service2##</td></row>
		<row><td>InstallValidate</td><td>##IDS_ACTIONTEXT_Validating##</td><td/></row>
		<row><td>LaunchConditions</td><td>##IDS_ACTIONTEXT_EvaluateLaunchConditions##</td><td/></row>
		<row><td>MigrateFeatureStates</td><td>##IDS_ACTIONTEXT_MigratingFeatureStates##</td><td>##IDS_ACTIONTEXT_Application##</td></row>
		<row><td>MoveFiles</td><td>##IDS_ACTIONTEXT_MovingFiles##</td><td>##IDS_ACTIONTEXT_FileDirSize3##</td></row>
		<row><td>PatchFiles</td><td>##IDS_ACTIONTEXT_PatchingFiles##</td><td>##IDS_ACTIONTEXT_FileDirSize4##</td></row>
		<row><td>ProcessComponents</td><td>##IDS_ACTIONTEXT_UpdateComponentRegistration##</td><td/></row>
		<row><td>PublishComponents</td><td>##IDS_ACTIONTEXT_PublishingQualifiedComponents##</td><td>##IDS_ACTIONTEXT_ComponentIDQualifier##</td></row>
		<row><td>PublishFeatures</td><td>##IDS_ACTIONTEXT_PublishProductFeatures##</td><td>##IDS_ACTIONTEXT_FeatureColon##</td></row>
		<row><td>PublishProduct</td><td>##IDS_ACTIONTEXT_PublishProductInfo##</td><td/></row>
		<row><td>RMCCPSearch</td><td>##IDS_ACTIONTEXT_SearchingQualifyingProducts##</td><td/></row>
		<row><td>RegisterClassInfo</td><td>##IDS_ACTIONTEXT_RegisterClassServer##</td><td>##IDS_ACTIONTEXT_ClassId##</td></row>
		<row><td>RegisterComPlus</td><td>##IDS_ACTIONTEXT_RegisteringComPlus##</td><td>##IDS_ACTIONTEXT_AppIdAppTypeRSN##</td></row>
		<row><td>RegisterExtensionInfo</td><td>##IDS_ACTIONTEXT_RegisterExtensionServers##</td><td>##IDS_ACTIONTEXT_Extension2##</td></row>
		<row><td>RegisterFonts</td><td>##IDS_ACTIONTEXT_RegisterFonts##</td><td>##IDS_ACTIONTEXT_Font##</td></row>
		<row><td>RegisterMIMEInfo</td><td>##IDS_ACTIONTEXT_RegisterMimeInfo##</td><td>##IDS_ACTIONTEXT_ContentTypeExtension##</td></row>
		<row><td>RegisterProduct</td><td>##IDS_ACTIONTEXT_RegisteringProduct##</td><td>##IDS_ACTIONTEXT_1b##</td></row>
		<row><td>RegisterProgIdInfo</td><td>##IDS_ACTIONTEXT_RegisteringProgIdentifiers##</td><td>##IDS_ACTIONTEXT_ProgID2##</td></row>
		<row><td>RegisterTypeLibraries</td><td>##IDS_ACTIONTEXT_RegisterTypeLibs##</td><td>##IDS_ACTIONTEXT_LibId##</td></row>
		<row><td>RegisterUser</td><td>##IDS_ACTIONTEXT_RegUser##</td><td>##IDS_ACTIONTEXT_1c##</td></row>
		<row><td>RemoveDuplicateFiles</td><td>##IDS_ACTIONTEXT_RemovingDuplicates##</td><td>##IDS_ACTIONTEXT_FileDir##</td></row>
		<row><td>RemoveEnvironmentStrings</td><td>##IDS_ACTIONTEXT_UpdateEnvironmentStrings##</td><td>##IDS_ACTIONTEXT_NameValueAction2##</td></row>
		<row><td>RemoveExistingProducts</td><td>##IDS_ACTIONTEXT_RemoveApps##</td><td>##IDS_ACTIONTEXT_AppCommandLine##</td></row>
		<row><td>RemoveFiles</td><td>##IDS_ACTIONTEXT_RemovingFiles##</td><td>##IDS_ACTIONTEXT_FileDir2##</td></row>
		<row><td>RemoveFolders</td><td>##IDS_ACTIONTEXT_RemovingFolders##</td><td>##IDS_ACTIONTEXT_Folder1##</td></row>
		<row><td>RemoveIniValues</td><td>##IDS_ACTIONTEXT_RemovingIni##</td><td>##IDS_ACTIONTEXT_FileSectionKeyValue##</td></row>
		<row><td>RemoveODBC</td><td>##IDS_ACTIONTEXT_RemovingODBC##</td><td/></row>
		<row><td>RemoveRegistryValues</td><td>##IDS_ACTIONTEXT_RemovingRegistry##</td><td>##IDS_ACTIONTEXT_KeyName##</td></row>
		<row><td>RemoveShortcuts</td><td>##IDS_ACTIONTEXT_RemovingShortcuts##</td><td>##IDS_ACTIONTEXT_Shortcut1##</td></row>
		<row><td>Rollback</td><td>##IDS_ACTIONTEXT_RollingBack##</td><td>##IDS_ACTIONTEXT_1d##</td></row>
		<row><td>RollbackCleanup</td><td>##IDS_ACTIONTEXT_RemovingBackup##</td><td>##IDS_ACTIONTEXT_File2##</td></row>
		<row><td>SelfRegModules</td><td>##IDS_ACTIONTEXT_RegisteringModules##</td><td>##IDS_ACTIONTEXT_FileFolder##</td></row>
		<row><td>SelfUnregModules</td><td>##IDS_ACTIONTEXT_UnregisterModules##</td><td>##IDS_ACTIONTEXT_FileFolder2##</td></row>
		<row><td>SetODBCFolders</td><td>##IDS_ACTIONTEXT_InitializeODBCDirs##</td><td/></row>
		<row><td>StartServices</td><td>##IDS_ACTIONTEXT_StartingServices##</td><td>##IDS_ACTIONTEXT_Service3##</td></row>
		<row><td>StopServices</td><td>##IDS_ACTIONTEXT_StoppingServices##</td><td>##IDS_ACTIONTEXT_Service4##</td></row>
		<row><td>UnmoveFiles</td><td>##IDS_ACTIONTEXT_RemovingMoved##</td><td>##IDS_ACTIONTEXT_FileDir3##</td></row>
		<row><td>UnpublishComponents</td><td>##IDS_ACTIONTEXT_UnpublishQualified##</td><td>##IDS_ACTIONTEXT_ComponentIdQualifier2##</td></row>
		<row><td>UnpublishFeatures</td><td>##IDS_ACTIONTEXT_UnpublishProductFeatures##</td><td>##IDS_ACTIONTEXT_Feature##</td></row>
		<row><td>UnpublishProduct</td><td>##IDS_ACTIONTEXT_UnpublishingProductInfo##</td><td/></row>
		<row><td>UnregisterClassInfo</td><td>##IDS_ACTIONTEXT_UnregisterClassServers##</td><td>##IDS_ACTIONTEXT_ClsID##</td></row>
		<row><td>UnregisterComPlus</td><td>##IDS_ACTIONTEXT_UnregisteringComPlus##</td><td>##IDS_ACTIONTEXT_AppId##</td></row>
		<row><td>UnregisterExtensionInfo</td><td>##IDS_ACTIONTEXT_UnregisterExtensionServers##</td><td>##IDS_ACTIONTEXT_Extension##</td></row>
		<row><td>UnregisterFonts</td><td>##IDS_ACTIONTEXT_UnregisteringFonts##</td><td>##IDS_ACTIONTEXT_Font2##</td></row>
		<row><td>UnregisterMIMEInfo</td><td>##IDS_ACTIONTEXT_UnregisteringMimeInfo##</td><td>##IDS_ACTIONTEXT_ContentTypeExtension2##</td></row>
		<row><td>UnregisterProgIdInfo</td><td>##IDS_ACTIONTEXT_UnregisteringProgramIds##</td><td>##IDS_ACTIONTEXT_ProgID##</td></row>
		<row><td>UnregisterTypeLibraries</td><td>##IDS_ACTIONTEXT_UnregTypeLibs##</td><td>##IDS_ACTIONTEXT_Libid2##</td></row>
		<row><td>WriteEnvironmentStrings</td><td>##IDS_ACTIONTEXT_EnvironmentStrings##</td><td>##IDS_ACTIONTEXT_NameValueAction##</td></row>
		<row><td>WriteIniValues</td><td>##IDS_ACTIONTEXT_WritingINI##</td><td>##IDS_ACTIONTEXT_FileSectionKeyValue2##</td></row>
		<row><td>WriteRegistryValues</td><td>##IDS_ACTIONTEXT_WritingRegistry##</td><td>##IDS_ACTIONTEXT_KeyNameValue##</td></row>
	</table>

	<table name="AdminExecuteSequence">
		<col key="yes" def="s72">Action</col>
		<col def="S255">Condition</col>
		<col def="I2">Sequence</col>
		<col def="S255">ISComments</col>
		<col def="I4">ISAttributes</col>
		<row><td>CostFinalize</td><td/><td>1000</td><td>CostFinalize</td><td/></row>
		<row><td>CostInitialize</td><td/><td>800</td><td>CostInitialize</td><td/></row>
		<row><td>FileCost</td><td/><td>900</td><td>FileCost</td><td/></row>
		<row><td>InstallAdminPackage</td><td/><td>3900</td><td>InstallAdminPackage</td><td/></row>
		<row><td>InstallFiles</td><td/><td>4000</td><td>InstallFiles</td><td/></row>
		<row><td>InstallFinalize</td><td/><td>6600</td><td>InstallFinalize</td><td/></row>
		<row><td>InstallInitialize</td><td/><td>1500</td><td>InstallInitialize</td><td/></row>
		<row><td>InstallValidate</td><td/><td>1400</td><td>InstallValidate</td><td/></row>
		<row><td>ScheduleReboot</td><td>ISSCHEDULEREBOOT</td><td>4010</td><td>ScheduleReboot</td><td/></row>
	</table>

	<table name="AdminUISequence">
		<col key="yes" def="s72">Action</col>
		<col def="S255">Condition</col>
		<col def="I2">Sequence</col>
		<col def="S255">ISComments</col>
		<col def="I4">ISAttributes</col>
		<row><td>AdminWelcome</td><td/><td>1010</td><td>AdminWelcome</td><td/></row>
		<row><td>CostFinalize</td><td/><td>1000</td><td>CostFinalize</td><td/></row>
		<row><td>CostInitialize</td><td/><td>800</td><td>CostInitialize</td><td/></row>
		<row><td>ExecuteAction</td><td/><td>1300</td><td>ExecuteAction</td><td/></row>
		<row><td>FileCost</td><td/><td>900</td><td>FileCost</td><td/></row>
		<row><td>SetupCompleteError</td><td/><td>-3</td><td>SetupCompleteError</td><td/></row>
		<row><td>SetupCompleteSuccess</td><td/><td>-1</td><td>SetupCompleteSuccess</td><td/></row>
		<row><td>SetupInitialization</td><td/><td>50</td><td>SetupInitialization</td><td/></row>
		<row><td>SetupInterrupted</td><td/><td>-2</td><td>SetupInterrupted</td><td/></row>
		<row><td>SetupProgress</td><td/><td>1020</td><td>SetupProgress</td><td/></row>
	</table>

	<table name="AdvtExecuteSequence">
		<col key="yes" def="s72">Action</col>
		<col def="S255">Condition</col>
		<col def="I2">Sequence</col>
		<col def="S255">ISComments</col>
		<col def="I4">ISAttributes</col>
		<row><td>CostFinalize</td><td/><td>1000</td><td>CostFinalize</td><td/></row>
		<row><td>CostInitialize</td><td/><td>800</td><td>CostInitialize</td><td/></row>
		<row><td>CreateShortcuts</td><td/><td>4500</td><td>CreateShortcuts</td><td/></row>
		<row><td>InstallFinalize</td><td/><td>6600</td><td>InstallFinalize</td><td/></row>
		<row><td>InstallInitialize</td><td/><td>1500</td><td>InstallInitialize</td><td/></row>
		<row><td>InstallValidate</td><td/><td>1400</td><td>InstallValidate</td><td/></row>
		<row><td>MsiPublishAssemblies</td><td/><td>6250</td><td>MsiPublishAssemblies</td><td/></row>
		<row><td>PublishComponents</td><td/><td>6200</td><td>PublishComponents</td><td/></row>
		<row><td>PublishFeatures</td><td/><td>6300</td><td>PublishFeatures</td><td/></row>
		<row><td>PublishProduct</td><td/><td>6400</td><td>PublishProduct</td><td/></row>
		<row><td>RegisterClassInfo</td><td/><td>4600</td><td>RegisterClassInfo</td><td/></row>
		<row><td>RegisterExtensionInfo</td><td/><td>4700</td><td>RegisterExtensionInfo</td><td/></row>
		<row><td>RegisterMIMEInfo</td><td/><td>4900</td><td>RegisterMIMEInfo</td><td/></row>
		<row><td>RegisterProgIdInfo</td><td/><td>4800</td><td>RegisterProgIdInfo</td><td/></row>
		<row><td>RegisterTypeLibraries</td><td/><td>4910</td><td>RegisterTypeLibraries</td><td/></row>
		<row><td>ScheduleReboot</td><td>ISSCHEDULEREBOOT</td><td>6410</td><td>ScheduleReboot</td><td/></row>
	</table>

	<table name="AdvtUISequence">
		<col key="yes" def="s72">Action</col>
		<col def="S255">Condition</col>
		<col def="I2">Sequence</col>
		<col def="S255">ISComments</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="AppId">
		<col key="yes" def="s38">AppId</col>
		<col def="S255">RemoteServerName</col>
		<col def="S255">LocalService</col>
		<col def="S255">ServiceParameters</col>
		<col def="S255">DllSurrogate</col>
		<col def="I2">ActivateAtStorage</col>
		<col def="I2">RunAsInteractiveUser</col>
	</table>

	<table name="AppSearch">
		<col key="yes" def="s72">Property</col>
		<col key="yes" def="s72">Signature_</col>
		<row><td>DOTNETVERSION40FULL</td><td>DotNet40Full</td></row>
	</table>

	<table name="BBControl">
		<col key="yes" def="s50">Billboard_</col>
		<col key="yes" def="s50">BBControl</col>
		<col def="s50">Type</col>
		<col def="i2">X</col>
		<col def="i2">Y</col>
		<col def="i2">Width</col>
		<col def="i2">Height</col>
		<col def="I4">Attributes</col>
		<col def="L50">Text</col>
	</table>

	<table name="Billboard">
		<col key="yes" def="s50">Billboard</col>
		<col def="s38">Feature_</col>
		<col def="S50">Action</col>
		<col def="I2">Ordering</col>
	</table>

	<table name="Binary">
		<col key="yes" def="s72">Name</col>
		<col def="V0">Data</col>
		<col def="S255">ISBuildSourcePath</col>
		<row><td>ISExpHlp.dll</td><td/><td>&lt;ISProductFolder&gt;\redist\language independent\i386\ISExpHlp.dll</td></row>
		<row><td>ISSELFREG.DLL</td><td/><td>&lt;ISProductFolder&gt;\redist\language independent\i386\isregsvr.dll</td></row>
		<row><td>NewBinary1</td><td/><td>&lt;ISProductFolder&gt;\Support\Themes\InstallShield Blue Theme\banner.jpg</td></row>
		<row><td>NewBinary10</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\CompleteSetupIco.ibd</td></row>
		<row><td>NewBinary11</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\CustomSetupIco.ibd</td></row>
		<row><td>NewBinary12</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\DestIcon.ibd</td></row>
		<row><td>NewBinary13</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\NetworkInstall.ico</td></row>
		<row><td>NewBinary14</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\DontInstall.ico</td></row>
		<row><td>NewBinary15</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\Install.ico</td></row>
		<row><td>NewBinary16</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\InstallFirstUse.ico</td></row>
		<row><td>NewBinary17</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\InstallPartial.ico</td></row>
		<row><td>NewBinary18</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\InstallStateMenu.ico</td></row>
		<row><td>NewBinary19</td><td/><td>&lt;ISProductFolder&gt;\redist\Language independent\i386\SerialNumCAHelper.dll</td></row>
		<row><td>NewBinary2</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\New.ibd</td></row>
		<row><td>NewBinary20</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\InstallerBackground.jpg</td></row>
		<row><td>NewBinary21</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\InstallerBackground.jpg</td></row>
		<row><td>NewBinary22</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\InstallerBackground.jpg</td></row>
		<row><td>NewBinary3</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\Up.ibd</td></row>
		<row><td>NewBinary4</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\WarningIcon.ibd</td></row>
		<row><td>NewBinary5</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\InstallerBackground.jpg</td></row>
		<row><td>NewBinary6</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\CustomSetupIco.ibd</td></row>
		<row><td>NewBinary7</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\ReinstIco.ibd</td></row>
		<row><td>NewBinary8</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\RemoveIco.ibd</td></row>
		<row><td>NewBinary9</td><td/><td>&lt;ISProductFolder&gt;\Redist\Language Independent\OS Independent\SetupIcon.ibd</td></row>
		<row><td>SetAllUsers.dll</td><td/><td>&lt;ISProductFolder&gt;\redist\language independent\i386\SetAllUsers.dll</td></row>
	</table>

	<table name="BindImage">
		<col key="yes" def="s72">File_</col>
		<col def="S255">Path</col>
	</table>

	<table name="CCPSearch">
		<col key="yes" def="s72">Signature_</col>
	</table>

	<table name="CheckBox">
		<col key="yes" def="s72">Property</col>
		<col def="S64">Value</col>
		<row><td>ISCHECKFORPRODUCTUPDATES</td><td>1</td></row>
		<row><td>LAUNCHPROGRAM</td><td>1</td></row>
		<row><td>LAUNCHREADME</td><td>1</td></row>
	</table>

	<table name="Class">
		<col key="yes" def="s38">CLSID</col>
		<col key="yes" def="s32">Context</col>
		<col key="yes" def="s72">Component_</col>
		<col def="S255">ProgId_Default</col>
		<col def="L255">Description</col>
		<col def="S38">AppId_</col>
		<col def="S255">FileTypeMask</col>
		<col def="S72">Icon_</col>
		<col def="I2">IconIndex</col>
		<col def="S32">DefInprocHandler</col>
		<col def="S255">Argument</col>
		<col def="s38">Feature_</col>
		<col def="I2">Attributes</col>
	</table>

	<table name="ComboBox">
		<col key="yes" def="s72">Property</col>
		<col key="yes" def="i2">Order</col>
		<col def="s64">Value</col>
		<col def="L64">Text</col>
	</table>

	<table name="CompLocator">
		<col key="yes" def="s72">Signature_</col>
		<col def="s38">ComponentId</col>
		<col def="I2">Type</col>
	</table>

	<table name="Complus">
		<col key="yes" def="s72">Component_</col>
		<col key="yes" def="I2">ExpType</col>
	</table>

	<table name="Component">
		<col key="yes" def="s72">Component</col>
		<col def="S38">ComponentId</col>
		<col def="s72">Directory_</col>
		<col def="i2">Attributes</col>
		<col def="S255">Condition</col>
		<col def="S72">KeyPath</col>
		<col def="I4">ISAttributes</col>
		<col def="S255">ISComments</col>
		<col def="S255">ISScanAtBuildFile</col>
		<col def="S255">ISRegFileToMergeAtBuild</col>
		<col def="S0">ISDotNetInstallerArgsInstall</col>
		<col def="S0">ISDotNetInstallerArgsCommit</col>
		<col def="S0">ISDotNetInstallerArgsUninstall</col>
		<col def="S0">ISDotNetInstallerArgsRollback</col>
		<row><td>Component</td><td>{38E56A9E-E540-4001-B776-6E6311941803}</td><td>INSTALLDIR</td><td>2</td><td/><td>File</td><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>
		<row><td>Component2</td><td>{8729D757-A6DA-4AFC-BB25-3CCFE2DBE652}</td><td>INSTALLDIR</td><td>2</td><td/><td>File2</td><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>
		<row><td>ISX_DEFAULTCOMPONENT</td><td>{08249481-D113-401C-9F1A-FB519C68356F}</td><td>INSTALLDIR</td><td>2</td><td/><td/><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>
		<row><td>NewComponent1</td><td>{0C5CC1E9-5A4A-4205-B48C-036C9D646095}</td><td>INSTALLDIR</td><td>2</td><td/><td/><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>
		<row><td>NewComponent2</td><td>{1C0FD176-7AE3-4700-918E-BE7899448863}</td><td>INSTALLDIR</td><td>2</td><td/><td/><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>
		<row><td>Tween.XmlSerializers.dll</td><td>{1451686E-A96E-4779-8C92-D818ADB0A605}</td><td>INSTALLDIR</td><td>2</td><td/><td>tween.xmlserializers.dll</td><td>17</td><td/><td/><td/><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td><td>/LogFile=</td></row>
	</table>

	<table name="Condition">
		<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="i2">Level</col>
		<col def="S255">Condition</col>
	</table>

	<table name="Control">
		<col key="yes" def="s72">Dialog_</col>
		<col key="yes" def="s50">Control</col>
		<col def="s20">Type</col>
		<col def="i2">X</col>
		<col def="i2">Y</col>
		<col def="i2">Width</col>
		<col def="i2">Height</col>
		<col def="I4">Attributes</col>
		<col def="S72">Property</col>
		<col def="L0">Text</col>
		<col def="S50">Control_Next</col>
		<col def="L50">Help</col>
		<col def="I4">ISWindowStyle</col>
		<col def="I4">ISControlId</col>
		<col def="S255">ISBuildSourcePath</col>
		<col def="S72">Binary_</col>
		<row><td>AdminChangeFolder</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>AdminChangeFolder</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>ComboText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>Combo</td><td>DirectoryCombo</td><td>21</td><td>64</td><td>277</td><td>80</td><td>458755</td><td>TARGETDIR</td><td>##IDS__IsAdminInstallBrowse_4##</td><td>Up</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>ComboText</td><td>Text</td><td>21</td><td>50</td><td>99</td><td>14</td><td>3</td><td/><td>##IDS__IsAdminInstallBrowse_LookIn##</td><td>Combo</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsAdminInstallBrowse_BrowseDestination##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsAdminInstallBrowse_ChangeDestination##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>List</td><td>DirectoryList</td><td>21</td><td>90</td><td>332</td><td>97</td><td>7</td><td>TARGETDIR</td><td>##IDS__IsAdminInstallBrowse_8##</td><td>TailText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>NewFolder</td><td>PushButton</td><td>335</td><td>66</td><td>19</td><td>19</td><td>3670019</td><td/><td/><td>List</td><td>##IDS__IsAdminInstallBrowse_CreateFolder##</td><td>0</td><td/><td/><td>NewBinary2</td></row>
		<row><td>AdminChangeFolder</td><td>OK</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_OK##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>Tail</td><td>PathEdit</td><td>21</td><td>207</td><td>332</td><td>17</td><td>3</td><td>TARGETDIR</td><td>##IDS__IsAdminInstallBrowse_11##</td><td>OK</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>TailText</td><td>Text</td><td>21</td><td>193</td><td>99</td><td>13</td><td>3</td><td/><td>##IDS__IsAdminInstallBrowse_FolderName##</td><td>Tail</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminChangeFolder</td><td>Up</td><td>PushButton</td><td>310</td><td>66</td><td>19</td><td>19</td><td>3670019</td><td/><td/><td>NewFolder</td><td>##IDS__IsAdminInstallBrowse_UpOneLevel##</td><td>0</td><td/><td/><td>NewBinary3</td></row>
		<row><td>AdminNetworkLocation</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>InstallNow</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>AdminNetworkLocation</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>Browse</td><td>PushButton</td><td>286</td><td>124</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsAdminInstallPoint_Change##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>SetupPathEdit</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsAdminInstallPoint_SpecifyNetworkLocation##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>DlgText</td><td>Text</td><td>21</td><td>51</td><td>326</td><td>40</td><td>131075</td><td/><td>##IDS__IsAdminInstallPoint_EnterNetworkLocation##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsAdminInstallPoint_NetworkLocationFormatted##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>InstallNow</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsAdminInstallPoint_Install##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>LBBrowse</td><td>Text</td><td>21</td><td>90</td><td>100</td><td>10</td><td>3</td><td/><td>##IDS__IsAdminInstallPoint_NetworkLocation##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminNetworkLocation</td><td>SetupPathEdit</td><td>PathEdit</td><td>21</td><td>102</td><td>330</td><td>17</td><td>3</td><td>TARGETDIR</td><td/><td>Browse</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminWelcome</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminWelcome</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminWelcome</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminWelcome</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>AdminWelcome</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminWelcome</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>196611</td><td/><td>##IDS__IsAdminInstallPointWelcome_Wizard##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>AdminWelcome</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>45</td><td>196611</td><td/><td>##IDS__IsAdminInstallPointWelcome_ServerImage##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CancelSetup</td><td>Icon</td><td>Icon</td><td>15</td><td>15</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary4</td></row>
		<row><td>CancelSetup</td><td>No</td><td>PushButton</td><td>135</td><td>57</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsCancelDlg_No##</td><td>Yes</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CancelSetup</td><td>Text</td><td>Text</td><td>48</td><td>15</td><td>194</td><td>30</td><td>131075</td><td/><td>##IDS__IsCancelDlg_ConfirmCancel##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CancelSetup</td><td>Yes</td><td>PushButton</td><td>62</td><td>57</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsCancelDlg_Yes##</td><td>No</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>CustomSetup</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Tree</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>ChangeFolder</td><td>PushButton</td><td>301</td><td>203</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_Change##</td><td>Help</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Details</td><td>PushButton</td><td>93</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_Space##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>DlgDesc</td><td>Text</td><td>17</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsCustomSelectionDlg_SelectFeatures##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>DlgText</td><td>Text</td><td>9</td><td>51</td><td>360</td><td>10</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_ClickFeatureIcon##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>DlgTitle</td><td>Text</td><td>9</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsCustomSelectionDlg_CustomSetup##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>FeatureGroup</td><td>GroupBox</td><td>235</td><td>67</td><td>131</td><td>120</td><td>1</td><td/><td>##IDS__IsCustomSelectionDlg_FeatureDescription##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Help</td><td>PushButton</td><td>22</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_Help##</td><td>Details</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>InstallLabel</td><td>Text</td><td>8</td><td>190</td><td>360</td><td>10</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_InstallTo##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>ItemDescription</td><td>Text</td><td>241</td><td>80</td><td>120</td><td>50</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_MultilineDescription##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Location</td><td>Text</td><td>8</td><td>203</td><td>291</td><td>20</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_FeaturePath##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Size</td><td>Text</td><td>241</td><td>133</td><td>120</td><td>50</td><td>3</td><td/><td>##IDS__IsCustomSelectionDlg_FeatureSize##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetup</td><td>Tree</td><td>SelectionTree</td><td>8</td><td>70</td><td>220</td><td>118</td><td>7</td><td>_BrowseProperty</td><td/><td>ChangeFolder</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>CustomSetupTips</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS_SetupTips_CustomSetupDescription##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS_SetupTips_CustomSetup##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>DontInstall</td><td>Icon</td><td>21</td><td>155</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary14</td></row>
		<row><td>CustomSetupTips</td><td>DontInstallText</td><td>Text</td><td>60</td><td>155</td><td>300</td><td>20</td><td>3</td><td/><td>##IDS_SetupTips_WillNotBeInstalled##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>FirstInstallText</td><td>Text</td><td>60</td><td>180</td><td>300</td><td>20</td><td>3</td><td/><td>##IDS_SetupTips_Advertise##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>Install</td><td>Icon</td><td>21</td><td>105</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary15</td></row>
		<row><td>CustomSetupTips</td><td>InstallFirstUse</td><td>Icon</td><td>21</td><td>180</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary16</td></row>
		<row><td>CustomSetupTips</td><td>InstallPartial</td><td>Icon</td><td>21</td><td>130</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary17</td></row>
		<row><td>CustomSetupTips</td><td>InstallStateMenu</td><td>Icon</td><td>21</td><td>52</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary18</td></row>
		<row><td>CustomSetupTips</td><td>InstallStateText</td><td>Text</td><td>21</td><td>91</td><td>300</td><td>10</td><td>3</td><td/><td>##IDS_SetupTips_InstallState##</td><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>CustomSetupTips</td><td>InstallText</td><td>Text</td><td>60</td><td>105</td><td>300</td><td>20</td><td>3</td><td/><td>##IDS_SetupTips_AllInstalledLocal##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>MenuText</td><td>Text</td><td>50</td><td>52</td><td>300</td><td>36</td><td>3</td><td/><td>##IDS_SetupTips_IconInstallState##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>NetworkInstall</td><td>Icon</td><td>21</td><td>205</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary13</td></row>
		<row><td>CustomSetupTips</td><td>NetworkInstallText</td><td>Text</td><td>60</td><td>205</td><td>300</td><td>20</td><td>3</td><td/><td>##IDS_SetupTips_Network##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>OK</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_SetupTips_OK##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomSetupTips</td><td>PartialText</td><td>Text</td><td>60</td><td>130</td><td>300</td><td>20</td><td>3</td><td/><td>##IDS_SetupTips_SubFeaturesInstalledLocal##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>CustomerInformation</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>NameLabel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>CompanyEdit</td><td>Edit</td><td>21</td><td>100</td><td>237</td><td>17</td><td>3</td><td>COMPANYNAME</td><td>##IDS__IsRegisterUserDlg_Tahoma80##</td><td>SerialLabel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>CompanyLabel</td><td>Text</td><td>21</td><td>89</td><td>75</td><td>10</td><td>3</td><td/><td>##IDS__IsRegisterUserDlg_Organization##</td><td>CompanyEdit</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsRegisterUserDlg_PleaseEnterInfo##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>DlgRadioGroupText</td><td>Text</td><td>21</td><td>161</td><td>300</td><td>14</td><td>2</td><td/><td>##IDS__IsRegisterUserDlg_InstallFor##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsRegisterUserDlg_CustomerInformation##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>NameEdit</td><td>Edit</td><td>21</td><td>63</td><td>237</td><td>17</td><td>3</td><td>USERNAME</td><td>##IDS__IsRegisterUserDlg_Tahoma50##</td><td>CompanyLabel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>NameLabel</td><td>Text</td><td>21</td><td>52</td><td>75</td><td>10</td><td>3</td><td/><td>##IDS__IsRegisterUserDlg_UserName##</td><td>NameEdit</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>RadioGroup</td><td>RadioButtonGroup</td><td>63</td><td>170</td><td>300</td><td>50</td><td>2</td><td>ApplicationUsers</td><td>##IDS__IsRegisterUserDlg_16##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>SerialLabel</td><td>Text</td><td>21</td><td>127</td><td>109</td><td>10</td><td>2</td><td/><td>##IDS__IsRegisterUserDlg_SerialNumber##</td><td>SerialNumber</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>CustomerInformation</td><td>SerialNumber</td><td>MaskedEdit</td><td>21</td><td>138</td><td>237</td><td>17</td><td>2</td><td>ISX_SERIALNUM</td><td/><td>RadioGroup</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>DatabaseFolder</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>ChangeFolder</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>ChangeFolder</td><td>PushButton</td><td>301</td><td>65</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CHANGE##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>DatabaseFolder</td><td>Icon</td><td>21</td><td>52</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary12</td></row>
		<row><td>DatabaseFolder</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__DatabaseFolder_ChangeFolder##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__DatabaseFolder_DatabaseFolder##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>LocLabel</td><td>Text</td><td>57</td><td>52</td><td>290</td><td>10</td><td>131075</td><td/><td>##IDS_DatabaseFolder_InstallDatabaseTo##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Location</td><td>Text</td><td>57</td><td>65</td><td>240</td><td>40</td><td>3</td><td>_BrowseProperty</td><td>##IDS__DatabaseFolder_DatabaseDir##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DatabaseFolder</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>DestinationFolder</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>ChangeFolder</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>ChangeFolder</td><td>PushButton</td><td>301</td><td>65</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__DestinationFolder_Change##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>DestFolder</td><td>Icon</td><td>21</td><td>52</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary12</td></row>
		<row><td>DestinationFolder</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__DestinationFolder_ChangeFolder##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__DestinationFolder_DestinationFolder##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>LocLabel</td><td>Text</td><td>57</td><td>52</td><td>290</td><td>10</td><td>131075</td><td/><td>##IDS__DestinationFolder_InstallTo##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Location</td><td>Text</td><td>57</td><td>65</td><td>240</td><td>40</td><td>3</td><td>_BrowseProperty</td><td>##IDS_INSTALLDIR##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DestinationFolder</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>DiskSpaceRequirements</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>DlgDesc</td><td>Text</td><td>17</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsFeatureDetailsDlg_SpaceRequired##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>DlgText</td><td>Text</td><td>10</td><td>185</td><td>358</td><td>41</td><td>3</td><td/><td>##IDS__IsFeatureDetailsDlg_VolumesTooSmall##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>DlgTitle</td><td>Text</td><td>9</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsFeatureDetailsDlg_DiskSpaceRequirements##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>List</td><td>VolumeCostList</td><td>8</td><td>55</td><td>358</td><td>125</td><td>393223</td><td/><td>##IDS__IsFeatureDetailsDlg_Numbers##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>DiskSpaceRequirements</td><td>OK</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsFeatureDetailsDlg_OK##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>FilesInUse</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsFilesInUse_FilesInUseMessage##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>DlgText</td><td>Text</td><td>21</td><td>51</td><td>348</td><td>33</td><td>3</td><td/><td>##IDS__IsFilesInUse_ApplicationsUsingFiles##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsFilesInUse_FilesInUse##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>Exit</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsFilesInUse_Exit##</td><td>List</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>Ignore</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsFilesInUse_Ignore##</td><td>Exit</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>List</td><td>ListBox</td><td>21</td><td>87</td><td>331</td><td>135</td><td>7</td><td>FileInUseProcess</td><td/><td>Retry</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>FilesInUse</td><td>Retry</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsFilesInUse_Retry##</td><td>Ignore</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>InstallChangeFolder</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>ComboText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Combo</td><td>DirectoryCombo</td><td>21</td><td>64</td><td>277</td><td>80</td><td>4128779</td><td>_BrowseProperty</td><td>##IDS__IsBrowseFolderDlg_4##</td><td>Up</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>ComboText</td><td>Text</td><td>21</td><td>50</td><td>99</td><td>14</td><td>3</td><td/><td>##IDS__IsBrowseFolderDlg_LookIn##</td><td>Combo</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsBrowseFolderDlg_BrowseDestFolder##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsBrowseFolderDlg_ChangeCurrentFolder##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>List</td><td>DirectoryList</td><td>21</td><td>90</td><td>332</td><td>97</td><td>15</td><td>_BrowseProperty</td><td>##IDS__IsBrowseFolderDlg_8##</td><td>TailText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>NewFolder</td><td>PushButton</td><td>335</td><td>66</td><td>19</td><td>19</td><td>3670019</td><td/><td/><td>List</td><td>##IDS__IsBrowseFolderDlg_CreateFolder##</td><td>0</td><td/><td/><td>NewBinary2</td></row>
		<row><td>InstallChangeFolder</td><td>OK</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsBrowseFolderDlg_OK##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Tail</td><td>PathEdit</td><td>21</td><td>207</td><td>332</td><td>17</td><td>15</td><td>_BrowseProperty</td><td>##IDS__IsBrowseFolderDlg_11##</td><td>OK</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>TailText</td><td>Text</td><td>21</td><td>193</td><td>99</td><td>13</td><td>3</td><td/><td>##IDS__IsBrowseFolderDlg_FolderName##</td><td>Tail</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallChangeFolder</td><td>Up</td><td>PushButton</td><td>310</td><td>66</td><td>19</td><td>19</td><td>3670019</td><td/><td/><td>NewFolder</td><td>##IDS__IsBrowseFolderDlg_UpOneLevel##</td><td>0</td><td/><td/><td>NewBinary3</td></row>
		<row><td>InstallWelcome</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Copyright</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallWelcome</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallWelcome</td><td>Copyright</td><td>Text</td><td>135</td><td>144</td><td>228</td><td>73</td><td>65539</td><td/><td>##IDS__IsWelcomeDlg_WarningCopyright##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallWelcome</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallWelcome</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>InstallWelcome</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallWelcome</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>196611</td><td/><td>##IDS__IsWelcomeDlg_WelcomeProductName##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>InstallWelcome</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>45</td><td>196611</td><td/><td>##IDS__IsWelcomeDlg_InstallProductName##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Agree</td><td>RadioButtonGroup</td><td>8</td><td>190</td><td>291</td><td>40</td><td>3</td><td>AgreeToLicense</td><td/><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>LicenseAgreement</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>ISPrintButton</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsLicenseDlg_ReadLicenseAgreement##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsLicenseDlg_LicenseAgreement##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>ISPrintButton</td><td>PushButton</td><td>301</td><td>188</td><td>65</td><td>17</td><td>3</td><td/><td>##IDS_PRINT_BUTTON##</td><td>Agree</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>LicenseAgreement</td><td>Memo</td><td>ScrollableText</td><td>8</td><td>55</td><td>358</td><td>130</td><td>7</td><td/><td/><td/><td/><td>0</td><td/><td>C:\Program Files\InstallShield\2011LE\Support\0409\EulaEval.rtf</td><td/></row>
		<row><td>LicenseAgreement</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>MaintenanceType</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>RadioGroup</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsMaintenanceDlg_MaitenanceOptions##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsMaintenanceDlg_ProgramMaintenance##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Ico1</td><td>Icon</td><td>35</td><td>75</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary6</td></row>
		<row><td>MaintenanceType</td><td>Ico2</td><td>Icon</td><td>35</td><td>135</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary7</td></row>
		<row><td>MaintenanceType</td><td>Ico3</td><td>Icon</td><td>35</td><td>195</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary8</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>RadioGroup</td><td>RadioButtonGroup</td><td>21</td><td>55</td><td>290</td><td>170</td><td>3</td><td>_IsMaintenance</td><td/><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Text1</td><td>Text</td><td>80</td><td>72</td><td>260</td><td>35</td><td>3</td><td/><td>##IDS__IsMaintenanceDlg_ChangeFeatures##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Text2</td><td>Text</td><td>80</td><td>135</td><td>260</td><td>35</td><td>3</td><td/><td>##IDS__IsMaintenanceDlg_RepairMessage##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceType</td><td>Text3</td><td>Text</td><td>80</td><td>192</td><td>260</td><td>35</td><td>131075</td><td/><td>##IDS__IsMaintenanceDlg_RemoveProductName##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceWelcome</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceWelcome</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceWelcome</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceWelcome</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>MaintenanceWelcome</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceWelcome</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>196611</td><td/><td>##IDS__IsMaintenanceWelcome_WizardWelcome##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MaintenanceWelcome</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>50</td><td>196611</td><td/><td>##IDS__IsMaintenanceWelcome_MaintenanceOptionsDescription##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>MsiRMFilesInUse</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Restart</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsFilesInUse_FilesInUseMessage##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>DlgText</td><td>Text</td><td>21</td><td>51</td><td>348</td><td>14</td><td>3</td><td/><td>##IDS__IsMsiRMFilesInUse_ApplicationsUsingFiles##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsFilesInUse_FilesInUse##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>List</td><td>ListBox</td><td>21</td><td>66</td><td>331</td><td>130</td><td>3</td><td>FileInUseProcess</td><td/><td>OK</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>OK</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_OK##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>MsiRMFilesInUse</td><td>Restart</td><td>RadioButtonGroup</td><td>19</td><td>187</td><td>343</td><td>40</td><td>3</td><td>RestartManagerOption</td><td/><td>List</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>OutOfSpace</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsDiskSpaceDlg_DiskSpace##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>DlgText</td><td>Text</td><td>21</td><td>51</td><td>326</td><td>43</td><td>3</td><td/><td>##IDS__IsDiskSpaceDlg_HighlightedVolumes##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsDiskSpaceDlg_OutOfDiskSpace##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>List</td><td>VolumeCostList</td><td>21</td><td>95</td><td>332</td><td>120</td><td>393223</td><td/><td>##IDS__IsDiskSpaceDlg_Numbers##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>OutOfSpace</td><td>Resume</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsDiskSpaceDlg_OK##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>PatchWelcome</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>PatchWelcome</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>PatchWelcome</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>PatchWelcome</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary21</td></row>
		<row><td>PatchWelcome</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsPatchDlg_Update##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>PatchWelcome</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>196611</td><td/><td>##IDS__IsPatchDlg_WelcomePatchWizard##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>PatchWelcome</td><td>TextLine2</td><td>Text</td><td>135</td><td>54</td><td>228</td><td>45</td><td>196611</td><td/><td>##IDS__IsPatchDlg_PatchClickUpdate##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadmeInformation</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1048579</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadmeInformation</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>3</td><td/><td/><td>DlgTitle</td><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>ReadmeInformation</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>ReadmeInformation</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>ReadmeInformation</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>1048579</td><td/><td>##IDS__IsReadmeDlg_Cancel##</td><td>Readme</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadmeInformation</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>232</td><td>16</td><td>65539</td><td/><td>##IDS__IsReadmeDlg_PleaseReadInfo##</td><td>Back</td><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>ReadmeInformation</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>3</td><td/><td/><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>ReadmeInformation</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>193</td><td>13</td><td>65539</td><td/><td>##IDS__IsReadmeDlg_ReadMeInfo##</td><td>DlgDesc</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadmeInformation</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>1048579</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadmeInformation</td><td>Readme</td><td>ScrollableText</td><td>10</td><td>55</td><td>353</td><td>166</td><td>3</td><td/><td/><td>Banner</td><td/><td>0</td><td/><td>&lt;ISProductFolder&gt;\Redist\0409\Readme.rtf</td><td/></row>
		<row><td>ReadyToInstall</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>GroupBox1</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>ReadyToInstall</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>CompanyNameText</td><td>Text</td><td>38</td><td>198</td><td>211</td><td>9</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_Company##</td><td>SerialNumberText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>CurrentSettingsText</td><td>Text</td><td>19</td><td>80</td><td>81</td><td>10</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_CurrentSettings##</td><td>InstallNow</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsVerifyReadyDlg_WizardReady##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgText1</td><td>Text</td><td>21</td><td>54</td><td>330</td><td>24</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_BackOrCancel##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgText2</td><td>Text</td><td>21</td><td>99</td><td>330</td><td>20</td><td>2</td><td/><td>##IDS__IsRegisterUserDlg_InstallFor##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65538</td><td/><td>##IDS__IsVerifyReadyDlg_ModifyReady##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgTitle2</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65538</td><td/><td>##IDS__IsVerifyReadyDlg_ReadyRepair##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>DlgTitle3</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65538</td><td/><td>##IDS__IsVerifyReadyDlg_ReadyInstall##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>GroupBox1</td><td>Text</td><td>19</td><td>92</td><td>330</td><td>133</td><td>65541</td><td/><td/><td>SetupTypeText1</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>8388611</td><td/><td>##IDS__IsVerifyReadyDlg_Install##</td><td>InstallPerMachine</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>PushButton</td><td>63</td><td>123</td><td>248</td><td>17</td><td>8388610</td><td/><td>##IDS__IsRegisterUserDlg_Anyone##</td><td>InstallPerUser</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>PushButton</td><td>63</td><td>143</td><td>248</td><td>17</td><td>2</td><td/><td>##IDS__IsRegisterUserDlg_OnlyMe##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>SerialNumberText</td><td>Text</td><td>38</td><td>211</td><td>306</td><td>9</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_Serial##</td><td>CurrentSettingsText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>SetupTypeText1</td><td>Text</td><td>23</td><td>97</td><td>306</td><td>13</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_SetupType##</td><td>SetupTypeText2</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>SetupTypeText2</td><td>Text</td><td>37</td><td>114</td><td>306</td><td>14</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_SelectedSetupType##</td><td>TargetFolderText1</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>TargetFolderText1</td><td>Text</td><td>24</td><td>136</td><td>306</td><td>11</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_DestFolder##</td><td>TargetFolderText2</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>TargetFolderText2</td><td>Text</td><td>37</td><td>151</td><td>306</td><td>13</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_Installdir##</td><td>UserInformationText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>UserInformationText</td><td>Text</td><td>23</td><td>171</td><td>306</td><td>13</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_UserInfo##</td><td>UserNameText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToInstall</td><td>UserNameText</td><td>Text</td><td>38</td><td>184</td><td>306</td><td>9</td><td>3</td><td/><td>##IDS__IsVerifyReadyDlg_UserName##</td><td>CompanyNameText</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>RemoveNow</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>ReadyToRemove</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsVerifyRemoveAllDlg_ChoseRemoveProgram##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>DlgText</td><td>Text</td><td>21</td><td>51</td><td>326</td><td>24</td><td>131075</td><td/><td>##IDS__IsVerifyRemoveAllDlg_ClickRemove##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>DlgText1</td><td>Text</td><td>21</td><td>79</td><td>330</td><td>23</td><td>3</td><td/><td>##IDS__IsVerifyRemoveAllDlg_ClickBack##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>DlgText2</td><td>Text</td><td>21</td><td>102</td><td>330</td><td>24</td><td>3</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsVerifyRemoveAllDlg_RemoveProgram##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>8388611</td><td/><td>##IDS__IsVerifyRemoveAllDlg_Remove##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Finish</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>CheckShowMsiLog</td><td>CheckBox</td><td>151</td><td>172</td><td>10</td><td>9</td><td>2</td><td>ISSHOWMSILOG</td><td/><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>Finish</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsFatalError_Finish##</td><td>Image</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>FinishText1</td><td>Text</td><td>135</td><td>80</td><td>228</td><td>50</td><td>65539</td><td/><td>##IDS__IsFatalError_NotModified##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>FinishText2</td><td>Text</td><td>135</td><td>135</td><td>228</td><td>25</td><td>65539</td><td/><td>##IDS__IsFatalError_ClickFinish##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td>CheckShowMsiLog</td><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>SetupCompleteError</td><td>RestContText1</td><td>Text</td><td>135</td><td>80</td><td>228</td><td>50</td><td>65539</td><td/><td>##IDS__IsFatalError_KeepOrRestore##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>RestContText2</td><td>Text</td><td>135</td><td>135</td><td>228</td><td>25</td><td>65539</td><td/><td>##IDS__IsFatalError_RestoreOrContinueLater##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>ShowMsiLogText</td><td>Text</td><td>164</td><td>172</td><td>198</td><td>10</td><td>65538</td><td/><td>##IDS__IsSetupComplete_ShowMsiLog##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>65539</td><td/><td>##IDS__IsFatalError_WizardCompleted##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteError</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>25</td><td>196611</td><td/><td>##IDS__IsFatalError_WizardInterrupted##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>OK</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_CANCEL##</td><td>Image</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>CheckBoxUpdates</td><td>CheckBox</td><td>135</td><td>164</td><td>10</td><td>9</td><td>2</td><td>ISCHECKFORPRODUCTUPDATES</td><td>CheckBox1</td><td>CheckShowMsiLog</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>CheckForUpdatesText</td><td>Text</td><td>152</td><td>162</td><td>190</td><td>30</td><td>65538</td><td/><td>##IDS__IsExitDialog_Update_YesCheckForUpdates##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>CheckLaunchProgram</td><td>CheckBox</td><td>151</td><td>114</td><td>10</td><td>9</td><td>2</td><td>LAUNCHPROGRAM</td><td/><td>CheckLaunchReadme</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>CheckLaunchReadme</td><td>CheckBox</td><td>151</td><td>148</td><td>10</td><td>9</td><td>2</td><td>LAUNCHREADME</td><td/><td>CheckBoxUpdates</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>CheckShowMsiLog</td><td>CheckBox</td><td>151</td><td>182</td><td>10</td><td>9</td><td>2</td><td>ISSHOWMSILOG</td><td/><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td>CheckLaunchProgram</td><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>SetupCompleteSuccess</td><td>LaunchProgramText</td><td>Text</td><td>164</td><td>112</td><td>98</td><td>15</td><td>65538</td><td/><td>##IDS__IsExitDialog_LaunchProgram##</td><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>LaunchReadmeText</td><td>Text</td><td>164</td><td>148</td><td>120</td><td>13</td><td>65538</td><td/><td>##IDS__IsExitDialog_ShowReadMe##</td><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>OK</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsExitDialog_Finish##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>ShowMsiLogText</td><td>Text</td><td>164</td><td>182</td><td>198</td><td>10</td><td>65538</td><td/><td>##IDS__IsSetupComplete_ShowMsiLog##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>65539</td><td/><td>##IDS__IsExitDialog_WizardCompleted##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>45</td><td>196610</td><td/><td>##IDS__IsExitDialog_InstallSuccess##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>TextLine3</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>45</td><td>196610</td><td/><td>##IDS__IsExitDialog_UninstallSuccess##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>UpdateTextLine1</td><td>Text</td><td>135</td><td>30</td><td>228</td><td>45</td><td>196610</td><td/><td>##IDS__IsExitDialog_Update_SetupFinished##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>UpdateTextLine2</td><td>Text</td><td>135</td><td>80</td><td>228</td><td>45</td><td>196610</td><td/><td>##IDS__IsExitDialog_Update_PossibleUpdates##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupCompleteSuccess</td><td>UpdateTextLine3</td><td>Text</td><td>135</td><td>120</td><td>228</td><td>45</td><td>65538</td><td/><td>##IDS__IsExitDialog_Update_InternetConnection##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>A</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsErrorDlg_Abort##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>C</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL2##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>ErrorIcon</td><td>Icon</td><td>15</td><td>15</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary4</td></row>
		<row><td>SetupError</td><td>ErrorText</td><td>Text</td><td>50</td><td>15</td><td>200</td><td>50</td><td>131075</td><td/><td>##IDS__IsErrorDlg_ErrorText##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>I</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsErrorDlg_Ignore##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>N</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsErrorDlg_NO##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>O</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsErrorDlg_OK##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>R</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsErrorDlg_Retry##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupError</td><td>Y</td><td>PushButton</td><td>192</td><td>80</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsErrorDlg_Yes##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>ActionData</td><td>Text</td><td>135</td><td>125</td><td>228</td><td>12</td><td>65539</td><td/><td>##IDS__IsInitDlg_1##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>ActionText</td><td>Text</td><td>135</td><td>109</td><td>220</td><td>36</td><td>65539</td><td/><td>##IDS__IsInitDlg_2##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>SetupInitialization</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_NEXT##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>196611</td><td/><td>##IDS__IsInitDlg_WelcomeWizard##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInitialization</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>30</td><td>196611</td><td/><td>##IDS__IsInitDlg_PreparingWizard##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Finish</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_CANCEL##</td><td>Image</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>CheckShowMsiLog</td><td>CheckBox</td><td>151</td><td>172</td><td>10</td><td>9</td><td>2</td><td>ISSHOWMSILOG</td><td/><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>Finish</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS__IsUserExit_Finish##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>FinishText1</td><td>Text</td><td>135</td><td>80</td><td>228</td><td>50</td><td>65539</td><td/><td>##IDS__IsUserExit_NotModified##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>FinishText2</td><td>Text</td><td>135</td><td>135</td><td>228</td><td>25</td><td>65539</td><td/><td>##IDS__IsUserExit_ClickFinish##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td>CheckShowMsiLog</td><td/><td>0</td><td/><td/><td>NewBinary22</td></row>
		<row><td>SetupInterrupted</td><td>RestContText1</td><td>Text</td><td>135</td><td>80</td><td>228</td><td>50</td><td>65539</td><td/><td>##IDS__IsUserExit_KeepOrRestore##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>RestContText2</td><td>Text</td><td>135</td><td>135</td><td>228</td><td>25</td><td>65539</td><td/><td>##IDS__IsUserExit_RestoreOrContinue##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>ShowMsiLogText</td><td>Text</td><td>164</td><td>172</td><td>198</td><td>10</td><td>65538</td><td/><td>##IDS__IsSetupComplete_ShowMsiLog##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>65539</td><td/><td>##IDS__IsUserExit_WizardCompleted##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupInterrupted</td><td>TextLine2</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>25</td><td>196611</td><td/><td>##IDS__IsUserExit_WizardInterrupted##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>ProgressBar</td><td>59</td><td>113</td><td>275</td><td>12</td><td>65537</td><td/><td>##IDS__IsProgressDlg_ProgressDone##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>ActionText</td><td>Text</td><td>59</td><td>100</td><td>275</td><td>12</td><td>3</td><td/><td>##IDS__IsProgressDlg_2##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>SetupProgress</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65538</td><td/><td>##IDS__IsProgressDlg_UninstallingFeatures2##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgDesc2</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65538</td><td/><td>##IDS__IsProgressDlg_UninstallingFeatures##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgText</td><td>Text</td><td>59</td><td>51</td><td>275</td><td>30</td><td>196610</td><td/><td>##IDS__IsProgressDlg_WaitUninstall2##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgText2</td><td>Text</td><td>59</td><td>51</td><td>275</td><td>30</td><td>196610</td><td/><td>##IDS__IsProgressDlg_WaitUninstall##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>196610</td><td/><td>##IDS__IsProgressDlg_InstallingProductName##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>DlgTitle2</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>196610</td><td/><td>##IDS__IsProgressDlg_Uninstalling##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>LbSec</td><td>Text</td><td>192</td><td>139</td><td>32</td><td>12</td><td>2</td><td/><td>##IDS__IsProgressDlg_SecHidden##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>LbStatus</td><td>Text</td><td>59</td><td>85</td><td>70</td><td>12</td><td>3</td><td/><td>##IDS__IsProgressDlg_Status##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>SetupIcon</td><td>Icon</td><td>21</td><td>51</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary9</td></row>
		<row><td>SetupProgress</td><td>ShowTime</td><td>Text</td><td>170</td><td>139</td><td>17</td><td>12</td><td>2</td><td/><td>##IDS__IsProgressDlg_Hidden##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupProgress</td><td>TextTime</td><td>Text</td><td>59</td><td>139</td><td>110</td><td>12</td><td>2</td><td/><td>##IDS__IsProgressDlg_HiddenTimeRemaining##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>DlgLine</td><td>Line</td><td>0</td><td>234</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>Image</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>234</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary20</td></row>
		<row><td>SetupResume</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>PreselectedText</td><td>Text</td><td>135</td><td>55</td><td>228</td><td>45</td><td>196611</td><td/><td>##IDS__IsResumeDlg_WizardResume##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>ResumeText</td><td>Text</td><td>135</td><td>46</td><td>228</td><td>45</td><td>196611</td><td/><td>##IDS__IsResumeDlg_ResumeSuspended##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupResume</td><td>TextLine1</td><td>Text</td><td>135</td><td>8</td><td>225</td><td>45</td><td>196611</td><td/><td>##IDS__IsResumeDlg_Resuming##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>Banner</td><td>Bitmap</td><td>0</td><td>0</td><td>374</td><td>44</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary1</td></row>
		<row><td>SetupType</td><td>BannerLine</td><td>Line</td><td>0</td><td>44</td><td>374</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>RadioGroup</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>CompText</td><td>Text</td><td>80</td><td>80</td><td>246</td><td>30</td><td>3</td><td/><td>##IDS__IsSetupTypeMinDlg_AllFeatures##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>CompleteIco</td><td>Icon</td><td>34</td><td>80</td><td>24</td><td>24</td><td>5242881</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary10</td></row>
		<row><td>SetupType</td><td>CustText</td><td>Text</td><td>80</td><td>171</td><td>246</td><td>30</td><td>2</td><td/><td>##IDS__IsSetupTypeMinDlg_ChooseFeatures##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>CustomIco</td><td>Icon</td><td>34</td><td>171</td><td>24</td><td>24</td><td>5242880</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary11</td></row>
		<row><td>SetupType</td><td>DlgDesc</td><td>Text</td><td>21</td><td>23</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsSetupTypeMinDlg_ChooseSetupType##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>DlgText</td><td>Text</td><td>22</td><td>49</td><td>326</td><td>10</td><td>3</td><td/><td>##IDS__IsSetupTypeMinDlg_SelectSetupType##</td><td/><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>SetupType</td><td>DlgTitle</td><td>Text</td><td>13</td><td>6</td><td>292</td><td>25</td><td>65539</td><td/><td>##IDS__IsSetupTypeMinDlg_SetupType##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>MinIco</td><td>Icon</td><td>34</td><td>125</td><td>24</td><td>24</td><td>5242880</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary11</td></row>
		<row><td>SetupType</td><td>MinText</td><td>Text</td><td>80</td><td>125</td><td>246</td><td>30</td><td>2</td><td/><td>##IDS__IsSetupTypeMinDlg_MinimumFeatures##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SetupType</td><td>RadioGroup</td><td>RadioButtonGroup</td><td>20</td><td>59</td><td>264</td><td>139</td><td>1048579</td><td>_IsSetupTypeMin</td><td/><td>Back</td><td/><td>0</td><td>0</td><td/><td/></row>
		<row><td>SplashBitmap</td><td>Back</td><td>PushButton</td><td>164</td><td>243</td><td>66</td><td>17</td><td>1</td><td/><td>##IDS_BACK##</td><td>Next</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SplashBitmap</td><td>Branding1</td><td>Text</td><td>4</td><td>229</td><td>50</td><td>13</td><td>3</td><td/><td>##IDS_INSTALLSHIELD_FORMATTED##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SplashBitmap</td><td>Branding2</td><td>Text</td><td>3</td><td>228</td><td>50</td><td>13</td><td>65537</td><td/><td>##IDS_INSTALLSHIELD##</td><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SplashBitmap</td><td>Cancel</td><td>PushButton</td><td>301</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_CANCEL##</td><td>Back</td><td/><td>0</td><td/><td/><td/></row>
		<row><td>SplashBitmap</td><td>DlgLine</td><td>Line</td><td>48</td><td>234</td><td>326</td><td>0</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td/></row>
		<row><td>SplashBitmap</td><td>Image</td><td>Bitmap</td><td>13</td><td>12</td><td>349</td><td>211</td><td>1</td><td/><td/><td/><td/><td>0</td><td/><td/><td>NewBinary5</td></row>
		<row><td>SplashBitmap</td><td>Next</td><td>PushButton</td><td>230</td><td>243</td><td>66</td><td>17</td><td>3</td><td/><td>##IDS_NEXT##</td><td>Cancel</td><td/><td>0</td><td/><td/><td/></row>
	</table>

	<table name="ControlCondition">
		<col key="yes" def="s72">Dialog_</col>
		<col key="yes" def="s50">Control_</col>
		<col key="yes" def="s50">Action</col>
		<col key="yes" def="s255">Condition</col>
		<row><td>CustomSetup</td><td>ChangeFolder</td><td>Hide</td><td>Installed</td></row>
		<row><td>CustomSetup</td><td>Details</td><td>Hide</td><td>Installed</td></row>
		<row><td>CustomSetup</td><td>InstallLabel</td><td>Hide</td><td>Installed</td></row>
		<row><td>CustomerInformation</td><td>DlgRadioGroupText</td><td>Hide</td><td>NOT Privileged</td></row>
		<row><td>CustomerInformation</td><td>DlgRadioGroupText</td><td>Hide</td><td>ProductState &gt; 0</td></row>
		<row><td>CustomerInformation</td><td>DlgRadioGroupText</td><td>Hide</td><td>Version9X</td></row>
		<row><td>CustomerInformation</td><td>DlgRadioGroupText</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>CustomerInformation</td><td>RadioGroup</td><td>Hide</td><td>NOT Privileged</td></row>
		<row><td>CustomerInformation</td><td>RadioGroup</td><td>Hide</td><td>ProductState &gt; 0</td></row>
		<row><td>CustomerInformation</td><td>RadioGroup</td><td>Hide</td><td>Version9X</td></row>
		<row><td>CustomerInformation</td><td>RadioGroup</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>CustomerInformation</td><td>SerialLabel</td><td>Show</td><td>SERIALNUMSHOW</td></row>
		<row><td>CustomerInformation</td><td>SerialNumber</td><td>Show</td><td>SERIALNUMSHOW</td></row>
		<row><td>InstallWelcome</td><td>Copyright</td><td>Hide</td><td>SHOWCOPYRIGHT="No"</td></row>
		<row><td>InstallWelcome</td><td>Copyright</td><td>Show</td><td>SHOWCOPYRIGHT="Yes"</td></row>
		<row><td>LicenseAgreement</td><td>Next</td><td>Disable</td><td>AgreeToLicense &lt;&gt; "Yes"</td></row>
		<row><td>LicenseAgreement</td><td>Next</td><td>Enable</td><td>AgreeToLicense = "Yes"</td></row>
		<row><td>ReadyToInstall</td><td>CompanyNameText</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>CurrentSettingsText</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>DlgText2</td><td>Hide</td><td>VersionNT &lt; "601" OR NOT ISSupportPerUser OR Installed</td></row>
		<row><td>ReadyToInstall</td><td>DlgText2</td><td>Show</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>DlgTitle</td><td>Show</td><td>ProgressType0="Modify"</td></row>
		<row><td>ReadyToInstall</td><td>DlgTitle2</td><td>Show</td><td>ProgressType0="Repair"</td></row>
		<row><td>ReadyToInstall</td><td>DlgTitle3</td><td>Show</td><td>ProgressType0="install"</td></row>
		<row><td>ReadyToInstall</td><td>GroupBox1</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>Disable</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>Enable</td><td>VersionNT &lt; "601" OR NOT ISSupportPerUser OR Installed</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>Hide</td><td>VersionNT &lt; "601" OR NOT ISSupportPerUser OR Installed</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>Show</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>Hide</td><td>VersionNT &lt; "601" OR NOT ISSupportPerUser OR Installed</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>Show</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>SerialNumberText</td><td>Hide</td><td>NOT SERIALNUMSHOW</td></row>
		<row><td>ReadyToInstall</td><td>SerialNumberText</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>SetupTypeText1</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>SetupTypeText2</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>TargetFolderText1</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>TargetFolderText2</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>UserInformationText</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>ReadyToInstall</td><td>UserNameText</td><td>Hide</td><td>VersionNT &gt;= "601" AND ISSupportPerUser AND NOT Installed</td></row>
		<row><td>SetupCompleteError</td><td>Back</td><td>Default</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>Back</td><td>Disable</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>Back</td><td>Enable</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>Cancel</td><td>Disable</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>Cancel</td><td>Enable</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>CheckShowMsiLog</td><td>Show</td><td>MsiLogFileLocation</td></row>
		<row><td>SetupCompleteError</td><td>Finish</td><td>Default</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>FinishText1</td><td>Hide</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>FinishText1</td><td>Show</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>FinishText2</td><td>Hide</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>FinishText2</td><td>Show</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>RestContText1</td><td>Hide</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>RestContText1</td><td>Show</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>RestContText2</td><td>Hide</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>RestContText2</td><td>Show</td><td>UpdateStarted</td></row>
		<row><td>SetupCompleteError</td><td>ShowMsiLogText</td><td>Show</td><td>MsiLogFileLocation</td></row>
		<row><td>SetupCompleteSuccess</td><td>CheckBoxUpdates</td><td>Show</td><td>ISENABLEDWUSFINISHDIALOG And NOT Installed And ACTION="INSTALL"</td></row>
		<row><td>SetupCompleteSuccess</td><td>CheckForUpdatesText</td><td>Show</td><td>ISENABLEDWUSFINISHDIALOG And NOT Installed And ACTION="INSTALL"</td></row>
		<row><td>SetupCompleteSuccess</td><td>CheckLaunchProgram</td><td>Show</td><td>SHOWLAUNCHPROGRAM="-1" And PROGRAMFILETOLAUNCHATEND &lt;&gt; "" And NOT Installed And NOT ISENABLEDWUSFINISHDIALOG</td></row>
		<row><td>SetupCompleteSuccess</td><td>CheckLaunchReadme</td><td>Show</td><td>SHOWLAUNCHREADME="-1"  And READMEFILETOLAUNCHATEND &lt;&gt; "" And NOT Installed And NOT ISENABLEDWUSFINISHDIALOG</td></row>
		<row><td>SetupCompleteSuccess</td><td>CheckShowMsiLog</td><td>Show</td><td>MsiLogFileLocation And NOT ISENABLEDWUSFINISHDIALOG</td></row>
		<row><td>SetupCompleteSuccess</td><td>LaunchProgramText</td><td>Show</td><td>SHOWLAUNCHPROGRAM="-1" And PROGRAMFILETOLAUNCHATEND &lt;&gt; "" And NOT Installed And NOT ISENABLEDWUSFINISHDIALOG</td></row>
		<row><td>SetupCompleteSuccess</td><td>LaunchReadmeText</td><td>Show</td><td>SHOWLAUNCHREADME="-1"  And READMEFILETOLAUNCHATEND &lt;&gt; "" And NOT Installed And NOT ISENABLEDWUSFINISHDIALOG</td></row>
		<row><td>SetupCompleteSuccess</td><td>ShowMsiLogText</td><td>Show</td><td>MsiLogFileLocation And NOT ISENABLEDWUSFINISHDIALOG</td></row>
		<row><td>SetupCompleteSuccess</td><td>TextLine2</td><td>Show</td><td>ProgressType2="installed" And ((ACTION&lt;&gt;"INSTALL") OR (NOT ISENABLEDWUSFINISHDIALOG) OR (ISENABLEDWUSFINISHDIALOG And Installed))</td></row>
		<row><td>SetupCompleteSuccess</td><td>TextLine3</td><td>Show</td><td>ProgressType2="uninstalled" And ((ACTION&lt;&gt;"INSTALL") OR (NOT ISENABLEDWUSFINISHDIALOG) OR (ISENABLEDWUSFINISHDIALOG And Installed))</td></row>
		<row><td>SetupCompleteSuccess</td><td>UpdateTextLine1</td><td>Show</td><td>ISENABLEDWUSFINISHDIALOG And NOT Installed And ACTION="INSTALL"</td></row>
		<row><td>SetupCompleteSuccess</td><td>UpdateTextLine2</td><td>Show</td><td>ISENABLEDWUSFINISHDIALOG And NOT Installed And ACTION="INSTALL"</td></row>
		<row><td>SetupCompleteSuccess</td><td>UpdateTextLine3</td><td>Show</td><td>ISENABLEDWUSFINISHDIALOG And NOT Installed And ACTION="INSTALL"</td></row>
		<row><td>SetupInterrupted</td><td>Back</td><td>Default</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>Back</td><td>Disable</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>Back</td><td>Enable</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>Cancel</td><td>Disable</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>Cancel</td><td>Enable</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>CheckShowMsiLog</td><td>Show</td><td>MsiLogFileLocation</td></row>
		<row><td>SetupInterrupted</td><td>Finish</td><td>Default</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>FinishText1</td><td>Hide</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>FinishText1</td><td>Show</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>FinishText2</td><td>Hide</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>FinishText2</td><td>Show</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>RestContText1</td><td>Hide</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>RestContText1</td><td>Show</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>RestContText2</td><td>Hide</td><td>NOT UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>RestContText2</td><td>Show</td><td>UpdateStarted</td></row>
		<row><td>SetupInterrupted</td><td>ShowMsiLogText</td><td>Show</td><td>MsiLogFileLocation</td></row>
		<row><td>SetupProgress</td><td>DlgDesc</td><td>Show</td><td>ProgressType2="installed"</td></row>
		<row><td>SetupProgress</td><td>DlgDesc2</td><td>Show</td><td>ProgressType2="uninstalled"</td></row>
		<row><td>SetupProgress</td><td>DlgText</td><td>Show</td><td>ProgressType3="installs"</td></row>
		<row><td>SetupProgress</td><td>DlgText2</td><td>Show</td><td>ProgressType3="uninstalls"</td></row>
		<row><td>SetupProgress</td><td>DlgTitle</td><td>Show</td><td>ProgressType1="Installing"</td></row>
		<row><td>SetupProgress</td><td>DlgTitle2</td><td>Show</td><td>ProgressType1="Uninstalling"</td></row>
		<row><td>SetupResume</td><td>PreselectedText</td><td>Hide</td><td>RESUME</td></row>
		<row><td>SetupResume</td><td>PreselectedText</td><td>Show</td><td>NOT RESUME</td></row>
		<row><td>SetupResume</td><td>ResumeText</td><td>Hide</td><td>NOT RESUME</td></row>
		<row><td>SetupResume</td><td>ResumeText</td><td>Show</td><td>RESUME</td></row>
	</table>

	<table name="ControlEvent">
		<col key="yes" def="s72">Dialog_</col>
		<col key="yes" def="s50">Control_</col>
		<col key="yes" def="s50">Event</col>
		<col key="yes" def="s255">Argument</col>
		<col key="yes" def="S255">Condition</col>
		<col def="I2">Ordering</col>
		<row><td>AdminChangeFolder</td><td>Cancel</td><td>EndDialog</td><td>Return</td><td>1</td><td>2</td></row>
		<row><td>AdminChangeFolder</td><td>Cancel</td><td>Reset</td><td>0</td><td>1</td><td>1</td></row>
		<row><td>AdminChangeFolder</td><td>NewFolder</td><td>DirectoryListNew</td><td>0</td><td>1</td><td>0</td></row>
		<row><td>AdminChangeFolder</td><td>OK</td><td>EndDialog</td><td>Return</td><td>1</td><td>0</td></row>
		<row><td>AdminChangeFolder</td><td>OK</td><td>SetTargetPath</td><td>TARGETDIR</td><td>1</td><td>1</td></row>
		<row><td>AdminChangeFolder</td><td>Up</td><td>DirectoryListUp</td><td>0</td><td>1</td><td>0</td></row>
		<row><td>AdminNetworkLocation</td><td>Back</td><td>NewDialog</td><td>AdminWelcome</td><td>1</td><td>0</td></row>
		<row><td>AdminNetworkLocation</td><td>Browse</td><td>SpawnDialog</td><td>AdminChangeFolder</td><td>1</td><td>0</td></row>
		<row><td>AdminNetworkLocation</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>AdminNetworkLocation</td><td>InstallNow</td><td>EndDialog</td><td>Return</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>3</td></row>
		<row><td>AdminNetworkLocation</td><td>InstallNow</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>2</td></row>
		<row><td>AdminNetworkLocation</td><td>InstallNow</td><td>SetTargetPath</td><td>TARGETDIR</td><td>1</td><td>1</td></row>
		<row><td>AdminWelcome</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>AdminWelcome</td><td>Next</td><td>NewDialog</td><td>AdminNetworkLocation</td><td>1</td><td>0</td></row>
		<row><td>CancelSetup</td><td>No</td><td>EndDialog</td><td>Return</td><td>1</td><td>0</td></row>
		<row><td>CancelSetup</td><td>Yes</td><td>DoAction</td><td>CleanUp</td><td>ISSCRIPTRUNNING="1"</td><td>1</td></row>
		<row><td>CancelSetup</td><td>Yes</td><td>EndDialog</td><td>Exit</td><td>1</td><td>2</td></row>
		<row><td>CustomSetup</td><td>Back</td><td>NewDialog</td><td>MaintenanceType</td><td>Installed</td><td>0</td></row>
		<row><td>CustomSetup</td><td>Back</td><td>NewDialog</td><td>SetupType</td><td>NOT Installed</td><td>0</td></row>
		<row><td>CustomSetup</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>CustomSetup</td><td>ChangeFolder</td><td>SelectionBrowse</td><td>InstallChangeFolder</td><td>1</td><td>0</td></row>
		<row><td>CustomSetup</td><td>Details</td><td>SelectionBrowse</td><td>DiskSpaceRequirements</td><td>1</td><td>1</td></row>
		<row><td>CustomSetup</td><td>Help</td><td>SpawnDialog</td><td>CustomSetupTips</td><td>1</td><td>1</td></row>
		<row><td>CustomSetup</td><td>Next</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>0</td></row>
		<row><td>CustomSetup</td><td>Next</td><td>NewDialog</td><td>ReadyToInstall</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>0</td></row>
		<row><td>CustomSetup</td><td>Next</td><td>[_IsSetupTypeMin]</td><td>Custom</td><td>1</td><td>0</td></row>
		<row><td>CustomSetupTips</td><td>OK</td><td>EndDialog</td><td>Return</td><td>1</td><td>1</td></row>
		<row><td>CustomerInformation</td><td>Back</td><td>NewDialog</td><td>LicenseAgreement</td><td>1</td><td>1</td></row>
		<row><td>CustomerInformation</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>CustomerInformation</td><td>Next</td><td>EndDialog</td><td>Exit</td><td>(SERIALNUMVALRETRYLIMIT) And (SERIALNUMVALRETRYLIMIT&lt;0) And (SERIALNUMVALRETURN&lt;&gt;SERIALNUMVALSUCCESSRETVAL)</td><td>2</td></row>
		<row><td>CustomerInformation</td><td>Next</td><td>NewDialog</td><td>ReadyToInstall</td><td>(Not SERIALNUMVALRETURN) OR (SERIALNUMVALRETURN=SERIALNUMVALSUCCESSRETVAL)</td><td>3</td></row>
		<row><td>CustomerInformation</td><td>Next</td><td>[ALLUSERS]</td><td>1</td><td>ApplicationUsers = "AllUsers" And Privileged</td><td>1</td></row>
		<row><td>CustomerInformation</td><td>Next</td><td>[ALLUSERS]</td><td>{}</td><td>ApplicationUsers = "OnlyCurrentUser" And Privileged</td><td>2</td></row>
		<row><td>DatabaseFolder</td><td>Back</td><td>NewDialog</td><td>CustomerInformation</td><td>1</td><td>1</td></row>
		<row><td>DatabaseFolder</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>1</td></row>
		<row><td>DatabaseFolder</td><td>ChangeFolder</td><td>SpawnDialog</td><td>InstallChangeFolder</td><td>1</td><td>1</td></row>
		<row><td>DatabaseFolder</td><td>ChangeFolder</td><td>[_BrowseProperty]</td><td>DATABASEDIR</td><td>1</td><td>2</td></row>
		<row><td>DatabaseFolder</td><td>Next</td><td>NewDialog</td><td>SetupType</td><td>1</td><td>1</td></row>
		<row><td>DestinationFolder</td><td>Back</td><td>NewDialog</td><td>InstallWelcome</td><td>NOT Installed</td><td>0</td></row>
		<row><td>DestinationFolder</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>1</td></row>
		<row><td>DestinationFolder</td><td>ChangeFolder</td><td>SpawnDialog</td><td>InstallChangeFolder</td><td>1</td><td>1</td></row>
		<row><td>DestinationFolder</td><td>ChangeFolder</td><td>[_BrowseProperty]</td><td>INSTALLDIR</td><td>1</td><td>2</td></row>
		<row><td>DestinationFolder</td><td>Next</td><td>NewDialog</td><td>ReadyToInstall</td><td>1</td><td>0</td></row>
		<row><td>DiskSpaceRequirements</td><td>OK</td><td>EndDialog</td><td>Return</td><td>1</td><td>0</td></row>
		<row><td>FilesInUse</td><td>Exit</td><td>EndDialog</td><td>Exit</td><td>1</td><td>0</td></row>
		<row><td>FilesInUse</td><td>Ignore</td><td>EndDialog</td><td>Ignore</td><td>1</td><td>0</td></row>
		<row><td>FilesInUse</td><td>Retry</td><td>EndDialog</td><td>Retry</td><td>1</td><td>0</td></row>
		<row><td>InstallChangeFolder</td><td>Cancel</td><td>EndDialog</td><td>Return</td><td>1</td><td>2</td></row>
		<row><td>InstallChangeFolder</td><td>Cancel</td><td>Reset</td><td>0</td><td>1</td><td>1</td></row>
		<row><td>InstallChangeFolder</td><td>NewFolder</td><td>DirectoryListNew</td><td>0</td><td>1</td><td>0</td></row>
		<row><td>InstallChangeFolder</td><td>OK</td><td>EndDialog</td><td>Return</td><td>1</td><td>3</td></row>
		<row><td>InstallChangeFolder</td><td>OK</td><td>SetTargetPath</td><td>[_BrowseProperty]</td><td>1</td><td>2</td></row>
		<row><td>InstallChangeFolder</td><td>Up</td><td>DirectoryListUp</td><td>0</td><td>1</td><td>0</td></row>
		<row><td>InstallWelcome</td><td>Back</td><td>NewDialog</td><td>SplashBitmap</td><td>Display_IsBitmapDlg</td><td>0</td></row>
		<row><td>InstallWelcome</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>InstallWelcome</td><td>Next</td><td>NewDialog</td><td>DestinationFolder</td><td>1</td><td>0</td></row>
		<row><td>LicenseAgreement</td><td>Back</td><td>NewDialog</td><td>InstallWelcome</td><td>1</td><td>0</td></row>
		<row><td>LicenseAgreement</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>LicenseAgreement</td><td>ISPrintButton</td><td>DoAction</td><td>ISPrint</td><td>1</td><td>0</td></row>
		<row><td>LicenseAgreement</td><td>Next</td><td>NewDialog</td><td>DestinationFolder</td><td>AgreeToLicense = "Yes"</td><td>0</td></row>
		<row><td>MaintenanceType</td><td>Back</td><td>NewDialog</td><td>MaintenanceWelcome</td><td>1</td><td>0</td></row>
		<row><td>MaintenanceType</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>NewDialog</td><td>CustomSetup</td><td>_IsMaintenance = "Change"</td><td>12</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>NewDialog</td><td>ReadyToInstall</td><td>_IsMaintenance = "Reinstall"</td><td>13</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>NewDialog</td><td>ReadyToRemove</td><td>_IsMaintenance = "Remove"</td><td>11</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>Reinstall</td><td>ALL</td><td>_IsMaintenance = "Reinstall"</td><td>10</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>ReinstallMode</td><td>[ReinstallModeText]</td><td>_IsMaintenance = "Reinstall"</td><td>9</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType0]</td><td>Modify</td><td>_IsMaintenance = "Change"</td><td>2</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType0]</td><td>Repair</td><td>_IsMaintenance = "Reinstall"</td><td>1</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType1]</td><td>Modifying</td><td>_IsMaintenance = "Change"</td><td>3</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType1]</td><td>Repairing</td><td>_IsMaintenance = "Reinstall"</td><td>4</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType2]</td><td>modified</td><td>_IsMaintenance = "Change"</td><td>6</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType2]</td><td>repairs</td><td>_IsMaintenance = "Reinstall"</td><td>5</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType3]</td><td>modifies</td><td>_IsMaintenance = "Change"</td><td>7</td></row>
		<row><td>MaintenanceType</td><td>Next</td><td>[ProgressType3]</td><td>repairs</td><td>_IsMaintenance = "Reinstall"</td><td>8</td></row>
		<row><td>MaintenanceWelcome</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>MaintenanceWelcome</td><td>Next</td><td>NewDialog</td><td>MaintenanceType</td><td>1</td><td>0</td></row>
		<row><td>MsiRMFilesInUse</td><td>Cancel</td><td>EndDialog</td><td>Exit</td><td>1</td><td>1</td></row>
		<row><td>MsiRMFilesInUse</td><td>OK</td><td>EndDialog</td><td>Return</td><td>1</td><td>1</td></row>
		<row><td>MsiRMFilesInUse</td><td>OK</td><td>RMShutdownAndRestart</td><td>0</td><td>RestartManagerOption="CloseRestart"</td><td>2</td></row>
		<row><td>OutOfSpace</td><td>Resume</td><td>NewDialog</td><td>AdminNetworkLocation</td><td>ACTION = "ADMIN"</td><td>0</td></row>
		<row><td>OutOfSpace</td><td>Resume</td><td>NewDialog</td><td>DestinationFolder</td><td>ACTION &lt;&gt; "ADMIN"</td><td>0</td></row>
		<row><td>PatchWelcome</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>1</td></row>
		<row><td>PatchWelcome</td><td>Next</td><td>EndDialog</td><td>Return</td><td>1</td><td>3</td></row>
		<row><td>PatchWelcome</td><td>Next</td><td>Reinstall</td><td>ALL</td><td>PATCH And REINSTALL=""</td><td>1</td></row>
		<row><td>PatchWelcome</td><td>Next</td><td>ReinstallMode</td><td>omus</td><td>PATCH And REINSTALLMODE=""</td><td>2</td></row>
		<row><td>ReadmeInformation</td><td>Back</td><td>NewDialog</td><td>LicenseAgreement</td><td>1</td><td>1</td></row>
		<row><td>ReadmeInformation</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>1</td></row>
		<row><td>ReadmeInformation</td><td>Next</td><td>NewDialog</td><td>CustomerInformation</td><td>1</td><td>1</td></row>
		<row><td>ReadyToInstall</td><td>Back</td><td>NewDialog</td><td>CustomSetup</td><td>Installed OR _IsSetupTypeMin = "Custom"</td><td>2</td></row>
		<row><td>ReadyToInstall</td><td>Back</td><td>NewDialog</td><td>DestinationFolder</td><td>NOT Installed</td><td>1</td></row>
		<row><td>ReadyToInstall</td><td>Back</td><td>NewDialog</td><td>MaintenanceType</td><td>Installed AND _IsMaintenance = "Reinstall"</td><td>3</td></row>
		<row><td>ReadyToInstall</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>EndDialog</td><td>Return</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>[ProgressType1]</td><td>Installing</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>[ProgressType2]</td><td>installed</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallNow</td><td>[ProgressType3]</td><td>installs</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>EndDialog</td><td>Return</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>[ALLUSERS]</td><td>1</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>[MSIINSTALLPERUSER]</td><td>{}</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>[ProgressType1]</td><td>Installing</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>[ProgressType2]</td><td>installed</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerMachine</td><td>[ProgressType3]</td><td>installs</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>EndDialog</td><td>Return</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>[ALLUSERS]</td><td>2</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>[MSIINSTALLPERUSER]</td><td>1</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>[ProgressType1]</td><td>Installing</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>[ProgressType2]</td><td>installed</td><td>1</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>InstallPerUser</td><td>[ProgressType3]</td><td>installs</td><td>1</td><td>0</td></row>
		<row><td>ReadyToRemove</td><td>Back</td><td>NewDialog</td><td>MaintenanceType</td><td>1</td><td>0</td></row>
		<row><td>ReadyToRemove</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>EndDialog</td><td>Return</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>2</td></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>2</td></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>Remove</td><td>ALL</td><td>1</td><td>1</td></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>[ProgressType1]</td><td>Uninstalling</td><td>1</td><td>0</td></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>[ProgressType2]</td><td>uninstalled</td><td>1</td><td>0</td></row>
		<row><td>ReadyToRemove</td><td>RemoveNow</td><td>[ProgressType3]</td><td>uninstalls</td><td>1</td><td>0</td></row>
		<row><td>SetupCompleteError</td><td>Back</td><td>EndDialog</td><td>Return</td><td>1</td><td>2</td></row>
		<row><td>SetupCompleteError</td><td>Back</td><td>[Suspend]</td><td>{}</td><td>1</td><td>1</td></row>
		<row><td>SetupCompleteError</td><td>Cancel</td><td>EndDialog</td><td>Return</td><td>1</td><td>2</td></row>
		<row><td>SetupCompleteError</td><td>Cancel</td><td>[Suspend]</td><td>1</td><td>1</td><td>1</td></row>
		<row><td>SetupCompleteError</td><td>Finish</td><td>DoAction</td><td>CleanUp</td><td>ISSCRIPTRUNNING="1"</td><td>1</td></row>
		<row><td>SetupCompleteError</td><td>Finish</td><td>DoAction</td><td>ShowMsiLog</td><td>MsiLogFileLocation And (ISSHOWMSILOG="1")</td><td>3</td></row>
		<row><td>SetupCompleteError</td><td>Finish</td><td>EndDialog</td><td>Exit</td><td>1</td><td>2</td></row>
		<row><td>SetupCompleteSuccess</td><td>OK</td><td>DoAction</td><td>CleanUp</td><td>ISSCRIPTRUNNING="1"</td><td>1</td></row>
		<row><td>SetupCompleteSuccess</td><td>OK</td><td>DoAction</td><td>LaunchProgramFileFromSetupCompleteSuccess</td><td>LAUNCHPROGRAM And PROGRAMFILETOLAUNCHATEND &lt;&gt; "" And NOT Installed</td><td>2</td></row>
		<row><td>SetupCompleteSuccess</td><td>OK</td><td>DoAction</td><td>ShowMsiLog</td><td>MsiLogFileLocation And (ISSHOWMSILOG="1") And NOT ISENABLEDWUSFINISHDIALOG</td><td>7</td></row>
		<row><td>SetupCompleteSuccess</td><td>OK</td><td>EndDialog</td><td>Exit</td><td>1</td><td>3</td></row>
		<row><td>SetupError</td><td>A</td><td>EndDialog</td><td>ErrorAbort</td><td>1</td><td>0</td></row>
		<row><td>SetupError</td><td>C</td><td>EndDialog</td><td>ErrorCancel</td><td>1</td><td>0</td></row>
		<row><td>SetupError</td><td>I</td><td>EndDialog</td><td>ErrorIgnore</td><td>1</td><td>0</td></row>
		<row><td>SetupError</td><td>N</td><td>EndDialog</td><td>ErrorNo</td><td>1</td><td>0</td></row>
		<row><td>SetupError</td><td>O</td><td>EndDialog</td><td>ErrorOk</td><td>1</td><td>0</td></row>
		<row><td>SetupError</td><td>R</td><td>EndDialog</td><td>ErrorRetry</td><td>1</td><td>0</td></row>
		<row><td>SetupError</td><td>Y</td><td>EndDialog</td><td>ErrorYes</td><td>1</td><td>0</td></row>
		<row><td>SetupInitialization</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>SetupInterrupted</td><td>Back</td><td>EndDialog</td><td>Exit</td><td>1</td><td>2</td></row>
		<row><td>SetupInterrupted</td><td>Back</td><td>[Suspend]</td><td>{}</td><td>1</td><td>1</td></row>
		<row><td>SetupInterrupted</td><td>Cancel</td><td>EndDialog</td><td>Exit</td><td>1</td><td>2</td></row>
		<row><td>SetupInterrupted</td><td>Cancel</td><td>[Suspend]</td><td>1</td><td>1</td><td>1</td></row>
		<row><td>SetupInterrupted</td><td>Finish</td><td>DoAction</td><td>CleanUp</td><td>ISSCRIPTRUNNING="1"</td><td>1</td></row>
		<row><td>SetupInterrupted</td><td>Finish</td><td>DoAction</td><td>ShowMsiLog</td><td>MsiLogFileLocation And (ISSHOWMSILOG="1")</td><td>3</td></row>
		<row><td>SetupInterrupted</td><td>Finish</td><td>EndDialog</td><td>Exit</td><td>1</td><td>2</td></row>
		<row><td>SetupProgress</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>SetupResume</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>SetupResume</td><td>Next</td><td>EndDialog</td><td>Return</td><td>OutOfNoRbDiskSpace &lt;&gt; 1</td><td>0</td></row>
		<row><td>SetupResume</td><td>Next</td><td>NewDialog</td><td>OutOfSpace</td><td>OutOfNoRbDiskSpace = 1</td><td>0</td></row>
		<row><td>SetupType</td><td>Back</td><td>NewDialog</td><td>CustomerInformation</td><td>1</td><td>1</td></row>
		<row><td>SetupType</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>NewDialog</td><td>CustomSetup</td><td>_IsSetupTypeMin = "Custom"</td><td>2</td></row>
		<row><td>SetupType</td><td>Next</td><td>NewDialog</td><td>ReadyToInstall</td><td>_IsSetupTypeMin &lt;&gt; "Custom"</td><td>1</td></row>
		<row><td>SetupType</td><td>Next</td><td>SetInstallLevel</td><td>100</td><td>_IsSetupTypeMin="Minimal"</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>SetInstallLevel</td><td>200</td><td>_IsSetupTypeMin="Typical"</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>SetInstallLevel</td><td>300</td><td>_IsSetupTypeMin="Custom"</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>[ISRUNSETUPTYPEADDLOCALEVENT]</td><td>1</td><td>1</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>[SelectedSetupType]</td><td>[DisplayNameCustom]</td><td>_IsSetupTypeMin = "Custom"</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>[SelectedSetupType]</td><td>[DisplayNameMinimal]</td><td>_IsSetupTypeMin = "Minimal"</td><td>0</td></row>
		<row><td>SetupType</td><td>Next</td><td>[SelectedSetupType]</td><td>[DisplayNameTypical]</td><td>_IsSetupTypeMin = "Typical"</td><td>0</td></row>
		<row><td>SplashBitmap</td><td>Cancel</td><td>SpawnDialog</td><td>CancelSetup</td><td>1</td><td>0</td></row>
		<row><td>SplashBitmap</td><td>Next</td><td>NewDialog</td><td>InstallWelcome</td><td>1</td><td>0</td></row>
	</table>

	<table name="CreateFolder">
		<col key="yes" def="s72">Directory_</col>
		<col key="yes" def="s72">Component_</col>
		<row><td>INSTALLDIR</td><td>NewComponent1</td></row>
		<row><td>INSTALLDIR</td><td>NewComponent2</td></row>
	</table>

	<table name="CustomAction">
		<col key="yes" def="s72">Action</col>
		<col def="i2">Type</col>
		<col def="S64">Source</col>
		<col def="S0">Target</col>
		<col def="I4">ExtendedType</col>
		<col def="S255">ISComments</col>
		<row><td>ISPreventDowngrade</td><td>19</td><td/><td>[IS_PREVENT_DOWNGRADE_EXIT]</td><td/><td>Exits install when a newer version of this product is found</td></row>
		<row><td>ISPrint</td><td>1</td><td>SetAllUsers.dll</td><td>PrintScrollableText</td><td/><td>Prints the contents of a ScrollableText control on a dialog.</td></row>
		<row><td>ISRunSetupTypeAddLocalEvent</td><td>1</td><td>ISExpHlp.dll</td><td>RunSetupTypeAddLocalEvent</td><td/><td>Run the AddLocal events associated with the Next button on the Setup Type dialog.</td></row>
		<row><td>ISSelfRegisterCosting</td><td>1</td><td>ISSELFREG.DLL</td><td>ISSelfRegisterCosting</td><td/><td/></row>
		<row><td>ISSelfRegisterFiles</td><td>3073</td><td>ISSELFREG.DLL</td><td>ISSelfRegisterFiles</td><td/><td/></row>
		<row><td>ISSelfRegisterFinalize</td><td>1</td><td>ISSELFREG.DLL</td><td>ISSelfRegisterFinalize</td><td/><td/></row>
		<row><td>ISUnSelfRegisterFiles</td><td>3073</td><td>ISSELFREG.DLL</td><td>ISUnSelfRegisterFiles</td><td/><td/></row>
		<row><td>LaunchProgramFileFromSetupCompleteSuccess</td><td>1</td><td>NewBinary19</td><td>LaunchProgram</td><td/><td/></row>
		<row><td>SetARPINSTALLLOCATION</td><td>51</td><td>ARPINSTALLLOCATION</td><td>[INSTALLDIR]</td><td/><td/></row>
		<row><td>SetAllUsersProfileNT</td><td>51</td><td>ALLUSERSPROFILE</td><td>[%SystemRoot]\Profiles\All Users</td><td/><td/></row>
		<row><td>ShowMsiLog</td><td>226</td><td>SystemFolder</td><td>[SystemFolder]notepad.exe "[MsiLogFileLocation]"</td><td/><td>Shows Property-driven MSI Log</td></row>
		<row><td>setAllUsersProfile2K</td><td>51</td><td>ALLUSERSPROFILE</td><td>[%ALLUSERSPROFILE]</td><td/><td/></row>
		<row><td>setUserProfileNT</td><td>51</td><td>USERPROFILE</td><td>[%USERPROFILE]</td><td/><td/></row>
	</table>

	<table name="Dialog">
		<col key="yes" def="s72">Dialog</col>
		<col def="i2">HCentering</col>
		<col def="i2">VCentering</col>
		<col def="i2">Width</col>
		<col def="i2">Height</col>
		<col def="I4">Attributes</col>
		<col def="L128">Title</col>
		<col def="s50">Control_First</col>
		<col def="S50">Control_Default</col>
		<col def="S50">Control_Cancel</col>
		<col def="S255">ISComments</col>
		<col def="S72">TextStyle_</col>
		<col def="I4">ISWindowStyle</col>
		<col def="I4">ISResourceId</col>
		<row><td>AdminChangeFolder</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Tail</td><td>OK</td><td>Cancel</td><td>Install Point Browse</td><td/><td>0</td><td/></row>
		<row><td>AdminNetworkLocation</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>InstallNow</td><td>InstallNow</td><td>Cancel</td><td>Network Location</td><td/><td>0</td><td/></row>
		<row><td>AdminWelcome</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Administration Welcome</td><td/><td>0</td><td/></row>
		<row><td>CancelSetup</td><td>50</td><td>50</td><td>260</td><td>85</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>No</td><td>No</td><td>No</td><td>Cancel</td><td/><td>0</td><td/></row>
		<row><td>CustomSetup</td><td>50</td><td>50</td><td>374</td><td>266</td><td>35</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Tree</td><td>Next</td><td>Cancel</td><td>Custom Selection</td><td/><td>0</td><td/></row>
		<row><td>CustomSetupTips</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>OK</td><td>OK</td><td>OK</td><td>Custom Setup Tips</td><td/><td>0</td><td/></row>
		<row><td>CustomerInformation</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>NameEdit</td><td>Next</td><td>Cancel</td><td>Identification</td><td/><td>0</td><td/></row>
		<row><td>DatabaseFolder</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Database Folder</td><td/><td>0</td><td/></row>
		<row><td>DestinationFolder</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Destination Folder</td><td/><td>0</td><td/></row>
		<row><td>DiskSpaceRequirements</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>OK</td><td>OK</td><td>OK</td><td>Feature Details</td><td/><td>0</td><td/></row>
		<row><td>FilesInUse</td><td>50</td><td>50</td><td>374</td><td>266</td><td>19</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Retry</td><td>Retry</td><td>Exit</td><td>Files in Use</td><td/><td>0</td><td/></row>
		<row><td>InstallChangeFolder</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Tail</td><td>OK</td><td>Cancel</td><td>Browse</td><td/><td>0</td><td/></row>
		<row><td>InstallWelcome</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Welcome Panel</td><td/><td>0</td><td/></row>
		<row><td>LicenseAgreement</td><td>50</td><td>50</td><td>374</td><td>266</td><td>2</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Agree</td><td>Next</td><td>Cancel</td><td>License Agreement</td><td/><td>0</td><td/></row>
		<row><td>MaintenanceType</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>RadioGroup</td><td>Next</td><td>Cancel</td><td>Change, Reinstall, Remove</td><td/><td>0</td><td/></row>
		<row><td>MaintenanceWelcome</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Maintenance Welcome</td><td/><td>0</td><td/></row>
		<row><td>MsiRMFilesInUse</td><td>50</td><td>50</td><td>374</td><td>266</td><td>19</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>OK</td><td>OK</td><td>Cancel</td><td>RestartManager Files in Use</td><td/><td>0</td><td/></row>
		<row><td>OutOfSpace</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Resume</td><td>Resume</td><td>Resume</td><td>Out Of Disk Space</td><td/><td>0</td><td/></row>
		<row><td>PatchWelcome</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS__IsPatchDlg_PatchWizard##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Patch Panel</td><td/><td>0</td><td/></row>
		<row><td>ReadmeInformation</td><td>50</td><td>50</td><td>374</td><td>266</td><td>7</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Readme Information</td><td/><td>0</td><td>0</td></row>
		<row><td>ReadyToInstall</td><td>50</td><td>50</td><td>374</td><td>266</td><td>35</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>InstallNow</td><td>InstallNow</td><td>Cancel</td><td>Ready to Install</td><td/><td>0</td><td/></row>
		<row><td>ReadyToRemove</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>RemoveNow</td><td>RemoveNow</td><td>Cancel</td><td>Verify Remove</td><td/><td>0</td><td/></row>
		<row><td>SetupCompleteError</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Finish</td><td>Finish</td><td>Finish</td><td>Fatal Error</td><td/><td>0</td><td/></row>
		<row><td>SetupCompleteSuccess</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>OK</td><td>OK</td><td>OK</td><td>Exit</td><td/><td>0</td><td/></row>
		<row><td>SetupError</td><td>50</td><td>50</td><td>270</td><td>110</td><td>65543</td><td>##IDS__IsErrorDlg_InstallerInfo##</td><td>ErrorText</td><td>O</td><td>C</td><td>Error</td><td/><td>0</td><td/></row>
		<row><td>SetupInitialization</td><td>50</td><td>50</td><td>374</td><td>266</td><td>5</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Cancel</td><td>Cancel</td><td>Cancel</td><td>Setup Initialization</td><td/><td>0</td><td/></row>
		<row><td>SetupInterrupted</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Finish</td><td>Finish</td><td>Finish</td><td>User Exit</td><td/><td>0</td><td/></row>
		<row><td>SetupProgress</td><td>50</td><td>50</td><td>374</td><td>266</td><td>5</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Cancel</td><td>Cancel</td><td>Cancel</td><td>Progress</td><td/><td>0</td><td/></row>
		<row><td>SetupResume</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Resume</td><td/><td>0</td><td/></row>
		<row><td>SetupType</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>RadioGroup</td><td>Next</td><td>Cancel</td><td>Setup Type</td><td/><td>0</td><td/></row>
		<row><td>SplashBitmap</td><td>50</td><td>50</td><td>374</td><td>266</td><td>3</td><td>##IDS_PRODUCTNAME_INSTALLSHIELD##</td><td>Next</td><td>Next</td><td>Cancel</td><td>Welcome Bitmap</td><td/><td>0</td><td/></row>
	</table>

	<table name="Directory">
		<col key="yes" def="s72">Directory</col>
		<col def="S72">Directory_Parent</col>
		<col def="l255">DefaultDir</col>
		<col def="S255">ISDescription</col>
		<col def="I4">ISAttributes</col>
		<col def="S255">ISFolderName</col>
		<row><td>ALLUSERSPROFILE</td><td>TARGETDIR</td><td>.:ALLUSE~1|All Users</td><td/><td>0</td><td/></row>
		<row><td>AdminToolsFolder</td><td>TARGETDIR</td><td>.:Admint~1|AdminTools</td><td/><td>0</td><td/></row>
		<row><td>AppDataFolder</td><td>TARGETDIR</td><td>.:APPLIC~1|Application Data</td><td/><td>0</td><td/></row>
		<row><td>CommonAppDataFolder</td><td>TARGETDIR</td><td>.:Common~1|CommonAppData</td><td/><td>0</td><td/></row>
		<row><td>CommonFilesFolder</td><td>TARGETDIR</td><td>.:Common</td><td/><td>0</td><td/></row>
		<row><td>DATABASEDIR</td><td>ISYourDataBaseDir</td><td>.</td><td/><td>0</td><td/></row>
		<row><td>DesktopFolder</td><td>TARGETDIR</td><td>.:Desktop</td><td/><td>3</td><td/></row>
		<row><td>FavoritesFolder</td><td>TARGETDIR</td><td>.:FAVORI~1|Favorites</td><td/><td>0</td><td/></row>
		<row><td>FontsFolder</td><td>TARGETDIR</td><td>.:Fonts</td><td/><td>0</td><td/></row>
		<row><td>GlobalAssemblyCache</td><td>TARGETDIR</td><td>.:Global~1|GlobalAssemblyCache</td><td/><td>0</td><td/></row>
		<row><td>INSTALLDIR</td><td>Tween2</td><td>.</td><td/><td>0</td><td/></row>
		<row><td>ISCommonFilesFolder</td><td>CommonFilesFolder</td><td>Instal~1|InstallShield</td><td/><td>0</td><td/></row>
		<row><td>ISMyCompanyDir</td><td>ProgramFilesFolder</td><td>MYCOMP~1|My Company Name</td><td/><td>0</td><td/></row>
		<row><td>ISMyProductDir</td><td>ISMyCompanyDir</td><td>MYPROD~1|My Product Name</td><td/><td>0</td><td/></row>
		<row><td>ISYourDataBaseDir</td><td>INSTALLDIR</td><td>Database</td><td/><td>0</td><td/></row>
		<row><td>LocalAppDataFolder</td><td>TARGETDIR</td><td>.:LocalA~1|LocalAppData</td><td/><td>0</td><td/></row>
		<row><td>MyPicturesFolder</td><td>TARGETDIR</td><td>.:MyPict~1|MyPictures</td><td/><td>0</td><td/></row>
		<row><td>NetHoodFolder</td><td>TARGETDIR</td><td>.:NetHood</td><td/><td>0</td><td/></row>
		<row><td>PersonalFolder</td><td>TARGETDIR</td><td>.:Personal</td><td/><td>0</td><td/></row>
		<row><td>PrimaryVolumePath</td><td>TARGETDIR</td><td>.:Primar~1|PrimaryVolumePath</td><td/><td>0</td><td/></row>
		<row><td>PrintHoodFolder</td><td>TARGETDIR</td><td>.:PRINTH~1|PrintHood</td><td/><td>0</td><td/></row>
		<row><td>ProgramFilesFolder</td><td>TARGETDIR</td><td>.:PROGRA~1|program files</td><td/><td>0</td><td/></row>
		<row><td>ProgramMenuFolder</td><td>TARGETDIR</td><td>.:Programs</td><td/><td>3</td><td/></row>
		<row><td>RecentFolder</td><td>TARGETDIR</td><td>.:Recent</td><td/><td>0</td><td/></row>
		<row><td>SendToFolder</td><td>TARGETDIR</td><td>.:SendTo</td><td/><td>3</td><td/></row>
		<row><td>StartMenuFolder</td><td>TARGETDIR</td><td>.:STARTM~1|Start Menu</td><td/><td>3</td><td/></row>
		<row><td>StartupFolder</td><td>TARGETDIR</td><td>.:StartUp</td><td/><td>3</td><td/></row>
		<row><td>System16Folder</td><td>TARGETDIR</td><td>.:System</td><td/><td>0</td><td/></row>
		<row><td>SystemFolder</td><td>TARGETDIR</td><td>.:System32</td><td/><td>0</td><td/></row>
		<row><td>TARGETDIR</td><td/><td>SourceDir</td><td/><td>0</td><td/></row>
		<row><td>TempFolder</td><td>TARGETDIR</td><td>.:Temp</td><td/><td>0</td><td/></row>
		<row><td>TemplateFolder</td><td>TARGETDIR</td><td>.:ShellNew</td><td/><td>0</td><td/></row>
		<row><td>Tween2</td><td>ProgramFilesFolder</td><td>Tween</td><td/><td>0</td><td/></row>
		<row><td>USERPROFILE</td><td>TARGETDIR</td><td>.:USERPR~1|UserProfile</td><td/><td>0</td><td/></row>
		<row><td>WindowsFolder</td><td>TARGETDIR</td><td>.:Windows</td><td/><td>0</td><td/></row>
		<row><td>WindowsVolume</td><td>TARGETDIR</td><td>.:WinRoot</td><td/><td>0</td><td/></row>
		<row><td>tween1</td><td>ProgramMenuFolder</td><td>Tween</td><td/><td>1</td><td/></row>
	</table>

	<table name="DrLocator">
		<col key="yes" def="s72">Signature_</col>
		<col key="yes" def="S72">Parent</col>
		<col key="yes" def="S255">Path</col>
		<col def="I2">Depth</col>
	</table>

	<table name="DuplicateFile">
		<col key="yes" def="s72">FileKey</col>
		<col def="s72">Component_</col>
		<col def="s72">File_</col>
		<col def="L255">DestName</col>
		<col def="S72">DestFolder</col>
	</table>

	<table name="Environment">
		<col key="yes" def="s72">Environment</col>
		<col def="l255">Name</col>
		<col def="L255">Value</col>
		<col def="s72">Component_</col>
	</table>

	<table name="Error">
		<col key="yes" def="i2">Error</col>
		<col def="L255">Message</col>
		<row><td>0</td><td>##IDS_ERROR_0##</td></row>
		<row><td>1</td><td>##IDS_ERROR_1##</td></row>
		<row><td>10</td><td>##IDS_ERROR_8##</td></row>
		<row><td>11</td><td>##IDS_ERROR_9##</td></row>
		<row><td>1101</td><td>##IDS_ERROR_22##</td></row>
		<row><td>12</td><td>##IDS_ERROR_10##</td></row>
		<row><td>13</td><td>##IDS_ERROR_11##</td></row>
		<row><td>1301</td><td>##IDS_ERROR_23##</td></row>
		<row><td>1302</td><td>##IDS_ERROR_24##</td></row>
		<row><td>1303</td><td>##IDS_ERROR_25##</td></row>
		<row><td>1304</td><td>##IDS_ERROR_26##</td></row>
		<row><td>1305</td><td>##IDS_ERROR_27##</td></row>
		<row><td>1306</td><td>##IDS_ERROR_28##</td></row>
		<row><td>1307</td><td>##IDS_ERROR_29##</td></row>
		<row><td>1308</td><td>##IDS_ERROR_30##</td></row>
		<row><td>1309</td><td>##IDS_ERROR_31##</td></row>
		<row><td>1310</td><td>##IDS_ERROR_32##</td></row>
		<row><td>1311</td><td>##IDS_ERROR_33##</td></row>
		<row><td>1312</td><td>##IDS_ERROR_34##</td></row>
		<row><td>1313</td><td>##IDS_ERROR_35##</td></row>
		<row><td>1314</td><td>##IDS_ERROR_36##</td></row>
		<row><td>1315</td><td>##IDS_ERROR_37##</td></row>
		<row><td>1316</td><td>##IDS_ERROR_38##</td></row>
		<row><td>1317</td><td>##IDS_ERROR_39##</td></row>
		<row><td>1318</td><td>##IDS_ERROR_40##</td></row>
		<row><td>1319</td><td>##IDS_ERROR_41##</td></row>
		<row><td>1320</td><td>##IDS_ERROR_42##</td></row>
		<row><td>1321</td><td>##IDS_ERROR_43##</td></row>
		<row><td>1322</td><td>##IDS_ERROR_44##</td></row>
		<row><td>1323</td><td>##IDS_ERROR_45##</td></row>
		<row><td>1324</td><td>##IDS_ERROR_46##</td></row>
		<row><td>1325</td><td>##IDS_ERROR_47##</td></row>
		<row><td>1326</td><td>##IDS_ERROR_48##</td></row>
		<row><td>1327</td><td>##IDS_ERROR_49##</td></row>
		<row><td>1328</td><td>##IDS_ERROR_122##</td></row>
		<row><td>1329</td><td>##IDS_ERROR_1329##</td></row>
		<row><td>1330</td><td>##IDS_ERROR_1330##</td></row>
		<row><td>1331</td><td>##IDS_ERROR_1331##</td></row>
		<row><td>1332</td><td>##IDS_ERROR_1332##</td></row>
		<row><td>1333</td><td>##IDS_ERROR_1333##</td></row>
		<row><td>1334</td><td>##IDS_ERROR_1334##</td></row>
		<row><td>1335</td><td>##IDS_ERROR_1335##</td></row>
		<row><td>1336</td><td>##IDS_ERROR_1336##</td></row>
		<row><td>14</td><td>##IDS_ERROR_12##</td></row>
		<row><td>1401</td><td>##IDS_ERROR_50##</td></row>
		<row><td>1402</td><td>##IDS_ERROR_51##</td></row>
		<row><td>1403</td><td>##IDS_ERROR_52##</td></row>
		<row><td>1404</td><td>##IDS_ERROR_53##</td></row>
		<row><td>1405</td><td>##IDS_ERROR_54##</td></row>
		<row><td>1406</td><td>##IDS_ERROR_55##</td></row>
		<row><td>1407</td><td>##IDS_ERROR_56##</td></row>
		<row><td>1408</td><td>##IDS_ERROR_57##</td></row>
		<row><td>1409</td><td>##IDS_ERROR_58##</td></row>
		<row><td>1410</td><td>##IDS_ERROR_59##</td></row>
		<row><td>15</td><td>##IDS_ERROR_13##</td></row>
		<row><td>1500</td><td>##IDS_ERROR_60##</td></row>
		<row><td>1501</td><td>##IDS_ERROR_61##</td></row>
		<row><td>1502</td><td>##IDS_ERROR_62##</td></row>
		<row><td>1503</td><td>##IDS_ERROR_63##</td></row>
		<row><td>16</td><td>##IDS_ERROR_14##</td></row>
		<row><td>1601</td><td>##IDS_ERROR_64##</td></row>
		<row><td>1602</td><td>##IDS_ERROR_65##</td></row>
		<row><td>1603</td><td>##IDS_ERROR_66##</td></row>
		<row><td>1604</td><td>##IDS_ERROR_67##</td></row>
		<row><td>1605</td><td>##IDS_ERROR_68##</td></row>
		<row><td>1606</td><td>##IDS_ERROR_69##</td></row>
		<row><td>1607</td><td>##IDS_ERROR_70##</td></row>
		<row><td>1608</td><td>##IDS_ERROR_71##</td></row>
		<row><td>1609</td><td>##IDS_ERROR_1609##</td></row>
		<row><td>1651</td><td>##IDS_ERROR_1651##</td></row>
		<row><td>17</td><td>##IDS_ERROR_15##</td></row>
		<row><td>1701</td><td>##IDS_ERROR_72##</td></row>
		<row><td>1702</td><td>##IDS_ERROR_73##</td></row>
		<row><td>1703</td><td>##IDS_ERROR_74##</td></row>
		<row><td>1704</td><td>##IDS_ERROR_75##</td></row>
		<row><td>1705</td><td>##IDS_ERROR_76##</td></row>
		<row><td>1706</td><td>##IDS_ERROR_77##</td></row>
		<row><td>1707</td><td>##IDS_ERROR_78##</td></row>
		<row><td>1708</td><td>##IDS_ERROR_79##</td></row>
		<row><td>1709</td><td>##IDS_ERROR_80##</td></row>
		<row><td>1710</td><td>##IDS_ERROR_81##</td></row>
		<row><td>1711</td><td>##IDS_ERROR_82##</td></row>
		<row><td>1712</td><td>##IDS_ERROR_83##</td></row>
		<row><td>1713</td><td>##IDS_ERROR_123##</td></row>
		<row><td>1714</td><td>##IDS_ERROR_124##</td></row>
		<row><td>1715</td><td>##IDS_ERROR_1715##</td></row>
		<row><td>1716</td><td>##IDS_ERROR_1716##</td></row>
		<row><td>1717</td><td>##IDS_ERROR_1717##</td></row>
		<row><td>1718</td><td>##IDS_ERROR_1718##</td></row>
		<row><td>1719</td><td>##IDS_ERROR_1719##</td></row>
		<row><td>1720</td><td>##IDS_ERROR_1720##</td></row>
		<row><td>1721</td><td>##IDS_ERROR_1721##</td></row>
		<row><td>1722</td><td>##IDS_ERROR_1722##</td></row>
		<row><td>1723</td><td>##IDS_ERROR_1723##</td></row>
		<row><td>1724</td><td>##IDS_ERROR_1724##</td></row>
		<row><td>1725</td><td>##IDS_ERROR_1725##</td></row>
		<row><td>1726</td><td>##IDS_ERROR_1726##</td></row>
		<row><td>1727</td><td>##IDS_ERROR_1727##</td></row>
		<row><td>1728</td><td>##IDS_ERROR_1728##</td></row>
		<row><td>1729</td><td>##IDS_ERROR_1729##</td></row>
		<row><td>1730</td><td>##IDS_ERROR_1730##</td></row>
		<row><td>1731</td><td>##IDS_ERROR_1731##</td></row>
		<row><td>1732</td><td>##IDS_ERROR_1732##</td></row>
		<row><td>18</td><td>##IDS_ERROR_16##</td></row>
		<row><td>1801</td><td>##IDS_ERROR_84##</td></row>
		<row><td>1802</td><td>##IDS_ERROR_85##</td></row>
		<row><td>1803</td><td>##IDS_ERROR_86##</td></row>
		<row><td>1804</td><td>##IDS_ERROR_87##</td></row>
		<row><td>1805</td><td>##IDS_ERROR_88##</td></row>
		<row><td>1806</td><td>##IDS_ERROR_89##</td></row>
		<row><td>1807</td><td>##IDS_ERROR_90##</td></row>
		<row><td>19</td><td>##IDS_ERROR_17##</td></row>
		<row><td>1901</td><td>##IDS_ERROR_91##</td></row>
		<row><td>1902</td><td>##IDS_ERROR_92##</td></row>
		<row><td>1903</td><td>##IDS_ERROR_93##</td></row>
		<row><td>1904</td><td>##IDS_ERROR_94##</td></row>
		<row><td>1905</td><td>##IDS_ERROR_95##</td></row>
		<row><td>1906</td><td>##IDS_ERROR_96##</td></row>
		<row><td>1907</td><td>##IDS_ERROR_97##</td></row>
		<row><td>1908</td><td>##IDS_ERROR_98##</td></row>
		<row><td>1909</td><td>##IDS_ERROR_99##</td></row>
		<row><td>1910</td><td>##IDS_ERROR_100##</td></row>
		<row><td>1911</td><td>##IDS_ERROR_101##</td></row>
		<row><td>1912</td><td>##IDS_ERROR_102##</td></row>
		<row><td>1913</td><td>##IDS_ERROR_103##</td></row>
		<row><td>1914</td><td>##IDS_ERROR_104##</td></row>
		<row><td>1915</td><td>##IDS_ERROR_105##</td></row>
		<row><td>1916</td><td>##IDS_ERROR_106##</td></row>
		<row><td>1917</td><td>##IDS_ERROR_107##</td></row>
		<row><td>1918</td><td>##IDS_ERROR_108##</td></row>
		<row><td>1919</td><td>##IDS_ERROR_109##</td></row>
		<row><td>1920</td><td>##IDS_ERROR_110##</td></row>
		<row><td>1921</td><td>##IDS_ERROR_111##</td></row>
		<row><td>1922</td><td>##IDS_ERROR_112##</td></row>
		<row><td>1923</td><td>##IDS_ERROR_113##</td></row>
		<row><td>1924</td><td>##IDS_ERROR_114##</td></row>
		<row><td>1925</td><td>##IDS_ERROR_115##</td></row>
		<row><td>1926</td><td>##IDS_ERROR_116##</td></row>
		<row><td>1927</td><td>##IDS_ERROR_117##</td></row>
		<row><td>1928</td><td>##IDS_ERROR_118##</td></row>
		<row><td>1929</td><td>##IDS_ERROR_119##</td></row>
		<row><td>1930</td><td>##IDS_ERROR_125##</td></row>
		<row><td>1931</td><td>##IDS_ERROR_126##</td></row>
		<row><td>1932</td><td>##IDS_ERROR_127##</td></row>
		<row><td>1933</td><td>##IDS_ERROR_128##</td></row>
		<row><td>1934</td><td>##IDS_ERROR_129##</td></row>
		<row><td>1935</td><td>##IDS_ERROR_1935##</td></row>
		<row><td>1936</td><td>##IDS_ERROR_1936##</td></row>
		<row><td>1937</td><td>##IDS_ERROR_1937##</td></row>
		<row><td>1938</td><td>##IDS_ERROR_1938##</td></row>
		<row><td>2</td><td>##IDS_ERROR_2##</td></row>
		<row><td>20</td><td>##IDS_ERROR_18##</td></row>
		<row><td>21</td><td>##IDS_ERROR_19##</td></row>
		<row><td>2101</td><td>##IDS_ERROR_2101##</td></row>
		<row><td>2102</td><td>##IDS_ERROR_2102##</td></row>
		<row><td>2103</td><td>##IDS_ERROR_2103##</td></row>
		<row><td>2104</td><td>##IDS_ERROR_2104##</td></row>
		<row><td>2105</td><td>##IDS_ERROR_2105##</td></row>
		<row><td>2106</td><td>##IDS_ERROR_2106##</td></row>
		<row><td>2107</td><td>##IDS_ERROR_2107##</td></row>
		<row><td>2108</td><td>##IDS_ERROR_2108##</td></row>
		<row><td>2109</td><td>##IDS_ERROR_2109##</td></row>
		<row><td>2110</td><td>##IDS_ERROR_2110##</td></row>
		<row><td>2111</td><td>##IDS_ERROR_2111##</td></row>
		<row><td>2112</td><td>##IDS_ERROR_2112##</td></row>
		<row><td>2113</td><td>##IDS_ERROR_2113##</td></row>
		<row><td>22</td><td>##IDS_ERROR_120##</td></row>
		<row><td>2200</td><td>##IDS_ERROR_2200##</td></row>
		<row><td>2201</td><td>##IDS_ERROR_2201##</td></row>
		<row><td>2202</td><td>##IDS_ERROR_2202##</td></row>
		<row><td>2203</td><td>##IDS_ERROR_2203##</td></row>
		<row><td>2204</td><td>##IDS_ERROR_2204##</td></row>
		<row><td>2205</td><td>##IDS_ERROR_2205##</td></row>
		<row><td>2206</td><td>##IDS_ERROR_2206##</td></row>
		<row><td>2207</td><td>##IDS_ERROR_2207##</td></row>
		<row><td>2208</td><td>##IDS_ERROR_2208##</td></row>
		<row><td>2209</td><td>##IDS_ERROR_2209##</td></row>
		<row><td>2210</td><td>##IDS_ERROR_2210##</td></row>
		<row><td>2211</td><td>##IDS_ERROR_2211##</td></row>
		<row><td>2212</td><td>##IDS_ERROR_2212##</td></row>
		<row><td>2213</td><td>##IDS_ERROR_2213##</td></row>
		<row><td>2214</td><td>##IDS_ERROR_2214##</td></row>
		<row><td>2215</td><td>##IDS_ERROR_2215##</td></row>
		<row><td>2216</td><td>##IDS_ERROR_2216##</td></row>
		<row><td>2217</td><td>##IDS_ERROR_2217##</td></row>
		<row><td>2218</td><td>##IDS_ERROR_2218##</td></row>
		<row><td>2219</td><td>##IDS_ERROR_2219##</td></row>
		<row><td>2220</td><td>##IDS_ERROR_2220##</td></row>
		<row><td>2221</td><td>##IDS_ERROR_2221##</td></row>
		<row><td>2222</td><td>##IDS_ERROR_2222##</td></row>
		<row><td>2223</td><td>##IDS_ERROR_2223##</td></row>
		<row><td>2224</td><td>##IDS_ERROR_2224##</td></row>
		<row><td>2225</td><td>##IDS_ERROR_2225##</td></row>
		<row><td>2226</td><td>##IDS_ERROR_2226##</td></row>
		<row><td>2227</td><td>##IDS_ERROR_2227##</td></row>
		<row><td>2228</td><td>##IDS_ERROR_2228##</td></row>
		<row><td>2229</td><td>##IDS_ERROR_2229##</td></row>
		<row><td>2230</td><td>##IDS_ERROR_2230##</td></row>
		<row><td>2231</td><td>##IDS_ERROR_2231##</td></row>
		<row><td>2232</td><td>##IDS_ERROR_2232##</td></row>
		<row><td>2233</td><td>##IDS_ERROR_2233##</td></row>
		<row><td>2234</td><td>##IDS_ERROR_2234##</td></row>
		<row><td>2235</td><td>##IDS_ERROR_2235##</td></row>
		<row><td>2236</td><td>##IDS_ERROR_2236##</td></row>
		<row><td>2237</td><td>##IDS_ERROR_2237##</td></row>
		<row><td>2238</td><td>##IDS_ERROR_2238##</td></row>
		<row><td>2239</td><td>##IDS_ERROR_2239##</td></row>
		<row><td>2240</td><td>##IDS_ERROR_2240##</td></row>
		<row><td>2241</td><td>##IDS_ERROR_2241##</td></row>
		<row><td>2242</td><td>##IDS_ERROR_2242##</td></row>
		<row><td>2243</td><td>##IDS_ERROR_2243##</td></row>
		<row><td>2244</td><td>##IDS_ERROR_2244##</td></row>
		<row><td>2245</td><td>##IDS_ERROR_2245##</td></row>
		<row><td>2246</td><td>##IDS_ERROR_2246##</td></row>
		<row><td>2247</td><td>##IDS_ERROR_2247##</td></row>
		<row><td>2248</td><td>##IDS_ERROR_2248##</td></row>
		<row><td>2249</td><td>##IDS_ERROR_2249##</td></row>
		<row><td>2250</td><td>##IDS_ERROR_2250##</td></row>
		<row><td>2251</td><td>##IDS_ERROR_2251##</td></row>
		<row><td>2252</td><td>##IDS_ERROR_2252##</td></row>
		<row><td>2253</td><td>##IDS_ERROR_2253##</td></row>
		<row><td>2254</td><td>##IDS_ERROR_2254##</td></row>
		<row><td>2255</td><td>##IDS_ERROR_2255##</td></row>
		<row><td>2256</td><td>##IDS_ERROR_2256##</td></row>
		<row><td>2257</td><td>##IDS_ERROR_2257##</td></row>
		<row><td>2258</td><td>##IDS_ERROR_2258##</td></row>
		<row><td>2259</td><td>##IDS_ERROR_2259##</td></row>
		<row><td>2260</td><td>##IDS_ERROR_2260##</td></row>
		<row><td>2261</td><td>##IDS_ERROR_2261##</td></row>
		<row><td>2262</td><td>##IDS_ERROR_2262##</td></row>
		<row><td>2263</td><td>##IDS_ERROR_2263##</td></row>
		<row><td>2264</td><td>##IDS_ERROR_2264##</td></row>
		<row><td>2265</td><td>##IDS_ERROR_2265##</td></row>
		<row><td>2266</td><td>##IDS_ERROR_2266##</td></row>
		<row><td>2267</td><td>##IDS_ERROR_2267##</td></row>
		<row><td>2268</td><td>##IDS_ERROR_2268##</td></row>
		<row><td>2269</td><td>##IDS_ERROR_2269##</td></row>
		<row><td>2270</td><td>##IDS_ERROR_2270##</td></row>
		<row><td>2271</td><td>##IDS_ERROR_2271##</td></row>
		<row><td>2272</td><td>##IDS_ERROR_2272##</td></row>
		<row><td>2273</td><td>##IDS_ERROR_2273##</td></row>
		<row><td>2274</td><td>##IDS_ERROR_2274##</td></row>
		<row><td>2275</td><td>##IDS_ERROR_2275##</td></row>
		<row><td>2276</td><td>##IDS_ERROR_2276##</td></row>
		<row><td>2277</td><td>##IDS_ERROR_2277##</td></row>
		<row><td>2278</td><td>##IDS_ERROR_2278##</td></row>
		<row><td>2279</td><td>##IDS_ERROR_2279##</td></row>
		<row><td>2280</td><td>##IDS_ERROR_2280##</td></row>
		<row><td>2281</td><td>##IDS_ERROR_2281##</td></row>
		<row><td>2282</td><td>##IDS_ERROR_2282##</td></row>
		<row><td>23</td><td>##IDS_ERROR_121##</td></row>
		<row><td>2302</td><td>##IDS_ERROR_2302##</td></row>
		<row><td>2303</td><td>##IDS_ERROR_2303##</td></row>
		<row><td>2304</td><td>##IDS_ERROR_2304##</td></row>
		<row><td>2305</td><td>##IDS_ERROR_2305##</td></row>
		<row><td>2306</td><td>##IDS_ERROR_2306##</td></row>
		<row><td>2307</td><td>##IDS_ERROR_2307##</td></row>
		<row><td>2308</td><td>##IDS_ERROR_2308##</td></row>
		<row><td>2309</td><td>##IDS_ERROR_2309##</td></row>
		<row><td>2310</td><td>##IDS_ERROR_2310##</td></row>
		<row><td>2315</td><td>##IDS_ERROR_2315##</td></row>
		<row><td>2318</td><td>##IDS_ERROR_2318##</td></row>
		<row><td>2319</td><td>##IDS_ERROR_2319##</td></row>
		<row><td>2320</td><td>##IDS_ERROR_2320##</td></row>
		<row><td>2321</td><td>##IDS_ERROR_2321##</td></row>
		<row><td>2322</td><td>##IDS_ERROR_2322##</td></row>
		<row><td>2323</td><td>##IDS_ERROR_2323##</td></row>
		<row><td>2324</td><td>##IDS_ERROR_2324##</td></row>
		<row><td>2325</td><td>##IDS_ERROR_2325##</td></row>
		<row><td>2326</td><td>##IDS_ERROR_2326##</td></row>
		<row><td>2327</td><td>##IDS_ERROR_2327##</td></row>
		<row><td>2328</td><td>##IDS_ERROR_2328##</td></row>
		<row><td>2329</td><td>##IDS_ERROR_2329##</td></row>
		<row><td>2330</td><td>##IDS_ERROR_2330##</td></row>
		<row><td>2331</td><td>##IDS_ERROR_2331##</td></row>
		<row><td>2332</td><td>##IDS_ERROR_2332##</td></row>
		<row><td>2333</td><td>##IDS_ERROR_2333##</td></row>
		<row><td>2334</td><td>##IDS_ERROR_2334##</td></row>
		<row><td>2335</td><td>##IDS_ERROR_2335##</td></row>
		<row><td>2336</td><td>##IDS_ERROR_2336##</td></row>
		<row><td>2337</td><td>##IDS_ERROR_2337##</td></row>
		<row><td>2338</td><td>##IDS_ERROR_2338##</td></row>
		<row><td>2339</td><td>##IDS_ERROR_2339##</td></row>
		<row><td>2340</td><td>##IDS_ERROR_2340##</td></row>
		<row><td>2341</td><td>##IDS_ERROR_2341##</td></row>
		<row><td>2342</td><td>##IDS_ERROR_2342##</td></row>
		<row><td>2343</td><td>##IDS_ERROR_2343##</td></row>
		<row><td>2344</td><td>##IDS_ERROR_2344##</td></row>
		<row><td>2345</td><td>##IDS_ERROR_2345##</td></row>
		<row><td>2347</td><td>##IDS_ERROR_2347##</td></row>
		<row><td>2348</td><td>##IDS_ERROR_2348##</td></row>
		<row><td>2349</td><td>##IDS_ERROR_2349##</td></row>
		<row><td>2350</td><td>##IDS_ERROR_2350##</td></row>
		<row><td>2351</td><td>##IDS_ERROR_2351##</td></row>
		<row><td>2352</td><td>##IDS_ERROR_2352##</td></row>
		<row><td>2353</td><td>##IDS_ERROR_2353##</td></row>
		<row><td>2354</td><td>##IDS_ERROR_2354##</td></row>
		<row><td>2355</td><td>##IDS_ERROR_2355##</td></row>
		<row><td>2356</td><td>##IDS_ERROR_2356##</td></row>
		<row><td>2357</td><td>##IDS_ERROR_2357##</td></row>
		<row><td>2358</td><td>##IDS_ERROR_2358##</td></row>
		<row><td>2359</td><td>##IDS_ERROR_2359##</td></row>
		<row><td>2360</td><td>##IDS_ERROR_2360##</td></row>
		<row><td>2361</td><td>##IDS_ERROR_2361##</td></row>
		<row><td>2362</td><td>##IDS_ERROR_2362##</td></row>
		<row><td>2363</td><td>##IDS_ERROR_2363##</td></row>
		<row><td>2364</td><td>##IDS_ERROR_2364##</td></row>
		<row><td>2365</td><td>##IDS_ERROR_2365##</td></row>
		<row><td>2366</td><td>##IDS_ERROR_2366##</td></row>
		<row><td>2367</td><td>##IDS_ERROR_2367##</td></row>
		<row><td>2368</td><td>##IDS_ERROR_2368##</td></row>
		<row><td>2370</td><td>##IDS_ERROR_2370##</td></row>
		<row><td>2371</td><td>##IDS_ERROR_2371##</td></row>
		<row><td>2372</td><td>##IDS_ERROR_2372##</td></row>
		<row><td>2373</td><td>##IDS_ERROR_2373##</td></row>
		<row><td>2374</td><td>##IDS_ERROR_2374##</td></row>
		<row><td>2375</td><td>##IDS_ERROR_2375##</td></row>
		<row><td>2376</td><td>##IDS_ERROR_2376##</td></row>
		<row><td>2379</td><td>##IDS_ERROR_2379##</td></row>
		<row><td>2380</td><td>##IDS_ERROR_2380##</td></row>
		<row><td>2381</td><td>##IDS_ERROR_2381##</td></row>
		<row><td>2382</td><td>##IDS_ERROR_2382##</td></row>
		<row><td>2401</td><td>##IDS_ERROR_2401##</td></row>
		<row><td>2402</td><td>##IDS_ERROR_2402##</td></row>
		<row><td>2501</td><td>##IDS_ERROR_2501##</td></row>
		<row><td>2502</td><td>##IDS_ERROR_2502##</td></row>
		<row><td>2503</td><td>##IDS_ERROR_2503##</td></row>
		<row><td>2601</td><td>##IDS_ERROR_2601##</td></row>
		<row><td>2602</td><td>##IDS_ERROR_2602##</td></row>
		<row><td>2603</td><td>##IDS_ERROR_2603##</td></row>
		<row><td>2604</td><td>##IDS_ERROR_2604##</td></row>
		<row><td>2605</td><td>##IDS_ERROR_2605##</td></row>
		<row><td>2606</td><td>##IDS_ERROR_2606##</td></row>
		<row><td>2607</td><td>##IDS_ERROR_2607##</td></row>
		<row><td>2608</td><td>##IDS_ERROR_2608##</td></row>
		<row><td>2609</td><td>##IDS_ERROR_2609##</td></row>
		<row><td>2611</td><td>##IDS_ERROR_2611##</td></row>
		<row><td>2612</td><td>##IDS_ERROR_2612##</td></row>
		<row><td>2613</td><td>##IDS_ERROR_2613##</td></row>
		<row><td>2614</td><td>##IDS_ERROR_2614##</td></row>
		<row><td>2615</td><td>##IDS_ERROR_2615##</td></row>
		<row><td>2616</td><td>##IDS_ERROR_2616##</td></row>
		<row><td>2617</td><td>##IDS_ERROR_2617##</td></row>
		<row><td>2618</td><td>##IDS_ERROR_2618##</td></row>
		<row><td>2619</td><td>##IDS_ERROR_2619##</td></row>
		<row><td>2620</td><td>##IDS_ERROR_2620##</td></row>
		<row><td>2621</td><td>##IDS_ERROR_2621##</td></row>
		<row><td>2701</td><td>##IDS_ERROR_2701##</td></row>
		<row><td>2702</td><td>##IDS_ERROR_2702##</td></row>
		<row><td>2703</td><td>##IDS_ERROR_2703##</td></row>
		<row><td>2704</td><td>##IDS_ERROR_2704##</td></row>
		<row><td>2705</td><td>##IDS_ERROR_2705##</td></row>
		<row><td>2706</td><td>##IDS_ERROR_2706##</td></row>
		<row><td>2707</td><td>##IDS_ERROR_2707##</td></row>
		<row><td>2708</td><td>##IDS_ERROR_2708##</td></row>
		<row><td>2709</td><td>##IDS_ERROR_2709##</td></row>
		<row><td>2710</td><td>##IDS_ERROR_2710##</td></row>
		<row><td>2711</td><td>##IDS_ERROR_2711##</td></row>
		<row><td>2712</td><td>##IDS_ERROR_2712##</td></row>
		<row><td>2713</td><td>##IDS_ERROR_2713##</td></row>
		<row><td>2714</td><td>##IDS_ERROR_2714##</td></row>
		<row><td>2715</td><td>##IDS_ERROR_2715##</td></row>
		<row><td>2716</td><td>##IDS_ERROR_2716##</td></row>
		<row><td>2717</td><td>##IDS_ERROR_2717##</td></row>
		<row><td>2718</td><td>##IDS_ERROR_2718##</td></row>
		<row><td>2719</td><td>##IDS_ERROR_2719##</td></row>
		<row><td>2720</td><td>##IDS_ERROR_2720##</td></row>
		<row><td>2721</td><td>##IDS_ERROR_2721##</td></row>
		<row><td>2722</td><td>##IDS_ERROR_2722##</td></row>
		<row><td>2723</td><td>##IDS_ERROR_2723##</td></row>
		<row><td>2724</td><td>##IDS_ERROR_2724##</td></row>
		<row><td>2725</td><td>##IDS_ERROR_2725##</td></row>
		<row><td>2726</td><td>##IDS_ERROR_2726##</td></row>
		<row><td>2727</td><td>##IDS_ERROR_2727##</td></row>
		<row><td>2728</td><td>##IDS_ERROR_2728##</td></row>
		<row><td>2729</td><td>##IDS_ERROR_2729##</td></row>
		<row><td>2730</td><td>##IDS_ERROR_2730##</td></row>
		<row><td>2731</td><td>##IDS_ERROR_2731##</td></row>
		<row><td>2732</td><td>##IDS_ERROR_2732##</td></row>
		<row><td>2733</td><td>##IDS_ERROR_2733##</td></row>
		<row><td>2734</td><td>##IDS_ERROR_2734##</td></row>
		<row><td>2735</td><td>##IDS_ERROR_2735##</td></row>
		<row><td>2736</td><td>##IDS_ERROR_2736##</td></row>
		<row><td>2737</td><td>##IDS_ERROR_2737##</td></row>
		<row><td>2738</td><td>##IDS_ERROR_2738##</td></row>
		<row><td>2739</td><td>##IDS_ERROR_2739##</td></row>
		<row><td>2740</td><td>##IDS_ERROR_2740##</td></row>
		<row><td>2741</td><td>##IDS_ERROR_2741##</td></row>
		<row><td>2742</td><td>##IDS_ERROR_2742##</td></row>
		<row><td>2743</td><td>##IDS_ERROR_2743##</td></row>
		<row><td>2744</td><td>##IDS_ERROR_2744##</td></row>
		<row><td>2745</td><td>##IDS_ERROR_2745##</td></row>
		<row><td>2746</td><td>##IDS_ERROR_2746##</td></row>
		<row><td>2747</td><td>##IDS_ERROR_2747##</td></row>
		<row><td>2748</td><td>##IDS_ERROR_2748##</td></row>
		<row><td>2749</td><td>##IDS_ERROR_2749##</td></row>
		<row><td>2750</td><td>##IDS_ERROR_2750##</td></row>
		<row><td>27500</td><td>##IDS_ERROR_130##</td></row>
		<row><td>27501</td><td>##IDS_ERROR_131##</td></row>
		<row><td>27502</td><td>##IDS_ERROR_27502##</td></row>
		<row><td>27503</td><td>##IDS_ERROR_27503##</td></row>
		<row><td>27504</td><td>##IDS_ERROR_27504##</td></row>
		<row><td>27505</td><td>##IDS_ERROR_27505##</td></row>
		<row><td>27506</td><td>##IDS_ERROR_27506##</td></row>
		<row><td>27507</td><td>##IDS_ERROR_27507##</td></row>
		<row><td>27508</td><td>##IDS_ERROR_27508##</td></row>
		<row><td>27509</td><td>##IDS_ERROR_27509##</td></row>
		<row><td>2751</td><td>##IDS_ERROR_2751##</td></row>
		<row><td>27510</td><td>##IDS_ERROR_27510##</td></row>
		<row><td>27511</td><td>##IDS_ERROR_27511##</td></row>
		<row><td>27512</td><td>##IDS_ERROR_27512##</td></row>
		<row><td>27513</td><td>##IDS_ERROR_27513##</td></row>
		<row><td>27514</td><td>##IDS_ERROR_27514##</td></row>
		<row><td>27515</td><td>##IDS_ERROR_27515##</td></row>
		<row><td>27516</td><td>##IDS_ERROR_27516##</td></row>
		<row><td>27517</td><td>##IDS_ERROR_27517##</td></row>
		<row><td>27518</td><td>##IDS_ERROR_27518##</td></row>
		<row><td>27519</td><td>##IDS_ERROR_27519##</td></row>
		<row><td>2752</td><td>##IDS_ERROR_2752##</td></row>
		<row><td>27520</td><td>##IDS_ERROR_27520##</td></row>
		<row><td>27521</td><td>##IDS_ERROR_27521##</td></row>
		<row><td>27522</td><td>##IDS_ERROR_27522##</td></row>
		<row><td>27523</td><td>##IDS_ERROR_27523##</td></row>
		<row><td>27524</td><td>##IDS_ERROR_27524##</td></row>
		<row><td>27525</td><td>##IDS_ERROR_27525##</td></row>
		<row><td>27526</td><td>##IDS_ERROR_27526##</td></row>
		<row><td>27527</td><td>##IDS_ERROR_27527##</td></row>
		<row><td>27528</td><td>##IDS_ERROR_27528##</td></row>
		<row><td>27529</td><td>##IDS_ERROR_27529##</td></row>
		<row><td>2753</td><td>##IDS_ERROR_2753##</td></row>
		<row><td>27530</td><td>##IDS_ERROR_27530##</td></row>
		<row><td>27531</td><td>##IDS_ERROR_27531##</td></row>
		<row><td>27532</td><td>##IDS_ERROR_27532##</td></row>
		<row><td>27533</td><td>##IDS_ERROR_27533##</td></row>
		<row><td>27534</td><td>##IDS_ERROR_27534##</td></row>
		<row><td>27535</td><td>##IDS_ERROR_27535##</td></row>
		<row><td>27536</td><td>##IDS_ERROR_27536##</td></row>
		<row><td>27537</td><td>##IDS_ERROR_27537##</td></row>
		<row><td>27538</td><td>##IDS_ERROR_27538##</td></row>
		<row><td>27539</td><td>##IDS_ERROR_27539##</td></row>
		<row><td>2754</td><td>##IDS_ERROR_2754##</td></row>
		<row><td>27540</td><td>##IDS_ERROR_27540##</td></row>
		<row><td>27541</td><td>##IDS_ERROR_27541##</td></row>
		<row><td>27542</td><td>##IDS_ERROR_27542##</td></row>
		<row><td>27543</td><td>##IDS_ERROR_27543##</td></row>
		<row><td>27544</td><td>##IDS_ERROR_27544##</td></row>
		<row><td>27545</td><td>##IDS_ERROR_27545##</td></row>
		<row><td>27546</td><td>##IDS_ERROR_27546##</td></row>
		<row><td>27547</td><td>##IDS_ERROR_27547##</td></row>
		<row><td>27548</td><td>##IDS_ERROR_27548##</td></row>
		<row><td>27549</td><td>##IDS_ERROR_27549##</td></row>
		<row><td>2755</td><td>##IDS_ERROR_2755##</td></row>
		<row><td>27550</td><td>##IDS_ERROR_27550##</td></row>
		<row><td>27551</td><td>##IDS_ERROR_27551##</td></row>
		<row><td>27552</td><td>##IDS_ERROR_27552##</td></row>
		<row><td>27553</td><td>##IDS_ERROR_27553##</td></row>
		<row><td>27554</td><td>##IDS_ERROR_27554##</td></row>
		<row><td>27555</td><td>##IDS_ERROR_27555##</td></row>
		<row><td>2756</td><td>##IDS_ERROR_2756##</td></row>
		<row><td>2757</td><td>##IDS_ERROR_2757##</td></row>
		<row><td>2758</td><td>##IDS_ERROR_2758##</td></row>
		<row><td>2759</td><td>##IDS_ERROR_2759##</td></row>
		<row><td>2760</td><td>##IDS_ERROR_2760##</td></row>
		<row><td>2761</td><td>##IDS_ERROR_2761##</td></row>
		<row><td>2762</td><td>##IDS_ERROR_2762##</td></row>
		<row><td>2763</td><td>##IDS_ERROR_2763##</td></row>
		<row><td>2765</td><td>##IDS_ERROR_2765##</td></row>
		<row><td>2766</td><td>##IDS_ERROR_2766##</td></row>
		<row><td>2767</td><td>##IDS_ERROR_2767##</td></row>
		<row><td>2768</td><td>##IDS_ERROR_2768##</td></row>
		<row><td>2769</td><td>##IDS_ERROR_2769##</td></row>
		<row><td>2770</td><td>##IDS_ERROR_2770##</td></row>
		<row><td>2771</td><td>##IDS_ERROR_2771##</td></row>
		<row><td>2772</td><td>##IDS_ERROR_2772##</td></row>
		<row><td>2801</td><td>##IDS_ERROR_2801##</td></row>
		<row><td>2802</td><td>##IDS_ERROR_2802##</td></row>
		<row><td>2803</td><td>##IDS_ERROR_2803##</td></row>
		<row><td>2804</td><td>##IDS_ERROR_2804##</td></row>
		<row><td>2806</td><td>##IDS_ERROR_2806##</td></row>
		<row><td>2807</td><td>##IDS_ERROR_2807##</td></row>
		<row><td>2808</td><td>##IDS_ERROR_2808##</td></row>
		<row><td>2809</td><td>##IDS_ERROR_2809##</td></row>
		<row><td>2810</td><td>##IDS_ERROR_2810##</td></row>
		<row><td>2811</td><td>##IDS_ERROR_2811##</td></row>
		<row><td>2812</td><td>##IDS_ERROR_2812##</td></row>
		<row><td>2813</td><td>##IDS_ERROR_2813##</td></row>
		<row><td>2814</td><td>##IDS_ERROR_2814##</td></row>
		<row><td>2815</td><td>##IDS_ERROR_2815##</td></row>
		<row><td>2816</td><td>##IDS_ERROR_2816##</td></row>
		<row><td>2817</td><td>##IDS_ERROR_2817##</td></row>
		<row><td>2818</td><td>##IDS_ERROR_2818##</td></row>
		<row><td>2819</td><td>##IDS_ERROR_2819##</td></row>
		<row><td>2820</td><td>##IDS_ERROR_2820##</td></row>
		<row><td>2821</td><td>##IDS_ERROR_2821##</td></row>
		<row><td>2822</td><td>##IDS_ERROR_2822##</td></row>
		<row><td>2823</td><td>##IDS_ERROR_2823##</td></row>
		<row><td>2824</td><td>##IDS_ERROR_2824##</td></row>
		<row><td>2825</td><td>##IDS_ERROR_2825##</td></row>
		<row><td>2826</td><td>##IDS_ERROR_2826##</td></row>
		<row><td>2827</td><td>##IDS_ERROR_2827##</td></row>
		<row><td>2828</td><td>##IDS_ERROR_2828##</td></row>
		<row><td>2829</td><td>##IDS_ERROR_2829##</td></row>
		<row><td>2830</td><td>##IDS_ERROR_2830##</td></row>
		<row><td>2831</td><td>##IDS_ERROR_2831##</td></row>
		<row><td>2832</td><td>##IDS_ERROR_2832##</td></row>
		<row><td>2833</td><td>##IDS_ERROR_2833##</td></row>
		<row><td>2834</td><td>##IDS_ERROR_2834##</td></row>
		<row><td>2835</td><td>##IDS_ERROR_2835##</td></row>
		<row><td>2836</td><td>##IDS_ERROR_2836##</td></row>
		<row><td>2837</td><td>##IDS_ERROR_2837##</td></row>
		<row><td>2838</td><td>##IDS_ERROR_2838##</td></row>
		<row><td>2839</td><td>##IDS_ERROR_2839##</td></row>
		<row><td>2840</td><td>##IDS_ERROR_2840##</td></row>
		<row><td>2841</td><td>##IDS_ERROR_2841##</td></row>
		<row><td>2842</td><td>##IDS_ERROR_2842##</td></row>
		<row><td>2843</td><td>##IDS_ERROR_2843##</td></row>
		<row><td>2844</td><td>##IDS_ERROR_2844##</td></row>
		<row><td>2845</td><td>##IDS_ERROR_2845##</td></row>
		<row><td>2846</td><td>##IDS_ERROR_2846##</td></row>
		<row><td>2847</td><td>##IDS_ERROR_2847##</td></row>
		<row><td>2848</td><td>##IDS_ERROR_2848##</td></row>
		<row><td>2849</td><td>##IDS_ERROR_2849##</td></row>
		<row><td>2850</td><td>##IDS_ERROR_2850##</td></row>
		<row><td>2851</td><td>##IDS_ERROR_2851##</td></row>
		<row><td>2852</td><td>##IDS_ERROR_2852##</td></row>
		<row><td>2853</td><td>##IDS_ERROR_2853##</td></row>
		<row><td>2854</td><td>##IDS_ERROR_2854##</td></row>
		<row><td>2855</td><td>##IDS_ERROR_2855##</td></row>
		<row><td>2856</td><td>##IDS_ERROR_2856##</td></row>
		<row><td>2857</td><td>##IDS_ERROR_2857##</td></row>
		<row><td>2858</td><td>##IDS_ERROR_2858##</td></row>
		<row><td>2859</td><td>##IDS_ERROR_2859##</td></row>
		<row><td>2860</td><td>##IDS_ERROR_2860##</td></row>
		<row><td>2861</td><td>##IDS_ERROR_2861##</td></row>
		<row><td>2862</td><td>##IDS_ERROR_2862##</td></row>
		<row><td>2863</td><td>##IDS_ERROR_2863##</td></row>
		<row><td>2864</td><td>##IDS_ERROR_2864##</td></row>
		<row><td>2865</td><td>##IDS_ERROR_2865##</td></row>
		<row><td>2866</td><td>##IDS_ERROR_2866##</td></row>
		<row><td>2867</td><td>##IDS_ERROR_2867##</td></row>
		<row><td>2868</td><td>##IDS_ERROR_2868##</td></row>
		<row><td>2869</td><td>##IDS_ERROR_2869##</td></row>
		<row><td>2870</td><td>##IDS_ERROR_2870##</td></row>
		<row><td>2871</td><td>##IDS_ERROR_2871##</td></row>
		<row><td>2872</td><td>##IDS_ERROR_2872##</td></row>
		<row><td>2873</td><td>##IDS_ERROR_2873##</td></row>
		<row><td>2874</td><td>##IDS_ERROR_2874##</td></row>
		<row><td>2875</td><td>##IDS_ERROR_2875##</td></row>
		<row><td>2876</td><td>##IDS_ERROR_2876##</td></row>
		<row><td>2877</td><td>##IDS_ERROR_2877##</td></row>
		<row><td>2878</td><td>##IDS_ERROR_2878##</td></row>
		<row><td>2879</td><td>##IDS_ERROR_2879##</td></row>
		<row><td>2880</td><td>##IDS_ERROR_2880##</td></row>
		<row><td>2881</td><td>##IDS_ERROR_2881##</td></row>
		<row><td>2882</td><td>##IDS_ERROR_2882##</td></row>
		<row><td>2883</td><td>##IDS_ERROR_2883##</td></row>
		<row><td>2884</td><td>##IDS_ERROR_2884##</td></row>
		<row><td>2885</td><td>##IDS_ERROR_2885##</td></row>
		<row><td>2886</td><td>##IDS_ERROR_2886##</td></row>
		<row><td>2887</td><td>##IDS_ERROR_2887##</td></row>
		<row><td>2888</td><td>##IDS_ERROR_2888##</td></row>
		<row><td>2889</td><td>##IDS_ERROR_2889##</td></row>
		<row><td>2890</td><td>##IDS_ERROR_2890##</td></row>
		<row><td>2891</td><td>##IDS_ERROR_2891##</td></row>
		<row><td>2892</td><td>##IDS_ERROR_2892##</td></row>
		<row><td>2893</td><td>##IDS_ERROR_2893##</td></row>
		<row><td>2894</td><td>##IDS_ERROR_2894##</td></row>
		<row><td>2895</td><td>##IDS_ERROR_2895##</td></row>
		<row><td>2896</td><td>##IDS_ERROR_2896##</td></row>
		<row><td>2897</td><td>##IDS_ERROR_2897##</td></row>
		<row><td>2898</td><td>##IDS_ERROR_2898##</td></row>
		<row><td>2899</td><td>##IDS_ERROR_2899##</td></row>
		<row><td>2901</td><td>##IDS_ERROR_2901##</td></row>
		<row><td>2902</td><td>##IDS_ERROR_2902##</td></row>
		<row><td>2903</td><td>##IDS_ERROR_2903##</td></row>
		<row><td>2904</td><td>##IDS_ERROR_2904##</td></row>
		<row><td>2905</td><td>##IDS_ERROR_2905##</td></row>
		<row><td>2906</td><td>##IDS_ERROR_2906##</td></row>
		<row><td>2907</td><td>##IDS_ERROR_2907##</td></row>
		<row><td>2908</td><td>##IDS_ERROR_2908##</td></row>
		<row><td>2909</td><td>##IDS_ERROR_2909##</td></row>
		<row><td>2910</td><td>##IDS_ERROR_2910##</td></row>
		<row><td>2911</td><td>##IDS_ERROR_2911##</td></row>
		<row><td>2912</td><td>##IDS_ERROR_2912##</td></row>
		<row><td>2919</td><td>##IDS_ERROR_2919##</td></row>
		<row><td>2920</td><td>##IDS_ERROR_2920##</td></row>
		<row><td>2924</td><td>##IDS_ERROR_2924##</td></row>
		<row><td>2927</td><td>##IDS_ERROR_2927##</td></row>
		<row><td>2928</td><td>##IDS_ERROR_2928##</td></row>
		<row><td>2929</td><td>##IDS_ERROR_2929##</td></row>
		<row><td>2932</td><td>##IDS_ERROR_2932##</td></row>
		<row><td>2933</td><td>##IDS_ERROR_2933##</td></row>
		<row><td>2934</td><td>##IDS_ERROR_2934##</td></row>
		<row><td>2935</td><td>##IDS_ERROR_2935##</td></row>
		<row><td>2936</td><td>##IDS_ERROR_2936##</td></row>
		<row><td>2937</td><td>##IDS_ERROR_2937##</td></row>
		<row><td>2938</td><td>##IDS_ERROR_2938##</td></row>
		<row><td>2939</td><td>##IDS_ERROR_2939##</td></row>
		<row><td>2940</td><td>##IDS_ERROR_2940##</td></row>
		<row><td>2941</td><td>##IDS_ERROR_2941##</td></row>
		<row><td>2942</td><td>##IDS_ERROR_2942##</td></row>
		<row><td>2943</td><td>##IDS_ERROR_2943##</td></row>
		<row><td>2944</td><td>##IDS_ERROR_2944##</td></row>
		<row><td>2945</td><td>##IDS_ERROR_2945##</td></row>
		<row><td>3001</td><td>##IDS_ERROR_3001##</td></row>
		<row><td>3002</td><td>##IDS_ERROR_3002##</td></row>
		<row><td>32</td><td>##IDS_ERROR_20##</td></row>
		<row><td>33</td><td>##IDS_ERROR_21##</td></row>
		<row><td>4</td><td>##IDS_ERROR_3##</td></row>
		<row><td>5</td><td>##IDS_ERROR_4##</td></row>
		<row><td>7</td><td>##IDS_ERROR_5##</td></row>
		<row><td>8</td><td>##IDS_ERROR_6##</td></row>
		<row><td>9</td><td>##IDS_ERROR_7##</td></row>
	</table>

	<table name="EventMapping">
		<col key="yes" def="s72">Dialog_</col>
		<col key="yes" def="s50">Control_</col>
		<col key="yes" def="s50">Event</col>
		<col def="s50">Attribute</col>
		<row><td>CustomSetup</td><td>ItemDescription</td><td>SelectionDescription</td><td>Text</td></row>
		<row><td>CustomSetup</td><td>Location</td><td>SelectionPath</td><td>Text</td></row>
		<row><td>CustomSetup</td><td>Size</td><td>SelectionSize</td><td>Text</td></row>
		<row><td>SetupInitialization</td><td>ActionData</td><td>ActionData</td><td>Text</td></row>
		<row><td>SetupInitialization</td><td>ActionText</td><td>ActionText</td><td>Text</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>AdminInstallFinalize</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>InstallFiles</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>MoveFiles</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>RemoveFiles</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>RemoveRegistryValues</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>SetProgress</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>UnmoveFiles</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>WriteIniValues</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionProgress95</td><td>WriteRegistryValues</td><td>Progress</td></row>
		<row><td>SetupProgress</td><td>ActionText</td><td>ActionText</td><td>Text</td></row>
	</table>

	<table name="Extension">
		<col key="yes" def="s255">Extension</col>
		<col key="yes" def="s72">Component_</col>
		<col def="S255">ProgId_</col>
		<col def="S64">MIME_</col>
		<col def="s38">Feature_</col>
	</table>

	<table name="Feature">
		<col key="yes" def="s38">Feature</col>
		<col def="S38">Feature_Parent</col>
		<col def="L64">Title</col>
		<col def="L255">Description</col>
		<col def="I2">Display</col>
		<col def="i2">Level</col>
		<col def="S72">Directory_</col>
		<col def="i2">Attributes</col>
		<col def="S255">ISReleaseFlags</col>
		<col def="S255">ISComments</col>
		<col def="S255">ISFeatureCabName</col>
		<col def="S255">ISProFeatureName</col>
		<row><td>AlwaysInstall</td><td/><td>##DN_AlwaysInstall##</td><td>Enter the description for this feature here.</td><td>0</td><td>1</td><td>INSTALLDIR</td><td>16</td><td/><td>Enter comments regarding this feature here.</td><td/><td/></row>
	</table>

	<table name="FeatureComponents">
		<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="s72">Component_</col>
		<row><td>AlwaysInstall</td><td>Component</td></row>
		<row><td>AlwaysInstall</td><td>Component2</td></row>
		<row><td>AlwaysInstall</td><td>ISX_DEFAULTCOMPONENT</td></row>
		<row><td>AlwaysInstall</td><td>NewComponent1</td></row>
		<row><td>AlwaysInstall</td><td>NewComponent2</td></row>
		<row><td>AlwaysInstall</td><td>Tween.XmlSerializers.dll</td></row>
	</table>

	<table name="File">
		<col key="yes" def="s72">File</col>
		<col def="s72">Component_</col>
		<col def="s255">FileName</col>
		<col def="i4">FileSize</col>
		<col def="S72">Version</col>
		<col def="S20">Language</col>
		<col def="I2">Attributes</col>
		<col def="i2">Sequence</col>
		<col def="S255">ISBuildSourcePath</col>
		<col def="I4">ISAttributes</col>
		<col def="S72">ISComponentSubFolder_</col>
		<row><td>File</td><td>Component</td><td>Tween.ローカライズされたリソース</td><td>0</td><td/><td/><td>0</td><td>1</td><td>&lt;Tween&gt;|LocalizedResourceDlls</td><td>3</td><td/></row>
		<row><td>File2</td><td>Component2</td><td>Tween.プライマリ出力</td><td>0</td><td/><td/><td>0</td><td>1</td><td>&lt;Tween&gt;|Built</td><td>3</td><td/></row>
		<row><td>roaming</td><td>ISX_DEFAULTCOMPONENT</td><td>roaming</td><td>0</td><td/><td/><td/><td>1</td><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\bin\x86\Debug\roaming</td><td>1</td><td/></row>
		<row><td>tween.xmlserializers.dll</td><td>Tween.XmlSerializers.dll</td><td>TWEENX~1.DLL|Tween.XmlSerializers.dll</td><td>0</td><td/><td/><td/><td>1</td><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\bin\x86\Release\Tween.XmlSerializers.dll</td><td>1</td><td/></row>
	</table>

	<table name="FileSFPCatalog">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="s255">SFPCatalog_</col>
	</table>

	<table name="Font">
		<col key="yes" def="s72">File_</col>
		<col def="S128">FontTitle</col>
	</table>

	<table name="ISAssistantTag">
		<col key="yes" def="s72">Tag</col>
		<col def="S255">Data</col>
		<row><td>PROJECT_ASSISTANT_DEFAULT_FEATURE</td><td>AlwaysInstall</td></row>
		<row><td>PROJECT_ASSISTANT_FEATURES</td><td>NonSelectable</td></row>
	</table>

	<table name="ISBillBoard">
		<col key="yes" def="s72">ISBillboard</col>
		<col def="i2">Duration</col>
		<col def="i2">Origin</col>
		<col def="i2">X</col>
		<col def="i2">Y</col>
		<col def="i2">Effect</col>
		<col def="i2">Sequence</col>
		<col def="i2">Target</col>
		<col def="S72">Color</col>
		<col def="S72">Style</col>
		<col def="S72">Font</col>
		<col def="L72">Title</col>
		<col def="S72">DisplayName</col>
	</table>

	<table name="ISCEApp">
		<col key="yes" def="s50">AppKey</col>
		<col def="s50">AppName</col>
		<col def="s200">CompanyName</col>
		<col def="s50">DefDir</col>
		<col def="S255">IconPath</col>
		<col def="I4">IconIndex</col>
		<col def="S255">DeviceFile</col>
		<col def="s50">DesktopTargetDir</col>
		<col def="S255">Description</col>
		<col def="i2">DeleteMedia</col>
		<col def="I4">InstallNetCF</col>
		<col def="I4">InstallSQLServer</col>
		<col def="I4">InstallSQLClient</col>
		<col def="I4">InstallSQLDev</col>
		<col def="S255">PreXML</col>
		<col def="S255">PostXML</col>
		<col def="I2">NoUninstall</col>
		<col def="S255">SPCFile</col>
		<col def="S255">PVKFile</col>
		<col def="I4">Attributes</col>
		<col def="S255">RawDeviceFile</col>
		<col def="S72">Component_</col>
		<col def="I4">InstallNetCF2</col>
		<col def="I4">InstallSQLServer2</col>
		<col def="I4">InstallSQLClient2</col>
		<col def="I4">InstallSQLDev2</col>
		<col def="S255">SPCPwd</col>
	</table>

	<table name="ISCEDir">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">DirKey</col>
		<col def="s50">DirParent</col>
		<col def="s255">DirValue</col>
	</table>

	<table name="ISCEFile">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">FileKey</col>
		<col def="s255">Name</col>
		<col def="s50">Destination</col>
		<col def="s255">Source</col>
		<col def="i4">Processor</col>
		<col def="i4">Platform</col>
		<col def="i4">CopyOption</col>
		<col def="i4">FileOption</col>
		<col def="I4">AdvancedOptions</col>
	</table>

	<table name="ISCEFileExt">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">ExtKey</col>
		<col def="s50">FileKey</col>
		<col def="S255">Description</col>
		<col def="s50">Extension</col>
		<col def="i4">IconIndex</col>
	</table>

	<table name="ISCEInstall">
		<col key="yes" def="s255">CEInstallKey</col>
		<col def="s255">CEAppName</col>
		<col def="s255">CEDesktopDir</col>
		<col def="s255">CEIniFileKey</col>
		<col def="s0">CECabs</col>
		<col def="s0">CEIcoFile</col>
		<col def="i2">DeleteMedia</col>
		<col def="S38">Component_</col>
	</table>

	<table name="ISCEOtherAppCABs">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">FileKey</col>
		<col def="s255">BuildSourcePath</col>
	</table>

	<table name="ISCERedist">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="S50">Name</col>
		<col def="I4">Platforms</col>
	</table>

	<table name="ISCERegistry">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">RegKey</col>
		<col def="i4">Root</col>
		<col def="s255">Key</col>
		<col def="S255">Name</col>
		<col def="S0">Value</col>
		<col def="i4">Processor</col>
		<col def="i4">Platform</col>
		<col def="i4">Overwrite</col>
	</table>

	<table name="ISCESetupFile">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">SetupFileKey</col>
		<col def="s255">Name</col>
		<col def="s255">Source</col>
		<col def="i4">Processor</col>
		<col def="i4">Platform</col>
	</table>

	<table name="ISCEShtCut">
		<col key="yes" def="s50">AppKey</col>
		<col key="yes" def="s50">ShtCutKey</col>
		<col def="s255">DisplayName</col>
		<col def="s255">Destination</col>
		<col def="s50">Target</col>
		<col def="i4">Platform</col>
		<col def="S255">StartScreenIcon</col>
	</table>

	<table name="ISChainPackage">
		<col key="yes" def="s72">Package</col>
		<col def="S255">SourcePath</col>
		<col def="S72">ProductCode</col>
		<col def="i2">Order</col>
		<col def="i4">Options</col>
		<col def="S255">InstallCondition</col>
		<col def="S255">RemoveCondition</col>
		<col def="S0">InstallProperties</col>
		<col def="S0">RemoveProperties</col>
		<col def="S255">ISReleaseFlags</col>
		<col def="S72">DisplayName</col>
	</table>

	<table name="ISChainPackageData">
		<col key="yes" def="s72">Package_</col>
		<col key="yes" def="s72">File</col>
		<col def="s50">FilePath</col>
		<col def="I4">Options</col>
		<col def="V0">Data</col>
		<col def="S255">ISBuildSourcePath</col>
	</table>

	<table name="ISClrWrap">
		<col key="yes" def="s72">Action_</col>
		<col key="yes" def="s72">Name</col>
		<col def="S0">Value</col>
	</table>

	<table name="ISComCatalogAttribute">
		<col key="yes" def="s72">ISComCatalogObject_</col>
		<col key="yes" def="s255">ItemName</col>
		<col def="S0">ItemValue</col>
	</table>

	<table name="ISComCatalogCollection">
		<col key="yes" def="s72">ISComCatalogCollection</col>
		<col def="s72">ISComCatalogObject_</col>
		<col def="s255">CollectionName</col>
	</table>

	<table name="ISComCatalogCollectionObjects">
		<col key="yes" def="s72">ISComCatalogCollection_</col>
		<col key="yes" def="s72">ISComCatalogObject_</col>
	</table>

	<table name="ISComCatalogObject">
		<col key="yes" def="s72">ISComCatalogObject</col>
		<col def="s255">DisplayName</col>
	</table>

	<table name="ISComPlusApplication">
		<col key="yes" def="s72">ISComCatalogObject_</col>
		<col def="S255">ComputerName</col>
		<col def="s72">Component_</col>
		<col def="I2">ISAttributes</col>
		<col def="S0">DepFiles</col>
	</table>

	<table name="ISComPlusApplicationDLL">
		<col key="yes" def="s72">ISComPlusApplicationDLL</col>
		<col def="s72">ISComPlusApplication_</col>
		<col def="s72">ISComCatalogObject_</col>
		<col def="s0">CLSID</col>
		<col def="S0">ProgId</col>
		<col def="S0">DLL</col>
		<col def="S0">AlterDLL</col>
	</table>

	<table name="ISComPlusProxy">
		<col key="yes" def="s72">ISComPlusProxy</col>
		<col def="s72">ISComPlusApplication_</col>
		<col def="S72">Component_</col>
		<col def="I2">ISAttributes</col>
		<col def="S0">DepFiles</col>
	</table>

	<table name="ISComPlusProxyDepFile">
		<col key="yes" def="s72">ISComPlusApplication_</col>
		<col key="yes" def="s72">File_</col>
		<col def="S0">ISPath</col>
	</table>

	<table name="ISComPlusProxyFile">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="s72">ISComPlusApplicationDLL_</col>
	</table>

	<table name="ISComPlusServerDepFile">
		<col key="yes" def="s72">ISComPlusApplication_</col>
		<col key="yes" def="s72">File_</col>
		<col def="S0">ISPath</col>
	</table>

	<table name="ISComPlusServerFile">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="s72">ISComPlusApplicationDLL_</col>
	</table>

	<table name="ISComponentExtended">
		<col key="yes" def="s72">Component_</col>
		<col def="I4">OS</col>
		<col def="S0">Language</col>
		<col def="s72">FilterProperty</col>
		<col def="I4">Platforms</col>
		<col def="S0">FTPLocation</col>
		<col def="S0">HTTPLocation</col>
		<col def="S0">Miscellaneous</col>
		<row><td>Component</td><td/><td/><td>_F4E70822_CCFD_4FB3_B9EB_8E3B847950A6_FILTER</td><td/><td/><td/><td/></row>
		<row><td>Component2</td><td/><td/><td>_2DF72AE0_09A8_4876_BFF4_F1FB31CE32FC_FILTER</td><td/><td/><td/><td/></row>
		<row><td>ISX_DEFAULTCOMPONENT</td><td/><td/><td>_1F3A5292_A620_44FF_8CB3_193FF4BA0B2D_FILTER</td><td/><td/><td/><td/></row>
		<row><td>NewComponent1</td><td/><td/><td>_A742E6C9_2A61_475A_AB73_A36B89F91139_FILTER</td><td/><td/><td/><td/></row>
		<row><td>NewComponent2</td><td/><td/><td>_ABBEF2B5_1516_43CA_8E06_EADDCB46D095_FILTER</td><td/><td/><td/><td/></row>
		<row><td>Tween.XmlSerializers.dll</td><td/><td/><td>_9EC04E01_E2CE_4847_85F9_709E54637246_FILTER</td><td/><td/><td/><td/></row>
	</table>

	<table name="ISCustomActionReference">
		<col key="yes" def="s72">Action_</col>
		<col def="S0">Description</col>
		<col def="S255">FileType</col>
		<col def="S255">ISCAReferenceFilePath</col>
	</table>

	<table name="ISDIMDependency">
		<col key="yes" def="s72">ISDIMReference_</col>
		<col def="s255">RequiredUUID</col>
		<col def="S255">RequiredMajorVersion</col>
		<col def="S255">RequiredMinorVersion</col>
		<col def="S255">RequiredBuildVersion</col>
		<col def="S255">RequiredRevisionVersion</col>
	</table>

	<table name="ISDIMReference">
		<col key="yes" def="s72">ISDIMReference</col>
		<col def="S0">ISBuildSourcePath</col>
	</table>

	<table name="ISDIMReferenceDependencies">
		<col key="yes" def="s72">ISDIMReference_Parent</col>
		<col key="yes" def="s72">ISDIMDependency_</col>
	</table>

	<table name="ISDIMVariable">
		<col key="yes" def="s72">ISDIMVariable</col>
		<col def="s72">ISDIMReference_</col>
		<col def="s0">Name</col>
		<col def="S0">NewValue</col>
		<col def="I4">Type</col>
	</table>

	<table name="ISDLLWrapper">
		<col key="yes" def="s72">EntryPoint</col>
		<col def="I4">Type</col>
		<col def="s0">Source</col>
		<col def="s255">Target</col>
	</table>

	<table name="ISDRMFile">
		<col key="yes" def="s72">ISDRMFile</col>
		<col def="S72">File_</col>
		<col def="S72">ISDRMLicense_</col>
		<col def="s255">Shell</col>
	</table>

	<table name="ISDRMFileAttribute">
		<col key="yes" def="s72">ISDRMFile_</col>
		<col key="yes" def="s72">Property</col>
		<col def="S0">Value</col>
	</table>

	<table name="ISDRMLicense">
		<col key="yes" def="s72">ISDRMLicense</col>
		<col def="S255">Description</col>
		<col def="S50">ProjectVersion</col>
		<col def="I4">Attributes</col>
		<col def="S255">LicenseNumber</col>
		<col def="S255">RequestCode</col>
		<col def="S255">ResponseCode</col>
	</table>

	<table name="ISDependency">
		<col key="yes" def="S50">ISDependency</col>
		<col def="I2">Exclude</col>
		<row><td>SYSTEM.RUNTIME.CACHING.DLL</td><td>1</td></row>
		<row><td>TWEEN.EXE</td><td>1</td></row>
	</table>

	<table name="ISDisk1File">
		<col key="yes" def="s72">ISDisk1File</col>
		<col def="s255">ISBuildSourcePath</col>
		<col def="I4">Disk</col>
	</table>

	<table name="ISDynamicFile">
		<col key="yes" def="s72">Component_</col>
		<col key="yes" def="s255">SourceFolder</col>
		<col def="I2">IncludeFlags</col>
		<col def="S0">IncludeFiles</col>
		<col def="S0">ExcludeFiles</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISFeatureDIMReferences">
		<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="s72">ISDIMReference_</col>
	</table>

	<table name="ISFeatureMergeModuleExcludes">
		<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="s255">ModuleID</col>
		<col key="yes" def="i2">Language</col>
	</table>

	<table name="ISFeatureMergeModules">
		<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="s255">ISMergeModule_</col>
		<col key="yes" def="i2">Language_</col>
	</table>

	<table name="ISFeatureSetupPrerequisites">
		<col key="yes" def="s38">Feature_</col>
		<col key="yes" def="s72">ISSetupPrerequisites_</col>
	</table>

	<table name="ISFileManifests">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="s72">Manifest_</col>
	</table>

	<table name="ISIISItem">
		<col key="yes" def="s72">ISIISItem</col>
		<col def="S72">ISIISItem_Parent</col>
		<col def="L255">DisplayName</col>
		<col def="i4">Type</col>
		<col def="S72">Component_</col>
	</table>

	<table name="ISIISProperty">
		<col key="yes" def="s72">ISIISProperty</col>
		<col key="yes" def="s72">ISIISItem_</col>
		<col def="S0">Schema</col>
		<col def="S255">FriendlyName</col>
		<col def="I4">MetaDataProp</col>
		<col def="I4">MetaDataType</col>
		<col def="I4">MetaDataUserType</col>
		<col def="I4">MetaDataAttributes</col>
		<col def="L0">MetaDataValue</col>
		<col def="I4">Order</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISInstallScriptAction">
		<col key="yes" def="s72">EntryPoint</col>
		<col def="I4">Type</col>
		<col def="s72">Source</col>
		<col def="S255">Target</col>
	</table>

	<table name="ISLanguage">
		<col key="yes" def="s50">ISLanguage</col>
		<col def="I2">Included</col>
		<row><td>1033</td><td>0</td></row>
		<row><td>1041</td><td>1</td></row>
	</table>

	<table name="ISLinkerLibrary">
		<col key="yes" def="s72">ISLinkerLibrary</col>
		<col def="s255">Library</col>
		<col def="i4">Order</col>
		<row><td>isrt.obl</td><td>isrt.obl</td><td>2</td></row>
		<row><td>iswi.obl</td><td>iswi.obl</td><td>1</td></row>
	</table>

	<table name="ISLocalControl">
		<col key="yes" def="s72">Dialog_</col>
		<col key="yes" def="s50">Control_</col>
		<col key="yes" def="s50">ISLanguage_</col>
		<col def="I4">Attributes</col>
		<col def="I2">X</col>
		<col def="I2">Y</col>
		<col def="I2">Width</col>
		<col def="I2">Height</col>
		<col def="S72">Binary_</col>
		<col def="S255">ISBuildSourcePath</col>
	</table>

	<table name="ISLocalDialog">
		<col key="yes" def="S50">Dialog_</col>
		<col key="yes" def="S50">ISLanguage_</col>
		<col def="I4">Attributes</col>
		<col def="S72">TextStyle_</col>
		<col def="i2">Width</col>
		<col def="i2">Height</col>
	</table>

	<table name="ISLocalRadioButton">
		<col key="yes" def="s72">Property</col>
		<col key="yes" def="i2">Order</col>
		<col key="yes" def="s50">ISLanguage_</col>
		<col def="i2">X</col>
		<col def="i2">Y</col>
		<col def="i2">Width</col>
		<col def="i2">Height</col>
	</table>

	<table name="ISLockPermissions">
		<col key="yes" def="s72">LockObject</col>
		<col key="yes" def="s32">Table</col>
		<col key="yes" def="S255">Domain</col>
		<col key="yes" def="s255">User</col>
		<col def="I4">Permission</col>
		<col def="I4">Attributes</col>
	</table>

	<table name="ISLogicalDisk">
		<col key="yes" def="i2">DiskId</col>
		<col key="yes" def="s255">ISProductConfiguration_</col>
		<col key="yes" def="s255">ISRelease_</col>
		<col def="i2">LastSequence</col>
		<col def="L64">DiskPrompt</col>
		<col def="S255">Cabinet</col>
		<col def="S32">VolumeLabel</col>
		<col def="S32">Source</col>
	</table>

	<table name="ISLogicalDiskFeatures">
		<col key="yes" def="i2">ISLogicalDisk_</col>
		<col key="yes" def="s255">ISProductConfiguration_</col>
		<col key="yes" def="s255">ISRelease_</col>
		<col key="yes" def="S38">Feature_</col>
		<col def="i2">Sequence</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISMergeModule">
		<col key="yes" def="s255">ISMergeModule</col>
		<col key="yes" def="i2">Language</col>
		<col def="s255">Name</col>
		<col def="S255">Destination</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISMergeModuleCfgValues">
		<col key="yes" def="s255">ISMergeModule_</col>
		<col key="yes" def="i2">Language_</col>
		<col key="yes" def="s72">ModuleConfiguration_</col>
		<col def="L0">Value</col>
		<col def="i2">Format</col>
		<col def="L255">Type</col>
		<col def="L255">ContextData</col>
		<col def="L255">DefaultValue</col>
		<col def="I2">Attributes</col>
		<col def="L255">DisplayName</col>
		<col def="L255">Description</col>
		<col def="L255">HelpLocation</col>
		<col def="L255">HelpKeyword</col>
	</table>

	<table name="ISObject">
		<col key="yes" def="s50">ObjectName</col>
		<col def="s15">Language</col>
	</table>

	<table name="ISObjectProperty">
		<col key="yes" def="S50">ObjectName</col>
		<col key="yes" def="S50">Property</col>
		<col def="S0">Value</col>
		<col def="I2">IncludeInBuild</col>
	</table>

	<table name="ISPalmApp">
		<col key="yes" def="s72">PalmApp</col>
		<col key="yes" def="s72">Component</col>
	</table>

	<table name="ISPalmAppFile">
		<col key="yes" def="s72">PalmApp</col>
		<col key="yes" def="s72">FileKey</col>
		<col def="i4">Destination</col>
	</table>

	<table name="ISPatchConfigImage">
		<col key="yes" def="S72">PatchConfiguration_</col>
		<col key="yes" def="s72">UpgradedImage_</col>
	</table>

	<table name="ISPatchConfiguration">
		<col key="yes" def="s72">Name</col>
		<col def="i2">CanPCDiffer</col>
		<col def="i2">CanPVDiffer</col>
		<col def="i2">IncludeWholeFiles</col>
		<col def="i2">LeaveDecompressed</col>
		<col def="i2">OptimizeForSize</col>
		<col def="i2">EnablePatchCache</col>
		<col def="S0">PatchCacheDir</col>
		<col def="i4">Flags</col>
		<col def="S0">PatchGuidsToReplace</col>
		<col def="s0">TargetProductCodes</col>
		<col def="s50">PatchGuid</col>
		<col def="s0">OutputPath</col>
		<col def="i2">MinMsiVersion</col>
		<col def="I4">Attributes</col>
	</table>

	<table name="ISPatchConfigurationProperty">
		<col key="yes" def="S72">ISPatchConfiguration_</col>
		<col key="yes" def="S50">Property</col>
		<col def="S50">Value</col>
	</table>

	<table name="ISPatchExternalFile">
		<col key="yes" def="s50">Name</col>
		<col key="yes" def="s13">ISUpgradedImage_</col>
		<col def="s72">FileKey</col>
		<col def="s255">FilePath</col>
	</table>

	<table name="ISPatchWholeFile">
		<col key="yes" def="s50">UpgradedImage</col>
		<col key="yes" def="s72">FileKey</col>
		<col def="S72">Component</col>
	</table>

	<table name="ISPathVariable">
		<col key="yes" def="s32">ISPathVariable</col>
		<col def="S255">Value</col>
		<col def="S255">TestValue</col>
		<col def="i4">Type</col>
		<row><td>CommonFilesFolder</td><td/><td/><td>1</td></row>
		<row><td>ISPROJECTDIR</td><td/><td/><td>1</td></row>
		<row><td>ISProductFolder</td><td/><td/><td>1</td></row>
		<row><td>ISProjectDataFolder</td><td/><td/><td>1</td></row>
		<row><td>ISProjectFolder</td><td/><td/><td>1</td></row>
		<row><td>ProgramFilesFolder</td><td/><td/><td>1</td></row>
		<row><td>SystemFolder</td><td/><td/><td>1</td></row>
		<row><td>Tween</td><td>Tween\Tween.vbproj</td><td/><td>2</td></row>
		<row><td>WindowsFolder</td><td/><td/><td>1</td></row>
	</table>

	<table name="ISProductConfiguration">
		<col key="yes" def="s72">ISProductConfiguration</col>
		<col def="S255">ProductConfigurationFlags</col>
		<col def="I4">GeneratePackageCode</col>
		<row><td>Express</td><td/><td>1</td></row>
	</table>

	<table name="ISProductConfigurationInstance">
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col key="yes" def="i2">InstanceId</col>
		<col key="yes" def="s72">Property</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISProductConfigurationProperty">
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col key="yes" def="s72">Property</col>
		<col def="L255">Value</col>
		<row><td>Express</td><td>SetupFileName</td><td>SetupEn</td></row>
	</table>

	<table name="ISRelease">
		<col key="yes" def="s72">ISRelease</col>
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col def="s255">BuildLocation</col>
		<col def="s255">PackageName</col>
		<col def="i4">Type</col>
		<col def="s0">SupportedLanguagesUI</col>
		<col def="i4">MsiSourceType</col>
		<col def="i4">ReleaseType</col>
		<col def="s72">Platforms</col>
		<col def="S0">SupportedLanguagesData</col>
		<col def="s6">DefaultLanguage</col>
		<col def="i4">SupportedOSs</col>
		<col def="s50">DiskSize</col>
		<col def="i4">DiskSizeUnit</col>
		<col def="i4">DiskClusterSize</col>
		<col def="S0">ReleaseFlags</col>
		<col def="i4">DiskSpanning</col>
		<col def="S255">SynchMsi</col>
		<col def="s255">MediaLocation</col>
		<col def="S255">URLLocation</col>
		<col def="S255">DigitalURL</col>
		<col def="S255">DigitalPVK</col>
		<col def="S255">DigitalSPC</col>
		<col def="S255">Password</col>
		<col def="S255">VersionCopyright</col>
		<col def="i4">Attributes</col>
		<col def="S255">CDBrowser</col>
		<col def="S255">DotNetBuildConfiguration</col>
		<col def="S255">MsiCommandLine</col>
		<col def="I4">ISSetupPrerequisiteLocation</col>
		<row><td>CD_ROM</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>Default</td><td>0</td><td>1041</td><td>0</td><td>2</td><td>Intel</td><td/><td>1041</td><td>0</td><td>650</td><td>0</td><td>2048</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>75805</td><td/><td/><td/><td>3</td></row>
		<row><td>Custom</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>Default</td><td>2</td><td>1033</td><td>0</td><td>2</td><td>Intel</td><td/><td>1033</td><td>0</td><td>100</td><td>0</td><td>1024</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>75805</td><td/><td/><td/><td>3</td></row>
		<row><td>DVD-10</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>Default</td><td>3</td><td>1033</td><td>0</td><td>2</td><td>Intel</td><td/><td>1033</td><td>0</td><td>8.75</td><td>1</td><td>2048</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>75805</td><td/><td/><td/><td>3</td></row>
		<row><td>DVD-18</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>Default</td><td>3</td><td>1033</td><td>0</td><td>2</td><td>Intel</td><td/><td>1033</td><td>0</td><td>15.83</td><td>1</td><td>2048</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>75805</td><td/><td/><td/><td>3</td></row>
		<row><td>DVD-5</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>Default</td><td>3</td><td>1041</td><td>0</td><td>2</td><td>Intel</td><td/><td>1041</td><td>0</td><td>4.38</td><td>1</td><td>2048</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>75805</td><td/><td/><td/><td>3</td></row>
		<row><td>DVD-9</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>Default</td><td>3</td><td>1033</td><td>0</td><td>2</td><td>Intel</td><td/><td>1033</td><td>0</td><td>7.95</td><td>1</td><td>2048</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>75805</td><td/><td/><td/><td>3</td></row>
		<row><td>SingleImage</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>PackageName</td><td>1</td><td>1041</td><td>0</td><td>1</td><td>Intel</td><td/><td>1041</td><td>0</td><td>0</td><td>0</td><td>0</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>108573</td><td/><td/><td/><td>2</td></row>
		<row><td>WebDeployment</td><td>Express</td><td>&lt;ISProjectDataFolder&gt;</td><td>PackageName</td><td>4</td><td>1033</td><td>2</td><td>1</td><td>Intel</td><td/><td>1033</td><td>0</td><td>0</td><td>0</td><td>0</td><td/><td>0</td><td/><td>MediaLocation</td><td/><td>http://</td><td/><td/><td/><td/><td>124941</td><td/><td/><td/><td>3</td></row>
	</table>

	<table name="ISReleaseASPublishInfo">
		<col key="yes" def="s72">ISRelease_</col>
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col key="yes" def="S0">Property</col>
		<col def="S0">Value</col>
	</table>

	<table name="ISReleaseExtended">
		<col key="yes" def="s72">ISRelease_</col>
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col def="I4">WebType</col>
		<col def="S255">WebURL</col>
		<col def="I4">WebCabSize</col>
		<col def="S255">OneClickCabName</col>
		<col def="S255">OneClickHtmlName</col>
		<col def="S255">WebLocalCachePath</col>
		<col def="I4">EngineLocation</col>
		<col def="S255">Win9xMsiUrl</col>
		<col def="S255">WinNTMsiUrl</col>
		<col def="I4">ISEngineLocation</col>
		<col def="S255">ISEngineURL</col>
		<col def="I4">OneClickTargetBrowser</col>
		<col def="S255">DigitalCertificateIdNS</col>
		<col def="S255">DigitalCertificateDBaseNS</col>
		<col def="S255">DigitalCertificatePasswordNS</col>
		<col def="I4">DotNetRedistLocation</col>
		<col def="S255">DotNetRedistURL</col>
		<col def="I4">DotNetVersion</col>
		<col def="S255">DotNetBaseLanguage</col>
		<col def="S0">DotNetLangaugePacks</col>
		<col def="S255">DotNetFxCmdLine</col>
		<col def="S255">DotNetLangPackCmdLine</col>
		<col def="S50">JSharpCmdLine</col>
		<col def="I4">Attributes</col>
		<col def="I4">JSharpRedistLocation</col>
		<col def="I4">MsiEngineVersion</col>
		<col def="S255">WinMsi30Url</col>
		<col def="S255">CertPassword</col>
		<row><td>CD_ROM</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>0</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1041</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>Custom</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>0</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1033</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>DVD-10</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>0</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1033</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>DVD-18</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>0</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1033</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>DVD-5</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>0</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1041</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>DVD-9</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>0</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1033</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>SingleImage</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>install</td><td>install</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>1</td><td>http://www.installengine.com/Msiengine20</td><td>http://www.installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>0</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1041</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
		<row><td>WebDeployment</td><td>Express</td><td>0</td><td>http://</td><td>0</td><td>setup</td><td>Default</td><td>[LocalAppDataFolder]Downloaded Installations</td><td>2</td><td>http://www.Installengine.com/Msiengine20</td><td>http://www.Installengine.com/Msiengine20</td><td>0</td><td>http://www.installengine.com/cert05/isengine</td><td>2</td><td/><td/><td/><td>3</td><td>http://www.installengine.com/cert05/dotnetfx</td><td>0</td><td>1033</td><td/><td/><td/><td/><td/><td>3</td><td/><td>http://www.installengine.com/Msiengine30</td><td/></row>
	</table>

	<table name="ISReleaseProperty">
		<col key="yes" def="s72">ISRelease_</col>
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col key="yes" def="s72">Name</col>
		<col def="s0">Value</col>
	</table>

	<table name="ISReleasePublishInfo">
		<col key="yes" def="s72">ISRelease_</col>
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col def="S255">Repository</col>
		<col def="S255">DisplayName</col>
		<col def="S255">Publisher</col>
		<col def="S255">Description</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISSQLConnection">
		<col key="yes" def="s72">ISSQLConnection</col>
		<col def="s255">Server</col>
		<col def="s255">Database</col>
		<col def="s255">UserName</col>
		<col def="s255">Password</col>
		<col def="s255">Authentication</col>
		<col def="i2">Attributes</col>
		<col def="i2">Order</col>
		<col def="S0">Comments</col>
		<col def="I4">CmdTimeout</col>
		<col def="S0">BatchSeparator</col>
		<col def="S0">ScriptVersion_Table</col>
		<col def="S0">ScriptVersion_Column</col>
	</table>

	<table name="ISSQLConnectionDBServer">
		<col key="yes" def="s72">ISSQLConnectionDBServer</col>
		<col key="yes" def="s72">ISSQLConnection_</col>
		<col key="yes" def="s72">ISSQLDBMetaData_</col>
		<col def="i2">Order</col>
	</table>

	<table name="ISSQLConnectionScript">
		<col key="yes" def="s72">ISSQLConnection_</col>
		<col key="yes" def="s72">ISSQLScriptFile_</col>
		<col def="i2">Order</col>
	</table>

	<table name="ISSQLDBMetaData">
		<col key="yes" def="s72">ISSQLDBMetaData</col>
		<col def="S0">DisplayName</col>
		<col def="S0">AdoDriverName</col>
		<col def="S0">AdoCxnDriver</col>
		<col def="S0">AdoCxnServer</col>
		<col def="S0">AdoCxnDatabase</col>
		<col def="S0">AdoCxnUserID</col>
		<col def="S0">AdoCxnPassword</col>
		<col def="S0">AdoCxnWindowsSecurity</col>
		<col def="S0">AdoCxnNetLibrary</col>
		<col def="S0">TestDatabaseCmd</col>
		<col def="S0">TestTableCmd</col>
		<col def="S0">VersionInfoCmd</col>
		<col def="S0">VersionBeginToken</col>
		<col def="S0">VersionEndToken</col>
		<col def="S0">LocalInstanceNames</col>
		<col def="S0">CreateDbCmd</col>
		<col def="S0">SwitchDbCmd</col>
		<col def="I4">ISAttributes</col>
		<col def="S0">TestTableCmd2</col>
		<col def="S0">WinAuthentUserId</col>
		<col def="S0">DsnODBCName</col>
		<col def="S0">AdoCxnPort</col>
		<col def="S0">AdoCxnAdditional</col>
		<col def="S0">QueryDatabasesCmd</col>
		<col def="S0">CreateTableCmd</col>
		<col def="S0">InsertRecordCmd</col>
		<col def="S0">SelectTableCmd</col>
		<col def="S0">ScriptVersion_Table</col>
		<col def="S0">ScriptVersion_Column</col>
		<col def="S0">ScriptVersion_ColumnType</col>
	</table>

	<table name="ISSQLRequirement">
		<col key="yes" def="s72">ISSQLRequirement</col>
		<col key="yes" def="s72">ISSQLConnection_</col>
		<col def="S15">MajorVersion</col>
		<col def="S25">ServicePackLevel</col>
		<col def="i4">Attributes</col>
		<col def="S72">ISSQLConnectionDBServer_</col>
	</table>

	<table name="ISSQLScriptError">
		<col key="yes" def="i4">ErrNumber</col>
		<col key="yes" def="S72">ISSQLScriptFile_</col>
		<col def="i2">ErrHandling</col>
		<col def="L255">Message</col>
		<col def="i2">Attributes</col>
	</table>

	<table name="ISSQLScriptFile">
		<col key="yes" def="s72">ISSQLScriptFile</col>
		<col def="s72">Component_</col>
		<col def="i2">Scheduling</col>
		<col def="L255">InstallText</col>
		<col def="L255">UninstallText</col>
		<col def="S0">ISBuildSourcePath</col>
		<col def="S0">Comments</col>
		<col def="i2">ErrorHandling</col>
		<col def="i2">Attributes</col>
		<col def="S15">Version</col>
		<col def="S255">Condition</col>
	</table>

	<table name="ISSQLScriptImport">
		<col key="yes" def="s72">ISSQLScriptFile_</col>
		<col def="S255">Server</col>
		<col def="S255">Database</col>
		<col def="S255">UserName</col>
		<col def="S255">Password</col>
		<col def="i4">Authentication</col>
		<col def="S0">IncludeTables</col>
		<col def="S0">ExcludeTables</col>
		<col def="i4">Attributes</col>
	</table>

	<table name="ISSQLScriptReplace">
		<col key="yes" def="s72">ISSQLScriptReplace</col>
		<col key="yes" def="s72">ISSQLScriptFile_</col>
		<col def="S0">Search</col>
		<col def="S0">Replace</col>
		<col def="i4">Attributes</col>
	</table>

	<table name="ISScriptFile">
		<col key="yes" def="s255">ISScriptFile</col>
	</table>

	<table name="ISSelfReg">
		<col key="yes" def="s72">FileKey</col>
		<col def="I2">Cost</col>
		<col def="I2">Order</col>
		<col def="S50">CmdLine</col>
	</table>

	<table name="ISSetupFile">
		<col key="yes" def="s72">ISSetupFile</col>
		<col def="S255">FileName</col>
		<col def="V0">Stream</col>
		<col def="S50">Language</col>
		<col def="I2">Splash</col>
		<col def="S0">Path</col>
	</table>

	<table name="ISSetupPrerequisites">
		<col key="yes" def="s72">ISSetupPrerequisites</col>
		<col def="S255">ISBuildSourcePath</col>
		<col def="I2">Order</col>
		<col def="I2">ISSetupLocation</col>
		<col def="S255">ISReleaseFlags</col>
		<row><td>_F4621A0F_6C70_4534_A477_516C6348831D_</td><td>Microsoft .NET Framework 4.0 Full (Web Download).prq</td><td/><td>2</td><td/></row>
	</table>

	<table name="ISSetupType">
		<col key="yes" def="s38">ISSetupType</col>
		<col def="L255">Description</col>
		<col def="L255">Display_Name</col>
		<col def="i2">Display</col>
		<col def="S255">Comments</col>
		<row><td>Custom</td><td>##IDS__IsSetupTypeMinDlg_ChooseFeatures##</td><td>##IDS__IsSetupTypeMinDlg_Custom##</td><td>3</td><td/></row>
		<row><td>Minimal</td><td>##IDS__IsSetupTypeMinDlg_MinimumFeatures##</td><td>##IDS__IsSetupTypeMinDlg_Minimal##</td><td>2</td><td/></row>
		<row><td>Typical</td><td>##IDS__IsSetupTypeMinDlg_AllFeatures##</td><td>##IDS__IsSetupTypeMinDlg_Typical##</td><td>1</td><td/></row>
	</table>

	<table name="ISSetupTypeFeatures">
		<col key="yes" def="s38">ISSetupType_</col>
		<col key="yes" def="s38">Feature_</col>
		<row><td>Custom</td><td>AlwaysInstall</td></row>
		<row><td>Minimal</td><td>AlwaysInstall</td></row>
		<row><td>Typical</td><td>AlwaysInstall</td></row>
	</table>

	<table name="ISStorages">
		<col key="yes" def="s72">Name</col>
		<col def="S0">ISBuildSourcePath</col>
	</table>

	<table name="ISString">
		<col key="yes" def="s255">ISString</col>
		<col key="yes" def="s50">ISLanguage_</col>
		<col def="S0">Value</col>
		<col def="I2">Encoded</col>
		<col def="S0">Comment</col>
		<col def="I4">TimeStamp</col>
		<row><td>COMPANY_NAME</td><td>1041</td><td>Tween</td><td>0</td><td/><td>-199245523</td></row>
		<row><td>DN_AlwaysInstall</td><td>1041</td><td>常にインストール</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_EXPRESS_LAUNCH_CONDITION_COLOR</td><td>1041</td><td>このｼｽﾃﾑの色設定では、[ProductName] を実行できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_EXPRESS_LAUNCH_CONDITION_DOTNETVERSION40FULL</td><td>1041</td><td>Microsoft .NET Framework 4.0 Full package or greater needs to be installed for this installation to continue.</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_EXPRESS_LAUNCH_CONDITION_OS</td><td>1041</td><td>このｵﾍﾟﾚｰﾃｨﾝｸﾞ ｼｽﾃﾑでは [ProductName] を実行できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_EXPRESS_LAUNCH_CONDITION_PROCESSOR</td><td>1041</td><td>このﾌﾟﾛｾｯｻでは [ProductName] を実行できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_EXPRESS_LAUNCH_CONDITION_RAM</td><td>1041</td><td>RAM の容量が [ProductName] を実行するのに十分ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_EXPRESS_LAUNCH_CONDITION_SCREEN</td><td>1041</td><td>画面解像度が [ProductName] を実行するのに十分ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_COMPACT</td><td>1041</td><td>最小</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_COMPACT_DESC</td><td>1041</td><td>最小の説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_COMPLETE</td><td>1041</td><td>完全</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_COMPLETE_DESC</td><td>1041</td><td>すべての説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_CUSTOM</td><td>1041</td><td>カスタム</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_CUSTOM_DESC</td><td>1041</td><td>カスタムの説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_CUSTOM_DESC_PRO</td><td>1041</td><td>カスタムの説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_TYPICAL</td><td>1041</td><td>標準</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDPROP_SETUPTYPE_TYPICAL_DESC</td><td>1041</td><td>標準の説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_1</td><td>1041</td><td>[1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_1b</td><td>1041</td><td>[1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_1c</td><td>1041</td><td>[1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_1d</td><td>1041</td><td>[1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Advertising</td><td>1041</td><td>後で使用するためにアプリケーションを設定しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_AllocatingRegistry</td><td>1041</td><td>レジストリ領域を割り振っています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_AppCommandLine</td><td>1041</td><td>アプリケーション : [1], コマンド ライン : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_AppId</td><td>1041</td><td>AppId : [1]{{, AppType : [2]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_AppIdAppTypeRSN</td><td>1041</td><td>AppId : [1]{{, AppType : [2], Users : [3], RSN : [4]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Application</td><td>1041</td><td>アプリケーション : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_BindingExes</td><td>1041</td><td>実行ファイルをバィンディングしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ClassId</td><td>1041</td><td>Class Id: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ClsID</td><td>1041</td><td>Class Id: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ComponentIDQualifier</td><td>1041</td><td>コンポーネント ID : [1], Qualifier : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ComponentIdQualifier2</td><td>1041</td><td>コンポーネント ID : [1], Qualifier : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ComputingSpace</td><td>1041</td><td>必要な領域を計算しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ComputingSpace2</td><td>1041</td><td>必要な領域を計算しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ComputingSpace3</td><td>1041</td><td>必要な領域を計算しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ContentTypeExtension</td><td>1041</td><td>MIME コンテント タイプ : [1], エクステンション : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ContentTypeExtension2</td><td>1041</td><td>MIME コンテント タイプ : [1], エクステンション : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_CopyingNetworkFiles</td><td>1041</td><td>ネットワーク インストール ファイルをコピーしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_CopyingNewFiles</td><td>1041</td><td>新しいファイルをコピーしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_CreatingDuplicate</td><td>1041</td><td>ファイルの複製を作成しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_CreatingFolders</td><td>1041</td><td>フォルダーを作成しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_CreatingIISRoots</td><td>1041</td><td>IIS Virtual Roots を作成しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_CreatingShortcuts</td><td>1041</td><td>ショートカットを作成しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_DeletingServices</td><td>1041</td><td>サービスを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_EnvironmentStrings</td><td>1041</td><td>環境ストリングを更新しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_EvaluateLaunchConditions</td><td>1041</td><td>起動状態を検証しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Extension</td><td>1041</td><td>エクステンション : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Extension2</td><td>1041</td><td>エクステンション : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Feature</td><td>1041</td><td>機能 : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FeatureColon</td><td>1041</td><td>機能 : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_File</td><td>1041</td><td>ファイル : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_File2</td><td>1041</td><td>ファイル : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDependencies</td><td>1041</td><td>ﾌｧｲﾙ: [1]、依存ﾌｧｲﾙ: [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDir</td><td>1041</td><td>ファイル : [1], ディレクトリ : [9]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDir2</td><td>1041</td><td>File: [1], Directory: [9]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDir3</td><td>1041</td><td>ファイル : [1], ディレクトリ : [9]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDirSize</td><td>1041</td><td>ファイル : [1], ディレクトリ : [9], サイズ : [6]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDirSize2</td><td>1041</td><td>File: [1],  Directory: [9],  Size: [6]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDirSize3</td><td>1041</td><td>ファイル : [1],  ディレクトリ : [9],  サイズ : [6]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDirSize4</td><td>1041</td><td>ファイル : [1],  ディレクトリ : [2],  サイズ : [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileDirectorySize</td><td>1041</td><td>ファイル : [1],  ディレクトリ : [9],  サイズ : [6]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileFolder</td><td>1041</td><td>ファイル : [1], フォルダー : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileFolder2</td><td>1041</td><td>ファイル : [1], フォルダー : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileSectionKeyValue</td><td>1041</td><td>ファイル : [1],  セクション : [2],  キー : [3], 値 : [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FileSectionKeyValue2</td><td>1041</td><td>ファイル : [1],  セクション : [2],  キー : [3], 値 : [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Folder</td><td>1041</td><td>フォルダー : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Folder1</td><td>1041</td><td>フォルダー : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Font</td><td>1041</td><td>フォント : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Font2</td><td>1041</td><td>フォント : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FoundApp</td><td>1041</td><td>見つかったアプリケーション : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_FreeSpace</td><td>1041</td><td>空き領域 : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_GeneratingScript</td><td>1041</td><td>アクションに対するスクリプト オペレーションを生成しています :</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ISLockPermissionsCost</td><td>1041</td><td>オブジェクトのアクセス許可情報を集めています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ISLockPermissionsInstall</td><td>1041</td><td>オブジェクトのアクセス許可情報を適用しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_InitializeODBCDirs</td><td>1041</td><td>ODBC ディレクトリを初期化しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_InstallODBC</td><td>1041</td><td>ODBC コンポーネントをインストールしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_InstallServices</td><td>1041</td><td>新しいサービスをインストールしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_InstallingSystemCatalog</td><td>1041</td><td>ｲﾝｽﾄｰﾙするｼｽﾃﾑ ｶﾀﾛｸﾞ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_KeyName</td><td>1041</td><td>キー : [1], 名前 : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_KeyNameValue</td><td>1041</td><td>キー : [1], 名前 : [2], 値 : [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_LibId</td><td>1041</td><td>LibID: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Libid2</td><td>1041</td><td>LibID: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_MigratingFeatureStates</td><td>1041</td><td>関連のアプリケーションから機能の状態を移行しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_MovingFiles</td><td>1041</td><td>ファイルを移動しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_NameValueAction</td><td>1041</td><td>名前 : [1], 値 : [2], アクション [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_NameValueAction2</td><td>1041</td><td>名前 : [1], 値 : [2], アクション [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_PatchingFiles</td><td>1041</td><td>ファイルを修正しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ProgID</td><td>1041</td><td>ProgId: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_ProgID2</td><td>1041</td><td>ProgId: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_PropertySignature</td><td>1041</td><td>プロパティ : [1], 署名 : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_PublishProductFeatures</td><td>1041</td><td>製品機能を発行しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_PublishProductInfo</td><td>1041</td><td>製品情報を発行しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_PublishingQualifiedComponents</td><td>1041</td><td>正規のコンポーネントを発行しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegUser</td><td>1041</td><td>ユーザーを登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisterClassServer</td><td>1041</td><td>クラス サーバーを登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisterExtensionServers</td><td>1041</td><td>エクステンション サーバーを登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisterFonts</td><td>1041</td><td>フォントを登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisterMimeInfo</td><td>1041</td><td>MIME 情報を登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisterTypeLibs</td><td>1041</td><td>タイプ ライブラリを登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisteringComPlus</td><td>1041</td><td>COM+ アプリケーションとコンポーネントを登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisteringModules</td><td>1041</td><td>モジュールを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisteringProduct</td><td>1041</td><td>製品を登録しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RegisteringProgIdentifiers</td><td>1041</td><td>プログラム識別子の登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemoveApps</td><td>1041</td><td>アプリケーションを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingBackup</td><td>1041</td><td>バックアップ ファイルを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingDuplicates</td><td>1041</td><td>重複しているファイルを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingFiles</td><td>1041</td><td>ファイルを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingFolders</td><td>1041</td><td>フォルダーを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingIISRoots</td><td>1041</td><td>IIS Virtual Roots を削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingIni</td><td>1041</td><td>INI ファイルのエントリを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingMoved</td><td>1041</td><td>移動したファイルを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingODBC</td><td>1041</td><td>ODBC コンポーネントを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingRegistry</td><td>1041</td><td>システム レジストリの値を削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RemovingShortcuts</td><td>1041</td><td>ショートカットを削除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_RollingBack</td><td>1041</td><td>アクションを元に戻しています :</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_SearchForRelated</td><td>1041</td><td>関連のアプリケーションを検索しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_SearchInstalled</td><td>1041</td><td>インストールされているアプリケーションを検索しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_SearchingQualifyingProducts</td><td>1041</td><td>正規の製品を検索しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_SearchingQualifyingProducts2</td><td>1041</td><td>正規の製品を検索しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Service</td><td>1041</td><td>サービス : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Service2</td><td>1041</td><td>サービス : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Service3</td><td>1041</td><td>サービス : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Service4</td><td>1041</td><td>サービス : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Shortcut</td><td>1041</td><td>ショートカット : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Shortcut1</td><td>1041</td><td>ショートカット : [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_StartingServices</td><td>1041</td><td>サービスを開始しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_StoppingServices</td><td>1041</td><td>サービスを停止しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnpublishProductFeatures</td><td>1041</td><td>製品機能の発行を停止しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnpublishQualified</td><td>1041</td><td>正規のコンポーネントの発行を停止しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnpublishingProductInfo</td><td>1041</td><td>製品情報の発行を停止しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregTypeLibs</td><td>1041</td><td>タイプライブラリの登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisterClassServers</td><td>1041</td><td>クラス サーバーの登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisterExtensionServers</td><td>1041</td><td>エクステンション サーバーの登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisterModules</td><td>1041</td><td>モジュールの登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisteringComPlus</td><td>1041</td><td>COM+ アプリケーションとコンポーネントの登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisteringFonts</td><td>1041</td><td>フォントの登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisteringMimeInfo</td><td>1041</td><td>MIME 情報の登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UnregisteringProgramIds</td><td>1041</td><td>プログラム識別子の登録を解除しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UpdateComponentRegistration</td><td>1041</td><td>コンポーネントの登録を更新しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_UpdateEnvironmentStrings</td><td>1041</td><td>環境ストリングを更新しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_Validating</td><td>1041</td><td>インストールを検証しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_WritingINI</td><td>1041</td><td>INI ファイルに値を書き込んでいます</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ACTIONTEXT_WritingRegistry</td><td>1041</td><td>システム レジストリの値を書き込んでいます</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_BACK</td><td>1041</td><td>{&amp;Tahoma9}&lt; 戻る(&amp;B)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_CANCEL</td><td>1041</td><td>{&amp;Tahoma9}キャンセル</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_CANCEL2</td><td>1041</td><td>{&amp;Tahoma9}キャンセル</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_CHANGE</td><td>1041</td><td>{&amp;Tahoma9}変更(&amp;C)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_COMPLUS_PROGRESSTEXT_COST</td><td>1041</td><td>COM+ アプリケーションのコスト分析: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_COMPLUS_PROGRESSTEXT_INSTALL</td><td>1041</td><td>COM+ アプリケーションのインストール: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_COMPLUS_PROGRESSTEXT_UNINSTALL</td><td>1041</td><td>COM+ アプリケーションのアンインストール: [1]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DIALOG_TEXT2_DESCRIPTION</td><td>1041</td><td>ダイアログ標準の記述</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DIALOG_TEXT_DESCRIPTION_EXTERIOR</td><td>1041</td><td>{&amp;TahomaBold10}ダイアログ太字タイトル</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DIALOG_TEXT_DESCRIPTION_INTERIOR</td><td>1041</td><td>{&amp;MSSansBold8}ダイアログ太字タイトル</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DIFX_AMD64</td><td>1041</td><td>[ProductName] は、X64 プロセッサが必要です。 「OK」 をクリックするとウィザードが閉じます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DIFX_IA64</td><td>1041</td><td>[ProductName] は、IA64 プロセッサが必要です。 「OK」 をクリックするとウィザードが閉じます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DIFX_X86</td><td>1041</td><td>[ProductName] は、X86 プロセッサが必要です。 「OK」 をクリックするとウィザードが閉じます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_DatabaseFolder_InstallDatabaseTo</td><td>1041</td><td>{&amp;Tahoma9}[ProductName] データベースのインストール先:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_0</td><td>1041</td><td>{{致命的なエラー : }}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1</td><td>1041</td><td>エラー [1]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_10</td><td>1041</td><td>=== ログ開始 : [Date]  [Time] ===</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_100</td><td>1041</td><td>ショートカット [2] を削除できません。ショートカット ファイルが存在するか、またこのファイルへのアクセス権を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_101</td><td>1041</td><td>ファイル [2] にタイプ ライブラリを登録できません。サポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_102</td><td>1041</td><td>ファイル [2] のタイプ ライブラリの登録を取り消すことができません。サポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_103</td><td>1041</td><td>Ini ファイル [2][3] を更新できません。目的のファイルが存在するかどうか、またこのファイルへのアクセス権を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_104</td><td>1041</td><td>再起動するときに、ファイル [3] を [2] で置き換えるようにスケジュールすることができません。ファイル [3] への書き込み権を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_105</td><td>1041</td><td>ODBC ドライバ マネージャの削除でエラーが発生しました。ODBCエラー [2] : [3]。サポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_106</td><td>1041</td><td>ODBC ドライバ マネージャのインストールでエラーが発生しました。ODBC エラー [2]: [3]。サポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_107</td><td>1041</td><td>ODBC ドライバ [4] の削除でエラーが発生しました。ODBC エラー [2] : [3]。ODBC ドライバを削除する権限を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_108</td><td>1041</td><td>ODBC ドライバ [4] のインストールでエラーが発生しました。 ODBC エラー [2] : [3]。ファイル [4] が存在するか、およびこのファイルへのアクセス権を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_109</td><td>1041</td><td>ODBC データ ソースの設定に失敗しました: [4]。ODBC エラー [2] : [3]。ファイル [4] が存在するか、またこのファイルへのアクセス権を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_11</td><td>1041</td><td>=== ログ終了 : [Date]  [Time] ===</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_110</td><td>1041</td><td>サービス '[2]' ([3]) が開始できませんでした。システム サービスを開始する権限を持っていることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_111</td><td>1041</td><td>サービス '[2]' ([3]) を停止できませんでした。システム サービスを停止する権限を持っていることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_112</td><td>1041</td><td>サービス '[2]' ([3]) を削除できませんでした。システム サービスを削除する権限を持っていることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_113</td><td>1041</td><td>サービス '[2]' ([3]) をインストールできませんでした。システム サービスをインストールする権限を持っていることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_114</td><td>1041</td><td>環境変数 '[2]' を更新できませんでした。環境変数を修正する権限を持っていることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_115</td><td>1041</td><td>アクセス権が不十分なため、このマシンのすべてのユーザーが使用できるようにアプリケーションをインストールすることはできません。管理者としてログインし、再度インストールしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_116</td><td>1041</td><td>ファイル '[3]' に対するセキュリティを設定できません。エラー : [2]。 このファイルに対してセキュリティを変更する権限を持っているかどうか確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_117</td><td>1041</td><td>コンポーネント サービス (COM+ 1.0) は、このコンピュータに正しくインストールされていません。インストールを完了するには コンポーネント サービスが必要です。コンポーネント サービスは、Windows 2000 で利用できます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_118</td><td>1041</td><td>COM+ アプリケーションの登録エラーです。詳しくはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_119</td><td>1041</td><td>COM+ アプリケーションの登録解除のエラーです。詳しくはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_12</td><td>1041</td><td>アクション開始 [Time] : [1]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_120</td><td>1041</td><td>このアプリケーションの古いバージョンを削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_121</td><td>1041</td><td>このアプリケーションの古いバージョンを削除する準備をしています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_122</td><td>1041</td><td>ファイル [2] にパッチを適用する時にエラーが発生しました。おそらく別の方法によってアップデートされたためです。このパッチで変更することはできません。詳細は、このパッチのベンダーにお問い合わせください。 {{システム エラー : [3]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_123</td><td>1041</td><td>[2] は要求された製品の 1 つをインストールできません。技術的に詳しい方にお問い合わせください。 {{システム エラー : [3]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_124</td><td>1041</td><td>[2] の古いバージョンを削除することはできません。技術的に詳しい方にお問い合わせください。 {{システム エラー : [3]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_125</td><td>1041</td><td>サービス '[2]' ([3]) の記述は変更できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_126</td><td>1041</td><td>Windows Installer サービスは、システム ファイル [2] をアップデートできません。このファイルは Windows によって保護されています。このプログラムを正しく動作させるには、オペレーティング システムをアップデートする必要がある可能性があります。 {{パッケージ バージョン : [3], OS プロテクテッド バージョン  : [4]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_127</td><td>1041</td><td>Windows Installer サービスでは、保護された Windows ファイル [2] をアップデートすることはできません。 {{パッケージ バージョン : [3], OS プロテクテッド バージョン 1 : [4], SFP エラー : [5]}}"</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_128</td><td>1041</td><td>Windows インストーラ サービスが、保護された Windows ファイルをアップデートできません。 SFP エラー： [2]保護されているファイルのリスト： [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_129</td><td>1041</td><td>マシンのポリシーにより、ユーザーのインストールが無効にされています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_13</td><td>1041</td><td>アクション終了 [Time] : [1]。 戻り値 [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_130</td><td>1041</td><td>このセットアップには、IIS 仮想ルートを構成するために Internet Information Server 4.0 またはそれ以降が必要です。 IIS 4.0 またはそれ以降を使用しているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_131</td><td>1041</td><td>このセットアップには、仮想ルートを構成するために Administrator (管理者) の特権が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1329</td><td>1041</td><td>キャビネット ファイル [2] はデジタル署名が付いていないため、必要なファイルをインストールできません。 キャビネット ファイルが壊れている可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1330</td><td>1041</td><td>キャビネット ファイル [2] のデジタル署名が無効なため、必要なファイルをインストールできません。 キャビネット ファイルが壊れている可能性があります。{ エラー [3] が WinVerifyTrust によって返されました。}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1331</td><td>1041</td><td>[2] ファイルを正しくコピーできませんでした: CRC エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1332</td><td>1041</td><td>[2] ファイルを正しくパッチできませんでした: CRC エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1333</td><td>1041</td><td>[2] ファイルを正しくパッチできませんでした: CRC エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1334</td><td>1041</td><td>ファイル '[2]' はキャビネット ファイル '[3]' にないため、インストールできません。 ネットワーク エラーまたは CD-ROM からの読み取りエラーが発生したか、このパッケージに問題がある可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1335</td><td>1041</td><td>このインストールに必要なキャビネット ファイル '[2]' は壊れているため使用できません。 ネットワーク エラーまたは CD-ROM からの読み取りエラーが発生したか、このパッケージに問題がある可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1336</td><td>1041</td><td>このインストールを完了するために必要な一時ファイルの作成中にエラーが発生しました。 フォルダー: [3]。システム エラー コード: [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_14</td><td>1041</td><td>残り時間 : {[1] 分 }{[2] 秒}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_15</td><td>1041</td><td>メモリ不足です。他のアプリケーションを閉じてから、再度実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_16</td><td>1041</td><td>インストール プログラムから応答がありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1609</td><td>1041</td><td>セキュリティ設定を適用するときにエラーが発生しました。 [2] は有効なユーザーまたはグループではありません。 パッケージに問題があるか、ネットワーク上のドメイン コントローラへの接続に問題がある可能性があります。 ネットワーク接続を確認し 「再試行」 をクリックするか、インストールを終了するには 「キャンセル」 をクリックしてください。 ユーザーの SID が見つかりません, システム エラー [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1651</td><td>1041</td><td>管理者ユーザーが、アドバタイズの状態にあるユーザーごとに管理されている、またはマシンごとに管理されているアプリケーションのパッチの適用に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_17</td><td>1041</td><td>インストール プログラムは完了前に中断されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1715</td><td>1041</td><td>[2] をインストールしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1716</td><td>1041</td><td>[2] を構成しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1717</td><td>1041</td><td>[2] を削除しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1718</td><td>1041</td><td>ファイル [2] はデジタル署名ポリシーによって拒否されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1719</td><td>1041</td><td>Windows Installer サービスにアクセスできませんでした。 サポート担当者に連絡して、Windows Installer が正しく登録されているか確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1720</td><td>1041</td><td>この Windows Installer パッケージには問題があります。 このインストールを完了するのに必要なスクリプトを実行できませんでした。 サポート担当者またはパッケージ製造元に問い合わせてください。 カスタム アクション [2] スクリプト エラー [3]、[4]: [5] 行 [6]、列 [7]、[8]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1721</td><td>1041</td><td>この Windows Installer パッケージには問題があります。 このインストールを完了するのに必要なプログラムを実行できませんでした。 サポート担当者またはパッケージ製造元に問い合わせてください。 アクション: [2]、場所: [3]、コマンド: [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1722</td><td>1041</td><td>この Windows Installer パッケージには問題があります。 セットアップの一部として実行されるプログラムを正しく完了しませんでした。 サポート担当者またはパッケージ製造元に問い合わせてください。 アクション: [2]、場所: [3]、コマンド: [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1723</td><td>1041</td><td>この Windows Installer パッケージには問題があります。 このインストールを完了するのに必要な Dll を実行できませんでした。 サポート担当者またはパッケージ製造元に問い合わせてください。 アクション: [2]、エントリ: [3]、ライブラリ: [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1724</td><td>1041</td><td>削除は正しく完了しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1725</td><td>1041</td><td>削除に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1726</td><td>1041</td><td>アドバタイズは正しく完了しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1727</td><td>1041</td><td>アドバタイズに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1728</td><td>1041</td><td>構成は正しく完了しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1729</td><td>1041</td><td>構成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1730</td><td>1041</td><td>このアプリケーションを削除するには、管理者である必要があります。 このアプリケーションを削除するには、管理者としてログオンするか、またはテクニカル サポート グループに問い合わせてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1731</td><td>1041</td><td>製品 [2] のソース インストール パッケージは、クライアント パッケージとの同期がとれていません。 インストール パッケージ '[3]' の有効なコピーを使ってもう一度インストールしてみてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1732</td><td>1041</td><td>[2] のインストールを完了するためには、コンピュータを再起動する必要があります。 ほかのユーザーが現在このコンピュータにログオンしています。再起動するとそのユーザーが保存していない作業が失われる可能性があります。 今すぐ再起動しますか?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_18</td><td>1041</td><td>Windows に [ProductName] を設定しています。しばらくお待ちください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_19</td><td>1041</td><td>必要な情報を集めています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1935</td><td>1041</td><td>アセンブリ コンポーネント [2] のインストール中にエラーが発生しました。 HRESULT: [3]. {{アセンブリ インターフェイス: [4], 関数: [5], アセンブリ名: [6]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1936</td><td>1041</td><td>アセンブリ '[6]' のインストール中にエラーが発生しました。 アセンブリ名の強さが十分でないか、長さの基準を満たしたキーを使って署名されていません。 HRESULT: [3]. {{アセンブリ インターフェイス: [4], 関数: [5], コンポーネント: [2]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1937</td><td>1041</td><td>アセンブリ '[6]' のインストール中にエラーが発生しました。 署名またはカタログを検証できないか、またはそれらが無効です。 HRESULT: [3]. {{アセンブリ インターフェイス: [4], 関数: [5], コンポーネント: [2]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_1938</td><td>1041</td><td>アセンブリ '[6]' のインストール中にエラーが発生しました。 アセンブリのモジュールの 1 つまたはそれ以上が見つかりません。 HRESULT: [3]. {{アセンブリ インターフェイス: [4], 関数: [5], コンポーネント: [2]}}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2</td><td>1041</td><td>警告 [1]。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_20</td><td>1041</td><td>{[ProductName] }のセットアップが正常終了しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_21</td><td>1041</td><td>{[ProductName] }のセットアップに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2101</td><td>1041</td><td>ショートカットは、このオペレーティング システムではサポートされません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2102</td><td>1041</td><td>.ini アクションが無効です: [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2103</td><td>1041</td><td>シェル フォルダー [2] のパスを解決できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2104</td><td>1041</td><td>.ini ファイルの書き込み: [3]: システム エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2105</td><td>1041</td><td>ショートカットの作成 [3] に失敗しました。システム エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2106</td><td>1041</td><td>ショートカットの削除 [3] に失敗しました。システム エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2107</td><td>1041</td><td>種類のライブラリ [2] の登録でエラー [3] が発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2108</td><td>1041</td><td>種類のライブラリ [2] の登録の解除でエラー [3] が発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2109</td><td>1041</td><td>.ini アクションのためのセクションが見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2110</td><td>1041</td><td>.ini アクションのためのキーが見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2111</td><td>1041</td><td>実行中のアプリケーションの検出に失敗し、パフォーマンス データを取得できませんでした。 返されたレジストリ操作 : [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2112</td><td>1041</td><td>実行中のアプリケーションの検出に失敗し、パフォーマンス インデックスを取得できませんでした。 返されたレジストリ操作 : [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2113</td><td>1041</td><td>実行中のアプリケーションの検出に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_22</td><td>1041</td><td>ファイル [2] の読み込みに失敗しました。{{ システム エラー [3]。}}このファイルが存在するかどうか、およびこのファイルへのアクセス権があるかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2200</td><td>1041</td><td>データベース: [2]。データベース オブジェクトの作成に失敗しました。モード = [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2201</td><td>1041</td><td>データベース: [2]。メモリ不足のため、初期化に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2202</td><td>1041</td><td>データベース: [2]。メモリ不足のため、データ アクセスに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2203</td><td>1041</td><td>データベース: [2]。データベース ファイルを開けません。システム エラー [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2204</td><td>1041</td><td>データベース: [2]。テーブルが既に存在します。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2205</td><td>1041</td><td>データベース: [2]。テーブルが存在しません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2206</td><td>1041</td><td>データベース: [2]。テーブルを削除できませんでした: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2207</td><td>1041</td><td>データベース: [2]。インテント違反。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2208</td><td>1041</td><td>データベース: [2]。実行に必要なパラメータが足りません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2209</td><td>1041</td><td>データベース: [2]。カーソルが無効の状態です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2210</td><td>1041</td><td>データベース: [2]。列 [3] の更新データの種類が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2211</td><td>1041</td><td>データベース: [2]。データベース テーブル [3] を作成できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2212</td><td>1041</td><td>データベース: [2]。データベースが書き込み可能な状態になっていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2213</td><td>1041</td><td>データベース: [2]。データベース テーブルの保存エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2214</td><td>1041</td><td>データベース: [2]。エクスポート ファイルの書き込みエラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2215</td><td>1041</td><td>データベース: [2]。インポート ファイルを開けません: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2216</td><td>1041</td><td>データベース: [2]。インポート ファイルの形式のエラー: [3]、行 [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2217</td><td>1041</td><td>データベース: [2]。CreateOutputDatabase [3] に対する状態が不適切です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2218</td><td>1041</td><td>データベース: [2]。テーブル名が指定されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2219</td><td>1041</td><td>データベース: [2]。インストーラ データベースの形式が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2220</td><td>1041</td><td>データベース: [2]。行/フィールド データが無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2221</td><td>1041</td><td>データベース: [2]。インポート ファイルでコードページが矛盾しています: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2222</td><td>1041</td><td>データベース: [2]。トランスフォームまたはマージのコード ページ [3] がデータベースのコード ページ [4] と異なります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2223</td><td>1041</td><td>データベース: [2]。データベースが同じです。トランスフォームは生成されません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2224</td><td>1041</td><td>データベース: [2]。GenerateTransform: データベースが壊れています。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2225</td><td>1041</td><td>データベース: [2]。トランスフォーム: 一時テーブルを変換できません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2226</td><td>1041</td><td>データベース: [2]。トランスフォームに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2227</td><td>1041</td><td>データベース: [2]。SQL クエリに無効な識別子 '[3]' が含まれています: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2228</td><td>1041</td><td>データベース: [2]。SQL クエリに不明なテーブル '[3]' が含まれています: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2229</td><td>1041</td><td>データベース: [2]。SQL クエリ内のテーブル '[3]' を読み込めませんでした: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2230</td><td>1041</td><td>データベース: [2]。SQL クエリ内でテーブル '[3]' が繰り返されています: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2231</td><td>1041</td><td>データベース: [2]。SQL クエリに ')' が足りません: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2232</td><td>1041</td><td>データベース: [2]。SQL クエリに予期しないトークン '[3]' が含まれています: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2233</td><td>1041</td><td>データベース: [2]。SQL クエリの SELECT 句に列がありません: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2234</td><td>1041</td><td>データベース: [2]。SQL クエリの ORDER BY 句に列がありません: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2235</td><td>1041</td><td>データベース: [2]。SQL クエリ内の列 '[3]' が存在しないかあいまいです: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2236</td><td>1041</td><td>データベース: [2]。SQL クエリの演算子 '[3]' が無効です: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2237</td><td>1041</td><td>データベース: [2]。クエリ文字列が無効であるか見つかりません: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2238</td><td>1041</td><td>データベース: [2]。SQL クエリに FROM 句がありません: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2239</td><td>1041</td><td>データベース: [2]。INSERT SQL ステートメントに値が足りません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2240</td><td>1041</td><td>データベース: [2]。UPDATE SQL ステートメントに更新列がありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2241</td><td>1041</td><td>データベース: [2]。INSERT SQL ステートメントに挿入列がありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2242</td><td>1041</td><td>データベース: [2]。列 '[3]' が繰り返されています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2243</td><td>1041</td><td>データベース: [2]。テーブル作成に主列が定義されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2244</td><td>1041</td><td>データベース: [2]。SQL クエリに無効な型指定子 '[3]' が含まれています: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2245</td><td>1041</td><td>IStorage::Stat がエラー [3] で失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2246</td><td>1041</td><td>データベース: [2]。インストーラのトランスフォーム形式が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2247</td><td>1041</td><td>データベース: [2]。トランスフォーム ストリームの読み取り/書き込みに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2248</td><td>1041</td><td>データベース: [2] 生成/トランスフォーム/マージ: 基本テーブルの列の種類が参照テーブルと一致しません。 テーブル: [3] 列番号: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2249</td><td>1041</td><td>データベース: [2] GenerateTransform: 基本テーブルに参照テーブルよりも多くの列があります。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2250</td><td>1041</td><td>データベース: [2] トランスフォーム: 既存の行は追加できません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2251</td><td>1041</td><td>データベース: [2] トランスフォーム: 存在しない行は削除できません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2252</td><td>1041</td><td>データベース: [2] トランスフォーム: 既存のテーブルは追加できません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2253</td><td>1041</td><td>データベース: [2] トランスフォーム: 存在しないテーブルは削除できません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2254</td><td>1041</td><td>データベース: [2] トランスフォーム: 存在しない行は更新できません。テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2255</td><td>1041</td><td>データベース: [2] トランスフォーム: 同じ名前の列が既にあります。 テーブル: [3] 列: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2256</td><td>1041</td><td>データベース [2] 生成/トランスフォーム/マージ: 基本テーブルの主キーの数が参照テーブルと一致しません。 テーブル: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2257</td><td>1041</td><td>データベース: [2]。読み取り専用のテーブルを変更しようとしました。[3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2258</td><td>1041</td><td>データベース: [2]。パラメータに種類の不一致があります: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2259</td><td>1041</td><td>データベース: [2] テーブルの更新に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2260</td><td>1041</td><td>ストレージで CopyTo に失敗しました。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2261</td><td>1041</td><td>ストリーム [2] を削除できませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2262</td><td>1041</td><td>ストリームが存在しません: [2]。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2263</td><td>1041</td><td>ストリーム [2] を開けませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2264</td><td>1041</td><td>ストリーム [2] を削除できませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2265</td><td>1041</td><td>ストレージをコミットできませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2266</td><td>1041</td><td>ストレージをロールバックできませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2267</td><td>1041</td><td>ストレージ [2] を削除できませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2268</td><td>1041</td><td>データベース: [2]。マージ: [3] テーブル内でマージの矛盾が報告されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2269</td><td>1041</td><td>データベース: [2]。マージ: 2 つのデータベースの '[3]' テーブルで列数が異なります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2270</td><td>1041</td><td>データベース: [2]。 トランスフォーム/マージの生成: 基本のテーブルの列名がリファレンス テーブルと一致しません。 テーブル: [3] 列番号: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2271</td><td>1041</td><td>トランスフォームのための SummaryInformation の書き込みに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2272</td><td>1041</td><td>データベース: [2]。データベースが読み取り専用で開かれているため、MergeDatabase では変更は書き込まれません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2273</td><td>1041</td><td>データベース: [2]。 MergeDatabase: 参照データベースとして基本データベースへの参照が渡されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2274</td><td>1041</td><td>データベース: [2]。 MergeDatabase: Error テーブルにエラーを書き込むことができません。 定義済みの Error テーブルにある非ヌル値列が原因である可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2275</td><td>1041</td><td>データベース: [2]。指定された Modify [3] 操作がテーブルの結合に対して無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2276</td><td>1041</td><td>データベース: [2]。コード ページ [3] はシステムでサポートされていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2277</td><td>1041</td><td>データベース: [2]。テーブル [3] の保存に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2278</td><td>1041</td><td>データベース: [2]。SQL クエリの WHERE 句で、式が制限数の 32 を超えています: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2279</td><td>1041</td><td>データベース: [2] トランスフォーム: 基本テーブル [3] 内の列が多すぎます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2280</td><td>1041</td><td>データベース: [2]。テーブル [4] に列 [3] を作成できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2281</td><td>1041</td><td>ストリーム [2] の名前を変更できませんでした。システム エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2282</td><td>1041</td><td>ストリーム名 [2] が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_23</td><td>1041</td><td>ファイル '[2]' を作成できません。この名前を持つディレクトリはすでに存在しています。インストールをキャンセルして、別の場所にインストールしてみてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2302</td><td>1041</td><td>パッチの通知: [2] バイト パッチ済み。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2303</td><td>1041</td><td>ボリューム情報の取得エラー。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2304</td><td>1041</td><td>ディスクの空き領域の取得エラー。GetLastError: [2]。ボリューム: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2305</td><td>1041</td><td>パッチのスレッド待ちのエラー。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2306</td><td>1041</td><td>パッチ プログラム アプリケーションのスレッドを作成できませんでした。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2307</td><td>1041</td><td>ソース ファイルのキー名が Null です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2308</td><td>1041</td><td>受け側のファイル名が Null です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2309</td><td>1041</td><td>パッチが既に進行しているときにファイル [2] をパッチしようとしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2310</td><td>1041</td><td>パッチが進行していないときにパッチを続行しようとしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2315</td><td>1041</td><td>パスの区切りがありません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2318</td><td>1041</td><td>ファイルが存在しません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2319</td><td>1041</td><td>ファイル属性の設定エラー: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2320</td><td>1041</td><td>ファイルが書き込み可能になっていません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2321</td><td>1041</td><td>ファイルの作成エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2322</td><td>1041</td><td>ユーザーがキャンセルされました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2323</td><td>1041</td><td>ファイルの属性が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2324</td><td>1041</td><td>ファイルを開くことができませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2325</td><td>1041</td><td>ファイルのファイル時刻を取得できませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2326</td><td>1041</td><td>FileToDosDateTime のエラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2327</td><td>1041</td><td>ディレクトリを削除できませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2328</td><td>1041</td><td>ファイルのファイル バージョン情報の取得エラー。[2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2329</td><td>1041</td><td>ファイルの削除エラー: [3]。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2330</td><td>1041</td><td>ファイル属性の取得エラー。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2331</td><td>1041</td><td>ライブラリ [2] のロードエラー、またはエントリポイント [3] の検索エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2332</td><td>1041</td><td>ファイル属性の取得エラー。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2333</td><td>1041</td><td>ファイル属性の設定エラー。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2334</td><td>1041</td><td>ファイルのファイル時刻をローカル時刻に変換するときにエラーが発生しました: [3]。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2335</td><td>1041</td><td>パス: [2] は [3] の親ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2336</td><td>1041</td><td>パスに対する一時ファイルの作成エラー: [3]。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2337</td><td>1041</td><td>ファイルを閉じることができませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2338</td><td>1041</td><td>ファイルのリソースを更新できませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2339</td><td>1041</td><td>ファイルのファイル時刻を設定できませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2340</td><td>1041</td><td>ファイルのリソースを更新できませんでした: [3]。リソースが見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2341</td><td>1041</td><td>ファイルのリソースを更新できませんでした: [3]。リソースが大きすぎます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2342</td><td>1041</td><td>ファイルのリソースを更新できませんでした: [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2343</td><td>1041</td><td>指定されたパスが空です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2344</td><td>1041</td><td>ファイルの検証に必要なファイル IMAGEHLP.DLL が見つかりませんでした:[2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2345</td><td>1041</td><td>[2]: 有効なチェックサム値がファイルに含まれていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2347</td><td>1041</td><td>ユーザーによる無視。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2348</td><td>1041</td><td>キャビネット ストリームからの読み取りでエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2349</td><td>1041</td><td>異なる情報でコピーが再開されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2350</td><td>1041</td><td>FDI サーバーのエラー</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2351</td><td>1041</td><td>キャビネット '[3]' にファイルのキー '[2]' が見つかりません。インストールを続行できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2352</td><td>1041</td><td>キャビネット ファイル サーバーを初期化できませんでした。 必要なファイル CABINET.DLL が見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2353</td><td>1041</td><td>キャビネットではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2354</td><td>1041</td><td>キャビネットを処理できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2355</td><td>1041</td><td>キャビネットが壊れています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2356</td><td>1041</td><td>ストリーム内にキャビネットが見つかりませんでした: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2357</td><td>1041</td><td>属性を設定できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2358</td><td>1041</td><td>ファイルが使用中であるかどうかを判別するときにエラーが発生しました: [3]。GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2359</td><td>1041</td><td>ターゲット ファイルを作成できません - ファイルが使用中である可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2360</td><td>1041</td><td>進行状況のティック。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2361</td><td>1041</td><td>次のキャビネットが必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2362</td><td>1041</td><td>フォルダーが見つかりません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2363</td><td>1041</td><td>フォルダーのサブフォルダーを列挙できませんでした: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2364</td><td>1041</td><td>CreateCopier 呼び出しの列挙定数が正しくありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2365</td><td>1041</td><td>exe ファイル [2] の BindImage に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2366</td><td>1041</td><td>ユーザーによる失敗。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2367</td><td>1041</td><td>ユーザーによる中止。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2368</td><td>1041</td><td>ネットワーク リソース情報を取得できませんでした。 エラー [2]、ネットワーク パス [3]。 拡張エラー: ネットワーク プロバイダ [5]、エラー コード [4]、エラーの説明 [6]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2370</td><td>1041</td><td>[2] ファイルの CRC チェックサム値が無効です。{ ヘッダによると、[3] がチェックサムで、計算結果は [4] になっています。}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2371</td><td>1041</td><td>ファイル [2] にパッチを適用できませんでした。GetLastError: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2372</td><td>1041</td><td>パッチ ファイル [2] が壊れているか、ファイルの形式が無効です。 ファイル [3] をパッチしようとしました。 GetLastError: [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2373</td><td>1041</td><td>ファイル [2] は有効なパッチ ファイルではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2374</td><td>1041</td><td>ファイル [2] はパッチ ファイル [3] の適用先として有効なファイルではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2375</td><td>1041</td><td>不明なパッチ エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2376</td><td>1041</td><td>キャビネットが見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2379</td><td>1041</td><td>ファイルを読み取り用に開くときにエラーが発生しました: [3]. GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2380</td><td>1041</td><td>ファイルを書き込み用に開くときにエラーが発生しました: [3]. GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2381</td><td>1041</td><td>ディレクトリが存在しません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2382</td><td>1041</td><td>ドライブの準備ができていません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_24</td><td>1041</td><td>ディスク [2] を挿入してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2401</td><td>1041</td><td>キー [2] について、64 ビット操作が 32 ビット オペレーティング システム上で試みられました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2402</td><td>1041</td><td>メモリ不足です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_25</td><td>1041</td><td>インストール プログラムにはディレクトリ [2] にアクセスする権限がありません。インストールを継続できません。管理者としてログインするか、またはシステム管理者にお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2501</td><td>1041</td><td>ロールバック スクリプトの列挙値を作成できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2502</td><td>1041</td><td>インストールが進行していないときに InstallFinalize が呼び出されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2503</td><td>1041</td><td>進行中としてマークされていないときに RunScript が呼び出されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_26</td><td>1041</td><td>ファイル [2] への書き込みに失敗しました。このディレクトリへのアクセス権があるかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2601</td><td>1041</td><td>プロパティ [2] の値が無効です: '[3]'</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2602</td><td>1041</td><td>[2] テーブルのエントリ '[3]' に、Media テーブル内に関連付けられたエントリがありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2603</td><td>1041</td><td>テーブル名 [2] は重複しています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2604</td><td>1041</td><td>[2] プロパティは定義されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2605</td><td>1041</td><td>[3] または [4] にサーバー [2] が見つかりませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2606</td><td>1041</td><td>プロパティ [2] の値が有効な完全なパスではありません: '[3]'。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2607</td><td>1041</td><td>Media テーブルが見つからないか空です (ファイルのインストールに必要です)。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2608</td><td>1041</td><td>オブジェクトのセキュリティ記述子を作成できませんでした。エラー: '[2]'。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2609</td><td>1041</td><td>初期化の前に製品の設定を移行しようとしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2611</td><td>1041</td><td>ファイル [2] は圧縮済みとしてマークされていますが、関連付けられたメディア エントリでキャビネットが指定されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2612</td><td>1041</td><td>'[2]' 列にストリームが見つかりません。主キー: '[3]'。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2613</td><td>1041</td><td>RemoveExistingProducts アクションの順序が正しくありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2614</td><td>1041</td><td>インストール パッケージから IStorage オブジェクトにアクセスできませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2615</td><td>1041</td><td>ソースの解決に失敗したため、モジュール [2] の登録解除はスキップされました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2616</td><td>1041</td><td>コンパニオン ファイル [2] の親が見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2617</td><td>1041</td><td>共有コンポーネント [2] が Component テーブルに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2618</td><td>1041</td><td>孤立したアプリケーション コンポーネント [2] が Component テーブルに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2619</td><td>1041</td><td>孤立したコンポーネント [2]、[3] は同じ機能の一部ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2620</td><td>1041</td><td>孤立したアプリケーション コンポーネント [2] のキー ファイルが File テーブルにありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2621</td><td>1041</td><td>ショートカット [2] のリソース DLL またはリソース ID が不適切に設定されています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27</td><td>1041</td><td>ファイル [2] の読み込みに失敗しました。{{ システム エラー [3]。}} ファイルが存在しアクセス権があることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2701</td><td>1041</td><td>機能の階層が、許容されるツリー階層のレベル [2] を超えています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2702</td><td>1041</td><td>Feature テーブルのレコード ([2]) が、Attributes フィールドに存在しない親を参照しています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2703</td><td>1041</td><td>ルート ソース パスのプロパティ名が定義されていません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2704</td><td>1041</td><td>ルート ディレクトリのプロパティが定義されていません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2705</td><td>1041</td><td>テーブルが無効です: [2]。ツリーとしてリンクできませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2706</td><td>1041</td><td>ソース パスは作成されませんでした。 Directory テーブルにエントリ [2] のパスが存在しません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2707</td><td>1041</td><td>ターゲット パスは作成されませんでした。 Directory テーブルにエントリ [2] のパスが存在しません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2708</td><td>1041</td><td>ファイル テーブルにエントリが見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2709</td><td>1041</td><td>指定されたコンポーネント名 ('[2]') が Component テーブルに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2710</td><td>1041</td><td>要求された 'Select' 状態はこのコンポーネントには無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2711</td><td>1041</td><td>指定された機能名 ('[2]') が Feature テーブルに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2712</td><td>1041</td><td>モードレス ダイアログからの戻り値が無効です: [3]。アクション: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2713</td><td>1041</td><td>Null が許容されない列に Null 値があります ('[4]' テーブルの '[3]' 列の '[2]'。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2714</td><td>1041</td><td>デフォルトのフォルダー名の値が無効です: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2715</td><td>1041</td><td>指定されたファイルのキー ('[2]') が File テーブルに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2716</td><td>1041</td><td>コンポーネント '[2]' のランダムなサブコンポーネント名を作成できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2717</td><td>1041</td><td>アクションの状態が正しくないか、カスタム アクション '[2]' の呼び出しエラーです。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2718</td><td>1041</td><td>製品コード '[2]' のパッケージ名がありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2719</td><td>1041</td><td>ソース '[2]' に UNC またはドライブ文字のいずれのパスも見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2720</td><td>1041</td><td>SourceList キーを開くときにエラーが発生しました。エラー: '[2]'。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2721</td><td>1041</td><td>カスタム アクション [2] が Binary テーブル ストリームに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2722</td><td>1041</td><td>カスタム アクション [2] が File テーブルに見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2723</td><td>1041</td><td>カスタム アクション [2] で、サポートされていない種類が指定されています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2724</td><td>1041</td><td>実行元のメディアにあるボリューム ラベル '[2]' が、Media テーブルのラベル '[3]' と一致しません。 これは、Media テーブルのエントリが 1 つだけのときのみ許可されます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2725</td><td>1041</td><td>データベース テーブルが無効です</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2726</td><td>1041</td><td>アクションが見つかりません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2727</td><td>1041</td><td>ディレクトリ エントリ '[2]' が Directory テーブルに存在しません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2728</td><td>1041</td><td>テーブルの定義のエラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2729</td><td>1041</td><td>インストール エンジンが初期化されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2730</td><td>1041</td><td>データベース内の値が正しくありません。 テーブル: '[2]'; 主キー: '[3]'; 列: '[4]'</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2731</td><td>1041</td><td>選択マネージャが初期化されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2732</td><td>1041</td><td>ディレクトリ マネージャが初期化されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2733</td><td>1041</td><td>'[4]' テーブルの '[3]' 列の外部キー ('[2]') が正しくありません</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2734</td><td>1041</td><td>再インストール モードの文字が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2735</td><td>1041</td><td>カスタム アクション '[2]' の実行は、未処理の例外が発生したため、中止されました。 これは、アクセス違反など、カスタム アクションの内部エラーが原因である可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2736</td><td>1041</td><td>カスタム アクションの一時ファイルの生成に失敗しました: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2737</td><td>1041</td><td>カスタム アクション [2]、エントリ [3]、ライブラリ [4] にアクセスできませんでした</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2738</td><td>1041</td><td>カスタム アクション [2] の VBScript ランタイムにアクセスできませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2739</td><td>1041</td><td>カスタム アクション [2] の JavaScript ランタイムにアクセスできませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2740</td><td>1041</td><td>カスタム アクション [2] スクリプト エラー [3]、[4]: [5] 行 [6]、列 [7]、[8]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2741</td><td>1041</td><td>製品 [2] の構成情報が壊れています。無効な情報: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2742</td><td>1041</td><td>サーバーに対するマーシャリングに失敗しました: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2743</td><td>1041</td><td>カスタム アクション [2] を実行できませんでした。場所: [3]、コマンド: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2744</td><td>1041</td><td>カスタム アクション [2] によって呼び出された EXE に失敗しました。場所: [3]、コマンド: [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2745</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた言語 [4]、検出された言語 [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2746</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた製品 [4]、検出された製品 [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2747</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた製品バージョン [4] より前、検出された製品バージョン [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2748</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた製品バージョン [4] 以前、検出された製品バージョン [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2749</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた製品バージョン [4]、検出された製品バージョン [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2750</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた製品バージョン [4] 以降、検出された製品バージョン [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27502</td><td>1041</td><td>[2] [3] に接続できませんでした。 [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27503</td><td>1041</td><td>[2] [3] のバージョン文字列読み取りエラー。 [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27504</td><td>1041</td><td>SQL バージョン要件が満たされていません: [3]. このインストールには [2] [4] 以降が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27505</td><td>1041</td><td>SQL スクリプトファイル [2] を開くことが出来ませんでした。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27506</td><td>1041</td><td>SQL スクリプト ［2］ の実行エラー。 [3] 行目。 [4]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27507</td><td>1041</td><td>データベースサーバーを参照する、またはそれに接続するには MDAC のインストールが必要です。セットアップが終了します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27508</td><td>1041</td><td>COM+ アプリケーション [2] のインストール中にエラーが発生しました。 [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27509</td><td>1041</td><td>COM+ アプリケーション [2] のアンインストール中にエラーが発生しました。 [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2751</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていた製品バージョン [4] より後、検出された製品バージョン [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27510</td><td>1041</td><td>COM+ アプリケーション [2] のインストール中にエラーが発生しました。 System.EnterpriseServices.RegistrationHelper オブジェクトを作成できませんでした。Microsoft(R) .NET サービス コンポーネントを登録するには、Microsoft(R) .NET Framework がインストールされている必要があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27511</td><td>1041</td><td>SQL スクリプト ファイル [2] を実行できませんでした。接続を開けません: [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27512</td><td>1041</td><td>[2] '[3]' のトランザクション開始中にエラーが発生しました。 データベース [4]。 [5]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27513</td><td>1041</td><td>[2] '[3]' のトランザクション実行中にエラーが発生しました。 データベース [4]。 [5]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27514</td><td>1041</td><td>このインストールを完了するには、Microsoft SQL Server が必要です。 指定されたサーバー '[3]' は、Microsoft SQL Server Desktop Engine またはSQL Server Express のいずれかです。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27515</td><td>1041</td><td>[2] '[3]' からスキーマ バージョンを取得中にエラーが発生しました。 データベース: '[4]'。 [5]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27516</td><td>1041</td><td>[2] '[3]' へスキーマ バージョンを書き込み中にエラーが発生しました。 データベース: '[4]'。 [5]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27517</td><td>1041</td><td>このインストールでは、COM+ アプリケーションのインストールを実行するために管理者権限が必要です。 管理者としてログオンしてからインストールを再試行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27518</td><td>1041</td><td>COM+ アプリケーション "[2]" は NT サービスとして実行するように構成されいるため、システム上に COM+ 1.5 以降が必要です。 システムには COM+ 1.0 が存在するため、アプリケーションをインストールすることはできません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27519</td><td>1041</td><td>XML ファイル [2] の更新中にエラーが発生しました。 [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2752</td><td>1041</td><td>パッケージ [4] の子ストレージとして格納されたトランスフォーム [2] を開けませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27520</td><td>1041</td><td>XML ファイル [2] を開く際にエラーが発生しました。 [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27521</td><td>1041</td><td>このセットアップでは、XML ファイルを構成するために MSXML 3.0 以降が必要です。 バージョン 3.0 以降があるかどうか確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27522</td><td>1041</td><td>XML ファイル [2] を作成中にエラーが発生しました。 [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27523</td><td>1041</td><td>サーバーのロード中にエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27524</td><td>1041</td><td>ログイン情報</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27525</td><td>1041</td><td>サーバーが見つかりません。 指定したサーバーが存在することを確認してください。 サーバー名を空白にすることはできません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27526</td><td>1041</td><td>ISNetApi.dll からの不特定エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27527</td><td>1041</td><td>バッファが小さすぎます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27528</td><td>1041</td><td>アクセスが拒否されました。 管理者権限を確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27529</td><td>1041</td><td>無効コンピューター。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2753</td><td>1041</td><td>ファイル '[2]' はインストール対象としてマークされていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27530</td><td>1041</td><td>NetAPI から不明なエラーが返されました。 システム エラー: [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27531</td><td>1041</td><td>未設定例外。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27532</td><td>1041</td><td>このサーバーまたはドメイン用のユーザー名が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27533</td><td>1041</td><td>大文字と小文字を区別するパスワードが一致しません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27534</td><td>1041</td><td>リストが空白です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27535</td><td>1041</td><td>アクセス違反。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27536</td><td>1041</td><td>グループの取得中にエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27537</td><td>1041</td><td>ユーザーをグループへ追加中にエラーが発生しました。このドメインまたはサーバーにグループが存在することを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27538</td><td>1041</td><td>ユーザー作成中にエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27539</td><td>1041</td><td>ERROR_NETAPI_ERROR_NOT_PRIMARY が NetAPI から戻りました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2754</td><td>1041</td><td>ファイル '[2]' は有効なパッチ ファイルではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27540</td><td>1041</td><td>指定したユーザーが既に存在します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27541</td><td>1041</td><td>指定したグループが既に存在します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27542</td><td>1041</td><td>パスワードが無効です。 パスワードがネットワークのパスワードポリシーに準拠していることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27543</td><td>1041</td><td>無効な名前。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27544</td><td>1041</td><td>無効なグループ。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27545</td><td>1041</td><td>ユーザー名は DOMAIN\Username の形式でなくてはなりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27546</td><td>1041</td><td>ユーザー TEMP ディレクトリに INI ファイルをロード中または作成中にエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27547</td><td>1041</td><td>ISNetAPI.dll  がロードされていない、または dll のロードエラーが発生しました。処理を実行するには、この dll のロードが必要です。dll が SUPPORTDIR ディレクトリに存在することを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27548</td><td>1041</td><td>新規ユーザー情報を含む INI ファイルを、ユーザーの TEMP ディレクトリから削除する際にエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27549</td><td>1041</td><td>プライマリ ドメイン コントローラ (PDC) の取得中にエラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2755</td><td>1041</td><td>パッケージ [3] をインストールするときに、サーバーが予期しないエラー [2] を返しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27550</td><td>1041</td><td>ユーザーを作成するには、全てのフィールドに値を入力してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27551</td><td>1041</td><td>[2] の ODBC ドライバが見つかりません。 これは [2] データベースサーバーへの接続に必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27552</td><td>1041</td><td>データベース [4] の作成の際に、エラーが発生しました。 サーバー。[2] [3]。[5]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27553</td><td>1041</td><td>データベース [4] への接続の際に、エラーが発生しました。 サーバー。[2] [3]。[5]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27554</td><td>1041</td><td>接続 [2] を開こうとして、エラーが発生しました。 この接続には有効なデータベース メタデータが関連付けられていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_27555</td><td>1041</td><td>オブジェクト '[2]' にアクセス許可を適用する際にエラーが発生しました。システム エラー: [3] ([4])</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2756</td><td>1041</td><td>プロパティ '[2]' は 1 つ以上のテーブルでディレクトリのプロパティとして使用されましたが、値は一度も割り当てられませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2757</td><td>1041</td><td>トランスフォーム [2] の概要情報を作成できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2758</td><td>1041</td><td>トランスフォーム [2] に、MSI バージョンがありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2759</td><td>1041</td><td>トランスフォーム [2] のバージョン [3] はエンジンと互換性がありません。最小: [4]、最大: [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2760</td><td>1041</td><td>トランスフォーム [2] はパッケージ [3] に無効です。 予期されていたアップグレード コード [4]。検出されたコード [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2761</td><td>1041</td><td>トランザクションを開始できません。 グローバル ミューテックスが適切に初期化されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2762</td><td>1041</td><td>スクリプトのレコードを書き込めません。トランザクションが開始されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2763</td><td>1041</td><td>スクリプトを実行できません。 トランザクションが開始されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2765</td><td>1041</td><td>AssemblyName テーブルにアセンブリ名がありません : コンポーネント: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2766</td><td>1041</td><td>ファイル [2] は、無効な MSI ストレージ ファイルです。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2767</td><td>1041</td><td>これ以上データはありません {[2] を列挙中}。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2768</td><td>1041</td><td>パッチ パッケージのトランスフォームが無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2769</td><td>1041</td><td>カスタム アクション [2] は、MSIHANDLE ([3]) を閉じませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2770</td><td>1041</td><td>キャッシュ フォルダー [2] が内部キャッシュ フォルダー テーブルで定義されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2771</td><td>1041</td><td>機能 [2] のアップグレードに、コンポーネントが 1 つ足りません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2772</td><td>1041</td><td>新しいアップグレード機能 [2] は、リーフ機能でなければなりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_28</td><td>1041</td><td>ファイル '[2]' は他のアプリケーションで使用されています。開いているアプリケーションをすべて閉じてから、「再試行」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2801</td><td>1041</td><td>不明なメッセージ -- 種類 [2]。 アクションは何も実行されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2802</td><td>1041</td><td>イベント [2] の発行元が見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2803</td><td>1041</td><td>ダイアログ ビューは、ダイアログ [2] のレコードを見つけられませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2804</td><td>1041</td><td>ダイアログ [2] のコントロール [3] のアクティブ化で、CMsiDialog が状態 [3] の評価に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2806</td><td>1041</td><td>ダイアログ [2] が状態 [3] の評価に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2807</td><td>1041</td><td>アクション [2] が認識されません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2808</td><td>1041</td><td>ダイアログ [2] にデフォルトのボタンが適切に定義されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2809</td><td>1041</td><td>ダイアログ [2] で、次のコントロール ポインタが循環参照を形成していません。 [3] から [4] へのポインタはありますが、それ以降のポインタがありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2810</td><td>1041</td><td>ダイアログ [2] で、次のコントロールを指すポインタが循環していません。 [3] と [5] の両方から [4] へのポインタがあります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2811</td><td>1041</td><td>ダイアログ [2] で、コントロール [3] がフォーカスを取得する必要がありますが、取得できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2812</td><td>1041</td><td>イベント [2] が認識されません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2813</td><td>1041</td><td>EndDialog イベントが引数 [2] で呼び出されましたが、ダイアログに親があります</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2814</td><td>1041</td><td>ダイアログ [2] で、コントロール [3] が、存在しないコントロール [4] を次のコントロールとして指定しています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2815</td><td>1041</td><td>ControlCondition テーブルに、ダイアログ [2] の状態がない行があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2816</td><td>1041</td><td>EventMapping テーブルが、イベント [3] についてダイアログ [2] の無効なコントロール [4] を参照しています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2817</td><td>1041</td><td>イベント [2] は、ダイアログ [3] のコントロール [4] の属性を設定できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2818</td><td>1041</td><td>ControlEvent テーブルの EndDialog に、認識されない引数 [2] があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2819</td><td>1041</td><td>ダイアログ [2] のコントロール [3] には、リンクするプロパティが必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2820</td><td>1041</td><td>既に初期化されているハンドラを初期化しようとしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2821</td><td>1041</td><td>既に初期化されているダイアログを初期化しようとしました: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2822</td><td>1041</td><td>ダイアログ [2] では、すべてのコントロールが追加されるまで、他のメソッドを呼び出せません</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2823</td><td>1041</td><td>既に初期化されているコントロールを初期化しようとしました: ダイアログ [2] の [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2824</td><td>1041</td><td>ダイアログの属性 [3] には、[2] 個以上のフィールドを持つレコードが必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2825</td><td>1041</td><td>コントロールの属性 [3] には、[2] 個以上のフィールドを持つレコードが必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2826</td><td>1041</td><td>ダイアログ [2] のコントロール [3] が、ダイアログの境界 [4] x [5] ピクセルを超えています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2827</td><td>1041</td><td>ダイアログ [2] のラジオ ボタン グループ [3] 内のボタン [4] が、グループの境界 [5] x [6] ピクセルを超えています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2828</td><td>1041</td><td>ダイアログ [2] からコントロール [3] を削除しようとしましたが、このコントロールはこのダイアログの一部ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2829</td><td>1041</td><td>初期化されていないダイアログを使用しようとしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2830</td><td>1041</td><td>ダイアログ [2] の初期化されていないコントロールを使用しようとしました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2831</td><td>1041</td><td>ダイアログ [2] のコントロール [3] では、[5] 属性 [4] はサポートされていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2832</td><td>1041</td><td>ダイアログ [2] では、属性 [3] はサポートされていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2833</td><td>1041</td><td>ダイアログ [3] のコントロール [4] で、メッセージ [2] が無視されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2834</td><td>1041</td><td>ダイアログ [2] で、次を指すポインタが単一のループになっていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2835</td><td>1041</td><td>ダイアログ [3] にコントロール [2] が見つかりませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2836</td><td>1041</td><td>ダイアログ [2] のコントロール [3] はフォーカスを取得できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2837</td><td>1041</td><td>ダイアログ [2] のコントロール [3] は、winproc が [4] を返すことを要求しています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2838</td><td>1041</td><td>選択テーブル内の項目 [2] が、それ自身を親として持っています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2839</td><td>1041</td><td>プロパティ [2] の設定に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2840</td><td>1041</td><td>ダイアログ名の不一致エラー。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2841</td><td>1041</td><td>エラー ダイアログに［OK］ボタンが見つかりませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2842</td><td>1041</td><td>エラー ダイアログにテキスト フィールドが見つかりませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2843</td><td>1041</td><td>標準ダイアログでは ErrorString 属性はサポートされていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2844</td><td>1041</td><td>エラー ダイアログを実行するには、エラー文字列を設定する必要があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2845</td><td>1041</td><td>ボタンの合計幅がエラー ダイアログのサイズを超えています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2846</td><td>1041</td><td>SetFocus は、必要なコントロールをエラー ダイアログ上に見つけられませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2847</td><td>1041</td><td>ダイアログ [2] のコントロール [3] に、アイコンとビットマップ形式の両方が設定されています｡</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2848</td><td>1041</td><td>コントロール [3] をダイアログ [2] のデフォルトのボタンとして設定しようとしましたが、そのコントロールが存在しません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2849</td><td>1041</td><td>ダイアログ [2] のコントロール [3] は、整数値を持てない種類のコントロールです｡</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2850</td><td>1041</td><td>ボリュームの種類が認識されません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2851</td><td>1041</td><td>アイコン [2] のデータが無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2852</td><td>1041</td><td>ダイアログ [2] を使用する前に、コントロールを 1 つ以上追加する必要があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2853</td><td>1041</td><td>ダイアログ [2] はモードレス ダイアログです。 これに対して実行メソッドを呼び出さないでください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2854</td><td>1041</td><td>ダイアログ [2] で、コントロール [3] が最初にアクティブなコントロールとして指定されていますが、このコントロールがありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2855</td><td>1041</td><td>ダイアログ [2] のラジオ ボタン グループ [3] にボタンが 1 個以下しかありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2856</td><td>1041</td><td>ダイアログ [2] の 2 つ目のコピーが作成されます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2857</td><td>1041</td><td>選択テーブルにディレクトリ [2] が記述されていますが、見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2858</td><td>1041</td><td>ビットマップ [2] のデータが無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2859</td><td>1041</td><td>テスト エラー メッセージ。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2860</td><td>1041</td><td>ダイアログ [2] に 「キャンセル」 ボタンが適切に定義されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2861</td><td>1041</td><td>ダイアログ [2] のコントロール [3] のラジオ ボタンで、次を指すポインタが循環していません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2862</td><td>1041</td><td>ダイアログ [2] のコントロール [3] の属性で、有効なアイコン サイズが定義されていません。 サイズは 16 に設定されます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2863</td><td>1041</td><td>ダイアログ [2] のコントロール [3] には、サイズ [5]x[5] の アイコン [4] が必要ですが、このサイズがありません。 最初に利用可能なサイズをロードします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2864</td><td>1041</td><td>ダイアログ [2] のコントロール [3] がブラウザ イベントを受け取りましたが、現在の選択に対する構成可能なディレクトリがありません。 考えられる原因: ブラウザ ボタンが正しく作成されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2865</td><td>1041</td><td>ビルボード [2] のコントロール [3] が、ビルボードの境界 [4] x [5] ピクセルを超えています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2866</td><td>1041</td><td>ダイアログ [2] では、引数 [3] を返せません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2867</td><td>1041</td><td>エラー ダイアログのプロパティが設定されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2868</td><td>1041</td><td>エラー ダイアログ [2] にエラーのスタイル ビットが設定されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2869</td><td>1041</td><td>ダイアログ [2] にエラーのスタイル ビットが設定されていますが、これはエラー ダイアログではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2870</td><td>1041</td><td>ダイアログ [2] のコントロール [3] のヘルプ文字列 [4] に、区切り文字が含まれていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2871</td><td>1041</td><td>[2] テーブルが古くなっています: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2872</td><td>1041</td><td>ダイアログ [2] の CheckPath コントロール イベントの引数が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2873</td><td>1041</td><td>ダイアログ [2] で、コントロール [3] に無効な文字列長制限が指定されています: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2874</td><td>1041</td><td>テキストのフォントを [2] に変更できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2875</td><td>1041</td><td>テキストの色を [2] に変更できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2876</td><td>1041</td><td>ダイアログ [2] のコントロール [3] で文字列が切り捨てられました: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2877</td><td>1041</td><td>バイナリ データ [2] が見つかりませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2878</td><td>1041</td><td>ダイアログ [2] で、コントロール [3] に次の可能な値が指定されています: [4]。 これは、無効であるか重複する値です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2879</td><td>1041</td><td>ダイアログ [2] のコントロール [3] で、マスク文字列を解析できません: [4]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2880</td><td>1041</td><td>残りのコントロール イベントは実行しないでください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2881</td><td>1041</td><td>CMsiHandler の初期化に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2882</td><td>1041</td><td>ダイアログのウィンドウ クラスの登録に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2883</td><td>1041</td><td>ダイアログ [2] の CreateNewDialog に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2884</td><td>1041</td><td>ダイアログ [2] のウィンドウの作成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2885</td><td>1041</td><td>ダイアログ [2] のコントロール [3] の作成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2886</td><td>1041</td><td>[2] テーブルの作成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2887</td><td>1041</td><td>[2] テーブルに対するカーソルの作成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2888</td><td>1041</td><td>[2] ビューの実行に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2889</td><td>1041</td><td>ダイアログ [2] のコントロール [3] のウィンドウの作成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2890</td><td>1041</td><td>ハンドラが、初期化されたダイアログの作成で失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2891</td><td>1041</td><td>ダイアログ [2] のウィンドウの破棄に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2892</td><td>1041</td><td>[2] は整数のみのコントロールです。[3] は有効な整数値ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2893</td><td>1041</td><td>ダイアログ [2] のコントロール [3] では、最高 [5] 文字までのプロパティ値が使用可能です｡ 値 [4] はこの制限を超えています｡そのため文字が切り捨てられました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2894</td><td>1041</td><td>RichEd20.dll の読み込みに失敗しました。GetLastError() の戻り値: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2895</td><td>1041</td><td>RichEd20.dll の解放に失敗しました。GetLastError() の戻り値: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2896</td><td>1041</td><td>アクション [2] の実行に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2897</td><td>1041</td><td>このシステムでの [2] フォントの作成に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2898</td><td>1041</td><td>[2] テキスト スタイルに対して、'[3]' フォントが [4] 文字セットで作成されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2899</td><td>1041</td><td>[2] テキストスタイルの作成に失敗しました。 GetLastError() はエラーを返しました: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_29</td><td>1041</td><td>ファイル [2] をインストールするために、十分なディスク領域がありません。ディスク領域を開放してから、「再試行」をクリックしてください。終了する場合は、「中止」をクリックします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2901</td><td>1041</td><td>操作 [2] に対するパラメータが無効です: パラメータ [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2902</td><td>1041</td><td>操作 [2] の呼び出し順序が間違っています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2903</td><td>1041</td><td>ファイル [2] が見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2904</td><td>1041</td><td>ファイル [2] の BindImage に失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2905</td><td>1041</td><td>スクリプト ファイル [2] からレコードを読み取れませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2906</td><td>1041</td><td>スクリプト ファイル [2] にヘッダーがありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2907</td><td>1041</td><td>セキュリティで保護されたセキュリティ記述子を作成できませんでした。エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2908</td><td>1041</td><td>コンポーネント [2] を登録できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2909</td><td>1041</td><td>コンポーネント [2] の登録を解除できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2910</td><td>1041</td><td>ユーザーのセキュリティ ID を特定できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2911</td><td>1041</td><td>フォルダー [2] を削除できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2912</td><td>1041</td><td>再起動時に、ファイル [2] を削除するよう設定できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2919</td><td>1041</td><td>圧縮ファイルにキャビネットが指定されていません: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2920</td><td>1041</td><td>ファイル [2] にソース ディレクトリが指定されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2924</td><td>1041</td><td>スクリプト [2] のバージョンはサポートされていません。 スクリプトのバージョン: [3]、最小バージョン: [4]、最大バージョン: [5]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2927</td><td>1041</td><td>ShellFolder ID [2] が無効です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2928</td><td>1041</td><td>ソースの最大数を超えています。ソース '[2]' はスキップされます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2929</td><td>1041</td><td>発行ルートを特定できませんでした。 エラー: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2932</td><td>1041</td><td>スクリプトのデータからファイル [2] を作成できませんでした。エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2933</td><td>1041</td><td>ロールバック スクリプト [2] を初期化できませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2934</td><td>1041</td><td>トランスフォーム [2] セキュリティで保護できませんでした。エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2935</td><td>1041</td><td>トランスフォーム [2] の保護を解除できませんでした。エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2936</td><td>1041</td><td>トランスフォーム [2] が見つかりませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2937</td><td>1041</td><td>Windows Installer は、システム ファイル保護のカタログをインストールできません。カタログ: [2]、エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2938</td><td>1041</td><td>Windows Installer は、システム ファイル保護のカタログをキャッシュから取得できません。 カタログ: [2]、エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2939</td><td>1041</td><td>Windows Installer は、システム ファイル保護のカタログをキャッシュから削除できません。 カタログ: [2]、エラー: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2940</td><td>1041</td><td>ディレクトリ マネージャがソースの解決に指定されていません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2941</td><td>1041</td><td>ファイル [2] の CRC を計算できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2942</td><td>1041</td><td>BindImage アクションが [2] ファイルに実行されませんでした。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2943</td><td>1041</td><td>このバージョンの Windows では 64 ビット パッケージの展開はサポートされていません。 スクリプト [2] は 64 ビット パッケージ用です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2944</td><td>1041</td><td>GetProductAssignmentType が失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_2945</td><td>1041</td><td>ComPlus アプリケーション [2] のインストールに失敗しました。エラー コード: [3]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_3</td><td>1041</td><td>情報 [1]。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_30</td><td>1041</td><td>ソース ファイル [2] が見つかりません。ファイルが存在するかどうか、およびこのファイルへのアクセス権があるかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_3001</td><td>1041</td><td>このリストのパッチには、正しくないシーケンス情報が含まれています: [2][3][4][5][6][7][8][9][10][11][12][13][14][15][16]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_3002</td><td>1041</td><td>パッチ [2] に無効なシーケンス情報が含まれています。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_31</td><td>1041</td><td>ファイル [3] からの読み込みに失敗しました。{{ システム エラー [2]。}} ファイルが存在するかどうか、およびこのファイルへのアクセス権があるかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_32</td><td>1041</td><td>ファイル [3] への書き込みに失敗しました。{{ システム エラー[2]。}} このディレクトリへのアクセス権があることを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_33</td><td>1041</td><td>ソース ファイルが見つかりません。{{(cabinet)}} : [2]。 ファイルが存在するかどうか、およびこのファイルへのアクセス権があるかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_34</td><td>1041</td><td>ディレクトリ '[2]' を作成できません。この名前を持つファイルがすでに存在しています。ファイルの名前を変更するか、削除してから、「再試行」をクリックしてください。または「キャンセル」をクリックして、終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_35</td><td>1041</td><td>ボリューム [2] は現在使用できません。別のボリュームを選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_36</td><td>1041</td><td>指定されたパス '[2]' は使用できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_37</td><td>1041</td><td>指定されたフォルダーに書き込めません :  '[2]'。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_38</td><td>1041</td><td>次のファイルを読み込み中に、ネットワーク エラーが発生しました : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_39</td><td>1041</td><td>ディレクトリ [2] を作成中に、エラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_4</td><td>1041</td><td>内部エラー [1]。 [2]{, [3]}{, [4]}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_40</td><td>1041</td><td>ディレクトリ [2] を作成中に、ネットワーク エラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_41</td><td>1041</td><td>ソース ファイル キャビネット [2] を開いている途中で、ネットワーク エラーが発生しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_42</td><td>1041</td><td>指定されたパスは長すぎます : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_43</td><td>1041</td><td>インストール プログラムには、ファイル [2] を修正する権限がありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_44</td><td>1041</td><td>フォルダー パス '[2]' の一部が不正です。パスが空であるかまたは長すぎるので、このシステムでは指定できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_45</td><td>1041</td><td>フォルダー パス '[2]' に使用されている単語の中に、フォルダー パスでは使用できないものが含まれています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_46</td><td>1041</td><td>フォルダー パス '[2]' に使用できない文字が含まれています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_47</td><td>1041</td><td>[2]' はショート ファイル名としては正しくありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_48</td><td>1041</td><td>ファイル セキュリティの取得に失敗しました : [3] GetLastError: [2]。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_49</td><td>1041</td><td>ドライブが正しくありません : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_5</td><td>1041</td><td>{{ディスクがいっぱいです : }}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_50</td><td>1041</td><td>キー [2] を作成できません。{{ システム エラー [3]。}} そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_51</td><td>1041</td><td>キー [2] を開くことができません。{{ システム エラー [3]。}} そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_52</td><td>1041</td><td>キー [3] から 値 [2] を削除できません。{{ システム エラー [4]。}}  そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_53</td><td>1041</td><td>キー [2] を削除できません。{{ システム エラー [3]。}} そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_54</td><td>1041</td><td>キー [3] から 値 [2] を読み込めません。{{ システム エラー [4]。}}  そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_55</td><td>1041</td><td>値 [2] をキー [3] に書き込めません。{{ システム エラー [4]。}}  そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_56</td><td>1041</td><td>キー [2] に対する値の名前を得ることができません。 {{ システム エラー [3]。}}  そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_57</td><td>1041</td><td>キー [2] に対するサブ キー名を得ることができません。 {{ システム エラー [3]。}}  そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_58</td><td>1041</td><td>キー [2] に対するセキュリティ情報が読めません。{{ システム エラー [3]。}}  そのキーへの十分なアクセス権を持っているかどうかを確認するか、またはサポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_59</td><td>1041</td><td>使用可能なレジストリ領域を増やすことはできません。このアプリケーションをインストールするには、[2] KB のレジストリ領域が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_6</td><td>1041</td><td>アクション [Time]: [1]。 [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_60</td><td>1041</td><td>別のインストールが現在行われています。このインストールが完了しなければ、新たにインストールを行うことはできません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_61</td><td>1041</td><td>保護されているデータへのアクセスでエラーが発生しました。Windows Installer が正しく設定されていることを確認してから、再度インストールを実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_62</td><td>1041</td><td>ユーザー '[2]' によって、すでに '[3]' のインストールが開始されています。このユーザーは、この製品を使う前に再度インストールを実行する必要があります。現在のインストールはこのまま継続されます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_63</td><td>1041</td><td>ユーザー '[2]' によって、すでに '[3]' のインストールが開始されています。このユーザーは、この製品を使う前に再度インストールを実行する必要があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_64</td><td>1041</td><td>ディスク領域が足りません -- ボリューム : '[2]'; 必要な領域 : [3] KB; 使用可能な領域 : [4] KB。ディスク領域を増やしてから、再度実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_65</td><td>1041</td><td>キャンセルしますか ?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_66</td><td>1041</td><td>ファイル [2][3] は使用中です {プロセス名 : [4], Id : [5], ウィンドウ タイトル : '[6]'} このアプリケーションを終了してから、再度実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_67</td><td>1041</td><td>'[2]' がすでにインストールされているため、この製品のインストールを妨げています。この 2 つの製品を 1 台のコンピュータに同時に存在させることはできません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_68</td><td>1041</td><td>ディスク領域が足りません -- ボリューム : '[2]'; 必要な領域 : [3] KB; 使用可能な領域 : [4] KB。 ロールバックを使用できないようにすると、十分な領域を確保できます。終了する場合は「キャンセル」、再度使用可能なディスク領域を確認する場合は「再試行」、ロールバックせずに継続する場合は「無視」 をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_69</td><td>1041</td><td>ネットワーク ロケーション [2] へアクセスできません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_7</td><td>1041</td><td>[ProductName]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_70</td><td>1041</td><td>インストールを継続するには、次のアプリケーションを閉じる必要があります :</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_71</td><td>1041</td><td>このマシンにこの製品をインストールするのに、前もってインストールされていなければならない製品が見つかりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_72</td><td>1041</td><td>キー [2] は正しくありません。正しいキーが入力されているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_73</td><td>1041</td><td>[2] の設定を継続する前に、システムを再起動する必要があります。すぐに再起動する場合は「はい」を、後で手動で再起動する場合は「いいえ」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_74</td><td>1041</td><td>[2] に対する設定変更を有効にするには、システムを再起動する必要があります。すぐに再起動する場合は「はい」を、後で手動で再起動する場合は「いいえ」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_75</td><td>1041</td><td>[2] のインストールは現在中断されています。このまま継続するには、インストールによって行われた変更を取り消す必要があります。これらの変更を取り消しますか ?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_76</td><td>1041</td><td>この製品の前のインストールは現在進行中です。このまま継続するには、このインストールによって行われた変更を取り消す必要があります。これらの変更を取り消しますか ?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_77</td><td>1041</td><td>製品 [2] に対する正式なソースが見つかりません。このインストレーション パッケージ '[3]' の正規のコピーを使用して、再度インストールを実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_78</td><td>1041</td><td>インストールは正常に終了しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_79</td><td>1041</td><td>インストールは失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_8</td><td>1041</td><td>{[2]}{, [3]}{, [4]}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_80</td><td>1041</td><td>製品 : [2] -- [3]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_81</td><td>1041</td><td>コンピュータを直前の状態に復元するか、後でインストールを継続することができます。復元しますか ?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_82</td><td>1041</td><td>インストール情報をディスクに書き込み中にエラーが発生しました。十分なディスク領域があることを確認してから、「再試行」をクリックして継続するか、または「キャンセル」をクリックしてインストールを中止してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_83</td><td>1041</td><td>お使いのコンピュータを直前の状態に復元するために必要なファイルの一部が見つかりません。復元することはできません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_84</td><td>1041</td><td>パス [2] が正しくありません。正しいパスを指定してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_85</td><td>1041</td><td>メモリ不足です。他のアプリケーションを閉じてから、再度実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_86</td><td>1041</td><td>ドライブ [2] にはディスクがありません。ディスクを挿入してから 「再試行」をクリックしてください。ボリュームの選択に戻るには、「キャンセル」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_87</td><td>1041</td><td>ドライブ [2] にはディスクがありません。ディスクを挿入してから「再試行」をクリックしてください。「検索」ダイアログ ボックスに戻って、別のボリュームを選択する場合は、「キャンセル」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_88</td><td>1041</td><td>フォルダー [2] は存在しません。既存のフォルダーへのパスを入力してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_89</td><td>1041</td><td>このフォルダーを読み込むには、アクセス権が不十分です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_9</td><td>1041</td><td>メッセージの種類 : [1], 引数 : [2]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_90</td><td>1041</td><td>正しいインストール先のフォルダーが確定できません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_91</td><td>1041</td><td>ソース インストール データベース [2] からの読み込みに失敗しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_92</td><td>1041</td><td>再起動操作のスケジューリング : ファイル [2] の名前を [3] に変更。操作を完了するには、再起動しなければなりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_93</td><td>1041</td><td>再起動時の操作のスケジューリング : ファイル [2] の削除。操作を完了するには、再起動しなければなりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_94</td><td>1041</td><td>モジュール [2] で登録に失敗しました。 HRESULT [3]。サポート担当者へお問い合わせください。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_95</td><td>1041</td><td>モジュール [2] で登録の解除に失敗しました。HRESULT [3]。サポート担当者へお問い合わせください。 </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_96</td><td>1041</td><td>パッケージ [2] のキャッシュに失敗しました。エラー : [3]。サポート担当者へお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_97</td><td>1041</td><td>フォント [2] を登録できません。このフォントをインストールする権利があるかどうか、またシステムがこのフォントをサポートしているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_98</td><td>1041</td><td>フォント [2] の登録を取り消すことができません。フォントを削除する権利があるかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ERROR_99</td><td>1041</td><td>ショートカット [2] を作成できません。目的のフォルダーが存在するか、またこのフォルダーへのアクセス権を持っているかどうかを確認してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_INSTALLDIR</td><td>1041</td><td>{&amp;Tahoma9}[INSTALLDIR]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_INSTALLSHIELD</td><td>1041</td><td>InstallShield</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_INSTALLSHIELD_FORMATTED</td><td>1041</td><td>{&amp;MSSWhiteSerif8}InstallShield</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ISSCRIPT_VERSION_MISSING</td><td>1041</td><td>このﾏｼﾝには InstallScript ｴﾝｼﾞﾝがありません。ISScript.msi がある場合はそれを実行し、ない場合はｻﾎﾟｰﾄ担当者にお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_ISSCRIPT_VERSION_OLD</td><td>1041</td><td>このﾏｼﾝにある InstallScript ｴﾝｼﾞﾝは、このｾｯﾄｱｯﾌﾟを実行するのに必要なﾊﾞｰｼﾞｮﾝより古いものです。最新ﾊﾞｰｼﾞｮﾝの ISScript.msi がある場合はそれを実行し、ない場合はｻﾎﾟｰﾄ担当者にお問い合わせください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_NEXT</td><td>1041</td><td>{&amp;Tahoma9}次へ(&amp;N) &gt;</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_OK</td><td>1041</td><td>{&amp;Tahoma9}OK</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PREREQUISITE_SETUP_BROWSE</td><td>1041</td><td>[ProductName] の元の [SETUPEXENAME] を開く</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PREREQUISITE_SETUP_INVALID</td><td>1041</td><td>この実行可能ファイルは、[ProductName] の元の実行可能ファイルではないようです。 元の [SETUPEXENAME] を使用せずに追加の依存関係をインストールすると、[ProductName] が適切に動作しない可能性があります。 元の [SETUPEXENAME] を検索しますか?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PREREQUISITE_SETUP_SEARCH</td><td>1041</td><td>このインストールは、追加の依存関係を必要とする可能性があります。 依存関係なしでは、[ProductName] が適切に動作しない可能性があります。 元の [SETUPEXENAME] を検索しますか?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PREVENT_DOWNGRADE_EXIT</td><td>1041</td><td>このコンピュータには、このアプリケーションのより新しいバージョンが既にインストールされています。 このバージョンをインストールするには、まずその新しいバージョンをアンインストールしてください。 「OK」 をクリックするとウィザードが閉じます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PRINT_BUTTON</td><td>1041</td><td>印刷(&amp;P)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PRODUCTNAME_INSTALLSHIELD</td><td>1041</td><td>[ProductName] - InstallShield Wizard</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEAPPPOOL</td><td>1041</td><td>アプリケーション プール %s の作成</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEAPPPOOLS</td><td>1041</td><td>アプリケーション プールの作成</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEVROOT</td><td>1041</td><td>IIS 仮想ディレクトリ %s の作成</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEVROOTS</td><td>1041</td><td>IIS 仮想ディレクトリを作成しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEWEBSERVICEEXTENSION</td><td>1041</td><td>Web サービス拡張の作成</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEWEBSERVICEEXTENSIONS</td><td>1041</td><td>Web サービス拡張を作成しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEWEBSITE</td><td>1041</td><td>IIS Web サイト %s を作成しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_CREATEWEBSITES</td><td>1041</td><td>IIS Web サイトを作成しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_EXTRACT</td><td>1041</td><td>IIS 仮想ディレクトリの情報を抽出しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_EXTRACTDONE</td><td>1041</td><td>IIS 仮想ディレクトリ用に抽出された情報...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEAPPPOOL</td><td>1041</td><td>アプリケーション プールの削除</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEAPPPOOLS</td><td>1041</td><td>アプリケーション プールを削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVESITE</td><td>1041</td><td>ポート %d で Web サイトを削除します</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEVROOT</td><td>1041</td><td>IIS 仮想ディレクトリ %s の削除</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEVROOTS</td><td>1041</td><td>IIS 仮想ディレクトリを削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEWEBSERVICEEXTENSION</td><td>1041</td><td>Web サービス拡張の削除</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEWEBSERVICEEXTENSIONS</td><td>1041</td><td>Web サービス拡張を削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_REMOVEWEBSITES</td><td>1041</td><td>IIS Web サイトを削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_ROLLBACKAPPPOOLS</td><td>1041</td><td>アプリケーション プールをロールバックしています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_ROLLBACKVROOTS</td><td>1041</td><td>仮想ディレクトリ、および Web サイトの変更をロールバックしています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_IIS_ROLLBACKWEBSERVICEEXTENSIONS</td><td>1041</td><td>Web サービス拡張をロールバックしています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_TEXTFILECHANGS_REPLACE</td><td>1041</td><td>%s を %s で置換しています (%s 中)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_XML_COSTING</td><td>1041</td><td>XML ファイルのコスト分析...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_XML_CREATE_FILE</td><td>1041</td><td>XML ファイル %s を作成しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_XML_FILES</td><td>1041</td><td>XML ファイルの変更を行っています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_XML_REMOVE_FILE</td><td>1041</td><td>XML ファイル %s を削除しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_XML_ROLLBACK_FILES</td><td>1041</td><td>XML ファイルの変更をロールバックしています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_PROGMSG_XML_UPDATE_FILE</td><td>1041</td><td>XML ファイル %s を更新しています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SETUPEXE_EXPIRE_MSG</td><td>1041</td><td>このセットアップは %s まで有効です。セットアップを終了します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SHORTCUT_DISPLAY_NAME1</td><td>1041</td><td>Tween</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SHORTCUT_DISPLAY_NAME2</td><td>1041</td><td/><td>0</td><td/><td>-199264536</td></row>
		<row><td>IDS_SHORTCUT_DISPLAY_NAME3</td><td>1041</td><td>Tween</td><td>0</td><td/><td>-199261875</td></row>
		<row><td>IDS_SHORTCUT_DISPLAY_NAME4</td><td>1041</td><td>Tween</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SHORTCUT_DISPLAY_NAME5</td><td>1033</td><td/><td>0</td><td/><td>-199274163</td></row>
		<row><td>IDS_SHORTCUT_DISPLAY_NAME5</td><td>1041</td><td>Launch |Built</td><td>0</td><td/><td>-199274163</td></row>
		<row><td>IDS_SQLBROWSE_INTRO</td><td>1041</td><td>以下のサーバーリストから、ターゲットにするデータベースサーバーを選択します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLBROWSE_INTRO_DB</td><td>1041</td><td>以下のカタログ名一覧から、ターゲットにするデータベース カタログを選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLBROWSE_INTRO_TEMPLATE</td><td>1041</td><td>[IS_SQLBROWSE_INTRO]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_BROWSE</td><td>1041</td><td>参照(&amp;R)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_BROWSE_DB</td><td>1041</td><td>参照(&amp;O)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_CATALOG</td><td>1041</td><td>データベース カタログ名(&amp;N):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_CONNECT</td><td>1041</td><td>接続方法:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_DESC</td><td>1041</td><td>データベースサーバーおよび認証方法を選択します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_ID</td><td>1041</td><td>ログイン ID（&amp;L）:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_INTRO</td><td>1041</td><td>下のリストからインストール先データベースサーバーを選択するか、［参照］ をクリックしてデータベースサーバーのリストを表示します。現在の認証情報、または SQL ログイン ID およびパスワードを利用したログインの認証方法を指定することもできます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_PSWD</td><td>1041</td><td>パスワード(&amp;P):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_SERVER</td><td>1041</td><td>データベースサーバー（&amp;D）:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_SERVER2</td><td>1041</td><td>インストール先データベース サーバー(&amp;D):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_SQL</td><td>1041</td><td>次のログイン ID およびパスワードを利用した S&amp;erver 認証</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_TITLE</td><td>1041</td><td>{&amp;Tahoma9}データベースサーバー</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLLOGIN_WIN</td><td>1041</td><td>現在のユーザーの Windows 認証情報（&amp;W）</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLSCRIPT_INSTALLING</td><td>1041</td><td>SQL インストールスクリプトを実行中…</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SQLSCRIPT_UNINSTALLING</td><td>1041</td><td>SQL アンインストールスクリプトを実行中…</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_STANDARD_USE_SETUPEXE</td><td>1041</td><td>このｲﾝｽﾄｰﾙは MSI ﾊﾟｯｹｰｼﾞを直接起動して実行できません。setup.exe を実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_Advertise</td><td>1041</td><td>{&amp;Tahoma9}最初の使用時にインストールします。(機能がこのオプションをサポートする場合のみ有効です)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_AllInstalledLocal</td><td>1041</td><td>{&amp;Tahoma9}ローカルのハードディスク ドライブにすべての機能をインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_CustomSetup</td><td>1041</td><td>{&amp;Tahoma9}カスタム セットアップ ティップ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_CustomSetupDescription</td><td>1041</td><td>{&amp;Tahoma9}カスタム セットアップを選択すると、必要なプログラム機能のみをインストールすることができます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_IconInstallState</td><td>1041</td><td>{&amp;Tahoma9}機能名の隣にあるアイコンは、機能のインストール状態を示しています。アイコンをクリックして、各機能の「インストール状態」メニューをドロップダウンしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_InstallState</td><td>1041</td><td>{&amp;Tahoma9}各アイコンは、次のインストール状態を示します...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_Network</td><td>1041</td><td>{&amp;Tahoma9}ネットワークから実行するようにインストールします。(機能がこのオプションをサポートする場合のみ有効です)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_OK</td><td>1041</td><td>{&amp;Tahoma9}OK</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_SubFeaturesInstalledLocal</td><td>1041</td><td>{&amp;Tahoma9}ローカルのハードディスク ドライブにサブ機能をインストールします。(機能に、サブ機能が含まれる場合のみ有効です)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_SetupTips_WillNotBeInstalled</td><td>1041</td><td>{&amp;Tahoma9}インストールしません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_Available</td><td>1041</td><td>使用可能なディスク容量</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_Bytes</td><td>1041</td><td>{&amp;Tahoma9}バイト</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_CompilingFeaturesCost</td><td>1041</td><td>{&amp;Tahoma9}この機能に対するコストをコンパイルしています...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_Differences</td><td>1041</td><td>残り</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_DiskSize</td><td>1041</td><td>ディスク サイズ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureCompletelyRemoved</td><td>1041</td><td>{&amp;Tahoma9}この機能をすべて削除します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureContinueNetwork</td><td>1041</td><td>{&amp;Tahoma9}この機能を継続してネットワークから実行するようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureFreeSpace</td><td>1041</td><td>{&amp;Tahoma9}この機能は、ハードディスク ドライブの [1] を開放します。.</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledCD</td><td>1041</td><td>この機能、およびすべてのサブ機能をCDから実行するようにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledCD2</td><td>1041</td><td>この機能をCDから実行するようにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledLocal</td><td>1041</td><td>この機能、およびすべてのサブ機能をローカルのハードディスク ドライブにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledLocal2</td><td>1041</td><td>この機能をローカルのハードディスク ドライブにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledNetwork</td><td>1041</td><td>この機能、およびすべてのサブ機能をネットワークから実行するようにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledNetwork2</td><td>1041</td><td>この機能をネットワークから実行するようにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledRequired</td><td>1041</td><td>{&amp;Tahoma9}要求があった場合に、インストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledWhenRequired</td><td>1041</td><td>{&amp;Tahoma9}要求があった場合に、この機能をインストールようにセットします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureInstalledWhenRequired2</td><td>1041</td><td>要求があった場合に、この機能をインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureLocal</td><td>1041</td><td>{&amp;Tahoma9}この機能をローカルのハードディスク ドライブにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureLocal2</td><td>1041</td><td>{&amp;Tahoma9}この機能をローカルのハードディスク ドライブにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureNetwork</td><td>1041</td><td>{&amp;Tahoma9}この機能をネットワークから実行するようにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureNetwork2</td><td>1041</td><td>{&amp;Tahoma9}この機能をネットワークから実行できるようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureNotAvailable</td><td>1041</td><td>この機能を使用できないようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureOnCD</td><td>1041</td><td>{&amp;Tahoma9}この機能をCDから実行するようにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureOnCD2</td><td>1041</td><td>{&amp;Tahoma9}この機能をCDから実行できるようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureRemainLocal</td><td>1041</td><td>{&amp;Tahoma9}この機能は、ローカルのハードディスク ドライブから削除されません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureRemoveNetwork</td><td>1041</td><td>{&amp;Tahoma9}この機能をローカルのハードディスク ドライブから削除しますが、ネットワークからは実行できるようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureRemovedCD</td><td>1041</td><td>{&amp;Tahoma9}この機能をローカルのハードディスク ドライブから削除しますが、CDからは実行できるようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureRemovedUnlessRequired</td><td>1041</td><td>{&amp;Tahoma9}この機能をローカルのハードディスク ドライブから削除して、要求があった場合にインストールするようにセットします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureRequiredSpace</td><td>1041</td><td>{&amp;Tahoma9}この機能をインストールするには、ローカルのハードディスク ドライブに [1] が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureRunFromCD</td><td>1041</td><td>{&amp;Tahoma9}この機能を継続してCDから実行するようにします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureSpaceFree</td><td>1041</td><td>{&amp;Tahoma9}この機能は、[1] を開放します。[2] / [3] のサブ機能が選択されています。サブ機能は、[4] を開放します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureSpaceFree2</td><td>1041</td><td>{&amp;Tahoma9}この機能は、[1] を開放します。[2] / [3] のサブ機能が選択されています。サブ機能をインストールするには、[4] が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureSpaceFree3</td><td>1041</td><td>{&amp;Tahoma9}この機能をインストールするには、[1] が必要です。[2] / [3] のサブ機能が選択されています。サブ機能は、[4] を開放します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureSpaceFree4</td><td>1041</td><td>{&amp;Tahoma9}この機能をインストールするには、[1] が必要です。[2] / [3] のサブ機能が選択されています。サブ機能には、[4] が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureUnavailable</td><td>1041</td><td>{&amp;Tahoma9}この機能は、使用できないようになります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureUninstallNoNetwork</td><td>1041</td><td>{&amp;Tahoma9}この機能を完全にアンインストールします。ネットワークから実行できないようになります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureWasCD</td><td>1041</td><td>{&amp;Tahoma9}この機能はCDから実行されましたが、要求があった場合にインストールするようにセットします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureWasCDLocal</td><td>1041</td><td>{&amp;Tahoma9}この機能はCDから実行されましたが、ローカルのハードディスク ドライブにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureWasOnNetworkInstalled</td><td>1041</td><td>{&amp;Tahoma9}この機能はネットワークから実行されましたが、要求があった場合にインストールするようにセットします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureWasOnNetworkLocal</td><td>1041</td><td>{&amp;Tahoma9}この機能はネットワークから実行されましたが、ローカルのハードディスク ドライブにインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_FeatureWillBeUninstalled</td><td>1041</td><td>{&amp;Tahoma9}この機能を完全にアンインストールします。CDから実行できないようになります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_Folder</td><td>1041</td><td>{&amp;Tahoma9}フォルダー|フォルダーの新規作成</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_GB</td><td>1041</td><td>GB</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_KB</td><td>1041</td><td>KB</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_MB</td><td>1041</td><td>MB</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_Required</td><td>1041</td><td>必要なディスク容量</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_TimeRemaining</td><td>1041</td><td>{&amp;Tahoma9}残り時間: {[1] 分 }{[2] 秒}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS_UITEXT_Volume</td><td>1041</td><td>ボリューム</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__AgreeToLicense_0</td><td>1041</td><td>{&amp;Tahoma9}使用許諾契約の条項に同意しません(&amp;D)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__AgreeToLicense_1</td><td>1041</td><td>{&amp;Tahoma9}使用許諾契約の条項に同意します(&amp;A)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DatabaseFolder_ChangeFolder</td><td>1041</td><td>{&amp;Tahoma9}このフォルダへインストールする場合は、「次へ」 をクリックしてください。別のフォルダへインストールする場合は、「変更」をクリックします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DatabaseFolder_DatabaseDir</td><td>1041</td><td>{&amp;Tahoma9}[DATABASEDIR]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DatabaseFolder_DatabaseFolder</td><td>1041</td><td>{&amp;Tahoma9}データベース フォルダ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DestinationFolder_Change</td><td>1041</td><td>{&amp;Tahoma9}変更(&amp;C)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DestinationFolder_ChangeFolder</td><td>1041</td><td>{&amp;Tahoma9}このフォルダにインストールする場合は、「次へ」をクリックしてください。 別のフォルダにインストールする場合は、「変更」をクリックします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DestinationFolder_DestinationFolder</td><td>1041</td><td>{&amp;Tahoma9}インストール先のフォルダ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DestinationFolder_InstallTo</td><td>1041</td><td>{&amp;Tahoma9}[ProductName] のインストール先:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DisplayName_Custom</td><td>1041</td><td>カスタム</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DisplayName_Minimal</td><td>1041</td><td>最小</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__DisplayName_Typical</td><td>1041</td><td>標準</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_11</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_4</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_8</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_BrowseDestination</td><td>1041</td><td>{&amp;Tahoma9}インストール先フォルダを参照してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_ChangeDestination</td><td>1041</td><td>{&amp;Tahoma9}インストール先フォルダの変更</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_CreateFolder</td><td>1041</td><td>フォルダーの新規作成|</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_FolderName</td><td>1041</td><td>{&amp;Tahoma9}フォルダ名(&amp;F):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_LookIn</td><td>1041</td><td>{&amp;Tahoma9}探す場所(&amp;L):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallBrowse_UpOneLevel</td><td>1041</td><td>1つ上のレベルに移動|</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPointWelcome_ServerImage</td><td>1041</td><td>{&amp;Tahoma9}InstallShield(R) ウィザードは、指定したネットワーク ロケーションに [ProductName] のサーバーイメージを作成します。続行するには、「次へ」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPointWelcome_Wizard</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]用のInstallShield ウィザードへようこそ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPoint_Change</td><td>1041</td><td>{&amp;Tahoma9}変更(&amp;C)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPoint_EnterNetworkLocation</td><td>1041</td><td>{&amp;Tahoma9}ネットワーク ロケーションを入力するか、あるいは「変更」をクリックしてロケーションを参照してください。「インストール」をクリックすると、指定したネットワーク ロケーションに [ProductName] のサーバーイメージを作成します。「キャンセル」をクリックすると、ウィザードを終了します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPoint_Install</td><td>1041</td><td>{&amp;Tahoma9}インストール(&amp;I)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPoint_NetworkLocation</td><td>1041</td><td>{&amp;Tahoma9}ネットワーク ロケーション(&amp;N):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPoint_NetworkLocationFormatted</td><td>1041</td><td>{&amp;Tahoma9}ネットワーク ロケーション</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsAdminInstallPoint_SpecifyNetworkLocation</td><td>1041</td><td>{&amp;Tahoma9}製品のサーバーイメージに対するネットワーク ロケーションを指定してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseButton</td><td>1041</td><td>参照(&amp;B)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_11</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_4</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_8</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_BrowseDestFolder</td><td>1041</td><td>{&amp;Tahoma9}インストール先フォルダーを参照してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_ChangeCurrentFolder</td><td>1041</td><td>{&amp;Tahoma9}インストール先フォルダーの変更</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_CreateFolder</td><td>1041</td><td>フォルダーの新規作成|</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_FolderName</td><td>1041</td><td>{&amp;Tahoma9}フォルダー名(&amp;F):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_LookIn</td><td>1041</td><td>{&amp;Tahoma9}探す場所(&amp;L):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_OK</td><td>1041</td><td>{&amp;Tahoma9}OK</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseFolderDlg_UpOneLevel</td><td>1041</td><td>1つ上のレベルに移動|</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseForAccount</td><td>1041</td><td>ユーザーアカウントの参照</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseGroup</td><td>1041</td><td>ユーザーリスト グループを選択します</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsBrowseUsernameTitle</td><td>1041</td><td>ユーザー名の選択</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCancelDlg_ConfirmCancel</td><td>1041</td><td>{&amp;Tahoma9}[ProductName] のインストールを中断してもよろしいですか ?</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCancelDlg_No</td><td>1041</td><td>{&amp;Tahoma9}いいえ(&amp;N)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCancelDlg_Yes</td><td>1041</td><td>{&amp;Tahoma9}はい(&amp;Y)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsConfirmPassword</td><td>1041</td><td>パスワードの確認(&amp;F):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCreateNewUserTitle</td><td>1041</td><td>新規ユーザー情報</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCreateUserBrowse</td><td>1041</td><td>新規ユーザー情報 (&amp;E)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_Change</td><td>1041</td><td>{&amp;Tahoma9}変更(&amp;C)...</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_ClickFeatureIcon</td><td>1041</td><td>{&amp;Tahoma9}下のリストにあるアイコンをクリックして、機能のインストール方法を変更してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_CustomSetup</td><td>1041</td><td>{&amp;Tahoma9}カスタム セットアップ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_FeatureDescription</td><td>1041</td><td>{&amp;Tahoma9}機能の説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_FeaturePath</td><td>1041</td><td>{&amp;Tahoma9}&lt;selected feature path&gt;</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_FeatureSize</td><td>1041</td><td>{&amp;Tahoma9}機能のサイズ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_Help</td><td>1041</td><td>{&amp;Tahoma9}ヘルプ(&amp;H)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_InstallTo</td><td>1041</td><td>{&amp;Tahoma9}インストール先:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_MultilineDescription</td><td>1041</td><td>{&amp;Tahoma9}選択しているアイテムの説明</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_SelectFeatures</td><td>1041</td><td>{&amp;Tahoma9}インストールするプログラムの機能を選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsCustomSelectionDlg_Space</td><td>1041</td><td>{&amp;Tahoma9}ディスク(&amp;U)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsDiskSpaceDlg_DiskSpace</td><td>1041</td><td>{&amp;Tahoma9}インストールには、現在使用可能なディスク領域よりも多くのディスク空き領域が必要です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsDiskSpaceDlg_HighlightedVolumes</td><td>1041</td><td>{&amp;Tahoma9}現在のボリュームには、選択している機能をインストールするために十分なディスク空き領域がありません。現在のボリュームからファイルを削除してディスク領域を確保するか、ローカル ディスクにインストールする機能を削減してください。あるいは、別のドライブを選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsDiskSpaceDlg_Numbers</td><td>1041</td><td>{&amp;Tahoma9}{120}{70}{70}{70}{70}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsDiskSpaceDlg_OK</td><td>1041</td><td>{&amp;Tahoma9}&amp;OK</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsDiskSpaceDlg_OutOfDiskSpace</td><td>1041</td><td>{&amp;Tahoma9}ディスク空き領域の不足</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsDomainOrServer</td><td>1041</td><td>ドメインまたはサーバー(&amp;D):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_Abort</td><td>1041</td><td>{&amp;Tahoma9}中止(&amp;A)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_ErrorText</td><td>1041</td><td>{&amp;Tahoma9}&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;&lt;error text goes here&gt;</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_Ignore</td><td>1041</td><td>{&amp;Tahoma9}無視(&amp;I)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_InstallerInfo</td><td>1041</td><td>[ProductName] のInstaller 情報</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_NO</td><td>1041</td><td>{&amp;Tahoma9}いいえ(&amp;N)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_OK</td><td>1041</td><td>{&amp;Tahoma9}&amp;OK</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_Retry</td><td>1041</td><td>{&amp;Tahoma9}再試行(&amp;R)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsErrorDlg_Yes</td><td>1041</td><td>{&amp;Tahoma9}はい(&amp;Y)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_Finish</td><td>1041</td><td>{&amp;Tahoma9}完了(&amp;F)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_InstallSuccess</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードは、[ProductName] を正常にインストールしました。「完了」をクリックして、ウィザードを終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_LaunchProgram</td><td>1041</td><td>{&amp;Tahoma9}プログラムの起動</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_ShowReadMe</td><td>1041</td><td>{&amp;Tahoma9}Readme ファイルを表示する</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_UninstallSuccess</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードは、[ProductName] を正常にアンインストールしました。「完了」をクリックして、ウィザードを終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_Update_InternetConnection</td><td>1041</td><td>ｲﾝﾀｰﾈｯﾄに接続すると、最新のｱｯﾌﾟﾃﾞｰﾄを確認できます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_Update_PossibleUpdates</td><td>1041</td><td>[ProductName] の購入以降にｱｯﾌﾟﾃﾞｰﾄされているﾌﾟﾛｸﾞﾗﾑ ﾌｧｲﾙがある可能性があります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_Update_SetupFinished</td><td>1041</td><td>[ProductName] のｲﾝｽﾄｰﾙを完了しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_Update_YesCheckForUpdates</td><td>1041</td><td>はい。ｾｯﾄｱｯﾌﾟの完了後、ﾌﾟﾛｸﾞﾗﾑのｱｯﾌﾟﾃﾞｰﾄを確認します (推奨)。(&amp;Y)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsExitDialog_WizardCompleted</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードを完了しました</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_ClickFinish</td><td>1041</td><td>{&amp;Tahoma9}「完了」をクリックして、ウィザードを終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_Finish</td><td>1041</td><td>{&amp;Tahoma9}完了(&amp;F)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_KeepOrRestore</td><td>1041</td><td>{&amp;Tahoma9}インストールしたコンポーネントをシステムに保存して後でこのインストールを続行するか、あるいはインストールを実行する前の状態に復元することができます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_NotModified</td><td>1041</td><td>{&amp;Tahoma9}システムの状態は変更されていません。改めてインストールする場合は、再度セットアップを実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_RestoreOrContinueLater</td><td>1041</td><td>{&amp;Tahoma9}「復元」、または「後で続行」をクリックしてウィザードを終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_WizardCompleted</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードを完了しました</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFatalError_WizardInterrupted</td><td>1041</td><td>{&amp;Tahoma9}ウィザードは、[ProductName] のインストールを完了する前に中断されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFeatureDetailsDlg_DiskSpaceRequirements</td><td>1041</td><td>{&amp;Tahoma9}必要なディスク空き領域</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFeatureDetailsDlg_Numbers</td><td>1041</td><td>{&amp;Tahoma9}{120}{70}{70}{70}{70}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFeatureDetailsDlg_OK</td><td>1041</td><td>{&amp;Tahoma9}OK</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFeatureDetailsDlg_SpaceRequired</td><td>1041</td><td>{&amp;Tahoma9}選択している機能をインストールするために必要なディスク空き領域です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFeatureDetailsDlg_VolumesTooSmall</td><td>1041</td><td>{&amp;Tahoma9}強調表示されているボリュームには、選択した機能をインストールするために十分なディスク空き領域がありません。ボリュームからファイルを削除してディスク領域を確保するか、ローカル ディスクにインストールする機能を削減してください。または、別のドライブを選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFilesInUse_ApplicationsUsingFiles</td><td>1041</td><td>{&amp;Tahoma9}次のアプリケーションが、このセットアップで更新する必要のあるファイルを使用しています。これらのアプリケーションを閉じてから、「再試行」をクリックして続行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFilesInUse_Exit</td><td>1041</td><td>{&amp;Tahoma9}終了(&amp;E)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFilesInUse_FilesInUse</td><td>1041</td><td>{&amp;Tahoma9}使用中のファイル</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFilesInUse_FilesInUseMessage</td><td>1041</td><td>{&amp;Tahoma9}更新する必要のあるファイルが使用中です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFilesInUse_Ignore</td><td>1041</td><td>{&amp;Tahoma9}無視(&amp;I)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsFilesInUse_Retry</td><td>1041</td><td>{&amp;Tahoma9}再試行(&amp;R)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsGroup</td><td>1041</td><td>ユーザーリスト グループ(&amp;G):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsGroupLabel</td><td>1041</td><td>ユーザーリスト グループ(&amp;O):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsInitDlg_1</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsInitDlg_2</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsInitDlg_PreparingWizard</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]セットアップは、プログラムセットアップの手順をご案内する InstallShield ウィザードを準備中です。しばらくお待ちください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsInitDlg_WelcomeWizard</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]用のInstallShield ウィザードへようこそ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsLicenseDlg_LicenseAgreement</td><td>1041</td><td>{&amp;Tahoma9}使用許諾契約</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsLicenseDlg_ReadLicenseAgreement</td><td>1041</td><td>{&amp;Tahoma9}次の使用許諾契約書を注意深くお読みください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsLogonInfoDescription</td><td>1041</td><td>このアプリケーションが利用するユーザーアカウントを指定します。 ユーザーアカウントは DOMAIN\Username の形式でなくてはなりません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsLogonInfoTitle</td><td>1041</td><td>{&amp;Tahoma9}ログイン情報</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsLogonInfoTitleDescription</td><td>1041</td><td>ユーザー名とパスワードを指定してください</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsLogonNewUserDescription</td><td>1041</td><td>下のボタンを選択して、インストール中に作成される新規ユーザーについての情報を指定します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_ChangeFeatures</td><td>1041</td><td>{&amp;Tahoma9}インストールするプログラム機能を変更します。このオプションでは、カスタム ダイアログを使ってインストールするプログラム機能を変更することができます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_MaitenanceOptions</td><td>1041</td><td>{&amp;Tahoma9}プログラムを変更、修復、および削除します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_Modify</td><td>1041</td><td>{&amp;Tahoma9}変更(&amp;M)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_ProgramMaintenance</td><td>1041</td><td>{&amp;Tahoma9}プログラムの保守</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_Remove</td><td>1041</td><td>{&amp;Tahoma9}削除(&amp;R)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_RemoveProductName</td><td>1041</td><td>{&amp;Tahoma9}コンピュータから [ProductName] を削除します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_Repair</td><td>1041</td><td>{&amp;Tahoma9}修復(&amp;P)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceDlg_RepairMessage</td><td>1041</td><td>{&amp;Tahoma9}プログラム中のエラーを修復します。このオプションでは、失われたり壊れたりしたファイル、ショートカット、およびレジストリ エントリを修正することができます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceWelcome_MaintenanceOptionsDescription</td><td>1041</td><td>{&amp;Tahoma9}InstallShield(R) ウィザードを使うと、[ProductName] を変更、修正、および削除することができます。 続行するには、「次へ」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMaintenanceWelcome_WizardWelcome</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]用のInstallShield ウィザードへようこそ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMsiRMFilesInUse_ApplicationsUsingFiles</td><td>1041</td><td>セットアップで更新する必要のあるファイルが、以下のアプリケーションで使用中です。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMsiRMFilesInUse_CloseRestart</td><td>1041</td><td>自動的に閉じて、アプリケーションを再起動する。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsMsiRMFilesInUse_RebootAfter</td><td>1041</td><td>アプリケーションを閉じない。 (再起動が必要です。)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsPatchDlg_PatchClickUpdate</td><td>1041</td><td>InstallShield(R) Wizard は、ご利用のコンピュータに [ProductName]用のパッチをインストールします。作業を続行するには、「更新」をクリックしてください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsPatchDlg_PatchWizard</td><td>1041</td><td>[ProductName] パッチ - InstallShield Wizard</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsPatchDlg_Update</td><td>1041</td><td>更新(&amp;U) &gt;</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsPatchDlg_WelcomePatchWizard</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]のパッチへようこそ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_2</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_Hidden</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_HiddenTimeRemaining</td><td>1041</td><td>{&amp;Tahoma9}推測の残り時間:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_InstallingProductName</td><td>1041</td><td>{&amp;Tahoma9}[ProductName] をインストールしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_ProgressDone</td><td>1041</td><td>{&amp;Tahoma9}プログレスの完了</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_SecHidden</td><td>1041</td><td>{&amp;Tahoma9}秒</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_Status</td><td>1041</td><td>{&amp;Tahoma9}ステータス:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_Uninstalling</td><td>1041</td><td>{&amp;Tahoma9}[ProductName] をアンインストールしています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_UninstallingFeatures</td><td>1041</td><td>{&amp;Tahoma9}選択したプログラム機能をアンインストールしています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_UninstallingFeatures2</td><td>1041</td><td>{&amp;Tahoma9}選択したプログラム機能をインストールしています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_WaitUninstall</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードは、[ProductName] をアンインストールしています。しばらくお待ちください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsProgressDlg_WaitUninstall2</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードは、[ProductName] をインストールしています。しばらくお待ちください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsReadmeDlg_Cancel</td><td>1041</td><td>キャンセル(&amp;C)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsReadmeDlg_PleaseReadInfo</td><td>1041</td><td>次の Readme 情報を注意深くお読みください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsReadmeDlg_ReadMeInfo</td><td>1041</td><td>{&amp;Tahoma9}Readme 情報</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_16</td><td>1041</td><td>{&amp;Tahoma9}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_Anyone</td><td>1041</td><td>{&amp;Tahoma9}このコンピュータを使用するすべてのユーザー(&amp;A)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_CustomerInformation</td><td>1041</td><td>{&amp;Tahoma9}ユーザー情報</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_InstallFor</td><td>1041</td><td>{&amp;Tahoma9}このアプリケーションを次のユーザーに対してインストールします:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_OnlyMe</td><td>1041</td><td>{&amp;Tahoma9}[USERNAME] のみ(&amp;M)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_Organization</td><td>1041</td><td>{&amp;Tahoma9}所属(&amp;O):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_PleaseEnterInfo</td><td>1041</td><td>{&amp;Tahoma9}情報を入力してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_SerialNumber</td><td>1041</td><td>シリアル番号(&amp;S):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_Tahoma50</td><td>1041</td><td>{&amp;Tahoma9}{50}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_Tahoma80</td><td>1041</td><td>{&amp;Tahoma9}{80}</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsRegisterUserDlg_UserName</td><td>1041</td><td>{&amp;Tahoma9}ユーザー名(&amp;U):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsResumeDlg_ResumeSuspended</td><td>1041</td><td>{&amp;Tahoma9}InstallShield(R) ウィザードは、中断していた [ProductName] のインストールを完了しました。「次へ」をクリックして、続行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsResumeDlg_Resuming</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]用のInstallShield ウィザードを続行しています</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsResumeDlg_WizardResume</td><td>1041</td><td>{&amp;Tahoma9}InstallShield(R) ウィザードは、ご使用のコンピュータへの [ProductName] のインストールを完了します。「次へ」をクリックして、続行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSelectDomainOrServer</td><td>1041</td><td>ドメインまたはサーバーの選択</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSelectDomainUserInstructions</td><td>1041</td><td>参照ボタンを利用して domain\server  およびユーザー名を選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupComplete_ShowMsiLog</td><td>1041</td><td>Windows Installer のログを表示</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_13</td><td>1041</td><td/><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_AllFeatures</td><td>1041</td><td>{&amp;Tahoma9}すべてのプログラム機能をインストールします。 (最大のディスク容量を必要とします)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_ChooseFeatures</td><td>1041</td><td>{&amp;Tahoma9}インストールするプログラム機能、およびインストール先を選択することができます。製品をよくご存知のユーザーにお勧めします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_ChooseSetupType</td><td>1041</td><td>{&amp;Tahoma9}ご利用方法に合わせて最適なセットアップ タイプを選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_Complete</td><td>1041</td><td>{&amp;Tahoma9}完全(&amp;C)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_Custom</td><td>1041</td><td>{&amp;Tahoma9}カスタム(&amp;S)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_Minimal</td><td>1041</td><td>{&amp;Tahoma9}最小(&amp;M)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_MinimumFeatures</td><td>1041</td><td>{&amp;Tahoma9}最小限必要な機能をインストールします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_SelectSetupType</td><td>1041</td><td>{&amp;Tahoma9}セットアップ タイプを選択してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_SetupType</td><td>1041</td><td>{&amp;Tahoma9}セットアップ タイプ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsSetupTypeMinDlg_Typical</td><td>1041</td><td>{&amp;Tahoma9}標準(&amp;T)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_ClickFinish</td><td>1041</td><td>{&amp;Tahoma9}「完了」をクリックして、ウィザードを終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_Finish</td><td>1041</td><td>{&amp;Tahoma9}完了(&amp;F)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_KeepOrRestore</td><td>1041</td><td>{&amp;Tahoma9}インストールしたコンポーネントをシステムに保存して後でこのインストールを続行するか、あるいはインストールを実行する前の状態に復元することができます。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_NotModified</td><td>1041</td><td>{&amp;Tahoma9}システムの状態は変更されていません。改めてインストールする場合は、再度セットアップを実行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_RestoreOrContinue</td><td>1041</td><td>{&amp;Tahoma9}「復元」、または「後で継続」をクリックして、インストールを終了してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_WizardCompleted</td><td>1041</td><td>{&amp;Tahoma9}InstallShield ウィザードを完了しました</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserExit_WizardInterrupted</td><td>1041</td><td>{&amp;Tahoma9}ウィザードは、[ProductName] のインストールを完了する前に中断されました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsUserNameLabel</td><td>1041</td><td>ユーザー名(&amp;U):</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_BackOrCancel</td><td>1041</td><td>{&amp;Tahoma9}インストールの設定を参照したり変更する場合は、「戻る」をクリックしてください。「キャンセル」をクリックすると、ウィザードを終了します。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_ClickInstall</td><td>1041</td><td>{&amp;Tahoma9}「インストール」をクリックして、インストールを開始してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_Company</td><td>1041</td><td>会社: [COMPANYNAME]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_CurrentSettings</td><td>1041</td><td>現在の設定: </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_DestFolder</td><td>1041</td><td>インストール先フｫルダ: </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_Install</td><td>1041</td><td>{&amp;Tahoma9}インストール(&amp;I)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_Installdir</td><td>1041</td><td>[INSTALLDIR]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_ModifyReady</td><td>1041</td><td>{&amp;Tahoma9}プログラムを変更する準備ができました</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_ReadyInstall</td><td>1041</td><td>{&amp;Tahoma9}プログラムをインストールする準備ができました</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_ReadyRepair</td><td>1041</td><td>{&amp;Tahoma9}プログラムを修復する準備ができました</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_SelectedSetupType</td><td>1041</td><td>[SelectedSetupType]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_Serial</td><td>1041</td><td>シリアル: [ISX_SERIALNUM]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_SetupType</td><td>1041</td><td>セットアップ タイプ:</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_UserInfo</td><td>1041</td><td>ユーザー情報: </td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_UserName</td><td>1041</td><td>名前:  [USERNAME]</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyReadyDlg_WizardReady</td><td>1041</td><td>{&amp;Tahoma9}ウィザードは、インストールを開始する準備ができました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyRemoveAllDlg_ChoseRemoveProgram</td><td>1041</td><td>{&amp;Tahoma9}ご使用のシステムからプログラムを削除するオプションを選択しました。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyRemoveAllDlg_ClickBack</td><td>1041</td><td>{&amp;Tahoma9}設定を参照したり変更する場合は、「戻る」をクリックします。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyRemoveAllDlg_ClickRemove</td><td>1041</td><td>{&amp;Tahoma9}「削除」をクリックして、コンピュータから [ProductName] を削除してください。削除を実行すると、このプログラムは、使用できなくなります。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyRemoveAllDlg_Remove</td><td>1041</td><td>{&amp;Tahoma9}削除(&amp;R)</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsVerifyRemoveAllDlg_RemoveProgram</td><td>1041</td><td>{&amp;Tahoma9}プログラムの削除</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsWelcomeDlg_InstallProductName</td><td>1041</td><td>{&amp;Tahoma9}InstallShield(R) ウィザードは、ご使用のコンピュータへ [ProductName] をインストールします。「次へ」をクリックして、続行してください。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsWelcomeDlg_WarningCopyright</td><td>1041</td><td>警告:  このプログラムは、著作権法および国際協定によって保護されています。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__IsWelcomeDlg_WelcomeProductName</td><td>1041</td><td>{&amp;Tahoma9}[ProductName]用のInstallShield ウィザードへようこそ</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__TargetReq_DESC_COLOR</td><td>1041</td><td>システムの色設定は、[ProductName] の実行に適切ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__TargetReq_DESC_OS</td><td>1041</td><td>このオペレーティングシステムは、[ProductName] の実行に適切ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__TargetReq_DESC_PROCESSOR</td><td>1041</td><td>このプロセッサは、[ProductName] の実行に適切ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__TargetReq_DESC_RAM</td><td>1041</td><td>RAM 容量が、[ProductName] の実行に適切ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>IDS__TargetReq_DESC_RESOLUTION</td><td>1041</td><td>画面の解像度が、[ProductName] の実行に適切ではありません。</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>ID_STRING1</td><td>1041</td><td/><td>0</td><td/><td>-199294675</td></row>
		<row><td>ID_STRING3</td><td>1041</td><td>Tween</td><td>0</td><td/><td>-199245523</td></row>
		<row><td>ID_STRING4</td><td>1041</td><td>Tween</td><td>0</td><td/><td>-199294675</td></row>
		<row><td>ID_STRING5</td><td>1041</td><td/><td>0</td><td/><td>-199294675</td></row>
		<row><td>ID_STRING9</td><td>1041</td><td>http://www.tweenapp.org</td><td>0</td><td/><td>-199286387</td></row>
		<row><td>IIDS_UITEXT_FeatureUninstalled</td><td>1041</td><td>{&amp;Tahoma9}この機能をアンインストールしたままにします。</td><td>0</td><td/><td>-199294675</td></row>
	</table>

	<table name="ISTargetImage">
		<col key="yes" def="s13">UpgradedImage_</col>
		<col key="yes" def="s13">Name</col>
		<col def="s0">MsiPath</col>
		<col def="i2">Order</col>
		<col def="I4">Flags</col>
		<col def="i2">IgnoreMissingFiles</col>
	</table>

	<table name="ISUpgradeMsiItem">
		<col key="yes" def="s72">UpgradeItem</col>
		<col def="s0">ObjectSetupPath</col>
		<col def="S255">ISReleaseFlags</col>
		<col def="i2">ISAttributes</col>
	</table>

	<table name="ISUpgradedImage">
		<col key="yes" def="s13">Name</col>
		<col def="s0">MsiPath</col>
		<col def="s8">Family</col>
	</table>

	<table name="ISVirtualDirectory">
		<col key="yes" def="s72">Directory_</col>
		<col key="yes" def="s72">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISVirtualFile">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="s72">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISVirtualPackage">
		<col key="yes" def="s72">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISVirtualRegistry">
		<col key="yes" def="s72">Registry_</col>
		<col key="yes" def="s72">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISVirtualRelease">
		<col key="yes" def="s72">ISRelease_</col>
		<col key="yes" def="s72">ISProductConfiguration_</col>
		<col key="yes" def="s255">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISVirtualShortcut">
		<col key="yes" def="s72">Shortcut_</col>
		<col key="yes" def="s72">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="ISXmlElement">
		<col key="yes" def="s72">ISXmlElement</col>
		<col def="s72">ISXmlFile_</col>
		<col def="S72">ISXmlElement_Parent</col>
		<col def="L0">XPath</col>
		<col def="L0">Content</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISXmlElementAttrib">
		<col key="yes" def="s72">ISXmlElementAttrib</col>
		<col key="yes" def="s72">ISXmlElement_</col>
		<col def="L255">Name</col>
		<col def="L0">Value</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="ISXmlFile">
		<col key="yes" def="s72">ISXmlFile</col>
		<col def="l255">FileName</col>
		<col def="s72">Component_</col>
		<col def="s72">Directory</col>
		<col def="I4">ISAttributes</col>
		<col def="S0">SelectionNamespaces</col>
		<col def="S255">Encoding</col>
	</table>

	<table name="ISXmlLocator">
		<col key="yes" def="s72">Signature_</col>
		<col key="yes" def="S72">Parent</col>
		<col def="S255">Element</col>
		<col def="S255">Attribute</col>
		<col def="I2">ISAttributes</col>
	</table>

	<table name="Icon">
		<col key="yes" def="s72">Name</col>
		<col def="V0">Data</col>
		<col def="S255">ISBuildSourcePath</col>
		<col def="I2">ISIconIndex</col>
		<row><td>ARPPRODUCTICON.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\Resources\4b.ico</td><td>0</td></row>
		<row><td>UNINST_Uninstall_T_0C1103B7009948ECBE2C67FD0B6574D4.exe</td><td/><td>C:\Program Files\InstallShield\2011LE\Redist\Language Independent\OS Independent\uninstall.ico</td><td>0</td></row>
		<row><td>_02B532A5B9B1499D9B47EB5E9242A39D.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_03061D35A7DC4CC7B023BBD5413DCFAD.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_042568D5AB4B488880C02D2E57C575DA.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_0502B32032CC4C86AF3931F689A1F2F8.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_073A1BD032C2416E95F0BD543E02CC01.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_08E7F3B3C55F4254B26D841D96BAE564.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_0C9608E667B04D43B207AAF5898C82BB.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_0CCC06537CAC41BDA2282BEBB6C44538.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_0F1F51E5E60E4152B2B1534F2A778F72.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_1038A2ED6C9349E3873E3399BC71EB0E.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_18FA61A72C474FF1BDFB48645C75A06D.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_1BF4E49C9CBB44B0908EC64DC6AF62C1.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_1FA073CC0B2F4D25A54864ECCB2F403D.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_220155E194A64E2CBF108F8AA710E57B.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_24F12721E050478183B8001E0FD8D488.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_2B93170B8EEF44DDA3027D3261436AEE.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_2CAD664A543C4D7F9C68B45B91BDE758.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_2CC284D1BB7E40E1A6613AC0E214B0A2.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_32F0E842174B42F7924DFEE93970F40A.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_3922578F95764674BAFBB6402078D3A6.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_3A364AB660204E5B90B1866271C15075.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_3F46E720BE8442678A0D6F022BABCA47.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_4010276F55544FC3BCB40674FD6AD9E0.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_40D8B830E9FA405E9E040261B450072B.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_46439A088C374F659E884E03FF5E161A.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_465A97AC8FDA442E8352BCD8EBBD6CA3.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_46C5EAB225E84B1CBEDEAFDD29F3BA62.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_4B1A412CC1C64B77B09C8C364B071377.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_5080E5C2BF0F492C85124CF305D1E068.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_56F98120DA0E49D4B52A4DE448FEFB9F.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_58703853DE3F47F1B7A39EC18343849A.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_592DA1E71E6B4995A4852FB8E3E6A014.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_6088914935E643C68AA4CED7BAC7C8B6.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_668B80B8F12F4172901C8F386EE02062.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_6AA82E2F8ECC4D5CA68333D96EB4C0E6.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_6D1C34B1A5AA402490E4E50B18D50D8D.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_72F27B46E0F74A459F4445F18B8EBE71.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_73856F29D092460CBEEE250B8AD8B4FF.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_82D5ECDA9B5D4997BD4A8E8C699928F1.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_8472FF4C8E484B0B8AB48445F1DAA71C.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_8675346147AF4EB6943C7238C4643F03.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_8C98F833E312426CB07FF4318A21CDF5.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_8CA8FE476BC04D79B7A31DDB45FF1270.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_8DD6ACEC5BFD411BBB5A1652DB4A778B.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_9212EE6233C24E64837FF5631FE7D71B.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_A1880271ACD04D898A81278797FBDE3D.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_A8281ACD1FA1458F9CE47D0A4A11CD8A.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_AB9B11DF43434F30AAF88B7D6B46AE07.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_ABB0E5BE342545959D20B5259B0B3519.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_B011B8C47A974259B10F25EEBF11A1C2.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_B392347709BE4F3BA39675C489A1BE3D.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_B7CCA9629FE24B1A8A5B9DE4C964479E.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_BA47EADD7A04463A85E77A21B7CD3991.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_BA6344A0C8214094A7EAE10118E896CC.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_C08550F51F92443AB7BE7277E42FE483.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_C61C473288944293A3EF11F3C1D1E656.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_C8BBBAD6B2A142D8B8DCC021240E3BF0.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_CD9E17CE25744E56943EB13B2FD97E71.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_D3D0997CC6964800A144399BC78C2713.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_D72486F376D64333ADC86F383FC55D05.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_D8A1A4598CE24F2ABFC8765C88B3FF0C.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_D8F500641817439991F2460D688E7611.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_DA16AD05C010476B8A549B67BD10A73B.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_DAAC02B659D54FADB5AF8D6E35E6BDE5.exe</td><td/><td>C:\Users\DA15USER\Documents\開発\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_E0438E2618234DC99D1A01968DDFA693.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Debug\Tween.exe</td><td>0</td></row>
		<row><td>_E95D963736D74AD2A92B39A6B55CE901.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_EFB15DFDA94A4237B2613FE25633D6A7.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_F24F7732591041CD879AF72BF63A2135.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_F2745887356944BD9877FB07CE74F497.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_F2E29CF8444947428B77CE6CC85FBF23.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_F4468E7CCA4F45DBA64B8D736E8D57E8.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
		<row><td>_FA2C71D6EADD4C4183EA8072ACD61E91.exe</td><td/><td>C:\Users\DA15USER\Documents\tween\trunk\Tween\obj\x86\Release\Tween.exe</td><td>0</td></row>
	</table>

	<table name="IniFile">
		<col key="yes" def="s72">IniFile</col>
		<col def="l255">FileName</col>
		<col def="S72">DirProperty</col>
		<col def="l255">Section</col>
		<col def="l128">Key</col>
		<col def="s255">Value</col>
		<col def="i2">Action</col>
		<col def="s72">Component_</col>
	</table>

	<table name="IniLocator">
		<col key="yes" def="s72">Signature_</col>
		<col def="s255">FileName</col>
		<col def="s96">Section</col>
		<col def="s128">Key</col>
		<col def="I2">Field</col>
		<col def="I2">Type</col>
	</table>

	<table name="InstallExecuteSequence">
		<col key="yes" def="s72">Action</col>
		<col def="S255">Condition</col>
		<col def="I2">Sequence</col>
		<col def="S255">ISComments</col>
		<col def="I4">ISAttributes</col>
		<row><td>AllocateRegistrySpace</td><td>NOT Installed</td><td>1550</td><td>AllocateRegistrySpace</td><td/></row>
		<row><td>AppSearch</td><td/><td>400</td><td>AppSearch</td><td/></row>
		<row><td>BindImage</td><td/><td>4300</td><td>BindImage</td><td/></row>
		<row><td>CCPSearch</td><td>CCP_TEST</td><td>500</td><td>CCPSearch</td><td/></row>
		<row><td>CostFinalize</td><td/><td>1000</td><td>CostFinalize</td><td/></row>
		<row><td>CostInitialize</td><td/><td>800</td><td>CostInitialize</td><td/></row>
		<row><td>CreateFolders</td><td/><td>3700</td><td>CreateFolders</td><td/></row>
		<row><td>CreateShortcuts</td><td/><td>4500</td><td>CreateShortcuts</td><td/></row>
		<row><td>DeleteServices</td><td>VersionNT</td><td>2000</td><td>DeleteServices</td><td/></row>
		<row><td>DuplicateFiles</td><td/><td>4210</td><td>DuplicateFiles</td><td/></row>
		<row><td>FileCost</td><td/><td>900</td><td>FileCost</td><td/></row>
		<row><td>FindRelatedProducts</td><td>NOT ISSETUPDRIVEN</td><td>420</td><td>FindRelatedProducts</td><td/></row>
		<row><td>ISPreventDowngrade</td><td>ISFOUNDNEWERPRODUCTVERSION</td><td>450</td><td>ISPreventDowngrade</td><td/></row>
		<row><td>ISRunSetupTypeAddLocalEvent</td><td>Not Installed And Not ISRUNSETUPTYPEADDLOCALEVENT</td><td>1050</td><td>ISRunSetupTypeAddLocalEvent</td><td/></row>
		<row><td>ISSelfRegisterCosting</td><td/><td>2201</td><td/><td/></row>
		<row><td>ISSelfRegisterFiles</td><td/><td>5601</td><td/><td/></row>
		<row><td>ISSelfRegisterFinalize</td><td/><td>6601</td><td/><td/></row>
		<row><td>ISUnSelfRegisterFiles</td><td/><td>2202</td><td/><td/></row>
		<row><td>InstallFiles</td><td/><td>4000</td><td>InstallFiles</td><td/></row>
		<row><td>InstallFinalize</td><td/><td>6600</td><td>InstallFinalize</td><td/></row>
		<row><td>InstallInitialize</td><td/><td>1501</td><td>InstallInitialize</td><td/></row>
		<row><td>InstallODBC</td><td/><td>5400</td><td>InstallODBC</td><td/></row>
		<row><td>InstallServices</td><td>VersionNT</td><td>5800</td><td>InstallServices</td><td/></row>
		<row><td>InstallValidate</td><td/><td>1400</td><td>InstallValidate</td><td/></row>
		<row><td>IsolateComponents</td><td/><td>950</td><td>IsolateComponents</td><td/></row>
		<row><td>LaunchConditions</td><td>Not Installed</td><td>410</td><td>LaunchConditions</td><td/></row>
		<row><td>MigrateFeatureStates</td><td/><td>1010</td><td>MigrateFeatureStates</td><td/></row>
		<row><td>MoveFiles</td><td/><td>3800</td><td>MoveFiles</td><td/></row>
		<row><td>MsiConfigureServices</td><td>VersionMsi &gt;= "5.00"</td><td>5850</td><td>MSI5 MsiConfigureServices</td><td/></row>
		<row><td>MsiPublishAssemblies</td><td/><td>6250</td><td>MsiPublishAssemblies</td><td/></row>
		<row><td>MsiUnpublishAssemblies</td><td/><td>1750</td><td>MsiUnpublishAssemblies</td><td/></row>
		<row><td>PatchFiles</td><td/><td>4090</td><td>PatchFiles</td><td/></row>
		<row><td>ProcessComponents</td><td/><td>1600</td><td>ProcessComponents</td><td/></row>
		<row><td>PublishComponents</td><td/><td>6200</td><td>PublishComponents</td><td/></row>
		<row><td>PublishFeatures</td><td/><td>6300</td><td>PublishFeatures</td><td/></row>
		<row><td>PublishProduct</td><td/><td>6400</td><td>PublishProduct</td><td/></row>
		<row><td>RMCCPSearch</td><td>Not CCP_SUCCESS And CCP_TEST</td><td>600</td><td>RMCCPSearch</td><td/></row>
		<row><td>RegisterClassInfo</td><td/><td>4600</td><td>RegisterClassInfo</td><td/></row>
		<row><td>RegisterComPlus</td><td/><td>5700</td><td>RegisterComPlus</td><td/></row>
		<row><td>RegisterExtensionInfo</td><td/><td>4700</td><td>RegisterExtensionInfo</td><td/></row>
		<row><td>RegisterFonts</td><td/><td>5300</td><td>RegisterFonts</td><td/></row>
		<row><td>RegisterMIMEInfo</td><td/><td>4900</td><td>RegisterMIMEInfo</td><td/></row>
		<row><td>RegisterProduct</td><td/><td>6100</td><td>RegisterProduct</td><td/></row>
		<row><td>RegisterProgIdInfo</td><td/><td>4800</td><td>RegisterProgIdInfo</td><td/></row>
		<row><td>RegisterTypeLibraries</td><td/><td>5500</td><td>RegisterTypeLibraries</td><td/></row>
		<row><td>RegisterUser</td><td/><td>6000</td><td>RegisterUser</td><td/></row>
		<row><td>RemoveDuplicateFiles</td><td/><td>3400</td><td>RemoveDuplicateFiles</td><td/></row>
		<row><td>RemoveEnvironmentStrings</td><td/><td>3300</td><td>RemoveEnvironmentStrings</td><td/></row>
		<row><td>RemoveExistingProducts</td><td/><td>1410</td><td>RemoveExistingProducts</td><td/></row>
		<row><td>RemoveFiles</td><td/><td>3500</td><td>RemoveFiles</td><td/></row>
		<row><td>RemoveFolders</td><td/><td>3600</td><td>RemoveFolders</td><td/></row>
		<row><td>RemoveIniValues</td><td/><td>3100</td><td>RemoveIniValues</td><td/></row>
		<row><td>RemoveODBC</td><td/><td>2400</td><td>RemoveODBC</td><td/></row>
		<row><td>RemoveRegistryValues</td><td/><td>2600</td><td>RemoveRegistryValues</td><td/></row>
		<row><td>RemoveShortcuts</td><td/><td>3200</td><td>RemoveShortcuts</td><td/></row>
		<row><td>ResolveSource</td><td>Not Installed</td><td>850</td><td>ResolveSource</td><td/></row>
		<row><td>ScheduleReboot</td><td>ISSCHEDULEREBOOT</td><td>6410</td><td>ScheduleReboot</td><td/></row>
		<row><td>SelfRegModules</td><td/><td>5600</td><td>SelfRegModules</td><td/></row>
		<row><td>SelfUnregModules</td><td/><td>2200</td><td>SelfUnregModules</td><td/></row>
		<row><td>SetARPINSTALLLOCATION</td><td/><td>1100</td><td>SetARPINSTALLLOCATION</td><td/></row>
		<row><td>SetAllUsersProfileNT</td><td>VersionNT = 400</td><td>970</td><td/><td/></row>
		<row><td>SetODBCFolders</td><td/><td>1200</td><td>SetODBCFolders</td><td/></row>
		<row><td>StartServices</td><td>VersionNT</td><td>5900</td><td>StartServices</td><td/></row>
		<row><td>StopServices</td><td>VersionNT</td><td>1900</td><td>StopServices</td><td/></row>
		<row><td>UnpublishComponents</td><td/><td>1700</td><td>UnpublishComponents</td><td/></row>
		<row><td>UnpublishFeatures</td><td/><td>1800</td><td>UnpublishFeatures</td><td/></row>
		<row><td>UnregisterClassInfo</td><td/><td>2700</td><td>UnregisterClassInfo</td><td/></row>
		<row><td>UnregisterComPlus</td><td/><td>2100</td><td>UnregisterComPlus</td><td/></row>
		<row><td>UnregisterExtensionInfo</td><td/><td>2800</td><td>UnregisterExtensionInfo</td><td/></row>
		<row><td>UnregisterFonts</td><td/><td>2500</td><td>UnregisterFonts</td><td/></row>
		<row><td>UnregisterMIMEInfo</td><td/><td>3000</td><td>UnregisterMIMEInfo</td><td/></row>
		<row><td>UnregisterProgIdInfo</td><td/><td>2900</td><td>UnregisterProgIdInfo</td><td/></row>
		<row><td>UnregisterTypeLibraries</td><td/><td>2300</td><td>UnregisterTypeLibraries</td><td/></row>
		<row><td>ValidateProductID</td><td/><td>700</td><td>ValidateProductID</td><td/></row>
		<row><td>WriteEnvironmentStrings</td><td/><td>5200</td><td>WriteEnvironmentStrings</td><td/></row>
		<row><td>WriteIniValues</td><td/><td>5100</td><td>WriteIniValues</td><td/></row>
		<row><td>WriteRegistryValues</td><td/><td>5000</td><td>WriteRegistryValues</td><td/></row>
		<row><td>setAllUsersProfile2K</td><td>VersionNT &gt;= 500</td><td>980</td><td/><td/></row>
		<row><td>setUserProfileNT</td><td>VersionNT</td><td>960</td><td/><td/></row>
	</table>

	<table name="InstallShield">
		<col key="yes" def="s72">Property</col>
		<col def="S0">Value</col>
		<row><td>ActiveLanguage</td><td>1041</td></row>
		<row><td>Comments</td><td/></row>
		<row><td>CurrentMedia</td><td dt:dt="bin.base64" md5="de9f554a3bc05c12be9c31b998217995">
UwBpAG4AZwBsAGUASQBtAGEAZwBlAAEARQB4AHAAcgBlAHMAcwA=
			</td></row>
		<row><td>DefaultProductConfiguration</td><td>Express</td></row>
		<row><td>ISCompilerOption_CompileBeforeBuild</td><td>1</td></row>
		<row><td>ISCompilerOption_Debug</td><td>0</td></row>
		<row><td>ISCompilerOption_IncludePath</td><td/></row>
		<row><td>ISCompilerOption_LibraryPath</td><td/></row>
		<row><td>ISCompilerOption_MaxErrors</td><td>50</td></row>
		<row><td>ISCompilerOption_MaxWarnings</td><td>50</td></row>
		<row><td>ISCompilerOption_OutputPath</td><td>&lt;ISProjectDataFolder&gt;\Script Files</td></row>
		<row><td>ISCompilerOption_PreProcessor</td><td>_ISSCRIPT_NEW_STYLE_DLG_DEFS</td></row>
		<row><td>ISCompilerOption_WarningLevel</td><td>3</td></row>
		<row><td>ISCompilerOption_WarningsAsErrors</td><td>1</td></row>
		<row><td>ISTheme</td><td>InstallShield Blue.theme</td></row>
		<row><td>ISUSLock</td><td>{FD6AEC95-2396-40CD-AE6D-995887682BAD}</td></row>
		<row><td>ISUSSignature</td><td>{B0F52E75-0910-4FEF-8ACD-D686D92D10D6}</td></row>
		<row><td>ISVisitedViews</td><td>viewAssistant,viewProject,viewUpdateService,viewSystemSearch,viewSetupDesign,viewShortcuts,viewFileExtensions,viewUI,viewFeatureFiles,viewAppFiles,viewRelease,viewDesignPatches,viewSetupTypes,viewUpgradePaths,viewISToday,viewEnvironmentVariables,viewTextMessages,viewCustomActions,viewSupportFiles,viewObjects,viewIniFiles,viewRegistry,viewVRoots</td></row>
		<row><td>Limited</td><td>1</td></row>
		<row><td>LockPermissionMode</td><td>1</td></row>
		<row><td>MsiExecCmdLineOptions</td><td/></row>
		<row><td>MsiLogFile</td><td/></row>
		<row><td>OnUpgrade</td><td>0</td></row>
		<row><td>Owner</td><td/></row>
		<row><td>PatchFamily</td><td>MyPatchFamily1</td></row>
		<row><td>PatchSequence</td><td>1.0.0</td></row>
		<row><td>SaveAsSchema</td><td/></row>
		<row><td>SccEnabled</td><td>0</td></row>
		<row><td>SccPath</td><td/></row>
		<row><td>SchemaVersion</td><td>771</td></row>
		<row><td>Type</td><td>MSIE</td></row>
	</table>

	<table name="InstallUISequence">
		<col key="yes" def="s72">Action</col>
		<col def="S255">Condition</col>
		<col def="I2">Sequence</col>
		<col def="S255">ISComments</col>
		<col def="I4">ISAttributes</col>
		<row><td>AppSearch</td><td/><td>400</td><td>AppSearch</td><td/></row>
		<row><td>CCPSearch</td><td>CCP_TEST</td><td>500</td><td>CCPSearch</td><td/></row>
		<row><td>CostFinalize</td><td/><td>1000</td><td>CostFinalize</td><td/></row>
		<row><td>CostInitialize</td><td/><td>800</td><td>CostInitialize</td><td/></row>
		<row><td>ExecuteAction</td><td/><td>1300</td><td>ExecuteAction</td><td/></row>
		<row><td>FileCost</td><td/><td>900</td><td>FileCost</td><td/></row>
		<row><td>FindRelatedProducts</td><td/><td>430</td><td>FindRelatedProducts</td><td/></row>
		<row><td>ISPreventDowngrade</td><td>ISFOUNDNEWERPRODUCTVERSION</td><td>450</td><td>ISPreventDowngrade</td><td/></row>
		<row><td>InstallWelcome</td><td>Not Installed</td><td>1210</td><td>InstallWelcome</td><td/></row>
		<row><td>IsolateComponents</td><td/><td>950</td><td>IsolateComponents</td><td/></row>
		<row><td>LaunchConditions</td><td>Not Installed</td><td>410</td><td>LaunchConditions</td><td/></row>
		<row><td>MaintenanceWelcome</td><td>Installed And Not RESUME And Not Preselected And Not PATCH</td><td>1230</td><td>MaintenanceWelcome</td><td/></row>
		<row><td>MigrateFeatureStates</td><td/><td>1200</td><td>MigrateFeatureStates</td><td/></row>
		<row><td>PatchWelcome</td><td>Installed And PATCH And Not IS_MAJOR_UPGRADE</td><td>1205</td><td>Patch Panel</td><td/></row>
		<row><td>RMCCPSearch</td><td>Not CCP_SUCCESS And CCP_TEST</td><td>600</td><td>RMCCPSearch</td><td/></row>
		<row><td>ResolveSource</td><td>Not Installed</td><td>990</td><td>ResolveSource</td><td/></row>
		<row><td>SetAllUsersProfileNT</td><td>VersionNT = 400</td><td>970</td><td/><td/></row>
		<row><td>SetupCompleteError</td><td/><td>-3</td><td>SetupCompleteError</td><td/></row>
		<row><td>SetupCompleteSuccess</td><td/><td>-1</td><td>SetupCompleteSuccess</td><td/></row>
		<row><td>SetupInitialization</td><td/><td>420</td><td>SetupInitialization</td><td/></row>
		<row><td>SetupInterrupted</td><td/><td>-2</td><td>SetupInterrupted</td><td/></row>
		<row><td>SetupProgress</td><td/><td>1240</td><td>SetupProgress</td><td/></row>
		<row><td>SetupResume</td><td>Installed And (RESUME Or Preselected) And Not PATCH</td><td>1220</td><td>SetupResume</td><td/></row>
		<row><td>ValidateProductID</td><td/><td>700</td><td>ValidateProductID</td><td/></row>
		<row><td>setAllUsersProfile2K</td><td>VersionNT &gt;= 500</td><td>980</td><td/><td/></row>
		<row><td>setUserProfileNT</td><td>VersionNT</td><td>960</td><td/><td/></row>
	</table>

	<table name="IsolatedComponent">
		<col key="yes" def="s72">Component_Shared</col>
		<col key="yes" def="s72">Component_Application</col>
	</table>

	<table name="LaunchCondition">
		<col key="yes" def="s255">Condition</col>
		<col def="l255">Description</col>
		<row><td>(Not Version9X) And (Not VersionNT=400) And (Not VersionNT=500)</td><td>##IDPROP_EXPRESS_LAUNCH_CONDITION_OS##</td></row>
		<row><td>DOTNETVERSION40FULL&gt;="#1"</td><td>##IDPROP_EXPRESS_LAUNCH_CONDITION_DOTNETVERSION40FULL##</td></row>
	</table>

	<table name="ListBox">
		<col key="yes" def="s72">Property</col>
		<col key="yes" def="i2">Order</col>
		<col def="s64">Value</col>
		<col def="L64">Text</col>
	</table>

	<table name="ListView">
		<col key="yes" def="s72">Property</col>
		<col key="yes" def="i2">Order</col>
		<col def="s64">Value</col>
		<col def="L64">Text</col>
		<col def="S72">Binary_</col>
	</table>

	<table name="LockPermissions">
		<col key="yes" def="s72">LockObject</col>
		<col key="yes" def="s32">Table</col>
		<col key="yes" def="S255">Domain</col>
		<col key="yes" def="s255">User</col>
		<col def="I4">Permission</col>
	</table>

	<table name="MIME">
		<col key="yes" def="s64">ContentType</col>
		<col def="s255">Extension_</col>
		<col def="S38">CLSID</col>
	</table>

	<table name="Media">
		<col key="yes" def="i2">DiskId</col>
		<col def="i2">LastSequence</col>
		<col def="L64">DiskPrompt</col>
		<col def="S255">Cabinet</col>
		<col def="S32">VolumeLabel</col>
		<col def="S32">Source</col>
	</table>

	<table name="MoveFile">
		<col key="yes" def="s72">FileKey</col>
		<col def="s72">Component_</col>
		<col def="L255">SourceName</col>
		<col def="L255">DestName</col>
		<col def="S72">SourceFolder</col>
		<col def="s72">DestFolder</col>
		<col def="i2">Options</col>
	</table>

	<table name="MsiAssembly">
		<col key="yes" def="s72">Component_</col>
		<col def="s38">Feature_</col>
		<col def="S72">File_Manifest</col>
		<col def="S72">File_Application</col>
		<col def="I2">Attributes</col>
	</table>

	<table name="MsiAssemblyName">
		<col key="yes" def="s72">Component_</col>
		<col key="yes" def="s255">Name</col>
		<col def="s255">Value</col>
	</table>

	<table name="MsiDigitalCertificate">
		<col key="yes" def="s72">DigitalCertificate</col>
		<col def="v0">CertData</col>
	</table>

	<table name="MsiDigitalSignature">
		<col key="yes" def="s32">Table</col>
		<col key="yes" def="s72">SignObject</col>
		<col def="s72">DigitalCertificate_</col>
		<col def="V0">Hash</col>
	</table>

	<table name="MsiDriverPackages">
		<col key="yes" def="s72">Component</col>
		<col def="i4">Flags</col>
		<col def="I4">Sequence</col>
		<col def="S0">ReferenceComponents</col>
	</table>

	<table name="MsiEmbeddedChainer">
		<col key="yes" def="s72">MsiEmbeddedChainer</col>
		<col def="S255">Condition</col>
		<col def="S255">CommandLine</col>
		<col def="s72">Source</col>
		<col def="I4">Type</col>
	</table>

	<table name="MsiEmbeddedUI">
		<col key="yes" def="s72">MsiEmbeddedUI</col>
		<col def="s255">FileName</col>
		<col def="i2">Attributes</col>
		<col def="I4">MessageFilter</col>
		<col def="V0">Data</col>
		<col def="S255">ISBuildSourcePath</col>
	</table>

	<table name="MsiFileHash">
		<col key="yes" def="s72">File_</col>
		<col def="i2">Options</col>
		<col def="i4">HashPart1</col>
		<col def="i4">HashPart2</col>
		<col def="i4">HashPart3</col>
		<col def="i4">HashPart4</col>
	</table>

	<table name="MsiLockPermissionsEx">
		<col key="yes" def="s72">MsiLockPermissionsEx</col>
		<col def="s72">LockObject</col>
		<col def="s32">Table</col>
		<col def="s0">SDDLText</col>
		<col def="S255">Condition</col>
	</table>

	<table name="MsiPackageCertificate">
		<col key="yes" def="s72">PackageCertificate</col>
		<col def="s72">DigitalCertificate_</col>
	</table>

	<table name="MsiPatchCertificate">
		<col key="yes" def="s72">PatchCertificate</col>
		<col def="s72">DigitalCertificate_</col>
	</table>

	<table name="MsiPatchMetadata">
		<col key="yes" def="s72">PatchConfiguration_</col>
		<col key="yes" def="S72">Company</col>
		<col key="yes" def="s72">Property</col>
		<col def="S0">Value</col>
	</table>

	<table name="MsiPatchOldAssemblyFile">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="S72">Assembly_</col>
	</table>

	<table name="MsiPatchOldAssemblyName">
		<col key="yes" def="s72">Assembly</col>
		<col key="yes" def="s255">Name</col>
		<col def="S255">Value</col>
	</table>

	<table name="MsiPatchSequence">
		<col key="yes" def="s72">PatchConfiguration_</col>
		<col key="yes" def="s0">PatchFamily</col>
		<col key="yes" def="S0">Target</col>
		<col def="s0">Sequence</col>
		<col def="i2">Supersede</col>
	</table>

	<table name="MsiServiceConfig">
		<col key="yes" def="s72">MsiServiceConfig</col>
		<col def="s255">Name</col>
		<col def="i2">Event</col>
		<col def="i4">ConfigType</col>
		<col def="S0">Argument</col>
		<col def="s72">Component_</col>
	</table>

	<table name="MsiServiceConfigFailureActions">
		<col key="yes" def="s72">MsiServiceConfigFailureActions</col>
		<col def="s255">Name</col>
		<col def="i2">Event</col>
		<col def="I4">ResetPeriod</col>
		<col def="L255">RebootMessage</col>
		<col def="L255">Command</col>
		<col def="S0">Actions</col>
		<col def="S0">DelayActions</col>
		<col def="s72">Component_</col>
	</table>

	<table name="MsiShortcutProperty">
		<col key="yes" def="s72">MsiShortcutProperty</col>
		<col def="s72">Shortcut_</col>
		<col def="s0">PropertyKey</col>
		<col def="s0">PropVariantValue</col>
	</table>

	<table name="ODBCAttribute">
		<col key="yes" def="s72">Driver_</col>
		<col key="yes" def="s40">Attribute</col>
		<col def="S255">Value</col>
	</table>

	<table name="ODBCDataSource">
		<col key="yes" def="s72">DataSource</col>
		<col def="s72">Component_</col>
		<col def="s255">Description</col>
		<col def="s255">DriverDescription</col>
		<col def="i2">Registration</col>
	</table>

	<table name="ODBCDriver">
		<col key="yes" def="s72">Driver</col>
		<col def="s72">Component_</col>
		<col def="s255">Description</col>
		<col def="s72">File_</col>
		<col def="S72">File_Setup</col>
	</table>

	<table name="ODBCSourceAttribute">
		<col key="yes" def="s72">DataSource_</col>
		<col key="yes" def="s32">Attribute</col>
		<col def="S255">Value</col>
	</table>

	<table name="ODBCTranslator">
		<col key="yes" def="s72">Translator</col>
		<col def="s72">Component_</col>
		<col def="s255">Description</col>
		<col def="s72">File_</col>
		<col def="S72">File_Setup</col>
	</table>

	<table name="Patch">
		<col key="yes" def="s72">File_</col>
		<col key="yes" def="i2">Sequence</col>
		<col def="i4">PatchSize</col>
		<col def="i2">Attributes</col>
		<col def="V0">Header</col>
		<col def="S38">StreamRef_</col>
		<col def="S255">ISBuildSourcePath</col>
	</table>

	<table name="PatchPackage">
		<col key="yes" def="s38">PatchId</col>
		<col def="i2">Media_</col>
	</table>

	<table name="ProgId">
		<col key="yes" def="s255">ProgId</col>
		<col def="S255">ProgId_Parent</col>
		<col def="S38">Class_</col>
		<col def="L255">Description</col>
		<col def="S72">Icon_</col>
		<col def="I2">IconIndex</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="Property">
		<col key="yes" def="s72">Property</col>
		<col def="L0">Value</col>
		<col def="S255">ISComments</col>
		<row><td>ARPINSTALLLOCATION</td><td/><td/></row>
		<row><td>ARPNOMODIFY</td><td>1</td><td/></row>
		<row><td>ARPPRODUCTICON</td><td>ARPPRODUCTICON.exe</td><td/></row>
		<row><td>ARPSIZE</td><td/><td/></row>
		<row><td>ARPURLINFOABOUT</td><td>##ID_STRING9##</td><td/></row>
		<row><td>AgreeToLicense</td><td>No</td><td/></row>
		<row><td>ApplicationUsers</td><td>AllUsers</td><td/></row>
		<row><td>DWUSINTERVAL</td><td>30</td><td/></row>
		<row><td>DWUSLINK</td><td>CEECD08889DBF76FCEACB72F4ECC978FA9CCB78F59ECC08FCE7B675F2E6B77CF099CB0DFE9AC</td><td/></row>
		<row><td>DefaultUIFont</td><td>ExpressDefault</td><td/></row>
		<row><td>DialogCaption</td><td>InstallShield for Windows Installer</td><td/></row>
		<row><td>DiskPrompt</td><td>[1]</td><td/></row>
		<row><td>DiskSerial</td><td>1234-5678</td><td/></row>
		<row><td>DisplayNameCustom</td><td>##IDS__DisplayName_Custom##</td><td/></row>
		<row><td>DisplayNameMinimal</td><td>##IDS__DisplayName_Minimal##</td><td/></row>
		<row><td>DisplayNameTypical</td><td>##IDS__DisplayName_Typical##</td><td/></row>
		<row><td>Display_IsBitmapDlg</td><td>1</td><td/></row>
		<row><td>ErrorDialog</td><td>SetupError</td><td/></row>
		<row><td>INSTALLLEVEL</td><td>200</td><td/></row>
		<row><td>ISCHECKFORPRODUCTUPDATES</td><td>1</td><td/></row>
		<row><td>ISENABLEDWUSFINISHDIALOG</td><td/><td/></row>
		<row><td>ISSHOWMSILOG</td><td/><td/></row>
		<row><td>ISVROOT_PORT_NO</td><td>0</td><td/></row>
		<row><td>IS_COMPLUS_PROGRESSTEXT_COST</td><td>##IDS_COMPLUS_PROGRESSTEXT_COST##</td><td/></row>
		<row><td>IS_COMPLUS_PROGRESSTEXT_INSTALL</td><td>##IDS_COMPLUS_PROGRESSTEXT_INSTALL##</td><td/></row>
		<row><td>IS_COMPLUS_PROGRESSTEXT_UNINSTALL</td><td>##IDS_COMPLUS_PROGRESSTEXT_UNINSTALL##</td><td/></row>
		<row><td>IS_PREVENT_DOWNGRADE_EXIT</td><td>##IDS_PREVENT_DOWNGRADE_EXIT##</td><td/></row>
		<row><td>IS_PROGMSG_TEXTFILECHANGS_REPLACE</td><td>##IDS_PROGMSG_TEXTFILECHANGS_REPLACE##</td><td/></row>
		<row><td>IS_PROGMSG_XML_COSTING</td><td>##IDS_PROGMSG_XML_COSTING##</td><td/></row>
		<row><td>IS_PROGMSG_XML_CREATE_FILE</td><td>##IDS_PROGMSG_XML_CREATE_FILE##</td><td/></row>
		<row><td>IS_PROGMSG_XML_FILES</td><td>##IDS_PROGMSG_XML_FILES##</td><td/></row>
		<row><td>IS_PROGMSG_XML_REMOVE_FILE</td><td>##IDS_PROGMSG_XML_REMOVE_FILE##</td><td/></row>
		<row><td>IS_PROGMSG_XML_ROLLBACK_FILES</td><td>##IDS_PROGMSG_XML_ROLLBACK_FILES##</td><td/></row>
		<row><td>IS_PROGMSG_XML_UPDATE_FILE</td><td>##IDS_PROGMSG_XML_UPDATE_FILE##</td><td/></row>
		<row><td>IS_SQLSERVER_AUTHENTICATION</td><td>0</td><td/></row>
		<row><td>IS_SQLSERVER_DATABASE</td><td/><td/></row>
		<row><td>IS_SQLSERVER_PASSWORD</td><td/><td/></row>
		<row><td>IS_SQLSERVER_SERVER</td><td/><td/></row>
		<row><td>IS_SQLSERVER_USERNAME</td><td>sa</td><td/></row>
		<row><td>InstallChoice</td><td>AR</td><td/></row>
		<row><td>LAUNCHPROGRAM</td><td>1</td><td/></row>
		<row><td>LAUNCHPROGRAMCOMPCODE</td><td>{8729D757-A6DA-4AFC-BB25-3CCFE2DBE652}</td><td/></row>
		<row><td>LAUNCHPROGRAMFILEKEY</td><td>File2</td><td/></row>
		<row><td>LAUNCHREADME</td><td>1</td><td/></row>
		<row><td>MSIFASTINSTALL</td><td>0</td><td/></row>
		<row><td>Manufacturer</td><td>##COMPANY_NAME##</td><td/></row>
		<row><td>PIDKEY</td><td/><td/></row>
		<row><td>PIDTemplate</td><td>12345&lt;###-%%%%%%%&gt;@@@@@</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEAPPPOOL</td><td>##IDS_PROGMSG_IIS_CREATEAPPPOOL##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEAPPPOOLS</td><td>##IDS_PROGMSG_IIS_CREATEAPPPOOLS##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEVROOT</td><td>##IDS_PROGMSG_IIS_CREATEVROOT##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEVROOTS</td><td>##IDS_PROGMSG_IIS_CREATEVROOTS##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEWEBSERVICEEXTENSION</td><td>##IDS_PROGMSG_IIS_CREATEWEBSERVICEEXTENSION##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEWEBSERVICEEXTENSIONS</td><td>##IDS_PROGMSG_IIS_CREATEWEBSERVICEEXTENSIONS##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEWEBSITE</td><td>##IDS_PROGMSG_IIS_CREATEWEBSITE##</td><td/></row>
		<row><td>PROGMSG_IIS_CREATEWEBSITES</td><td>##IDS_PROGMSG_IIS_CREATEWEBSITES##</td><td/></row>
		<row><td>PROGMSG_IIS_EXTRACT</td><td>##IDS_PROGMSG_IIS_EXTRACT##</td><td/></row>
		<row><td>PROGMSG_IIS_EXTRACTDONE</td><td>##IDS_PROGMSG_IIS_EXTRACTDONE##</td><td/></row>
		<row><td>PROGMSG_IIS_EXTRACTDONEz</td><td>##IDS_PROGMSG_IIS_EXTRACTDONE##</td><td/></row>
		<row><td>PROGMSG_IIS_EXTRACTzDONE</td><td>##IDS_PROGMSG_IIS_EXTRACTDONE##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEAPPPOOL</td><td>##IDS_PROGMSG_IIS_REMOVEAPPPOOL##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEAPPPOOLS</td><td>##IDS_PROGMSG_IIS_REMOVEAPPPOOLS##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVESITE</td><td>##IDS_PROGMSG_IIS_REMOVESITE##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEVROOT</td><td>##IDS_PROGMSG_IIS_REMOVEVROOT##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEVROOTS</td><td>##IDS_PROGMSG_IIS_REMOVEVROOTS##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEWEBSERVICEEXTENSION</td><td>##IDS_PROGMSG_IIS_REMOVEWEBSERVICEEXTENSION##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEWEBSERVICEEXTENSIONS</td><td>##IDS_PROGMSG_IIS_REMOVEWEBSERVICEEXTENSIONS##</td><td/></row>
		<row><td>PROGMSG_IIS_REMOVEWEBSITES</td><td>##IDS_PROGMSG_IIS_REMOVEWEBSITES##</td><td/></row>
		<row><td>PROGMSG_IIS_ROLLBACKAPPPOOLS</td><td>##IDS_PROGMSG_IIS_ROLLBACKAPPPOOLS##</td><td/></row>
		<row><td>PROGMSG_IIS_ROLLBACKVROOTS</td><td>##IDS_PROGMSG_IIS_ROLLBACKVROOTS##</td><td/></row>
		<row><td>PROGMSG_IIS_ROLLBACKWEBSERVICEEXTENSIONS</td><td>##IDS_PROGMSG_IIS_ROLLBACKWEBSERVICEEXTENSIONS##</td><td/></row>
		<row><td>PROGRAMFILETOLAUNCHATEND</td><td>[INSTALLDIR]Tween.プライマリ出力</td><td/></row>
		<row><td>ProductCode</td><td>{18D9215D-F789-4188-9CFB-51055E5D9EDF}</td><td/></row>
		<row><td>ProductName</td><td>Tween</td><td/></row>
		<row><td>ProductVersion</td><td>1.0.5.0</td><td/></row>
		<row><td>ProgressType0</td><td>install</td><td/></row>
		<row><td>ProgressType1</td><td>Installing</td><td/></row>
		<row><td>ProgressType2</td><td>installed</td><td/></row>
		<row><td>ProgressType3</td><td>installs</td><td/></row>
		<row><td>RebootYesNo</td><td>Yes</td><td/></row>
		<row><td>ReinstallFileVersion</td><td>o</td><td/></row>
		<row><td>ReinstallModeText</td><td>omus</td><td/></row>
		<row><td>ReinstallRepair</td><td>r</td><td/></row>
		<row><td>RestartManagerOption</td><td>CloseRestart</td><td/></row>
		<row><td>SERIALNUMBER</td><td/><td/></row>
		<row><td>SERIALNUMVALSUCCESSRETVAL</td><td>1</td><td/></row>
		<row><td>SHOWLAUNCHPROGRAM</td><td>-1</td><td/></row>
		<row><td>SecureCustomProperties</td><td>ISFOUNDNEWERPRODUCTVERSION;USERNAME;COMPANYNAME;ISX_SERIALNUM;SUPPORTDIR;DOTNETVERSION40FULL</td><td/></row>
		<row><td>SelectedSetupType</td><td>##IDS__DisplayName_Typical##</td><td/></row>
		<row><td>SetupType</td><td>Typical</td><td/></row>
		<row><td>UpgradeCode</td><td>{8796CB30-4C03-482A-8ABB-1141D870882D}</td><td/></row>
		<row><td>_IsMaintenance</td><td>Change</td><td/></row>
		<row><td>_IsSetupTypeMin</td><td>Typical</td><td/></row>
	</table>

	<table name="PublishComponent">
		<col key="yes" def="s38">ComponentId</col>
		<col key="yes" def="s255">Qualifier</col>
		<col key="yes" def="s72">Component_</col>
		<col def="L0">AppData</col>
		<col def="s38">Feature_</col>
	</table>

	<table name="RadioButton">
		<col key="yes" def="s72">Property</col>
		<col key="yes" def="i2">Order</col>
		<col def="s64">Value</col>
		<col def="i2">X</col>
		<col def="i2">Y</col>
		<col def="i2">Width</col>
		<col def="i2">Height</col>
		<col def="L64">Text</col>
		<col def="L50">Help</col>
		<col def="I4">ISControlId</col>
		<row><td>AgreeToLicense</td><td>1</td><td>No</td><td>0</td><td>15</td><td>291</td><td>15</td><td>##IDS__AgreeToLicense_0##</td><td/><td/></row>
		<row><td>AgreeToLicense</td><td>2</td><td>Yes</td><td>0</td><td>0</td><td>291</td><td>15</td><td>##IDS__AgreeToLicense_1##</td><td/><td/></row>
		<row><td>ApplicationUsers</td><td>1</td><td>AllUsers</td><td>1</td><td>7</td><td>290</td><td>14</td><td>##IDS__IsRegisterUserDlg_Anyone##</td><td/><td/></row>
		<row><td>ApplicationUsers</td><td>2</td><td>OnlyCurrentUser</td><td>1</td><td>23</td><td>290</td><td>14</td><td>##IDS__IsRegisterUserDlg_OnlyMe##</td><td/><td/></row>
		<row><td>RestartManagerOption</td><td>1</td><td>CloseRestart</td><td>6</td><td>9</td><td>331</td><td>14</td><td>##IDS__IsMsiRMFilesInUse_CloseRestart##</td><td/><td/></row>
		<row><td>RestartManagerOption</td><td>2</td><td>Reboot</td><td>6</td><td>21</td><td>331</td><td>14</td><td>##IDS__IsMsiRMFilesInUse_RebootAfter##</td><td/><td/></row>
		<row><td>_IsMaintenance</td><td>1</td><td>Change</td><td>0</td><td>0</td><td>290</td><td>14</td><td>##IDS__IsMaintenanceDlg_Modify##</td><td/><td/></row>
		<row><td>_IsMaintenance</td><td>2</td><td>Reinstall</td><td>0</td><td>60</td><td>290</td><td>14</td><td>##IDS__IsMaintenanceDlg_Repair##</td><td/><td/></row>
		<row><td>_IsMaintenance</td><td>3</td><td>Remove</td><td>0</td><td>120</td><td>290</td><td>14</td><td>##IDS__IsMaintenanceDlg_Remove##</td><td/><td/></row>
		<row><td>_IsSetupTypeMin</td><td>1</td><td>Typical</td><td>1</td><td>6</td><td>264</td><td>14</td><td>##IDS__IsSetupTypeMinDlg_Typical##</td><td/><td/></row>
	</table>

	<table name="RegLocator">
		<col key="yes" def="s72">Signature_</col>
		<col def="i2">Root</col>
		<col def="s255">Key</col>
		<col def="S255">Name</col>
		<col def="I2">Type</col>
		<row><td>DotNet40Full</td><td>2</td><td>SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full</td><td>Install</td><td>2</td></row>
	</table>

	<table name="Registry">
		<col key="yes" def="s72">Registry</col>
		<col def="i2">Root</col>
		<col def="s255">Key</col>
		<col def="S255">Name</col>
		<col def="S0">Value</col>
		<col def="s72">Component_</col>
		<col def="I4">ISAttributes</col>
	</table>

	<table name="RemoveFile">
		<col key="yes" def="s72">FileKey</col>
		<col def="s72">Component_</col>
		<col def="L255">FileName</col>
		<col def="s72">DirProperty</col>
		<col def="i2">InstallMode</col>
		<row><td>FileKey1</td><td>Component2</td><td/><td>uberme_1_ubermedia</td><td>2</td></row>
		<row><td>FileKey2</td><td>Component2</td><td/><td>tween</td><td>2</td></row>
		<row><td>FileKey3</td><td>Component2</td><td/><td>tweenp_1_tweenproject</td><td>2</td></row>
		<row><td>FileKey4</td><td>Component2</td><td/><td>tween1</td><td>2</td></row>
		<row><td>FileKey5</td><td>Component2</td><td/><td>tween</td><td>2</td></row>
		<row><td>FileKey6</td><td>Component2</td><td/><td>tween</td><td>2</td></row>
		<row><td>UNINST_Uninstall_Tween</td><td>NewComponent2</td><td/><td>tween1</td><td>2</td></row>
		<row><td>_Built</td><td>Component2</td><td/><td>tween1</td><td>2</td></row>
	</table>

	<table name="RemoveIniFile">
		<col key="yes" def="s72">RemoveIniFile</col>
		<col def="l255">FileName</col>
		<col def="S72">DirProperty</col>
		<col def="l96">Section</col>
		<col def="l128">Key</col>
		<col def="L255">Value</col>
		<col def="i2">Action</col>
		<col def="s72">Component_</col>
	</table>

	<table name="RemoveRegistry">
		<col key="yes" def="s72">RemoveRegistry</col>
		<col def="i2">Root</col>
		<col def="l255">Key</col>
		<col def="L255">Name</col>
		<col def="s72">Component_</col>
	</table>

	<table name="ReserveCost">
		<col key="yes" def="s72">ReserveKey</col>
		<col def="s72">Component_</col>
		<col def="S72">ReserveFolder</col>
		<col def="i4">ReserveLocal</col>
		<col def="i4">ReserveSource</col>
	</table>

	<table name="SFPCatalog">
		<col key="yes" def="s255">SFPCatalog</col>
		<col def="V0">Catalog</col>
		<col def="S0">Dependency</col>
	</table>

	<table name="SelfReg">
		<col key="yes" def="s72">File_</col>
		<col def="I2">Cost</col>
	</table>

	<table name="ServiceControl">
		<col key="yes" def="s72">ServiceControl</col>
		<col def="s255">Name</col>
		<col def="i2">Event</col>
		<col def="S255">Arguments</col>
		<col def="I2">Wait</col>
		<col def="s72">Component_</col>
	</table>

	<table name="ServiceInstall">
		<col key="yes" def="s72">ServiceInstall</col>
		<col def="s255">Name</col>
		<col def="L255">DisplayName</col>
		<col def="i4">ServiceType</col>
		<col def="i4">StartType</col>
		<col def="i4">ErrorControl</col>
		<col def="S255">LoadOrderGroup</col>
		<col def="S255">Dependencies</col>
		<col def="S255">StartName</col>
		<col def="S255">Password</col>
		<col def="S255">Arguments</col>
		<col def="s72">Component_</col>
		<col def="L255">Description</col>
	</table>

	<table name="Shortcut">
		<col key="yes" def="s72">Shortcut</col>
		<col def="s72">Directory_</col>
		<col def="l128">Name</col>
		<col def="s72">Component_</col>
		<col def="s255">Target</col>
		<col def="S255">Arguments</col>
		<col def="L255">Description</col>
		<col def="I2">Hotkey</col>
		<col def="S72">Icon_</col>
		<col def="I2">IconIndex</col>
		<col def="I2">ShowCmd</col>
		<col def="S72">WkDir</col>
		<col def="S255">DisplayResourceDLL</col>
		<col def="I2">DisplayResourceId</col>
		<col def="S255">DescriptionResourceDLL</col>
		<col def="I2">DescriptionResourceId</col>
		<col def="S255">ISComments</col>
		<col def="S255">ISShortcutName</col>
		<col def="I4">ISAttributes</col>
		<row><td>UNINST_Uninstall_Tween</td><td>tween1</td><td>UNINST|Uninstall Tween</td><td>NewComponent2</td><td>[SystemFolder]msiexec.exe</td><td>/x {18D9215D-F789-4188-9CFB-51055E5D9EDF}</td><td/><td/><td>UNINST_Uninstall_T_0C1103B7009948ECBE2C67FD0B6574D4.exe</td><td>0</td><td>1</td><td/><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>_Built</td><td>tween1</td><td>##IDS_SHORTCUT_DISPLAY_NAME3##</td><td>Component2</td><td>AlwaysInstall</td><td/><td/><td/><td/><td/><td>1</td><td/><td/><td/><td/><td/><td/><td/><td/></row>
	</table>

	<table name="Signature">
		<col key="yes" def="s72">Signature</col>
		<col def="s255">FileName</col>
		<col def="S20">MinVersion</col>
		<col def="S20">MaxVersion</col>
		<col def="I4">MinSize</col>
		<col def="I4">MaxSize</col>
		<col def="I4">MinDate</col>
		<col def="I4">MaxDate</col>
		<col def="S255">Languages</col>
	</table>

	<table name="TextStyle">
		<col key="yes" def="s72">TextStyle</col>
		<col def="s32">FaceName</col>
		<col def="i2">Size</col>
		<col def="I4">Color</col>
		<col def="I2">StyleBits</col>
		<row><td>Arial8</td><td>Arial</td><td>8</td><td/><td/></row>
		<row><td>Arial9</td><td>Arial</td><td>9</td><td/><td/></row>
		<row><td>ArialBlue10</td><td>Arial</td><td>10</td><td>16711680</td><td/></row>
		<row><td>ArialBlueStrike10</td><td>Arial</td><td>10</td><td>16711680</td><td>8</td></row>
		<row><td>CourierNew8</td><td>Courier New</td><td>8</td><td/><td/></row>
		<row><td>CourierNew9</td><td>Courier New</td><td>9</td><td/><td/></row>
		<row><td>ExpressDefault</td><td>Tahoma</td><td>8</td><td/><td/></row>
		<row><td>MSGothic9</td><td>MS Gothic</td><td>9</td><td/><td/></row>
		<row><td>MSSGreySerif8</td><td>MS Sans Serif</td><td>8</td><td>8421504</td><td/></row>
		<row><td>MSSWhiteSerif8</td><td>Tahoma</td><td>8</td><td>16777215</td><td/></row>
		<row><td>MSSansBold8</td><td>Tahoma</td><td>8</td><td/><td>1</td></row>
		<row><td>MSSansSerif8</td><td>MS Sans Serif</td><td>8</td><td/><td/></row>
		<row><td>MSSansSerif9</td><td>MS Sans Serif</td><td>9</td><td/><td/></row>
		<row><td>Tahoma10</td><td>Tahoma</td><td>10</td><td/><td/></row>
		<row><td>Tahoma8</td><td>Tahoma</td><td>8</td><td/><td/></row>
		<row><td>Tahoma9</td><td>Tahoma</td><td>9</td><td/><td/></row>
		<row><td>TahomaBold10</td><td>Tahoma</td><td>10</td><td/><td>1</td></row>
		<row><td>TahomaBold8</td><td>Tahoma</td><td>8</td><td/><td>1</td></row>
		<row><td>Times8</td><td>Times New Roman</td><td>8</td><td/><td/></row>
		<row><td>Times9</td><td>Times New Roman</td><td>9</td><td/><td/></row>
		<row><td>TimesItalic12</td><td>Times New Roman</td><td>12</td><td/><td>2</td></row>
		<row><td>TimesItalicBlue10</td><td>Times New Roman</td><td>10</td><td>16711680</td><td>2</td></row>
		<row><td>TimesRed16</td><td>Times New Roman</td><td>16</td><td>255</td><td/></row>
		<row><td>VerdanaBold14</td><td>Verdana</td><td>13</td><td/><td>1</td></row>
	</table>

	<table name="TypeLib">
		<col key="yes" def="s38">LibID</col>
		<col key="yes" def="i2">Language</col>
		<col key="yes" def="s72">Component_</col>
		<col def="I4">Version</col>
		<col def="L128">Description</col>
		<col def="S72">Directory_</col>
		<col def="s38">Feature_</col>
		<col def="I4">Cost</col>
	</table>

	<table name="UIText">
		<col key="yes" def="s72">Key</col>
		<col def="L255">Text</col>
		<row><td>AbsentPath</td><td/></row>
		<row><td>GB</td><td>##IDS_UITEXT_GB##</td></row>
		<row><td>KB</td><td>##IDS_UITEXT_KB##</td></row>
		<row><td>MB</td><td>##IDS_UITEXT_MB##</td></row>
		<row><td>MenuAbsent</td><td>##IDS_UITEXT_FeatureNotAvailable##</td></row>
		<row><td>MenuAdvertise</td><td>##IDS_UITEXT_FeatureInstalledWhenRequired2##</td></row>
		<row><td>MenuAllCD</td><td>##IDS_UITEXT_FeatureInstalledCD##</td></row>
		<row><td>MenuAllLocal</td><td>##IDS_UITEXT_FeatureInstalledLocal##</td></row>
		<row><td>MenuAllNetwork</td><td>##IDS_UITEXT_FeatureInstalledNetwork##</td></row>
		<row><td>MenuCD</td><td>##IDS_UITEXT_FeatureInstalledCD2##</td></row>
		<row><td>MenuLocal</td><td>##IDS_UITEXT_FeatureInstalledLocal2##</td></row>
		<row><td>MenuNetwork</td><td>##IDS_UITEXT_FeatureInstalledNetwork2##</td></row>
		<row><td>NewFolder</td><td>##IDS_UITEXT_Folder##</td></row>
		<row><td>SelAbsentAbsent</td><td>##IDS_UITEXT_GB##</td></row>
		<row><td>SelAbsentAdvertise</td><td>##IDS_UITEXT_FeatureInstalledWhenRequired##</td></row>
		<row><td>SelAbsentCD</td><td>##IDS_UITEXT_FeatureOnCD##</td></row>
		<row><td>SelAbsentLocal</td><td>##IDS_UITEXT_FeatureLocal##</td></row>
		<row><td>SelAbsentNetwork</td><td>##IDS_UITEXT_FeatureNetwork##</td></row>
		<row><td>SelAdvertiseAbsent</td><td>##IDS_UITEXT_FeatureUnavailable##</td></row>
		<row><td>SelAdvertiseAdvertise</td><td>##IDS_UITEXT_FeatureInstalledRequired##</td></row>
		<row><td>SelAdvertiseCD</td><td>##IDS_UITEXT_FeatureOnCD2##</td></row>
		<row><td>SelAdvertiseLocal</td><td>##IDS_UITEXT_FeatureLocal2##</td></row>
		<row><td>SelAdvertiseNetwork</td><td>##IDS_UITEXT_FeatureNetwork2##</td></row>
		<row><td>SelCDAbsent</td><td>##IDS_UITEXT_FeatureWillBeUninstalled##</td></row>
		<row><td>SelCDAdvertise</td><td>##IDS_UITEXT_FeatureWasCD##</td></row>
		<row><td>SelCDCD</td><td>##IDS_UITEXT_FeatureRunFromCD##</td></row>
		<row><td>SelCDLocal</td><td>##IDS_UITEXT_FeatureWasCDLocal##</td></row>
		<row><td>SelChildCostNeg</td><td>##IDS_UITEXT_FeatureFreeSpace##</td></row>
		<row><td>SelChildCostPos</td><td>##IDS_UITEXT_FeatureRequiredSpace##</td></row>
		<row><td>SelCostPending</td><td>##IDS_UITEXT_CompilingFeaturesCost##</td></row>
		<row><td>SelLocalAbsent</td><td>##IDS_UITEXT_FeatureCompletelyRemoved##</td></row>
		<row><td>SelLocalAdvertise</td><td>##IDS_UITEXT_FeatureRemovedUnlessRequired##</td></row>
		<row><td>SelLocalCD</td><td>##IDS_UITEXT_FeatureRemovedCD##</td></row>
		<row><td>SelLocalLocal</td><td>##IDS_UITEXT_FeatureRemainLocal##</td></row>
		<row><td>SelLocalNetwork</td><td>##IDS_UITEXT_FeatureRemoveNetwork##</td></row>
		<row><td>SelNetworkAbsent</td><td>##IDS_UITEXT_FeatureUninstallNoNetwork##</td></row>
		<row><td>SelNetworkAdvertise</td><td>##IDS_UITEXT_FeatureWasOnNetworkInstalled##</td></row>
		<row><td>SelNetworkLocal</td><td>##IDS_UITEXT_FeatureWasOnNetworkLocal##</td></row>
		<row><td>SelNetworkNetwork</td><td>##IDS_UITEXT_FeatureContinueNetwork##</td></row>
		<row><td>SelParentCostNegNeg</td><td>##IDS_UITEXT_FeatureSpaceFree##</td></row>
		<row><td>SelParentCostNegPos</td><td>##IDS_UITEXT_FeatureSpaceFree2##</td></row>
		<row><td>SelParentCostPosNeg</td><td>##IDS_UITEXT_FeatureSpaceFree3##</td></row>
		<row><td>SelParentCostPosPos</td><td>##IDS_UITEXT_FeatureSpaceFree4##</td></row>
		<row><td>TimeRemaining</td><td>##IDS_UITEXT_TimeRemaining##</td></row>
		<row><td>VolumeCostAvailable</td><td>##IDS_UITEXT_Available##</td></row>
		<row><td>VolumeCostDifference</td><td>##IDS_UITEXT_Differences##</td></row>
		<row><td>VolumeCostRequired</td><td>##IDS_UITEXT_Required##</td></row>
		<row><td>VolumeCostSize</td><td>##IDS_UITEXT_DiskSize##</td></row>
		<row><td>VolumeCostVolume</td><td>##IDS_UITEXT_Volume##</td></row>
		<row><td>bytes</td><td>##IDS_UITEXT_Bytes##</td></row>
	</table>

	<table name="Upgrade">
		<col key="yes" def="s38">UpgradeCode</col>
		<col key="yes" def="S20">VersionMin</col>
		<col key="yes" def="S20">VersionMax</col>
		<col key="yes" def="S255">Language</col>
		<col key="yes" def="i4">Attributes</col>
		<col def="S255">Remove</col>
		<col def="s72">ActionProperty</col>
		<col def="S72">ISDisplayName</col>
		<row><td>{00000000-0000-0000-0000-000000000000}</td><td>***ALL_VERSIONS***</td><td></td><td></td><td>2</td><td/><td>ISFOUNDNEWERPRODUCTVERSION</td><td>ISPreventDowngrade</td></row>
	</table>

	<table name="Verb">
		<col key="yes" def="s255">Extension_</col>
		<col key="yes" def="s32">Verb</col>
		<col def="I2">Sequence</col>
		<col def="L255">Command</col>
		<col def="L255">Argument</col>
	</table>

	<table name="_Validation">
		<col key="yes" def="s32">Table</col>
		<col key="yes" def="s32">Column</col>
		<col def="s4">Nullable</col>
		<col def="I4">MinValue</col>
		<col def="I4">MaxValue</col>
		<col def="S255">KeyTable</col>
		<col def="I2">KeyColumn</col>
		<col def="S32">Category</col>
		<col def="S255">Set</col>
		<col def="S255">Description</col>
		<row><td>ActionText</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to be described.</td></row>
		<row><td>ActionText</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Localized description displayed in progress dialog and log when action is executing.</td></row>
		<row><td>ActionText</td><td>Template</td><td>Y</td><td/><td/><td/><td/><td>Template</td><td/><td>Optional localized format template used to format action data records for display during action execution.</td></row>
		<row><td>AdminExecuteSequence</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to invoke, either in the engine or the handler DLL.</td></row>
		<row><td>AdminExecuteSequence</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Optional expression which skips the action if evaluates to expFalse.If the expression syntax is invalid, the engine will terminate, returning iesBadActionData.</td></row>
		<row><td>AdminExecuteSequence</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store MM Custom Action Types</td></row>
		<row><td>AdminExecuteSequence</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Sequence.</td></row>
		<row><td>AdminExecuteSequence</td><td>Sequence</td><td>Y</td><td>-4</td><td>32767</td><td/><td/><td/><td/><td>Number that determines the sort order in which the actions are to be executed.  Leave blank to suppress action.</td></row>
		<row><td>AdminUISequence</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to invoke, either in the engine or the handler DLL.</td></row>
		<row><td>AdminUISequence</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Optional expression which skips the action if evaluates to expFalse.If the expression syntax is invalid, the engine will terminate, returning iesBadActionData.</td></row>
		<row><td>AdminUISequence</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store MM Custom Action Types</td></row>
		<row><td>AdminUISequence</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Sequence.</td></row>
		<row><td>AdminUISequence</td><td>Sequence</td><td>Y</td><td>-4</td><td>32767</td><td/><td/><td/><td/><td>Number that determines the sort order in which the actions are to be executed.  Leave blank to suppress action.</td></row>
		<row><td>AdvtExecuteSequence</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to invoke, either in the engine or the handler DLL.</td></row>
		<row><td>AdvtExecuteSequence</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Optional expression which skips the action if evaluates to expFalse.If the expression syntax is invalid, the engine will terminate, returning iesBadActionData.</td></row>
		<row><td>AdvtExecuteSequence</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store MM Custom Action Types</td></row>
		<row><td>AdvtExecuteSequence</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Sequence.</td></row>
		<row><td>AdvtExecuteSequence</td><td>Sequence</td><td>Y</td><td>-4</td><td>32767</td><td/><td/><td/><td/><td>Number that determines the sort order in which the actions are to be executed.  Leave blank to suppress action.</td></row>
		<row><td>AdvtUISequence</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to invoke, either in the engine or the handler DLL.</td></row>
		<row><td>AdvtUISequence</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Optional expression which skips the action if evaluates to expFalse.If the expression syntax is invalid, the engine will terminate, returning iesBadActionData.</td></row>
		<row><td>AdvtUISequence</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store MM Custom Action Types</td></row>
		<row><td>AdvtUISequence</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Sequence.</td></row>
		<row><td>AdvtUISequence</td><td>Sequence</td><td>Y</td><td>-4</td><td>32767</td><td/><td/><td/><td/><td>Number that determines the sort order in which the actions are to be executed.  Leave blank to suppress action.</td></row>
		<row><td>AppId</td><td>ActivateAtStorage</td><td>Y</td><td>0</td><td>1</td><td/><td/><td/><td/><td/></row>
		<row><td>AppId</td><td>AppId</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td/></row>
		<row><td>AppId</td><td>DllSurrogate</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>AppId</td><td>LocalService</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>AppId</td><td>RemoteServerName</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td/></row>
		<row><td>AppId</td><td>RunAsInteractiveUser</td><td>Y</td><td>0</td><td>1</td><td/><td/><td/><td/><td/></row>
		<row><td>AppId</td><td>ServiceParameters</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>AppSearch</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The property associated with a Signature</td></row>
		<row><td>AppSearch</td><td>Signature_</td><td>N</td><td/><td/><td>ISXmlLocator;Signature</td><td>1</td><td>Identifier</td><td/><td>The Signature_ represents a unique file signature and is also the foreign key in the Signature,  RegLocator, IniLocator, CompLocator and the DrLocator tables.</td></row>
		<row><td>BBControl</td><td>Attributes</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>A 32-bit word that specifies the attribute flags to be applied to this control.</td></row>
		<row><td>BBControl</td><td>BBControl</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the control. This name must be unique within a billboard, but can repeat on different billboard.</td></row>
		<row><td>BBControl</td><td>Billboard_</td><td>N</td><td/><td/><td>Billboard</td><td>1</td><td>Identifier</td><td/><td>External key to the Billboard table, name of the billboard.</td></row>
		<row><td>BBControl</td><td>Height</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Height of the bounding rectangle of the control.</td></row>
		<row><td>BBControl</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>A string used to set the initial text contained within a control (if appropriate).</td></row>
		<row><td>BBControl</td><td>Type</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The type of the control.</td></row>
		<row><td>BBControl</td><td>Width</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Width of the bounding rectangle of the control.</td></row>
		<row><td>BBControl</td><td>X</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Horizontal coordinate of the upper left corner of the bounding rectangle of the control.</td></row>
		<row><td>BBControl</td><td>Y</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Vertical coordinate of the upper left corner of the bounding rectangle of the control.</td></row>
		<row><td>Billboard</td><td>Action</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The name of an action. The billboard is displayed during the progress messages received from this action.</td></row>
		<row><td>Billboard</td><td>Billboard</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the billboard.</td></row>
		<row><td>Billboard</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>An external key to the Feature Table. The billboard is shown only if this feature is being installed.</td></row>
		<row><td>Billboard</td><td>Ordering</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>A positive integer. If there is more than one billboard corresponding to an action they will be shown in the order defined by this column.</td></row>
		<row><td>Binary</td><td>Data</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>Binary stream. The binary icon data in PE (.DLL or .EXE) or icon (.ICO) format.</td></row>
		<row><td>Binary</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path to the ICO or EXE file.</td></row>
		<row><td>Binary</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Unique key identifying the binary data.</td></row>
		<row><td>BindImage</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>The index into the File table. This must be an executable file.</td></row>
		<row><td>BindImage</td><td>Path</td><td>Y</td><td/><td/><td/><td/><td>Paths</td><td/><td>A list of ;  delimited paths that represent the paths to be searched for the import DLLS. The list is usually a list of properties each enclosed within square brackets [] .</td></row>
		<row><td>CCPSearch</td><td>Signature_</td><td>N</td><td/><td/><td>Signature</td><td>1</td><td>Identifier</td><td/><td>The Signature_ represents a unique file signature and is also the foreign key in the Signature,  RegLocator, IniLocator, CompLocator and the DrLocator tables.</td></row>
		<row><td>CheckBox</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A named property to be tied to the item.</td></row>
		<row><td>CheckBox</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value string associated with the item.</td></row>
		<row><td>Class</td><td>AppId_</td><td>Y</td><td/><td/><td>AppId</td><td>1</td><td>Guid</td><td/><td>Optional AppID containing DCOM information for associated application (string GUID).</td></row>
		<row><td>Class</td><td>Argument</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>optional argument for LocalServers.</td></row>
		<row><td>Class</td><td>Attributes</td><td>Y</td><td/><td>32767</td><td/><td/><td/><td/><td>Class registration attributes.</td></row>
		<row><td>Class</td><td>CLSID</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td>The CLSID of an OLE factory.</td></row>
		<row><td>Class</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table, specifying the component for which to return a path when called through LocateComponent.</td></row>
		<row><td>Class</td><td>Context</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The numeric server context for this server. CLSCTX_xxxx</td></row>
		<row><td>Class</td><td>DefInprocHandler</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td>1;2;3</td><td>Optional default inproc handler.  Only optionally provided if Context=CLSCTX_LOCAL_SERVER.  Typically "ole32.dll" or "mapi32.dll"</td></row>
		<row><td>Class</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Localized description for the Class.</td></row>
		<row><td>Class</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Feature Table, specifying the feature to validate or install in order for the CLSID factory to be operational.</td></row>
		<row><td>Class</td><td>FileTypeMask</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Optional string containing information for the HKCRthis CLSID) key. If multiple patterns exist, they must be delimited by a semicolon, and numeric subkeys will be generated: 0,1,2...</td></row>
		<row><td>Class</td><td>IconIndex</td><td>Y</td><td>-32767</td><td>32767</td><td/><td/><td/><td/><td>Optional icon index.</td></row>
		<row><td>Class</td><td>Icon_</td><td>Y</td><td/><td/><td>Icon</td><td>1</td><td>Identifier</td><td/><td>Optional foreign key into the Icon Table, specifying the icon file associated with this CLSID. Will be written under the DefaultIcon key.</td></row>
		<row><td>Class</td><td>ProgId_Default</td><td>Y</td><td/><td/><td>ProgId</td><td>1</td><td>Text</td><td/><td>Optional ProgId associated with this CLSID.</td></row>
		<row><td>ComboBox</td><td>Order</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>A positive integer used to determine the ordering of the items within one list.	The integers do not have to be consecutive.</td></row>
		<row><td>ComboBox</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A named property to be tied to this item. All the items tied to the same property become part of the same combobox.</td></row>
		<row><td>ComboBox</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The visible text to be assigned to the item. Optional. If this entry or the entire column is missing, the text is the same as the value.</td></row>
		<row><td>ComboBox</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value string associated with this item. Selecting the line will set the associated property to this value.</td></row>
		<row><td>CompLocator</td><td>ComponentId</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td>A string GUID unique to this component, version, and language.</td></row>
		<row><td>CompLocator</td><td>Signature_</td><td>N</td><td/><td/><td>Signature</td><td>1</td><td>Identifier</td><td/><td>The table key. The Signature_ represents a unique file signature and is also the foreign key in the Signature table.</td></row>
		<row><td>CompLocator</td><td>Type</td><td>Y</td><td>0</td><td>1</td><td/><td/><td/><td/><td>A boolean value that determines if the registry value is a filename or a directory location.</td></row>
		<row><td>Complus</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing Component that controls the ComPlus component.</td></row>
		<row><td>Complus</td><td>ExpType</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>ComPlus component attributes.</td></row>
		<row><td>Component</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Remote execution option, one of irsEnum</td></row>
		<row><td>Component</td><td>Component</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular component record.</td></row>
		<row><td>Component</td><td>ComponentId</td><td>Y</td><td/><td/><td/><td/><td>Guid</td><td/><td>A string GUID unique to this component, version, and language.</td></row>
		<row><td>Component</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>A conditional statement that will disable this component if the specified condition evaluates to the 'True' state. If a component is disabled, it will not be installed, regardless of the 'Action' state associated with the component.</td></row>
		<row><td>Component</td><td>Directory_</td><td>N</td><td/><td/><td>Directory</td><td>1</td><td>Identifier</td><td/><td>Required key of a Directory table record. This is actually a property name whose value contains the actual path, set either by the AppSearch action or with the default setting obtained from the Directory table.</td></row>
		<row><td>Component</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store Installshield custom properties of a component.</td></row>
		<row><td>Component</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>User Comments.</td></row>
		<row><td>Component</td><td>ISDotNetInstallerArgsCommit</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Arguments passed to the key file of the component if if implements the .NET Installer class</td></row>
		<row><td>Component</td><td>ISDotNetInstallerArgsInstall</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Arguments passed to the key file of the component if if implements the .NET Installer class</td></row>
		<row><td>Component</td><td>ISDotNetInstallerArgsRollback</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Arguments passed to the key file of the component if if implements the .NET Installer class</td></row>
		<row><td>Component</td><td>ISDotNetInstallerArgsUninstall</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Arguments passed to the key file of the component if if implements the .NET Installer class</td></row>
		<row><td>Component</td><td>ISRegFileToMergeAtBuild</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Path and File name of a .REG file to merge into the component at build time.</td></row>
		<row><td>Component</td><td>ISScanAtBuildFile</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>File used by the Dot Net scanner to populate dependant assemblies' File_Application field.</td></row>
		<row><td>Component</td><td>KeyPath</td><td>Y</td><td/><td/><td>File;ODBCDataSource;Registry</td><td>1</td><td>Identifier</td><td/><td>Either the primary key into the File table, Registry table, or ODBCDataSource table. This extract path is stored when the component is installed, and is used to detect the presence of the component and to return the path to it.</td></row>
		<row><td>Condition</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Expression evaluated to determine if Level in the Feature table is to change.</td></row>
		<row><td>Condition</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Reference to a Feature entry in Feature table.</td></row>
		<row><td>Condition</td><td>Level</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>New selection Level to set in Feature table if Condition evaluates to TRUE.</td></row>
		<row><td>Control</td><td>Attributes</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>A 32-bit word that specifies the attribute flags to be applied to this control.</td></row>
		<row><td>Control</td><td>Binary_</td><td>Y</td><td/><td/><td>Binary</td><td>1</td><td>Identifier</td><td/><td>External key to the Binary table.</td></row>
		<row><td>Control</td><td>Control</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the control. This name must be unique within a dialog, but can repeat on different dialogs.</td></row>
		<row><td>Control</td><td>Control_Next</td><td>Y</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>The name of an other control on the same dialog. This link defines the tab order of the controls. The links have to form one or more cycles!</td></row>
		<row><td>Control</td><td>Dialog_</td><td>N</td><td/><td/><td>Dialog</td><td>1</td><td>Identifier</td><td/><td>External key to the Dialog table, name of the dialog.</td></row>
		<row><td>Control</td><td>Height</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Height of the bounding rectangle of the control.</td></row>
		<row><td>Control</td><td>Help</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The help strings used with the button. The text is optional.</td></row>
		<row><td>Control</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path to .rtf file for scrollable text control</td></row>
		<row><td>Control</td><td>ISControlId</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>A number used to represent the control ID of the Control, Used in Dialog export</td></row>
		<row><td>Control</td><td>ISWindowStyle</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>A 32-bit word that specifies non-MSI window styles to be applied to this control.</td></row>
		<row><td>Control</td><td>Property</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The name of a defined property to be linked to this control.</td></row>
		<row><td>Control</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>A string used to set the initial text contained within a control (if appropriate).</td></row>
		<row><td>Control</td><td>Type</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The type of the control.</td></row>
		<row><td>Control</td><td>Width</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Width of the bounding rectangle of the control.</td></row>
		<row><td>Control</td><td>X</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Horizontal coordinate of the upper left corner of the bounding rectangle of the control.</td></row>
		<row><td>Control</td><td>Y</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Vertical coordinate of the upper left corner of the bounding rectangle of the control.</td></row>
		<row><td>ControlCondition</td><td>Action</td><td>N</td><td/><td/><td/><td/><td/><td>Default;Disable;Enable;Hide;Show</td><td>The desired action to be taken on the specified control.</td></row>
		<row><td>ControlCondition</td><td>Condition</td><td>N</td><td/><td/><td/><td/><td>Condition</td><td/><td>A standard conditional statement that specifies under which conditions the action should be triggered.</td></row>
		<row><td>ControlCondition</td><td>Control_</td><td>N</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>A foreign key to the Control table, name of the control.</td></row>
		<row><td>ControlCondition</td><td>Dialog_</td><td>N</td><td/><td/><td>Dialog</td><td>1</td><td>Identifier</td><td/><td>A foreign key to the Dialog table, name of the dialog.</td></row>
		<row><td>ControlEvent</td><td>Argument</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>A value to be used as a modifier when triggering a particular event.</td></row>
		<row><td>ControlEvent</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>A standard conditional statement that specifies under which conditions an event should be triggered.</td></row>
		<row><td>ControlEvent</td><td>Control_</td><td>N</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>A foreign key to the Control table, name of the control</td></row>
		<row><td>ControlEvent</td><td>Dialog_</td><td>N</td><td/><td/><td>Dialog</td><td>1</td><td>Identifier</td><td/><td>A foreign key to the Dialog table, name of the dialog.</td></row>
		<row><td>ControlEvent</td><td>Event</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>An identifier that specifies the type of the event that should take place when the user interacts with control specified by the first two entries.</td></row>
		<row><td>ControlEvent</td><td>Ordering</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>An integer used to order several events tied to the same control. Can be left blank.</td></row>
		<row><td>CreateFolder</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table.</td></row>
		<row><td>CreateFolder</td><td>Directory_</td><td>N</td><td/><td/><td>Directory</td><td>1</td><td>Identifier</td><td/><td>Primary key, could be foreign key into the Directory table.</td></row>
		<row><td>CustomAction</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, name of action, normally appears in sequence table unless private use.</td></row>
		<row><td>CustomAction</td><td>ExtendedType</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The numeric custom action type info flags.</td></row>
		<row><td>CustomAction</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments for this custom action.</td></row>
		<row><td>CustomAction</td><td>Source</td><td>Y</td><td/><td/><td/><td/><td>CustomSource</td><td/><td>The table reference of the source of the code.</td></row>
		<row><td>CustomAction</td><td>Target</td><td>Y</td><td/><td/><td>ISDLLWrapper;ISInstallScriptAction</td><td>1</td><td>Formatted</td><td/><td>Excecution parameter, depends on the type of custom action</td></row>
		<row><td>CustomAction</td><td>Type</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>The numeric custom action type, consisting of source location, code type, entry, option flags.</td></row>
		<row><td>Dialog</td><td>Attributes</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>A 32-bit word that specifies the attribute flags to be applied to this dialog.</td></row>
		<row><td>Dialog</td><td>Control_Cancel</td><td>Y</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>Defines the cancel control. Hitting escape or clicking on the close icon on the dialog is equivalent to pushing this button.</td></row>
		<row><td>Dialog</td><td>Control_Default</td><td>Y</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>Defines the default control. Hitting return is equivalent to pushing this button.</td></row>
		<row><td>Dialog</td><td>Control_First</td><td>N</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>Defines the control that has the focus when the dialog is created.</td></row>
		<row><td>Dialog</td><td>Dialog</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the dialog.</td></row>
		<row><td>Dialog</td><td>HCentering</td><td>N</td><td>0</td><td>100</td><td/><td/><td/><td/><td>Horizontal position of the dialog on a 0-100 scale. 0 means left end, 100 means right end of the screen, 50 center.</td></row>
		<row><td>Dialog</td><td>Height</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Height of the bounding rectangle of the dialog.</td></row>
		<row><td>Dialog</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments for this dialog.</td></row>
		<row><td>Dialog</td><td>ISResourceId</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>A Number the Specifies the Dialog ID to be used in Dialog Export</td></row>
		<row><td>Dialog</td><td>ISWindowStyle</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>A 32-bit word that specifies non-MSI window styles to be applied to this control. This is only used in Script Based Setups.</td></row>
		<row><td>Dialog</td><td>TextStyle_</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign Key into TextStyle table, only used in Script Based Projects.</td></row>
		<row><td>Dialog</td><td>Title</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>A text string specifying the title to be displayed in the title bar of the dialog's window.</td></row>
		<row><td>Dialog</td><td>VCentering</td><td>N</td><td>0</td><td>100</td><td/><td/><td/><td/><td>Vertical position of the dialog on a 0-100 scale. 0 means top end, 100 means bottom end of the screen, 50 center.</td></row>
		<row><td>Dialog</td><td>Width</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Width of the bounding rectangle of the dialog.</td></row>
		<row><td>Directory</td><td>DefaultDir</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The default sub-path under parent's path.</td></row>
		<row><td>Directory</td><td>Directory</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Unique identifier for directory entry, primary key. If a property by this name is defined, it contains the full path to the directory.</td></row>
		<row><td>Directory</td><td>Directory_Parent</td><td>Y</td><td/><td/><td>Directory</td><td>1</td><td>Identifier</td><td/><td>Reference to the entry in this table specifying the default parent directory. A record parented to itself or with a Null parent represents a root of the install tree.</td></row>
		<row><td>Directory</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td>0;1;2;3;4;5;6;7</td><td>This is used to store Installshield custom properties of a directory.  Currently the only one is Shortcut.</td></row>
		<row><td>Directory</td><td>ISDescription</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Description of folder</td></row>
		<row><td>Directory</td><td>ISFolderName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>This is used in Pro projects because the pro identifier used in the tree wasn't necessarily unique.</td></row>
		<row><td>DrLocator</td><td>Depth</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The depth below the path to which the Signature_ is recursively searched. If absent, the depth is assumed to be 0.</td></row>
		<row><td>DrLocator</td><td>Parent</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The parent file signature. It is also a foreign key in the Signature table. If null and the Path column does not expand to a full path, then all the fixed drives of the user system are searched using the Path.</td></row>
		<row><td>DrLocator</td><td>Path</td><td>Y</td><td/><td/><td/><td/><td>AnyPath</td><td/><td>The path on the user system. This is a either a subpath below the value of the Parent or a full path. The path may contain properties enclosed within [ ] that will be expanded.</td></row>
		<row><td>DrLocator</td><td>Signature_</td><td>N</td><td/><td/><td>Signature</td><td>1</td><td>Identifier</td><td/><td>The Signature_ represents a unique file signature and is also the foreign key in the Signature table.</td></row>
		<row><td>DuplicateFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing Component that controls the duplicate file.</td></row>
		<row><td>DuplicateFile</td><td>DestFolder</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of a property whose value is assumed to resolve to the full pathname to a destination folder.</td></row>
		<row><td>DuplicateFile</td><td>DestName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Filename to be given to the duplicate file.</td></row>
		<row><td>DuplicateFile</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular file entry</td></row>
		<row><td>DuplicateFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing the source file to be duplicated.</td></row>
		<row><td>Environment</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table referencing component that controls the installing of the environmental value.</td></row>
		<row><td>Environment</td><td>Environment</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Unique identifier for the environmental variable setting</td></row>
		<row><td>Environment</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the environmental value.</td></row>
		<row><td>Environment</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value to set in the environmental settings.</td></row>
		<row><td>Error</td><td>Error</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Integer error number, obtained from header file IError(...) macros.</td></row>
		<row><td>Error</td><td>Message</td><td>Y</td><td/><td/><td/><td/><td>Template</td><td/><td>Error formatting template, obtained from user ed. or localizers.</td></row>
		<row><td>EventMapping</td><td>Attribute</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The name of the control attribute, that is set when this event is received.</td></row>
		<row><td>EventMapping</td><td>Control_</td><td>N</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>A foreign key to the Control table, name of the control.</td></row>
		<row><td>EventMapping</td><td>Dialog_</td><td>N</td><td/><td/><td>Dialog</td><td>1</td><td>Identifier</td><td/><td>A foreign key to the Dialog table, name of the Dialog.</td></row>
		<row><td>EventMapping</td><td>Event</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>An identifier that specifies the type of the event that the control subscribes to.</td></row>
		<row><td>Extension</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table, specifying the component for which to return a path when called through LocateComponent.</td></row>
		<row><td>Extension</td><td>Extension</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The extension associated with the table row.</td></row>
		<row><td>Extension</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Feature Table, specifying the feature to validate or install in order for the CLSID factory to be operational.</td></row>
		<row><td>Extension</td><td>MIME_</td><td>Y</td><td/><td/><td>MIME</td><td>1</td><td>Text</td><td/><td>Optional Context identifier, typically "type/format" associated with the extension</td></row>
		<row><td>Extension</td><td>ProgId_</td><td>Y</td><td/><td/><td>ProgId</td><td>1</td><td>Text</td><td/><td>Optional ProgId associated with this extension.</td></row>
		<row><td>Feature</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td>0;1;2;4;5;6;8;9;10;16;17;18;20;21;22;24;25;26;32;33;34;36;37;38;48;49;50;52;53;54</td><td>Feature attributes</td></row>
		<row><td>Feature</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Longer descriptive text describing a visible feature item.</td></row>
		<row><td>Feature</td><td>Directory_</td><td>Y</td><td/><td/><td>Directory</td><td>1</td><td>UpperCase</td><td/><td>The name of the Directory that can be configured by the UI. A non-null value will enable the browse button.</td></row>
		<row><td>Feature</td><td>Display</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Numeric sort order, used to force a specific display ordering.</td></row>
		<row><td>Feature</td><td>Feature</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular feature record.</td></row>
		<row><td>Feature</td><td>Feature_Parent</td><td>Y</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Optional key of a parent record in the same table. If the parent is not selected, then the record will not be installed. Null indicates a root item.</td></row>
		<row><td>Feature</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Comments</td></row>
		<row><td>Feature</td><td>ISFeatureCabName</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Name of CAB used when compressing CABs by Feature. Used to override build generated name for CAB file.</td></row>
		<row><td>Feature</td><td>ISProFeatureName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the feature used by pro projects.  This doesn't have to be unique.</td></row>
		<row><td>Feature</td><td>ISReleaseFlags</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Release Flags that specify whether this  feature will be built in a particular release.</td></row>
		<row><td>Feature</td><td>Level</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The install level at which record will be initially selected. An install level of 0 will disable an item and prevent its display.</td></row>
		<row><td>Feature</td><td>Title</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Short text identifying a visible feature item.</td></row>
		<row><td>FeatureComponents</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Component table.</td></row>
		<row><td>FeatureComponents</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>File</td><td>Attributes</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Integer containing bit flags representing file attributes (with the decimal value of each bit position in parentheses)</td></row>
		<row><td>File</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing Component that controls the file.</td></row>
		<row><td>File</td><td>File</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token, must match identifier in cabinet.  For uncompressed files, this field is ignored.</td></row>
		<row><td>File</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>File name used for installation.  This may contain a "short name|long name" pair.  It may be just a long name, hence it cannot be of the Filename data type.</td></row>
		<row><td>File</td><td>FileSize</td><td>N</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>Size of file in bytes (long integer).</td></row>
		<row><td>File</td><td>ISAttributes</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>This field contains the following attributes: UseSystemSettings(0x1)</td></row>
		<row><td>File</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path, the category is of Text instead of Path because of potential use of path variables.</td></row>
		<row><td>File</td><td>ISComponentSubFolder_</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key referencing component subfolder containing this file.  Only for Pro.</td></row>
		<row><td>File</td><td>Language</td><td>Y</td><td/><td/><td/><td/><td>Language</td><td/><td>List of decimal language Ids, comma-separated if more than one.</td></row>
		<row><td>File</td><td>Sequence</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>Sequence with respect to the media images; order must track cabinet order.</td></row>
		<row><td>File</td><td>Version</td><td>Y</td><td/><td/><td>File</td><td>1</td><td>Version</td><td/><td>Version string for versioned files;  Blank for unversioned files.</td></row>
		<row><td>FileSFPCatalog</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>File associated with the catalog</td></row>
		<row><td>FileSFPCatalog</td><td>SFPCatalog_</td><td>N</td><td/><td/><td>SFPCatalog</td><td>1</td><td>Text</td><td/><td>Catalog associated with the file</td></row>
		<row><td>Font</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Primary key, foreign key into File table referencing font file.</td></row>
		<row><td>Font</td><td>FontTitle</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Font name.</td></row>
		<row><td>ISAssistantTag</td><td>Data</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISAssistantTag</td><td>Tag</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Color</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Duration</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Effect</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Font</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>ISBillboard</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Origin</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Sequence</td><td>N</td><td>-32767</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Style</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Target</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Title</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>X</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISBillBoard</td><td>Y</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>AppName</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>CompanyName</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>Component_</td><td>Y</td><td/><td/><td>Component</td><td>1</td><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>DefDir</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>DeleteMedia</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>DesktopTargetDir</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>DeviceFile</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>IconIndex</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>IconPath</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallNetCF</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallNetCF2</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallSQLClient</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallSQLClient2</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallSQLDev</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallSQLDev2</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallSQLServer</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>InstallSQLServer2</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>NoUninstall</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>PVKFile</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>PostXML</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>PreXML</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>RawDeviceFile</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>SPCFile</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEApp</td><td>SPCPwd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEDir</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEDir</td><td>DirKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEDir</td><td>DirParent</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEDir</td><td>DirValue</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>AdvancedOptions</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>CopyOption</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>Destination</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>FileOption</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>Name</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>Platform</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>Processor</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFile</td><td>Source</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFileExt</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFileExt</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFileExt</td><td>ExtKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFileExt</td><td>Extension</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFileExt</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEFileExt</td><td>IconIndex</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>CEAppName</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>CECabs</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>CEDesktopDir</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>CEIcoFile</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>CEIniFileKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>CEInstallKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>Component_</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEInstall</td><td>DeleteMedia</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEOtherAppCABs</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEOtherAppCABs</td><td>BuildSourcePath</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEOtherAppCABs</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERedist</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERedist</td><td>Name</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERedist</td><td>Platforms</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Key</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Name</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Overwrite</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Platform</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Processor</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>RegKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Root</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCERegistry</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCESetupFile</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCESetupFile</td><td>Name</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCESetupFile</td><td>Platform</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCESetupFile</td><td>Processor</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCESetupFile</td><td>SetupFileKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCESetupFile</td><td>Source</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>AppKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>Destination</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>DisplayName</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>Platform</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>ShtCutKey</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>StartScreenIcon</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISCEShtCut</td><td>Target</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISChainPackage</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Display name for the chained package. Used only in the IDE.</td></row>
		<row><td>ISChainPackage</td><td>ISReleaseFlags</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISChainPackage</td><td>InstallCondition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>InstallProperties</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>Options</td><td>N</td><td/><td/><td/><td/><td>Integer</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>Order</td><td>N</td><td/><td/><td/><td/><td>Integer</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>Package</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>ProductCode</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISChainPackage</td><td>RemoveCondition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>RemoveProperties</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td/></row>
		<row><td>ISChainPackage</td><td>SourcePath</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISChainPackageData</td><td>Data</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>Binary stream. The binary icon data in PE (.DLL or .EXE) or icon (.ICO) format.</td></row>
		<row><td>ISChainPackageData</td><td>File</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td/></row>
		<row><td>ISChainPackageData</td><td>FilePath</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td/></row>
		<row><td>ISChainPackageData</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path to the ICO or EXE file.</td></row>
		<row><td>ISChainPackageData</td><td>Options</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISChainPackageData</td><td>Package_</td><td>N</td><td/><td/><td>ISChainPackage</td><td>1</td><td>Identifier</td><td/><td/></row>
		<row><td>ISClrWrap</td><td>Action_</td><td>N</td><td/><td/><td>CustomAction</td><td>1</td><td>Identifier</td><td/><td>Foreign key into CustomAction table</td></row>
		<row><td>ISClrWrap</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Property associated with this Action</td></row>
		<row><td>ISClrWrap</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Value associated with this Property</td></row>
		<row><td>ISComCatalogAttribute</td><td>ISComCatalogObject_</td><td>N</td><td/><td/><td>ISComCatalogObject</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComCatalogObject table.</td></row>
		<row><td>ISComCatalogAttribute</td><td>ItemName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The named attribute for a catalog object.</td></row>
		<row><td>ISComCatalogAttribute</td><td>ItemValue</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>A value associated with the attribute defined in the ItemName column.</td></row>
		<row><td>ISComCatalogCollection</td><td>CollectionName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>A catalog collection name.</td></row>
		<row><td>ISComCatalogCollection</td><td>ISComCatalogCollection</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique key for the ISComCatalogCollection table.</td></row>
		<row><td>ISComCatalogCollection</td><td>ISComCatalogObject_</td><td>N</td><td/><td/><td>ISComCatalogObject</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComCatalogObject table.</td></row>
		<row><td>ISComCatalogCollectionObjects</td><td>ISComCatalogCollection_</td><td>N</td><td/><td/><td>ISComCatalogCollection</td><td>1</td><td>Identifier</td><td/><td>A unique key for the ISComCatalogCollection table.</td></row>
		<row><td>ISComCatalogCollectionObjects</td><td>ISComCatalogObject_</td><td>N</td><td/><td/><td>ISComCatalogObject</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComCatalogObject table.</td></row>
		<row><td>ISComCatalogObject</td><td>DisplayName</td><td>N</td><td/><td/><td/><td/><td/><td/><td>The display name of a catalog object.</td></row>
		<row><td>ISComCatalogObject</td><td>ISComCatalogObject</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique key for the ISComCatalogObject table.</td></row>
		<row><td>ISComPlusApplication</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table that a COM+ application belongs to.</td></row>
		<row><td>ISComPlusApplication</td><td>ComputerName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Computer name that a COM+ application belongs to.</td></row>
		<row><td>ISComPlusApplication</td><td>DepFiles</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>List of the dependent files.</td></row>
		<row><td>ISComPlusApplication</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>InstallShield custom attributes associated with a COM+ application.</td></row>
		<row><td>ISComPlusApplication</td><td>ISComCatalogObject_</td><td>N</td><td/><td/><td>ISComCatalogObject</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComCatalogObject table.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>AlterDLL</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Alternate filename of the COM+ application component. Will be used for a .NET serviced component.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>CLSID</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>CLSID of the COM+ application component.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>DLL</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Filename of the COM+ application component.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>ISComCatalogObject_</td><td>N</td><td/><td/><td>ISComCatalogObject</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComCatalogObject table.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>ISComPlusApplicationDLL</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique key for the ISComPlusApplicationDLL table.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>ISComPlusApplication_</td><td>N</td><td/><td/><td>ISComPlusApplication</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComPlusApplication table.</td></row>
		<row><td>ISComPlusApplicationDLL</td><td>ProgId</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>ProgId of the COM+ application component.</td></row>
		<row><td>ISComPlusProxy</td><td>Component_</td><td>Y</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table that a COM+ application proxy belongs to.</td></row>
		<row><td>ISComPlusProxy</td><td>DepFiles</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>List of the dependent files.</td></row>
		<row><td>ISComPlusProxy</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>InstallShield custom attributes associated with a COM+ application proxy.</td></row>
		<row><td>ISComPlusProxy</td><td>ISComPlusApplication_</td><td>N</td><td/><td/><td>ISComPlusApplication</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComPlusApplication table that a COM+ application proxy belongs to.</td></row>
		<row><td>ISComPlusProxy</td><td>ISComPlusProxy</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique key for the ISComPlusProxy table.</td></row>
		<row><td>ISComPlusProxyDepFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table.</td></row>
		<row><td>ISComPlusProxyDepFile</td><td>ISComPlusApplication_</td><td>N</td><td/><td/><td>ISComPlusApplication</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComPlusApplication table.</td></row>
		<row><td>ISComPlusProxyDepFile</td><td>ISPath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path of the dependent file.</td></row>
		<row><td>ISComPlusProxyFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table.</td></row>
		<row><td>ISComPlusProxyFile</td><td>ISComPlusApplicationDLL_</td><td>N</td><td/><td/><td>ISComPlusApplicationDLL</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComPlusApplicationDLL table.</td></row>
		<row><td>ISComPlusServerDepFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table.</td></row>
		<row><td>ISComPlusServerDepFile</td><td>ISComPlusApplication_</td><td>N</td><td/><td/><td>ISComPlusApplication</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComPlusApplication table.</td></row>
		<row><td>ISComPlusServerDepFile</td><td>ISPath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path of the dependent file.</td></row>
		<row><td>ISComPlusServerFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table.</td></row>
		<row><td>ISComPlusServerFile</td><td>ISComPlusApplicationDLL_</td><td>N</td><td/><td/><td>ISComPlusApplicationDLL</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISComPlusApplicationDLL table.</td></row>
		<row><td>ISComponentExtended</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Primary key used to identify a particular component record.</td></row>
		<row><td>ISComponentExtended</td><td>FTPLocation</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>FTP Location</td></row>
		<row><td>ISComponentExtended</td><td>FilterProperty</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Property to set if you want to filter a component</td></row>
		<row><td>ISComponentExtended</td><td>HTTPLocation</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>HTTP Location</td></row>
		<row><td>ISComponentExtended</td><td>Language</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Language</td></row>
		<row><td>ISComponentExtended</td><td>Miscellaneous</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Miscellaneous</td></row>
		<row><td>ISComponentExtended</td><td>OS</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>bitwise addition of OSs</td></row>
		<row><td>ISComponentExtended</td><td>Platforms</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>bitwise addition of Platforms.</td></row>
		<row><td>ISCustomActionReference</td><td>Action_</td><td>N</td><td/><td/><td>CustomAction</td><td>1</td><td>Identifier</td><td/><td>Foreign key into theICustomAction table.</td></row>
		<row><td>ISCustomActionReference</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Contents of the file speciifed in ISCAReferenceFilePath. This column is only used by MSI.</td></row>
		<row><td>ISCustomActionReference</td><td>FileType</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>file type of the file specified  ISCAReferenceFilePath. This column is only used by MSI.</td></row>
		<row><td>ISCustomActionReference</td><td>ISCAReferenceFilePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path, the category is of Text instead of Path because of potential use of path variables.  This column only exists in ISM.</td></row>
		<row><td>ISDIMDependency</td><td>ISDIMReference_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>This is the primary key to the ISDIMDependency table</td></row>
		<row><td>ISDIMDependency</td><td>RequiredBuildVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>the build version identifying the required DIM</td></row>
		<row><td>ISDIMDependency</td><td>RequiredMajorVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>the major version identifying the required DIM</td></row>
		<row><td>ISDIMDependency</td><td>RequiredMinorVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>the minor version identifying the required DIM</td></row>
		<row><td>ISDIMDependency</td><td>RequiredRevisionVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>the revision version identifying the required DIM</td></row>
		<row><td>ISDIMDependency</td><td>RequiredUUID</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>the UUID identifying the required DIM</td></row>
		<row><td>ISDIMReference</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path, the category is of Text instead of Path because of potential use of path variables.</td></row>
		<row><td>ISDIMReference</td><td>ISDIMReference</td><td>N</td><td/><td/><td>ISDIMDependency</td><td>1</td><td>Identifier</td><td/><td>This is the primary key to the ISDIMReference table</td></row>
		<row><td>ISDIMReferenceDependencies</td><td>ISDIMDependency_</td><td>N</td><td/><td/><td>ISDIMDependency</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISDIMDependency table.</td></row>
		<row><td>ISDIMReferenceDependencies</td><td>ISDIMReference_Parent</td><td>N</td><td/><td/><td>ISDIMReference</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISDIMReference table.</td></row>
		<row><td>ISDIMVariable</td><td>ISDIMReference_</td><td>N</td><td/><td/><td>ISDIMReference</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISDIMReference table.</td></row>
		<row><td>ISDIMVariable</td><td>ISDIMVariable</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>This is the primary key to the ISDIMVariable table</td></row>
		<row><td>ISDIMVariable</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of a variable defined in the .dim file</td></row>
		<row><td>ISDIMVariable</td><td>NewValue</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>New value that you want to override with</td></row>
		<row><td>ISDIMVariable</td><td>Type</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Type of the variable. 0: Build Variable, 1: Runtime Variable</td></row>
		<row><td>ISDLLWrapper</td><td>EntryPoint</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>This is a foreign key to the target column in the CustomAction table</td></row>
		<row><td>ISDLLWrapper</td><td>Source</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>This is column points to the source file for the DLLWrapper Custom Action</td></row>
		<row><td>ISDLLWrapper</td><td>Target</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The function signature</td></row>
		<row><td>ISDLLWrapper</td><td>Type</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Type</td></row>
		<row><td>ISDRMFile</td><td>File_</td><td>Y</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into File table.  A null value will cause a build warning.</td></row>
		<row><td>ISDRMFile</td><td>ISDRMFile</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Unique identifier for this item.</td></row>
		<row><td>ISDRMFile</td><td>ISDRMLicense_</td><td>Y</td><td/><td/><td>ISDRMLicense</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing License that packages this file.</td></row>
		<row><td>ISDRMFile</td><td>Shell</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Text indicating the activation shell used at runtime.</td></row>
		<row><td>ISDRMFileAttribute</td><td>ISDRMFile_</td><td>N</td><td/><td/><td>ISDRMFile</td><td>1</td><td>Identifier</td><td/><td>Primary foreign key into ISDRMFile table.</td></row>
		<row><td>ISDRMFileAttribute</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the attribute</td></row>
		<row><td>ISDRMFileAttribute</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The value of the attribute</td></row>
		<row><td>ISDRMLicense</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td>Number</td><td/><td>Bitwise field used to specify binary attributes of this license.</td></row>
		<row><td>ISDRMLicense</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>An internal description of this license.</td></row>
		<row><td>ISDRMLicense</td><td>ISDRMLicense</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Unique key identifying the license record.</td></row>
		<row><td>ISDRMLicense</td><td>LicenseNumber</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The license number.</td></row>
		<row><td>ISDRMLicense</td><td>ProjectVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The version of the project that this license is tied to.</td></row>
		<row><td>ISDRMLicense</td><td>RequestCode</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The request code.</td></row>
		<row><td>ISDRMLicense</td><td>ResponseCode</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The response code.</td></row>
		<row><td>ISDependency</td><td>Exclude</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISDependency</td><td>ISDependency</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISDisk1File</td><td>Disk</td><td>Y</td><td/><td/><td/><td/><td/><td>-1;0;1</td><td>Used to differentiate between disk1(1), last disk(-1), and other(0).</td></row>
		<row><td>ISDisk1File</td><td>ISBuildSourcePath</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path of file to be copied to Disk1 folder</td></row>
		<row><td>ISDisk1File</td><td>ISDisk1File</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key for ISDisk1File table</td></row>
		<row><td>ISDynamicFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing Component that controls the file.</td></row>
		<row><td>ISDynamicFile</td><td>ExcludeFiles</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Wildcards for excluded files.</td></row>
		<row><td>ISDynamicFile</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td>0;1;2;3;4;5;6;7;8;9;10;11;12;13;14;15</td><td>This is used to store Installshield custom properties of a dynamic filet.  Currently the only one is SelfRegister.</td></row>
		<row><td>ISDynamicFile</td><td>IncludeFiles</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Wildcards for included files.</td></row>
		<row><td>ISDynamicFile</td><td>IncludeFlags</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Include flags.</td></row>
		<row><td>ISDynamicFile</td><td>SourceFolder</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path, the category is of Text instead of Path because of potential use of path variables.</td></row>
		<row><td>ISFeatureDIMReferences</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>ISFeatureDIMReferences</td><td>ISDIMReference_</td><td>N</td><td/><td/><td>ISDIMReference</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISDIMReference table.</td></row>
		<row><td>ISFeatureMergeModuleExcludes</td><td>Feature_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>ISFeatureMergeModuleExcludes</td><td>Language</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Foreign key into ISMergeModule table.</td></row>
		<row><td>ISFeatureMergeModuleExcludes</td><td>ModuleID</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into ISMergeModule table.</td></row>
		<row><td>ISFeatureMergeModules</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>ISFeatureMergeModules</td><td>ISMergeModule_</td><td>N</td><td/><td/><td>ISMergeModule</td><td>1</td><td>Text</td><td/><td>Foreign key into ISMergeModule table.</td></row>
		<row><td>ISFeatureMergeModules</td><td>Language_</td><td>N</td><td/><td/><td>ISMergeModule</td><td>2</td><td/><td/><td>Foreign key into ISMergeModule table.</td></row>
		<row><td>ISFeatureSetupPrerequisites</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>ISFeatureSetupPrerequisites</td><td>ISSetupPrerequisites_</td><td>N</td><td/><td/><td>ISSetupPrerequisites</td><td>1</td><td/><td/><td/></row>
		<row><td>ISFileManifests</td><td>File_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into File table.</td></row>
		<row><td>ISFileManifests</td><td>Manifest_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into File table.</td></row>
		<row><td>ISIISItem</td><td>Component_</td><td>Y</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key to Component table.</td></row>
		<row><td>ISIISItem</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Localizable Item Name.</td></row>
		<row><td>ISIISItem</td><td>ISIISItem</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key for each item.</td></row>
		<row><td>ISIISItem</td><td>ISIISItem_Parent</td><td>Y</td><td/><td/><td>ISIISItem</td><td>1</td><td>Identifier</td><td/><td>This record's parent record.</td></row>
		<row><td>ISIISItem</td><td>Type</td><td>N</td><td/><td/><td/><td/><td/><td/><td>IIS resource type.</td></row>
		<row><td>ISIISProperty</td><td>FriendlyName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>IIS property name.</td></row>
		<row><td>ISIISProperty</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Flags.</td></row>
		<row><td>ISIISProperty</td><td>ISIISItem_</td><td>N</td><td/><td/><td>ISIISItem</td><td>1</td><td>Identifier</td><td/><td>Primary key for table, foreign key into ISIISItem.</td></row>
		<row><td>ISIISProperty</td><td>ISIISProperty</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key for table.</td></row>
		<row><td>ISIISProperty</td><td>MetaDataAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>IIS property attributes.</td></row>
		<row><td>ISIISProperty</td><td>MetaDataProp</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>IIS property ID.</td></row>
		<row><td>ISIISProperty</td><td>MetaDataType</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>IIS property data type.</td></row>
		<row><td>ISIISProperty</td><td>MetaDataUserType</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>IIS property user data type.</td></row>
		<row><td>ISIISProperty</td><td>MetaDataValue</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>IIS property value.</td></row>
		<row><td>ISIISProperty</td><td>Order</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Order sequencing.</td></row>
		<row><td>ISIISProperty</td><td>Schema</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>IIS7 schema information.</td></row>
		<row><td>ISInstallScriptAction</td><td>EntryPoint</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>This is a foreign key to the target column in the CustomAction table</td></row>
		<row><td>ISInstallScriptAction</td><td>Source</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>This is column points to the source file for the DLLWrapper Custom Action</td></row>
		<row><td>ISInstallScriptAction</td><td>Target</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The function signature</td></row>
		<row><td>ISInstallScriptAction</td><td>Type</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Type</td></row>
		<row><td>ISLanguage</td><td>ISLanguage</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>This is the language ID.</td></row>
		<row><td>ISLanguage</td><td>Included</td><td>Y</td><td/><td/><td/><td/><td/><td>0;1</td><td>Specify whether this language should be included.</td></row>
		<row><td>ISLinkerLibrary</td><td>ISLinkerLibrary</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Unique identifier for the link library.</td></row>
		<row><td>ISLinkerLibrary</td><td>Library</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path of the object library (.obl file).</td></row>
		<row><td>ISLinkerLibrary</td><td>Order</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Order of the Library</td></row>
		<row><td>ISLocalControl</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>A 32-bit word that specifies the attribute flags to be applied to this control.</td></row>
		<row><td>ISLocalControl</td><td>Binary_</td><td>Y</td><td/><td/><td>Binary</td><td>1</td><td>Identifier</td><td/><td>External key to the Binary table.</td></row>
		<row><td>ISLocalControl</td><td>Control_</td><td>N</td><td/><td/><td>Control</td><td>2</td><td>Identifier</td><td/><td>Name of the control. This name must be unique within a dialog, but can repeat on different dialogs.</td></row>
		<row><td>ISLocalControl</td><td>Dialog_</td><td>N</td><td/><td/><td>Dialog</td><td>1</td><td>Identifier</td><td/><td>External key to the Dialog table, name of the dialog.</td></row>
		<row><td>ISLocalControl</td><td>Height</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Height of the bounding rectangle of the control.</td></row>
		<row><td>ISLocalControl</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path to .rtf file for scrollable text control</td></row>
		<row><td>ISLocalControl</td><td>ISLanguage_</td><td>N</td><td/><td/><td>ISLanguage</td><td>1</td><td>Text</td><td/><td>This is a foreign key to the ISLanguage table.</td></row>
		<row><td>ISLocalControl</td><td>Width</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Width of the bounding rectangle of the control.</td></row>
		<row><td>ISLocalControl</td><td>X</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Horizontal coordinate of the upper left corner of the bounding rectangle of the control.</td></row>
		<row><td>ISLocalControl</td><td>Y</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Vertical coordinate of the upper left corner of the bounding rectangle of the control.</td></row>
		<row><td>ISLocalDialog</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>A 32-bit word that specifies the attribute flags to be applied to this dialog.</td></row>
		<row><td>ISLocalDialog</td><td>Dialog_</td><td>Y</td><td/><td/><td>Dialog</td><td>1</td><td>Identifier</td><td/><td>Name of the dialog.</td></row>
		<row><td>ISLocalDialog</td><td>Height</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Height of the bounding rectangle of the dialog.</td></row>
		<row><td>ISLocalDialog</td><td>ISLanguage_</td><td>Y</td><td/><td/><td>ISLanguage</td><td>1</td><td>Text</td><td/><td>This is a foreign key to the ISLanguage table.</td></row>
		<row><td>ISLocalDialog</td><td>TextStyle_</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign Key into TextStyle table, only used in Script Based Projects.</td></row>
		<row><td>ISLocalDialog</td><td>Width</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Width of the bounding rectangle of the dialog.</td></row>
		<row><td>ISLocalRadioButton</td><td>Height</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The height of the button.</td></row>
		<row><td>ISLocalRadioButton</td><td>ISLanguage_</td><td>N</td><td/><td/><td>ISLanguage</td><td>1</td><td>Text</td><td/><td>This is a foreign key to the ISLanguage table.</td></row>
		<row><td>ISLocalRadioButton</td><td>Order</td><td>N</td><td>1</td><td>32767</td><td>RadioButton</td><td>2</td><td/><td/><td>A positive integer used to determine the ordering of the items within one list..The integers do not have to be consecutive.</td></row>
		<row><td>ISLocalRadioButton</td><td>Property</td><td>N</td><td/><td/><td>RadioButton</td><td>1</td><td>Identifier</td><td/><td>A named property to be tied to this radio button. All the buttons tied to the same property become part of the same group.</td></row>
		<row><td>ISLocalRadioButton</td><td>Width</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The width of the button.</td></row>
		<row><td>ISLocalRadioButton</td><td>X</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The horizontal coordinate of the upper left corner of the bounding rectangle of the radio button.</td></row>
		<row><td>ISLocalRadioButton</td><td>Y</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The vertical coordinate of the upper left corner of the bounding rectangle of the radio button.</td></row>
		<row><td>ISLockPermissions</td><td>Attributes</td><td>Y</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Permissions attributes mask, 1==Deny access; 2==No inherit</td></row>
		<row><td>ISLockPermissions</td><td>Domain</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Domain name for user whose permissions are being set.</td></row>
		<row><td>ISLockPermissions</td><td>LockObject</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into CreateFolder, Registry, or File table</td></row>
		<row><td>ISLockPermissions</td><td>Permission</td><td>Y</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Permission Access mask.</td></row>
		<row><td>ISLockPermissions</td><td>Table</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td>CreateFolder;File;Registry</td><td>Reference to another table name</td></row>
		<row><td>ISLockPermissions</td><td>User</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>User for permissions to be set. This can be a property, hardcoded named, or SID string</td></row>
		<row><td>ISLogicalDisk</td><td>Cabinet</td><td>Y</td><td/><td/><td/><td/><td>Cabinet</td><td/><td>If some or all of the files stored on the media are compressed in a cabinet, the name of that cabinet.</td></row>
		<row><td>ISLogicalDisk</td><td>DiskId</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>Primary key, integer to determine sort order for table.</td></row>
		<row><td>ISLogicalDisk</td><td>DiskPrompt</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Disk name: the visible text actually printed on the disk.  This will be used to prompt the user when this disk needs to be inserted.</td></row>
		<row><td>ISLogicalDisk</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISLogicalDisk</td><td>ISRelease_</td><td>N</td><td/><td/><td>ISRelease</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISRelease table.</td></row>
		<row><td>ISLogicalDisk</td><td>LastSequence</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>File sequence number for the last file for this media.</td></row>
		<row><td>ISLogicalDisk</td><td>Source</td><td>Y</td><td/><td/><td/><td/><td>Property</td><td/><td>The property defining the location of the cabinet file.</td></row>
		<row><td>ISLogicalDisk</td><td>VolumeLabel</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The label attributed to the volume.</td></row>
		<row><td>ISLogicalDiskFeatures</td><td>Feature_</td><td>Y</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Feature Table,</td></row>
		<row><td>ISLogicalDiskFeatures</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store Installshield custom properties, like Compressed, etc.</td></row>
		<row><td>ISLogicalDiskFeatures</td><td>ISLogicalDisk_</td><td>N</td><td>1</td><td>32767</td><td>ISLogicalDisk</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the ISLogicalDisk table.</td></row>
		<row><td>ISLogicalDiskFeatures</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISLogicalDiskFeatures</td><td>ISRelease_</td><td>N</td><td/><td/><td>ISRelease</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISRelease table.</td></row>
		<row><td>ISLogicalDiskFeatures</td><td>Sequence</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>File sequence number for the file for this media.</td></row>
		<row><td>ISMergeModule</td><td>Destination</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Destination.</td></row>
		<row><td>ISMergeModule</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store Installshield custom properties of a merge module.</td></row>
		<row><td>ISMergeModule</td><td>ISMergeModule</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The GUID identifying the merge module.</td></row>
		<row><td>ISMergeModule</td><td>Language</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Default decimal language of module.</td></row>
		<row><td>ISMergeModule</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the merge module.</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Attributes (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>ContextData</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>ContextData  (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>DefaultValue</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>DefaultValue  (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Description (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>DisplayName (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>Format</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Format (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>HelpKeyword</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>HelpKeyword (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>HelpLocation</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>HelpLocation (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>ISMergeModule_</td><td>N</td><td/><td/><td>ISMergeModule</td><td>1</td><td>Text</td><td/><td>The module signature, a foreign key into the ISMergeModule table</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>Language_</td><td>N</td><td/><td/><td>ISMergeModule</td><td>2</td><td/><td/><td>Default decimal language of module.</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>ModuleConfiguration_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Identifier, foreign key into ModuleConfiguration table (ModuleConfiguration.Name)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>Type</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Type (from configurable merge module)</td></row>
		<row><td>ISMergeModuleCfgValues</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Value for this item.</td></row>
		<row><td>ISObject</td><td>Language</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>ISObject</td><td>ObjectName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>ISObjectProperty</td><td>IncludeInBuild</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Boolean, 0 for false non 0 for true</td></row>
		<row><td>ISObjectProperty</td><td>ObjectName</td><td>Y</td><td/><td/><td>ISObject</td><td>1</td><td>Text</td><td/><td/></row>
		<row><td>ISObjectProperty</td><td>Property</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>ISObjectProperty</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>ISPalmApp</td><td>Component</td><td>N</td><td/><td/><td>Component</td><td>1</td><td/><td/><td/></row>
		<row><td>ISPalmApp</td><td>PalmApp</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISPalmAppFile</td><td>Destination</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISPalmAppFile</td><td>FileKey</td><td>N</td><td/><td/><td>File</td><td>1</td><td/><td/><td/></row>
		<row><td>ISPalmAppFile</td><td>PalmApp</td><td>N</td><td/><td/><td>ISPalmApp</td><td>1</td><td/><td/><td/></row>
		<row><td>ISPatchConfigImage</td><td>PatchConfiguration_</td><td>Y</td><td/><td/><td>ISPatchConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key to the ISPatchConfigurationTable</td></row>
		<row><td>ISPatchConfigImage</td><td>UpgradedImage_</td><td>N</td><td/><td/><td>ISUpgradedImage</td><td>1</td><td>Text</td><td/><td>Foreign key to the ISUpgradedImageTable</td></row>
		<row><td>ISPatchConfiguration</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>PatchConfiguration attributes</td></row>
		<row><td>ISPatchConfiguration</td><td>CanPCDiffer</td><td>N</td><td/><td/><td/><td/><td/><td/><td>This is determine whether Product Codes may differ</td></row>
		<row><td>ISPatchConfiguration</td><td>CanPVDiffer</td><td>N</td><td/><td/><td/><td/><td/><td/><td>This is determine whether the Major Product Version may differ</td></row>
		<row><td>ISPatchConfiguration</td><td>EnablePatchCache</td><td>N</td><td/><td/><td/><td/><td/><td/><td>This is determine whether to Enable Patch cacheing</td></row>
		<row><td>ISPatchConfiguration</td><td>Flags</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Patching API Flags</td></row>
		<row><td>ISPatchConfiguration</td><td>IncludeWholeFiles</td><td>N</td><td/><td/><td/><td/><td/><td/><td>This is determine whether to build a binary level patch</td></row>
		<row><td>ISPatchConfiguration</td><td>LeaveDecompressed</td><td>N</td><td/><td/><td/><td/><td/><td/><td>This is determine whether to leave intermediate files devcompressed when finished</td></row>
		<row><td>ISPatchConfiguration</td><td>MinMsiVersion</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Minimum Required MSI Version</td></row>
		<row><td>ISPatchConfiguration</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the Patch Configuration</td></row>
		<row><td>ISPatchConfiguration</td><td>OptimizeForSize</td><td>N</td><td/><td/><td/><td/><td/><td/><td>This is determine whether to Optimize for large files</td></row>
		<row><td>ISPatchConfiguration</td><td>OutputPath</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Build Location</td></row>
		<row><td>ISPatchConfiguration</td><td>PatchCacheDir</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Directory to recieve the Patch Cache information</td></row>
		<row><td>ISPatchConfiguration</td><td>PatchGuid</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Unique Patch Identifier</td></row>
		<row><td>ISPatchConfiguration</td><td>PatchGuidsToReplace</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>List Of Patch Guids to unregister</td></row>
		<row><td>ISPatchConfiguration</td><td>TargetProductCodes</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>List Of target Product Codes</td></row>
		<row><td>ISPatchConfigurationProperty</td><td>ISPatchConfiguration_</td><td>Y</td><td/><td/><td>ISPatchConfiguration</td><td>1</td><td>Text</td><td/><td>Name of the Patch Configuration</td></row>
		<row><td>ISPatchConfigurationProperty</td><td>Property</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the Patch Configuration Property value</td></row>
		<row><td>ISPatchConfigurationProperty</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Value of the Patch Configuration Property</td></row>
		<row><td>ISPatchExternalFile</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Filekey</td></row>
		<row><td>ISPatchExternalFile</td><td>FilePath</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Filepath</td></row>
		<row><td>ISPatchExternalFile</td><td>ISUpgradedImage_</td><td>N</td><td/><td/><td>ISUpgradedImage</td><td>1</td><td>Text</td><td/><td>Foreign key to the isupgraded image table</td></row>
		<row><td>ISPatchExternalFile</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Uniqu name to identify this record.</td></row>
		<row><td>ISPatchWholeFile</td><td>Component</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Component containing file key</td></row>
		<row><td>ISPatchWholeFile</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Key of file to be included as whole</td></row>
		<row><td>ISPatchWholeFile</td><td>UpgradedImage</td><td>N</td><td/><td/><td>ISUpgradedImage</td><td>1</td><td>Text</td><td/><td>Foreign key to ISUpgradedImage Table</td></row>
		<row><td>ISPathVariable</td><td>ISPathVariable</td><td>N</td><td/><td/><td/><td/><td/><td/><td>The name of the path variable.</td></row>
		<row><td>ISPathVariable</td><td>TestValue</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The test value of the path variable.</td></row>
		<row><td>ISPathVariable</td><td>Type</td><td>N</td><td/><td/><td/><td/><td/><td>1;2;4;8</td><td>The type of the path variable.</td></row>
		<row><td>ISPathVariable</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The value of the path variable.</td></row>
		<row><td>ISProductConfiguration</td><td>GeneratePackageCode</td><td>Y</td><td/><td/><td/><td/><td>Number</td><td>0;1</td><td>Indicates whether or not to generate a package code.</td></row>
		<row><td>ISProductConfiguration</td><td>ISProductConfiguration</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the product configuration.</td></row>
		<row><td>ISProductConfiguration</td><td>ProductConfigurationFlags</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Product configuration (release) flags.</td></row>
		<row><td>ISProductConfigurationInstance</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISProductConfigurationInstance</td><td>InstanceId</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Identifies the instance number of this instance. This value is stored in the Property InstanceId.</td></row>
		<row><td>ISProductConfigurationInstance</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Product Congiuration property name</td></row>
		<row><td>ISProductConfigurationInstance</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>String value for property.</td></row>
		<row><td>ISProductConfigurationProperty</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISProductConfigurationProperty</td><td>Property</td><td>N</td><td/><td/><td>Property</td><td>1</td><td>Text</td><td/><td>Product Congiuration property name</td></row>
		<row><td>ISProductConfigurationProperty</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>String value for property. Never null or empty.</td></row>
		<row><td>ISRelease</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Bitfield holding boolean values for various release attributes.</td></row>
		<row><td>ISRelease</td><td>BuildLocation</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Build location.</td></row>
		<row><td>ISRelease</td><td>CDBrowser</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Demoshield browser location.</td></row>
		<row><td>ISRelease</td><td>DefaultLanguage</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Default language for setup.</td></row>
		<row><td>ISRelease</td><td>DigitalPVK</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Digital signing private key (.pvk) file.</td></row>
		<row><td>ISRelease</td><td>DigitalSPC</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Digital signing Software Publisher Certificate (.spc) file.</td></row>
		<row><td>ISRelease</td><td>DigitalURL</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Digital signing URL.</td></row>
		<row><td>ISRelease</td><td>DiskClusterSize</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Disk cluster size.</td></row>
		<row><td>ISRelease</td><td>DiskSize</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Disk size.</td></row>
		<row><td>ISRelease</td><td>DiskSizeUnit</td><td>N</td><td/><td/><td/><td/><td/><td>0;1;2</td><td>Disk size units (KB or MB).</td></row>
		<row><td>ISRelease</td><td>DiskSpanning</td><td>N</td><td/><td/><td/><td/><td/><td>0;1;2</td><td>Disk spanning (automatic, enforce size, etc.).</td></row>
		<row><td>ISRelease</td><td>DotNetBuildConfiguration</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Build Configuration for .NET solutions.</td></row>
		<row><td>ISRelease</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISRelease</td><td>ISRelease</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the release.</td></row>
		<row><td>ISRelease</td><td>ISSetupPrerequisiteLocation</td><td>Y</td><td/><td/><td/><td/><td/><td>0;1;2;3</td><td>Location the Setup Prerequisites will be placed in</td></row>
		<row><td>ISRelease</td><td>MediaLocation</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Media location on disk.</td></row>
		<row><td>ISRelease</td><td>MsiCommandLine</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Command line passed to the msi package from setup.exe</td></row>
		<row><td>ISRelease</td><td>MsiSourceType</td><td>N</td><td>-1</td><td>4</td><td/><td/><td/><td/><td>MSI media source type.</td></row>
		<row><td>ISRelease</td><td>PackageName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Package name.</td></row>
		<row><td>ISRelease</td><td>Password</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Password.</td></row>
		<row><td>ISRelease</td><td>Platforms</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Platforms supported (Intel, Alpha, etc.).</td></row>
		<row><td>ISRelease</td><td>ReleaseFlags</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Release flags.</td></row>
		<row><td>ISRelease</td><td>ReleaseType</td><td>N</td><td/><td/><td/><td/><td/><td>1;2;4</td><td>Release type (single, uncompressed, etc.).</td></row>
		<row><td>ISRelease</td><td>SupportedLanguagesData</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Languages supported (for component filtering).</td></row>
		<row><td>ISRelease</td><td>SupportedLanguagesUI</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>UI languages supported.</td></row>
		<row><td>ISRelease</td><td>SupportedOSs</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Indicate which operating systmes are supported.</td></row>
		<row><td>ISRelease</td><td>SynchMsi</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>MSI file to synchronize file keys and other data with (patch-like functionality).</td></row>
		<row><td>ISRelease</td><td>Type</td><td>N</td><td>0</td><td>6</td><td/><td/><td/><td/><td>Release type (CDROM, Network, etc.).</td></row>
		<row><td>ISRelease</td><td>URLLocation</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Media location via URL.</td></row>
		<row><td>ISRelease</td><td>VersionCopyright</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Version stamp information.</td></row>
		<row><td>ISReleaseASPublishInfo</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISReleaseASPublishInfo</td><td>ISRelease_</td><td>N</td><td/><td/><td>ISRelease</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISRelease table.</td></row>
		<row><td>ISReleaseASPublishInfo</td><td>Property</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>AS Repository property name</td></row>
		<row><td>ISReleaseASPublishInfo</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>AS Repository property value</td></row>
		<row><td>ISReleaseExtended</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Bitfield holding boolean values for various release attributes.</td></row>
		<row><td>ISReleaseExtended</td><td>CertPassword</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Digital certificate password</td></row>
		<row><td>ISReleaseExtended</td><td>DigitalCertificateDBaseNS</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Path to cerificate database for Netscape digital  signature</td></row>
		<row><td>ISReleaseExtended</td><td>DigitalCertificateIdNS</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Path to cerificate ID for Netscape digital  signature</td></row>
		<row><td>ISReleaseExtended</td><td>DigitalCertificatePasswordNS</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Password for Netscape digital  signature</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetBaseLanguage</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Base Languge of .NET Redist</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetFxCmdLine</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Command Line to pass to DotNetFx.exe</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetLangPackCmdLine</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Command Line to pass to LangPack.exe</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetLangaugePacks</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>.NET Redist language packs to include</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetRedistLocation</td><td>Y</td><td>0</td><td>3</td><td/><td/><td/><td/><td>Location of .NET framework Redist (Web, SetupExe, Source, None)</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetRedistURL</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>URL to .NET framework Redist</td></row>
		<row><td>ISReleaseExtended</td><td>DotNetVersion</td><td>Y</td><td>0</td><td>2</td><td/><td/><td/><td/><td>Version of .NET framework Redist (1.0, 1.1)</td></row>
		<row><td>ISReleaseExtended</td><td>EngineLocation</td><td>Y</td><td>0</td><td>2</td><td/><td/><td/><td/><td>Location of msi engine (Web, SetupExe...)</td></row>
		<row><td>ISReleaseExtended</td><td>ISEngineLocation</td><td>Y</td><td>0</td><td>2</td><td/><td/><td/><td/><td>Location of ISScript  engine (Web, SetupExe...)</td></row>
		<row><td>ISReleaseExtended</td><td>ISEngineURL</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>URL to InstallShield scripting engine</td></row>
		<row><td>ISReleaseExtended</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISReleaseExtended</td><td>ISRelease_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the release.</td></row>
		<row><td>ISReleaseExtended</td><td>JSharpCmdLine</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Command Line to pass to vjredist.exe</td></row>
		<row><td>ISReleaseExtended</td><td>JSharpRedistLocation</td><td>Y</td><td>0</td><td>3</td><td/><td/><td/><td/><td>Location of J# framework Redist (Web, SetupExe, Source, None)</td></row>
		<row><td>ISReleaseExtended</td><td>MsiEngineVersion</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Bitfield holding selected MSI engine versions included in this release</td></row>
		<row><td>ISReleaseExtended</td><td>OneClickCabName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>File name of generated cabfile</td></row>
		<row><td>ISReleaseExtended</td><td>OneClickHtmlName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>File name of generated html page</td></row>
		<row><td>ISReleaseExtended</td><td>OneClickTargetBrowser</td><td>Y</td><td>0</td><td>2</td><td/><td/><td/><td/><td>Target browser (IE, Netscape, both...)</td></row>
		<row><td>ISReleaseExtended</td><td>WebCabSize</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>Size of the cabfile</td></row>
		<row><td>ISReleaseExtended</td><td>WebLocalCachePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Directory to cache downloaded package</td></row>
		<row><td>ISReleaseExtended</td><td>WebType</td><td>Y</td><td>0</td><td>2</td><td/><td/><td/><td/><td>Type of web install (One Executable, Downloader...)</td></row>
		<row><td>ISReleaseExtended</td><td>WebURL</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>URL to .msi package</td></row>
		<row><td>ISReleaseExtended</td><td>Win9xMsiUrl</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>URL to Ansi MSI engine</td></row>
		<row><td>ISReleaseExtended</td><td>WinMsi30Url</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>URL to MSI 3.0 engine</td></row>
		<row><td>ISReleaseExtended</td><td>WinNTMsiUrl</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>URL to Unicode MSI engine</td></row>
		<row><td>ISReleaseProperty</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Foreign key into ISProductConfiguration table.</td></row>
		<row><td>ISReleaseProperty</td><td>ISRelease_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Foreign key into ISRelease table.</td></row>
		<row><td>ISReleaseProperty</td><td>Name</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property name</td></row>
		<row><td>ISReleaseProperty</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISReleasePublishInfo</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Repository item description</td></row>
		<row><td>ISReleasePublishInfo</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Repository item display name</td></row>
		<row><td>ISReleasePublishInfo</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Bitfield holding various attributes</td></row>
		<row><td>ISReleasePublishInfo</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td>ISProductConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key into the ISProductConfiguration table.</td></row>
		<row><td>ISReleasePublishInfo</td><td>ISRelease_</td><td>N</td><td/><td/><td>ISRelease</td><td>1</td><td>Text</td><td/><td>The name of the release.</td></row>
		<row><td>ISReleasePublishInfo</td><td>Publisher</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Repository item publisher</td></row>
		<row><td>ISReleasePublishInfo</td><td>Repository</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Repository which to  publish the built merge module</td></row>
		<row><td>ISSQLConnection</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>Authentication</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>BatchSeparator</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>CmdTimeout</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>Comments</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>Database</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>ISSQLConnection</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular ISSQLConnection record.</td></row>
		<row><td>ISSQLConnection</td><td>Order</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>Password</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>ScriptVersion_Column</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>ScriptVersion_Table</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>Server</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnection</td><td>UserName</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnectionDBServer</td><td>ISSQLConnectionDBServer</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular ISSQLConnectionDBServer record.</td></row>
		<row><td>ISSQLConnectionDBServer</td><td>ISSQLConnection_</td><td>N</td><td/><td/><td>ISSQLConnection</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLConnection table.</td></row>
		<row><td>ISSQLConnectionDBServer</td><td>ISSQLDBMetaData_</td><td>N</td><td/><td/><td>ISSQLDBMetaData</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLDBMetaData table.</td></row>
		<row><td>ISSQLConnectionDBServer</td><td>Order</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLConnectionScript</td><td>ISSQLConnection_</td><td>N</td><td/><td/><td>ISSQLConnection</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLConnection table.</td></row>
		<row><td>ISSQLConnectionScript</td><td>ISSQLScriptFile_</td><td>N</td><td/><td/><td>ISSQLScriptFile</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLScriptFile table.</td></row>
		<row><td>ISSQLConnectionScript</td><td>Order</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnAdditional</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnDatabase</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnDriver</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnNetLibrary</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnPassword</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnPort</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnServer</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnUserID</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoCxnWindowsSecurity</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>AdoDriverName</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>CreateDbCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>CreateTableCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>DsnODBCName</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>ISSQLDBMetaData</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular ISSQLDBMetaData record.</td></row>
		<row><td>ISSQLDBMetaData</td><td>InsertRecordCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>LocalInstanceNames</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>QueryDatabasesCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>ScriptVersion_Column</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>ScriptVersion_ColumnType</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>ScriptVersion_Table</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>SelectTableCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>SwitchDbCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>TestDatabaseCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>TestTableCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>TestTableCmd2</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>VersionBeginToken</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>VersionEndToken</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>VersionInfoCmd</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLDBMetaData</td><td>WinAuthentUserId</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLRequirement</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLRequirement</td><td>ISSQLConnectionDBServer_</td><td>Y</td><td/><td/><td>ISSQLConnectionDBServer</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLConnectionDBServer table.</td></row>
		<row><td>ISSQLRequirement</td><td>ISSQLConnection_</td><td>N</td><td/><td/><td>ISSQLConnection</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLConnection table.</td></row>
		<row><td>ISSQLRequirement</td><td>ISSQLRequirement</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular ISSQLRequirement record.</td></row>
		<row><td>ISSQLRequirement</td><td>MajorVersion</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLRequirement</td><td>ServicePackLevel</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptError</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptError</td><td>ErrHandling</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptError</td><td>ErrNumber</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptError</td><td>ISSQLScriptFile_</td><td>Y</td><td/><td/><td>ISSQLScriptFile</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLScriptFile table</td></row>
		<row><td>ISSQLScriptError</td><td>Message</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Custom end-user message. Reserved for future use.</td></row>
		<row><td>ISSQLScriptFile</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptFile</td><td>Comments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Comments</td></row>
		<row><td>ISSQLScriptFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing Component that controls the SQL script.</td></row>
		<row><td>ISSQLScriptFile</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>A conditional statement that will disable this script if the specified condition evaluates to the 'False' state. If a script is disabled, it will not be installed regardless of the 'Action' state associated with the component.</td></row>
		<row><td>ISSQLScriptFile</td><td>ErrorHandling</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptFile</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path, the category is of Text instead of Path because of potential use of path variables.</td></row>
		<row><td>ISSQLScriptFile</td><td>ISSQLScriptFile</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>This is the primary key to the ISSQLScriptFile table</td></row>
		<row><td>ISSQLScriptFile</td><td>InstallText</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Feedback end-user text at install</td></row>
		<row><td>ISSQLScriptFile</td><td>Scheduling</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptFile</td><td>UninstallText</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Feedback end-user text at Uninstall</td></row>
		<row><td>ISSQLScriptFile</td><td>Version</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Schema Version (####.#####.####)</td></row>
		<row><td>ISSQLScriptImport</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>Authentication</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>Database</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>ExcludeTables</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>ISSQLScriptFile_</td><td>N</td><td/><td/><td>ISSQLScriptFile</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLScriptFile table.</td></row>
		<row><td>ISSQLScriptImport</td><td>IncludeTables</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>Password</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>Server</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptImport</td><td>UserName</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptReplace</td><td>Attributes</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptReplace</td><td>ISSQLScriptFile_</td><td>N</td><td/><td/><td>ISSQLScriptFile</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSQLScriptFile table.</td></row>
		<row><td>ISSQLScriptReplace</td><td>ISSQLScriptReplace</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular ISSQLScriptReplace record.</td></row>
		<row><td>ISSQLScriptReplace</td><td>Replace</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSQLScriptReplace</td><td>Search</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISScriptFile</td><td>ISScriptFile</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>This is the full path of the script file. The path portion may be expressed in path variable form.</td></row>
		<row><td>ISSelfReg</td><td>CmdLine</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSelfReg</td><td>Cost</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSelfReg</td><td>FileKey</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key to the file table</td></row>
		<row><td>ISSelfReg</td><td>Order</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSetupFile</td><td>FileName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>This is the file name to use when streaming the file to the support files location</td></row>
		<row><td>ISSetupFile</td><td>ISSetupFile</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>This is the primary key to the ISSetupFile table</td></row>
		<row><td>ISSetupFile</td><td>Language</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Four digit language identifier.  0 for Language Neutral</td></row>
		<row><td>ISSetupFile</td><td>Path</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Link to the source file on the build machine</td></row>
		<row><td>ISSetupFile</td><td>Splash</td><td>Y</td><td/><td/><td/><td/><td>Short</td><td/><td>Boolean value indication whether his setup file entry belongs in the Splasc Screen section</td></row>
		<row><td>ISSetupFile</td><td>Stream</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>Binary stream. The bits to stream to the support location</td></row>
		<row><td>ISSetupPrerequisites</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSetupPrerequisites</td><td>ISReleaseFlags</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Release Flags that specify whether this prereq  will be included in a particular release.</td></row>
		<row><td>ISSetupPrerequisites</td><td>ISSetupLocation</td><td>Y</td><td/><td/><td/><td/><td/><td>0;1;2</td><td/></row>
		<row><td>ISSetupPrerequisites</td><td>ISSetupPrerequisites</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSetupPrerequisites</td><td>Order</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISSetupType</td><td>Comments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>User Comments.</td></row>
		<row><td>ISSetupType</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Longer descriptive text describing a visible feature item.</td></row>
		<row><td>ISSetupType</td><td>Display</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Numeric sort order, used to force a specific display ordering.</td></row>
		<row><td>ISSetupType</td><td>Display_Name</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>A string used to set the initial text contained within a control (if appropriate).</td></row>
		<row><td>ISSetupType</td><td>ISSetupType</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular feature record.</td></row>
		<row><td>ISSetupTypeFeatures</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>ISSetupTypeFeatures</td><td>ISSetupType_</td><td>N</td><td/><td/><td>ISSetupType</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISSetupType table.</td></row>
		<row><td>ISStorages</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Path to the file to stream into sub-storage</td></row>
		<row><td>ISStorages</td><td>Name</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Name of the sub-storage key</td></row>
		<row><td>ISString</td><td>Comment</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Comment</td></row>
		<row><td>ISString</td><td>Encoded</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Encoding for multi-byte strings.</td></row>
		<row><td>ISString</td><td>ISLanguage_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>This is a foreign key to the ISLanguage table.</td></row>
		<row><td>ISString</td><td>ISString</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>String id.</td></row>
		<row><td>ISString</td><td>TimeStamp</td><td>Y</td><td/><td/><td/><td/><td>Time/Date</td><td/><td>Time Stamp. MSI's Time/Date column type is just an int, with bits packed in a certain order.</td></row>
		<row><td>ISString</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>real string value.</td></row>
		<row><td>ISTargetImage</td><td>Flags</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>relative order of the target image</td></row>
		<row><td>ISTargetImage</td><td>IgnoreMissingFiles</td><td>N</td><td/><td/><td/><td/><td/><td/><td>If true, ignore missing source files when creating patch</td></row>
		<row><td>ISTargetImage</td><td>MsiPath</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Path to the target image</td></row>
		<row><td>ISTargetImage</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the TargetImage</td></row>
		<row><td>ISTargetImage</td><td>Order</td><td>N</td><td/><td/><td/><td/><td/><td/><td>relative order of the target image</td></row>
		<row><td>ISTargetImage</td><td>UpgradedImage_</td><td>N</td><td/><td/><td>ISUpgradedImage</td><td>1</td><td>Text</td><td/><td>foreign key to the upgraded Image table</td></row>
		<row><td>ISUpgradeMsiItem</td><td>ISAttributes</td><td>N</td><td/><td/><td/><td/><td/><td>0;1</td><td/></row>
		<row><td>ISUpgradeMsiItem</td><td>ISReleaseFlags</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>ISUpgradeMsiItem</td><td>ObjectSetupPath</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The path to the setup you want to upgrade.</td></row>
		<row><td>ISUpgradeMsiItem</td><td>UpgradeItem</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the Upgrade Item.</td></row>
		<row><td>ISUpgradedImage</td><td>Family</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the image family</td></row>
		<row><td>ISUpgradedImage</td><td>MsiPath</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Path to the upgraded image</td></row>
		<row><td>ISUpgradedImage</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the UpgradedImage</td></row>
		<row><td>ISVirtualDirectory</td><td>Directory_</td><td>N</td><td/><td/><td>Directory</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Directory table.</td></row>
		<row><td>ISVirtualDirectory</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Property name</td></row>
		<row><td>ISVirtualDirectory</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISVirtualFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into File  table.</td></row>
		<row><td>ISVirtualFile</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Property name</td></row>
		<row><td>ISVirtualFile</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISVirtualPackage</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Property name</td></row>
		<row><td>ISVirtualPackage</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISVirtualRegistry</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Property name</td></row>
		<row><td>ISVirtualRegistry</td><td>Registry_</td><td>N</td><td/><td/><td>Registry</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Registry table.</td></row>
		<row><td>ISVirtualRegistry</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISVirtualRelease</td><td>ISProductConfiguration_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Foreign key into ISProductConfiguration table.</td></row>
		<row><td>ISVirtualRelease</td><td>ISRelease_</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Foreign key into ISRelease table.</td></row>
		<row><td>ISVirtualRelease</td><td>Name</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property name</td></row>
		<row><td>ISVirtualRelease</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISVirtualShortcut</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Property name</td></row>
		<row><td>ISVirtualShortcut</td><td>Shortcut_</td><td>N</td><td/><td/><td>Shortcut</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Shortcut table.</td></row>
		<row><td>ISVirtualShortcut</td><td>Value</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Property value</td></row>
		<row><td>ISXmlElement</td><td>Content</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Element contents</td></row>
		<row><td>ISXmlElement</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td>Number</td><td/><td>Internal XML element attributes</td></row>
		<row><td>ISXmlElement</td><td>ISXmlElement</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized, internal token for Xml element</td></row>
		<row><td>ISXmlElement</td><td>ISXmlElement_Parent</td><td>Y</td><td/><td/><td>ISXmlElement</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISXMLElement table.</td></row>
		<row><td>ISXmlElement</td><td>ISXmlFile_</td><td>N</td><td/><td/><td>ISXmlFile</td><td>1</td><td>Identifier</td><td/><td>Foreign key into XmlFile table.</td></row>
		<row><td>ISXmlElement</td><td>XPath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>XPath fragment including any operators</td></row>
		<row><td>ISXmlElementAttrib</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td>Number</td><td/><td>Internal XML elementattib attributes</td></row>
		<row><td>ISXmlElementAttrib</td><td>ISXmlElementAttrib</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized, internal token for Xml element attribute</td></row>
		<row><td>ISXmlElementAttrib</td><td>ISXmlElement_</td><td>N</td><td/><td/><td>ISXmlElement</td><td>1</td><td>Identifier</td><td/><td>Foreign key into ISXMLElement table.</td></row>
		<row><td>ISXmlElementAttrib</td><td>Name</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Localized attribute name</td></row>
		<row><td>ISXmlElementAttrib</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Localized attribute value</td></row>
		<row><td>ISXmlFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Component table.</td></row>
		<row><td>ISXmlFile</td><td>Directory</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into Directory table.</td></row>
		<row><td>ISXmlFile</td><td>Encoding</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>XML File Encoding</td></row>
		<row><td>ISXmlFile</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Localized XML file name</td></row>
		<row><td>ISXmlFile</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td>Number</td><td/><td>Internal XML file attributes</td></row>
		<row><td>ISXmlFile</td><td>ISXmlFile</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized,internal token for Xml file</td></row>
		<row><td>ISXmlFile</td><td>SelectionNamespaces</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Selection namespaces</td></row>
		<row><td>ISXmlLocator</td><td>Attribute</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>The name of an attribute within the XML element.</td></row>
		<row><td>ISXmlLocator</td><td>Element</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>XPath query that will locate an element in an XML file.</td></row>
		<row><td>ISXmlLocator</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td>0;1;2</td><td/></row>
		<row><td>ISXmlLocator</td><td>Parent</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The parent file signature. It is also a foreign key in the Signature table.</td></row>
		<row><td>ISXmlLocator</td><td>Signature_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The Signature_ represents a unique file signature and is also the foreign key in the Signature,  RegLocator, IniLocator, ISXmlLocator, CompLocator and the DrLocator tables.</td></row>
		<row><td>Icon</td><td>Data</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>Binary stream. The binary icon data in PE (.DLL or .EXE) or icon (.ICO) format.</td></row>
		<row><td>Icon</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path to the ICO or EXE file.</td></row>
		<row><td>Icon</td><td>ISIconIndex</td><td>Y</td><td>-32767</td><td>32767</td><td/><td/><td/><td/><td>Optional icon index to be extracted.</td></row>
		<row><td>Icon</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key. Name of the icon file.</td></row>
		<row><td>IniFile</td><td>Action</td><td>N</td><td/><td/><td/><td/><td/><td>0;1;3</td><td>The type of modification to be made, one of iifEnum</td></row>
		<row><td>IniFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table referencing component that controls the installing of the .INI value.</td></row>
		<row><td>IniFile</td><td>DirProperty</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into the Directory table denoting the directory where the .INI file is.</td></row>
		<row><td>IniFile</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The .INI file name in which to write the information</td></row>
		<row><td>IniFile</td><td>IniFile</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>IniFile</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The .INI file key below Section.</td></row>
		<row><td>IniFile</td><td>Section</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The .INI file Section.</td></row>
		<row><td>IniFile</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value to be written.</td></row>
		<row><td>IniLocator</td><td>Field</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The field in the .INI line. If Field is null or 0 the entire line is read.</td></row>
		<row><td>IniLocator</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The .INI file name.</td></row>
		<row><td>IniLocator</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Key value (followed by an equals sign in INI file).</td></row>
		<row><td>IniLocator</td><td>Section</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Section name within in file (within square brackets in INI file).</td></row>
		<row><td>IniLocator</td><td>Signature_</td><td>N</td><td/><td/><td>Signature</td><td>1</td><td>Identifier</td><td/><td>The table key. The Signature_ represents a unique file signature and is also the foreign key in the Signature table.</td></row>
		<row><td>IniLocator</td><td>Type</td><td>Y</td><td>0</td><td>2</td><td/><td/><td/><td/><td>An integer value that determines if the .INI value read is a filename or a directory location or to be used as is w/o interpretation.</td></row>
		<row><td>InstallExecuteSequence</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to invoke, either in the engine or the handler DLL.</td></row>
		<row><td>InstallExecuteSequence</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Optional expression which skips the action if evaluates to expFalse.If the expression syntax is invalid, the engine will terminate, returning iesBadActionData.</td></row>
		<row><td>InstallExecuteSequence</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store MM Custom Action Types</td></row>
		<row><td>InstallExecuteSequence</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Sequence.</td></row>
		<row><td>InstallExecuteSequence</td><td>Sequence</td><td>Y</td><td>-4</td><td>32767</td><td/><td/><td/><td/><td>Number that determines the sort order in which the actions are to be executed.  Leave blank to suppress action.</td></row>
		<row><td>InstallShield</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of property, uppercase if settable by launcher or loader.</td></row>
		<row><td>InstallShield</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>String value for property.</td></row>
		<row><td>InstallUISequence</td><td>Action</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of action to invoke, either in the engine or the handler DLL.</td></row>
		<row><td>InstallUISequence</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td>Optional expression which skips the action if evaluates to expFalse.If the expression syntax is invalid, the engine will terminate, returning iesBadActionData.</td></row>
		<row><td>InstallUISequence</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store MM Custom Action Types</td></row>
		<row><td>InstallUISequence</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Sequence.</td></row>
		<row><td>InstallUISequence</td><td>Sequence</td><td>Y</td><td>-4</td><td>32767</td><td/><td/><td/><td/><td>Number that determines the sort order in which the actions are to be executed.  Leave blank to suppress action.</td></row>
		<row><td>IsolatedComponent</td><td>Component_Application</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Key to Component table item for application</td></row>
		<row><td>IsolatedComponent</td><td>Component_Shared</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Key to Component table item to be isolated</td></row>
		<row><td>LaunchCondition</td><td>Condition</td><td>N</td><td/><td/><td/><td/><td>Condition</td><td/><td>Expression which must evaluate to TRUE in order for install to commence.</td></row>
		<row><td>LaunchCondition</td><td>Description</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Localizable text to display when condition fails and install must abort.</td></row>
		<row><td>ListBox</td><td>Order</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>A positive integer used to determine the ordering of the items within one list..The integers do not have to be consecutive.</td></row>
		<row><td>ListBox</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A named property to be tied to this item. All the items tied to the same property become part of the same listbox.</td></row>
		<row><td>ListBox</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The visible text to be assigned to the item. Optional. If this entry or the entire column is missing, the text is the same as the value.</td></row>
		<row><td>ListBox</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value string associated with this item. Selecting the line will set the associated property to this value.</td></row>
		<row><td>ListView</td><td>Binary_</td><td>Y</td><td/><td/><td>Binary</td><td>1</td><td>Identifier</td><td/><td>The name of the icon to be displayed with the icon. The binary information is looked up from the Binary Table.</td></row>
		<row><td>ListView</td><td>Order</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>A positive integer used to determine the ordering of the items within one list..The integers do not have to be consecutive.</td></row>
		<row><td>ListView</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A named property to be tied to this item. All the items tied to the same property become part of the same listview.</td></row>
		<row><td>ListView</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The visible text to be assigned to the item. Optional. If this entry or the entire column is missing, the text is the same as the value.</td></row>
		<row><td>ListView</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The value string associated with this item. Selecting the line will set the associated property to this value.</td></row>
		<row><td>LockPermissions</td><td>Domain</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Domain name for user whose permissions are being set. (usually a property)</td></row>
		<row><td>LockPermissions</td><td>LockObject</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into Registry or File table</td></row>
		<row><td>LockPermissions</td><td>Permission</td><td>Y</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Permission Access mask.  Full Control = 268435456 (GENERIC_ALL = 0x10000000)</td></row>
		<row><td>LockPermissions</td><td>Table</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td>Directory;File;Registry</td><td>Reference to another table name</td></row>
		<row><td>LockPermissions</td><td>User</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>User for permissions to be set.  (usually a property)</td></row>
		<row><td>MIME</td><td>CLSID</td><td>Y</td><td/><td/><td>Class</td><td>1</td><td>Guid</td><td/><td>Optional associated CLSID.</td></row>
		<row><td>MIME</td><td>ContentType</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Primary key. Context identifier, typically "type/format".</td></row>
		<row><td>MIME</td><td>Extension_</td><td>N</td><td/><td/><td>Extension</td><td>1</td><td>Text</td><td/><td>Optional associated extension (without dot)</td></row>
		<row><td>Media</td><td>Cabinet</td><td>Y</td><td/><td/><td/><td/><td>Cabinet</td><td/><td>If some or all of the files stored on the media are compressed in a cabinet, the name of that cabinet.</td></row>
		<row><td>Media</td><td>DiskId</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>Primary key, integer to determine sort order for table.</td></row>
		<row><td>Media</td><td>DiskPrompt</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Disk name: the visible text actually printed on the disk.  This will be used to prompt the user when this disk needs to be inserted.</td></row>
		<row><td>Media</td><td>LastSequence</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>File sequence number for the last file for this media.</td></row>
		<row><td>Media</td><td>Source</td><td>Y</td><td/><td/><td/><td/><td>Property</td><td/><td>The property defining the location of the cabinet file.</td></row>
		<row><td>Media</td><td>VolumeLabel</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The label attributed to the volume.</td></row>
		<row><td>MoveFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>If this component is not "selected" for installation or removal, no action will be taken on the associated MoveFile entry</td></row>
		<row><td>MoveFile</td><td>DestFolder</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of a property whose value is assumed to resolve to the full path to the destination directory</td></row>
		<row><td>MoveFile</td><td>DestName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Name to be given to the original file after it is moved or copied.  If blank, the destination file will be given the same name as the source file</td></row>
		<row><td>MoveFile</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key that uniquely identifies a particular MoveFile record</td></row>
		<row><td>MoveFile</td><td>Options</td><td>N</td><td>0</td><td>1</td><td/><td/><td/><td/><td>Integer value specifying the MoveFile operating mode, one of imfoEnum</td></row>
		<row><td>MoveFile</td><td>SourceFolder</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of a property whose value is assumed to resolve to the full path to the source directory</td></row>
		<row><td>MoveFile</td><td>SourceName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the source file(s) to be moved or copied.  Can contain the '*' or '?' wildcards.</td></row>
		<row><td>MsiAssembly</td><td>Attributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Assembly attributes</td></row>
		<row><td>MsiAssembly</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Component table.</td></row>
		<row><td>MsiAssembly</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Feature table.</td></row>
		<row><td>MsiAssembly</td><td>File_Application</td><td>Y</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into File table, denoting the application context for private assemblies. Null for global assemblies.</td></row>
		<row><td>MsiAssembly</td><td>File_Manifest</td><td>Y</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table denoting the manifest file for the assembly.</td></row>
		<row><td>MsiAssemblyName</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into Component table.</td></row>
		<row><td>MsiAssemblyName</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name part of the name-value pairs for the assembly name.</td></row>
		<row><td>MsiAssemblyName</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The value part of the name-value pairs for the assembly name.</td></row>
		<row><td>MsiDigitalCertificate</td><td>CertData</td><td>N</td><td/><td/><td/><td/><td>Binary</td><td/><td>A certificate context blob for a signer certificate</td></row>
		<row><td>MsiDigitalCertificate</td><td>DigitalCertificate</td><td>N</td><td/><td/><td>MsiPackageCertificate</td><td>2</td><td>Identifier</td><td/><td>A unique identifier for the row</td></row>
		<row><td>MsiDigitalSignature</td><td>DigitalCertificate_</td><td>N</td><td/><td/><td>MsiDigitalCertificate</td><td>1</td><td>Identifier</td><td/><td>Foreign key to MsiDigitalCertificate table identifying the signer certificate</td></row>
		<row><td>MsiDigitalSignature</td><td>Hash</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>The encoded hash blob from the digital signature</td></row>
		<row><td>MsiDigitalSignature</td><td>SignObject</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Foreign key to Media table</td></row>
		<row><td>MsiDigitalSignature</td><td>Table</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Reference to another table name (only Media table is supported)</td></row>
		<row><td>MsiDriverPackages</td><td>Component</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Primary key used to identify a particular component record.</td></row>
		<row><td>MsiDriverPackages</td><td>Flags</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Driver package flags</td></row>
		<row><td>MsiDriverPackages</td><td>ReferenceComponents</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>MsiDriverPackages</td><td>Sequence</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>Installation sequence number</td></row>
		<row><td>MsiEmbeddedChainer</td><td>CommandLine</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td/></row>
		<row><td>MsiEmbeddedChainer</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Condition</td><td/><td/></row>
		<row><td>MsiEmbeddedChainer</td><td>MsiEmbeddedChainer</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td/></row>
		<row><td>MsiEmbeddedChainer</td><td>Source</td><td>N</td><td/><td/><td/><td/><td>CustomSource</td><td/><td/></row>
		<row><td>MsiEmbeddedChainer</td><td>Type</td><td>Y</td><td/><td/><td/><td/><td>Integer</td><td>2;18;50</td><td/></row>
		<row><td>MsiEmbeddedUI</td><td>Attributes</td><td>N</td><td>0</td><td>3</td><td/><td/><td>Integer</td><td/><td>Information about the data in the Data column.</td></row>
		<row><td>MsiEmbeddedUI</td><td>Data</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>This column contains binary information.</td></row>
		<row><td>MsiEmbeddedUI</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Filename</td><td/><td>The name of the file that receives the binary information in the Data column.</td></row>
		<row><td>MsiEmbeddedUI</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>MsiEmbeddedUI</td><td>MessageFilter</td><td>Y</td><td>0</td><td>234913791</td><td/><td/><td>Integer</td><td/><td>Specifies the types of messages that are sent to the user interface DLL. This column is only relevant for rows with the msidbEmbeddedUI attribute.</td></row>
		<row><td>MsiEmbeddedUI</td><td>MsiEmbeddedUI</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The primary key for the table.</td></row>
		<row><td>MsiFileHash</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Primary key, foreign key into File table referencing file with this hash</td></row>
		<row><td>MsiFileHash</td><td>HashPart1</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Size of file in bytes (long integer).</td></row>
		<row><td>MsiFileHash</td><td>HashPart2</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Size of file in bytes (long integer).</td></row>
		<row><td>MsiFileHash</td><td>HashPart3</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Size of file in bytes (long integer).</td></row>
		<row><td>MsiFileHash</td><td>HashPart4</td><td>N</td><td/><td/><td/><td/><td/><td/><td>Size of file in bytes (long integer).</td></row>
		<row><td>MsiFileHash</td><td>Options</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Various options and attributes for this hash.</td></row>
		<row><td>MsiLockPermissionsEx</td><td>Condition</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Expression which must evaluate to TRUE in order for this set of permissions to be applied</td></row>
		<row><td>MsiLockPermissionsEx</td><td>LockObject</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into Registry, File, CreateFolder, or ServiceInstall table</td></row>
		<row><td>MsiLockPermissionsEx</td><td>MsiLockPermissionsEx</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token</td></row>
		<row><td>MsiLockPermissionsEx</td><td>SDDLText</td><td>N</td><td/><td/><td/><td/><td>FormattedSDDLText</td><td/><td>String to indicate permissions to be applied to the LockObject</td></row>
		<row><td>MsiLockPermissionsEx</td><td>Table</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td>CreateFolder;File;Registry;ServiceInstall</td><td>Reference to another table name</td></row>
		<row><td>MsiPackageCertificate</td><td>DigitalCertificate_</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A foreign key to the digital certificate table</td></row>
		<row><td>MsiPackageCertificate</td><td>PackageCertificate</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique identifier for the row</td></row>
		<row><td>MsiPatchCertificate</td><td>DigitalCertificate_</td><td>N</td><td/><td/><td>MsiDigitalCertificate</td><td>1</td><td>Identifier</td><td/><td>A foreign key to the digital certificate table</td></row>
		<row><td>MsiPatchCertificate</td><td>PatchCertificate</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique identifier for the row</td></row>
		<row><td>MsiPatchMetadata</td><td>Company</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Optional company name</td></row>
		<row><td>MsiPatchMetadata</td><td>PatchConfiguration_</td><td>N</td><td/><td/><td>ISPatchConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key to the ISPatchConfiguration table</td></row>
		<row><td>MsiPatchMetadata</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the metadata</td></row>
		<row><td>MsiPatchMetadata</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Value of the metadata</td></row>
		<row><td>MsiPatchOldAssemblyFile</td><td>Assembly_</td><td>Y</td><td/><td/><td>MsiPatchOldAssemblyName</td><td>1</td><td/><td/><td/></row>
		<row><td>MsiPatchOldAssemblyFile</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td/><td/><td/></row>
		<row><td>MsiPatchOldAssemblyName</td><td>Assembly</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>MsiPatchOldAssemblyName</td><td>Name</td><td>N</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>MsiPatchOldAssemblyName</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td/><td/><td/></row>
		<row><td>MsiPatchSequence</td><td>PatchConfiguration_</td><td>N</td><td/><td/><td>ISPatchConfiguration</td><td>1</td><td>Text</td><td/><td>Foreign key to the patch configuration table</td></row>
		<row><td>MsiPatchSequence</td><td>PatchFamily</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the family to which this patch belongs</td></row>
		<row><td>MsiPatchSequence</td><td>Sequence</td><td>N</td><td/><td/><td/><td/><td>Version</td><td/><td>The version of this patch in this family</td></row>
		<row><td>MsiPatchSequence</td><td>Supersede</td><td>N</td><td/><td/><td/><td/><td>Integer</td><td/><td>Supersede</td></row>
		<row><td>MsiPatchSequence</td><td>Target</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Target product codes for this patch family</td></row>
		<row><td>MsiServiceConfig</td><td>Argument</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Argument(s) for service configuration. Value depends on the content of the ConfigType field</td></row>
		<row><td>MsiServiceConfig</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table that controls the configuration of the service</td></row>
		<row><td>MsiServiceConfig</td><td>ConfigType</td><td>N</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Service Configuration Option</td></row>
		<row><td>MsiServiceConfig</td><td>Event</td><td>N</td><td>0</td><td>7</td><td/><td/><td/><td/><td>Bit field:   0x1 = Install, 0x2 = Uninstall, 0x4 = Reinstall</td></row>
		<row><td>MsiServiceConfig</td><td>MsiServiceConfig</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>MsiServiceConfig</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Name of a service. /, \, comma and space are invalid</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>Actions</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>A list of integer actions separated by [~] delimiters: 0 = SC_ACTION_NONE, 1 = SC_ACTION_RESTART, 2 = SC_ACTION_REBOOT, 3 = SC_ACTION_RUN_COMMAND. Terminate with [~][~]</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>Command</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Command line of the process to CreateProcess function to execute</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table that controls the configuration of the service</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>DelayActions</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>A list of delays (time in milli-seconds), separated by [~] delmiters, to wait before taking the corresponding Action. Terminate with [~][~]</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>Event</td><td>N</td><td>0</td><td>7</td><td/><td/><td/><td/><td>Bit field:   0x1 = Install, 0x2 = Uninstall, 0x4 = Reinstall</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>MsiServiceConfigFailureActions</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Name of a service. /, \, comma and space are invalid</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>RebootMessage</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Message to be broadcast to server users before rebooting</td></row>
		<row><td>MsiServiceConfigFailureActions</td><td>ResetPeriod</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>Time in seconds after which to reset the failure count to zero. Leave blank if it should never be reset</td></row>
		<row><td>MsiShortcutProperty</td><td>MsiShortcutProperty</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token</td></row>
		<row><td>MsiShortcutProperty</td><td>PropVariantValue</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>String representation of the value in the property</td></row>
		<row><td>MsiShortcutProperty</td><td>PropertyKey</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Canonical string representation of the Property Key being set</td></row>
		<row><td>MsiShortcutProperty</td><td>Shortcut_</td><td>N</td><td/><td/><td>Shortcut</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Shortcut table</td></row>
		<row><td>ODBCAttribute</td><td>Attribute</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of ODBC driver attribute</td></row>
		<row><td>ODBCAttribute</td><td>Driver_</td><td>N</td><td/><td/><td>ODBCDriver</td><td>1</td><td>Identifier</td><td/><td>Reference to ODBC driver in ODBCDriver table</td></row>
		<row><td>ODBCAttribute</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Value for ODBC driver attribute</td></row>
		<row><td>ODBCDataSource</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Reference to associated component</td></row>
		<row><td>ODBCDataSource</td><td>DataSource</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized.internal token for data source</td></row>
		<row><td>ODBCDataSource</td><td>Description</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Text used as registered name for data source</td></row>
		<row><td>ODBCDataSource</td><td>DriverDescription</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Reference to driver description, may be existing driver</td></row>
		<row><td>ODBCDataSource</td><td>Registration</td><td>N</td><td>0</td><td>1</td><td/><td/><td/><td/><td>Registration option: 0=machine, 1=user, others t.b.d.</td></row>
		<row><td>ODBCDriver</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Reference to associated component</td></row>
		<row><td>ODBCDriver</td><td>Description</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Text used as registered name for driver, non-localized</td></row>
		<row><td>ODBCDriver</td><td>Driver</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized.internal token for driver</td></row>
		<row><td>ODBCDriver</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Reference to key driver file</td></row>
		<row><td>ODBCDriver</td><td>File_Setup</td><td>Y</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Optional reference to key driver setup DLL</td></row>
		<row><td>ODBCSourceAttribute</td><td>Attribute</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of ODBC data source attribute</td></row>
		<row><td>ODBCSourceAttribute</td><td>DataSource_</td><td>N</td><td/><td/><td>ODBCDataSource</td><td>1</td><td>Identifier</td><td/><td>Reference to ODBC data source in ODBCDataSource table</td></row>
		<row><td>ODBCSourceAttribute</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Value for ODBC data source attribute</td></row>
		<row><td>ODBCTranslator</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Reference to associated component</td></row>
		<row><td>ODBCTranslator</td><td>Description</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>Text used as registered name for translator</td></row>
		<row><td>ODBCTranslator</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Reference to key translator file</td></row>
		<row><td>ODBCTranslator</td><td>File_Setup</td><td>Y</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Optional reference to key translator setup DLL</td></row>
		<row><td>ODBCTranslator</td><td>Translator</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized.internal token for translator</td></row>
		<row><td>Patch</td><td>Attributes</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Integer containing bit flags representing patch attributes</td></row>
		<row><td>Patch</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Primary key, non-localized token, foreign key to File table, must match identifier in cabinet.</td></row>
		<row><td>Patch</td><td>Header</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>Binary stream. The patch header, used for patch validation.</td></row>
		<row><td>Patch</td><td>ISBuildSourcePath</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Full path to patch header.</td></row>
		<row><td>Patch</td><td>PatchSize</td><td>N</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>Size of patch in bytes (long integer).</td></row>
		<row><td>Patch</td><td>Sequence</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Primary key, sequence with respect to the media images; order must track cabinet order.</td></row>
		<row><td>Patch</td><td>StreamRef_</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>External key into the MsiPatchHeaders table specifying the row that contains the patch header stream.</td></row>
		<row><td>PatchPackage</td><td>Media_</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Foreign key to DiskId column of Media table. Indicates the disk containing the patch package.</td></row>
		<row><td>PatchPackage</td><td>PatchId</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td>A unique string GUID representing this patch.</td></row>
		<row><td>ProgId</td><td>Class_</td><td>Y</td><td/><td/><td>Class</td><td>1</td><td>Guid</td><td/><td>The CLSID of an OLE factory corresponding to the ProgId.</td></row>
		<row><td>ProgId</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Localized description for the Program identifier.</td></row>
		<row><td>ProgId</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store Installshield custom properties of a component, like ExtractIcon, etc.</td></row>
		<row><td>ProgId</td><td>IconIndex</td><td>Y</td><td>-32767</td><td>32767</td><td/><td/><td/><td/><td>Optional icon index.</td></row>
		<row><td>ProgId</td><td>Icon_</td><td>Y</td><td/><td/><td>Icon</td><td>1</td><td>Identifier</td><td/><td>Optional foreign key into the Icon Table, specifying the icon file associated with this ProgId. Will be written under the DefaultIcon key.</td></row>
		<row><td>ProgId</td><td>ProgId</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The Program Identifier. Primary key.</td></row>
		<row><td>ProgId</td><td>ProgId_Parent</td><td>Y</td><td/><td/><td>ProgId</td><td>1</td><td>Text</td><td/><td>The Parent Program Identifier. If specified, the ProgId column becomes a version independent prog id.</td></row>
		<row><td>Property</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>User Comments.</td></row>
		<row><td>Property</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of property, uppercase if settable by launcher or loader.</td></row>
		<row><td>Property</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>String value for property.</td></row>
		<row><td>PublishComponent</td><td>AppData</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>This is localisable Application specific data that can be associated with a Qualified Component.</td></row>
		<row><td>PublishComponent</td><td>ComponentId</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td>A string GUID that represents the component id that will be requested by the alien product.</td></row>
		<row><td>PublishComponent</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table.</td></row>
		<row><td>PublishComponent</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Feature table.</td></row>
		<row><td>PublishComponent</td><td>Qualifier</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>This is defined only when the ComponentId column is an Qualified Component Id. This is the Qualifier for ProvideComponentIndirect.</td></row>
		<row><td>RadioButton</td><td>Height</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The height of the button.</td></row>
		<row><td>RadioButton</td><td>Help</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The help strings used with the button. The text is optional.</td></row>
		<row><td>RadioButton</td><td>ISControlId</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>A number used to represent the control ID of the Control, Used in Dialog export</td></row>
		<row><td>RadioButton</td><td>Order</td><td>N</td><td>1</td><td>32767</td><td/><td/><td/><td/><td>A positive integer used to determine the ordering of the items within one list..The integers do not have to be consecutive.</td></row>
		<row><td>RadioButton</td><td>Property</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A named property to be tied to this radio button. All the buttons tied to the same property become part of the same group.</td></row>
		<row><td>RadioButton</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The visible title to be assigned to the radio button.</td></row>
		<row><td>RadioButton</td><td>Value</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value string associated with this button. Selecting the button will set the associated property to this value.</td></row>
		<row><td>RadioButton</td><td>Width</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The width of the button.</td></row>
		<row><td>RadioButton</td><td>X</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The horizontal coordinate of the upper left corner of the bounding rectangle of the radio button.</td></row>
		<row><td>RadioButton</td><td>Y</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The vertical coordinate of the upper left corner of the bounding rectangle of the radio button.</td></row>
		<row><td>RegLocator</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>RegPath</td><td/><td>The key for the registry value.</td></row>
		<row><td>RegLocator</td><td>Name</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The registry value name.</td></row>
		<row><td>RegLocator</td><td>Root</td><td>N</td><td>0</td><td>3</td><td/><td/><td/><td/><td>The predefined root key for the registry value, one of rrkEnum.</td></row>
		<row><td>RegLocator</td><td>Signature_</td><td>N</td><td/><td/><td>Signature</td><td>1</td><td>Identifier</td><td/><td>The table key. The Signature_ represents a unique file signature and is also the foreign key in the Signature table. If the type is 0, the registry values refers a directory, and _Signature is not a foreign key.</td></row>
		<row><td>RegLocator</td><td>Type</td><td>Y</td><td>0</td><td>18</td><td/><td/><td/><td/><td>An integer value that determines if the registry value is a filename or a directory location or to be used as is w/o interpretation.</td></row>
		<row><td>Registry</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table referencing component that controls the installing of the registry value.</td></row>
		<row><td>Registry</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store Installshield custom properties of a registry item.  Currently the only one is Automatic.</td></row>
		<row><td>Registry</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>RegPath</td><td/><td>The key for the registry value.</td></row>
		<row><td>Registry</td><td>Name</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The registry value name.</td></row>
		<row><td>Registry</td><td>Registry</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>Registry</td><td>Root</td><td>N</td><td>-1</td><td>3</td><td/><td/><td/><td/><td>The predefined root key for the registry value, one of rrkEnum.</td></row>
		<row><td>Registry</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The registry value.</td></row>
		<row><td>RemoveFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key referencing Component that controls the file to be removed.</td></row>
		<row><td>RemoveFile</td><td>DirProperty</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of a property whose value is assumed to resolve to the full pathname to the folder of the file to be removed.</td></row>
		<row><td>RemoveFile</td><td>FileKey</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key used to identify a particular file entry</td></row>
		<row><td>RemoveFile</td><td>FileName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Name of the file to be removed.</td></row>
		<row><td>RemoveFile</td><td>InstallMode</td><td>N</td><td/><td/><td/><td/><td/><td>1;2;3</td><td>Installation option, one of iimEnum.</td></row>
		<row><td>RemoveIniFile</td><td>Action</td><td>N</td><td/><td/><td/><td/><td/><td>2;4</td><td>The type of modification to be made, one of iifEnum.</td></row>
		<row><td>RemoveIniFile</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table referencing component that controls the deletion of the .INI value.</td></row>
		<row><td>RemoveIniFile</td><td>DirProperty</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Foreign key into the Directory table denoting the directory where the .INI file is.</td></row>
		<row><td>RemoveIniFile</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The .INI file name in which to delete the information</td></row>
		<row><td>RemoveIniFile</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The .INI file key below Section.</td></row>
		<row><td>RemoveIniFile</td><td>RemoveIniFile</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>RemoveIniFile</td><td>Section</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The .INI file Section.</td></row>
		<row><td>RemoveIniFile</td><td>Value</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The value to be deleted. The value is required when Action is iifIniRemoveTag</td></row>
		<row><td>RemoveRegistry</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table referencing component that controls the deletion of the registry value.</td></row>
		<row><td>RemoveRegistry</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>RegPath</td><td/><td>The key for the registry value.</td></row>
		<row><td>RemoveRegistry</td><td>Name</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The registry value name.</td></row>
		<row><td>RemoveRegistry</td><td>RemoveRegistry</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>RemoveRegistry</td><td>Root</td><td>N</td><td>-1</td><td>3</td><td/><td/><td/><td/><td>The predefined root key for the registry value, one of rrkEnum</td></row>
		<row><td>ReserveCost</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Reserve a specified amount of space if this component is to be installed.</td></row>
		<row><td>ReserveCost</td><td>ReserveFolder</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of a property whose value is assumed to resolve to the full path to the destination directory</td></row>
		<row><td>ReserveCost</td><td>ReserveKey</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key that uniquely identifies a particular ReserveCost record</td></row>
		<row><td>ReserveCost</td><td>ReserveLocal</td><td>N</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>Disk space to reserve if linked component is installed locally.</td></row>
		<row><td>ReserveCost</td><td>ReserveSource</td><td>N</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>Disk space to reserve if linked component is installed to run from the source location.</td></row>
		<row><td>SFPCatalog</td><td>Catalog</td><td>Y</td><td/><td/><td/><td/><td>Binary</td><td/><td>SFP Catalog</td></row>
		<row><td>SFPCatalog</td><td>Dependency</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Parent catalog - only used by SFP</td></row>
		<row><td>SFPCatalog</td><td>SFPCatalog</td><td>N</td><td/><td/><td/><td/><td>Filename</td><td/><td>File name for the catalog.</td></row>
		<row><td>SelfReg</td><td>Cost</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The cost of registering the module.</td></row>
		<row><td>SelfReg</td><td>File_</td><td>N</td><td/><td/><td>File</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table denoting the module that needs to be registered.</td></row>
		<row><td>ServiceControl</td><td>Arguments</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Arguments for the service.  Separate by [~].</td></row>
		<row><td>ServiceControl</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table that controls the startup of the service</td></row>
		<row><td>ServiceControl</td><td>Event</td><td>N</td><td>0</td><td>187</td><td/><td/><td/><td/><td>Bit field:  Install:  0x1 = Start, 0x2 = Stop, 0x8 = Delete, Uninstall: 0x10 = Start, 0x20 = Stop, 0x80 = Delete</td></row>
		<row><td>ServiceControl</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Name of a service. /, \, comma and space are invalid</td></row>
		<row><td>ServiceControl</td><td>ServiceControl</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>ServiceControl</td><td>Wait</td><td>Y</td><td>0</td><td>1</td><td/><td/><td/><td/><td>Boolean for whether to wait for the service to fully start</td></row>
		<row><td>ServiceInstall</td><td>Arguments</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Arguments to include in every start of the service, passed to WinMain</td></row>
		<row><td>ServiceInstall</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table that controls the startup of the service</td></row>
		<row><td>ServiceInstall</td><td>Dependencies</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Other services this depends on to start.  Separate by [~], and end with [~][~]</td></row>
		<row><td>ServiceInstall</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Description of service.</td></row>
		<row><td>ServiceInstall</td><td>DisplayName</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>External Name of the Service</td></row>
		<row><td>ServiceInstall</td><td>ErrorControl</td><td>N</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Severity of error if service fails to start</td></row>
		<row><td>ServiceInstall</td><td>LoadOrderGroup</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>LoadOrderGroup</td></row>
		<row><td>ServiceInstall</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Internal Name of the Service</td></row>
		<row><td>ServiceInstall</td><td>Password</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>password to run service with.  (with StartName)</td></row>
		<row><td>ServiceInstall</td><td>ServiceInstall</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>ServiceInstall</td><td>ServiceType</td><td>N</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Type of the service</td></row>
		<row><td>ServiceInstall</td><td>StartName</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>User or object name to run service as</td></row>
		<row><td>ServiceInstall</td><td>StartType</td><td>N</td><td>0</td><td>4</td><td/><td/><td/><td/><td>Type of the service</td></row>
		<row><td>Shortcut</td><td>Arguments</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The command-line arguments for the shortcut.</td></row>
		<row><td>Shortcut</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Component table denoting the component whose selection gates the the shortcut creation/deletion.</td></row>
		<row><td>Shortcut</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The description for the shortcut.</td></row>
		<row><td>Shortcut</td><td>DescriptionResourceDLL</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>This field contains a Formatted string value for the full path to the language neutral file that contains the MUI manifest.</td></row>
		<row><td>Shortcut</td><td>DescriptionResourceId</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The description name index for the shortcut.</td></row>
		<row><td>Shortcut</td><td>Directory_</td><td>N</td><td/><td/><td>Directory</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the Directory table denoting the directory where the shortcut file is created.</td></row>
		<row><td>Shortcut</td><td>DisplayResourceDLL</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>This field contains a Formatted string value for the full path to the language neutral file that contains the MUI manifest.</td></row>
		<row><td>Shortcut</td><td>DisplayResourceId</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The display name index for the shortcut.</td></row>
		<row><td>Shortcut</td><td>Hotkey</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The hotkey for the shortcut. It has the virtual-key code for the key in the low-order byte, and the modifier flags in the high-order byte.</td></row>
		<row><td>Shortcut</td><td>ISAttributes</td><td>Y</td><td/><td/><td/><td/><td/><td/><td>This is used to store Installshield custom properties of a shortcut.  Mainly used in pro project types.</td></row>
		<row><td>Shortcut</td><td>ISComments</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Author’s comments on this Shortcut.</td></row>
		<row><td>Shortcut</td><td>ISShortcutName</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>A non-unique name for the shortcut.  Mainly used by pro pro project types.</td></row>
		<row><td>Shortcut</td><td>IconIndex</td><td>Y</td><td>-32767</td><td>32767</td><td/><td/><td/><td/><td>The icon index for the shortcut.</td></row>
		<row><td>Shortcut</td><td>Icon_</td><td>Y</td><td/><td/><td>Icon</td><td>1</td><td>Identifier</td><td/><td>Foreign key into the File table denoting the external icon file for the shortcut.</td></row>
		<row><td>Shortcut</td><td>Name</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the shortcut to be created.</td></row>
		<row><td>Shortcut</td><td>Shortcut</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Primary key, non-localized token.</td></row>
		<row><td>Shortcut</td><td>ShowCmd</td><td>Y</td><td/><td/><td/><td/><td/><td>1;3;7</td><td>The show command for the application window.The following values may be used.</td></row>
		<row><td>Shortcut</td><td>Target</td><td>N</td><td/><td/><td/><td/><td>Shortcut</td><td/><td>The shortcut target. This is usually a property that is expanded to a file or a folder that the shortcut points to.</td></row>
		<row><td>Shortcut</td><td>WkDir</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of property defining location of working directory.</td></row>
		<row><td>Signature</td><td>FileName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The name of the file. This may contain a "short name|long name" pair.</td></row>
		<row><td>Signature</td><td>Languages</td><td>Y</td><td/><td/><td/><td/><td>Language</td><td/><td>The languages supported by the file.</td></row>
		<row><td>Signature</td><td>MaxDate</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The maximum creation date of the file.</td></row>
		<row><td>Signature</td><td>MaxSize</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The maximum size of the file.</td></row>
		<row><td>Signature</td><td>MaxVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The maximum version of the file.</td></row>
		<row><td>Signature</td><td>MinDate</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The minimum creation date of the file.</td></row>
		<row><td>Signature</td><td>MinSize</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The minimum size of the file.</td></row>
		<row><td>Signature</td><td>MinVersion</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The minimum version of the file.</td></row>
		<row><td>Signature</td><td>Signature</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>The table key. The Signature represents a unique file signature.</td></row>
		<row><td>TextStyle</td><td>Color</td><td>Y</td><td>0</td><td>16777215</td><td/><td/><td/><td/><td>A long integer indicating the color of the string in the RGB format (Red, Green, Blue each 0-255, RGB = R + 256*G + 256^2*B).</td></row>
		<row><td>TextStyle</td><td>FaceName</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>A string indicating the name of the font used. Required. The string must be at most 31 characters long.</td></row>
		<row><td>TextStyle</td><td>Size</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The size of the font used. This size is given in our units (1/12 of the system font height). Assuming that the system font is set to 12 point size, this is equivalent to the point size.</td></row>
		<row><td>TextStyle</td><td>StyleBits</td><td>Y</td><td>0</td><td>15</td><td/><td/><td/><td/><td>A combination of style bits.</td></row>
		<row><td>TextStyle</td><td>TextStyle</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of the style. The primary key of this table. This name is embedded in the texts to indicate a style change.</td></row>
		<row><td>TypeLib</td><td>Component_</td><td>N</td><td/><td/><td>Component</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Component Table, specifying the component for which to return a path when called through LocateComponent.</td></row>
		<row><td>TypeLib</td><td>Cost</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The cost associated with the registration of the typelib. This column is currently optional.</td></row>
		<row><td>TypeLib</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td/></row>
		<row><td>TypeLib</td><td>Directory_</td><td>Y</td><td/><td/><td>Directory</td><td>1</td><td>Identifier</td><td/><td>Optional. The foreign key into the Directory table denoting the path to the help file for the type library.</td></row>
		<row><td>TypeLib</td><td>Feature_</td><td>N</td><td/><td/><td>Feature</td><td>1</td><td>Identifier</td><td/><td>Required foreign key into the Feature Table, specifying the feature to validate or install in order for the type library to be operational.</td></row>
		<row><td>TypeLib</td><td>Language</td><td>N</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>The language of the library.</td></row>
		<row><td>TypeLib</td><td>LibID</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td>The GUID that represents the library.</td></row>
		<row><td>TypeLib</td><td>Version</td><td>Y</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The version of the library. The major version is in the upper 8 bits of the short integer. The minor version is in the lower 8 bits.</td></row>
		<row><td>UIText</td><td>Key</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>A unique key that identifies the particular string.</td></row>
		<row><td>UIText</td><td>Text</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The localized version of the string.</td></row>
		<row><td>Upgrade</td><td>ActionProperty</td><td>N</td><td/><td/><td/><td/><td>UpperCase</td><td/><td>The property to set when a product in this set is found.</td></row>
		<row><td>Upgrade</td><td>Attributes</td><td>N</td><td>0</td><td>2147483647</td><td/><td/><td/><td/><td>The attributes of this product set.</td></row>
		<row><td>Upgrade</td><td>ISDisplayName</td><td>Y</td><td/><td/><td>ISUpgradeMsiItem</td><td>1</td><td/><td/><td/></row>
		<row><td>Upgrade</td><td>Language</td><td>Y</td><td/><td/><td/><td/><td>Language</td><td/><td>A comma-separated list of languages for either products in this set or products not in this set.</td></row>
		<row><td>Upgrade</td><td>Remove</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The list of features to remove when uninstalling a product from this set.  The default is "ALL".</td></row>
		<row><td>Upgrade</td><td>UpgradeCode</td><td>N</td><td/><td/><td/><td/><td>Guid</td><td/><td>The UpgradeCode GUID belonging to the products in this set.</td></row>
		<row><td>Upgrade</td><td>VersionMax</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The maximum ProductVersion of the products in this set.  The set may or may not include products with this particular version.</td></row>
		<row><td>Upgrade</td><td>VersionMin</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>The minimum ProductVersion of the products in this set.  The set may or may not include products with this particular version.</td></row>
		<row><td>Verb</td><td>Argument</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>Optional value for the command arguments.</td></row>
		<row><td>Verb</td><td>Command</td><td>Y</td><td/><td/><td/><td/><td>Formatted</td><td/><td>The command text.</td></row>
		<row><td>Verb</td><td>Extension_</td><td>N</td><td/><td/><td>Extension</td><td>1</td><td>Text</td><td/><td>The extension associated with the table row.</td></row>
		<row><td>Verb</td><td>Sequence</td><td>Y</td><td>0</td><td>32767</td><td/><td/><td/><td/><td>Order within the verbs for a particular extension. Also used simply to specify the default verb.</td></row>
		<row><td>Verb</td><td>Verb</td><td>N</td><td/><td/><td/><td/><td>Text</td><td/><td>The verb for the command.</td></row>
		<row><td>_Validation</td><td>Category</td><td>Y</td><td/><td/><td/><td/><td/><td>"Text";"Formatted";"Template";"Condition";"Guid";"Path";"Version";"Language";"Identifier";"Binary";"UpperCase";"LowerCase";"Filename";"Paths";"AnyPath";"WildCardFilename";"RegPath";"KeyFormatted";"CustomSource";"Property";"Cabinet";"Shortcut";"URL";"DefaultDir"</td><td>String category</td></row>
		<row><td>_Validation</td><td>Column</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of column</td></row>
		<row><td>_Validation</td><td>Description</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Description of column</td></row>
		<row><td>_Validation</td><td>KeyColumn</td><td>Y</td><td>1</td><td>32</td><td/><td/><td/><td/><td>Column to which foreign key connects</td></row>
		<row><td>_Validation</td><td>KeyTable</td><td>Y</td><td/><td/><td/><td/><td>Identifier</td><td/><td>For foreign key, Name of table to which data must link</td></row>
		<row><td>_Validation</td><td>MaxValue</td><td>Y</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Maximum value allowed</td></row>
		<row><td>_Validation</td><td>MinValue</td><td>Y</td><td>-2147483647</td><td>2147483647</td><td/><td/><td/><td/><td>Minimum value allowed</td></row>
		<row><td>_Validation</td><td>Nullable</td><td>N</td><td/><td/><td/><td/><td/><td>Y;N;@</td><td>Whether the column is nullable</td></row>
		<row><td>_Validation</td><td>Set</td><td>Y</td><td/><td/><td/><td/><td>Text</td><td/><td>Set of values that are permitted</td></row>
		<row><td>_Validation</td><td>Table</td><td>N</td><td/><td/><td/><td/><td>Identifier</td><td/><td>Name of table</td></row>
	</table>
</msi>
