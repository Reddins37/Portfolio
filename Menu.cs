using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetDB1._3
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            AddEntry add = new AddEntry();
            add.Show();
        }

        private void btnMainEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnMenuView_Click(object sender, EventArgs e)
        {
            this.Hide();
            View view = new View();
            view.Show();
        }

        private void btnDeleteAndEdit_Click(object sender, EventArgs e)
        {
            this.Hide();
            editEmp editEmp = new editEmp();
            editEmp.Show();
        }

        private void btnDaEAsset_Click(object sender, EventArgs e)
        {
            this.Hide();
            editAsset editAsset = new editAsset();
            editAsset.Show();
        }

        private void btnDaESoftware_Click(object sender, EventArgs e)
        {
            this.Hide();
            editSoftware editSoftware = new editSoftware();
            editSoftware.Show();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            this.Hide();
            History history = new History();
            history.Show();
        }
    }
}
