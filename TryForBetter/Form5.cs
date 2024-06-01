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

namespace TryForBetter
{
    public partial class RegisterForm : Form
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\kaigi\OneDrive\Documents\LoginData.mdf;Integrated Security=True;Connect Timeout=30") ;
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void btregister_Click(object sender, EventArgs e)
        {
            if (txtemail.Text == "" || txtpass.Text == "" || txtusername.Text == "")
            {
                MessageBox.Show("you have to fill all gaps", "INFO", MessageBoxButtons.OK);
            }

            else 
            { 
            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    String checkusername = "SELECT * FROM admin WHERE " +
                        "username = '" + txtusername.Text.Trim() + "'";
                    using (SqlCommand checkuser = new SqlCommand(checkusername, connect))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(checkuser);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            MessageBox.Show(txtusername.Text + " da ton tai roi", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            string insertdata = "INSERT INTO admin (email,username,passsword)" +
                                "VALUES(@email,@username,@pass)";
                            using (SqlCommand cmd = new SqlCommand(insertdata, connect))
                            {
                                cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim());
                                cmd.Parameters.AddWithValue("@username", txtusername.Text.Trim());
                                cmd.Parameters.AddWithValue("@pass", txtpass.Text.Trim());

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Sign up SUCCESSFUL!!!!", "Congratulations!!!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                SignUpForm fm = new SignUpForm();
                                fm.Show();
                                this.Hide();

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connect.Close();
                }
            }
        }
    }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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

        private void label6_Click(object sender, EventArgs e)
        {
            SignUpForm sign = new SignUpForm();
            sign.Show();
            this.Hide();
        }

        private void btclear_Click(object sender, EventArgs e)
        {
            txtemail.Clear();
            txtpass.Clear();
            txtusername.Clear();
        }
    }
}
