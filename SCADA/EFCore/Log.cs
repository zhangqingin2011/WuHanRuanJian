using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace SCADA
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log : Entity
    {
        /// <summary>
        /// 级别
        /// </summary>
        [Display(Name = "级别")]
        [Required]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// 发生时间
        /// </summary>
        [Display(Name = "发生时间")]
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// 日志信息产生的来源
        /// </summary>
        [Display(Name = "来源")]
        public string Provider { get; set; }

        /// <summary>
        /// 事件类型的ID
        /// </summary>
        [Display(Name = "事件类型的ID")]
        public int EventID { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        [Display(Name = "操作")]
        public string Keywords { get; set; }

        /// <summary>
        /// 日志具体信息
        /// </summary>
        [Display(Name = "日志具体信息")]
        public string EventData { get; set; }
    }
    //
    // 摘要:
    //     Defines logging severity levels.
    public enum LogLevel
    {
        //
        // 摘要:
        //     Logs that contain the most detailed messages. These messages may contain sensitive
        //     application data. These messages are disabled by default and should never be
        //     enabled in a production environment.
        Trace = 0,
        //
        // 摘要:
        //     Logs that are used for interactive investigation during development. These logs
        //     should primarily contain information useful for debugging and have no long-term
        //     value.
        Debug = 1,
        //
        // 摘要:
        //     Logs that track the general flow of the application. These logs should have long-term
        //     value.
        Information = 2,
        //
        // 摘要:
        //     Logs that highlight an abnormal or unexpected event in the application flow,
        //     but do not otherwise cause the application execution to stop.
        Warning = 3,
        //
        // 摘要:
        //     Logs that highlight when the current flow of execution is stopped due to a failure.
        //     These should indicate a failure in the current activity, not an application-wide
        //     failure.
        Error = 4,
        //
        // 摘要:
        //     Logs that describe an unrecoverable application or system crash, or a catastrophic
        //     failure that requires immediate attention.
        Critical = 5,
        //
        // 摘要:
        //     Not used for writing log messages. Specifies that a logging category should not
        //     write any messages.
        None = 6
    }
}

