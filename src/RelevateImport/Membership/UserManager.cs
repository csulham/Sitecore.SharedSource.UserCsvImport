﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using RelevateImport.CSVParser;
using RelevateImport.CustomItems.CustomProfiles;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Security.Accounts;

namespace RelevateImport.Membership
{
	public class UserManager
	{
		private readonly string _roleName;
		private readonly string _identityFieldName;
		private readonly string _emailFieldName;
		private string[] _fullNameFieldNames;
		private Item _customProfileItem;
		private Sitecore.Security.Accounts.Role UserRole { get; set; }

		public UserManager(string role, Item profile)
		{
			if (string.IsNullOrEmpty(role))
			{
				throw new ArgumentException("Role must be specified.");
			}
			if (profile == null)
			{
				throw new ArgumentException("Custom profile Item must be provided.");
			}
			_roleName = AddDomain(role);
			_customProfileItem = profile;
		}

		public void CreateUsers(List<CsvUser> users)
		{
			foreach (CsvUser user in users)
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

		private void CreateUser(CsvUser csvUser)
		{
			UserRole = GetOrCreateRole();

			if (UserRole == null)
			{
				throw new MembershipCreateUserException("Could not get role " + _roleName);
			}

			User membershipUser = GetOrCreateUser(csvUser);
			if (membershipUser == null)
			{
				throw new MembershipCreateUserException(string.Format("Error creating user {0}, {1}", csvUser.Identity, csvUser.Email));
			}

			// Add Role
			if (!membershipUser.IsInRole(UserRole))
			{
				membershipUser.Roles.Add(UserRole);
			}

			// Update Profile
			UpdateProfile(membershipUser, csvUser);
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

		private User GetOrCreateUser(CsvUser csvUser)
		{
			string userName = AddDomain(csvUser.Identity);
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
				newUser.Profile.ProfileItemId = _customProfileItem.ID.ToString();
				newUser.Profile.Save();
				return newUser;
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("Error creating user {0}, {1}", csvUser.Identity, csvUser.Email), ex, this);
				throw;
			}
		}


		public IEnumerable<string> GetProfileFields()
		{
			Template profileTemplate = TemplateManager.GetTemplate(_customProfileItem);
			return profileTemplate.GetFields(false).Select(f => f.Name);
		}

		private void UpdateProfile(User user, CsvUser csvUser)
		{
			user.Profile.Email = csvUser.Email;
			user.Profile.FullName = csvUser.Name;

			foreach (string profileField in GetProfileFields())
			{
				if (string.IsNullOrEmpty(profileField))
				{
					continue;
				}

				if (!csvUser.ProfileProperties.ContainsKey(profileField))
				{
					Log.Warn(string.Format("User profile property {0} was not set for user {1} {2}.", profileField, csvUser.Identity, csvUser.Name), this);
				}
				user.Profile.SetCustomProperty(profileField, csvUser.ProfileProperties[profileField]);
			}
			
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
