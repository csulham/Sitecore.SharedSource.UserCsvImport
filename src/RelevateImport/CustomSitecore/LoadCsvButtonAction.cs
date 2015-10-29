using System;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Text;
using Sitecore.Shell.Framework.Commands;

namespace RelevateImport.CustomSitecore
{
	public class LoadCsvButtonAction : Sitecore.Shell.Framework.Commands.Command
	{
		/*
		public override void Execute(CommandContext context)
		{
			UserCSVSheetItem userCsvSheet = context.Items[0];
			userCsvSheet.ImportUsersFromCsv();

		}*/

		/// <summary>
		/// Executes the command in the specified context.
		/// 
		/// </summary>
		/// <param name="context">The context.</param>
		public override void Execute(CommandContext context)
		{
			Error.AssertObject((object)context, "context");
			if (context.Items.Length != 1 || context.Items[0] == null)
				return;
			Item obj = context.Items[0];
			UrlString urlString = new UrlString(UIUtil.GetUri("control:CsvUserImport"));
			urlString.Append("id", obj.ID.ToString());
			urlString.Append("database", obj.Database.ToString());
			Context.ClientPage.ClientResponse.ShowModalDialog(urlString.ToString());
		}

		/// <summary>
		/// Queries the state of the command.
		/// 
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// The state of the command.
		/// </returns>
		public override CommandState QueryState(CommandContext context)
		{
			Error.AssertObject((object)context, "context");
			if (context.Items.Length != 1 || !Settings.Publishing.Enabled)
				return CommandState.Hidden;
			return base.QueryState(context);
		}
	}
}
