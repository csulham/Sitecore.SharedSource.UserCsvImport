namespace Sitecore.SharedSource.UserCsvImport.Membership
{
	public class UserImportOptions
	{
		public string CsvSheetItemId { get; set; }
		public string Database { get; set; }
		public bool DownloadLatestFile { get; set; }
		public bool IgnoreFileDate { get; set; }
	}
}
