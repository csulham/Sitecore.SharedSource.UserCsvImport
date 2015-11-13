using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;

namespace UserCsvImport.Downloader
{
	public class DirectoryUtil
	{
		public static bool TryGetOrCreateDirectoryForFile(string filePath, out DirectoryInfo directory)
		{
			directory = null;
			try
			{
				// Don't create directory if the one in the settings isn't defined or doesn't exist
				if (!Directory.Exists(UserCsvImportSettings.CsvFolderPath))
				{
					Log.Error(string.Format("Specified CSV directory {0} does not exist.", UserCsvImportSettings.CsvFolderPath),
						typeof (DirectoryUtil));
					return false;
				}

				FileInfo fi = new FileInfo(filePath);
				if (fi.DirectoryName == null || fi.Directory == null)
				{
					Log.Error(string.Format("Specified CSV directory {0} does not exist for file.", filePath), typeof (DirectoryUtil));
					return false;
				}
				if (fi.Directory.Exists)
				{
					directory = fi.Directory;
					return true;
				}

				// Create it if it doesn't exist
				Log.Debug("Creating directory " + fi.DirectoryName, typeof (DirectoryUtil));
				directory = Directory.CreateDirectory(fi.DirectoryName);
				return directory.Exists;
			}
			catch (Exception e)
			{
				Log.Error("Could not create directory.", e, typeof(DirectoryUtil));
				return false;
			}
			
		}

		public static bool VerifyDirectory(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				Log.Error("Cannot download a file with no name.", typeof(DirectoryUtil));
				return false;
			}
			var outputPath = string.Format("{0}\\{1}", UserCsvImportSettings.CsvFolderPath, fileName);

			DirectoryInfo directory;
			if (!DirectoryUtil.TryGetOrCreateDirectoryForFile(outputPath, out directory))
			{
				return false;
			}
			return directory.Exists;
		}
	}
}
