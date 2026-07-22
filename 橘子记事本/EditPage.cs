using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace 橘子记事本
{
    public partial class EditPage : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string title { get;set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string note { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] 
        public int EdId { get;set; }
        public EditPage()
        {
            InitializeComponent();
            EdId = -1;
        }
        public void LoadFromValues(string t, string n, int id)
        {
            title = t ?? string.Empty;
            note = n ?? string.Empty;
            EdId = id;
            try
            {
                titleEditBox.Text = title;
                editNoteBox.Text = note;
            }
            catch { }
        }
        public void syncData()
        {
            title = titleEditBox.Text;
            note = editNoteBox.Text;
        }
        public void reset() { 
            title=null;
            note= null;
            titleEditBox.Text = "";
            editNoteBox.Text = "";
        }
    }
}
