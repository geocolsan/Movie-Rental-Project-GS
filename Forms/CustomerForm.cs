﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace MovieRentalProject
{

    public partial class CustomerForm : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MovieRental"].ConnectionString;

        public CustomerForm()
        {
            InitializeComponent();
            SetupDataGridView();
            LoadCustomerData();
            customerDataViewGrid.SelectionChanged += Customer_SelectionChanged;
        }
        private void SetupDataGridView()
        {
            // Configure the DataGridView
            customerDataViewGrid.ColumnCount = 12;
            customerDataViewGrid.Columns[0].Name = "CustomerID";
            customerDataViewGrid.Columns[1].Name = "First Name";
            customerDataViewGrid.Columns[2].Name = "Last Name";
            customerDataViewGrid.Columns[3].Name = "Address";
            customerDataViewGrid.Columns[4].Name = "City";
            customerDataViewGrid.Columns[5].Name = "Province";
            customerDataViewGrid.Columns[6].Name = "PostalCode";
            customerDataViewGrid.Columns[7].Name = "Email";
            customerDataViewGrid.Columns[8].Name = "Account Number";
            customerDataViewGrid.Columns[9].Name = "Account Creation";
            customerDataViewGrid.Columns[10].Name = "Credit Card";
            customerDataViewGrid.Columns[11].Name = "Rating";

            // Optionally, make columns read-only
            foreach (DataGridViewColumn column in customerDataViewGrid.Columns)
            {
                column.ReadOnly = true;
            }

            // Set styles for the header band
            customerDataViewGrid.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Navy;
            customerDataViewGrid.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            customerDataViewGrid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold);
            customerDataViewGrid.EnableHeadersVisualStyles = false;

            // Set other properties
            customerDataViewGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        private void backButton_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm();
            menuForm.Show();
            this.Hide();
        }
        private void AddCustomerButton_Click(object sender, EventArgs e)
        {
            NewCustomer newCustomer = new NewCustomer();
            MessageBox.Show("NewCustomer clicked.");
            newCustomer.Show();
            this.Hide();
        }

        private void customerSearchButton_Click(object sender, EventArgs e)
        {
            string searchTitle = customerSearchBox.Text.Trim();
            LoadCustomerData(searchTitle);
        }
        private void LoadCustomerData(string searchTitle = "")
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Corrected query with proper spacing and formatting
                    string query = "SELECT CustomerID, FirstName, LastName, Addr, City, Province, PostalCode, EmailAddress, AccountNumber, AccountDateCreation, CreditCardNumber, Rating " +
                                   "FROM Customer " +
                                   "WHERE FirstName LIKE @SearchTitle + '%'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTitle", searchTitle);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Clear existing rows before adding new data
                            customerDataViewGrid.Rows.Clear();

                            while (reader.Read())
                            {


                                customerDataViewGrid.Rows.Add(
                                    reader["CustomerID"].ToString(),
                                    reader["FirstName"].ToString(),
                                    reader["LastName"].ToString(),
                                    reader["Addr"].ToString(),
                                    reader["City"].ToString(),
                                    reader["Province"].ToString(),
                                    reader["PostalCode"].ToString(),
                                    reader["EmailAddress"].ToString(),
                                    reader["AccountNumber"].ToString(),
                                    reader["AccountDateCreation"].ToString(),
                                    reader["CreditCardNumber"].ToString(),
                                    reader["Rating"].ToString()
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void Customer_SelectionChanged(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (customerDataViewGrid.SelectedRows.Count > 0)
            {
                // Get the selected CustomerID
                Global.GlobalCustID = customerDataViewGrid.SelectedRows[0].Cells["CustomerID"].Value.ToString();

                // Call method to query Queue_Up and Movie tables
            }
        }

        private void customerEditButton_Click(object sender, EventArgs e)
        {
            if (customerDataViewGrid.SelectedRows.Count > 0)
            {
                // Get the CustomerID from the selected row (assuming it's the first column)
                string selectedCustomerID = customerDataViewGrid.SelectedRows[0].Cells[0].Value.ToString();

                // Open the EditCustomer form and pass the selected CustomerID
                EditCustomer edt = new EditCustomer();
                edt.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.", "Edit Customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
