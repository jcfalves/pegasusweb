using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class CFOP
    {
        public string Code
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public bool Credit
        {
            get;
            set;
        }

        public bool Debit
        {
            get;
            set;
        }

        public int OperationType
        {
            get;
            set;
        }

        public object DataObject { get; set; }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Description))
                return Code;

            return Code + " - " + Description;
        }
    }
}
