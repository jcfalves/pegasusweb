using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Bayer.Pegasus.Entities
{

    /// <summary>
    /// Product
    /// </summary>
    [DataContract]
    public class ProductPrice
    {

        public ProductPrice() {

        }
        public int id { get; set;}

        [DataMember(Name = "Nr_Linha")]
        public int NumberLine { get; set; }

        [DataMember(Name = "Sg_Moeda")]
        public string CoinTypes { get; set; }

        [DataMember(Name = "Nm_Diretoria")]
        public string BoardName { get; set; }

        [DataMember(Name = "Cd_Cluster")]
        public string CodeCluster { get; set; }

        [DataMember(Name = "Nm_Regional")]
        public string RegionalName { get; set; }

        [DataMember(Name = "Nm_Marca")]
        public string TradeMark { get; set; }
        [DataMember(Name = "Cd_Produto")]
        public string CodeProduct { get; set; }

        [DataMember(Name = "Nm_Produto")]
        public string NameProduct { get; set; }
        [DataMember(Name = "Vl_Produto")]
        public string ValueProduct { get; set; }

        [DataMember(Name = "Id_Processamento")]
        public long ProcessId { get; set; }

        [DataMember(Name = "Dt_Criacao")]
        public DateTime Created { get; set; }

        [DataMember(Name = "Fl_Tratamento")]        
        public bool FL_Processing { get; set; }

        [DataMember(Name = "Id_Categoria_Erro")]
        public int ErrorCategoryId { get; set; }
        [DataMember(Name = "Ds_Categoria_Erro")]
        public string DescriptionErrorCategory { get; set; }
        [DataMember(Name = "Cd_Erro")]
        public string CodeError { get; set; }
        [DataMember(Name = "Ds_Erro")]
        public string DescriptionError { get; set; }

    }
}
