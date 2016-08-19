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
    public partial class AddEntry : Form
    {
        public AddEntry()
        {
            InitializeComponent();

            //Add text to labels, buttons, and tabs
            tabPage1.Text = "Add Asset";
            tabPage2.Text = "Add Software";
            tabPage3.Text = "Add Employee";
            lblAddEmp.Text = "Please enter the first and last name of employee.";
            btnAddEmp.Text = "Add Employee";
            btnEmpReturn.Text = "Return to Menu";
            lblFName.Text = "First Name";
            lblLName.Text = "Last Name";

            //populate comboboxes lists from database on startup
            popAssetCB();
            popEmpCB();
            popSoftwareCB();

        }

        //connection strings
        string connectionString = "Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5";

        //button to return to Menu form on Employee tab
        private void btnEmpReturn_Click(object sender, EventArgs e)
        {
            this.Hide(); //hide Add form
            Menu menu = new Menu();
            menu.Show(); //show Menu form
        }

        //button to return to Menu form on Asset tab
        private void btnAssetReturn_Click(object sender, EventArgs e)
        {
            this.Hide(); //hide Add form
            Menu menu = new Menu();
            menu.Show(); //show Menu form
        }

        //button to return to menu from Software tab
        private void btnSoftwareReturn_Click(object sender, EventArgs e)
        {
            this.Hide(); //hide Add form
            Menu menu = new Menu();
            menu.Show(); //show Menu form
        }

        //button to add new employee to database
        private void btnAddEmp_Click(object sender, EventArgs e)
        {
            //sql query to add employee first and last names
            string query = "INSERT INTO dbo.Employees(fName,lName) VALUES(@fName,@lName);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@fName", SqlDbType.VarChar).Value = txtFName.Text;
                cmd.Parameters.Add("@lName", SqlDbType.VarChar).Value = txtLName.Text;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }


            //retrieve newly created empID
            string queryEmp = "SELECT TOP 1 empID FROM employees ORDER BY empID DESC";
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

            SqlCommand command = new SqlCommand(queryEmp); //sql command for query

            command.Connection = myConnection;
            myConnection.Open(); //open connection

            //initialize strings
            String empID;

            //get Strings with Reader
            using (myConnection)
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                empID = reader.GetInt32(0).ToString();
            }


            //sql query to add record to History
            string query2 = "INSERT INTO dbo.History(aID,empID,date) VALUES(@aID,@empID,@Date);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query2, con))
            {
                cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAssetChoice.SelectedValue;
                cmd.Parameters.Add("@empID", SqlDbType.Int).Value = empID;
                cmd.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            //create message box with confirmation text
            String confirm = "Entry Added!";
            MessageBox.Show(confirm);

            //refresh comboboxes
            popAssetCB();
            popEmpCB();
            popSoftwareCB();

            //clear text fields
            txtFName.Clear();
            txtLName.Clear();

        }

        //button to add new Asset to the database
        private void btnAddAsset_Click(object sender, EventArgs e)
        {
            //sql query to add Asset to database
            string query1 = "INSERT INTO dbo.Assets(make,model,assetTag,serialNum,empID) VALUES(@make,@model,@assetTag,@serialNum,@empID);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd2 = new SqlCommand(query1, con))
            {
                cmd2.Parameters.Add("@make", SqlDbType.VarChar).Value = txtMake.Text;
                cmd2.Parameters.Add("@model", SqlDbType.VarChar).Value = txtModel.Text;
                cmd2.Parameters.Add("@assetTag", SqlDbType.Int).Value = txtAID.Text;
                cmd2.Parameters.Add("@serialNum", SqlDbType.VarChar).Value = txtSN.Text;
                cmd2.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmpID.SelectedValue;

                con.Open();
                cmd2.ExecuteNonQuery();
                con.Close();
            }

            //retrieve newly created aID
            string queryAsset = "SELECT TOP 1 aID FROM assets ORDER BY aID DESC";
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");
            SqlCommand command = new SqlCommand(queryAsset); //sql command for query

            command.Connection = myConnection;
            myConnection.Open(); //open connection

            //initialize strings
            String aID;

            //get Strings with Reader
            using (myConnection)
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                aID = reader.GetInt32(0).ToString();
            }

            //sql query to add record to History
            string query2 = "INSERT INTO dbo.History(aID,empID,date) VALUES(@aID,@empID,@Date);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd2 = new SqlCommand(query2, con))
            {
                cmd2.Parameters.Add("@aID", SqlDbType.Int).Value = aID;
                cmd2.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmpID.SelectedValue;
                cmd2.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open();
                cmd2.ExecuteNonQuery();
                con.Close();
            }

            //message box confirmation message
            String confirm = "Entry Added!";
            MessageBox.Show(confirm);

            //refresh comboboxes
            popAssetCB();
            popEmpCB();
            popSoftwareCB();

            //clear fields
            txtAID.Clear();
            txtMake.Clear();
            txtModel.Clear();
            txtSN.Clear();
        }

        //button to add Software instance to the database
        private void btnAddSoftware_Click(object sender, EventArgs e)
        {
            //sql query to add Software Instance to database
            string query = "INSERT INTO dbo.swInstance(swID,productKey,aID) VALUES(@swID,@productKey,@aID);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@swID", SqlDbType.Int).Value = cbSoftwareChoice.SelectedValue;
                cmd.Parameters.Add("@productKey", SqlDbType.VarChar).Value = txtPK.Text;
                cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAssetChoice.SelectedValue;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            //retrieve newly created softwareID
            string queryAsset = "SELECT TOP 1 softwareID FROM swInstance ORDER BY softwareID DESC";
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");
            SqlCommand command = new SqlCommand(queryAsset); //sql command for query

            command.Connection = myConnection;
            myConnection.Open(); //open connection

            //initialize strings
            String softwareID;

            //get Strings with Reader
            using (myConnection)
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                softwareID = reader.GetInt32(0).ToString();
            }


            //sql query to add record to History
            string query2 = "INSERT INTO dbo.History(aID,softwareID,date) VALUES(@aID,@softwareID,@Date);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd2 = new SqlCommand(query2, con))
            {
                cmd2.Parameters.Add("@aID", SqlDbType.Int).Value = cbAssetChoice.SelectedValue;
                cmd2.Parameters.Add("@softwareID", SqlDbType.Int).Value = softwareID;
                cmd2.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open();
                cmd2.ExecuteNonQuery();
                con.Close();
            }

            //message box confirmation message
            String confirm = "Entry Added!";
            MessageBox.Show(confirm);

            //refresh comboboxes
            popAssetCB();
            popEmpCB();
            popSoftwareCB();

            //reset text fields
            txtPK.Clear();
        }

        //method to populate Employee ComboBox
        private void popEmpCB()
        {
            try
            {
                string query = "SELECT empID, (fName + ' ' + lName) AS Name FROM employees ORDER BY lName ASC"; //create a string for query

                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Employees"); //fill dataset from Employees

                cbEmpID.DisplayMember = "Name"; //display text in combox
                cbEmpID.ValueMember = "empID"; //value of member
                cbEmpID.DataSource = ds.Tables["Employees"]; //datasource

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
                string query = "SELECT aID, assetTag FROM assets ORDER BY assetTag ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Assets"); //fill dataset from Assets

                cbAssetChoice.DisplayMember = "assetTag"; //display text in combox
                cbAssetChoice.ValueMember = "aID"; //value of member
                cbAssetChoice.DataSource = ds.Tables["Assets"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //method to populate Software ComboBox
        private void popSoftwareCB()
        {

            try
            {
                string query = "SELECT swID, swName FROM software ORDER BY swName ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Software"); //fill dataset from Software

                cbSoftwareChoice.DisplayMember = "swName"; //display text in combobox
                cbSoftwareChoice.ValueMember = "swID"; //value of member
                cbSoftwareChoice.DataSource = ds.Tables["Software"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }
        }

        //tab select event method
        private void tabEmp_Selecting(object sender, TabControlCancelEventArgs e)
        {
            popAssetCB();
            popEmpCB();
            popSoftwareCB();
        }

    }

}
