using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelevateImport
{
	public class RelevateSettings
	{
		public static string CsvFolderPath
		{
			get
			{
				return string.Format("{0}\\{1}", 
					Sitecore.Configuration.Settings.DataFolder, Sitecore.Configuration.Settings.GetSetting("RelevateImport.CsvFolder"));
			}
		}

		public static string FtpHostname { get { return Sitecore.Configuration.Settings.GetSetting("RelevateImport.FtpHostname"); } }
		public static string FtpUsername { get { return Sitecore.Configuration.Settings.GetSetting("RelevateImport.FtpUsername"); } }
		public static string FtpPassword { get { return Sitecore.Configuration.Settings.GetSetting("RelevateImport.FtpPassword"); } }

		public static string RelevateUserDomain { get { return Sitecore.Configuration.Settings.GetSetting("RelevateImport.NewUserDomain"); } }
		public static string DefaultUserPassword { get { return Sitecore.Configuration.Settings.GetSetting("RelevateImport.NewUserPassword"); } }

		public static string CustomProfileId { get { return Sitecore.Configuration.Settings.GetSetting("RelevateImport.CustomProfileId"); } }
	}
}
