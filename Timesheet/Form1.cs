using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timesheet.BusinessLogical;

namespace Timesheet
{
    

    public partial class Form1 : Form
    {
        
        private BindingSource bindingSource1 = new BindingSource();
        private TimesheetManager manager = new TimesheetManager();
        private bool newRow = false;
        private int rowId;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Binding rateBinding = new Binding("Text", manager, "DefaultRate", true, DataSourceUpdateMode.OnValidation);
            rateBinding.FormattingEnabled = true;
            textBox1.DataBindings.Add(rateBinding);
            textBox1.DataBindings[0].FormattingEnabled = true;
            textBox1.DataBindings[0].FormatString = "c";

            // Initialize the DataGridView.
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSize = true;
            dataGridView1.DataSource = bindingSource1;

            dataGridView1.Columns.Add(CreateComboBoxState());
            
            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
            column1.DataPropertyName = "Title";
            column1.Name = "Title";
            dataGridView1.Columns.Add(column1);

            dataGridView1.Columns.Add(CreateComboBoxType());

            DataGridViewColumn column3 = new DataGridViewTextBoxColumn();
            column3.DataPropertyName = "Duration";
            column3.Name = "Duration";
            column3.DefaultCellStyle.Format = "h\\:mm";
            dataGridView1.Columns.Add(column3);
            

            DataGridViewColumn column4 = new DataGridViewTextBoxColumn();
            column4.DataPropertyName = "Rate";
            column4.Name = "Rate";
            column4.DefaultCellStyle.Format = "c";
            dataGridView1.Columns.Add(column4);

            DataGridViewColumn column5 = new DataGridViewTextBoxColumn();
            column5.Name = "Total";
            column5.DefaultCellStyle.Format = "c";
            dataGridView1.Columns.Add(column5);

            DataGridViewImageColumn saveButton = new DataGridViewImageColumn();
            saveButton.Image = Image.FromFile(Environment.CurrentDirectory + "/images/save.png");
            saveButton.Width = 20;
            saveButton.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns.Add(saveButton);

            DataGridViewImageColumn cancelButton = new DataGridViewImageColumn();
            cancelButton.Image = Image.FromFile(Environment.CurrentDirectory + "/images/cancel.png");
            cancelButton.Width = 20;
            cancelButton.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridView1.Columns.Add(cancelButton);

            DataGridViewColumn column8 = new DataGridViewTextBoxColumn();
            column8.DataPropertyName = "Id";
            column8.Name = "Id";
            column8.Visible = false;
            dataGridView1.Columns.Add(column8);
        }

        DataGridViewComboBoxColumn CreateComboBoxState()
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.DataSource = Enum.GetValues(typeof(Status));
            combo.DataPropertyName = "State";
            combo.Name = "State";
            return combo;
        }

        DataGridViewComboBoxColumn CreateComboBoxType()
        {
            DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn();
            combo.DataSource = Enum.GetValues(typeof(WorkType));
            combo.DataPropertyName = "Type";
            combo.Name = "Type";
            return combo;
        }

        private bool CheckForReadOnly(int rowIndex, int colIndex)
        {
            if ((Status)Enum.Parse(typeof(Status), dataGridView1.Rows[rowIndex].Cells[colIndex].Value.ToString()) == Status.Submitted)
                return true;
            else
                return false;
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            string value;
            NumberStyles style;
            CultureInfo culture;
            decimal currency;

            value = textBox1.Text;
            style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            culture = CultureInfo.CreateSpecificCulture("en-AU");
            if (!Decimal.TryParse(value, style, culture, out currency))
            {
                MessageBox.Show("Please enter a valid currency amount.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            } 
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            newRow = true;

            Item item = manager.AddNewTimesheet();
            if (bindingSource1.Count == 0)
                bindingSource1.Add(item);
            else
                bindingSource1.Insert(0, item);
        }

        private void cmdSubmit_Click(object sender, EventArgs e)
        {
            int id = int.Parse(dataGridView1.Rows[rowId].Cells[8].Value.ToString());
            if (manager.Submit(id))
            {
                dataGridView1.Rows[rowId].Cells[0].Value = Status.Submitted;
            }
        }

        #region datagridview
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 || e.ColumnIndex == 4)
            {
                string duration = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                decimal rate = Convert.ToDecimal(dataGridView1.CurrentRow.Cells[4].Value);

                dataGridView1.CurrentRow.Cells["Total"].Value = manager.CalculateTotal(duration, rate);

            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int id;
            Status state;
            string title;
            WorkType type;
            TimeSpan duration;
            decimal rate;
            int newId;

            if (dataGridView1.CurrentRow != null)
                rowId = dataGridView1.CurrentRow.Index;

            if (e.ColumnIndex == 6 && newRow) //save icon button is clicked
            {
                //save to list
                dataGridView1.Rows[e.RowIndex].Cells[0].Value = Status.Active;
                title = dataGridView1.Rows[e.RowIndex].Cells[1].Value == null ? String.Empty : dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                type = (WorkType)Enum.Parse(typeof(WorkType), dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                duration = (TimeSpan)dataGridView1.Rows[e.RowIndex].Cells[3].Value;
                rate = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());
                newId = manager.Add(title, rate, duration, type);
                dataGridView1.Rows[e.RowIndex].Cells[8].Value = newId;

                newRow = false;

                dataGridView1.Rows[e.RowIndex].Cells[6].Value = Image.FromFile(Environment.CurrentDirectory + "/images/edit.png");
                dataGridView1.Rows[e.RowIndex].Cells[7].Value = Image.FromFile(Environment.CurrentDirectory + "/images/delete.png");
            }
            else if (e.ColumnIndex == 7 && newRow) //save icon button is clicked
            {
                //remove from datagrideview
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
            else if (e.ColumnIndex == 6 && e.RowIndex >= 0 && newRow != true)
            {
                //enable edit
                id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString());
                state = (Status)Enum.Parse(typeof(Status), dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                title = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                type = (WorkType)Enum.Parse(typeof(WorkType), dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
                duration = (TimeSpan)dataGridView1.Rows[e.RowIndex].Cells[3].Value;
                rate = decimal.Parse(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString());

                manager.Edit(id, state, title, rate, duration, type);
            }
            else if (e.ColumnIndex == 7 && e.RowIndex >= 0 && newRow != true)
            {
                DialogResult result = MessageBox.Show("Do you want to delete this record?", "Confirmation", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    //delete from the list
                    id = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString());
                    if (manager.Delete(id))
                        dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 1; i < dataGridView1.Rows.Count; i++)
            {
                if (CheckForReadOnly(i, 0))
                {
                    dataGridView1.Rows[i].ReadOnly = true;
                }
                else
                    dataGridView1.Rows[i].ReadOnly = false;

            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (CheckForReadOnly(e.RowIndex, 0))
                    dataGridView1.Rows[e.RowIndex].ReadOnly = true;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                if (e.Value != null && e.Value != DBNull.Value)
                    e.Value = ((TimeSpan)e.Value).Hours.ToString("00") + ":" +
                               ((TimeSpan)e.Value).Minutes.ToString("00");
            }
        }

        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {

        }

        #endregion
    }
}
