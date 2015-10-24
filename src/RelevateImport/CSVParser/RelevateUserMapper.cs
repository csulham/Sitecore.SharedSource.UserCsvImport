using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Sitecore.Diagnostics;

namespace RelevateImport.CSVParser
{
	public class RelevateUserMapper : CsvClassMap<RelevateUser>
	{
		public RelevateUserMapper()
		{
			Map(m => m.ContactId).Name("str_contact_id");
			Map(m => m.FirstName).Name("First_Name");
			Map(m => m.LastName).Name("Last_Name");
			Map(m => m.Email).Name("EMail");
			Map(m => m.ZipCode).Name("Zip_Code");
			Map(m => m.PromoCode).Name("Promo_Code");
			Map(m => m.Channel).Name("Channel");
			Map(m => m.SegmentName).Name("Segment Name");
		}
	}
}
