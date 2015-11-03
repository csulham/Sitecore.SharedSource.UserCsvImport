using System;
using System.IO;
using System.Threading;
using RelevateImport.CustomItems.CsvImport;
using RelevateImport.Membership;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Publishing;

namespace RelevateImport.CustomSitecore
{
	public class UserImportManager
	{
		public static Handle ImportUsers(string userSheetItemId, string database, bool downloadFile, bool ignoreDate)
		{
			UserImportStatus importStatus = new UserImportStatus();
			UserImportOptions options = new UserImportOptions()
			{
				CsvSheetItemId = userSheetItemId,
				Database = database,
				DownloadLatestFile = downloadFile, 
				IgnoreFileDate = ignoreDate
			};
			JobOptions options1 = new JobOptions("ImportCsvUsers", "UserImportManager", "shell", (object)new UserImportManager(), "DoImport", new object[2]
      {
        (object) options,
        (object) importStatus
      });
			options1.ContextUser = Context.User;
			options1.ClientLanguage = Context.Language;
			options1.AfterLife = TimeSpan.FromMilliseconds(1000);
			options1.Priority = ThreadPriority.Normal;
			options1.ExecuteInManagedThreadPool = true;
			importStatus.State = JobState.Queued;
			Job job = JobManager.Start(options1);
			job.Options.CustomData = (object)importStatus;
			return job.Handle;
		}

		private UserImportManager()
		{
			
		}

		public void DoImport(UserImportOptions options, UserImportStatus status)
		{
			status.State = JobState.Initializing;
			try
			{
				UserCSVSheetItem userCsvSheetItem = Factory.GetDatabase(options.Database).GetItem(options.CsvSheetItemId);
				status.State = JobState.Running;

				if (options.DownloadLatestFile)
				{
					userCsvSheetItem.DownloadLatestCsv();
				}
				FileInfo file = new FileInfo(userCsvSheetItem.CsvFileFullPath);
				if (!file.Exists)
				{
					status.ErrorMessage = userCsvSheetItem.FileName + ": File cannot be found.";
				}
				status.CsvFileName = file.FullName;
				status.CsvFileDate = file.LastWriteTime;

				userCsvSheetItem.ImportUsersFromCsv(options.IgnoreFileDate, status);
				// FAKE IMPORT FOR TESTING SHEER DIALOG
				//status.TotalUserCount = 1000;
				//do
				//{
				//	status.UsersProcessed++;
				//	Thread.Sleep(10);
				//} while (status.UsersProcessed < status.TotalUserCount);
			}
			catch (Exception e)
			{
				status.ErrorMessage = e.Message;
				Log.Error("Error importing users", e, this);
			}
			status.State = JobState.Finished;
		}
	}
}