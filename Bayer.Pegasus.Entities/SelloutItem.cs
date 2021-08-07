using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class SelloutItem
    {

        public Partner Partner
        {
            get;
            set;
        }


        public Partner Unit
        {
            get;
            set;
        }

        public Product Product
        {
            get;
            set;
        }

        public string FiscalCode
        {
            get;
            set;

        }

        public string FiscalIssuing
        {
            get;
            set;
        }

        public string FiscalIssuingCnpj
        {
            get;
            set;
        }
        
        public DateTime FiscalDate {
            get;
            set;
        }

        public string Transaction {
            get;
            set;
        }

        public Customer Customer {
            get;
            set;
        }

        public City City {
            get;
            set;
        }

        public decimal Quantity {
            get;
            set;
        }

        public CFOP CFOP {
            get;

            set;
        }

        public Dictionary<string, decimal> Values
        {
            get;
            set;
        }



        public DateTime UpdateDate {
            get;
            set;
        }

        
    }
}
