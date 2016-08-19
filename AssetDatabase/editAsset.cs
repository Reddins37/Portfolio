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
    public partial class editAsset : Form
    {
        public editAsset()
        {
            InitializeComponent();

            //start text, labels, buttons visibilities as false
            txtMake.Visible = false;
            txtModel.Visible = false;
            txtSN.Visible = false;
            txtAssetTag.Visible = false;
            cbAddSoftware.Visible = false;
            btnAddSoftware.Visible = false;
            btnDeleteAsset.Visible = false;
            btnEditAssetInfo.Visible = false;
            btnNoOwner.Visible = false;

            //call methods to populate comboboxes
            popViewAssetCB();
            popAddSoftware();
        }

        //global SQL Connection
        string connectionString = "Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5";


        //button to return to menu window
        private void btnEditReturntoMenu_Click(object sender, EventArgs e)
        {
            this.Close();
            Menu menu = new Menu();
            menu.Show();
        }

        //populate the Asset selection combobox
        private void popViewAssetCB()
        {
            try
            {
                string query = "SELECT aID, assetTag FROM assets WHERE assetTag != 0 ORDER BY assetTag ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");
                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query
                myConnection.Open(); //open connection
                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Assets"); //fill dataset from Assets
                cbAsset.DisplayMember = "assetTag"; //display text in combox
                cbAsset.ValueMember = "aID"; //value of member
                cbAsset.DataSource = ds.Tables["Assets"]; //datasource
                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //populate the Software selection combobox
        private void popAddSoftware()
        {
            try
            {
                String query = "SELECT I.softwareID, (S.swName + ' - ' + I.productKey) AS swPK FROM swInstance I LEFT JOIN Software S ON I.swID = S.swID ORDER BY swName ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Software"); //fill dataset from Employees

                cbAddSoftware.DisplayMember = "swPK"; //display text in combox
                cbAddSoftware.ValueMember = "softwareID"; //value of member
                cbAddSoftware.DataSource = ds.Tables["Software"]; //datasource
                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //button to select Asset choice from combobox
        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                String query = "SELECT assetTag, make, model, serialNum FROM assets WHERE aID = " + cbAsset.SelectedValue; ; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlCommand command = new SqlCommand(query); //sql command for query
                command.Connection = myConnection;

                myConnection.Open(); //open connection

                String make, model, serialNum;
                String assetTag;

                using (myConnection)
                using (var reader = command.ExecuteReader())
                {
                    //use reader to get values from database
                    reader.Read();
                    assetTag = reader.GetInt32(0).ToString();
                    make = reader.GetString(1);
                    model = reader.GetString(2);
                    serialNum = reader.GetString(3);
                }

                //set textbox texts to values from the reader
                txtAssetTag.Text = assetTag;
                txtMake.Text = make;
                txtModel.Text = model;
                txtSN.Text = serialNum;

                myConnection.Close();
            }

            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
                MessageBox.Show(f.ToString());
            }

            txtMake.Visible = true;
            txtModel.Visible = true;
            txtSN.Visible = true;
            txtAssetTag.Visible = true;
            cbAddSoftware.Visible = true;
            btnAddSoftware.Visible = true;
            btnDeleteAsset.Visible = true;
            btnEditAssetInfo.Visible = true;
            btnNoOwner.Visible = true;
        }

        //button to confirm Asset Edit
        private void btnEditAssetInfo_Click(object sender, EventArgs e)
        {

            String query = "UPDATE dbo.Assets SET assetTag = @assetTag, make = @make, model = @model, serialNum = @serialNum WHERE aID = @aID";
            using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
            using (SqlCommand cmd = new SqlCommand(query, con)) //use new sql command that uses the string query
            {
                //query parameters
                cmd.Parameters.Add("@assetTag", SqlDbType.Int).Value = txtAssetTag.Text;
                cmd.Parameters.Add("@make", SqlDbType.VarChar).Value = txtMake.Text;
                cmd.Parameters.Add("@model", SqlDbType.VarChar).Value = txtModel.Text;
                cmd.Parameters.Add("@serialNum", SqlDbType.VarChar).Value = txtSN.Text;
                cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;

                con.Open(); //open connection
                cmd.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }
            popViewAssetCB(); //populate the view asset combobox to update it
            MessageBox.Show("Asset has been updated");
        }

        //button to delete Asset
        private void btnDeleteAsset_Click(object sender, EventArgs e)
        {
            string message = "Are you sure you want to delete this asset from the database? \n\nEmployees and software associated with this employee will not be deleted, but will have no longer be associated.";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            string caption = "Delete this asset?";

            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                //deletes from history tables
                String query1 = "DELETE FROM dbo.History WHERE aID = @aID";
                using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
                using (SqlCommand cmd = new SqlCommand(query1, con)) //use new sql command that uses the string query
                {
                    //query parameters
                    cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue; ;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }

                //changes any asset id to null for any software instance if it matches the id being deleted
                String query2 = "UPDATE dbo.swInstance SET aID = 2 WHERE aID = @aID";
                using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
                using (SqlCommand cmd = new SqlCommand(query2, con)) //use new sql command that uses the string query
                {
                    //query parameters
                    cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }

                //deletes the row from the asset table
                String query3 = "DELETE FROM dbo.Assets WHERE aID = @aID";
                using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
                using (SqlCommand cmd = new SqlCommand(query3, con)) //use new sql command that uses the string query
                {
                    //query parameters
                    cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }

                popViewAssetCB(); //populate the View Asset combobox to update it
                txtAssetTag.Clear();
                txtMake.Clear();
                txtModel.Clear();
                txtSN.Clear();
            }
        }

        //button to add Software to Asset
        private void btnAddSoftware_Click(object sender, EventArgs e)
        {
            String query = "UPDATE dbo.swInstance SET aID = @aID WHERE softwareID = @softwareID"; //create a string for query
            using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
            using (SqlCommand cmd = new SqlCommand(query, con)) //use new sql command that uses the string query
            {
                //query parameters
                cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;
                cmd.Parameters.Add("@softwareID", SqlDbType.Int).Value = cbAddSoftware.SelectedValue;

                con.Open(); //open connection
                cmd.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //Update history table
            string query2 = "INSERT INTO dbo.History(aID,softwareID,date) VALUES(@aID,@softwareID,@Date);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd2 = new SqlCommand(query2, con))
            {
                //query parameters
                cmd2.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;
                cmd2.Parameters.Add("@softwareID", SqlDbType.Int).Value = cbAddSoftware.SelectedValue;
                cmd2.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open(); //open connection
                cmd2.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //show confirmation message box
            MessageBox.Show("Software has been added to asset.");
        }

        //button to unassign empID for Asset
        private void btnNoOwner_Click(object sender, EventArgs e)
        {
            String query = "UPDATE dbo.Assets SET empID = 1 WHERE aID = @aID"; //create a string for query
            using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
            using (SqlCommand cmd = new SqlCommand(query, con)) //use new sql command that uses the string query
            {
                //query parameters
                cmd.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;


                con.Open(); //open connection
                cmd.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //Update history table
            string query2 = "INSERT INTO dbo.History(aID,empID,date) VALUES(@aID,@empID,@Date);";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd2 = new SqlCommand(query2, con))
            {
                //query parameters
                cmd2.Parameters.Add("@aID", SqlDbType.Int).Value = cbAsset.SelectedValue;
                cmd2.Parameters.Add("@empID", SqlDbType.Int).Value = 1;
                cmd2.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open(); //open connection
                cmd2.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //show confirmation message box
            MessageBox.Show("Asset now has no owner.");
        }
    }
}
