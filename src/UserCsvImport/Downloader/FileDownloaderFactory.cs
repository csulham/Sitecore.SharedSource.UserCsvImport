namespace Sitecore.SharedSource.UserCsvImport.Downloader
{
	public class FileDownloaderFactory
	{
		public static IFileDownloader GetDownloader()
		{
			if (UserCsvImportSettings.FtpHostname.ToLower().StartsWith("ftp"))
			{
				return new FtpFileDownloader(UserCsvImportSettings.FtpHostname, UserCsvImportSettings.FtpUsername, UserCsvImportSettings.FtpPassword);
			}
			if (UserCsvImportSettings.FtpHostname.ToLower().StartsWith("sftp"))
			{
				return new SshFileDownloader(UserCsvImportSettings.FtpHostname, UserCsvImportSettings.FtpUsername, UserCsvImportSettings.FtpPassword);
			}
			return null;
		}
	}
}
