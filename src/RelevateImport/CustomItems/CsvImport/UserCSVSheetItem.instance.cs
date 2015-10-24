using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RelevateImport.CSVParser;
using RelevateImport.Downloader;
using RelevateImport.Membership;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace RelevateImport.CustomItems.CsvImport
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

			var loader = new CsvFileLoader(CsvFileFullPath);

			if (!loader.CsvFile.Exists)
			{
				throw new FileNotFoundException("Csv file could not be loaded.", CsvFileFullPath);
			}

			if (LastUpdated.DateTime != DateTime.MinValue && LastUpdated.DateTime >= loader.CsvFile.LastWriteTime)
			{
				Log.Info(string.Format("File {0} not modified since the last import was run", FileName.Raw), this);
			}

			// Load the File
			var mapped = loader.GetUsersFromCsv();

			// Parse the mapped users
			var users = LoadMappedUsers(mapped);

			// Take the timestamp
			DateTime editStartTime = loader.CsvFile.LastWriteTime;

			// Load the users
			var um = new UserManager(Role.Raw);
			um.CreateUsers(users.ToList());

			// Update the File Date
			using (new SecurityDisabler())
			{
				InnerItem.Editing.BeginEdit();
				LastUpdated.Field.InnerField.SetValue(DateUtil.ToIsoDate(editStartTime, false), false);
				InnerItem.Editing.EndEdit();
			}
		}

		private IEnumerable<CsvUser> LoadMappedUsers(List<DictionaryMapped> mapped)
		{
			foreach (var map in mapped)
			{
				var user = new CsvUser("str_contact_id", new[] { "First_Name", "Last_Name" }, "EMail", map.Attributes);
				if (string.IsNullOrEmpty(user.Identity))
				{
					Log.Debug("User contained no identity data. \nRow Data = " + map.DictionaryData, this);
					continue;
				}
				if (string.IsNullOrEmpty(user.Email))
				{
					Log.Debug("User contained no email data. \nRow Data = "+ map.DictionaryData, this);
					continue;
				}

				yield return user;
			}
		}

		public void DownloadLatestCsv()
		{
			FtpFileDownloader ftp = new FtpFileDownloader(RelevateSettings.FtpHostname, RelevateSettings.FtpUsername, RelevateSettings.FtpPassword);
			ftp.TryGetFileFromFtp(FileName.Raw, LastUpdated.DateTime);
		}
	}
}