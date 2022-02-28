using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQ5EDML.Componentes
{
    class Error
    {
        private int tipo, indice, linea;
        private string descripcion;

        public Error(int t, int i, int l, string d)
        {
            Tipo = t;
            Indice = i;
            Linea = l;
            Descripcion = d;
        }

        public int Tipo { get => tipo; set => tipo = value; }
        public int Indice { get => indice; set => indice = value; }
        public int Linea { get => linea; set => linea = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
    }
}