using System.Windows.Forms;
using 橘子记事本;
namespace UI测试
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void NoticeForm_Create_Test()
        {
            ApplicationConfiguration.Initialize();
            twdata tw=new twdata();
            NoticeSettingForm form =
                new NoticeSettingForm(ref tw,0);

            Assert.IsNotNull(form);

            form.Dispose();
        }
    }
}
