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
    public partial class editSoftware : Form
    {
        public editSoftware()
        {
            InitializeComponent();

            //populate Software Name ComboBox
            popSoftwareName();

        }

        //global SQL Connection
        string connectionString = "Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5";

        //button to delete product key
        private void btnDeletePk_Click(object sender, EventArgs e)
        {
            //create strings and buttons for message box
            string message = "Are you sure you want to delete this Product Key from the database?\n\nAssets associated with this product key will not be deleted.";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            string caption = "Delete this product key?";

            //new dialogbox called result
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);

            //if user selects yes
            if (result == System.Windows.Forms.DialogResult.Yes)
            {

                String query1 = "DELETE FROM dbo.History WHERE softwareID = @softwareID";
                using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
                using (SqlCommand cmd = new SqlCommand(query1, con)) //use new sql command that uses the string query
                {
                    //query parameters
                    cmd.Parameters.Add("@softwareID", SqlDbType.Int).Value = cbDeletePK.SelectedValue; ;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }

                String query2 = "DELETE FROM dbo.swInstance WHERE softwareID = @softwareID"; 
                using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
                using (SqlCommand cmd = new SqlCommand(query2, con)) //use new sql command that uses the string query
                {
                    //query parameters
                    cmd.Parameters.Add("@softwareID", SqlDbType.Int).Value = cbDeletePK.SelectedValue; ;

                    con.Open(); //open connection
                    cmd.ExecuteNonQuery(); //execute query
                    con.Close(); //close connection
                }
                //populate software name to update combobox after edits
                popSoftwareName();
            }
        }

        //populate combobox that displays product keys and software name
        private void popSoftwareName()
        {
            try
            {
                String query = "SELECT I.softwareID, (S.swName + ' - ' + I.productKey) as swPK from swInstance I LEFT JOIN Software S ON I.swID = S.swID ORDER BY swName ASC"; //create a string for query

                SqlConnection myConnection = new SqlConnection("Password=[DATABASE PASSWORD];Persist Security Info=True;User ID=sa;Initial Catalog=Assets;Data Source=[DATABASE IP];Connection timeout = 5");

                SqlDataAdapter da = new SqlDataAdapter(query, myConnection); //sql command for query

                myConnection.Open(); //open connection

                DataSet ds = new DataSet(); //create new data set ds
                da.Fill(ds, "Software"); //fill dataset from Software

                cbDeletePK.DisplayMember = "swPK"; //display text in combobox
                cbDeletePK.ValueMember = "softwareID"; //value of member
                cbDeletePK.DataSource = ds.Tables["Software"]; //datasource

                myConnection.Close(); //close connection
            }
            catch (Exception f)
            {
                Console.WriteLine(f.ToString());
            }

        }

        //button to navigate back to the menu
        private void btnMenu_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Menu menu = new Menu();
            menu.Show();
        }

        private void btnNoOwner_Click(object sender, EventArgs e)
        {
            String query = "UPDATE dbo.swInstance SET aID = 2 WHERE softwareID = @softwareID"; //create a string for query
            using (SqlConnection con = new SqlConnection(connectionString)) //use global connection string
            using (SqlCommand cmd = new SqlCommand(query, con)) //use new sql command that uses the string query
            {
                //query parameters
                cmd.Parameters.Add("@softwareID", SqlDbType.Int).Value = cbDeletePK.SelectedValue;

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
                cmd2.Parameters.Add("@aID", SqlDbType.Int).Value = 2;
                cmd2.Parameters.Add("@softwareID", SqlDbType.Int).Value = cbDeletePK.SelectedValue;
                cmd2.Parameters.Add("@date", SqlDbType.Date).Value = System.DateTime.Now;

                con.Open(); //open connection
                cmd2.ExecuteNonQuery(); //execute query
                con.Close(); //close connection
            }

            //show confirmation message box
            MessageBox.Show("Product Key is now unassigned.");
        }
    }
}
