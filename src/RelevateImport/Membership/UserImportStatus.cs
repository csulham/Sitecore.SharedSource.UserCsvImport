using System;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Jobs;

namespace RelevateImport.Membership
{
	public class UserImportStatus
	{
		public int TotalUserCount { get; set; }
		public int UsersProcessed { get; set; }
		public string CsvFileName { get; set; }
		public DateTime CsvFileDate { get; set; }
		public bool CsvFileDownloaded { get; set; }
		public bool UserAbort { get; set; }
		public string ErrorMessage { get; set; }
		public JobState State { get; set; }

		public static UserImportStatus GetStatus(Handle handle)
		{
			Error.AssertObject(handle, "handle");
			var job = JobManager.GetJob(handle);
			if (job != null)
				return job.Options.CustomData as UserImportStatus;
			return null;
		}
	}
}