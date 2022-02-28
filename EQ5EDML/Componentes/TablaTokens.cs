using EQ5EDML.Elementos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQ5EDML.Componentes
{
    class TablaTokens
    {
        private List<Token> tokens;
        private List<Error> errores;
        private List<Prueba> pruebaDeEscritorio;

        private List<Token> tpalares; // 1. almacena las palabras reservadas
        private List<Token> tsintact; // 3. almacena las reglas sintacticas
        private List<Token> tidentif; // 4. almacena los identificadores
        private List<Token> tdelimit; // 5. almacena los delimitadores
        private List<Token> tconstan; // 6. almacena las constantes
        private List<Token> toperado; // 7. almacena los operadores comunes
        private List<Token> trelacio; // 8. almacena los operadores relacionales

        public TablaTokens(MatchCollection matches)
        {
            errores = new List<Error>();
            Tokens = new List<Token>();

            tpalares = new List<Token>(); // del 10 al 29
            Tsintact = new List<Token>(); // del 300 al 399
            Tidentif = new List<Token>(); // del 400 al 599
            tdelimit = new List<Token>(); // del -
            Tconstan = new List<Token>(); // del 600 al 799
            toperado = new List<Token>(); // del -
            trelacio = new List<Token>(); // del -

            loadStaticTokens();

            PruebaDeEscritorio = new List<Prueba>();

            LoadTokens(matches);
            if(errores.Count < 1)
                algoritmoLL();

            MessageBox.Show(""+errores.Count);
        }

        internal List<Token> Tokens { get => tokens; set => tokens = value; }
        internal List<Error> Errores { get => errores; set => errores = value; }
        internal List<Token> Tsintact { get => tsintact; set => tsintact = value; }
        internal List<Prueba> PruebaDeEscritorio { get => pruebaDeEscritorio; set => pruebaDeEscritorio = value; }
        internal List<Token> Tidentif { get => tidentif; set => tidentif = value; }
        internal List<Token> Tconstan { get => tconstan; set => tconstan = value; }

        private void LoadTokens(MatchCollection matches)
        {
            int linea = 1;
            foreach (Match m in matches)
            {
                string c = m.Value;
                if (m.Groups[1].Value != "")
                {
                    linea++;
                }
                if (c != "\r" && c != "\r\n" && c != "\n" && c != " " && c != "")
                {
                    AddToken(c, linea);
                }
            }
        }

        private void AddToken(string v, int l)
        {
            if(PalaresValidar(v)) // 1
            {
                var a = getSimbolo(v, tpalares);
                Tokens.Add(new Token(v, a.indice, l, 1, a.simbolo));
            } else
            if (AlfanumValidar(v)) // 3
            {
                int resindex = getIndexIfExist(v, 6, "a", l);
                if (resindex >= 0)
                    Tokens.Add(new Token(v, 601 + resindex, l, 6, "a"));
                else
                    // Limite de constantes excedido
                    errores.Add(new Error(1, 102, l, "Limite de constantes excedido"));
            }
            else
            if (IdentifValidar(v)) // 4
            {
                int resindex = getIndexIfExist(v, 4, "i", l);
                if (resindex >= 0)
                    Tokens.Add(new Token(v, 401+resindex, l, 4, "i"));
                else
                    // Limite de identificadores excedido
                    errores.Add(new Error(1, 103, l, "Limite de identificadores excedido"));
            } else
            if(DelimitValidar(v)) // 5
            {
                var a = getSimbolo(v, tdelimit);
                Tokens.Add(new Token(v, a.indice, l, 5, a.simbolo));
            } else
            if (ConstaValidar(v)) // 6
            {
                int resindex = getIndexIfExist(v, 6, "d", l);
                if (resindex >= 0)
                    Tokens.Add(new Token(v, 601+resindex, l, 6, "d"));
                else
                    // Limite de constantes excedido
                    errores.Add(new Error(1, 104, l, "Limite de constantes excedido"));
            } else
            if (OperadValidar(v)) // 7
            {
                var a = getSimbolo(v, toperado);
                Tokens.Add(new Token(v, a.indice, l, 7, v));
            } else
            if (RelaciValidar(v)) // 8
            {
                var a = getSimbolo(v, trelacio);
                Tokens.Add(new Token(v, a.indice, l, 8, a.simbolo));
            } else
            {
                if(v.Length > 1)
                {
                    // Cadena desconocida
                    errores.Add(new Error(1, 101, l, "Simbolo desconocida"));
                }
                else
                {
                    // Elemento desconocido
                    errores.Add(new Error(1, 102, l, "Elemento invalido"));
                }
            }
        }

        #region Métodos de identificación

        private bool AlfanumValidar(string cad)
        {
            // valida si la cadena es constante alfanúmerica
            if (Regex.IsMatch(cad, @"('|‘|’)[^'‘’]*('|’|‘)"))
                return true; // regresa un sí, en caso de que lo sea
            else
                return false; // y un no, en caso de que no lo sea
        }

        private bool ConstaValidar(string cad)
        {
            // valida si la cadena es constante númerica
            if (Regex.IsMatch(cad, @"\b\d{1,3}(\,?\d{3})?(\.?\d{1,3})?\b"))
                return true; // regresa un sí, en caso de que lo sea
            else
                return false; // y un no, en caso de que no lo sea
        }

        private bool DelimitValidar(string cad)
        {
            // valida si la cadena es un delimitador
            if (Regex.IsMatch(cad, @"([](),'‘’.{}[])"))
                return true; // regresa un sí, en caso de que lo sea
            else
                return false; // y un no, en caso de que no lo sea
        }

        private bool IdentifValidar(string cad)
        {
            // valida si la cadena es un identificador
            if (Regex.IsMatch(cad, @"\A[A-Za-z#@_]+[A-Za-z#@_0-9.]*"))
                return true; // regresa un sí, en caso de que lo sea
            else
                return false; // y un no, en caso de que no lo sea
        }

        private bool OperadValidar(string cad)
        {
            // valida si la cadena es un operador
            if (Regex.IsMatch(cad, @"[-+/*]"))
                return true; // regresa un sí, en caso de que lo sea
            else
                return false; // y un no, en caso de que no lo sea
        }

        private bool PalaresValidar(string cad)
        {
            if (!Regex.IsMatch(cad, @"('|‘|’)[^'‘’]*('|’|‘)"))
            {
                // valida si la cadena es una palabra reservada
                if (Regex.IsMatch(cad, @"\b(?i)\bSELECT\b|\bFROM\b|\bWHERE\b|\bAND\b|\bOR\b|\bIN\b(?i)\b"))
                    return true; // regresa un sí, en caso de que lo sea
                else
                    return false; // y un no, en caso de que no lo sea
            }
            else
                return false; // y un no, en caso de que no lo sea

        }

        private bool RelaciValidar(string cad)
        {
            // valida si la cadena es un operador
            if (Regex.IsMatch(cad, @"[<=|==|<|=|>|>=]"))
                return true; // regresa un sí, en caso de que lo sea
            else
                return false; // y un no, en caso de que no lo sea
        }

        #endregion

        #region Métodos de busqueda y registro de tokens

        private int getIndexIfExist(string v, int t, string s, int l)
        {
            var res = (false, 0);
            switch (t)
            {
                case 1:
                    res = tokenExists(v, Tidentif);
                    return res.Item2; 
                case 4:
                    res = tokenExists(v, Tidentif);
                    return addTokenType(v, Tidentif, res.Item1, res.Item2, t, l);
                case 5:
                    res = tokenExists(v, Tidentif);
                    return res.Item2;
                case 6:
                    res = tokenExists(v, Tconstan);
                    return addTokenType(v, Tconstan, res.Item1, res.Item2, t, l);
                case 7:
                    res = tokenExists(v, Tidentif);
                    return res.Item2;
                case 8:
                    res = tokenExists(v, Tidentif);
                    return res.Item2;
                default:
                    return 0;
            }
        }

        #region Métodos auxiliares

        private (bool Resultado, int Valor) tokenExists(string v, List<Token> lista)
        {
            foreach(Token t in lista)
            {
                if(t.Valor == v) 
                {
                    return (true, t.Indice);
                }
            }
            return (false, -1);
        }

        private int addTokenType(string v, List<Token> lista, bool e, int i, int t, int l)
        {
            if (e)
            {
                int indice = i;
                lista[getListIndex(v, lista)].setLine(l);
                return indice; // retorna el valor existente
            }
            else
            {
                int indice = 0;
                indice = lista.Count;
                lista.Add(new Token(v, indice, t, "", l));
                lista[getListIndex(v, lista)].setLine(l);
                return indice;

            }
        }

        private int getListIndex(string v, List<Token> lista)
        {
            int i = 0;
            foreach(Token t in lista)
            {
                if(v == t.Valor)
                {
                    return i;
                }
                i++;
            }
            return 0;
        }

        private (string simbolo, int indice) getSimbolo(string valor, List<Token> lista) 
        { 
            foreach(Token t in lista)
            {
                if(valor == t.Valor)
                {
                    return (t.Simbolo, t.Indice);
                }
            }
            return ("", 0);
        }

        private void loadStaticTokens()
        {
            tpalares.Add(new Token("SELECT", 10, 1, "s", 0));
            tpalares.Add(new Token("FROM", 11, 1, "f", 0));
            tpalares.Add(new Token("WHERE", 12, 1, "w", 0));
            tpalares.Add(new Token("IN", 13, 1, "n", 0));
            tpalares.Add(new Token("AND", 14, 1, "y", 0));
            tpalares.Add(new Token("OR", 15, 1, "o", 0));
            tpalares.Add(new Token("CREATE", 16, 1, "c", 0));
            tpalares.Add(new Token("TABLA", 17, 1, "t", 0));
            tpalares.Add(new Token("CHAR", 18, 1, "h", 0));
            tpalares.Add(new Token("NUMERIC", 19, 1, "u", 0));
            tpalares.Add(new Token("NOT", 20, 1, "e", 0));
            tpalares.Add(new Token("NULL", 21, 1, "g", 0));
            tpalares.Add(new Token("CONSTRAINT", 22, 1, "b", 0));
            tpalares.Add(new Token("KEY", 23, 1, "k", 0));
            tpalares.Add(new Token("PRIMARY", 24, 1, "p", 0));
            tpalares.Add(new Token("FOREIGN", 25, 1, "j", 0));
            tpalares.Add(new Token("REFERENCES", 26, 1, "l", 0));
            tpalares.Add(new Token("INSERT", 27, 1, "m", 0));
            tpalares.Add(new Token("INTO", 28, 1, "q", 0));
            tpalares.Add(new Token("VALUES", 29, 1, "v", 0));

            tdelimit.Add(new Token(",", 50, 5, ",", 0));
            tdelimit.Add(new Token(".", 51, 5, ".", 0));
            tdelimit.Add(new Token("(", 52, 5, "(", 0));
            tdelimit.Add(new Token(")", 53, 5, ")", 0));
            tdelimit.Add(new Token("'", 54, 5, "'", 0));
            tdelimit.Add(new Token("‘", 55, 5, "‘", 0));
            tdelimit.Add(new Token("’", 56, 5, "’", 0));

            toperado.Add(new Token("+", 70, 7, "+", 0));
            toperado.Add(new Token("-", 71, 7, "-", 0));
            toperado.Add(new Token("*", 72, 7, "*", 0));
            toperado.Add(new Token("/", 73, 7, "/", 0));

            trelacio.Add(new Token(">", 81, 8, "r", 0));
            trelacio.Add(new Token("<", 82, 8, "r", 0));
            trelacio.Add(new Token("=", 83, 8, "r", 0));
            trelacio.Add(new Token(">=", 84, 8, "r", 0));
            trelacio.Add(new Token("<=", 85, 8, "r", 0));

            // Para la regla Q
            List<string> temp = new List<string>();
            temp.Add("s");
            temp.Add("A");
            temp.Add("f");
            temp.Add("F");
            temp.Add("J");
            Tsintact.Add(new Token("Q", 300, temp));
            // Para la regla A
            temp = new List<string>();
            temp.Add("B");
            temp.Add("|");
            temp.Add("*");
            Tsintact.Add(new Token("A", 301, temp));
            // Para la regla B
            temp = new List<string>();
            temp.Add("C");
            temp.Add("D");
            Tsintact.Add(new Token("B", 302, temp));
            // Para la regla D
            temp = new List<string>();
            temp.Add(",");
            temp.Add("B");
            temp.Add("|");
            temp.Add("λ");
            Tsintact.Add(new Token("D", 303, temp));
            // Para la regla C
            temp = new List<string>();
            temp.Add("i");
            temp.Add("E");
            Tsintact.Add(new Token("C", 304, temp));
            // Para la regla E
            temp = new List<string>();
            temp.Add(".");
            temp.Add("i");
            temp.Add("|");
            temp.Add("λ");
            Tsintact.Add(new Token("E", 305, temp));
            // Para la regla F
            temp = new List<string>();
            temp.Add("G");
            temp.Add("H");
            Tsintact.Add(new Token("F", 306, temp));
            // Para la regla H
            temp = new List<string>();
            temp.Add(",");
            temp.Add("F");
            temp.Add("|");
            temp.Add("λ");
            Tsintact.Add(new Token("H", 307, temp));
            // Para la regla G
            temp = new List<string>();
            temp.Add("i");
            temp.Add("I");
            Tsintact.Add(new Token("G", 308, temp));
            // Para la regla I
            temp = new List<string>();
            temp.Add("i");
            temp.Add("|");
            temp.Add("λ");
            Tsintact.Add(new Token("I", 309, temp));
            // Para la regla J
            temp = new List<string>();
            temp.Add("w");
            temp.Add("K");
            temp.Add("|");
            temp.Add("λ");
            Tsintact.Add(new Token("J", 310, temp));
            // Para la regla K
            temp = new List<string>();
            temp.Add("L");
            temp.Add("V");
            Tsintact.Add(new Token("K", 311, temp));
            // Para la regla V
            temp = new List<string>();
            temp.Add("P");
            temp.Add("K");
            temp.Add("|");
            temp.Add("λ");
            Tsintact.Add(new Token("V", 312, temp));
            // Para la regla L
            temp = new List<string>();
            temp.Add("C");
            temp.Add("M");
            Tsintact.Add(new Token("L", 313, temp));
            // Para la regla M
            temp = new List<string>();
            temp.Add("N");
            temp.Add("O");
            temp.Add("|");
            temp.Add("n");
            temp.Add("(");
            temp.Add("Q");
            temp.Add(")");
            Tsintact.Add(new Token("M", 314, temp));
            // Para la regla N
            temp = new List<string>();
            temp.Add("r");
            Tsintact.Add(new Token("N", 315, temp));
            // Para la regla O
            temp = new List<string>();
            temp.Add("C");
            temp.Add("|");
            temp.Add("R");
            temp.Add("|");
            temp.Add("T");
            Tsintact.Add(new Token("O", 316, temp));
            // Para la regla P
            temp = new List<string>();
            temp.Add("y");
            temp.Add("|");
            temp.Add("o");
            Tsintact.Add(new Token("P", 317, temp));
            // Para la regla R
            temp = new List<string>();
            temp.Add("a");
            Tsintact.Add(new Token("R", 318, temp));
            // Para la regla T
            temp = new List<string>();
            temp.Add("d");
            Tsintact.Add(new Token("T", 319, temp));
        }

        #endregion

        #endregion

        #region Métodos del parser

        // Método de validación de consistencia LL
        public void algoritmoLL()
        {
            int i = 0;
            int nada = 0;
            string x = "", k = "";

            List<string> condicionesEncontradas = new List<string>();

            string mensaje = "";

            Stack<string> Pila = new Stack<string>();
            Pila.Push("$");
            Pila.Push("Q");

            List<Token> Expresion = new List<Token>();
            copiarTokens(ref Expresion, Tokens);

            do
            {
                mensaje = mostrarPila(Pila, mensaje);
                x = Pila.Pop();
                k = Expresion[i].Simbolo;

                pruebaDeEscritorio.Add(new Prueba(x, mensaje, k));
                //MessageBox.Show("Pila: " + mensaje + "\n\r" + " X: " + x + "\n\r" + " K: " +k);
                
                if (comprobarTerminal(x) | x == "$" | x == "λ")
                {
                    if (x == k)
                    {
                        i++;
                    }
                    else
                    {
                        // Caso de error
                        var r = encontrarError(k);
                        MessageBox.Show(" " + k + " " + x);
                        errores.Add(new Error(2, r.id, Expresion[i].Linea, r.mensaje));
                        break;
                    }
                }
                else
                {
                    condicionesEncontradas = new List<string>();
                    if (existeCoincidencia(x, k, ref condicionesEncontradas))
                    {
                        if(!existeCondicionVacia(condicionesEncontradas))
                        {
                            apilarCondiciones(condicionesEncontradas, ref Pila);
                        }
                    }
                    else
                    {
                        // Caso de error
                        var r = encontrarError(k);
                        MessageBox.Show(" "+k+" "+x);
                        errores.Add(new Error(2, r.id, Expresion[i].Linea, r.mensaje));
                        break;
                    }
                }
            }
            while (x != "$");
        }

        // Métodos auxiliares para el analisis sintactico
        // con el algoritmo LL, tabla sintactica y pila

        // Método para pasar tokens de una lista a otra
        private void copiarTokens(ref List<Token> templt, List<Token> lt)
        {
            foreach (Token t in lt)
            {
                templt.Add(t);
            }
            templt.Add(new Token("$", 0, 0, 0, "$"));
        }
        // Método para comprobar si la pila tiene un valor
        // de tipo Terminal actualmente
        private bool comprobarTerminal(string sim)
        {
            foreach (Token t in Tokens) // palabra reservada
            {
                if (t.Simbolo == sim)
                {
                    return true;
                }
            }
            //foreach (Token t in trelacio) // relacionales
            //{
            //    if (t.Simbolo == sim)
            //    {
            //        return true;
            //    }
            //}
            //foreach (Token t in toperado) // operadores
            //{
            //    if (t.Simbolo == sim)
            //    {
            //        return true;
            //    }
            //}
            //foreach (Token t in tdelimit) // delimitadores
            //{
            //    if (t.Simbolo == sim)
            //    {
            //        return true;
            //    }
            //}
            //foreach (Token t in tidentif) // identificadores
            //{
            //    if (t.Simbolo == sim)
            //    {
            //        return true;
            //    }
            //}
            //foreach (Token t in tconstan) // constanes
            //{
            //    if (t.Simbolo == sim)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }
        // Método para comprobar si la pila tiene un valor
        // de tipo Regla actualmente
        private bool comprobarRegla(string sim)
        {
            switch(sim)
            {
                case "Q":
                    return true;
                case "A":
                    return true;
                case "B":
                    return true;
                case "D":
                    return true;
                case "C":
                    return true;
                case "E":
                    return true;
                case "F":
                    return true;
                case "H":
                    return true;
                case "G":
                    return true;
                case "I":
                    return true;
                case "J":
                    return true;
                case "K":
                    return true;
                case "V":
                    return true;
                case "L":
                    return true;
                case "M":
                    return true;
                case "N":
                    return true;
                case "O":
                    return true;
                case "P":
                    return true;
                case "R":
                    return true;
                case "T":
                    return true;
                default:
                    return false;
            }
        }

        // Método para ver si hay una producción en M(X,Y)
        private bool existeCoincidencia(string x, string k, ref List<string> ltcondiciones)
        {
            foreach (Token t in tsintact)
            {
                if (t.Simbolo == x)
                {
                    if(OrExists(t.Producciones))
                    {
                        for(int i = 0; i < t.Producciones.Count; i++)
                        {
                            if(t.Producciones[i] != "|")
                            {
                                ltcondiciones.Add(t.Producciones[i]);
                            } else
                            {
                                if(existeTerminalAceptable(ltcondiciones[0],k))
                                {
                                    return true;
                                }
                                else
                                {
                                    ltcondiciones.Clear();
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        loadCond(t, ref ltcondiciones);
                        return true;
                    }
                }
            }
            return false;
        }

        private void loadCond(Token t, ref List<string> l)
        {
            foreach (string s in t.Producciones)
            {
                l.Add(s);
            }
        }

        private bool OrExists(List<string> liscond)
        {
            foreach (string s in liscond)
            {
                if (s == "|")
                {
                    return true;
                }
            }
            return false;
        }

        private bool existeTerminalAceptable(string x, string k)
        {
            // comprueba si x es una terminal
            if(comprobarTerminal(x))
            {
                // si es una terminal, pregunta si es aceptable
                if (x == k | x == "λ")
                    return true;
                else // si no es, retorna un false
                    return false;
            }
            else
            {
                // debe buscar si es una regla
                if(comprobarRegla(x))
                {
                    return existeTerminalAceptable(buscarTerminales(x), k);
                }
                else
                {
                    return false;
                }
            }
        }

        private string buscarTerminales(string x)
        {
            foreach (Token t in tsintact)
            {
                if (t.Simbolo == x)
                {
                    return t.Producciones[0];
                }
            }
            return "&";
        }

        private bool existeCondicionVacia(List<string> liscond)
        {
            foreach (string s in liscond)
            {
                if (s == "λ")
                {
                    return true;
                }
            }
            return false;
        }

        // Método para agregar la lista de producciones a
        // la cola en M(X,Y)
        private void apilarCondiciones(List<string> liscond, ref Stack<string> p)
        {
            liscond.Reverse();
            foreach (string s in liscond)
            {
                p.Push(s);
            }
        }

        private string mostrarPila(Stack<string> p, string cadena)
        {
            cadena = "";
            Stack<string> temp = p;
            foreach(string s in p)
            {
                cadena += s + " ";
            }
            return cadena;
        }

        private (int id, string mensaje) encontrarError(string simbolo)
        {
            switch(simbolo)
            {
                case "s":
                    return (201, "Error en la ultima Palabra Reservada");
                case "f":
                    return (201, "Error en la ultima Palabra Reservada");
                case "w":
                    return (201, "Error en la ultima Palabra Reservada");
                case "n":
                    return (201, "Error en la ultima Palabra Reservada");
                case "y":
                    return (201, "Error en la ultima Palabra Reservada");
                case "o":
                    return (201, "Error en la ultima Palabra Reservada");
                case "c":
                    return (201, "Error en la ultima Palabra Reservada");
                case "t":
                    return (201, "Error en la ultima Palabra Reservada");
                case "h":
                    return (201, "Error en la ultima Palabra Reservada");
                case "u":
                    return (201, "Error en la ultima Palabra Reservada");
                case "e":
                    return (201, "Error en la ultima Palabra Reservada");
                case "g":
                    return (201, "Error en la ultima Palabra Reservada");
                case "b":
                    return (201, "Error en la ultima Palabra Reservada");
                case "k":
                    return (201, "Error en la ultima Palabra Reservada");
                case "p":
                    return (201, "Error en la ultima Palabra Reservada");
                case "j":
                    return (201, "Error en la ultima Palabra Reservada");
                case "l":
                    return (201, "Error en la ultima Palabra Reservada");
                case "m":
                    return (201, "Error en la ultima Palabra Reservada");
                case "q":
                    return (201, "Error en la ultima Palabra Reservada");
                case "v":
                    return (201, "Error en la ultima Palabra Reservada");
                case ",":
                    return (202, "Error en la ultima Delimitador");
                case ".":
                    return (202, "Error en la ultima Delimitador");
                case "(":
                    return (202, "Error en la ultima Delimitador");
                case ")":
                    return (202, "Error en la ultima Delimitador");
                case "'":
                    return (202, "Error en la ultima Delimitador");
                case "‘":
                    return (202, "Error en la ultima Delimitador");
                case "’":
                    return (202, "Error en la ultima Delimitador");
                case "+":
                    return (203, "Error en la ultima Operador");
                case "-":
                    return (203, "Error en la ultima Operador");
                case "*":
                    return (203, "Error en la ultima Operador");
                case "/":
                    return (203, "Error en la ultima Operador");
                case "r":
                    return (204, "Error en la ultima Operador Relacional");
                case "d":
                    return (206, "Error en la ultima Constante");
                case "a":
                    return (207, "Error en la ultima Constante");
                case "i":
                    return (207, "Error en el ultimo Identificador");
                default:
                    MessageBox.Show("si te falto uno: " + simbolo);
                    return (0, "");
            }
            return (0, "");
        }

        #endregion
    }
}