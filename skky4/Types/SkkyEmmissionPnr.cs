using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace skky.Types
{
    public class SkkyEmmissionPnr
    {
        private string recordLocator = "";
        public string RecordLocator
        {
            get { return recordLocator; }
            set { recordLocator = value; }
        }

        private string accountNumber = "";
        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }

        private string departmentNumber = "";
        public string DepartmentNumber
        {
            get { return departmentNumber; }
            set { departmentNumber = value; }
        }

        private string psgrFirstName = "";
        public string PsgrFirstName
        {
            get { return psgrFirstName; }
            set { psgrFirstName = value; }
        }

        private string psgrLastName = "";
        public string PsgrLastName
        {
            get { return psgrLastName; }
            set { psgrLastName = value; }
        }
        
        private List<SkkySegment> segments = new List<SkkySegment>();
        public List<SkkySegment> Segments
        {
            get { return segments; }
            set { segments = value; }
        }

        public void Clear()
        {
            recordLocator = "";
            accountNumber = "";
            psgrFirstName = "";
            psgrLastName = "";
            segments.Clear();
        }

        public bool HasSegmentsOfType(int segType)
        {
            bool rc = false;

            foreach (SkkySegment seg in segments)
            {
                if (seg.SegmentType == segType)
                {
                    rc = true;
                    break;
                }
            }

            return rc;
        }
       
    }
}
