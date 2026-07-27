namespace 橘子记事本
{
    public class twdata
    {
        public string version { get; private set; } = "TANGERINE_TWRITER_CONFIG_V1.0_\\//\\/";
        public List<string> titles { get; set; }
        public List<string> notes { get; set; }
        public List<string> tasks { get; set; }
        public List<Boolean> isNoticeEnabled { get; set; }

        public List<int> tasksNoticeMethod { get; set; }//提醒方式
        public List<int> taskNoticeType { get; set; }//计时类型
        public List<int> taskNoticeCfg1 { get; set; }
        public List<int> taskNoticeCfg2 { get; set; }
        public List<DateTime> tasksNoticeTime { get; set; }
        public List<TimeSpan> taskNoticeTime2 { get; set; }
        public bool isSoundBeforeNotice { get; set; } = true;
    }
    public class twdata1_0//为数据更新做好准备(1.0->1.1)
    {
        public string version { get; private set; } = "TANGERINE_TWRITER_CONFIG_V1.0_\\//\\/";
        public List<string> titles { get; set; }
        public List<string> notes { get; set; }
        public List<string> tasks { get; set; }
        public List<Boolean> isNoticeEnabled { get; set; }

        public List<int> tasksNoticeMethod { get; set; }//提醒方式
        public List<int> taskNoticeType { get; set; }//计时类型
        public List<int> taskNoticeCfg1 { get; set; }
        public List<int> taskNoticeCfg2 { get; set; }
        public List<DateTime> tasksNoticeTime { get; set; }
        public List<TimeSpan> taskNoticeTime2 { get; set; }
        public bool isSoundBeforeNotice { get; set; } = true;
    }
}
