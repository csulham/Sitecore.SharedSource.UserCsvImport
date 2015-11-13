using System.Collections.Generic;
using System.Linq;

namespace Sitecore.SharedSource.UserCsvImport.CSVParser
{
	public class DictionaryMapped
	{
		public DictionaryMapped(IDictionary<string, object> attributes)
		{
			Attributes = attributes.ToDictionary(pair => pair.Key, pair => ((string)pair.Value).Trim());
		}

		public IDictionary<string, string> Attributes { get; internal set; }

		/// <summary>
		/// Returns the dictionary fields for debug/logging purposes.
		/// </summary>
		public string DictionaryData
		{
			get
			{
				return string.Join(", ", Attributes.Select(x => x.Key + ":" + x.Value).ToArray());
			}
		}
	}
}