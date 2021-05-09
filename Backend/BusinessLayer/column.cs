using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Reflection;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class column
    {
        private readonly int id;
        private Dictionary<int ,Task> tasksDictionary;
        private IList<Task> listOfTasks;
        private int numOfTasks;
        private int maxNumOfTasks;
        private string boardId;//unique id for board

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public column(int id, string boardId)
        {
            this.id = id;
            tasksDictionary = new Dictionary<int, Task>();
            listOfTasks = new List<Task>();
            numOfTasks = 0;
            maxNumOfTasks = -1; //-1 if there is not limitation
            this.boardId = boardId;
        }
        public int getid()
        {
            return id;
        }
        public IList<Task> getTasksList()
        {
            return listOfTasks;
        }
        public int getMaxNumOfTasks()
        {
            return maxNumOfTasks;
        }
        public string getBoardId()
        {
            return boardId;
        }
        public string getColumnName()
        {
            if (getid() == 0)
                return "backlog";
            else if (getid() == 1)
                return "in progress";
            else return "done";
        }
        internal void ChangeMaxNumTasks(int MaxNumTasks)
        {
            
            if (MaxNumTasks < 0 & MaxNumTasks!= -1) 
            {
                log.Debug("max number of tasks should be positive or -1");
                throw new Exception("max number of tasks should be positive or -1");
            }
            if(numOfTasks > MaxNumTasks)
                throw new Exception("cannot set a max number of tasks for tasks that is exceeded the limit");
            maxNumOfTasks = MaxNumTasks;
        }
        internal void removeTask(int taskId)
        {
            if (!tasksDictionary.ContainsKey(taskId))
            {
                log.Debug("Task not exist in this column");
                throw new Exception("Task not exist in this column");
            }
            listOfTasks.Remove(tasksDictionary[taskId]);
            tasksDictionary.Remove(taskId);
            numOfTasks = numOfTasks - 1;
        }
//adding a new task
        internal Task addNewTask(DateTime dueDate, string title, string description, int taskid)
        {
            //throw exception if unlegal task or this is not backlog column
            if (getid() == 1 | getid() == 2)
            {
                log.Debug("can add new task only to 0 column");
                throw new Exception("can add new task only to 0 column");
            }
            if (!isNotFull())
            {
                log.Debug("no place for new Task in this column");
                throw new Exception("no place for new Task in this column");
            }
            Task newTask = new Task(dueDate, title, description, getBoardId(), taskid);
            tasksDictionary.Add(newTask.getTaskId(), newTask);
            listOfTasks.Add(newTask);
            numOfTasks++;
            return newTask;

        }
        //adding an existing task that was advanced form previous column 
        internal void addTask(Task task)
        {
            
            if (!isNotFull())
            {
                log.Debug("column is full, cant add an adittion task");
                throw new Exception("column is full, cant add an adittion task");
            }
            tasksDictionary.Add(task.getTaskId(), task);
            listOfTasks.Add(task);
            task.setState(this.id);
            numOfTasks++;
            
            
        }
        internal Task getTask(int taskId)
        {
            if (!tasksDictionary.ContainsKey(taskId))
            {
                log.Debug("cant get this task, there isnt task with that task id");
                throw new Exception("cant get this task, there isnt task with that task id");
            }
            return tasksDictionary[taskId];
        }
        internal bool isNotFull()
        {
            bool isnotFull = false;
            if (maxNumOfTasks == -1)
            {
                isnotFull = true;
                return isnotFull;
            }
            isnotFull = maxNumOfTasks > numOfTasks;
            return isnotFull;
        }
        


    }
}