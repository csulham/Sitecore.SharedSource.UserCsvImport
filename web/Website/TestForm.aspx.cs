using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RelevateImport;
using RelevateImport.CustomSitecore;
using RelevateImport.Downloader;

namespace Website
{
	public partial class TestForm : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnDownload_OnClick(object sender, EventArgs e)
		{
			//string fn = txtFilename.Text;

			//FtpFileDownloader ftp = new FtpFileDownloader(RelevateSettings.FtpHostname, RelevateSettings.FtpUsername, RelevateSettings.FtpPassword);

			//if (ftp.TryGetFileFromFtp(fn, DateTime.MinValue)) Response.Write("Great Success.");

			RelevateUserImportAgent agent = new RelevateUserImportAgent("{48E393D3-E8CE-4EB2-AA3D-2E34733726F5}");
			agent.Run();
		}
	}
}