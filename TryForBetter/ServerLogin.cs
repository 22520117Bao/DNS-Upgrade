using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TryForBetter
{
    public partial class ServerLogin : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kaigi\OneDrive\Documents\LoginData.mdf;Integrated Security=True;Connect Timeout=30");
        public ServerLogin()
        {
            InitializeComponent();
        }

        private void btregister_Click(object sender, EventArgs e)
        {

            if (txtusername.Text == "" || txtpass.Text == "")
            {
                MessageBox.Show("Please fil all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        String selectData = "SELECT * FROM admin WHERE username = @username AND passsword = @pass";
                        using (SqlCommand cmd = new SqlCommand(selectData, connect))
                        {
                            if (txtusername.Text != "admin")
                            {
                                MessageBox.Show("Wrong to connect Server", "Warning", MessageBoxButtons.OK);

                            }
                            else 
                            { 
                            cmd.Parameters.AddWithValue("@username", txtusername.Text);
                            cmd.Parameters.AddWithValue("@pass", txtpass.Text);
                            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count >= 1)
                            {
                                MessageBox.Show("Logged In successfully", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                ServerForm mForm = new ServerForm();
                                mForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Username/Password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Connecting: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void ckbpass_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbpass.Checked)
            {
                txtpass.PasswordChar = '\0';
            }
            else
            {
                txtpass.PasswordChar = '*';
            }
        }
    }
}
