using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQ5EDML.Componentes
{
    class Prueba
    {
        string x, k, cadena;

        public Prueba(string x, string cadena, string k)
        {
            X = x;
            Cadena = cadena;
            K = k;
        }

        public string X { get => x; set => x = value; }
        public string K { get => k; set => k = value; }
        public string Cadena { get => cadena; set => cadena = value; }
    }
}