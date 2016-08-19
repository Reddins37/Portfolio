using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace AssetDB1._3
{
    public partial class History : Form
    {
        public History()
        {
            InitializeComponent();

            //populate comboboxes
            popAssetCB();
            popEmpCB();
        }

      
        //method to populate Employee ComboBox
        private void popEmpCB()
        {
            try
            {
                string query = "SELECT empID, (fName + ' ' + lName) AS Name FROM employees WHERE empID != 1 ORDER BY lName ASC"; //create a string for query

                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Employees"); //fill dataset from Employees

                cbEmpHist.DisplayMember = "Name"; //display text in combox
                cbEmpHist.ValueMember = "empID"; //value of member
                cbEmpHist.DataSource = ds.Tables["Employees"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //method to populate Asset ComboBox
        private void popAssetCB()
        {
            try
            {
                string query = "SELECT aID, assetTag FROM assets WHERE assetTag != 0 ORDER BY assetTag ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Assets"); //fill dataset from Assets

                cbAssetHist.DisplayMember = "assetTag"; //display text in combox
                cbAssetHist.ValueMember = "aID"; //value of member
                cbAssetHist.DataSource = ds.Tables["Assets"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        private void btnEmpHistory_Click(object sender, EventArgs e)
        {
            String query = "EXEC selEmployeeHistory " + cbEmpHist.SelectedValue;
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

            SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for stored procedure
            myConnection.Open(); //open connection

            //Data Table
            DataTable dt = new DataTable(); //create new data table
            da.Fill(dt); //fill datatable from query

            //Fill and Format dataGridView
            dataEmpHistory.DataSource = dt; //datagridview source set equal to datatable
            dataEmpHistory.Columns[0].ReadOnly = true;
            dataEmpHistory.Columns[1].ReadOnly = true;
            dataEmpHistory.Columns[2].ReadOnly = true;
            dataEmpHistory.Columns[3].ReadOnly = true;
            dataEmpHistory.Columns[4].ReadOnly = true;
            dataEmpHistory.Columns[5].ReadOnly = true;
            dataEmpHistory.Columns[6].ReadOnly = true;

            myConnection.Close(); //close connection
        }

        private void btnAssetHistory_Click(object sender, EventArgs e)
        {
            String query = "EXEC selAssetHistory " + cbAssetHist.SelectedValue;
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

            SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for stored procedure
            myConnection.Open(); //open connection

            //Data Table
            DataTable dt = new DataTable(); //create new data table
            da.Fill(dt); //fill datatable from query

            //Fill and Format dataGridView
            dataAssetHistory.DataSource = dt; //datagridview source set equal to datatable
            dataAssetHistory.Columns[0].ReadOnly = true;
            dataAssetHistory.Columns[1].ReadOnly = true;
            dataAssetHistory.Columns[2].ReadOnly = true;
            dataAssetHistory.Columns[3].ReadOnly = true;
            dataAssetHistory.Columns[4].ReadOnly = true;
            dataAssetHistory.Columns[5].ReadOnly = true;
            dataAssetHistory.Columns[6].ReadOnly = true;
            dataAssetHistory.Columns[7].ReadOnly = true;
            dataAssetHistory.Columns[8].ReadOnly = true;


            myConnection.Close(); //close connection
        }

        private void btnEditReturntoMenu_Click(object sender, EventArgs e)
        {
            this.Close();
            Menu menu = new Menu();
            menu.Show();
        }
    }
}
