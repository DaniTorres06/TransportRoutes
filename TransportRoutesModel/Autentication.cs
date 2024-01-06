using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportRoutesModel
{
    public class Autentication
    {
        public string? Token { get; set; }

        public Autentication() 
        {
            Token = string.Empty;
        }
    }
}
