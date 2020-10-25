using System;
using System.Windows.Forms;

namespace BilgeAdam.AdoNet.FormApp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void msbProducts_Click(object sender, EventArgs e)
        {
            OpenForm<frmProducts>();
        }

        private void msnNewProduct_Click(object sender, EventArgs e)
        {
            OpenForm<frmNewProduct>();
        }

        public void OpenForm<T>() where T: Form
        {
            var f = Activator.CreateInstance<T>();
            f.MdiParent = this;
            f.Show();
        }

        private void msbSearchProduct_Click(object sender, EventArgs e)
        {
            OpenForm<frmSearch>();
        }
    }
}
