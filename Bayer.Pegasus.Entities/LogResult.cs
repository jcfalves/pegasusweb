using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class LogResult
    {
        public int Id_Log_Processamento
        {
            get;
            set;
        }

        public int Id_Processamento
        {
            get;
            set;
        }

        public int Cd_Fase_Processamento
        {
            get;
            set;
        }

        public DateTime Dt_Inclusao
        {
            get;
            set;
        }

        public string Ds_Log_Processamento
        {
            get;
            set;
        }

        public string Fl_Tipo_Log
        {
            get;
            set;
        }
    }
}
