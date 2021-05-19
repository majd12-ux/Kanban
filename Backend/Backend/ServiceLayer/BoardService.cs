using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class BoardService
    {
        BoardsController BC;
        UserController UC;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BoardService(UserController userController)
        {
            BC = new BoardsController();
            this.UC = userController;
        }

        public BoardsController getBoardsController()
        {
            return BC;
        }
        public Response DeleteData()
        {
            try
            {
                BC.DeleteData();
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response LoadData()
        {
            try
            {
                BC.LoadAllBoards();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response LimitColumn(string email, string name, int columnOrdinal, int limit)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BC.LimitColumn(email, name, columnOrdinal, limit);
                log.Info(email + " limited number of tasks to " + limit.ToString() + " in board: " + name + " in column: " + columnOrdinal.ToString());
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response<int> GetColumnLimit(string email, string name, int columnOrdinal)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                int var = BC.GetBoard(email, name).GetColumn(columnOrdinal).getMaxNumOfTasks();
                return Response<int>.FromValue(var);

            }
            catch (Exception e)
            {
                return Response<int>.FromError(e.Message);
            }

        }

        public Response<string> GetColumnName(string email, string name, int columnOrdinal)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                string var = BC.GetGetColumnName(email, name, columnOrdinal);
                return Response<string>.FromValue(var);
            }
            catch (Exception e)
            {
                return Response<string>.FromError(e.Message);
            }
        }

        
        public Response<Task> AddTask(string email, string name, string title, string description, DateTime dueDate,string assigne)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BusinessLayer.Task task = BC.AddTask(email, name, title, description, dueDate, assigne);
                log.Info("new task added to "+  email+" boards, with title: '"+title+"'+ in board "+ name);
                Task t = new Task(task.getTaskId(), task.GetCreationTime(), title, description, dueDate, assigne);
                return Response<Task>.FromValue(t);
            }
            catch (Exception e)
            {
                return Response<Task>.FromError(e.Message);
            }
        }
        

        public Response UpdateTaskDueDate(string email, string name, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BC.editTaskDueDate(email, name, columnOrdinal, taskId, dueDate);
                log.Info("task " + taskId + "duedate changed");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response UpdateTaskTitle(string email, string name, int columnOrdinal, int taskId, string title)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BC.editTaskTitle(email, name, columnOrdinal, taskId, title);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        public Response UpdateTaskDescription(string email, string name, int columnOrdinal, int taskId, string description)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BC.editTaskDescription(email, name, columnOrdinal, taskId, description);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response AdvanceTask(string email, string name, int columnOrdinal, int taskId)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BC.AdvanceTask(email, name, columnOrdinal, taskId);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        public Response<IList<Task>> GetColumn(string email, string name, int columnOrdinal)
        {
            try
            {
                UC.ValidateUserLoggin(email);

                IList<Task> tasks = new List<Task>();
                IList<BusinessLayer.Task> BTasksList = BC.GetTasksList(email, name, columnOrdinal);
                foreach (BusinessLayer.Task t in BTasksList)
                {
                    Task j = new Task(t.getTaskId(), t.GetCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate(),t.EmailAssignee);
                    tasks.Add(j);
                }
                
                return Response<IList<Task>>.FromValue(tasks);
            }
            catch (Exception e)
            {
                return Response<IList<Task>>.FromError(e.Message);
            }
        }

        public Response AddBoard(string email, string name)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                Board b = BC.AddBoard(name, email);
                UC.getUser(email).AddTheExistBoard(email, b);//accept and add user email and Board  to the user boards Dictionary
                log.Info("board " + name + "to " + email + " boards");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public Response RemoveBoard(string email, string name)
        {
            try
            {
                UC.ValidateUserLoggin(email);
                BC.RemoveBoard(name, email);
                UC.getUser(email).removeBoard(name);
                log.Info(name + " was removed from " + email + "boards");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
    }
        
        
}
