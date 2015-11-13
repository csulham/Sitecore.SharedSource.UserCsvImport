using System.Collections.Generic;
using System.Linq;

namespace Sitecore.SharedSource.UserCsvImport.CSVParser
{
	public class CsvUser
	{
		/// <summary>
		/// The field or fields that contain the name information. Will be joined with a whitespace.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// The field data that contains the email address of the user. REQUIRED
		/// </summary>
		public string Email { get; private set; }
		/// <summary>
		/// The field data that contains the unique id for creating the user. REQUIRED
		/// </summary>
		public string Identity { get; private set; }

		public IDictionary<string, string> ProfileProperties { get; private set; } 

		public CsvUser(string identityField, string[] nameFields, string emailField, IDictionary<string,string> attributes )
		{
			ProfileProperties = attributes ?? new Dictionary<string, string>();
			SetIdentity(identityField);
			SetName(nameFields);
			SetEmail(emailField);
		}

		private void SetIdentity(string identityField)
		{
			Identity = ProfileProperties.ContainsKey(identityField) ? ProfileProperties[identityField] : string.Empty;
		}

		private void SetName(string [] nameFields)
		{
			var names = nameFields.Where(ProfileProperties.ContainsKey).Select(n => ProfileProperties[n]);
			Name = string.Join(" ", names.ToArray());
		}

		private void SetEmail(string emailField)
		{
			Email = ProfileProperties.ContainsKey(emailField) ? ProfileProperties[emailField] : string.Empty;
		}
	}
}
