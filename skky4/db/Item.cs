using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace skky.db
{
	public partial class Item
	{
		public Item(int itemId)
		{
			this.id = itemId;
		}

		static public IEnumerable<Item> GetAllFromCustomer(ObjectsDataContext db, int customerID)
		{
			return from it in db.Items
				   where it.ItemType.idCustomer == customerID
				   orderby it.Name
				   select it;
		}
		static public List<Item> GetAllItems(int customerID)
		{
			using (var db = new ObjectsDataContext())
			{
				IEnumerable<Item> list = GetAllFromCustomer(db, customerID);
				if (list != null && list.Count() > 0)
					return list.ToList();
			}

			return new List<Item>();
		}

		public static void SaveItem(int clientID, Item item)
		{
			if (item == null)
				throw new Exception("NULL item passed to SaveItem.");

			item.Save(clientID);
		}

		public void Remove(int clientID)
		{
			using (var db = new ObjectsDataContext())
			{
				var item = db.Items.SingleOrDefault(i => i.id == this.id);
				db.Items.DeleteOnSubmit(item);

				db.SubmitChanges();
			}
		}

		public void Save(int clientID)
		{
			if (string.IsNullOrEmpty(this.Name))
				throw new Exception("The name of an Item must have a value.");

			using (var db = new ObjectsDataContext())
			{
				Item item = null;
				if (id > 0)
				{
					item = (from it in db.Items
							where it.id == id
							select it).Single();
					if (item != null)
					{
						item.Name = Name;
						item.Notes = Notes;
					}
				}
				else
				{
					db.Items.InsertOnSubmit(this);
				}

				db.SubmitChanges();
			}
		}
	}
}
