using System.ComponentModel;
using System.Diagnostics;
using System.Text;

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
        int lastLineCount = 0;
        public void LoadFromValues(string t, string n, int id)
        {
            title = t ?? string.Empty;
            note = n ?? string.Empty;
            EdId = id;
            lineNumBox.Partner = editNoteBox;
            editNoteBox.Partner = lineNumBox;
            editNoteBox.Text = note;
            try
            {
                titleEditBox.Text = title;
                LoadLineNumBox();
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
            lineNumBox.Clear();
        }
        private void EditPage_Load(object sender, EventArgs e)
        {
            lineNumBox.ScrollBars = RichTextBoxScrollBars.None;
        }
        void LoadLineNumBox()
        {
            lineNumBox.SuspendLayout();

            lineNumBox.Text ="";

            lineNumBox.ResumeLayout();
            int lineCount = editNoteBox.GetLineFromCharIndex(editNoteBox.TextLength) + 1;
            lastLineCount = lineCount;
            StringBuilder sb = new(lineCount * 3);
            for (int i = 1; i <= lineCount; i++)
            {
                sb.Append(i);
                sb.Append('\n');
            }
            lineNumBox.Text = sb.ToString();
        }
        private void editNoteBox_TextChanged(object sender, EventArgs e)
        {
            int lineCount = editNoteBox.Lines.Length;
            if (lineCount != lastLineCount)
            {
                lastLineCount = lineCount;
                LoadLineNumBox();
            }
        }
    }
}
