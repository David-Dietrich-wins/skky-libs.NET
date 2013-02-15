using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
	public partial class ItemType : ClientDataContext, IDisposable
	{
		public ItemType(int clientID)
		{
			this.idCustomer = clientID;
		}

		public static List<ItemType> GetAll(int clientID)
		{
			ItemType item = new ItemType(clientID);
			return item.GetAllItemTypes();
		}
		public List<ItemType> GetAllItemTypes()
		{
			if (this.idCustomer > 0)
			{
				using (var db = InitializeDataContext(this.idCustomer))
				{
					var list = from it in db.ItemTypes
							   orderby it.Name
							   select it;

					return list.ToList();
				}
			}

			return new List<ItemType>();
		}

		public static void SaveItemType(ItemType item)
		{
			if (item == null)
				throw new Exception("NULL ItemType passed to SaveItemType.");

			item.Save();
		}
		public void Remove()
		{
			using (var db = InitializeDataContext(this.idCustomer))
			{
				var item = db.ItemTypes.SingleOrDefault(i => i.id == this.id);
				db.ItemTypes.DeleteOnSubmit(item);

				db.SubmitChanges();
			}
		}
		public void Save()
		{
			if (string.IsNullOrEmpty(this.Name))
				throw new Exception("The type of an Item Type must have a value.");

			using (var db = InitializeDataContext(this.idCustomer))
			{
				ItemType item = null;
				if (id > 0)
				{
					item = (from it in db.ItemTypes
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
					db.ItemTypes.InsertOnSubmit(this);
				}

				db.SubmitChanges();
			}
		}
	}
}
