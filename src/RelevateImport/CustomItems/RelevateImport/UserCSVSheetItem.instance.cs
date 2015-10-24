using System;
using System.IO;
using RelevateImport.CSVParser;
using RelevateImport.Downloader;
using RelevateImport.Membership;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace RelevateImport.CustomItems.RelevateImport
{
	public partial class UserCSVSheetItem
	{
		public string CsvFileFullPath
		{
			get
			{
				return string.IsNullOrEmpty(FileName.Raw)
					? string.Empty
					: string.Format("{0}\\{1}", RelevateSettings.CsvFolderPath, FileName.Raw);
			}
		}

		public void ImportUsersFromCsv()
		{
			// Check parameters
			if (string.IsNullOrEmpty(CsvFileFullPath))
			{
				throw new ArgumentNullException("FileName", "File Name must be specified.");
			}

			var loader = new RelevateFileLoader(CsvFileFullPath);

			if (!loader.CsvFile.Exists)
			{
				throw new FileNotFoundException("Csv file could not be loaded.", CsvFileFullPath);
			}

			if (LastUpdated.DateTime != DateTime.MinValue && LastUpdated.DateTime >= loader.CsvFile.LastWriteTime)
			{
				Log.Info(string.Format("File {0} not modified since the last import was run", FileName.Raw), this);
			}

			// Load the File
			var users = loader.GetUsersFromCsv();

			// Take the timestamp
			DateTime editStartTime = loader.CsvFile.LastWriteTime;

			// Load the users
			var um = new UserManager(Role.Raw);
			um.CreateUsers(users);

			// Update the File Date
			using (new SecurityDisabler())
			{
				InnerItem.Editing.BeginEdit();
				LastUpdated.Field.InnerField.SetValue(DateUtil.ToIsoDate(editStartTime, false), false);
				InnerItem.Editing.EndEdit();
			}
		}

		public void DownloadLatestCsv()
		{
			FtpFileDownloader ftp = new FtpFileDownloader(RelevateSettings.FtpHostname, RelevateSettings.FtpUsername, RelevateSettings.FtpPassword);
			ftp.TryGetFileFromFtp(FileName.Raw, LastUpdated.DateTime);
		}
	}
}