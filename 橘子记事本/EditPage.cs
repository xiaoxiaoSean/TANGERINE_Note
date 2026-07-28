using System.ComponentModel;

namespace 橘子记事本
{
    public partial class EditPage : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string title { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string note { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int EdId { get; set; }
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
            lineNumBox.Partner = editNoteBox;
            editNoteBox.Partner = lineNumBox;
            try
            {
                titleEditBox.Text = title;
                editNoteBox.Text = note;
                loadlineNumBox();
            }
            catch { }
        }
        public void syncData()
        {
            title = titleEditBox.Text;
            note = editNoteBox.Text;
        }
        public void reset()
        {
            title = null;
            note = null;
            titleEditBox.Text = "";
            editNoteBox.Text = "";
        }
        private void EditPage_Load(object sender, EventArgs e)
        {
            lineNumBox.ScrollBars = RichTextBoxScrollBars.None;
        }
        void loadlineNumBox()
        {
            if (note != null)
            {
                int count = note.Count(n => n == '\n');
                int i = 0;
                for (i = 1; i < count; i++)
                {
                    lineNumBox.AppendText(i.ToString() + "\n");
                }
                if (note[^1]!='\n')
                {
                    i++;
                    lineNumBox.AppendText(i.ToString() + "\n");
                }//补最后一行
                i++;
                lineNumBox.AppendText(i.ToString() + "\n");//补第一行
            }

        }
    }
}
