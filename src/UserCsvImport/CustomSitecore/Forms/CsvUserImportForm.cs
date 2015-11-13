using System;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.SharedSource.UserCsvImport.CustomItems.CsvImport;
using Sitecore.SharedSource.UserCsvImport.Membership;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.SharedSource.UserCsvImport.CustomSitecore.Forms
{
	public class CsvUserImportForm : WizardForm
	{
		/// <summary>
		/// The error text.
		/// 
		/// </summary>
		protected Memo ErrorText;
		
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
				DownloadingText.InnerHtml = string.Format("Downloading latest file from {0}...", UserCsvImportSettings.FtpHostname);
			}
			return base.ActivePageChanging(page, ref newpage);
		}
		
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
				// TODO: progress bar on downloader?
			}
			var handle = UserImportManager.ImportUsers(ItemID, Database, DownloadLatest.Checked, IgnoreFileDate.Checked);
			this.JobHandle = handle.ToString();
			SheerResponse.Timer("CheckStatus", 500);
		}

	}
}