using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class ObjectsDataContext
	{
		public ObjectsDataContext(Client client)
			: this()
			//	Don't have a connection string for multiple clients
			//			: this(client.Name)
		{
			this.Client = client;
		}


		public System.Linq.IQueryable<Client> DisplayableCompanies
		{
			get
			{
				return Clients.Where(c => c.Name != string.Empty && c.id > 0);
			}
		}

		public Client Client { get; private set; }
	}
}
