using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EQ5EDML.Elementos
{
    class Token   
    {
        private string valor, simbolo, lineas;
        private int indice, linea, tipo;
        private List<string> producciones;

        public Token(string v, int i, int t, string s, int l)
        {
            Valor = v;
            Indice = i;
            Tipo = t;
            Simbolo = s;
            Linea = l;
        }

        public Token(string v, int i, int l, int t, string s)
        {
            Linea = l;
            Valor = v;
            Indice = i;
            Tipo = t;
            Simbolo = s;
        }

        public Token(string s, int i, List<string> p)
        {
            Producciones = p;
            Simbolo = s;
            Indice = i;
        }

        public string getProducciones() 
        {
            string cadena = "";
            foreach(string s in producciones)
            {
                cadena += s + " ";
            }
            return cadena;
        }

        public int Linea { get => linea; set => linea = value; }
        public string Valor { get => valor; set => valor = value; }
        public int Indice { get => indice; set => indice = value; }
        public string Simbolo { get => simbolo; set => simbolo = value; }
        public int Tipo { get => tipo; set => tipo = value; }
        public string Lineas { get => lineas; set => lineas = value; }
        public List<string> Producciones { get => Producciones1; set => Producciones1 = value; }
        public List<string> Producciones1 { get => producciones; set => producciones = value; }
    }
}