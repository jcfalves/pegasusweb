using System.Runtime.Serialization;

namespace Bayer.Pegasus.Entities
{
    [DataContract(Name = "Retroativo")]
    public class Retroativo
    {
        public Retroativo()
        {

        }
        [DataMember(Name = "idArquivoretroativo")]
        public int idArquivoretroativo { get; set; }

        [DataMember(Name = "idAcao")]
        public int idAcao { get; set; }

        [DataMember(Name = "dsAcao")]
        public string dsAcao { get; set; }

        [DataMember(Name = "dsNome")]
        public string dsNome { get; set; }

        [DataMember(Name = "dsStatus")]
        public string dsStatus { get; set; }

        [DataMember(Name = "dtAcao")]
        public string dtAcao { get; set; }

    }
}
