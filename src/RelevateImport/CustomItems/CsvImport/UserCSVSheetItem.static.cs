using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace RelevateImport.CustomItems.CsvImport
{
	public partial class UserCSVSheetItem 
	{
		public static IEnumerable<UserCSVSheetItem> GetAllImportSheets(string parentFolderRoot)
		{
			Sitecore.Data.Database master = Factory.GetDatabase("master");
			Item folder = master.GetItem(parentFolderRoot);
			if (folder == null || !folder.HasChildren)
			{
				yield break;
			}

			foreach (Item child in folder.Children)
			{
				if (child.TemplateID.ToString() == TemplateId)
				{
					yield return new UserCSVSheetItem(child);
				}
			}
		}
	}
}