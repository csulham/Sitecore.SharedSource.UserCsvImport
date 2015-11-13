using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using Sitecore.SharedSource.UserCsvImport.CSVParser;
using Sitecore.SharedSource.UserCsvImport.Downloader;
using Sitecore.SharedSource.UserCsvImport.Membership;

namespace Sitecore.SharedSource.UserCsvImport.CustomItems.CsvImport
{
	public partial class UserCSVSheetItem
	{
		public string CsvFileFullPath
		{
			get
			{
				return string.IsNullOrEmpty(FileName.Raw)
					? string.Empty
					: string.Format("{0}\\{1}", UserCsvImportSettings.CsvFolderPath, FileName.Raw);
			}
		}

		public void ImportUsersFromCsv(bool ignoreDate, UserImportStatus statusObject)
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
				Log.Info(string.Format("File {0} not modified since the last import was run. Skipping UserCsvSheet item {1}.", FileName.Raw, this.Name), this);
				if (!ignoreDate)
				{
					return;
				}
			}

			// Load the File
			var mapped = loader.GetUsersFromCsv();

			// Parse the mapped users
			var users = LoadMappedUsers(mapped);

			// Take the timestamp
			DateTime editStartTime = loader.CsvFile.LastWriteTime;

			// Load the users
			var um = new UserManager(Role.Raw, CustomProfileItem, statusObject);
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
				var user = new CsvUser(IdentityFieldName.Raw, NameFields, EmailFieldName.Raw, map.Attributes);
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

		private Sitecore.Data.Items.Item _profileItem;
		public Sitecore.Data.Items.Item CustomProfileItem
		{
			get
			{
				if (_profileItem == null)
				{
					Database core = Factory.GetDatabase("core");
					_profileItem = core.GetItem(CustomProfile.Raw);
				}
				return _profileItem;
			}
		}

		private string[] _nameFields;
		public string[] NameFields
		{
			get
			{
				if (_nameFields == null)
				{
					if (string.IsNullOrEmpty(FullNameFieldNames.Raw))
					{
						Log.Warn("Full Name fields not defined for UserCsvSheet item " + this.Name, this);
						_nameFields = new string[] {};
					}
					_nameFields = FullNameFieldNames.Raw.Split(',').Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).ToArray();
				}
				return _nameFields;
			}
		}

		public bool DownloadLatestCsv(bool ignoreDate)
		{
			IFileDownloader downloader = FileDownloaderFactory.GetDownloader();
			return downloader.TryGetFileFromFtp(FileName.Raw, LastUpdated.DateTime);
		}
	}
}