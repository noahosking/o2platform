// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.ExtensionMethods;



namespace O2.XRules.Database.Utils
{	
	public static class Control_extensionMethods
	{
		public static T closeForm<T>(this T control)
			where T : Control
		{
			control.parentForm().close();
			return control;
		}
		
		public static T closeForm_InNSeconds<T>(this T control, int seconds)
			where T : Control
		{
			O2Thread.mtaThread(
				()=>{
						control.sleep(seconds*1000);
						control.closeForm();
					});
			return control;
		}
		
		// new one
		public static T resizeFormToControlSize<T>(this T control, Control controlToSync)
			where T : Control
		{
			if (controlToSync.notNull())
			{
				var parentForm = control.parentForm();
				if (parentForm.notNull())
				{
					var top = controlToSync.PointToScreen(System.Drawing.Point.Empty).Y;
					var left = controlToSync.PointToScreen(System.Drawing.Point.Empty).X;
					var width = controlToSync.Width;
					var height = controlToSync.Height;
					"Setting parentForm location to {0}x{1} : {2}x{3}".info(top, left, width, height);
					parentForm.Top = top;
					parentForm.Left = left;
					parentForm.Width = width;
					parentForm.Height = height;
				}
			}
			return control;
		}
		
		public static string saveImageFromClipboardToFile(this object _object)
		{
			var clipboardImagePath = _object.saveImageFromClipboard();
			if (clipboardImagePath.fileExists())
			{
				var fileToSave = O2Forms.askUserForFileToSave(PublicDI.config.O2TempDir,"*.jpg");
				if (fileToSave.valid())
				{
					Files.MoveFile(clipboardImagePath, fileToSave);
					"Clipboard Image saved to: {0}".info(fileToSave);
				}
			}
			return clipboardImagePath;
		}						
		
		//Label

		public static Label autoSize(this Label label, bool value)
		{
			label.invokeOnThread(
				()=>{						
						label.AutoSize = value;
					});
			return label;
		}
		
		public static Label text_Center(this Label label)			
		{			
			label.invokeOnThread(
				()=>{						
						label.autoSize(false);
						label.TextAlign = ContentAlignment.MiddleCenter;
					});
			return label;
		}				
		
		//LinkLabel
		
		public static List<LinkLabel> links(this Control control)
		{
			return control.controls<LinkLabel>(true);
		}
		
		public static LinkLabel link(this Control control, string text)
		{
			foreach(var link in control.links())
				if (link.get_Text() == text)
					return link;
			return null;
		}
		
		public static LinkLabel onClick(this LinkLabel link, Action callback)
		{
			link.invokeOnThread(	
				()=>{
						link.Click += (sender,e) => callback();
					});
			return link;
		}
		//Control (Font)			
				
		public static T size<T>(this T control, int value)
			where T : Control
		{
			return control.textSize(value);
		}
		
		public static T size<T>(this T control, string value)
			where T : Control
		{
			return control.textSize(value.toInt());
		}
		
		public static T font<T>(this T control, string fontFamily, string size)
			where T : Control
		{
			return control.font(fontFamily, size.toInt());
		}
		
		public static T font<T>(this T control, string fontFamily, int size)
			where T : Control
		{
			return control.font(new FontFamily(fontFamily), size);
		}
		
		public static T font<T>(this T control, FontFamily fontFamily, string size)
			where T : Control
		{
			return control.font(fontFamily, size.toInt());
		}
		
		public static T font<T>(this T control, FontFamily fontFamily, int size)
			where T : Control
		{
			if (control.isNull())
				return null;
			control.invokeOnThread(
				()=>{
						if (fontFamily.isNull())
							fontFamily = control.Font.FontFamily;
						control.Font = new Font(fontFamily, size);
					});
			return control;
		}
		
		public static T font<T>(this T control, string fontFamily)
			where T : Control
		{
			return control.fontFamily(fontFamily);
		}
		
		public static T fontFamily<T>(this T control, string fontFamily)
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font(new FontFamily(fontFamily), control.Font.Size));			
			return control;
		}
		
		public static T textSize<T>(this T control, int value)
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font(control.Font.FontFamily, value));			
			return control;
		}
		
		public static T font_bold<T>(this T control)		// just bold() conficts with WPF version
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font( control.Font, control.Font.Style | FontStyle.Bold ));
			return control;
		}
		
		public static T font_italic<T>(this T control)
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font( control.Font, control.Font.Style | FontStyle.Italic ));
			return control;
		}
		
		//CheckBox
		public static CheckBox append_CheckBox(this Control control, string text, Action<bool> action)
		{
			return control.append_Control<CheckBox>()
						  .set_Text(text)
						  .autoSize()
						  .onChecked(action);
		}
		public static CheckBox onClick(this CheckBox checkBox, Action<bool> action)
		{
			return checkBox.onChecked(action);
		}
		
		public static CheckBox onChecked(this CheckBox checkBox, Action<bool> action)
		{
			return checkBox.checkedChanged(action);
		}
		public static CheckBox checkedChanged(this CheckBox checkBox, Action<bool> action)
		{
			checkBox.invokeOnThread(
				()=> checkBox.CheckedChanged+= (sender,e) => {action(checkBox.value());});
			return checkBox;
		}
		//WebBrowser
		
		public static WebBrowser onNavigated(this WebBrowser webBrowser, Action<string> callback)
		{
			webBrowser.invokeOnThread(()=> webBrowser.Navigated+= (sender,e)=> callback(e.Url.str()));
			return webBrowser;													
		}
		
		public static WebBrowser add_NavigationBar(this WebBrowser webBrowser)
		{
			var navigationBar = webBrowser.insert_Above(20).add_TextBox("url","");
			webBrowser.onNavigated((url)=> navigationBar.set_Text(url));
			navigationBar.onEnter((text)=>webBrowser.open(text));
			return webBrowser;
		}
		//ListBox
		
		public static ListBox add_ListBox(this Control control)
		{
			return control.add_Control<ListBox>();
		}
		
		public static ListBox add_Item(this ListBox listBox, object item)
		{
			return listBox.add_Items(item);
		}
		
		public static ListBox add_Items(this ListBox listBox, params object[] items)
		{
			return (ListBox)listBox.invokeOnThread(
				()=>{
						listBox.Items.AddRange(items);
						return listBox;
					});					
		}
		
		public static object selectedItem(this ListBox listBox)
		{
			return (object)listBox.invokeOnThread(
				()=>{	
						return listBox.SelectedItem;	
					});
		}
		
		public static T selectedItem<T>(this ListBox listBox)
		{			
			var selectedItem = listBox.selectedItem();
			if (selectedItem is T) 
				return (T) selectedItem;
			return default(T);					
		}
		
		public static ListBox select(this ListBox listBox, int selectedIndex)
		{
			return (ListBox)listBox.invokeOnThread(
				()=>{
						if (listBox.Items.size() > selectedIndex)
							listBox.SelectedIndex = selectedIndex;
						return listBox;
					});					
		}
		
		public static ListBox selectFirst(this ListBox listBox)
		{
			return listBox.select(0);
		}
		
		
		//TabControl
		
		public static TabControl remove_Tab(this TabControl tabControl, string text)
		{
			var tabToRemove = tabControl.tab(text);
			if (tabToRemove.notNull())
				tabControl.remove_Tab(tabToRemove);
			return tabControl;
		}
		
		public static bool has_Tab(this TabControl tabControl, string text)
		{
			return tabControl.tab(text).notNull();
		}
		
		public static TabPage tab(this TabControl tabControl, string text)
		{
			foreach(var tab in tabControl.tabs())
				if (tab.get_Text() == text)
					return tab;
			return null;
		}
		public static List<TabPage> tabs(this TabControl tabControl)
		{
			return tabControl.tabPages();
		}
		
		public static List<TabPage> tabPages(this TabControl tabControl)
		{
			return (List<TabPage>)tabControl.invokeOnThread(
									()=>{
											var tabPages = new List<TabPage>();
											foreach(TabPage tabPage in tabControl.TabPages)
												tabPages.Add(tabPage);
											return tabPages;											
										});
		}
		
		
	}
	
		public static class WinFormControls_ExtensionMethods
	{
		public static List<Control> add_1x1(this Control control, Action<Control> buildPanel1,  Action<Control> buildPanel2)
		{
			var controls = control.add_1x1();
			buildPanel1(controls[0].add_Panel());
			buildPanel2(controls[1].add_Panel());
			return controls;
		}
		
		public static T insert_LogViewer<T>(this T control)
			where T : Control
		{
			control.insert_Below(100)
				   .add_LogViewer();
			return control;
		}
		// insert_...()
		public static Panel insert_Left(this Control control)
		{
			return control.insert_Left(control.width()/2);			
		}
		
		public static Panel insert_Right(this Control control)
		{
			return control.insert_Right<Panel>(control.width()/2);
		}
		
		public static Panel insert_Above(this Control control)
		{			
			return control.insert_Above<Panel>(control.height()/2);
		}
		
		public static Panel insert_Below(this Control control)
		{
			return control.insert_Below<Panel>(control.height()/2);
		}		
		// insert_...(width)
		public static Panel insert_Left(this Control control, int width)
		{			
			var panel = control.insert_Left<Panel>(width); 
			// to deal with bug in insert_Left<Panel>
			//replace with (when merging extension methods): panel.splitterDistance(width);
			{
				var splitContainer = control.parent<SplitContainer>();
				Ascx_ExtensionMethods.splitterDistance(splitContainer,width);
			}
			
			return panel;
		}
		
		public static Panel insert_Right(this Control control, int width)
		{
			return control.insert_Right<Panel>(width);
		}
		
		public static Panel insert_Above(this Control control, int width)
		{
			return control.insert_Above<Panel>(width);
		}
		
		public static Panel insert_Below(this Control control, int width)
		{
			return control.insert_Below<Panel>(width);
		}
		// insert_...(title)
		public static Panel insert_Left(this Control control, string title)
		{
			return control.insert_Left(control.width()/2, title);
		}
		
		public static Panel insert_Right(this Control control, string title)
		{
			return control.insert_Right(control.width()/2, title);
		}
		
		public static Panel insert_Above(this Control control, string title)
		{
			return control.insert_Above(control.height()/2, title);
		}
		
		public static Panel insert_Below(this Control control, string title)
		{
			return control.insert_Below(control.height()/2, title);
		}
		// insert_...(width, title)
		public static Panel insert_Left(this Control control, int width, string title)
		{
			return control.insert_Left<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static Panel insert_Right(this Control control, int width, string title)
		{
			return control.insert_Right<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static Panel insert_Above(this Control control, int width, string title)
		{
			return control.insert_Above<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static Panel insert_Below(this Control control, int width, string title)
		{
			return control.insert_Below<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static T white<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.White);
		}
		
		public static T pink<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.LightPink);
		}
		
		public static T azure<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.Azure);
		}					
	}

}    	