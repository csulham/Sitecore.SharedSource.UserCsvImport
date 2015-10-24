using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Sitecore.Diagnostics;

namespace RelevateImport.CSVParser
{
	public class RelevateFileLoader
	{
		private string CsvFilePath { get; set; }
		public FileInfo CsvFile { get; private set; } 

		public RelevateFileLoader(string csvFileName)
		{
			CsvFilePath = csvFileName;
			CsvFile = new FileInfo(CsvFilePath);
		}

		public List<RelevateUser> GetUsersFromCsv()
		{
			if (!File.Exists(CsvFilePath))
			{
				throw new FileNotFoundException("Could not locate CSV at path " + CsvFilePath, CsvFilePath);
			}

			try
			{
				using (var sr = new StreamReader(CsvFilePath))
				{
					Log.Debug(string.Format("Attempting to parse user CSV sheet {0}.", CsvFilePath));
					using (var reader = new CsvReader(sr))
					{
						reader.Configuration.RegisterClassMap<RelevateUserMapper>();
						return reader.GetRecords<RelevateUser>().ToList();
					}
				}
			}
			catch (Exception fl)
			{
				Log.Error("An error occured loading the CSV file " + CsvFilePath, fl, typeof(RelevateUserMapper));
				throw;
			}
		}
	}
}
