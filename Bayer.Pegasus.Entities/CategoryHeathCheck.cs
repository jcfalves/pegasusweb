using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    [DataContract(Name = "Categoria_Erro")]
    public class CategoryHeathCheck
    {
        public CategoryHeathCheck()
        {

        }
        [DataMember(Name = "Id_Categoria_Erro")]
        public int CategoryId { get; set; }

        [DataMember(Name = "Ds_Categoria_Erro")]
        public string Description { get; set; }
    }
}