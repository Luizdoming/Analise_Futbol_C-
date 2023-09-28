using System;
using System.Windows.Forms;

namespace StatisticasFutbol
{
    public partial class frm_Main : Form
    {
        public frm_Main()
        {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            using (frm_brasil fr = new frm_brasil())
            {
                fr.ShowDialog();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            using (frm_futbol fr = new frm_futbol())
            {
                fr.ShowDialog();
            }
        }

        private void btn_Ht_Click(object sender, EventArgs e)
        {
            using (frmHT fr = new frmHT())
            {
                fr.ShowDialog(this);
                //Thread t = new Thread(() => Application.Run(new frm_HT()));
                //t.Start();
            }
        }
    }
}
