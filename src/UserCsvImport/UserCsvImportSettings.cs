namespace Sitecore.SharedSource.UserCsvImport
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

		public static bool FtpUseSecureDownloader
		{
			get
			{
				return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.Ftp.UseSsh").ToLower() == "true";
			}
		}

		public static string CsvUserDomain { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.NewUserDomain"); } }
		public static string DefaultUserPassword { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.NewUserPassword"); } }

		public static string CustomProfileId { get { return Sitecore.Configuration.Settings.GetSetting("UserCsvImport.CustomProfileId"); } }
	}
}
