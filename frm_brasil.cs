using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StatisticasFutbol
{
    public partial class frm_brasil : Form
    {
        public frm_brasil()
        {
            InitializeComponent();
        }

        private readonly Conexao cn = new Conexao();

        private void frm_brasil_Load(object sender, EventArgs e)
        {
            AdcionarDadosGridBrasil();
            CalcularMediaGeralCompeticao();
            img_awayBr.Visible = false;
            img_homeBr.Visible = false;
            lbl_br.Visible = false;
        }

        void AdcionarDadosGridBrasil()
        {
            int season = DateTime.Now.Year;
            Conexao conectar = new Conexao();

            try
            {
                //Usando windows Function no Mysql Para trazer os rank das equipes
                string Window_Function = "SELECT ROW_NUMBER() OVER (PARTITION BY Liga ORDER BY PT DESC) as '#' ," +
                                         " Liga, Equipe, Jogos, V, E, D, GF, GS, SG, PT, CS" +
                                         " FROM tbligas WHERE Liga = 'brasil' ORDER BY Liga, PT DESC ";

                conectar.AdcionarDadosCombo_Brasil(Convert.ToString(season), cbo_Home_brasil);
                conectar.AdcionarDadosCombo_Brasil(Convert.ToString(season), cbo_away_brasil);
                conectar.AdcionarDados_Grid(Window_Function, dgv_brasil);
                dgv_brasil.Columns[1].Visible = false;
                dgv_brasil.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                ColorirAsLinhasDaGridview();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao adcionar os dados do brasileirão " + ex.Message);
                conectar.Desconectar();
            }
        }

        private void cbo_Home_brasil_SelectedValueChanged(object sender, EventArgs e)
        {
            Conexao conectar = new Conexao();
            string equipe;

            string img = cbo_Home_brasil.SelectedItem.ToString() + ".png";
            img_homeBr.Image = Image.FromFile("img/" + img);
            img_homeBr.Visible = true;
            lbl_br.Visible = true;

            try
            {
                if (R_b_Home.Checked == true)
                {
                    equipe = cbo_Home_brasil.SelectedItem.ToString();

                    string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, Away, TotalE as E_Partida, TotalEHT as E_HT,         CartaoHome as Cartao_Casa, resultado as R " +
                            "FROM brasil " +
                            "WHERE Home ='" + equipe.Trim() + "'" +
                            "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                            "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC";

                    conectar.AdcionarDados_Grid(SQL_Home, dgv_brasil);
                    conectar.Modificar_Coluna_Gridview_Home(dgv_brasil, 8);
                    Calcular_jogos_Home();
                    return;
                }

                //Buscando os 10 ultimos jogos Home
                else if (R_10_Brasil.Checked == true)
                {
                    equipe = cbo_Home_brasil.SelectedItem.ToString();

                    string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, Away, TotalE as E " +
                            "FROM brasil " +
                            "WHERE home ='" + equipe.Trim() + "'" +
                            "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                            "OR Away ='" + equipe.Trim() + "'" +
                            "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                            "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 10";

                    conectar.AdcionarDados_Grid(SQL_Home, dgv_brasil);
                    Calcular_jogos_Home();
                    return;
                }
                // Buscar os 20 ultimos jogos Home
                else if (R_20_Brasil.Checked == true)
                {
                    equipe = cbo_Home_brasil.SelectedItem.ToString();

                    string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, Away, TotalE as E " +
                               "FROM brasil " +
                               "WHERE home ='" + equipe + "'" +
                               "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE()" +
                               "OR away ='" + equipe + "'" +
                               "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                               "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 20";

                    conectar.AdcionarDados_Grid(SQL_Home, dgv_brasil);
                    Calcular_jogos_Home();
                    return;
                }
                else
                {
                    H2h_Brasil();
                    Calcular_jogos_Home();
                    Calcular_h2h();
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

        private void r_tabela_CheckedChanged(object sender, EventArgs e)
        {
            string Window_Function = "SELECT ROW_NUMBER() OVER (PARTITION BY Liga ORDER BY PT DESC) as '#' ," +
                                         " Liga, Equipe, Jogos, V, E, D, GF, GS, SG, PT, CS" +
                                         " FROM tbligas WHERE Liga = 'brasil' ORDER BY Liga, PT DESC ";

            Conexao conectar = new Conexao();
            conectar.AdcionarDados_Grid(Window_Function, dgv_brasil);
            ColorirAsLinhasDaGridview();
            Limpar_Imagens_BR();
            dgv_brasil.Columns[1].Visible = false;
            dgv_brasil.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        public void H2h_Brasil()
        {
            string home;
            string away;
            Conexao conectar = new Conexao();

            try
            {
                conectar.Conectar();
                if (R_h2h_Brasil.Checked == true && cbo_away_brasil.SelectedItem != null && cbo_Home_brasil.SelectedItem != null)
                {
                    home = cbo_Home_brasil.SelectedItem.ToString();
                    away = cbo_away_brasil.SelectedItem.ToString();
                    string sql = "SELECT Data, Home as Mandante, golhome as GH, golaway as GA, Away as Visitante, TotalE as E FROM brasil" +
                        " WHERE home ='" + home + "'" +
                        " AND away ='" + away + "'" +
                        " OR home ='" + away + "'" +
                        " AND away ='" + home + "'" +
                        " ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, DAY(Data) DESC ";

                    conectar.AdcionarDados_Grid(sql, dgv_brasil);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao buscar jogos entre as equipes " + ex.Message);
            }
            finally { conectar.Desconectar(); }
        }

        private void cbo_away_brasil_SelectedValueChanged(object sender, EventArgs e)
        {
            Conexao conectar = new Conexao();
            string equipe;

            string img = cbo_away_brasil.SelectedItem.ToString() + ".png";
            img_awayBr.Image = Image.FromFile("img/" + img);

            img_awayBr.Visible = true;
            lbl_br.Visible = true;

            //para buscar jogos da equipe como visitante
            if (R_b_Away.Checked == true)
            {
                equipe = cbo_away_brasil.SelectedItem.ToString();

                string SQL_Away = "SELECT Data, Home, golHome as GH, golAway as GA, Away, TotalE as E_Partida, TotalEHT as E_HT,             CartaoAway as Cartao_Fora, resultado as R " +
                        "FROM brasil " +
                        "WHERE Away ='" + equipe.Trim() + "'" +
                        "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                        "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC";

                conectar.AdcionarDados_Grid(SQL_Away, dgv_brasil);
                conectar.Modificar_Gridview_Away(dgv_brasil, 8);
                Calcular_Jogos_Away();
                return;
            }
            //Buscando os 10 ultimos jogos fora
            else if (R_10_Brasil.Checked == true)
            {
                equipe = cbo_away_brasil.SelectedItem.ToString();

                string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, Away, TotalE as E " +
                        "FROM brasil " +
                        "WHERE Away ='" + equipe.Trim() + "'" +
                        "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                        "OR Home ='" + equipe.Trim() + "'" +
                        "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                        "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 10";

                conectar.AdcionarDados_Grid(SQL_Home, dgv_brasil);
                Calcular_Jogos_Away();
                return;
            }
            // Buscar os 20 ultimos jogos fora
            else if (R_20_Brasil.Checked == true)
            {
                equipe = cbo_away_brasil.SelectedItem.ToString();

                string SQL_Home = "SELECT Data, Home, golHome as GH, golAway as GA, Away, TotalE as E " +
                           "FROM brasil " +
                           "WHERE Away ='" + equipe.Trim() + "'" +
                           "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                           "OR Home ='" + equipe.Trim() + "'" +
                           "AND Data BETWEEN '" + cn.Data_Br + "' and CURRENT_DATE() " +
                           "ORDER BY YEAR(Data) DESC, MONTH(Data) DESC, Day(Data) DESC LIMIT 20";

                conectar.AdcionarDados_Grid(SQL_Home, dgv_brasil);
                Calcular_Jogos_Away();
                return;
            }
            else
            {
                H2h_Brasil();
                Calcular_Jogos_Away();
                Calcular_h2h();
            }

        }

        public void Calcular_Jogos_Away()
        {

            //Conta quantos jogos a equipe ja realizou fora de casa, quantos gols fez,  média.
            string equipe = cbo_away_brasil.SelectedItem.ToString();

            string sql = "SELECT COUNT(*), SUM(golaway), ROUND(AVG(golaway), 1), GhA.AVG_Golaway, GhA.Gol_Away, " +
                " T_JOGOS.TJ, JMais_um_Gol.JGMAIS_um_GOL, JMais_Dois_Gol.JGMAIS_dois_GOL," +
                " SemMarcarGols.Sem_Maracar_Gols, Mcantos_Away.MCaway " +
                " FROM brasil " +

                //Media de gols tomado jogando fora de casa, e quantos gols sofreu
                ", (SELECT SUM(golhome) as Gol_Away, ROUND(AVG(golhome), 1) as AVG_Golaway FROM brasil WHERE Away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE()) GhA " +
                //'" + conectar.Data_Br + "'

                // quantas vitorias obteve fora de casa
                ", (SELECT COUNT(*) as TJ FROM brasil WHERE Away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND Resultado = 'A') T_JOGOS " +

                //Jogos com mais de 1 gol
                ", (SELECT COUNT(*) as JGMAIS_Um_GOL FROM brasil WHERE Away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND totalgol > 1) JMais_um_Gol " +

                // jogos com amis de 2 gols
                ", (SELECT COUNT(*) as JGMAIS_dois_GOL FROM brasil WHERE Away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND totalgol > 2) JMais_Dois_Gol " +

                // Jogos onde a equipe ficou sem marcar jogando fora de casa
                ", (SELECT COUNT(*) as Sem_Maracar_Gols FROM brasil WHERE Away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND Golaway = 0 ) SemMarcarGols" +

                //Media de cantos da equipe Away - Visitante
                ", (SELECT ROUND(AVG(Eaway),1) as MCaway FROM brasil WHERE Away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() ) Mcantos_Away " +

                " WHERE away ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() ";

            cn.Calculo_geral_das_competicoes(sql);
            Br_jogosAway.Text = Convert.ToString(cn.Total_jogos);
            Br_golsaway.Text = Convert.ToString(cn.Total_gols);
            Br_mediagosAway.Text = Convert.ToString(cn.Media_gols);
            Br_totalsofridoaway.Text = Convert.ToString(cn.Total_Jogos_Ambas);
            Br_mediasofridoaway.Text = Convert.ToString(cn.Media_cantos);
            Br_Vitoriasaway.Text = Convert.ToString(cn.Total_Jogos_Mais_de_1_Gol);
            Br_mais_1_Away.Text = Convert.ToString(cn.Total_jogos_mais_de_2_Gol);
            Br_mais_2_away.Text = Convert.ToString(cn.Total_Jogos_sem_Gols);
            Br_semtomagolAway.Text = Convert.ToString(cn.Mcantos_HT);
            btn_media_canto_away.Text = Convert.ToString(cn.CV);
        }

        public void Calcular_jogos_Home()
        {

            //Conta quantos jogos a equipe ja realizou fora de casa, quantos gols fez,  média.
            string equipe = cbo_Home_brasil.SelectedItem.ToString();

            string sql = "SELECT COUNT(*), SUM(golhome), ROUND(AVG(golhome), 1), GhA.AVG_Golaway, GhA.Gol_Away, " +
                " T_JOGOS.TJ, JMais_um_Gol.JGMAIS_um_GOL, JMais_Dois_Gol.JGMAIS_dois_GOL," +
                " SemMarcarGols.Sem_Maracar_Gols, Mcantos_Away.MCaway " +
                " FROM brasil " +

                //Media de gols tomado jogando fora de casa, e quantos gols sofreu
                ", (SELECT SUM(golaway) as Gol_Away, ROUND(AVG(golaway), 1) as AVG_Golaway FROM brasil WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE()) GhA " +

                // quantas vitorias obteve fora de casa
                ", (SELECT COUNT(*) as TJ FROM brasil WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND Resultado = 'H') T_JOGOS " +

                //Jogos com mais de 1 gol
                ", (SELECT COUNT(*) as JGMAIS_Um_GOL FROM brasil WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND totalgol > 1) JMais_um_Gol " +

                // jogos com amis de 2 gols
                ", (SELECT COUNT(*) as JGMAIS_dois_GOL FROM brasil WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND totalgol > 2) JMais_Dois_Gol " +

                // Jogos onde a equipe ficou sem marcar jogando fora de casa
                ", (SELECT COUNT(*) as Sem_Maracar_Gols FROM brasil WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() AND Golhome = 0 ) SemMarcarGols" +

                //Media de cantos da equipe Away - Visitante
                ", (SELECT ROUND(AVG(Ehome),1) as MCaway FROM brasil WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() ) Mcantos_Away " +

                " WHERE home ='" + equipe + "' AND Data BETWEEN '" + cn.Data_Br + "' AND CURDATE() ";

            cn.Calculo_geral_das_competicoes(sql);
            Br_total_Jogos.Text = Convert.ToString(cn.Total_jogos);
            Br_golsFeito.Text = Convert.ToString(cn.Total_gols);
            Br_mediagolFeito.Text = Convert.ToString(cn.Media_gols);
            Br_totalGolSofrido.Text = Convert.ToString(cn.Total_Jogos_Ambas);
            Br_MediaGolSofrido.Text = Convert.ToString(cn.Media_cantos);
            Br_Vitoria.Text = Convert.ToString(cn.Total_Jogos_Mais_de_1_Gol);
            Br_MaisDe_1_Gol.Text = Convert.ToString(cn.Total_jogos_mais_de_2_Gol);
            Br_Maisde_2_gol.Text = Convert.ToString(cn.Total_Jogos_sem_Gols);
            Br_JogosSemMarcar.Text = Convert.ToString(cn.Mcantos_HT);
            btn_media_canto_home.Text = Convert.ToString(cn.CV);
        }

        // Método para realizar os calculos de resultados entre as equipes
        public void Calcular_h2h()
        {
            Conexao conectar = new Conexao();
            int totalJogos;

            if (R_h2h_Brasil.Checked == true && cbo_Home_brasil.SelectedItem != null && cbo_away_brasil.SelectedItem != null)
            {
                try
                {
                    string equipe_away = cbo_away_brasil.SelectedItem.ToString();
                    string equipe_home = cbo_Home_brasil.SelectedItem.ToString();

                    // Calcula quantos jogos saiu mais de 1 gols
                    string sql_Mais_1_gol = "SELECT COUNT(*) FROM brasil " +
                                            " WHERE totalgol > 1 " +
                                            " AND home ='" + equipe_home + "'" +
                                            " AND away ='" + equipe_away + "'" +
                                            " OR home = '" + equipe_away + "'" +
                                            " AND away = '" + equipe_home + "'" +
                                            " AND totalgol > 1";

                    //Calcula quantos jogos saiu mais de 2 gols
                    string sql_Jogos_Mais_2_gol = "SELECT COUNT(*) FROM brasil " +
                                            " WHERE totalgol > 2 " +
                                            " AND home ='" + equipe_home + "'" +
                                            " AND away ='" + equipe_away + "'" +
                                            " OR home = '" + equipe_away + "'" +
                                            " AND away = '" + equipe_home + "'" +
                                            " AND totalgol > 2";

                    // Calcula quantos jogos ambas equipes marcaram
                    string sql_ambasBR = "SELECT COUNT(*) FROM brasil" + "" +
                                        " WHERE home ='" + equipe_home + "'" +
                                        " AND away ='" + equipe_away + "'" +
                                        " AND golhome > 0  and golaway > 0" +
                                        " OR home ='" + equipe_away + "'" +
                                        " AND away ='" + equipe_home + "'" +
                                        " AND golhome > 0 AND golaway > 0" +
                                        " AND Data BETWEEN '2010-01-01' AND CURDATE()";



                    //Calcular quantos jogos a equipe da casa esta a marcar mais de 1 gol nas partidas anteriores
                    string sql_Jogos_mais_de_1_GolMarcadonasUltimasPartidas = "SELECT COUNT(*) FROM brasil " +
                        " WHERE Home ='" + equipe_home + "'" +
                        " AND golhome > 1  " +
                        " AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE() " +
                        " OR away ='" + equipe_home + "'" +
                        " AND golaway > 1 " +
                        " AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE() LIMIT 10 ";

                    string sql_Jogos_mais_de_1_GolMarcadonasUltimasPartidas_away = "SELECT COUNT(*) FROM brasil " +
                        " WHERE away ='" + equipe_away + "'" +
                        " AND golaway > 1  " +
                        " AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE() " +
                        " OR home ='" + equipe_away + "'" +
                        " AND golhome > 1 " +
                        " AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE() LIMIT 10 ";

                    //Quantos Jogos Tiveram mais de 1.5 gols Entre as Equipes Nos Ultimos Confrontos
                    string sql_Jogos_mais_de_1_Gol_Entre_As_Equipes = "SELECT COUNT(*) FROM brasil " +
                        " WHERE Away ='" + equipe_away + "'" +
                        " AND Home ='" + equipe_home + "'" +
                        " AND golaway > 0 AND golHome > 0 " +
                        " OR Home ='" + equipe_away + "'" +
                        " AND Away ='" + equipe_home + "'" +
                        " AND Golhome > 0 AND golaway > 0 AND Data BETWEEN '2010-01-01' AND CURDATE() ";

                    // Total de jogos H2H
                    totalJogos = dgv_brasil.RowCount;

                    conectar.Jogos_H2H(sql_Mais_1_gol, "Nos ultimos encontros ", lbl_mais_1_Gol, "Jogos bateu a média Over 1.5");

                    conectar.Jogos_H2H(sql_Jogos_Mais_2_gol, "De " + totalJogos + " Jogos disputados entre as equipes, tivemos", lbl_mais_de_2_gol, " com Over 2.5");

                    conectar.Jogos_H2H(sql_ambasBR, "BTTS - ", lbl_ambas, " Jogos Bateu nos ultimos " + totalJogos + " confrontos");

                    conectar.Jogos_H2H(sql_Jogos_mais_de_1_GolMarcadonasUltimasPartidas, equipe_home + " Marcou 1 ou mais em ", lbl_jogos_maisde_1_gol_casa, " nas ultimas 10 partidas");

                    conectar.Jogos_H2H(sql_Jogos_mais_de_1_GolMarcadonasUltimasPartidas_away, equipe_away + " Marcou 1 ou mais em ", lbl_jogos_mais_de_1_away, " nas ultimas 10 partidas");

                    // conectar.Jogos_H2H(sql_Jogos_mais_de_1_Gol_Entre_As_Equipes, "De " + totalJogos + " Jogos Entre as Equipes Tivemos ", lbl_jogos_HT_Cantos_HT, " Jogos Que Bateu OVER 1.5");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Erro ao realisar as istatisticas do h2h " + ex.Message);
                }
                finally
                {
                    conectar.Desconectar();
                }
            }
        }

        public void Limpar_Imagens_BR()
        {
            if (img_awayBr.Image != null || img_homeBr.Image != null)
            {
                img_homeBr.Image = null;
                img_awayBr.Image = null;
            }

            foreach (Label texto in this.painel_br_h2h.Controls)
            {
                texto.Text = string.Empty;
            }

            foreach (Control bt in this.Controls)
            {
                if (bt is Guna2Button)
                {
                    if (bt.Tag.Equals("A") || bt.Tag.Equals("H"))
                    {
                        bt.Text = "0";
                    }
                }

            }
        }

        public void CalcularMediaGeralCompeticao()
        {
            Conexao conectar = new Conexao();

            //Quantos jogos foram realizado até o momento
            string sql = "SELECT COUNT(*), SUM(totalgol), ROUND(AVG(totalgol), 1), MCantos.Mca, Am.Ambas, Maisum.MaisdeUm , Mdois.MaisDois " +
                ", Sgol.SemGols, MCHT.McaHT FROM brasil, " +

                " (SELECT COUNT(*) as Ambas FROM brasil WHERE golhome > 0 AND golaway > 0 AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE()) Am , " +
                " (SELECT COUNT(*) as MaisdeUm FROM brasil  WHERE totalgol > 1 AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE()) Maisum, " +
                " (SELECT COUNT(*) as MaisDois FROM brasil WHERE totalgol > 2 AND Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE()) Mdois, " +
                " (SELECT COUNT(*) as SemGols FROM brasil where totalgol = 0 and data between '" + conectar.Data_Br + "' AND CURDATE()) Sgol," +

                " (SELECT ROUND(AVG(TotalE), 1) as Mca FROM brasil WHERE Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE()) MCantos, " +

                " (SELECT ROUND(AVG(TotalEHT), 1) as McaHT FROM brasil WHERE Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE()) MCHT " +

                " WHERE Data BETWEEN '" + conectar.Data_Br + "' AND CURDATE()";

            conectar.Calculo_geral_das_competicoes(sql);
            bt_jogos_geral.Text = Convert.ToString(conectar.Total_jogos);
            bt_totalGol_geral.Text = Convert.ToString(conectar.Total_gols);
            br_media_gol_geral.Text = Convert.ToString(conectar.Media_gols);
            bt_ambas_geral.Text = Convert.ToString(conectar.Total_Jogos_Ambas);
            bt_mais_1_geral.Text = Convert.ToString(conectar.Total_Jogos_Mais_de_1_Gol);
            bt_mais_2_gearl.Text = Convert.ToString(conectar.Total_jogos_mais_de_2_Gol);
            bt_Mcantos.Text = Convert.ToString(conectar.Media_cantos);
            bt_semgols_geral.Text = Convert.ToString(conectar.Total_Jogos_sem_Gols);
            bt_M_Canto_HT.Text = Convert.ToString(conectar.Mcantos_HT);
        }

        private void R_Mais_De_Um_gol_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // Jogos que obteveram gols no HT
                Limpar_Imagens_BR();
                int total;
                string sql = "SELECT * FROM jogosMaisDeUmGol ";
                cn.AdcionarDados_Grid(sql, dgv_brasil);
                total = dgv_brasil.RowCount;
                this.lbl_jogos_HT_Cantos_HT.Text = "Tivemos um total de " + Convert.ToString(total) +
                                                                        " jogos Com 1 gol ou mais no HT";

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao buscar os dados da view " + "\n" + ex.Message);
            }
            finally { cn.Desconectar(); }
        }

        private void R_Cantos_HT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Limpar_Imagens_BR();
                int total;
                string sql = "SELECT * FROM jogosComMaisDeTresCantosNoHT ";

                cn.AdcionarDados_Grid(sql, dgv_brasil);
                total = dgv_brasil.RowCount;

                this.lbl_jogos_HT_Cantos_HT.Text = "Tivemos um total de " + Convert.ToString(total) + " jogos Com mais de 3 escanteios no HT";
            }

            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao Buscar os dados de cantos no HT " + "\n" + ex.Message);
            }
            finally { cn.Desconectar(); }
        }

        private void R_10_Brasil_CheckedChanged(object sender, EventArgs e)
        {
            Limpar_Imagens_BR();
        }

        private void R_20_Brasil_CheckedChanged(object sender, EventArgs e)
        {
            Limpar_Imagens_BR();
        }

        private void R_b_Home_CheckedChanged(object sender, EventArgs e)
        {
            Limpar_Imagens_BR();
        }

        private void R_b_Away_CheckedChanged(object sender, EventArgs e)
        {
            Limpar_Imagens_BR();
        }

        private void R_h2h_Brasil_CheckedChanged(object sender, EventArgs e)
        {
            Limpar_Imagens_BR();
        }

        private void ColorirAsLinhasDaGridview()
        {
            foreach (DataGridViewRow row in dgv_brasil.Rows)
            {
                if (row.Cells[0].Value.ToString() == "1" || row.Cells[0].Value.ToString() == "2" || row.Cells[0].Value.ToString() == "3" || row.Cells[0].Value.ToString() == "4")
                {
                    row.Cells[0].Style.ForeColor = Color.Black;
                    row.Cells[0].Style.BackColor = Color.LightGreen;
                    row.Cells[0].Style.Font = new Font(dgv_brasil.Font, FontStyle.Bold);

                }

                if (row.Cells[0].Value.ToString() == "17" || row.Cells[0].Value.ToString() == "18" || row.Cells[0].Value.ToString() == "19" || row.Cells[0].Value.ToString() == "20")
                {
                    row.Cells[0].Style.ForeColor = Color.Black;
                    row.Cells[0].Style.BackColor = Color.IndianRed;
                    row.Cells[0].Style.Font = new Font(dgv_brasil.Font, FontStyle.Bold);
                }

                row.Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                row.Cells[5].Selected = false;

            }
        }

        private void frm_brasil_FormClosed(object sender, FormClosedEventArgs e)
        {
            //using (frm_futbol fr = new frm_futbol())
            //{
            //    Thread t = new Thread(() => Application.Run(new frm_futbol()));
            //    t.Start();
            //}
        }

        private void AdcionarJogosSemGols()
        {
            string sql = "SELECT Data, Home, GolHome as GH, GolAway as GA , Away, TotalE as Escanteios, TotalC as Cartão FROM Brasil" +
                        " WHERE YEAR(Data) = YEAR(NOW())" +
                        " AND GolHome = 0 AND GolAway = 0" +
                        " ORDER BY Data DESC";
            try
            {
                cn.AdcionarDados_Grid(sql, dgv_brasil);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao Buscar os dados dos Jogos sem Gols \n" + ex.Message);
            }
            finally { cn.Desconectar(); }

            lbl_jogos_HT_Cantos_HT.Text = "Tivemos " + dgv_brasil.RowCount + " Jogos Com Placar de 0 x 0";

        }

        private void rb_JogosSemGols_CheckedChanged(object sender, EventArgs e)
        {
            AdcionarJogosSemGols();
        }

        private void Rb_CartaHome_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT Home, SUM(CartaoHome) as Quantidade, ROUND(AVG(CartaoHome), 2) as Media FROM brasil "
                            + "WHERE YEAR(Data) = YEAR(Now()) GROUP BY Home ORDER BY Quantidade DESC";
                cn.AdcionarDados_Grid(sql, dgv_brasil);

                foreach (DataGridViewRow rows in dgv_brasil.Rows)
                {
                    rows.Cells[0].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                    rows.Cells[1].Style.Alignment = DataGridViewContentAlignment.TopCenter;
                    rows.Cells[2].Style.Alignment = DataGridViewContentAlignment.TopCenter;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao Buscar Os Dados Sobre Cartãos Das Equipes Jogando Em Casa" + "\n" + ex.Message);
            }
            finally { cn.Desconectar(); }
        }

        private void Rb_CartaoAway_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT Away, SUM(CartaoAway) as Quantidade, ROUND(AVG(CartaoAway), 2) as Media FROM brasil "
                            + "WHERE YEAR(Data) = YEAR(Now()) GROUP BY Away ORDER BY Quantidade DESC";
                cn.AdcionarDados_Grid(sql, dgv_brasil);

                foreach (DataGridViewRow rows in dgv_brasil.Rows)
                {
                    rows.Cells[0].Style.Alignment = DataGridViewContentAlignment.TopLeft;
                    rows.Cells[1].Style.Alignment = DataGridViewContentAlignment.TopCenter;
                    rows.Cells[2].Style.Alignment = DataGridViewContentAlignment.TopCenter;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao Buscar Os Dados Sobre Cartão Das Equipes Jogando Fora" + "\n" + ex.Message);
            }
            finally { cn.Desconectar(); }
        }
    }//fim class
}// fim namespace


