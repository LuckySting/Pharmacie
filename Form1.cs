using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Pharmacie
{
    public partial class Form1 : Form
    {
        public IEnumerable<object[]> stored;
        public Form1()
        {
            InitializeComponent();
            fill();
        }

        private void fill()
        {
            var filename = "db.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var purchaseOrderFilepath = Path.Combine(currentDirectory, filename);
            var doc = XElement.Load(purchaseOrderFilepath);
            var elemets = from item in doc.Elements("Item")
                          select new object[] {
                            item.Element("Pharmacy").Value,
                            item.Element("Medicine").Value,
                            item.Element("Quantity").Value,
                            item.Element("Price").Value,
                            item.Element("Date").Value,
                            item.Element("Expires").Value
                          };
            stored = elemets;
            this.dataGridView1.Rows.Clear();
            foreach (var element in elemets)
            {
                this.dataGridView1.Rows.Add(element);
            }
        }

        private void commit()
        {
            var filename = "db.xml";
            var doc = new XElement("Items");
            foreach(DataGridViewRow row in this.dataGridView1.Rows)
            {
                try
                {
                    XElement item = new XElement("Item",
                    new XElement("Pharmacy", row.Cells[0].Value.ToString()),
                    new XElement("Medicine", row.Cells[1].Value.ToString()),
                    new XElement("Quantity", row.Cells[2].Value.ToString()),
                    new XElement("Price", row.Cells[3].Value.ToString()),
                    new XElement("Date", row.Cells[4].Value.ToString()),
                    new XElement("Expires", row.Cells[5].Value.ToString())
                    );
                    doc.Add(item);
                } catch (NullReferenceException exp)
                {
                    if (row.Index != this.dataGridView1.Rows.Count - 1)
                    {
                        MessageBox.Show("Row " + row.Index + " has wrong values");
                    }
                }
            }
            doc.Save(filename);
            fill();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.SearchField.Text = "";
            fill();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.SearchField.Text == "")
            {
                commit();
            } else
            {
                MessageBox.Show("Commit blocked while search input is not empty");
            }
        }

        private void SearchField_TextChanged(object sender, EventArgs e)
        {
            var txt = this.SearchField.Text.ToLower();
            if (txt != "")
            {
                var elemets = from item in this.stored
                              where item[0].ToString().ToLower().Contains(txt) || item[1].ToString().ToLower().Contains(txt) || item[4].ToString().ToLower().Contains(txt)
                              select item;
                this.dataGridView1.Rows.Clear();
                foreach (var element in elemets)
                {
                    this.dataGridView1.Rows.Add(element);
                }
            } else
            {
                this.dataGridView1.Rows.Clear();
                foreach (var element in this.stored)
                {
                    this.dataGridView1.Rows.Add(element);
                }
            }
        }
    }
}
