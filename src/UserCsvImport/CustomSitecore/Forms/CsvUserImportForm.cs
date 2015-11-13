﻿using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Extensions;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Pipelines;
using Sitecore.Publishing;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Shell;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using UserCsvImport.CustomItems.CsvImport;
using UserCsvImport.Membership;

namespace UserCsvImport.CustomSitecore.Forms
{
	public class CsvUserImportForm : WizardForm
	{
		/// <summary>
		/// The error text.
		/// 
		/// </summary>
		protected Memo ErrorText;
		
		protected Border NoChanges;


		protected Checkbox DownloadLatest;

		protected Checkbox IgnoreFileDate;
		
		protected Border ResultLabel;

		protected Memo ResultText;
		
		protected Scrollbox SettingsPane;
		
		protected Border ShowResultPane;

		protected Border DownloadingText;

		protected Border ImportingText;
		
		protected Literal Status;

		protected Literal Welcome;

		/// <summary> Gets or sets the item ID.  </summary> 
		/// <value> The item ID. </value>
		/// <contract><requires name="value" condition="not null"/><ensures condition="not null"/></contract>
		protected string ItemID
		{
			get
			{
				return StringUtil.GetString(this.ServerProperties["ItemID"]);
			}
			set
			{
				Assert.ArgumentNotNull((object)value, "value");
				this.ServerProperties["ItemID"] = (object)value;
			}
		}

		protected string Database
		{
			get
			{
				return StringUtil.GetString(this.ServerProperties["Database"]);
			}
			set
			{
				Assert.ArgumentNotNull((object)value, "value");
				this.ServerProperties["Database"] = (object)value;
			}
		}

		private UserCSVSheetItem _csvSheetItem;
		protected UserCSVSheetItem CsvSheetItem
		{
			get
			{
				if (CsvSheetItem == null)
				{
					_csvSheetItem = Factory.GetDatabase(Database).GetItem(ItemID);
				}
				return _csvSheetItem;
			}
		}

		/// <summary>
		/// Gets or sets the handle. 
		/// </summary> 
		/// <value> The handle. </value>
		/// <contract><requires name="value" condition="not empty"/><ensures condition="not null"/></contract>
		protected string JobHandle
		{
			get
			{
				return StringUtil.GetString(this.ServerProperties["JobHandle"]);
			}
			set
			{
				Assert.ArgumentNotNullOrEmpty(value, "value");
				this.ServerProperties["JobHandle"] = (object)value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Sitecore.Shell.Applications.Dialogs.Publish.PublishForm"/> is rebuild.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// <c>true</c> if rebuild; otherwise, <c>false</c>.
		/// </value>
		protected bool Rebuild
		{
			get
			{
				return WebUtil.GetQueryString("mo") == "rebuild";
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user has confirmed selected options.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// <c>true</c> if user has confirmed; otherwise, <c>false</c>.
		/// 
		/// </value>
		protected bool ConfirmedOptions
		{
			get
			{
				return (bool)Context.ClientPage.ServerProperties["ConfirmedOptions"];
			}
			set
			{
				Context.ClientPage.ServerProperties["ConfirmedOptions"] = (object)(value ? 1 : 0);
			}
		}

		/// <summary>
		/// Checks the status.
		/// 
		/// </summary>
		/// <exception cref="T:System.Exception"><c>Exception</c>.
		///             </exception>
		public void CheckStatus()
		{
			Handle handle = Handle.Parse(this.JobHandle);
			if (!handle.IsLocal)
			{
				SheerResponse.Timer("CheckStatus", 500);
			}
			else
			{
				UserImportStatus status = UserImportStatus.GetStatus(handle);
				if (status == null)
					throw new Exception("The publishing process was unexpectedly interrupted.");
				if (!string.IsNullOrEmpty(status.ErrorMessage))
				{
					this.Active = "Retry";
					this.NextButton.Disabled = true;
					this.BackButton.Disabled = false;
					this.CancelButton.Disabled = false;
					this.ErrorText.Value = status.ErrorMessage + "\nPlease check the logs.";
				}
				else
				{
					string str1 = "Importing...";
					if (status.State == JobState.Running)
						str1 = string.Format("Processing user {0} of {1}.", status.UsersProcessed, status.TotalUserCount);
					else if (status.State == JobState.Initializing)
						str1 = "Initializing.";
					else if (status.State == JobState.Finished)
					{
						this.Status.Text = Translate.Text("Items processed: {0}.", status.UsersProcessed);
						this.Active = "LastPage";
						this.BackButton.Disabled = true;
						// Don't invoke the status page again if finished.
						return;
					}
					
					SheerResponse.SetInnerHtml("ImportingText", str1);
					SheerResponse.Timer("CheckStatus", 500);
					
				}
			}
		}

		protected override void ActivePageChanged(string page, string oldPage)
		{
			Assert.ArgumentNotNull((object)page, "page");
			Assert.ArgumentNotNull((object)oldPage, "oldPage");
			this.NextButton.Header = Translate.Text("Next >");
			if (page == "Settings")
				this.NextButton.Header = "Import >";
			base.ActivePageChanged(page, oldPage);
			if (page != "Importing")
				return;
			this.NextButton.Disabled = true;
			this.BackButton.Disabled = true;
			this.CancelButton.Disabled = true;
			SheerResponse.SetInnerHtml("ImportingText", "Importing users...");
			SheerResponse.Timer("StartImporter", 10);
		}

		/// <summary>
		/// Called when the active page is changing.
		/// 
		/// </summary>
		/// <param name="page">The page that is being left.
		///             </param><param name="newpage">The new page that is being entered.
		///             </param>
		/// <returns>
		/// True, if the change is allowed, otherwise false.
		/// 
		/// </returns>
		/// 
		/// <remarks>
		/// Set the newpage parameter to another page ID to control the
		///             path through the wizard pages.
		/// 
		/// </remarks>
		protected override bool ActivePageChanging(string page, ref string newpage)
		{
			Assert.ArgumentNotNull((object)page, "page");
			Assert.ArgumentNotNull((object)newpage, "newpage");
			if (page == "Retry")
				newpage = "Settings";
			if (newpage == "Importing")
			{
				DownloadingText.Visible = DownloadLatest.Checked;				
			}
			return base.ActivePageChanging(page, ref newpage);
		}

		/// <summary>
		/// Confirms the publishing options.
		/// 
		/// </summary>
		/// <param name="args">The args.</param>
		//protected void ConfirmPublishingOptions(ClientPipelineArgs args)
		//{
		//	Assert.ArgumentNotNull((object)args, "args");
		//	if (!args.HasResult)
		//	{
		//		bool checked1 = this.PublishChildren.Checked;
		//		bool checked2 = this.PublishRelatedItems.Checked;
		//		bool checked3 = this.Republish.Checked;
		//		string str = string.Empty;
		//		if (!string.IsNullOrEmpty(this.ItemID))
		//		{
		//			if (checked2 && checked1)
		//				str = Translate.Text("You are about to publish the current item, its subitems, and related items.");
		//			else if (checked2)
		//				str = Translate.Text("You are about to publish the current item and its related items.");
		//			else if (checked1)
		//				str = Translate.Text("You are about to publish the current item and its subitems.");
		//		}
		//		else
		//			str += Translate.Text("You are about to republish the whole database.");
		//		if (checked3)
		//		{
		//			if (!string.IsNullOrEmpty(str))
		//				str += "\n\n";
		//			str += Translate.Text("Republishing is an expensive operation that overwrites every item in the selected languages, even if that data has not changed.\nYou should only republish if the databases appear to be inconsistent and only after you have tried to fix the problem by performing a Smart Publishing operation.");
		//		}
		//		if (!string.IsNullOrEmpty(str))
		//		{
		//			SheerResponse.Confirm(str + "\n\n" + Translate.Text("Do you want to proceed?"));
		//			args.WaitForPostBack(true);
		//		}
		//		else
		//		{
		//			this.ConfirmedOptions = true;
		//			this.Next();
		//		}
		//	}
		//	else
		//	{
		//		if (!(args.Result == "yes"))
		//			return;
		//		this.ConfirmedOptions = true;
		//		this.Next();
		//	}
		//}

		/// <summary>
		/// Raises the load event.
		/// 
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.
		///             </param>
		/// <remarks>
		/// This method notifies the server control that it should perform actions common to each HTTP
		///             request for the page it is associated with, such as setting up a database query. At this
		///             stage in the page lifecycle, server controls in the hierarchy are created and initialized,
		///             view state is restored, and form controls reflect client-side data. Use the IsPostBack
		///             property to determine whether the page is being loaded in response to a client postback,
		///             or if it is being loaded and accessed for the first time.
		/// 
		/// </remarks>
		protected override void OnLoad(EventArgs e)
		{
			Assert.ArgumentNotNull((object)e, "e");
			base.OnLoad(e);
			if (Context.ClientPage.IsEvent)
				return;
			this.ItemID = WebUtil.GetQueryString("id");
			this.Database = WebUtil.GetQueryString("database");


			//this.BuildPublishingTargets();
			//this.BuildLanguages();
			//this.BuildPublishTypes();
			//this.IncrementalPublish.Checked = UserOptions.Publishing.IncrementalPublish;
			//this.SmartPublish.Checked = UserOptions.Publishing.SmartPublish;
			//this.Republish.Checked = UserOptions.Publishing.Republish;
			//this.PublishChildren.Checked = UserOptions.Publishing.PublishChildren;
			//this.PublishRelatedItems.Checked = UserOptions.Publishing.PublishRelatedItems;
		}

		/// <summary>
		/// Shows the result.
		/// 
		/// </summary>
		protected void ShowResult()
		{
			this.ShowResultPane.Visible = false;
			this.ResultText.Visible = true;
			this.ResultLabel.Visible = true;
		}

		protected void StartImporter()
		{
			if (DownloadLatest.Checked)
			{
			
			}
			var handle = UserImportManager.ImportUsers(ItemID, Database, DownloadLatest.Checked, IgnoreFileDate.Checked);
			this.JobHandle = handle.ToString();
			SheerResponse.Timer("CheckStatus", 500);
		}

		/// <summary>
		/// Starts the publisher.
		/// 
		/// </summary>
		protected void StartPublisher()
		{
			//Language[] languages = GetLanguages();
			//List<Item> publishingTargets = GetPublishingTargets();
			//Database[] publishingTargetDatabases = GetPublishingTargetDatabases();
			//bool b1 = Context.ClientPage.ClientRequest.Form["PublishMode"] == "IncrementalPublish";
			//bool flag = Context.ClientPage.ClientRequest.Form["PublishMode"] == "SmartPublish";
			//bool b2 = Context.ClientPage.ClientRequest.Form["PublishMode"] == "Republish";
			//bool rebuild = this.Rebuild;
			//bool checked1 = this.PublishChildren.Checked;
			//bool checked2 = this.PublishRelatedItems.Checked;
			//string str = this.ItemID;
			//if (string.IsNullOrEmpty(str))
			//	str = "null";
			//string message;
			//if (rebuild)
			//	message = string.Format("Rebuild database, databases: {0}", (object)StringUtil.Join((IEnumerable)publishingTargetDatabases, ", "));
			//else
			//	message = string.Format("Publish, root: {0}, languages:{1}, targets:{2}, databases:{3}, incremental:{4}, smart:{5}, republish:{6}, children:{7}, related:{8}", (object)str, (object)StringUtil.Join((IEnumerable)languages, ", "), (object)StringUtil.Join((IEnumerable)publishingTargets, ", ", "Name"), (object)StringUtil.Join((IEnumerable)publishingTargetDatabases, ", "), (object)MainUtil.BoolToString(b1), (object)MainUtil.BoolToString(flag), (object)MainUtil.BoolToString(b2), (object)MainUtil.BoolToString(checked1), (object)MainUtil.BoolToString(checked2));
			//Log.Audit(message, this.GetType());
			//ListString listString1 = new ListString();
			//foreach (Language language in languages)
			//	listString1.Add(language.ToString());
			//Registry.SetString("/Current_User/Publish/Languages", listString1.ToString());
			//ListString listString2 = new ListString();
			//foreach (Item obj in publishingTargets)
			//	listString2.Add(obj.ID.ToString());
			//Registry.SetString("/Current_User/Publish/Targets", listString2.ToString());
			//UserOptions.Publishing.IncrementalPublish = b1;
			//UserOptions.Publishing.SmartPublish = flag;
			//UserOptions.Publishing.Republish = b2;
			//UserOptions.Publishing.PublishChildren = checked1;
			//UserOptions.Publishing.PublishRelatedItems = checked2;
			//this.JobHandle = (string.IsNullOrEmpty(this.ItemID) ? (!b1 ? (!flag ? (!rebuild ? (object)PublishManager.Republish(Sitecore.Client.ContentDatabase, publishingTargetDatabases, languages, Context.Language) : (object)PublishManager.RebuildDatabase(Sitecore.Client.ContentDatabase, publishingTargetDatabases)) : (object)PublishManager.PublishSmart(Sitecore.Client.ContentDatabase, publishingTargetDatabases, languages, Context.Language)) : (object)PublishManager.PublishIncremental(Sitecore.Client.ContentDatabase, publishingTargetDatabases, languages, Context.Language)) : (object)PublishManager.PublishItem(Sitecore.Client.GetItemNotNull(this.ItemID), publishingTargetDatabases, languages, checked1, flag, checked2)).ToString();
			//SheerResponse.Timer("CheckStatus", Settings.Publishing.PublishDialogPollingInterval);
		}


		
	}
}