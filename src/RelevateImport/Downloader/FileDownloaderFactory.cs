using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelevateImport.Downloader
{
	public class FileDownloaderFactory
	{
		public static IFileDownloader GetDownloader()
		{
			if (RelevateSettings.FtpHostname.ToLower().StartsWith("ftp"))
			{
				return new FtpFileDownloader(RelevateSettings.FtpHostname, RelevateSettings.FtpUsername, RelevateSettings.FtpPassword);
			}
			if (RelevateSettings.FtpHostname.ToLower().StartsWith("sftp"))
			{
				return new SshFileDownloader(RelevateSettings.FtpHostname, RelevateSettings.FtpUsername, RelevateSettings.FtpPassword);
			}
			return null;
		}
	}
}
