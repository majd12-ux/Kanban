using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers;
using IntroSE.Kanban.Backend.DataAccessLayer;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class BoardsController
    {
       
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private  Dictionary<string, Board> boards;
        private  Dictionary<string, string> idAndName;                 
        private  Dictionary<string, string> idAndCreatorEmail;
        private BoardDcontroller BoardDcontroller = new BoardDcontroller();
        private TaskDcontroller DataOfTask;
        private ColumnDcontroller DataOfColumn;



        internal BoardsController()
        {
            boards = new Dictionary<string, Board>();
            idAndName = new Dictionary<string, string>();
            idAndCreatorEmail = new Dictionary<string, string>();
            DataOfTask = new TaskDcontroller();
            DataOfColumn = new ColumnDcontroller();

        }
        /// <summary>
        /// Loading all the boards in the dataBase to the system(Ram/Code)!
        /// </summary>
        public void LoadAllBoards()
        {
            List<BoardD> boardds = BoardDcontroller.SelectAllBoards();
            foreach (BoardD b in boardds)
            {
                Board bordddd = new Board(b);
                boards.Add(b.Email, bordddd);
            }
        }
        /// <summary>
        /// Deleting all the boards/columns/tasks stored in the data base!
        /// </summary>
        public void DeleteData()
        {

            bool rs1 = DataOfTask.DeleteAll();
            if (!rs1)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
            bool rs2 = DataOfColumn.DeleteAll();
            if (!rs2)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
            bool rs3 = BoardDcontroller.DeleteAll();
            if (!rs3)
            {
                throw new Exception("An Error Accured During Deletion Of Data!");
            }
        }


        internal void LimitColumn(string email, string name, int columnOrdinal, int limit)
        {
            
            GetBoard(email, name).GetColumn(columnOrdinal).ChangeMaxNumTasks(limit);
        }

        internal string GetGetColumnName(string email, string name, int columnOrdinal)
        {
            
            return GetBoard(email, name).GetColumn(columnOrdinal).getColumnName();
        }

        internal Task AddTask(string email, string name, string title, string description, DateTime dueDate,string emailAssignee)
        {
            int taskId = GetBoard(email, name).GetfinishId() + 1;
            Task t = GetBoard(email, name).GetColumn(0).addNewTask(dueDate, title, description,taskId, emailAssignee);
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

        //createBoard from the user.
        internal void createBoard(string name,String email)
        {
            email = email.ToLower();
            if (this.boards.ContainsKey(email)) throw new Exception("this user already have board");
            log.Info("new Board opened for the User: " + email);
           boards.Add(email, new Board(name,email));
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
            Board bb = new Board(name, userEmail);//we have to check..
            if (boards[idAndCreatorEmail[id]].Equals(new Board(name, userEmail)))
            {
                boards.Remove(id);
                idAndCreatorEmail.Remove(id);
                idAndName.Remove(id);
               
            }
            else {
                log.Debug("you are not the creator");
                throw new Exception("you are not the creator");
            }


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

        //check the host 
        internal void validateHost(String emailHost)
        {
            if (String.IsNullOrWhiteSpace(emailHost))
            {
                log.Debug("throwing Exception: email is null");
                throw new Exception("emailHost is null");
            }
            emailHost = emailHost.ToLower();
            if (!boards.ContainsKey(emailHost)) throw new Exception(emailHost + " does not have a board");
            if (!emailHost.Equals(boards[emailHost].CreatorEmail)) throw new Exception(emailHost + " is not a host");
        }
        //join to exist board ...
        internal void joinExistBoard(String email, String emailHost)
        {
            email = email.ToLower();
            emailHost = emailHost.ToLower();
            if (this.boards.ContainsKey(email)) throw new Exception("this user already have board");
            if (!this.boards.ContainsKey(emailHost)) throw new Exception(emailHost + " does not have a board");

            boards.Add(email, boards[emailHost]);
            this.BoardDcontroller.InsertToHosts(boards[emailHost].Id, email);
        }

        


    }
}

