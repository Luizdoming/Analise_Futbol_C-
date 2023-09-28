using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace StatisticasFutbol
{
    public class Conexao
    {
        //string de conexão
        readonly string strconexao = "datasource=localhost; port=3306; username=root; password=270815**;database=futbol";

        // conector
        public MySqlConnection cn;


        //Criaçao das propriedades
        public double Media_cantos { get; set; }
        public int Total_gols { get; set; }
        public double Media_gols { get; set; }
        public int Total_jogos { get; set; }
        public int Total_Jogos_Ambas { get; set; }
        public int Total_Jogos_sem_Gols { get; set; }
        public int Total_Jogos_Mais_de_1_Gol { get; set; }
        public int Total_jogos_mais_de_2_Gol { get; set; }
        public double Mcantos_HT { get; set; }
        public double CV { get; set; }

        //Data referente ao inicio dos capeonatos Europeus
        public string Data = "2023-08-01";

        //Data referente ao inicio do capeonato brasileiro
        public string Data_Br = "2023-04-01";

        //metodo para conectar ao banco de dados
        public void Conectar()
        {
            try
            {
                cn = new MySqlConnection(strconexao);
                cn.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao conectar com o banco de dados" + e.Message);
                return;
            }
        }

        //metodo para fechar a conexao com obanco de dados
        public void Desconectar()
        {
            try
            {
                using (cn = new MySqlConnection(strconexao))
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("erro ao fechar a conexão" + e.Message);
                return;
            }
        }

        public void AdcionarDados_ComboLigas(string liga, Guna2ComboBox combobox)
        {
            try
            {
                Conectar();
                string sql = "SELECT DISTINCT Home FROM fdados WHERE Liga ='" + liga + "'";
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    MySqlDataReader dr = null;
                    dr = cmd.ExecuteReader();
                    // limpamos a combobox
                    combobox.Items.Clear();
                    while (dr.Read())
                    {
                        combobox.Items.Add(dr.GetString(0).ToString());
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao adcionar os dados" + e.Message);
                return;
            }
            finally { Desconectar(); }
        }

        public void AdcionarDados_Grid(string sql, Guna2DataGridView dgv)
        {
            try
            {
                Conectar();
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    //para adcionar dados dentro de uma DataGridview - usamos o Adapter
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
                    DataTable dt = new DataTable();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dt);
                    dgv.DataSource = dt;
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao adcionar os dados na datagridview" + e.Message);
                return;
            }
            finally { Desconectar(); }
        }

        public void funcaoAgregacaoHome(string sql, Guna2Button L_jogos, Guna2Button L_GolsFeitos,
            Guna2Button L_MediaGolFeitos, Guna2Button M_Escanteios)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0) || dr.IsDBNull(1) || dr.IsDBNull(2) || dr.IsDBNull(3))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada", "Aviso!");
                    return;
                }
                else
                {
                    L_jogos.Text = dr.GetString(0).ToString();
                    L_GolsFeitos.Text = dr.GetString(1).ToString();
                    L_MediaGolFeitos.Text = dr.GetString(2).ToString();
                    M_Escanteios.Text = dr.GetString(3).ToString();
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }
        }


        public void FuncaoAgregacaoAway(string sql, Guna2Button L_Gols_sofridos, Guna2Button L_Media_Gols_sofridos)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0) || dr.IsDBNull(1))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada", "Aviso!");
                    return;
                }
                else
                {
                    L_Gols_sofridos.Text = dr.GetString(0).ToString();
                    L_Media_Gols_sofridos.Text = dr.GetString(1).ToString();
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }
        }

        public void funcaoAgregacao_Jogos_Acima(string sql, Guna2Button L_Jogos_Acima)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada", "Aviso!");
                    return;
                }
                else
                {
                    L_Jogos_Acima.Text = dr.GetString(0).ToString();
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }
        }

        public void AdcionarDadosCombo_Brasil(string liga, Guna2ComboBox combobox)
        {
            try
            {
                Conectar();
                string sql = "SELECT DISTINCT Home FROM brasil WHERE Season = " + liga;
                using (MySqlCommand cmd = new MySqlCommand(sql, cn))
                {
                    MySqlDataReader dr;
                    dr = cmd.ExecuteReader();
                    //Limpar o combobox
                    combobox.Items.Clear();
                    while (dr.Read())
                    {
                        //Se o Campo for diferente de null, vai adcionar os dados dentro da combobox 
                        if (!dr.IsDBNull(0))
                            combobox.Items.Add(dr.GetString(0));
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao adcionar os dados na como brasileirão " + e.Message);
            }
            finally { Desconectar(); }
        }

        public void Modificar_Coluna_Gridview_Home(Guna2DataGridView dataGridView, int coluna)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                String value;

                if (Convert.ToUInt16(row.Cells[2].Value.ToString()) > Convert.ToInt16(row.Cells[3].Value.ToString()))
                {
                    value = row.Cells[coluna].Value.ToString();

                    row.Cells[coluna].Value = value.Replace(value, "V");

                    row.Cells[coluna].Style.ForeColor = Color.White;

                    row.Cells[coluna].Style.BackColor = Color.DarkSeaGreen;

                    row.Cells[coluna].Style.Font = new Font(dataGridView.Font, FontStyle.Bold);
                }
                else if (Convert.ToUInt16(row.Cells[2].Value.ToString()) == Convert.ToInt16(row.Cells[3].Value.ToString()))
                {
                    value = row.Cells[coluna].Value.ToString();

                    row.Cells[coluna].Value = value.Replace(value, "E");

                    row.Cells[coluna].Style.ForeColor = Color.White;

                    row.Cells[coluna].Style.BackColor = Color.DarkSlateGray;

                    row.Cells[coluna].Style.Font = new Font(dataGridView.Font, FontStyle.Bold);
                }
                else if (Convert.ToUInt16(row.Cells[2].Value.ToString()) < Convert.ToInt16(row.Cells[3].Value.ToString()))
                {
                    value = row.Cells[coluna].Value.ToString();
                    row.Cells[coluna].Value = value.Replace(value, "D");
                    row.Cells[coluna].Style.ForeColor = Color.White;
                    row.Cells[coluna].Style.BackColor = Color.Red;
                    row.Cells[coluna].Style.Font = new Font(dataGridView.Font, FontStyle.Bold);
                }
                row.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        public void Modificar_Gridview_Away(Guna2DataGridView dgv, int coluna)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                String value;

                if (Convert.ToUInt16(row.Cells[2].Value.ToString()) > Convert.ToInt16(row.Cells[3].Value.ToString()))
                {
                    value = row.Cells[coluna].Value.ToString();

                    row.Cells[coluna].Value = value.Replace(value, "D");

                    row.Cells[coluna].Style.ForeColor = Color.White;

                    row.Cells[coluna].Style.BackColor = Color.DarkRed;

                    row.Cells[coluna].Style.Font = new Font(dgv.Font, FontStyle.Bold);
                }
                else if (Convert.ToUInt16(row.Cells[2].Value.ToString()) == Convert.ToInt16(row.Cells[3].Value.ToString()))
                {
                    value = row.Cells[coluna].Value.ToString();

                    row.Cells[coluna].Value = value.Replace(value, "E");

                    row.Cells[coluna].Style.ForeColor = Color.White;

                    row.Cells[coluna].Style.BackColor = Color.DarkGray;

                    row.Cells[coluna].Style.Font = new Font(dgv.Font, FontStyle.Bold);
                }
                else if (Convert.ToUInt16(row.Cells[2].Value.ToString()) < Convert.ToInt16(row.Cells[3].Value.ToString()))
                {
                    value = row.Cells[coluna].Value.ToString();
                    row.Cells[coluna].Value = value.Replace(value, "V");
                    row.Cells[coluna].Style.ForeColor = Color.White;
                    row.Cells[coluna].Style.BackColor = Color.DarkSeaGreen;
                    row.Cells[coluna].Style.Font = new Font(dgv.Font, FontStyle.Bold);
                }
                row.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        public void FuncaoBR_Jogos_Home(string sql, Guna2Button bt_total_jogos, Guna2Button bt_total_gols_Feito, Guna2Button Bt_Media_gols_Feito)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0) || dr.IsDBNull(1) || dr.IsDBNull(2))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada", "Aviso!");
                    return;
                }
                else
                {
                    bt_total_jogos.Text = dr.GetString(0).ToString();
                    bt_total_gols_Feito.Text = dr.GetString(1).ToString();
                    Bt_Media_gols_Feito.Text = dr.GetString(2).ToString();
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }
        }

        public void Funcao_BR_jogos_Fora(string sql, Guna2Button bt_gols_Sofridos, Guna2Button bt_Media_gols_sofridos)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0) || dr.IsDBNull(1))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada", "Aviso!");
                    return;
                }
                else
                {
                    bt_gols_Sofridos.Text = dr.GetString(0).ToString();
                    bt_Media_gols_sofridos.Text = dr.GetString(1).ToString();
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }

        }

        public void Jogos_BR_Acima_de_2Gols_1Gols(string sql, Guna2Button bt_Total)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada BR", "Aviso!");
                    return;
                }
                else
                {
                    bt_Total.Text = dr.GetString(0).ToString();
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }
        }

        public void Jogos_H2H(string sql, string texto, Label textBox, string texto2)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (dr.IsDBNull(0))
                {
                    MessageBox.Show("Não existe dados para a equipe selecionada H2H", "Aviso!");
                    return;
                }
                else
                {
                    textBox.Text = texto + " " + dr.GetString(0).ToString() + " " + texto2;
                }

            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo" + e.Message);
            }
            finally { Desconectar(); }
        }

        public void Calculo_geral_das_competicoes(string sql)
        {
            try
            {
                Conectar();
                MySqlCommand cmd = new MySqlCommand(sql, cn);
                MySqlDataReader dr;
                dr = cmd.ExecuteReader();
                dr.Read();

                if (!dr.IsDBNull(0))
                {
                    Total_jogos = dr.GetInt16(0);
                }

                if (!dr.IsDBNull(1))
                {
                    Total_gols = dr.GetInt16(1);
                }

                if (!dr.IsDBNull(2))
                {
                    Media_gols = dr.GetDouble(2);
                }

                if (dr.FieldCount > 3)
                {
                    if (!dr.IsDBNull(3))
                    {
                        Media_cantos = dr.GetDouble(3);
                    }
                }

                if (dr.FieldCount > 4)
                {
                    if (!dr.IsDBNull(4))
                    {
                        Total_Jogos_Ambas = dr.GetInt16(4);
                    }
                }

                if (dr.FieldCount > 5)
                {
                    if (!dr.IsDBNull(5))
                    {
                        Total_Jogos_Mais_de_1_Gol = dr.GetInt16(5);
                    }
                }

                if (dr.FieldCount > 6)
                {
                    if (!dr.IsDBNull(6))
                    {
                        Total_jogos_mais_de_2_Gol = dr.GetInt16(6);
                    }
                }

                if (dr.FieldCount > 7)
                {
                    if (!dr.IsDBNull(7))
                    {
                        Total_Jogos_sem_Gols = dr.GetInt16(7);
                    }
                }

                if (dr.FieldCount > 8)
                {
                    if (!dr.IsDBNull(8))
                    {
                        Mcantos_HT = dr.GetDouble(8);
                    }
                }

                if (dr.FieldCount > 9)
                {
                    if (!dr.IsDBNull(9))
                    {
                        CV = dr.GetDouble(9);
                    }
                }
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erro ao realizar o calculo da competição " + e.Message);
            }
            finally { Desconectar(); }
        }
    }
}
