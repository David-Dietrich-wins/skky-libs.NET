using System;
using System.Web;
using System.Web.Security;
using System.Threading;
using System.Web.Profile;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using skky.Types;
using skkyWeb.Charts;
using skky.db;

namespace skkyWeb.Security
{
	public static class RoleNames
	{
		public const string SystemAdmin = "System/Admin";
		public const string SystemUser = "System/User";
		public const string SystemReadOnly = "System/ReadOnly";

		public const string ClientAdmin = "Client/Admin";
		public const string ClientUser = "Client/User";
		public const string ClientReadOnly = "Client/ReadOnly";

		public const string CustomerAdmin = "Customer/Admin";
		public const string CustomerUser = "Customer/User";
		public const string CustomerReadOnly = "Customer/ReadOnly";

		public const string LowestPermissions = CustomerReadOnly;
	}

	public class UserDictionary : Dictionary<string, List<PortalUser>>
	{
		public const string Const_All = "All";

		static object padlock = new object();

		public UserDictionary()
		{ }

		public List<PortalUser> GetAllUsers()
		{
			lock (padlock)
			{
				if (!ContainsKey(Const_All))
				{
					Add(Const_All, UserController.GetAllPortalUsers());
				}

				return this[Const_All];
			}
		}

		public List<PortalUser> GetPortalUsersByCustomer(string customerName)
		{
			lock (padlock)
			{
				if (!ContainsKey(customerName))
				{
					var customerUsers = (from u in GetAllUsers()
										 where (customerName == "*** Administrators ***" && u.IsCustomerAdmin)
										 || (customerName == "*** Disabled Users ***" && !u.IsApproved)
										 || u.CustomerNames.Contains(customerName)
										 select u)
										 .ToList();

					Add(customerName, customerUsers);
				}

				return this[customerName];
			}
		}

		public List<PortalUser> GetPortalUsersByCustomerId(int customerId)
		{
			lock (padlock)
			{
				if (!ContainsKey(customerId.ToString()))
				{
					var customerUsers = (from u in GetAllUsers()
										 where u.CustomerID == customerId && !u.IsSystemAdmin
										 select u)
										 .ToList();

					Add(customerId.ToString(), customerUsers);
				}

				return this[customerId.ToString()];
			}
		}

		public void CleanUserCollection()
		{
			lock (padlock)
				Clear();
		}
	}

	public static class UserController
	{
		public const string DefaultCustomer = "GrayArrow Consulting";

		public const string Const_Admin = "Admin";
		public const string Const_Client = "Client";
		public const string Const_Customer = "Customer";
		public const string Const_ReadOnly = "ReadOnly";
		public const string Const_System = "System";
		public const string Const_User = "User";

		static UserController()
		{
			InitializeRoles();
		}

		private static UserDictionary userDictionary;
		public static UserDictionary UserDictionary
		{
			get
			{
				if (userDictionary == null)
					userDictionary = new UserDictionary();

				return userDictionary;
			}
		}

		public static void CleanUserDictionary()
		{
			UserDictionary.CleanUserCollection();
		}

		public static List<PortalUser> GetAllPortalUsers()
		{
			List<PortalUser> userlist = new List<PortalUser>();
			MembershipUserCollection rawUsers = Membership.GetAllUsers();

			foreach (MembershipUser member in rawUsers)
			{
				var user = FindUser(member.UserName, false);
				if (user != null && user.Customers != null)
				{
					if (user.Customers.Count() > 0)
						userlist.Add(user);
				}
			}

			return userlist;
		}

		public static bool IsValidUser(string userName, string password)
		{
			return Membership.ValidateUser(userName, password);
		}
		public static MembershipUser FindMembershipUser(string userName, string password)
		{
			if (IsValidUser(userName, password))
				return FindMembershipUser(userName);

			return null;
		}
		public static Guid? FindProviderUserKey(string userName, string password)
		{
			MembershipUser mu = FindMembershipUser(userName, password);
			if (mu != null)
				return (mu.ProviderUserKey as Guid?);

			return null;
		}

		public static MembershipUser FindMembershipUser(Guid providerKey)
		{
			if (providerKey == null)
				throw new Exception("Empty Provider Key passed to FindMembershipUser.");

			return Membership.GetUser(providerKey);
		}
		public static MembershipUser FindMembershipUser(string userName)
		{
			if (string.IsNullOrEmpty(userName))
				throw new Exception("Empty UserName passed to FindUser.");

			return Membership.GetUser(userName);
		}
		private static PortalUser FromMembershipUser(MembershipUser user)
		{
			PortalUser portalUser = null;

			if (user != null)
			{
				var roles = new List<string>(System.Web.Security.Roles.GetRolesForUser(user.UserName));

				portalUser = new PortalUser(user, roles);
				//portalUser = new PortalUser(user.ProviderName, user.UserName, user.ProviderUserKey, user.Email, user.PasswordQuestion
				//     , user.Comment, user.IsApproved, user.IsLockedOut, user.CreationDate, user.LastLoginDate, user.LastActivityDate
				//     , user.LastPasswordChangedDate, user.LastLockoutDate);

				// Objects have problems loading in services.
				// So let's isolate every object get from the profile.
				ProfileBase profile = null;
				try
				{
					profile = ProfileBase.Create(user.UserName);
					portalUser.DefaultCustomerName = (string)profile["CustomerName"];
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.Write("Error saving User Profile Base.\n" + ex);
				}

				try
				{
					portalUser.Settings = (UserWebSettings)profile["UserWebSettings"];
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.Write("Error retrieving User WebSettings.\n" + ex);
				}
			}

			return portalUser;
		}
		public static PortalUser FindUserRaw(Guid providerKey)
		{
			MembershipUser user = FindMembershipUser(providerKey);
			return FromMembershipUser(user);
		}
		public static PortalUser FindUserRaw(string userName)
		{
			MembershipUser user = FindMembershipUser(userName);
			return FromMembershipUser(user);
		}
		public static PortalUser FindExistingUser(string userName)
		{
			PortalUser portalUser = FindUserRaw(userName);

			if (portalUser == null)
				throw new Exception(string.Format("Unable to find portal user: {0}.", userName));

			return portalUser;
		}
		public static PortalUser FindExistingUser(Guid providerKey)
		{
			PortalUser portalUser = FindUserRaw(providerKey);

			if (portalUser == null)
				throw new Exception(string.Format("Unable to find portal user: {0}.", providerKey.ToString()));

			return portalUser;
		}
		public static PortalUser FindUser(string userName, bool mustBeEnabled)
		{
			PortalUser portalUser = FindExistingUser(userName);

			bool isEnabled = portalUser.IsEnabled;
			if (mustBeEnabled && !isEnabled)
				throw new Exception(string.Format("Portal user: {0} is not enabled.", userName));

			return portalUser;
		}
		public static PortalUser GetUserAndCreateDefaultName(string userName)
		{
			PortalUser portalUser = FindUser(userName, true);

			if (string.IsNullOrEmpty(portalUser.DefaultCustomerName))
			{
				try
				{
					portalUser.DefaultCustomerName = portalUser.DefaultCustomer.Name;
					SavePortalUserProfile(userName, portalUser.DefaultCustomerName, portalUser.Settings);
				}
				catch (Exception ex)
				{
					skky.util.Trace.Warning("Saving Portal User in GetUserByName", ex);
				}
			}

			return portalUser;
		}

		public static PortalUser CurrentPortalUser
		{
			get
			{
				PortalUser portalUser = null;
				HttpContext curHttp = HttpContext.Current;
				if (curHttp != null)
				{
					if (curHttp.Items["PortalUser"] != null)
						portalUser = curHttp.Items["PortalUser"] as PortalUser;

					if (portalUser == null)
					{
						if (curHttp.User.Identity.IsAuthenticated)
						{
							portalUser = GetUserAndCreateDefaultName(curHttp.User.Identity.Name);

							// Allows host to mimic any customer based functionality
							//if (portalUser.IsSystemAdmin)
							//{
							//    HttpCookie cookie = HttpContext.Current.Request.Cookies[AuthenticationController.Const_DefaultCookieName];

							//    if (cookie != null && !string.IsNullOrEmpty(cookie["ImpersonateCustomer"]))
							//        portalUser.DefaultCustomerName = cookie["ImpersonateCustomer"];
							//}
						}

						curHttp.Items["PortalUser"] = portalUser;
					}
				}

				return portalUser;
			}
			set
			{
				HttpContext curHttp = HttpContext.Current;
				if (curHttp != null)
				{
					if (curHttp.Items["PortalUser"] == null)
						HttpContext.Current.Items["PortalUser"] = value;
				}
			}
		}

		private const int MaxCustomerAdmins = 2;
		private const int MaxCustomerUsers = 8;

		private static bool AreCustomerAdminAccountsAvailable(string customerName)
		{
			return true;
			//return GetPortalUsersByCustomer(customerName).Where(u => u.IsCustomerAdmin).Count() < MaxCustomerAdmins;
		}

		private static bool AreCustomerUserAccountsAvailable(string customerName)
		{
			return true;
			//return GetPortalUsersByCustomer(customerName).Where(u => !u.IsCustomerAdmin).Count() < MaxCustomerUsers;
		}

		public static PortalUser CreatePortalUser(UserFormData form)
		{
			if (string.IsNullOrEmpty(form.Password))
				throw new ApplicationException("New users must have a password.");

			CheckAccountLimit(form.Roles, form.CustomerName);

			try
			{
				MembershipCreateStatus status;
				var user = new PortalUser(Membership.CreateUser(form.Email, form.Password, form.Email, "Question?", "Answer", form.IsApproved, out status), form.Roles)
					{
						//FirstName = form.FirstName,
						//LastName = form.LastName,
						DefaultCustomerName = form.CustomerName,
					};

				//roles
				SaveMembershipRoles(form.Email, form.Roles);

				//profile
				SavePortalUserProfile(form.Email, form.CustomerName, null);

				return user;
			}
			finally
			{
				CleanUserDictionary();
			}
		}

		private static MembershipUser SaveMembershipUser(UserFormData form)
		{
			MembershipUser user = null;
			try
			{
				user = FindMembershipUser(form.Email);
				if (user == null)
				{
					MembershipCreateStatus status;
					user = Membership.CreateUser(form.Email, form.Password, form.Email, "Question?", "Answer", form.IsApproved, out status);
					if (user == null)
					{
						if (status == MembershipCreateStatus.InvalidPassword)
							throw new Exception("Invalid Password");
						else
							throw new Exception("Error adding Membership user. Error status code: " + status.ToString());
					}
				}
				else
				{
					// TODO: Need a work around to change the user name.
					//user.UserName = form.Email;
					user.Email = form.Email;
					user.IsApproved = form.IsApproved;
					string generatedPassword = user.ResetPassword();
					user.ChangePassword(generatedPassword, form.Password);

					Membership.UpdateUser(user);
				}

				SaveMembershipRoles(user.UserName, form.Roles);
			}
			catch (Exception e)
			{
				skky.util.Trace.Error(e);
				throw;
			}

			return user;
		}

		public static PortalUser SavePortalUser(PortalUser lastUser, UserFormData form)
		{
			if (string.IsNullOrEmpty(form.Password))
				throw new ApplicationException("Users must have a password.");

			using (var db = new ObjectsDataContext())
			{
				// Should always have a customer.
				Customer customer = db.Customers.First(customr => customr.Name == form.CustomerName);
				if (customer == null)
					throw new Exception("Cannot save portal user without valid Customer.");

				MembershipUser memUser = SaveMembershipUser(form);
				Guid aspNetUserID = (Guid)memUser.ProviderUserKey;
				User user = null;

				var fuser = from usr in db.Users
							where usr.idaspnet_Users == aspNetUserID
							select usr;
				if(fuser.Any())
					user = fuser.First();

				if (user == null)
				{
					user = new User()
					{
						FirstName = form.FirstName,
						LastName = form.LastName,
						idaspnet_Users = aspNetUserID,
						Status = form.IsApproved ? 1 : 0,
						Startdate = DateTime.Now,
						Stopdate = new DateTime(2020, 10, 10),
						Address1 = "",
						Address2 = "",
						City = "",
						Country = "",
						Description = "",
						Phone1 = "",
						Phone2 = "",
						State = "",
						ZipCode = "",
					};

					db.Users.InsertOnSubmit(user);
				}
				else
				{
					user.FirstName = form.FirstName;
					user.LastName = form.LastName;
					user.Status = form.IsApproved ? 1 : 0;
				}

				CustomerUser customerUser = null;
				var custuser = from cst in db.CustomerUsers
							   where cst.idCustomer == customer.id && cst.idUsers == user.id
							   select cst;
				if (custuser.Any())
					customerUser = custuser.First();

				if (customerUser == null)
				{
					customerUser = new CustomerUser();

					customerUser.Customer = customer;
					customerUser.User = user;

					db.CustomerUsers.InsertOnSubmit(customerUser);
				}
				else
				{
					customerUser.Customer = customer;
					customerUser.User = user;
				}

				try
				{
					db.SubmitChanges();
					CleanUserDictionary();
				}
				catch (Exception ex)
				{
					skky.util.Trace.Warning(ex);
					//Membership.DeleteUser();
				}
			}

			SavePortalUserProfile(form.Email, form.CustomerName, null);

			return FindUser(form.Email, true);
		}

		public static void SavePortalUser(PortalUser user, string password)
		{
			if (user.DefaultCustomerName != DefaultCustomer)
				CheckAccountLimit(user.Roles, user.DefaultCustomerName);

			try
			{
				if (!string.IsNullOrEmpty(password))
				{
					string generatedPassword = user.ResetPassword();
					user.ChangePassword(generatedPassword, password);
				}

				Membership.UpdateUser(user);

				SaveMembershipRoles(user.UserName, user.Roles);

				SavePortalUserProfile(user.UserName, user.DefaultCustomerName, user.Settings);
			}
			finally
			{
				CleanUserDictionary();
			}
		}

		private static void CheckAccountLimit(List<string> newRoles, string customerName)
		{
			if (!newRoles.Contains(RoleNames.SystemAdmin)
			 && !newRoles.Contains(RoleNames.SystemUser)
			 && !newRoles.Contains(RoleNames.SystemReadOnly))
			{
				if (newRoles.Where(r => r.EndsWith(Const_Admin)).Count() > 0 && !AreCustomerAdminAccountsAvailable(customerName))
					throw new ApplicationException("Maximum Number of administrator accounts have been exceeded [" + MaxCustomerAdmins + "]");
				else if (!AreCustomerUserAccountsAvailable(customerName))
					throw new ApplicationException("Maximum Number of user accounts have been exceeded [" + MaxCustomerUsers + "]");
			}
		}

		public static void ClearWebSettings(PortalUser user)
		{
			if (user != null)
			{
				user.Settings = null;
				SavePortalUserProfile(user);
			}
		}

		public delegate void SavePortalUserHandler(object sender, PortalUser portalUser);

		public static void SavePortalUserProfile(PortalUser user)
		{
			SavePortalUserProfile(user.UserName, user.DefaultCustomerName, user.Settings);
		}

		public static void SavePortalUserProfile(string userName, string defaultCustomerName, UserWebSettings webSettings)
		{
			ProfileBase userProfile = ProfileBase.Create(userName);
			userProfile["CustomerName"] = (defaultCustomerName ?? string.Empty);

			if (webSettings == null)
				webSettings = new UserWebSettings();

			webSettings.PreSave();
			userProfile["UserWebSettings"] = webSettings;

			userProfile.Save();
		}

		public static void DeletePortalUser(MembershipUser user)
		{
			Membership.DeleteUser(user.UserName);

			CleanUserDictionary();
		}

		public static void SaveMembershipRoles(string userName, List<string> roles)
		{
			if (roles == null)
				roles = new List<string>();

			var userRoles = Roles.GetRolesForUser(userName);

			if (userRoles != null && userRoles.Length > 0)
				Roles.RemoveUserFromRoles(userName, userRoles);

			var q = from role in roles
					where !Roles.GetAllRoles().Contains(role)
					select role;

			foreach (var role in q)
				Roles.CreateRole(role);

			if (roles.Count > 0)
				Roles.AddUserToRoles(userName, roles.ToArray());
		}

		public static Dictionary<string, List<string>> KeyedSystemRoles { get; private set; }
		public static List<string> AllSystemRoles
		{
			get { return (from sr in KeyedSystemRoles from role in sr.Value select role).ToList(); }
		}

		public static List<string> ClientRoles
		{
			get { return (from role in KeyedSystemRoles[Const_Client] select role).ToList(); }
		}
		public static List<string> CustomerRoles
		{
			get { return (from role in KeyedSystemRoles[Const_Customer] select role).ToList(); }
		}
		public static List<string> SystemRoles
		{
			get { return (from role in KeyedSystemRoles[Const_System] select role).ToList(); }
		}

		private static void InitializeRoles()
		{
			KeyedSystemRoles = new Dictionary<string, List<string>>();
			KeyedSystemRoles.Add(Const_Client, new List<String> { RoleNames.ClientAdmin, RoleNames.ClientUser, RoleNames.ClientReadOnly });
			KeyedSystemRoles.Add(Const_Customer, new List<String> { RoleNames.CustomerAdmin, RoleNames.CustomerUser, RoleNames.CustomerReadOnly });
			KeyedSystemRoles.Add(Const_System, new List<string> { RoleNames.SystemAdmin, RoleNames.SystemUser, RoleNames.SystemReadOnly });

			var allRoles = Roles.GetAllRoles();

			//Makes sure all roles exist
			foreach (var role in AllSystemRoles.Where(x => !allRoles.Contains(x)))
				Roles.CreateRole(role);

			//Makes sure admin still have access with new role structure.
			if (Roles.RoleExists(Const_Admin) && Roles.GetUsersInRole(Const_Admin).Count() > 0)
				Roles.AddUsersToRoles(Roles.GetUsersInRole(Const_Admin), new string[] { RoleNames.SystemAdmin });

			//makes sure all obsolete roles are removed
			foreach (var role in allRoles.Where(x => !AllSystemRoles.Contains(x)))
			{
				if (Roles.GetUsersInRole(role).Count() > 0)
					Roles.RemoveUsersFromRoles(Roles.GetUsersInRole(role), new string[] { role });

				Roles.DeleteRole(role);
			}
		}
	}

	public class UserFormData
	{
		public bool IsApproved { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CustomerName { get; set; }
		public List<string> Roles { get; set; }
	}
}
