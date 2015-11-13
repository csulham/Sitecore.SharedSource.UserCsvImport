using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.UserCsvImport.CustomItems.CsvImport
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