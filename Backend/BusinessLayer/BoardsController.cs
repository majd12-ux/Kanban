using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class BoardsController
    {
       
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private  Dictionary<string, Board> boards;
        private  Dictionary<string, string> idAndName;                 
        private  Dictionary<string, string> idAndCreatorEmail;          


        internal BoardsController()
        {
            boards = new Dictionary<string, Board>();
            idAndName = new Dictionary<string, string>();
            idAndCreatorEmail = new Dictionary<string, string>();

        }


        internal void LimitColumn(string email, string name, int columnOrdinal, int limit)
        {
            
            GetBoard(email, name).GetColumn(columnOrdinal).ChangeMaxNumTasks(limit);
        }

        internal string GetGetColumnName(string email, string name, int columnOrdinal)
        {
            
            return GetBoard(email, name).GetColumn(columnOrdinal).getColumnName();
        }

        internal Task AddTask(string email, string name, string title, string description, DateTime dueDate)
        {
            int taskId = GetBoard(email, name).GetfinishId() + 1;
            Task t = GetBoard(email, name).GetColumn(0).addNewTask(dueDate, title, description,taskId );
            GetBoard(email, name).advancefinishId();
            return t;
        }


        internal void editTaskDueDate(string email, string name, int columnOrdinal, int taskId, DateTime dueDate)
        {
            GetBoard(email, name).GetColumn(columnOrdinal).getTask(taskId).changeDueDate(dueDate);
        }

        internal void editTaskTitle(string email, string name, int columnOrdinal, int taskId, string title)
        {
            GetBoard(email, name).GetColumn(columnOrdinal).getTask(taskId).setTitle(title);
        }


        internal void editTaskDescription(string email, string name, int columnOrdinal, int taskId, string description)
        {
            GetBoard(email, name).GetColumn(columnOrdinal).getTask(taskId).changeDescription(description);
        }


        internal void AdvanceTask(string email, string name, int columnOrdinal, int taskId)
        {
            GetBoard(email, name).AdvanceTask(columnOrdinal, taskId);
        }



        internal Board AddBoard(string name, string userEmail)
        {
            string id = name + "@" + userEmail;
            if (boardNameIsTaken(id))
            {
                log.Debug("this user has board with such name");
                throw new Exception("this user has board with such name");
            }
            Board b = new Board(name, userEmail);
            boards.Add(id, b);
            idAndCreatorEmail.Add(id, userEmail);
            idAndName.Add(id, name);
            return b;
            
        }



        internal void RemoveBoard(string name, string userEmail)
        {
            if (name == null)
            {
                log.Debug("board name cannot be null");
                throw new Exception("board name cannot be null");
            }
            string id = name + "@" + userEmail;
            if (!boardNameIsTaken(id))
            {
                log.Debug("no such board for that user");
                throw new Exception("no such board for that user");
            }
            boards.Remove(id);
            idAndCreatorEmail.Remove(id);
            idAndName.Remove(id);

        }

        internal IList<Task> GetTasksList(string email, string name, int columnOrdinal)
        {
            return GetBoard(email, name).GetColumn(columnOrdinal).getTasksList();
        }


        internal Board GetBoard(string email, string name)
        {
            if (name == null)
            {
                log.Debug("board name cannot be null");
                throw new Exception("board name cannot be null");
            }
            string id = name + "@" + email;
            if (!boardNameIsTaken(id))
            {
                log.Debug("no such board for that user");
                throw new Exception("no such board for that user");
            }
            return boards[id];
        }

        internal bool boardNameIsTaken(string id)
        {
            return idAndName.ContainsKey(id);
        }



    }
}

