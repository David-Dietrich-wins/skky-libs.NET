using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace skky.db
{
    public partial class Pnr
    {
        public static int AddPnr(string locator, string accountNumber, string departmentNumber, string psgrFirstName, string psgrLastName, int gds)
        {
            Pnr pnr = QueryPnr(locator, gds);
            if (null == pnr)
            {
                using (var db = new ObjectsDataContext())
                {
                    pnr = new Pnr();
                    pnr.Locator = locator.ToUpper();
                    pnr.AccountNumber = accountNumber;

                    Passenger psgr = new Passenger()
                    {
                        LastName = psgrLastName,
                        FirstName = psgrFirstName,
                        DeptCode = departmentNumber,
                        PsgrNum = 1,
                        Remark = "",
                        FirstNameNum = 1,
                        LastNameNum = 1,
                        Title = "",
                        Type = 1,
                    };
                    pnr.Passengers.Add(psgr);

                    pnr.PsgrFirstName = psgrFirstName;
                    pnr.PsgrLastName = psgrLastName;
                    pnr.GDS = Convert.ToByte(gds);
                    db.Pnrs.InsertOnSubmit(pnr);
                    db.Passengers.InsertAllOnSubmit(pnr.Passengers);
                    db.SubmitChanges();
                }
                pnr = QueryPnr(locator, gds);
            }
            return pnr.id;
        }

        public static Pnr QueryPnr(string locator, int gds)
        {
            using (var db = new ObjectsDataContext())
            {
                var result = from foundPnr in db.Pnrs
                             where foundPnr.Locator == locator.ToUpper() && foundPnr.GDS == (byte)gds
                             select foundPnr;
                if (result.Count() > 0)
                    return result.First();
            }
            return null;
        } 
    }
}
