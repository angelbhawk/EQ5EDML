using EQ5EDML.Componentes;
using EQ5EDML.Elementos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EQ5EDML
{
    public partial class Form : System.Windows.Forms.Form
    {
        private TablaTokens tokens;

        public Form()
        {
            InitializeComponent();
        }

        private void btnEscanear_Click(object sender, EventArgs e)
        {
            tokens = new TablaTokens(Tokenizador.getTokens(tbxEntrada.Text));

            if(tokens.Errores.Count > 0)
            {
                // Muestra los errores
                cbxTablas.Enabled = false;
                MostrarErrores();
            }
            else
            {
                // Muestra los resultados
                cbxTablas.Enabled = true;
                MostrarResultados();
            }
        }

        private void cbxTablas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxTablas.SelectedIndex == 0)
            {
                MostrarResultados();
            }
            if (cbxTablas.SelectedIndex == 1)
            {
                MostrarTablaIdentificadores();
            }
            if (cbxTablas.SelectedIndex == 2)
            {
                MostrarTablaConstantes();
            }
            if (cbxTablas.SelectedIndex == 3)
            {
                MostrarTablaSintactica();
            }
            if (cbxTablas.SelectedIndex == 4)
            {
                MostrarPruebas();
            }
        }

        private void MostrarResultados()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("clmNum", "Numero");
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmCad", "Cadena");
            dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmTip", "Tipo");
            dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmSim", "Simbolo");
            dgv.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmLin", "Linea");
            dgv.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmInd", "Indice");
            dgv.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Color c = Color.FromArgb(32, 32, 32);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = c;
            dgv.DefaultCellStyle.SelectionBackColor = c;

            int i = 0;
            foreach (Token t in tokens.Tokens)
            {
                i++;
                this.dgv.Rows.Add(i, t.Valor, t.Tipo, t.Simbolo, t.Linea, t.Indice);
            }
        }

        private void MostrarErrores()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("clmErr", "Error");
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Color c = Color.FromArgb(32, 32, 32);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = c;
            dgv.DefaultCellStyle.SelectionBackColor = c;

            foreach (Error e in tokens.Errores)
            {
                this.dgv.Rows.Add(e.Tipo + ":" + e.Indice + " Linea " + e.Linea + ": "+e.Descripcion);
            }
        }

        private void MostrarTablaIdentificadores()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("clmNum", "Nombre");
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmCad", "Código");
            dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmTip", "Linea");
            dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Color c = Color.FromArgb(32, 32, 32);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = c;
            dgv.DefaultCellStyle.SelectionBackColor = c;

            int i = 0;
            foreach (Token t in tokens.Tidentif)
            {
                i++;
                this.dgv.Rows.Add(t.Valor, t.Indice, t.Lineas);
            }
        }

        private void MostrarTablaConstantes()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("clmNum", "Numero");
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmCad", "Valor");
            dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmTip", "Tipo");
            dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmTip2", "Indice");
            dgv.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Color c = Color.FromArgb(32, 32, 32);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = c;
            dgv.DefaultCellStyle.SelectionBackColor = c;

            int i = 0;
            foreach (Token t in tokens.Tconstan)
            {
                i++;
                this.dgv.Rows.Add(i, t.Valor, t.Tipo, t.Indice);
            }
        }

        private void MostrarTablaSintactica()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("clmNum", "Numero");
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmCad", "Reglas");
            dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmTip", "Indice");
            dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmSim", "Producciones");
            dgv.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Color c = Color.FromArgb(32, 32, 32);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = c;
            dgv.DefaultCellStyle.SelectionBackColor = c;

            int i = 0;
            foreach (Token t in tokens.Tsintact)
            {
                i++;
                this.dgv.Rows.Add(i, t.Simbolo, t.Indice, t.getProducciones());
            }
        }

        private void MostrarPruebas()
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("clmErr", "X");
            dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmErr", "Cadena");
            dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add("clmErr", "K");
            dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Color c = Color.FromArgb(32, 32, 32);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.BackColor = c;
            dgv.DefaultCellStyle.SelectionBackColor = c;

            foreach (Prueba p in tokens.PruebaDeEscritorio)
            {
                this.dgv.Rows.Add(p.X, p.Cadena, p.K);
            }
        }
    }
}