using System;
using System.Collections.Generic;
using System.Text;

namespace Bayer.Pegasus.Entities
{
    public class TokenSAML
    {
        public string Token {
            get;
            set;
        }

        public DateTime Updated {
            get;
            set;
        }

        public bool IsUpdated {
            get {
                if ((System.DateTime.Now - Updated).Minutes > 15)
                {
                    return false;
                }
                else
                {
                    return true;
                }   
            }
        }

        public TokenSAML(string token) {
            this.Token = token;
            this.Updated = System.DateTime.Now;
        }
    }
}
