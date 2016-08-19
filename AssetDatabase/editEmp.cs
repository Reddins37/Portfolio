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
    public partial class editEmp : Form
    {
        public editEmp()
        {
            InitializeComponent();

            //text, buttons, etc. start visibility set to false
            txtfName.Visible = false;
            txtlName.Visible = false;
            btnGiveAsset.Visible = false;
            btnDeleteEmp.Visible = false;
            btnEditEmpInfo.Visible = false;
            cbGiveAsset.Visible = false;

            //populate comboboxes
            popViewEmpCB();
            popgiveAssetCB();
        }

        //global variables
        //SQL Connection
        string connectionString = "Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5";

        //populate View Employees ComboBox
        private void popViewEmpCB()
        {
            try
            {
                //string for query
                string query = "SELECT empID, (fName + ' ' + lName) as Name FROM employees WHERE empID != 1 ORDER BY lName ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Employees"); //fill dataset from Employees

                cbEmp.DisplayMember = "Name"; //display text in combox
                cbEmp.ValueMember = "empID"; //value of member
                cbEmp.DataSource = ds.Tables["Employees"]; //datasource

                myConnection.Close(); //close connection
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //populate View Asset ComboBox
        private void popgiveAssetCB()
        {
            try
            {
                string query = "SELECT aID, assetTag FROM assets WHERE assetTag != 0 ORDER BY assetTag ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Assets"); //fill dataset from Assets

                cbGiveAsset.DisplayMember = "assetTag"; //display text in combox
                cbGiveAsset.ValueMember = "aID"; //value of member
                cbGiveAsset.DataSource = ds.Tables["Assets"]; //datasource

                myConnection.Close(); //close connection
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //button to confirm employee information edits
        private void btnEditEmpInfo_Click(object sender, EventArgs e)
        {

            //sql query to update employee first and last names
            string query = "UPDATE dbo.Employees SET fName = @fName, lName = @lName WHERE empID = @empID;";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.Add("@fName", SqlDbType.VarChar).Value = txtfName.Text;
                cmd.Parameters.Add("@lName", SqlDbType.VarChar).Value = txtlName.Text;
                cmd.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmp.SelectedValue;

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                popViewEmpCB(); //populate View Employee ComboBox
            }

        }

        //button to select Employee
        private void Select_Click(object sender, EventArgs e)
        {
            try
            {
                //create a string for query
                string query = "SELECT fName, lName FROM employees WHERE empID = " + cbEmp.SelectedValue;
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");


                SqlCommand command = new SqlCommand(query); //sql command for query

                command.Connection = myConnection;
                myConnection.Open(); //open connection

                //initialize strings
                string fName, lName;

                //get Strings with Reader
                using (myConnection)
                using (var reader = command.ExecuteReader())
                {
                    reader.Read();
                    fName = reader.GetString(0);
                    lName = reader.GetString(1);
                }

                //set TextBox text
                txtfName.Text = fName;
                txtlName.Text = lName;

                //close connections
                myConnection.Close();
            }

            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
                MessageBox.Show(f.ToString());
            }

            //set Visibility
            txtfName.Visible = true;
            txtlName.Visible = true;
            cbGiveAsset.Visible = true;
            btnGiveAsset.Visible = true;
            btnDeleteEmp.Visible = true;
            btnEditEmpInfo.Visible = true;
        }

        //button to Return to Menu
        private void btnEditReturntoMenu_Click(object sender, EventArgs e)
        {
            this.Close();
            Menu menu = new Menu();
            menu.Show();
        }

        //button to Delete Employee Entry
        private void btnDeleteEmp_Click(object sender, EventArgs e)
        {
            string message = "Are you sure you want to delete this employee from the database? Assets and software associated with this employee will not be deleted, but will have no owner.";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            string caption = "Delete this employee?";
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                String query1 = "Delete FROM dbo.History WHERE empID = @empID";
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query1, con))
                {
                    //query parameters
                    cmd.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmp.SelectedValue;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }

                String query2 = "UPDATE dbo.Assets SET empID = 1 WHERE empID = @empID";
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query2, con))
                {
                    //query parameters
                    cmd.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmp.SelectedValue;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }


                String query3 = "DELETE FROM dbo.Employees WHERE empID = @empID";
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query3, con))
                {
                    //query parameters
                    cmd.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmp.SelectedValue;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }

                popViewEmpCB(); //populate View Employee ComboBox


            }
        }

        //button to confirm Give Asset to Employee
        private void btnGiveAsset_Click(object sender, EventArgs e)
        {
            //string for update query
            String query = "UPDATE dbo.Assets Set empID = @empID WHERE assetTag = @assetTag"; //create a string for query

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                //query parameters
                cmd.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmp.SelectedValue;
                cmd.Parameters.Add("@assetTag", SqlDbType.Int).Value = cbGiveAsset.SelectedValue;

                con.Open(); //open connection
                cmd.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //create History Log 

            //string for history query
            string query2 = "INSERT INTO dbo.History(aID,empID,date) VALUES(@aID,@empID,@date);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd2 = new SqlCommand(query2, con))
            {
                //query parameters
                cmd2.Parameters.Add("@aID", SqlDbType.Int).Value = cbGiveAsset.SelectedValue;
                cmd2.Parameters.Add("@empID", SqlDbType.Int).Value = cbEmp.SelectedValue;
                cmd2.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open(); //open connection
                cmd2.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //show confirmation message box
            MessageBox.Show("Asset has been given to employee.");
        }

    }
    
}
