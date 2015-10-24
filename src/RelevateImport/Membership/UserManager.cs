using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using RelevateImport.CSVParser;
using RelevateImport.CustomItems.CustomProfiles;
using Sitecore.Diagnostics;
using Sitecore.Security.Accounts;

namespace RelevateImport.Membership
{
	public class UserManager
	{
		private readonly string _roleName;
		private Sitecore.Security.Accounts.Role UserRole { get; set; }

		public UserManager(string role)
		{
			if (string.IsNullOrEmpty(role))
			{
				throw new ArgumentException("Role must be specified.");
			}
			_roleName = AddDomain(role);
		}

		public void CreateUsers(List<RelevateUser> users)
		{
			foreach (RelevateUser user in users)
			{
				try
				{
					CreateUser(user);
				}
				catch (Exception e)
				{
					Log.Error("Error creating user.", e, typeof(UserManager));
				}
			}
		}

		private void CreateUser(RelevateUser relevateUser)
		{
			UserRole = GetOrCreateRole();

			if (UserRole == null)
			{
				throw new MembershipCreateUserException("Could not get role " + _roleName);
			}

			User membershipUser = GetOrCreateUser(relevateUser);
			if (membershipUser == null)
			{
				throw new MembershipCreateUserException(string.Format("Error creating user {0}, {1}", relevateUser.ContactId, relevateUser.Email));
			}

			// Add Role
			if (!membershipUser.IsInRole(UserRole))
			{
				membershipUser.Roles.Add(UserRole);
			}

			// Update Profile
			UpdateProfile(membershipUser, relevateUser);
		}

		private Role GetOrCreateRole()
		{
			try
			{
				if (!Sitecore.Security.Accounts.Role.Exists(_roleName))
				{
					System.Web.Security.Roles.CreateRole(_roleName);
				}
				return Role.FromName(_roleName);
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Error fetching role {2}: Message: {0}; Source:{1}", ex.Message, ex.Source, _roleName), this);
				throw;
			}
		}

		private User GetOrCreateUser(RelevateUser relevateUser)
		{
			string userName = AddDomain(relevateUser.ContactId);
			try
			{
				foreach (System.Web.Security.MembershipUser mUser in
					System.Web.Security.Membership.FindUsersByName(userName))
				{
					// have to authenticate to edit profiles
					return User.FromName(mUser.UserName, true);
				}

				// User does not exist so create it
				var newUser = User.Create(userName, RelevateSettings.DefaultUserPassword);
				newUser.Profile.ProfileItemId = RelevateProfileItem.ProfileId;
				newUser.Profile.Save();
				return newUser;
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Error creating user {0}, {1}", relevateUser.ContactId, relevateUser.Email), ex, this);
				throw;
			}
		}

		private void UpdateProfile(User user, RelevateUser relevateUser)
		{
			user.Profile.Email = relevateUser.Email.Trim();
			user.Profile.FullName = string.Format("{0} {1}", relevateUser.FirstName.Trim(), relevateUser.LastName.Trim()).Trim();

			user.Profile.SetCustomProperty(RelevateProfileItem.ContactIdFieldName, relevateUser.ContactId.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.FirstNameFieldName, relevateUser.FirstName.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.LastNameFieldName, relevateUser.LastName.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.EmailFieldName, relevateUser.Email.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.ZipCodeFieldName, relevateUser.ZipCode.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.PromoCodeFieldName, relevateUser.PromoCode.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.ChannelFieldName, relevateUser.Channel.Trim());
			user.Profile.SetCustomProperty(RelevateProfileItem.SegmentNameFieldName, relevateUser.SegmentName.Trim());

			user.Profile.Save();
		}

		/// <summary>
		/// Adds the desginated domain prefix to the user name or domain name.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		private string AddDomain(string userName_or_domainName)
		{
			if (string.IsNullOrEmpty(userName_or_domainName) || string.IsNullOrEmpty(RelevateSettings.RelevateUserDomain))
			{
				// throw an exception on purpose here when we try to 
				// do something with a null string so we don't make 
				// a bad user
				return null;
			}
			return string.Format("{0}\\{1}", RelevateSettings.RelevateUserDomain, userName_or_domainName);
		}
	}
}
