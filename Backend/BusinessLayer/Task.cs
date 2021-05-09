using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using System.Reflection;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class Task
    {
        /// <summary>
        /// state = 0 for backlog, state = 1 for inprogress, state = 2 for done
        /// a user cant change the task if it in state = 2 (done)
        /// </summary>
        private int id;

        private int taskId;
        private readonly DateTime creationTime;
        private string title;
        private string description;
        private DateTime dueDate;
        
        private readonly int maxTilteLength = 50;
        private readonly int maxDescriptionLength = 300;

        private string boardId;//assignee (who is the user in this column)

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //*************************//constructors//***********************
        internal Task(DateTime dueDate, string title, string description, string boardId, int taskId)
        {
            isValid(dueDate, title, description, boardId, taskId);
            
            this.creationTime = DateTime.Now;
            this.dueDate = dueDate;
            this.title = title;
            this.description = description;
            id = 0;
            this.taskId = taskId;
            this.boardId = boardId;
        }

        private void isValid(DateTime dueDate, string title, string description, string boardId, int taskId)
        {
            if (title is null || title.Length == 0)
            {
                log.Debug("title is empty or null");
                throw new Exception("title is empty or null");
            }
            if (title.Length > maxTilteLength)
            {
                log.Debug("title is too long");
                throw new Exception("title is too long");
            }
            var Symbols = new Regex(@"\/:*?<>|");
            if (!Symbols.IsMatch(title))
            {
                log.Debug("title cannot contain special case character.");
                throw new Exception("title cannot contain special case character.");
            }
            if (description is null)
            {
                log.Debug("description is null");
                throw new Exception("description is null");
            }
            if (description.Length > maxDescriptionLength)
            {
                log.Debug("the description is too long");
                throw new Exception("the description is too long");
            }
            if (dueDate < creationTime)
            {
                log.Debug("duedate alredy passed");
                throw new Exception("duedate alredy passed");
            }
         
        }

        public void setState(int newId)
        {
            id = newId;
        }
        
        public void setTitle(string newTitle) 
        {
            int isDone = 2;

            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new Exception("IsNullOrWhiteSpace(email)");
            }
            if (id == isDone)
            {
                log.Debug("you cant change task after it done");
                throw new Exception("you cant change task after it done"); 
            }
            //throw exception if it is too long
            if (newTitle.Length > maxTilteLength)
            {
                log.Debug("too long title");
                throw new Exception("too long title");
            }
            if (newTitle is null || newTitle.Length == 0)
            {
                log.Debug("must enter a title");
                throw new Exception("must enter a title");
            }
            title = newTitle;
        }
        public void changeDueDate(DateTime newDuedate)
        {
            int isDone = 2;
            if (id == isDone)
            {
                log.Debug("you cant change task after it done");
                throw new Exception("you cant change task after it done");
            }
            if ( newDuedate < DateTime.Now)
            {
                log.Debug("duedate alredy passed ");
                throw new Exception("duedate alredy passed ");
            }
            this.dueDate = newDuedate;

        }
        public void changeDescription(string newDescription)
        {
            int isDone = 2;
            if (id == isDone)
            {
                log.Debug("you cant change task after it done");
                throw new Exception("you cant change task after it done");
            }
            //throw exception if it is too long
            if (newDescription.Length > maxDescriptionLength)
            {
                log.Debug("too long title");
                throw new Exception("too long title");
            }
            description = newDescription;
        }
        public string getDescription()
        {
            if (description.Length == 0)
            {
                log.Debug("description is empty");
                throw new Exception("description is empty");
                //return "description is now empty";
            }
           
            return description;
        }
        public int getTaskId()
        {
            return taskId;
        }
        public DateTime GetCreationTime()
        {
            return creationTime; 
        }
        public string getTitle()
        {
            return title;
        }
        public DateTime getDueDate()
        {
            return dueDate;
        }
       


    }
}