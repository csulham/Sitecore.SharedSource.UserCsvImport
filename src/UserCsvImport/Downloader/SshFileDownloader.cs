using System;
using System.IO;
using System.Net;
using Renci.SshNet;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.UserCsvImport.Downloader
{
	public class SshFileDownloader : IFileDownloader
	{
		public SshFileDownloader(string hostname, string username, string password, int port = 22)
		{
			Hostname = hostname;
			Username = username;
			Password = password;
			Port = port;
		}

		public string Hostname { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }
		public int Port { get; private set; }

		public bool TryGetFileFromFtp(string fileName, DateTime lastUpdated)
		{
			if (!DirectoryUtil.VerifyDirectory(fileName))
			{
				return false;
			}

			var fileToDownload =  fileName;
			Log.Debug(string.Format("Attempting to download file " + fileToDownload));
			
			try
			{
				Log.Debug("Opening FTP Connection to " + Hostname);
				using (var client = new SftpClient(Hostname, Username, Password))
				{
					client.Connect();
					Log.Debug(string.Format("Connection to {0} opened.", Hostname));
					var fileUpdated = client.GetLastWriteTime(fileName);
					Log.Debug(string.Format("File {0} was last modified on {1}.", fileName, DateUtil.ToIsoDate(fileUpdated)));

					if (fileUpdated <= lastUpdated)
					{
						Log.Info(string.Format("Did not download file {0}, it was last modified {1} and we last processed it on {2}.",
																		fileName, DateUtil.ToIsoDate(fileUpdated), DateUtil.ToIsoDate(lastUpdated)), this);
						return false;
					}

					var outputPath = string.Format("{0}\\{1}", UserCsvImportSettings.CsvFolderPath, fileName);
					Log.Debug(string.Format("Downloading file {0} and saving to path {1}.", fileName, outputPath));
					using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
					{
						client.DownloadFile(fileName, fileStream);
						Log.Debug("File successfully written to " + fileStream.Name);
					}
					Log.Debug("File Download complete.");
					Log.Debug("Updating timestamp.");
					File.SetLastWriteTime(outputPath, fileUpdated);
					Log.Debug("File timestamp set to " + fileUpdated.ToString());
				}
			}
			catch (Exception e)
			{
				Log.Error("File did not download successfully.", this);
				Log.Error(string.Format("Could not download file {0} from {1} using user {2}.", fileToDownload, Hostname, Username), e,
					this);
				return false;
				 
			}

			return true;
		}

		private FtpWebRequest GetNewRequest(string fileToDownload)
		{
			var request = (FtpWebRequest)WebRequest.Create(fileToDownload);
			request.Credentials = new NetworkCredential(Username, Password);
			return request;
		}
	}
}