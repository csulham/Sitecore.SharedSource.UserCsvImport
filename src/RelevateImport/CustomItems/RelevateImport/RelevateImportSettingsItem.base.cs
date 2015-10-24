using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;

namespace RelevateImport.CustomItems.RelevateImport
{
public partial class RelevateImportSettingsItem : CustomItem
{

public static readonly string TemplateId = "{335B9502-FD2C-406F-94ED-4C77A0ACAF7F}";


#region Boilerplate CustomItem Code

public RelevateImportSettingsItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator RelevateImportSettingsItem(Item innerItem)
{
	return innerItem != null ? new RelevateImportSettingsItem(innerItem) : null;
}

public static implicit operator Item(RelevateImportSettingsItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


#endregion //Field Instance Methods
}
}