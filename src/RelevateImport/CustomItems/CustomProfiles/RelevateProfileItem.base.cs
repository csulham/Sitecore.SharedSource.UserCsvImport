using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Data.Items;

namespace RelevateImport.CustomItems.CustomProfiles
{
	// not a normal custom item, should never be instantiated
	public class RelevateProfileItem
	{
		/// <summary>
		/// The ID of the profile item in the Core DB.
		/// </summary>
		public static readonly string ProfileId = "{61C888DB-68C2-4677-BDF4-390F7AC86D9A}";

		public static readonly string ContactIdFieldName = "Contact Id";
		public static readonly string FirstNameFieldName = "First Name";
		public static readonly string LastNameFieldName = "Last Name";
		public static readonly string EmailFieldName = "Relevate Email";
		public static readonly string ZipCodeFieldName = "Zip Code";
		public static readonly string PromoCodeFieldName = "Promo Code";
		public static readonly string ChannelFieldName = "Channel";
		public static readonly string SegmentNameFieldName = "Segment Name";

		
	}
}