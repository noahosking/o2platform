// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Data; 
using System.Linq; 
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics;   
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;  
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;   
using NUnit.Framework; 
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;

//O2File:Test_TM_Config.cs  
//O2File:TM_Test_XmlDatabase.cs

//O2Ref:nunit.framework.dll     

 
namespace O2.SecurityInnovation.TeamMentor.WebClient.JavascriptProxy_XmlDatabase
{		 
	[TestFixture]
    public class Test_Libraries : TM_Test_XmlDatabase
    {     	
    	//static TM_Xml_Database_JavaScriptProxy tmXmlDatabase_JavascriptProxy {get;set;}    	
    	//static TM_Xml_Database tmXmlDatabase { get; set;}
    	//TM_WebServices tmWebServices { get; set; }
    	    	    	    	
    	public Guid SI_LIBRARY_GUID = "ea854894-8e16-46c8-9c61-737ef46d7e82".guid();
    	
     	static Test_Libraries()
     	{
     		TMConfig.BaseFolder = Test_TM.tmWebSiteFolder;    		     	
     	} 
     	
    	public Test_Libraries() 
    	{     		
    	//	if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift)
    	//		testsSetUp();									
		//	tmWebServices = new TM_WebServices();
    	}
    	
/*    	public static void testsSetUp()
		{			
    		ActivityDB.DontLog = true;     		    		    		
			TM_Xml_Database.setDataFromCurrentScript("..\\..");
			tmXmlDatabase_JavascriptProxy = UnityInjection.useEnvironment_XmlDatabase();
			tmXmlDatabase = tmXmlDatabase_JavascriptProxy.tmXmlDatabase;
			
		}
*/		
    	  
    	[Test]    
    	public void Test_XmlDatabase_Setup()
    	{ 
    		"in Test_XmlDatabase_Setup".info();
    		Assert.IsNotNull(tmWebServices.javascriptProxy,"JavascriptProxy");
    		var proxyType = tmWebServices.javascriptProxy.ProxyType;
    		Assert.IsNotNull(proxyType,"proxyType"); 
    		Assert.AreEqual(proxyType,"TM Xml Database", "proxyType value");
    		Assert.IsNotNull(TM_Xml_Database.GuidanceExplorers_XmlFormat, "tmXmlDatabase");
    		Assert.That(TM_Xml_Database.GuidanceExplorers_XmlFormat.Values.size() > 0 , "GuidanceExplorers_XmlFormat was empty");
    		//Assert.That(tmXmlDatabase.JavascriptProxy.GetLibraries().size() > 0 , "JavascriptProxy.GetLibraries() was empty");    		
    	}
    	 
    	[Test]  
    	public void IJavascriptProxy_XmlDb_GetLibraries_Test_SerializedData() 
    	{    
    		//moq_tmWebServices.details();
    		//show.info(new TM_Moq_Database());
    		var libraries = tmWebServices.javascriptProxy.GetLibraries(); 
    		Assert.That(libraries != null , "response was null");
    		Assert.That(libraries.size()> 0 , "no libraries returned");    		    		
    		//specific tests for the data current serialized to WebClient\3_0_WebSite\WebServices\MoqDatabase
    		Assert.That(libraries.size() > 1 , "there should be at least  one library");    		
    		var siLibrary = tmWebServices.GetLibraryById(SI_LIBRARY_GUID);
    		Assert.That(siLibrary.id.guid() == SI_LIBRARY_GUID , "the library 'id' value didn't match");
    		Assert.That(siLibrary.caption == "SI" , "the library 'caption' value didn't match");    		
    	}    	    	
    	    	 
    	[Test]     	
    	public void IJavascriptProxy_XmlDb_GetFolders() 
    	{     		
    		var siLibrary = tmWebServices.GetLibraryById(SI_LIBRARY_GUID);
    		var folders = tmWebServices.javascriptProxy.GetFolders(siLibrary.id.guid());    		
    		Assert.That(folders != null , "folders was null");
    		Assert.That(folders.size() > 0 , "no folders returned");
    		 
    		var folderId = "d78b6ba3-c3a3-4125-8f05-0a5fb10ab207";
    		var expectedName = "PCI DSS Code Review";    		 		
    		    		
    		var tmFolder = tmXmlDatabase.tmFolder(siLibrary.id.guid(),folderId.guid());
    		
    		Assert.That(tmFolder.notNull(),"could not find folder with Id: {0}".format(folderId));    		
    		Assert.AreEqual(expectedName, tmFolder.name,"expected Name didn't match");    		
    	}
 
		[Test]     	
    	public void IJavascriptProxy_XmlDb_GetGuidanceItemsInView() 
    	{
    		var siLibrary = tmWebServices.GetLibraryById(SI_LIBRARY_GUID); 
    		var folders = tmWebServices.javascriptProxy.GetFolders(siLibrary.id.guid());
			var expectedFolderId = "d78b6ba3-c3a3-4125-8f05-0a5fb10ab207".guid();    		
    		var expectedFolder =  (from folder in folders
								   where folder.folderId ==expectedFolderId
								   select folder).first();
			Assert.That(expectedFolder.notNull(), "couldn't find expected folder: {0}".format(expectedFolderId));    		    										   
			
			var expectedViewId = "48057cf0-ac88-482b-948c-03f37a1c94fc";			
			Assert.That(expectedFolder.views.guids().contains(expectedViewId.guid()), "Folders didn't contain expected view id");														
			
    		var guidanceItems = tmWebServices.javascriptProxy.GetGuidanceItemsInView(expectedViewId.guid());     		
    		Assert.That(guidanceItems != null , "guidanceItems was null");
    		Assert.That(folders.size() > 0 , "no guidanceItems returned");    	 	
    		var expectedId = "b3a939b6-732f-49d0-b204-0422dbfbdbaa";
    		var expectedTitle = "Input is Validated for Length, Range, Format, and Type";
    		var expectedTopic = "Security";
    		var guidanceItem = guidanceItems.tmGuidanceItem(expectedId.guid()); 
    		
			Assert.That(guidanceItem.Id.str() == expectedId,"expected Id didn't match: {0} it was '{1}'".format(expectedId, guidanceItems[0].Id));
    		Assert.That(guidanceItem.Title == expectedTitle,"expected Title didn't match: {0} it was '{1}'".format(expectedTitle, guidanceItems[0].Title));
    		Assert.That(guidanceItem.Topic == expectedTopic,"expected Topic didn't match: {0} it was '{1}'".format(expectedTopic, guidanceItems[0].Topic));    		
    		    		
    	}    	

		[Test]     	
    	public void IJavascriptProxy_XmlDb_GetGuidanceItemsInViews() 
    	{ 
    		var siLibrary = tmWebServices.GetLibraryById(SI_LIBRARY_GUID); 
    		var folders = tmWebServices.javascriptProxy.GetFolders(siLibrary.id.guid());
    		var viewIds = folders[0].views;
    		var guidanceItems = tmWebServices.javascriptProxy.GetGuidanceItemsInViews(viewIds.guids());    		    		
    		Assert.That(guidanceItems != null , "guidanceItems was null");
    		Assert.That(folders.size() > 0 , "no guidanceItems returned");    		    		
    	}    	
 
 		[Test]     	 
    	public void IJavascriptProxy_XmlDb_GetGuidanceItemHtml() 
    	{    
    		var siLibrary = tmWebServices.GetLibraryById(SI_LIBRARY_GUID); 
    		var folders = tmWebServices.javascriptProxy.GetFolders(siLibrary.id.guid());
    		var guidanceItems = tmWebServices.javascriptProxy.GetGuidanceItemsInView(folders[0].views.guids()[0]);    		
    		//show.info(guidanceItems);
    		var guidanceItem = guidanceItems[0];
    		var html = tmWebServices.javascriptProxy.GetGuidanceItemHtml(guidanceItem.Id);    		
    		Assert.That(html != null , "GuidanceItemHtml was null");
    		Assert.That(html.size() > 0 , "GuidanceItemHtml was empty");    					
 		}
 		
		[Test]     	
    	public void IJavascriptProxy_XmlDb_GetAllGuidanceItems() 
    	{   
    		var allGuidanceItems = tmWebServices.javascriptProxy.GetAllGuidanceItems(); 
    		Assert.That(allGuidanceItems != null , "allGuidanceItems was null");
    		Assert.That(allGuidanceItems.size()> 0 , "no allGuidanceItems returned");    		
    		"There where  {0} items returned".info(allGuidanceItems.size());    		
    	}
    	
    	[Test]     	 
    	public void IJavascriptProxy_XmlDb_GetGuidanceItemsInLibrary() 
    	{   
    		var libraries = tmWebServices.javascriptProxy.GetLibraries();    		    	
    		var guidanceItemsInLibrary = tmWebServices.javascriptProxy.GetGuidanceItemsInLibrary(SI_LIBRARY_GUID);
    		Assert.That(guidanceItemsInLibrary != null , "guidanceItemsInLibrary was null");
    		Assert.That(guidanceItemsInLibrary.size()> 0 , "no guidanceItemsInLibrary returned");    		
    		"There where  {0} items returned".info(guidanceItemsInLibrary.size());    		
    	}
    	
    	[Test] 
    	public void Create_Rename_Delete_Libraries()
    	{
    		var originalName = "createAndDelete";  
			var newName 	 = originalName + "_new";
			var libraryPath_originalName  = tmXmlDatabase.xmlDB_LibraryPath(originalName);
			var libraryPath_newName  	  = tmXmlDatabase.xmlDB_LibraryPath(newName);			 			
			
			Assert.IsFalse(libraryPath_originalName.fileExists() , "libraryPath_originalName should not exists");
			Assert.IsFalse(libraryPath_newName	  .fileExists()	, "libraryPath_newName should not exists");
			
			//Create Library
			var newLibrary = tmWebServices.CreateLibrary(new Library() { caption = originalName });
			Assert.IsNotNull(newLibrary, "newLibrary Created OK");
			Assert.IsTrue(libraryPath_originalName.fileExists() , "libraryPath_originalName should exist after creation");
			
			//Rename Library
			var renameResult = tmWebServices.RenameLibrary(newLibrary.libraryId, newName);  
			Assert.IsTrue(renameResult, "renameResult");
			
			Assert.IsFalse(libraryPath_originalName.fileExists() , "libraryPath_originalName should not exist after rename");
			Assert.IsTrue (libraryPath_newName	   .fileExists() , "libraryPath_newName should exist after rename");
			
			var library_by_Id 				= tmWebServices.GetLibraryById(newLibrary.libraryId);    
			var library_by_originalName 	= tmWebServices.GetLibraryByName(originalName);
			var library_by_newName    		= tmWebServices.GetLibraryByName(newName); 
			var library_by_caption    		= tmWebServices.GetLibraryByName(library_by_Id.caption);
			
			Assert.IsNotNull(library_by_Id			 , "library_by_Id");
			Assert.IsNull   (library_by_originalName , "library_by_originalName");
			Assert.IsNotNull(library_by_newName		 , "library_by_newName");
			Assert.IsNotNull(library_by_caption		 , "library_by_caption");
			
			Assert.IsTrue(tmWebServices.GetLibraries().names().contains(newName), "new name was not on tmWebServices.GetLibraries()") ;
			//Delete Library
			var deleteResult = tmWebServices.DeleteLibrary(newLibrary.libraryId);  
			Assert.IsTrue(deleteResult, "deleteResult");
			
			Assert.IsFalse(libraryPath_newName	   .fileExists() , "libraryPath_newName should not exist after delete");
			Assert.IsFalse(libraryPath_originalName.fileExists() , "libraryPath_originalName should not exist at the end");

    	}
    	
    	
    	[Test] 
    	public void Create_Delete_Libraries_with_a_GuidanceItem()
    	{
    	
    		var originalName = "temp_lib_createLibraryWithGuidanceItemAndDelete";    

			var libraryPath_originalName  = tmXmlDatabase.xmlDB_LibraryPath(originalName);    
			var libraryPath_GuidanceItemsFolder = tmXmlDatabase.xmlDB_LibraryPath_GuidanceItems(originalName); 
			//Files.deleteFile(libraryPath_originalName);  //temp while developing  
			//Files.deleteFolder(libraryPath_GuidanceItemsFolder,true);
			 
			Assert.IsFalse(libraryPath_originalName.fileExists() 		, "libraryPath_originalName should not exists");
			Assert.IsFalse(libraryPath_GuidanceItemsFolder.dirExists()  , "libraryPath_GuidanceItemsFolder should not exists");
			 
			//Create Library 
			var newLibrary = tmWebServices.CreateLibrary(new Library() { caption = originalName });
			Assert.IsTrue(libraryPath_originalName.fileExists() 		, "libraryPath_originalName should exist after creation");
			Assert.IsFalse(libraryPath_GuidanceItemsFolder.dirExists()  , "libraryPath_GuidanceItemsFolder should not exists after library creation");
		
			
			//Create GuidanceItem
			var newGuidanceItem = new GuidanceItem_V3() 
											{
												libraryId = newLibrary.libraryId
											};								
			var guidanceItem = tmWebServices.CreateGuidanceItem(newGuidanceItem);
			
			Assert.AreNotEqual(guidanceItem, Guid.Empty, "guidance Item was not created");
			Assert.IsTrue(libraryPath_GuidanceItemsFolder.dirExists()  , "libraryPath_GuidanceItemsFolder should exist after library creation");
			
		
			//Delete Library
			var deleteResult = tmWebServices.DeleteLibrary(newLibrary.libraryId);  
			Assert.IsTrue(deleteResult, "deleteResult");
			Assert.IsFalse(libraryPath_originalName.fileExists() , "libraryPath_originalName should not exist at the end");
			
			Assert.IsFalse(libraryPath_GuidanceItemsFolder.dirExists()  , "libraryPath_GuidanceItemsFolder should not exist after delete");
    	}
    } 
}    
	