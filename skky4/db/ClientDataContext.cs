using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.Data.Linq;

using skky.util;

namespace skky.db
{
	public abstract class ClientDataContext
	{
		internal ObjectsDataContext clientDataContext;

		protected ObjectsDataContext InitializeDataContext(int clientID)
		{
			if (clientDataContext == null)
			{
				ObjectsDataContext db = new ObjectsDataContext();
				Client client = db.Clients.SingleOrDefault(c => c.id == clientID);

				if (client != null)
				{
					//clientDataContext = new ObjectsDataContext(client);
					clientDataContext = db;
					//clientDataContext.DeferredLoadingEnabled = true;
				}
				else
				{
					throw new Exception(string.Format("Unable to find a clientID for client ID {0}", clientID));
				}
			}

			return clientDataContext;
		}

		static protected int GetStartRow(int page, int rowsPerPage)
		{
			return page < 2 ? 1 : (page - 1) * rowsPerPage;
		}

		#region SubmitChanges
		protected void SubmitChanges(DataContext dataContext)
		{
			try
			{
				try
				{
					dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
				}
				catch (ChangeConflictException)
				{
					Trace.Information("Found conflict exceptions.");
					foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
					{
						if (!occ.IsDeleted)
							occ.Resolve(RefreshMode.KeepChanges);
					}
					dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
				}
				catch (Exception exception)
				{
					Trace.Critical(exception);
					dataContext.SubmitChanges(ConflictMode.ContinueOnConflict);
				}
			}
			catch (Exception exception)
			{
				Trace.Critical(exception);
				throw new FaultException<Exception>(exception, new FaultReason(exception.Message));
			}
		}
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (clientDataContext != null)
				clientDataContext.Dispose();
		}

		#endregion

	}
}
