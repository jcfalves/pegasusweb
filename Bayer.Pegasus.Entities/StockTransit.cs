using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class StockTransit
    {

        public int id
        {
            get;
            set;
        }

        public string Id_Processamento
        {
            get;
            set;
        }

        public string Dt_Criacao
        {
            get;
            set;
        }

        public string Fl_Tratamento
        {
            get;
            set;
        }

        public string Cd_Erro
        {
            get;
            set;
        }

        public string Ds_Erro
        {
            get;
            set;
        }

        public int NumberLine
        {
            get;
            set;
        }

        public string NF
        {
            get;
            set;
        }

        public string IssuanceDate
        {
            get;
            set;
        }

        public string CD
        {
            get;
            set;
        }

        public string CDSAP
        {
            get;
            set;
        }

        public string NameRecipient
        {
            get;
            set;
        }

        public string CNPJRecipient
        {
            get;
            set;
        }

        public string CityRecipient
        {
            get;
            set;
        }

        public string UF
        {
            get;
            set;
        }

        public string ValueNF
        {
            get;
            set;
        }

        public string WeightNF
        {
            get;
            set;
        }

        public string ShippingCompany
        {
            get;
            set;
        }

        public string DeliveryDate
        {
            get;
            set;
        }

        public string StatusDelivery
        {
            get;
            set;
        }

        public string ComplementaryInformation
        {
            get;
            set;
        }
    }
}
