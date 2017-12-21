using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OdbcClient {
    public partial class Form1 : Form {
        string odbcConnectionName = "DB2A";
        public Form1() {
            InitializeComponent();
            odbcDataSources();
            setTextBoxValues();
        }

        private void odbcDataSources() {
            Microsoft.Win32.RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\ODBC\\ODBC.INI\\ODBC Data Sources");

            List<string> odbcNameList = new List<string>();
            odbcNameList.AddRange(registryKey.GetValueNames());
            odbcNameList.Sort();
            odbcComboBox.Items.AddRange(odbcNameList.ToArray());
            odbcComboBox.SelectedItem = odbcConnectionName;
        }
        private void button1_Click(object sender, EventArgs e) {
            //dataGridView1.Columns.Add("ColumnName1","ColumnHeader1");
            //dataGridView1.Columns.Add("ColumnName2", "ColumnHeader2");
            //dataGridView1.Columns.Add("ColumnName3", "ColumnHeader3");
            execSql();
            tabControl1.SelectedTab=tabPage2;
        }

        private void setTextBoxValues() {
            userIdTextBox.Text = "U3249";
            passwordTextBox.Text = "";
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

        }

        private void odbcComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            odbcConnectionName = (string)odbcComboBox.SelectedItem;
        }

        private void execSql() {
            //h ttp://stackoverflow.com/questions/13257401/c-sharp-prepared-statement-and-multiple-queries
            string myConnectionString = "DSN=" + odbcConnectionName + ";UID=" + userIdTextBox.Text + ";PWD=" + passwordTextBox.Text + ";";
            OdbcConnection myConnection = new OdbcConnection();
            myConnection.ConnectionString = myConnectionString;
            myConnection.Open();
            OdbcCommand odbcCommand = myConnection.CreateCommand();

            odbcCommand.CommandText = "select count(*) from testc.vpfact_fad";
            object obj = odbcCommand.ExecuteScalar();


            odbcCommand.Parameters.Clear();

            //odbcCommand.Parameters.Add("?ID7", OdbcType.VarChar).Value = parameter7;
            //queryStatement1() returns the query string insert into TABLE (field1, field2...) values(?,?,...)

            odbcCommand.CommandText = "select * from testc.vpfact_fad fetch first 3 rows only";
            OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();

            textBox1.AppendText("RowCount a: " + dataGridView1.RowCount + "\n");

            //dataGridView1.ColumnCount = odbcDataReader.FieldCount;
            for(int i = 0; i < odbcDataReader.FieldCount; ++i)
                dataGridView1.Columns.Add(odbcDataReader.GetName(i), odbcDataReader.GetName(i));
            textBox1.AppendText("RowCount b: " + dataGridView1.RowCount + "\n");

            for (int i = 0; i < odbcDataReader.FieldCount; ++i) textBox1.AppendText("[" + i + "] " + odbcDataReader.GetName(i) + "   ");


            //h ttps://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.columns%28v=vs.110%29.aspx

            while(odbcDataReader.Read()) {
                int row = dataGridView1.Rows.Add();

                for(int i = 0; i < odbcDataReader.FieldCount; ++i) {
                    dataGridView1.Rows[row].Cells[i].Value = odbcDataReader.GetString(i);

                }
            }
            textBox1.AppendText("RowCount c: " + dataGridView1.RowCount + "\n");

            myConnection.Close();
        }
    }
}
