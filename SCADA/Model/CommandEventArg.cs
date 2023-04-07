using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Model
{
    /// <summary>
    /// 任务：
    /// 1.上料：
    ///     （1）前端盖上料
    ///     （2）后端盖上料
    ///     （3）轴类上料
    /// 2.下料
    ///     （1）前端盖下料
    ///         （a）毛坯下料
    ///         （b）合格下料
    ///         （c）不合格下料
    ///     （2）后端盖下料
    ///         （a）毛坯下料
    ///         （b）合格下料
    ///         （c）不合格下料
    ///     （3）轴类下料
    ///         （a）毛坯下料
    ///         （b）合格下料
    ///         （c）不合格下料
    /// 3.出库
    ///     1.前端盖出库
    ///     2.后端盖出库
    ///     3.轴类出库
    /// 4.入库
    ///     1.前端盖入库
    ///         （a）合格入库
    ///         （b）不合格入库
    ///     2.后端盖入库
    ///         （a）合格入库
    ///         （b）不合格入库
    ///     3.轴类入库
    ///         （a）合格入库
    ///         （b）不合格入库
    /// </summary>
    /// 
    public class CommandEventArg:EventArgs
    {
        public Agv_Task task { get; set; }

        public CommandEventArg(Agv_Task newTask)
        {
            task = newTask;
        }
    }

    public class PublishNewTask
    {
        public event EventHandler<CommandEventArg> NewTask;

        public void NewTaskHasComed(Agv_Task task)
        {
            if (!string.IsNullOrEmpty(task.P1) && !string.IsNullOrEmpty(task.P2) && task.Status == 0)
            {
                if (NewTask != null)
                {
                    NewTask.Invoke(this, new CommandEventArg(task));
                }
            }
        }
    }

    public class TaskReceiver
    {
        public string ConnnectStr { get; set; }

        public TaskReceiver(string connectStr)
        {
            ConnnectStr = connectStr;
        }
        public async void ExcuteTask(object sender,CommandEventArg e)
        {
            await e.task.InsesrtToDB(ConnnectStr);
        }
    }

}
