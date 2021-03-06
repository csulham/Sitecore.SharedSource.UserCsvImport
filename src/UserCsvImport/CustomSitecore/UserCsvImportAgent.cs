﻿using System;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.UserCsvImport.CustomItems.CsvImport;
using Sitecore.SharedSource.UserCsvImport.Membership;

namespace Sitecore.SharedSource.UserCsvImport.CustomSitecore
{
	public class UserCsvImportAgent : Sitecore.Tasks.BaseAgent
	{
		private readonly string CsvFolderRoot;

		public UserCsvImportAgent(string csvFolderRoot)
		{
			CsvFolderRoot = csvFolderRoot;
		}

		public void Run()
		{
			LogInfo("Starting Relevate user file import job.");

			LogDebug("RelevateUserImportAgent: csvFolderRoot = " + CsvFolderRoot);

			// Collect the items
			var csvsToProcess = UserCSVSheetItem.GetAllImportSheets(CsvFolderRoot);

			foreach (UserCSVSheetItem sheet in csvsToProcess)
			{
				try
				{
					LogDebug(string.Format("START: Processing item {0}.", sheet.Name));
					// Get the file from the FTP
					LogDebug(string.Format("START: Attempting to download CSV {0} from FTP server.", sheet.FileName.Raw));
					sheet.DownloadLatestCsv(false);
					LogDebug(string.Format("END: Downloaded CSV {0} from FTP server.", sheet.FileName.Raw));


					// Process the file
					LogDebug(string.Format("START: Loading users from CSV {0}.", sheet.CsvFileFullPath));
					sheet.ImportUsersFromCsv(false,new UserImportStatus());
					LogDebug(string.Format("END: Loaded users from CSV {0}.", sheet.CsvFileFullPath));

					LogDebug(string.Format("END: Processed item {0}.", sheet.Name));
				}
				catch (Exception e)
				{
					Log.Error(string.Format("Error processing item {0}.", sheet.Name), e, this);
				}
			}

			LogInfo("Relevate user file import job ended.");
		}

		

	}
}
