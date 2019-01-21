using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace LoginWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=STUGOTS\SQLEXPRESS;Initial Catalog=LoginDB;Integrated Security=True");
        int ContactId = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                if (btnSave.Content == "Save")
                {
                    SqlCommand sqlCmd = new SqlCommand("ContactAddOrEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@mode", "Add");
                    sqlCmd.Parameters.AddWithValue("@ContactId", 0);
                    sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@MobileNumber", txtMobileNumber.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    sqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Saved successfully");
                }
                else
                {
                    SqlCommand sqlCmd = new SqlCommand("ContactAddOrEdit", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@mode", "Edit");
                    sqlCmd.Parameters.AddWithValue("@ContactId", ContactId);
                    sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@MobileNumber", txtMobileNumber.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                    sqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Updated successfully");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        
        void FillDataGridView()
        {
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
            SqlDataAdapter sqlDa = new SqlDataAdapter("ContactViewOrSearch", sqlCon);
            sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
            sqlDa.SelectCommand.Parameters.AddWithValue("@ContactName", txtSearch.Text.Trim());
            DataTable dtbl = new DataTable();
            sqlDa.Fill(dtbl);
            dgvContacts.ItemsSource = dtbl.AsDataView();
            sqlCon.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DgvContacts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DataRowView row = dgvContacts.SelectedItem as DataRowView;
            ContactId = Convert.ToInt32(row.Row[0]);
            txtName.Text = row.Row[1].ToString();
            txtMobileNumber.Text = row.Row[2].ToString();
            txtAddress.Text = row.Row[3].ToString();
            btnSave.Content = "Update";
            btnDelete.IsEnabled = true;
        }

        private void Reset()
        {
            txtName.Text = "";
            txtMobileNumber.Text = "";
            txtAddress.Text = "";
            btnSave.Content = "Save";
            ContactId = 0;
            btnDelete.IsEnabled = false;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            FillDataGridView();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sqlCon.State == ConnectionState.Closed)
                    sqlCon.Open();
                if (btnSave.Content == "Save")
                {
                    SqlCommand sqlCmd = new SqlCommand("ContactDeletion", sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@ContactId", ContactId);
                    sqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Deleted successfully");
                    Reset();
                    FillDataGridView();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
