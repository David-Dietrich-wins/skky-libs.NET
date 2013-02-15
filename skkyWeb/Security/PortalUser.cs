using System;
using System.Web.UI;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Threading;
using System.Web.Profile;
using System.Web.Security;
using skky.Types;
using skky.db;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace skkyWeb.Security
{
	/*	[DebuggerDisplay("DisplayName = {DisplayName}, Customer = {Customer.Name}")] */

	/// <summary>
	///  Be sure to call IsEnabled to load the user information.
	/// </summary>
	[DataContract]
	public class PortalUser : MembershipUser
	{
		internal PortalUser()
		{ }

		internal PortalUser(MembershipUser member, List<string> roles) :
			base(member.ProviderName, member.UserName, member.ProviderUserKey, member.Email, member.PasswordQuestion, member.Comment,
			member.IsApproved, member.IsLockedOut, member.CreationDate, member.LastLoginDate, member.LastActivityDate, member.LastPasswordChangedDate,
			member.LastLockoutDate)
		{
			Roles = roles ?? new List<string>();
		}

		public PortalUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate, List<string> roles) :
			base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
		{
			Roles = roles ?? new List<string>();
		}

		[DataMember]
		public string DefaultCustomerName { get; set; }

		public Client Client { get; private set; }
		public List<Customer> Customers { get; private set; }
		public Customer DefaultCustomer { get; private set; }

		private User skkydbUser;
		public User skkyUser
		{
			get
			{
				if (skkydbUser == null)
				{
					Client = null;
					DefaultCustomer = null;
					Customers = null;

					User user = null;
					Client client = null;
					List<Customer> customerList = new List<Customer>();
					Customer defaultCustomer = null;

					try
					{
						using (var db = new ObjectsDataContext())
						{
							user = db.Users.First(finduser => finduser.idaspnet_Users == ASPnetUserID);

							if (user != null && user.CustomerUsers.Count() > 0)
							{
								foreach (var customerUser in user.CustomerUsers)
								{
									var cust = customerUser.Customer;
									customerList.Add(cust);

									if (cust.Name.ToLower() == DefaultCustomerName.ToLower())
									{
										defaultCustomer = cust;
									}
								}

								if (defaultCustomer == null)
									defaultCustomer = customerList[0];

								if (defaultCustomer != null)
									client = defaultCustomer.Client;
							}
						}
					}
					catch (Exception ex)
					{
						Trace.WriteLine("Error retrieving user: " + ex.Message);
						throw;
					}

					if (user == null)
						throw new Exception("No skkyUser found");
					//					skkyuser = new User();

					if (customerList.Count() < 1)
						throw new Exception("No Customers Found");

					if (defaultCustomer == null)
						throw new Exception("No Default Customer Found");

					if (client == null)
						throw new Exception("No Client Found");

					skkydbUser = user;
					Customers = customerList;
					DefaultCustomer = defaultCustomer;
					Client = client;
				}

				return skkydbUser;
			}

			private set
			{
				skkydbUser = value;
			}
		}

		public Customer Customer
		{
			get
			{
				return DefaultCustomer;
			}
		}
		public int CustomerID
		{
			get
			{
				return Customer.id;
			}
		}
		public string CustomerName
		{
			get
			{
				return Customer.Name;
			}

			private set { }
		}
		public List<string> CustomerNames
		{
			get
			{
				List<string> list = new List<string>();
				foreach(var cust in Customers)
					list.Add(cust.Name);

				return list;
			}
		}

		public int UserID
		{
			get
			{
				return skkyUser.id;
			}
		}

		public string FirstName
		{
			get
			{
				return skkyUser.FirstName;
			}
		}
		public string LastName
		{
			get
			{
				return skkyUser.LastName;
			}
		}

		public string DisplayName
		{
			get
			{
				string str = FirstName ?? string.Empty;
				if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
					str += " ";

				str += LastName;
				if (string.IsNullOrEmpty(str))
					str = Email;

				return str;
			}

			private set { }
		}
		public string DisplayNameWithCustomer
		{
			get
			{
				return CustomerName + " - " + DisplayName;
			}
		}

		public Guid ASPnetUserID
		{
			get
			{
				return (Guid)base.ProviderUserKey;
			}
		}

		public List<string> Roles { get; private set; }

		public bool IsInRole(string role)
		{
			return Roles.Contains(role);
		}

		public bool IsSystemAdmin
		{
			get
			{
				return (from r in Roles where r.Contains(RoleNames.SystemAdmin) select r).Count() > 0;
			}
		}
		public bool IsClientAdmin
		{
			get
			{
				return (IsSystemAdmin
					|| (from r in Roles where r.Contains(RoleNames.ClientAdmin) select r).Count() > 0);
			}
		}
		public bool IsCustomerAdmin
		{
			get
			{
				return (IsClientAdmin
					|| (from r in Roles where r.Contains(RoleNames.CustomerAdmin) select r).Count() > 0);
			}
		}

		public bool IsReadOnly
		{
			get { return (from r in Roles where r.EndsWith(UserController.Const_ReadOnly) select r).Count() > 0; }
		}

		private bool IsUserAspEnabled()
		{
			return (!base.IsLockedOut && base.IsApproved && Roles.Count() > 0);
		}
		private bool IsUserSkkyEnabled()
		{
			return (skkyUser.Status > 0 && Customer.Status > 0 && Client.Status > 0);
		}
		public bool IsEnabled
		{
			get
			{
				return (IsUserAspEnabled() && IsUserSkkyEnabled());
			}
		}

		int maxCustomerNameLength = 15;
		string displayRole;
		public string DisplayRole
		{
			get
			{
				if (String.IsNullOrEmpty(displayRole))
				{
					displayRole = Customer.Name;

					if (displayRole.Length > maxCustomerNameLength)
						displayRole = displayRole.Substring(0, maxCustomerNameLength) + "...";

					if (this.IsSystemAdmin)
						displayRole = "Admin: " + displayRole;
				}

				return displayRole;
			}
		}

		private UserWebSettings settings;
		[DataMember]
		public UserWebSettings Settings
		{
			get
			{
				if (settings == null)
					settings = new UserWebSettings();

				return settings;
			}

			set
			{
				settings = value;
			}
		}
	}
}
