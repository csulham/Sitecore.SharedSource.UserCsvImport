using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCsvImport
{
	public class UserCsvImportSettings
	{
		public static string CsvFolderPath
		{
			get
			{
				return string.Format("{0}\\{1}", 
					Sitecore.Configuration.Settings.DataFolder, Sitecore.Configuration.Settings.GetSetting("UserCsvImport.CsvFolder"));
			}
		}

		public static string FtpHostname { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.FtpHostname"); } }
		public static string FtpUsername { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.FtpUsername"); } }
		public static string FtpPassword { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.FtpPassword"); } }

		public static string CsvUserDomain { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.NewUserDomain"); } }
		public static string DefaultUserPassword { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.NewUserPassword"); } }

		public static string CustomProfileId { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.CustomProfileId"); } }
	}
}
