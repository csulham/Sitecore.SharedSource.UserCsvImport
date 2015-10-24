using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RelevateImport.CSVParser;
using RelevateImport.CustomItems.RelevateImport;
using RelevateImport.Membership;
using Sitecore.Shell.Framework.Commands;

namespace RelevateImport.CustomSitecore
{
	public class LoadCsvButtonAction : Sitecore.Shell.Framework.Commands.Command
	{
		public override void Execute(CommandContext context)
		{
			UserCSVSheetItem userCsvSheet = context.Items[0];
			userCsvSheet.ImportUsersFromCsv();

		}
	}
}
