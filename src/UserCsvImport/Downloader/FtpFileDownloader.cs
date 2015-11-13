using System;
using System.IO;
using System.Net;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.UserCsvImport.Downloader
{
	public class FtpFileDownloader : IFileDownloader
	{
		public FtpFileDownloader(string hostname, string username, string password, int port = 21)
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
			var outputPath = string.Format("{0}\\{1}", UserCsvImportSettings.CsvFolderPath, fileName);

			var fileToDownload = string.Format("ftp://{0}/{1}", Hostname, fileName);

			Log.Debug(string.Format("Attempting to download file " + fileToDownload));

			var request = GetNewRequest(fileToDownload);
			
			try
			{
				// Have to do 2 requests, one to get the timestamp then one to get the file itself.
				request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
				DateTime fileUpdated;
				Log.Debug("Opening FTP Connection to " + Hostname);
				using (var response = (FtpWebResponse) request.GetResponse())
				{
					Log.Debug("Getting response from FTP server...");
					fileUpdated = response.LastModified;
					Log.Debug(string.Format("File {0} was last modified on {1}.", fileName, DateUtil.ToIsoDate(fileUpdated)));
				}

				if (fileUpdated <= lastUpdated)
				{
					Log.Info(string.Format("Did not download file {0}, it was last modified {1} and we last processed it on {2}.", 
																	fileName, DateUtil.ToIsoDate(fileUpdated), DateUtil.ToIsoDate(lastUpdated)), 
						this);
					return false;
				}

				request = GetNewRequest(fileToDownload);
				request.Method = WebRequestMethods.Ftp.DownloadFile;
				Log.Debug("Opening FTP Connection to " + Hostname);
				using (var response = (FtpWebResponse) request.GetResponse())
				{
					Log.Debug("Getting response from FTP server...");
					using (var responseStream = response.GetResponseStream())
					{
						Log.Debug(string.Format("Downloading file {0} and saving to path {1}.", fileName, outputPath));
						using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
						{
							if (responseStream != null) responseStream.CopyTo(fileStream);
							Log.Debug("File successfully written to " + fileStream.Name);
						}
						Log.Debug("File Download complete.");
						Log.Debug("Updating timestamp.");
						File.SetLastWriteTime(outputPath, fileUpdated);
						Log.Debug("File timestamp set to " + fileUpdated.ToString());
					}
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