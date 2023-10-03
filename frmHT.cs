using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StatisticasFutbol
{
    public partial class frmHT : Form
    {
        readonly Conexao conexao = new Conexao();

        public frmHT()
        {
            InitializeComponent();
            AdcinarEquiepesCombo();
        }


        public void AdcinarEquiepesCombo()
        {
            List<string> Equipes = new List<string>();
            Cbo_equipes.Items.Clear();

            Equipes.Add("America MG");
            Equipes.Add("Atletico-MG");
            Equipes.Add("Athletico-PR");
            Equipes.Add("Bahia");
            Equipes.Add("Botafogo RJ");
            Equipes.Add("Bragantino");
            Equipes.Add("Fluminense");
            Equipes.Add("Corinthians");
            Equipes.Add("Coritiba");
            Equipes.Add("Cruzeiro");
            Equipes.Add("Cuiaba");
            Equipes.Add("Flamengo RJ");
            Equipes.Add("Fortaleza");
            Equipes.Add("Goias");
            Equipes.Add("Gremio");
            Equipes.Add("Internacional");
            Equipes.Add("Palmeiras");
            Equipes.Add("Santos");
            Equipes.Add("Sao Paulo");
            Equipes.Add("Vasco");

            Equipes.ForEach(e => Cbo_equipes.Items.Add(e));
        }


        private void Rb_GolsHtAway_CheckedChanged(object sender, System.EventArgs e)
        {
            string sql = "SELECT * FROM Media_golsHt_Aways";
            conexao.AdcionarDados_Grid(sql, dgv_dados);
        }

        private void Rb_GolsHtHome_CheckedChanged(object sender, System.EventArgs e)
        {
            string sql = "SELECT * FROM Media_golsHt_Home";
            conexao.AdcionarDados_Grid(sql, dgv_dados);
        }

        private void Rb_Cantos_Ht_home_CheckedChanged(object sender, System.EventArgs e)
        {
            string sql = "SELECT * FROM EscanteiosHtHome";
            conexao.AdcionarDados_Grid(sql, dgv_dados);
        }

        private void Rb_Cantos_Ht_Away_CheckedChanged(object sender, System.EventArgs e)
        {
            string sql = "SELECT * FROM EscanteiosHtAway";
            conexao.AdcionarDados_Grid(sql, dgv_dados);
        }

        private void SelecionarQuery()
        {
            string sql;
            string equipe;
            lbl_result.Text = string.Empty;

            //BUSCAR JOGOS COM MAIS DE 3 ESCANTEIOS DAS EQUIPES JOGANDO EM CASA
            if (Cbo_equipes.SelectedValue != null || rb_Jogos_Cantos_Ht_Mandante.Checked == true)
            {
                equipe = Cbo_equipes.SelectedItem.ToString();
                sql = "SELECT Data, Home, EHTHome as Cantos_H, EHTAway as Cantos_A, Away, TotalEHT as Total " +
                      "FROM brasil WHERE Home ='" + equipe + "'" + " AND TotalEHT > 3 " +
                      "OR Away ='" + equipe + "'" + " AND TotalEHT > 3 " +
                      "AND Data BETWEEN " + conexao.Data_Br + " AND CURDATE() ORDER BY Data DESC";

                conexao.AdcionarDados_Grid(sql, dgv_dados);
                lbl_result.Text = $"Como Mandante Encontramos {dgv_dados.RowCount} Jogos Para a Equipe Do {equipe} ";
                return;
            }

            //BUSCAR JOGOS ONDE TIVEMOS PELO MENOS 1 GOL OU MAIS NO HT DA EQUIPE JOGANDO EM CASA
            if (Cbo_equipes.SelectedValue != null || rb_Jogos_Gols_Ht_Mandante.Checked)
            {
                equipe = Cbo_equipes.SelectedItem.ToString();
                sql = "SELECT Data, Home, GolHome as Gol_C, GolAway as Gol_V, Away, TotalGolHT as Total " +
                      "FROM brasil WHERE Home ='" + equipe + "'" + " AND TotalGolHT > 0 " +
                      "OR Away ='" + equipe + "'" + " AND TotalGolHT > 0 " +
                      "AND Data BETWEEN " + conexao.Data_Br + " AND CURDATE() ORDER BY Data DESC";

                conexao.AdcionarDados_Grid(sql, dgv_dados);
                lbl_result.Text = $"Encontramos {dgv_dados.RowCount} Jogos Com Gols No HT Para a Equipe Do {equipe} ";
                return;
            }

        }

        private void Cbo_equipes_SelectedValueChanged(object sender, System.EventArgs e)
        {
            SelecionarQuery();
        }

        private double CalcularMedia(Guna2DataGridView dgv)
        {
            double media;
            double total = 0;

            foreach (DataGridViewRow lin in dgv.Rows)
            {
                total += Convert.ToDouble(lin.Cells[5].Value);
            }

            media = Math.Round(total / dgv_dados.RowCount, 2);
            return media;
        }

        private void Media_Mandante()
        {
            string sql = "SELECT b.Home AS 'Equipe', COUNT(b.Home) AS 'Jogos'," +
                " SUM(b.GolHome) AS 'Gols', ROUND(AVG(b.GolHome),2) AS 'M_Gols', SUM(b.EHome) AS 'Escanteios'," +
                " round(AVG(b.EHome),2) AS 'M_Escanteios', SUM(b.CartaoHome) AS 'Cartao'," +
                " ROUND(AVG(b.CartaoHome),2) AS 'M_Cartao'" +
                " FROM brasil b" +
                " WHERE YEAR(DATA) = YEAR(CURDATE()) GROUP BY home ORDER BY Gols DESC, M_Gols DESC;";

            conexao.AdcionarDados_Grid(sql, dgv_dados);
        }

        private void Media_Visitante()
        {
            string sql = "SELECT b.Away AS 'Equipe', COUNT(b.Away) AS 'Jogos', SUM(b.GolAway) AS 'Gols'," +
                " ROUND(AVG(b.GolAway),2) AS 'M_Gols', SUM(b.Eaway) AS 'Escanteios'," +
                " ROUND(AVG(b.Eaway),2) AS 'M_Escanteios', SUM(b.CartaoAway) AS 'Cartao'," +
                " ROUND(AVG(b.CartaoAway),2) AS 'M_Cartao'" +
                " FROM brasil b " +
                " WHERE YEAR(DATA) = YEAR(CURDATE()) GROUP BY Away ORDER BY Gols DESC, M_Gols DESC;";

            conexao.AdcionarDados_Grid(sql, dgv_dados);
        }

        private void guna2RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Media_Mandante();
        }

        private void guna2RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Media_Visitante();
        }
    }// Fim Class
}// Fim name Space
