using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;

namespace UserCsvImport.CustomItems.CsvImport
{
public partial class UserCSVSheetItem : CustomItem
{

public static readonly string TemplateId = "{F153064C-A206-49DD-BE97-221AE2F4E8CA}";


#region Boilerplate CustomItem Code

public UserCSVSheetItem(Item innerItem) : base(innerItem)
{

}

public static implicit operator UserCSVSheetItem(Item innerItem)
{
	return innerItem != null ? new UserCSVSheetItem(innerItem) : null;
}

public static implicit operator Item(UserCSVSheetItem customItem)
{
	return customItem != null ? customItem.InnerItem : null;
}

#endregion //Boilerplate CustomItem Code


#region Field Instance Methods


public CustomTextField Role
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Role"]);
	}
}


public CustomTextField IdentityFieldName
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Identity Field Name"]);
	}
}


public CustomTextField EmailFieldName
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Email Field Name"]);
	}
}


public CustomTextField FullNameFieldNames
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["Full Name Field Names"]);
	}
}


public CustomTextField FileName
{
	get
	{
		return new CustomTextField(InnerItem, InnerItem.Fields["File Name"]);
	}
}


public CustomMultiListField CustomProfile
{
	get
	{
		return new CustomMultiListField(InnerItem, InnerItem.Fields["Custom Profile"]);
	}
}


public CustomDateField LastUpdated
{
	get
	{
		return new CustomDateField(InnerItem, InnerItem.Fields["Last Updated"]);
	}
}


#endregion //Field Instance Methods
}
}