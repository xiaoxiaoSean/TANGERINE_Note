namespace 橘子记事本
{
    public class twdatav1
    {
        public string version = "TANGERINE_TWRITER_CONFIG_V1.0_\\//\\/";
        public string[] titles { get; set; }
        public string[] notes { get; set; }
        public string[] tasks { get; set; }
        public List<Boolean> isNoticeEnabled { get; set; }

        public List<int> tasksNoticeMethod { get; set; }//提醒方式
        public List<int> taskNoticeType { get; set; }//计时类型
        public List<int> taskNoticeCfg1 { get; set; }
        public List<int> taskNoticeCfg2 { get; set; }
        public List<DateTime> tasksNoticeTime { get; set; }
        public List<TimeSpan> taskNoticeTime2 { get; set; }
    }
}
