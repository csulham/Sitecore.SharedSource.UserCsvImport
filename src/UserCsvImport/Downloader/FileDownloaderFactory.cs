namespace Sitecore.SharedSource.UserCsvImport.Downloader
{
	public class FileDownloaderFactory
	{
		public static IFileDownloader GetDownloader()
		{
			if (UserCsvImportSettings.FtpUseSecureDownloader)
			{
				return new SshFileDownloader(UserCsvImportSettings.FtpHostname, UserCsvImportSettings.FtpUsername, UserCsvImportSettings.FtpPassword);
			}
			return new FtpFileDownloader(UserCsvImportSettings.FtpHostname, UserCsvImportSettings.FtpUsername, UserCsvImportSettings.FtpPassword);
		}
	}
}
