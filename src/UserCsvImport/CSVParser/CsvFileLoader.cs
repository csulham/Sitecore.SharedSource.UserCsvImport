using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Sitecore.Diagnostics;

namespace UserCsvImport.CSVParser
{
	public class CsvFileLoader
	{
		private string CsvFilePath { get; set; }
		public FileInfo CsvFile { get; private set; } 

		public CsvFileLoader(string csvFileName)
		{
			CsvFilePath = csvFileName;
			CsvFile = new FileInfo(CsvFilePath);
		}

		public List<DictionaryMapped> GetUsersFromCsv()
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
						var records = reader.GetRecords<dynamic>();
						return records.Select(r => new DictionaryMapped(r as IDictionary<string, object>)).ToList();
					}
				}
			}
			catch (Exception fl)
			{
				Log.Error("An error occured loading the CSV file " + CsvFilePath, fl, this);
				throw;
			}
		}
	}
}
