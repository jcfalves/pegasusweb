using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class StockTransitResult
    {
        public string Fl_Situacao
        {
            get;
            set;
        }

        public string Dt_Inicio_Processamento
        {
            get;
            set;
        }

        public string Dt_Fim_Processamento
        {
            get;
            set;
        }

        public string Qt_Registro_Lido
        {
            get;
            set;
        }

        public string Qt_Registro_Rejeitado
        {
            get;
            set;
        }

        public string Qt_Registro_Gravado
        {
            get;
            set;
        }
    }
}
