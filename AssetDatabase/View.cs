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
    public partial class View : Form
    {
        public View()
        {
            InitializeComponent();

            //Populate ComboBoxes
            popViewAssetCB();
            popViewEmpCB();
            popViewSoftwareCB();

            //Cell End Edit Events Initialization
            dataViewEmployee.CellEndEdit += DataViewEmployee_CellEndEdit;
            dataViewAsset.CellEndEdit += DataViewAsset_CellEndEdit;
            dataViewSoftware.CellEndEdit += DataViewSoftware_CellEndEdit;

            //Start Label Visibility as false
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
        }

        //global variables
        //Connection to Database
        string connectionString = "Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5";

        //method to populate View Employee ComboBox
        private void popViewEmpCB()
        {
            //populate Employee Combobox from Employees table in database

            try
            {
                string query = "SELECT empID, (fName + ' ' + lName) AS Name FROM employees ORDER BY lName ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Employees"); //fill dataset from Employees

                cbViewEmp.DisplayMember = "Name"; //display text in combox
                cbViewEmp.ValueMember = "empID"; //value of member
                cbViewEmp.DataSource = ds.Tables["Employees"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }
        }

        //method to populate View Asset ComboBox
        private void popViewAssetCB()
        {
            //populate Asset Combobox from Software table in database
            try
            {
                string query = "SELECT aID, assetTag FROM assets ORDER BY assetTag ASC"; //create a string for query
                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Assets"); //fill dataset from Assets

                cbViewAsset.DisplayMember = "assetTag"; //display text in combox
                cbViewAsset.ValueMember = "assetTag"; //value of member
                cbViewAsset.DataSource = ds.Tables["Assets"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }
        }

        //method to populate View Software Instance ComboBox
        private void popViewSoftwareCB()
        {
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");
            //populate Software Combobox from Software table in database
            try
            {
                string query = "SELECT swID, swName FROM software ORDER BY swName ASC"; //create a string for query
                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Software"); //fill dataset from Software

                cbViewSoftware.DisplayMember = "swName"; //display text in combobox
                cbViewSoftware.ValueMember = "swID"; //value of member
                cbViewSoftware.DataSource = ds.Tables["Software"]; //datasource

                myConnection.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }
        }

        //tab select events
        private void tabView_Selecting(object sender, TabControlCancelEventArgs e)
        {
            popViewAssetCB();
            popViewEmpCB();
            popViewSoftwareCB();
        }


        //button action methods

        //navigate to menu button
        private void btnViewReturn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Menu menu = new Menu();
            menu.Show();
        }

        //navigate to add entry button
        private void btnViewToAdd_Click(object sender, EventArgs e)
        {
            this.Close();
            AddEntry add = new AddEntry();
            add.Show();
        }

        //view Employee assets and software button
        private void button1_Click(object sender, EventArgs e)
        {
            String query = "EXEC selAssetsAndSoftwareByEmployee " + cbViewEmp.SelectedValue;
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");


            SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for stored procedure
            myConnection.Open(); //open connection

            //Data Table
            DataTable dt = new DataTable(); //create new data table
            da.Fill(dt); //fill datatable from query

            //Fill and Format dataGridView
            dataViewEmployee.DataSource = dt; //datagridview source set equal to datatable
            dataViewEmployee.Columns[3].Width = 150; //set column widths
            dataViewEmployee.Columns[3].Width = 200;
            dataViewEmployee.Columns[0].ReadOnly = true;
            dataViewEmployee.Columns[4].ReadOnly = true;
            dataViewEmployee.Columns[6].Visible = false;

            myConnection.Close(); //close connection

            //labels and buttons now visible
            label4.Visible = true;
            label5.Visible = true;
            label5.Text = cbViewEmp.Text;
        }

        //view Asset employees and software button
        private void btnSearchAsset_Click(object sender, EventArgs e)
        {
            string query = "EXEC selEmployeeAndSoftwareByAsset " + cbViewAsset.SelectedValue;
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");


            SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for stored procedure
            myConnection.Open(); //open connection

            //Data Table
            DataTable dt = new DataTable(); //create new data table
            da.Fill(dt); //fill datatable from query

            //Fill and Format dataGridView
            dataViewAsset.DataSource = dt;
            dataViewAsset.Columns[0].ReadOnly = true;
            dataViewAsset.Columns[1].ReadOnly = true;
            dataViewAsset.Columns[3].Width = 200; //set column width
            dataViewAsset.Columns[4].Visible = false;
            dataViewAsset.Columns[5].Visible = false;
            dataViewAsset.Columns[2].ReadOnly = true;

            myConnection.Close(); //close connection

            //labels and buttons now visible
            label6.Visible = true;
            label7.Visible = true;
            label6.Text = cbViewAsset.Text;
        }

        //view Software assets and employees button
        private void btnSearchSoftware_Click(object sender, EventArgs e)
        {
            string query = "EXEC selEmployeeAndAssetBySoftware " + cbViewSoftware.SelectedValue;
            SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");


            SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for stored procedure
            myConnection.Open(); //open connection

            //DataTable
            DataTable dt = new DataTable(); //create new data table
            da.Fill(dt); //fill datatable from query

            //Fill and Format dataGridView
            dataViewSoftware.DataSource = dt;
            dataViewSoftware.Columns[2].ReadOnly = true;
            dataViewSoftware.Columns[6].Visible = false;

            myConnection.Close(); //close connection

            //labels and text now visible
            label8.Visible = true;
            label9.Visible = true;
            label8.Text = cbViewSoftware.Text;
        }

        //end button methods



        //cell editing properties methods 

        //Employees Table Cell End Edit Method
        private void DataViewEmployee_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if(e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
            {
                //declare variables
                int i = e.RowIndex;
                String cellAssetTag = dataViewEmployee.Rows[i].Cells[0].Value.ToString();
                String cellMake = dataViewEmployee.Rows[i].Cells[1].Value.ToString();
                String cellModel = dataViewEmployee.Rows[i].Cells[2].Value.ToString();
                String cellSN = dataViewEmployee.Rows[i].Cells[3].Value.ToString();

                String query = "UPDATE dbo.Assets SET make = @make, model = @model, serialNum = @serialNum FROM Employees WHERE assetTag = @assetTag;";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@make", SqlDbType.VarChar).Value = cellMake;
                    cmd.Parameters.Add("@model", SqlDbType.VarChar).Value = cellModel;
                    cmd.Parameters.Add("@serialNum", SqlDbType.VarChar).Value = cellSN;
                    cmd.Parameters.Add("@assetTag", SqlDbType.Int).Value = cellAssetTag;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                dataViewEmployee.Update(); //update data table
                
            }

            else if(e.ColumnIndex == 5)
            {
                //declare variables
                int i = e.RowIndex;
                String cellPK = dataViewEmployee.Rows[i].Cells[5].Value.ToString();
                String cellInstance = dataViewEmployee.Rows[i].Cells[6].Value.ToString();

                String query = "UPDATE dbo.swInstance SET productKey = @productKey FROM Software WHERE softwareID = @softwareID;";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@productKey", SqlDbType.VarChar).Value = cellPK;
                    cmd.Parameters.Add("@softwareID", SqlDbType.Int).Value = cellInstance;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                dataViewEmployee.Update(); //update data table

            }

        }

        //Assets Table Cell End Edit Method
        private void DataViewAsset_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                //Declare variables
                int i = e.RowIndex;
                String cellFName = dataViewAsset.Rows[i].Cells[0].Value.ToString();
                String cellLName = dataViewAsset.Rows[i].Cells[1].Value.ToString();
                String cellEmpID = dataViewAsset.Rows[i].Cells[4].Value.ToString();

                String query = "UPDATE dbo.Employees SET fName = @fName, lName = @lName WHERE empID = @empID;";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@fName", SqlDbType.VarChar).Value = cellFName;
                    cmd.Parameters.Add("@lName", SqlDbType.VarChar).Value = cellLName;
                    cmd.Parameters.Add("@empID", SqlDbType.VarChar).Value = cellEmpID;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                //update Asset Table
                dataViewAsset.Update();
            }

            else if (e.ColumnIndex == 3)
            {
                //declare variables
                int i = e.RowIndex;
                String cellPK = dataViewAsset.Rows[i].Cells[3].Value.ToString();
                String cellInstance = dataViewAsset.Rows[i].Cells[5].Value.ToString();

                //string for query
                String query = "UPDATE dbo.swInstance SET productKey = @productKey FROM Software WHERE softwareID = @softwareID";
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@productKEy", SqlDbType.VarChar).Value = cellPK;
                    cmd.Parameters.Add("@softwareID", SqlDbType.Int).Value = cellInstance;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }


                //update Asset Table
                dataViewAsset.Update();

            }

        }

        //Software Table Cell End Edit Method
        private void DataViewSoftware_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4 || e.ColumnIndex == 5)
            {
                //declare variables
                int i = e.RowIndex;
                String cellAssetTag = dataViewSoftware.Rows[i].Cells[2].Value.ToString();
                String cellMake = dataViewSoftware.Rows[i].Cells[3].Value.ToString();
                String cellModel = dataViewSoftware.Rows[i].Cells[4].Value.ToString();
                String cellSN = dataViewSoftware.Rows[i].Cells[5].Value.ToString();

                String query = "UPDATE dbo.Assets SET make = @make, model = @model, serialNum = @serialNum FROM Employees WHERE assetTag = @assetTag;";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@make", SqlDbType.VarChar).Value = cellMake;
                    cmd.Parameters.Add("@model", SqlDbType.VarChar).Value = cellModel;
                    cmd.Parameters.Add("@serialNum", SqlDbType.VarChar).Value = cellSN;
                    cmd.Parameters.Add("@assetTag", SqlDbType.Int).Value = cellAssetTag;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                dataViewSoftware.Update(); //update data table

            }

            else if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
            {
                //Declare variables
                int i = e.RowIndex;
                String cellFName = dataViewSoftware.Rows[i].Cells[0].Value.ToString();
                String cellLName = dataViewSoftware.Rows[i].Cells[1].Value.ToString();
                String cellEmpID = dataViewSoftware.Rows[i].Cells[6].Value.ToString();

                String query = "UPDATE dbo.Employees SET fName = @fName, lName = @lName WHERE empID = @empID;";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@fName", SqlDbType.VarChar).Value = cellFName;
                    cmd.Parameters.Add("@lName", SqlDbType.VarChar).Value = cellLName;
                    cmd.Parameters.Add("@empID", SqlDbType.VarChar).Value = cellEmpID;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                dataViewSoftware.Update(); //update data table

            }

        }
    }
}
