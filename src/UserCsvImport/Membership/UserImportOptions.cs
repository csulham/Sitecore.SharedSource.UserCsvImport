using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace UserCsvImport.Membership
{
	public class UserImportOptions
	{
		public string CsvSheetItemId { get; set; }
		public string Database { get; set; }
		public bool DownloadLatestFile { get; set; }
		public bool IgnoreFileDate { get; set; }
	}
}
