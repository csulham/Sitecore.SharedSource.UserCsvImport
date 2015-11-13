using System;

namespace UserCsvImport.Downloader
{
	public interface IFileDownloader
	{
		string Hostname { get; }
		string Username { get; }
		string Password { get; }
		int Port { get; }

		bool TryGetFileFromFtp(string fileName, DateTime lastUpdated);

	}
}