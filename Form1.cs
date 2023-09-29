using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace StatisticasFutbol
{
    public partial class frm_futbol : Form
    {
        public frm_futbol()
        {
            InitializeComponent();
        }

        private readonly Conexao conectar = new Conexao();

        private void frm_futbol_Load(object sender, EventArgs e)
        {
            this.lbl_dividii_magem.Visible = false;
            this.img_away.Visible = false;
            this.img_home.Visible = false;

        }

        void Pegar_todosJogos(RadioButton chk)
        {
            string liga = chk.Text;
            Conexao conectar = new Conexao();
            try
            {
                string Window_Function = "SELECT ROW_NUMBER() OVER (PARTITION BY Liga ORDER BY PT DESC) as '#' ," +
                                         " Liga, Equipe, Jogos, V, E, D, GF, GS, SG, PT, CS" +
                                         " FROM tbligas WHERE Liga ='" + liga.Trim() + "'" + " ORDER BY Liga, PT DESC ";

                conectar.AdcionarDados_Grid(Window_Function, DGVResumo);
                ColorirLinhas();
                DGVResumo.Columns[1].Visible = false;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Não foi possivél carregar os dados" + ex.Message);
                conectar.Desconectar();
            }

        }

        private void cbo_e_SelectedValueChanged(object sender, EventArgs e)
        {
            string liga;
            string equipe;
            Limpar_Botoes_ResumoHome();

            string foto = cbo_e.SelectedItem.ToString() + ".png";
            img_home.Visible = true;
            img_home.Image = Image.FromFile("img/" + foto);

            try
            {
                foreach (RadioButton ct in painel.Controls)
                {
                    //para buscar jogos da equipe em casa
                    if (ct.Checked == true && R_JogosCasa.Checked == true)
                    {
                        liga = ct.Text;
                        equipe = cbo_e.SelectedItem.ToString();

                        string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, " +
                                "Away, totalcantos as EC, resultado as R " +
                                "FROM fdados " +
                                "WHERE Liga ='" + liga.Trim() + "'" +
                                "AND Home ='" + equipe + "'" +
                                "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE() " +
                                "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC";

                        conectar.AdcionarDados_Grid(SQL_Home, DGVResumo);
                        conectar.Modificar_Coluna_Gridview_Home(DGVResumo, 6);
                        Resumo_JogosHome();
                        Jogos_Acima_Home_Gol();
                        return;
                    }

                    //Buscando os 10 ultimos jogos
                    else if (RadioDEZJogos.Checked == true && ct.Checked == true)
                    {
                        liga = ct.Text;
                        equipe = cbo_e.SelectedItem.ToString();

                        string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, " +
                                "Away, totalcantos as EC " +
                                "FROM fdados " +
                                "WHERE Liga ='" + liga.Trim() + "'" +
                                "AND Away ='" + equipe + "'" +
                                "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE() " +
                                "OR Home ='" + equipe + "'" +
                                "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE()" +
                                "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 10";

                        conectar.AdcionarDados_Grid(SQL_Home, DGVResumo);
                        return;
                    }

                    // Buscar os 20 ultimos jogos
                    else if (RVinteJogos.Checked == true && ct.Checked == true)
                    {
                        liga = ct.Text;
                        equipe = cbo_e.SelectedItem.ToString();

                        string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, " +
                                "Away, totalcantos as EC" +
                                "FROM fdados " +
                                "WHERE Liga ='" + liga.Trim() + "'" +
                                "AND Away ='" + equipe + "'" +
                                "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE() " +
                                "OR Home ='" + equipe + "'" +
                                "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE()" +
                                "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 20";

                        conectar.AdcionarDados_Grid(SQL_Home, DGVResumo);
                        return;
                    }

                    else if (R_Hed_To_Hed.Checked == true)
                    {
                        ResumoGeral_h2h();
                        Resumo_JogosHome();
                        Jogos_Acima_Home_Gol();
                        Calculo_h2h_();
                        return;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao realizar o calculo" + ex.Message);
            }
            finally
            {
                conectar.Desconectar();
            }
        }

        private void ckLaliga_CheckedChanged_1(object sender, EventArgs e)
        {
            string liga = ckLaliga.Text;
            Conexao conectar = new Conexao();
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_e);
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_Away);
            Pegar_todosJogos(ckLaliga);
            Calcular_media_Geral_Competicao();
            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();
        }

        private void chk_ingla_CheckedChanged_1(object sender, EventArgs e)
        {
            string liga = chk_ingla.Text;
            Calcular_media_Geral_Competicao();
            Pegar_todosJogos(chk_ingla);

            Conexao conectar = new Conexao();
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_e);
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_Away);

            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();
        }

        private void chk_italia_CheckedChanged_1(object sender, EventArgs e)
        {
            string liga = chk_italia.Text;
            Conexao conectar = new Conexao();
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_e);
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_Away);
            Pegar_todosJogos(chk_italia);
            Calcular_media_Geral_Competicao();
            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();
        }

        private void chk_alemanha_CheckedChanged_1(object sender, EventArgs e)
        {
            string liga = chk_alemanha.Text;
            Conexao conectar = new Conexao();
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_e);
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_Away);
            Pegar_todosJogos(chk_alemanha);
            Calcular_media_Geral_Competicao();
            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();
        }

        private void chk_franca_CheckedChanged_1(object sender, EventArgs e)
        {
            string liga = chk_franca.Text;
            Conexao conectar = new Conexao();
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_e);
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_Away);
            Pegar_todosJogos(chk_franca);
            Calcular_media_Geral_Competicao();
            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();
        }

        private void chkPortu_CheckedChanged_1(object sender, EventArgs e)
        {
            string liga = chkPortu.Text;
            Conexao conectar = new Conexao();
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_e);
            conectar.AdcionarDados_ComboLigas(liga.Trim(), cbo_Away);
            Pegar_todosJogos(chkPortu);
            Calcular_media_Geral_Competicao();
            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();
        }

        public void ResumoGeral_h2h()
        {
            string equipehome;
            string equipeAway;

            Conexao conectar = new Conexao();

            try
            {
                foreach (RadioButton item in painel.Controls)
                {
                    if (item.Checked == true && cbo_e.SelectedItem != null && cbo_Away.SelectedItem != null)
                    {
                        equipehome = cbo_e.SelectedItem.ToString();
                        equipeAway = cbo_Away.SelectedItem.ToString();

                        string sql = "SELECT Data, Home as Mandante, golhome as 'GH', golAway as 'GA', " +
                                     " away as Visitante, totalcantos as Escanteios FROM fdados " +
                                     "WHERE Home ='" + equipehome + "'" +
                                     "AND Away ='" + equipeAway + "'" +
                                     "OR Away ='" + equipehome + "'" +
                                     "AND Home ='" + equipeAway + "'" +
                                     "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, DAY(Data) DESC ";
                        conectar.AdcionarDados_Grid(sql, DGVResumo);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao buscar os jogos das equipes h2h " + ex.Message);
                conectar.Desconectar();
            }
        }

        private void R_Hed_To_Hed_CheckedChanged(object sender, EventArgs e)
        {
            //ResumoGeral_h2h(); Limpar_Botoes_Resumo();
        }

        private void chk_brasil_CheckedChanged(object sender, EventArgs e)
        {
            Limpar_Botoes_ResumoHome();
            Limpar_Botoes_ResumoAway();
            LimparImagens();

            if (chk_brasil.Checked == true)
            {
                this.Close();
                Thread t = new Thread(() => Application.Run(new frm_brasil()));
                t.Start();
            }
        }

        public void Resumo_JogosHome()
        {
            string liga;
            string equipe;
            Conexao conectar = new Conexao();

            try
            {
                foreach (RadioButton rb in painel.Controls)
                {
                    if (rb.Checked == true)
                    {
                        liga = rb.Text.Trim();
                        equipe = cbo_e.SelectedItem.ToString();

                        string sql_home = "SELECT COUNT(*), SUM(GolHome), ROUND(AVG(GolHome), 1), ROUND(AVG(escanteioHome) ,1) " +
                       " FROM fdados " +
                       " WHERE Liga ='" + liga.ToString() + "'" +
                       " AND home ='" + equipe + "'" +
                       " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE() ";

                        string sql_Away = "select sum(golaway), round(avg(golaway), 1) " +
                            "FROM fdados " +
                            "WHERE Liga ='" + liga + "'" +
                            "AND home ='" + equipe + "'" +
                            "AND Data BETWEEN '" + conectar.Data + "' AND CURDATE() ";

                        conectar.funcaoAgregacaoHome(sql_home, total_Jogos, golsFeito, mediagolFeito, MediaCanto);
                        conectar.FuncaoAgregacaoAway(sql_Away, totalGolSofrido, MediaGolSofrido);
                        return;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao realizar as consulta de agregação " + ex.Message);
            }
        }

        public void Resumo_JogosAway()
        {
            string liga;
            string equipe;
            Conexao conectar = new Conexao();

            try
            {
                foreach (RadioButton rb in painel.Controls)
                {
                    if (rb.Checked == true)
                    {
                        liga = rb.Text.Trim();
                        equipe = cbo_Away.SelectedItem.ToString();

                        string sql = "SELECT COUNT(*), SUM(Golaway), ROUND(AVG(Golaway), 1), ROUND(AVG(escanteioaway) ,1) " +
                       "FROM fdados " +
                       "WHERE Liga ='" + liga.ToString() + "'" +
                       "AND away ='" + equipe + "'" +
                       "AND Data BETWEEN '" + conectar.Data + "' AND CURDATE() ";


                        string sql_Away = "SELECT SUM(Golhome), ROUND(AVG(Golhome), 1) " +
                       "FROM fdados " +
                       "WHERE Liga ='" + liga.ToString() + "'" +
                       "AND away ='" + equipe + "'" +
                       "AND Data BETWEEN '" + conectar.Data + "' AND CURDATE() ";

                        conectar.funcaoAgregacaoHome(sql, jogosAway, golsaway, mediagosAway, mediacantosaway);
                        conectar.FuncaoAgregacaoAway(sql_Away, totalsofridoaway, mediasofridoaway);
                        return;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao realizar as consulta de agregação " + ex.Message);
            }
        }

        private void cbo_Away_SelectedValueChanged(object sender, EventArgs e)
        {
            string liga;
            string equipe;
            Limpar_Botoes_ResumoAway();

            string img = cbo_Away.SelectedItem.ToString() + ".png";
            img_away.Visible = true;
            lbl_dividii_magem.Visible = true;
            img_away.Image = Image.FromFile("img/" + img);


            //para buscar jogos da equipe como visitante
            foreach (RadioButton rb in painel.Controls)
            {
                if (rb.Checked == true && R_jogo_Fora.Checked == true)
                {
                    liga = rb.Text;
                    equipe = cbo_Away.SelectedItem.ToString();

                    string SQL_Away = "SELECT Data, Home, golHome as GH, golAway as GA," +
                            "Away, totalcantos as EC, resultado as R " +
                            "FROM fdados " +
                            "WHERE Liga ='" + liga.Trim() + "'" +
                            "AND Away ='" + equipe + "'" +
                            "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE() " +
                            "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC";

                    conectar.AdcionarDados_Grid(SQL_Away, DGVResumo);
                    conectar.Modificar_Gridview_Away(DGVResumo, 6);
                    Resumo_JogosAway();
                    Jogos_Acima_Away_gols();

                    return;
                }

                //Buscar os 10 ultimos 
                else if (rb.Checked == true && RadioDEZJogos.Checked == true)
                {
                    liga = rb.Text;
                    equipe = cbo_Away.SelectedItem.ToString();

                    string SQL_away = "SELECT Data, Home, golHome as GH, golAway as GA, " +
                            "Away, totalcantos as EC " +
                            "FROM fdados " +
                            "WHERE Liga ='" + liga.Trim() + "'" +
                            "AND Away ='" + equipe + "'" +
                            "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE() " +
                            "OR Home ='" + equipe + "'" +
                            "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE()" +
                            "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 10";

                    conectar.AdcionarDados_Grid(SQL_away, DGVResumo);
                    return;


                }
                //Buscar os 20 ultimos 
                else if (rb.Checked == true && RVinteJogos.Checked == true)
                {
                    liga = rb.Text;
                    equipe = cbo_Away.SelectedItem.ToString();

                    string SQL_away = "SELECT Data, Home, golHome as GH, golAway as GA, " +
                            "Away, totalcantos as EC " +
                            "FROM fdados " +
                            "WHERE Liga ='" + liga.Trim() + "'" +
                            "AND Away ='" + equipe + "'" +
                            "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE() " +
                            "OR Home ='" + equipe + "'" +
                            "AND Data BETWEEN '" + conectar.Data + "' and CURRENT_DATE()" +
                            "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 20";

                    conectar.AdcionarDados_Grid(SQL_away, DGVResumo);
                    return;
                }
                //Buscar H2H
                else if (R_Hed_To_Hed.Checked == true)
                {
                    ResumoGeral_h2h();
                    Resumo_JogosAway();
                    Jogos_Acima_Away_gols();
                    Calculo_h2h_();
                    return;
                }
            }
        }

        public void Limpar_Botoes_ResumoHome()
        {
            foreach (Control bt in this.Controls)
            {
                if (bt is Guna2Button)
                {
                    if (bt.Tag.ToString() == "H")
                    {
                        bt.Text = "0";
                    }

                }
            }
        }

        public void Limpar_Botoes_ResumoAway()
        {
            foreach (Control bt in this.Controls)
            {
                if (bt is Guna2Button)
                {
                    if (bt.Tag.ToString() == "A")
                    {
                        bt.Text = "0";
                    }
                }
            }
        }

        public void Jogos_Acima_Home_Gol()
        {
            Conexao conectar = new Conexao();
            string equipe;
            if (cbo_e.SelectedItem != null)
            {
                equipe = cbo_e.SelectedItem.ToString();
                string sql_1_home = "SELECT COUNT(*) FROM fdados" +
                        " WHERE home ='" + equipe + "'" +
                        " AND DATA BETWEEN '" + conectar.Data + "' AND CURDATE()" +
                        " AND totalgols > 1";

                string sql_2_home = "SELECT COUNT(*) FROM fdados" +
                        " WHERE home ='" + equipe + "'" +
                        " AND DATA BETWEEN '" + conectar.Data + "' AND CURDATE()" +
                        " AND totalgols > 2";

                string sql_jogos_sem_Marcar = "SELECT COUNT(*) FROM fdados " +
                                              " WHERE home ='" + equipe + "'" +
                                              " AND golhome = 0" +
                                              " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE()";

                conectar.funcaoAgregacao_Jogos_Acima(sql_1_home, MaisDe_1_Gol);
                conectar.funcaoAgregacao_Jogos_Acima(sql_2_home, Maisde_2_gol);
                conectar.funcaoAgregacao_Jogos_Acima(sql_jogos_sem_Marcar, JogosSemMarcar);
            }

        }

        public void Jogos_Acima_Away_gols()
        {
            Conexao conectar = new Conexao();
            string equipe;
            if (cbo_Away.SelectedItem != null)
            {
                equipe = cbo_Away.SelectedItem.ToString();
                string sql_1_away = "SELECT COUNT(*) FROM fdados" +
                        " WHERE away ='" + equipe + "'" +
                        " AND DATA BETWEEN '" + conectar.Data + "' AND CURDATE()" +
                        " AND totalgols > 1";

                string sql_2_away = "SELECT COUNT(*) FROM fdados" +
                        " WHERE away ='" + equipe + "'" +
                        " AND DATA BETWEEN '" + conectar.Data + "' AND CURDATE()" +
                        " AND totalgols > 2";

                string sql_jogos_sem_marcar_away = "SELECT COUNT(*) FROM fdados" +
                                            " WHERE away ='" + equipe + "'" +
                                            " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE() " +
                                            " AND golaway = 0";
                conectar.funcaoAgregacao_Jogos_Acima(sql_jogos_sem_marcar_away, semtomagolAway);
                conectar.funcaoAgregacao_Jogos_Acima(sql_1_away, mais_1_Away);
                conectar.funcaoAgregacao_Jogos_Acima(sql_2_away, mais_2_away);
            }
        }

        public void LimparImagens()
        {
            if (img_home.Image != null || img_away.Image != null)
            {
                img_away.Image = null;
                img_home.Image = null;
            }

            foreach (Label texto in this.painel_europa_h2h.Controls)
            {
                texto.Text = string.Empty;
            }
        }

        public void Calcular_media_Geral_Competicao()
        {
            foreach (RadioButton rb in painel.Controls)
            {
                if (rb.Checked == true && rb.Text != "Brasileirao")
                {
                    string liga = rb.Text;

                    string sql = "SELECT COUNT(*), SUM(totalGols), " +
                        " ROUND(AVG(totalGols), 1), ROUND(AVG(totalCantos) ,1), Am.Ambas, Mum.maisum, Mdois.MaisDois, Gzero.Zerogol " +
                        " FROM fdados, " +

                        " (SELECT COUNT(*) as Ambas FROM fdados WHERE Liga ='" + liga.Trim() + "' AND golhome <> 0 AND golaway <> 0 " +
                        " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE()) Am, " +

                        " (SELECT COUNT(*) as Maisum FROM fdados WHERE Liga ='" + liga.Trim() + "' AND totalgols > 1 " +
                        " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE()) Mum, " +

                        " (SELECT COUNT(*) as MaisDois FROM fdados WHERE Liga ='" + liga.Trim() + "' AND totalgols > 2 " +
                        " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE()) Mdois, " +

                        " (SELECT COUNT(*) as ZeroGol FROM fdados WHERE Liga ='" + liga.Trim() + "' AND totalgols = 0 " +
                        " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE()) Gzero " +

                        " WHERE Liga ='" + liga.Trim() + "'" +
                        " AND Data BETWEEN '" + conectar.Data + "' AND CURDATE() ";

                    conectar.Calculo_geral_das_competicoes(sql);
                    bt_ambas_europa.Text = Convert.ToString(conectar.Total_Jogos_Ambas);
                    bt_total_jogos.Text = Convert.ToString(conectar.Total_jogos);
                    btb_total_gols_geral.Text = Convert.ToString(conectar.Total_gols);
                    bt_media_gols_geral.Text = Convert.ToString(conectar.Media_gols);
                    bt_media_cantos_geral.Text = Convert.ToString(conectar.Media_cantos);
                    bt_maisDe1Gol_geral.Text = Convert.ToString(conectar.Total_Jogos_Mais_de_1_Gol);
                    bt_maisde2Geral.Text = Convert.ToString(conectar.Total_jogos_mais_de_2_Gol);
                    bt_jos_semGols.Text = Convert.ToString(conectar.Total_Jogos_sem_Gols);
                    return;
                }
            }

        }

        public void Calculo_h2h_()
        {
            foreach (RadioButton rb in painel.Controls)
            {
                if (rb.Checked && rb.Text != "Brasileirao" && R_Hed_To_Hed.Checked == true && cbo_e.SelectedItem != null && cbo_Away.SelectedItem != null)
                {
                    string liga = rb.Text.Trim();
                    string home = cbo_e.SelectedItem.ToString();
                    string away = cbo_Away.SelectedItem.ToString();

                    int total_jogos = DGVResumo.RowCount;

                    //Calculando quantos jogos entre as equipes terminaram com mais de 1.5 gols
                    string sql_jogos_Mais_1_gol = "SELECT COUNT(*) FROM fdados" +
                        " WHERE Liga ='" + liga + "'" +
                        " AND home ='" + home + "'" +
                        " AND away ='" + away + "'" +
                        " AND totalgols > 1 " +
                        " OR Home ='" + away + "'" +
                        " AND away ='" + home + "'" +
                        " AND totalgols > 1";

                    //Jogos com mais de 2.5 gols
                    string sql_jogos_Mais_2_gol = "SELECT COUNT(*) FROM fdados" +
                        " WHERE Liga ='" + liga + "'" +
                        " AND home ='" + home + "'" +
                        " AND away ='" + away + "'" +
                        " AND totalgols > 2 " +
                        " OR Home ='" + away + "'" +
                        " AND away ='" + home + "'" +
                        " AND totalgols > 2";

                    /*/Media de Escanteios Entre As Equipes
                    string sql_Media_Cantos = "SELECT ROUND(AVG(totalCantos),2) FROM fdados " +
                        " WHERE Liga ='" + liga + "'" +
                        " AND home ='" + home + "'" +
                        " AND away ='" + away + "'" +
                        " OR home ='" + away + "'" +
                        " AND away ='" + home + "'";
                    */

                    //Solução usando Recurso Nativo
                    //------------------------------------------------------------------------
                    double media;
                    double total = 0;
                    int valor = 0;

                    foreach (DataGridViewRow row in DGVResumo.Rows)
                    {
                        total += Convert.ToDouble(row.Cells[5].Value);
                        if ((int)row.Cells[5].Value > 6)
                        {
                            valor++;
                        }
                    }

                    media = Math.Round(total / DGVResumo.RowCount, 2);
                    lbl_media_Cantos.Text = $"Nos Ultimos {total_jogos} Encontros A média de Escanteios Entre as Equipes é {media}";
                    lbl_europa_ambas.Text = $"Tivemos {valor} Jogos Com Mais de 6 Escanteios Entre as Equipes";
                    //-------------------------------------------------------------------------

                    conectar.Jogos_H2H(sql_jogos_Mais_1_gol, "De " + total_jogos + " Jogos ", lbl_europa_maisde_1gol, " bateu OVER 1.5");

                    conectar.Jogos_H2H(sql_jogos_Mais_2_gol, "De " + total_jogos + " Jogos ", lbl_europa_maisde2_gol, " bateu OVER 2.5");


                }
            }


        }

        public void ColorirLinhas()
        {
            foreach (DataGridViewRow row in DGVResumo.Rows)
            {
                if (row.Cells[0].Value.ToString() == "1" || row.Cells[0].Value.ToString() == "2" || row.Cells[0].Value.ToString() == "3" || row.Cells[0].Value.ToString() == "4")
                {
                    row.Cells[0].Style.ForeColor = Color.Black;
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                    row.Cells[0].Style.Font = new Font(DGVResumo.Font, FontStyle.Bold);

                }

                if (row.Cells[0].Value.ToString() == "17" || row.Cells[0].Value.ToString() == "18" || row.Cells[0].Value.ToString() == "19" || row.Cells[0].Value.ToString() == "20")
                {
                    row.Cells[0].Style.ForeColor = Color.Black;
                    row.Cells[0].Style.BackColor = Color.IndianRed;
                    row.Cells[0].Style.Font = new Font(DGVResumo.Font, FontStyle.Bold);
                }

                row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            }
        }

    }//fim da class
}  // fim name space
