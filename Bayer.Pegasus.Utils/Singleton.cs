using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Bayer.Pegasus.Utils
{
    [System.Diagnostics.DebuggerStepThrough()]
    public abstract class Singleton<T> where T : new()
    {
        protected Singleton() { }

        private static T _instance;


        

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new T();

                return _instance;
            }
        }
    }
}
