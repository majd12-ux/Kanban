using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class UserService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly UserController userController;

        public UserService()
        {
            userController = new UserController();
            log.Info("Starting log!");
        }
     
     internal UserController GetUserController()
        {
            return this.userController;
        }
        public Response LoadData()
        {
            try
            {
                userController.LoadUsers();
                return new Response();
            }
            catch (Exception ex)
            {
                return new Response(ex.Message);
            }
        }
        public Response register(string email, string password)
        {

            try
            {
                userController.register(email, password);
                log.Info("new user registed with the email " + email);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        //TODO:
        public Response<User> login(string email, string password)
        {
            try
            {
                userController.Login(email, password);
                log.Info(email + " logging");
                User user = new User(email);
                return Response<User>.FromValue(user);
            }
            catch (Exception e)
            {
                return Response<User>.FromError(e.Message);
            }
        }

        public Response Logout(string email)
        {
            try
            {
                userController.Logout(email);
                log.Info(email + " logging out ");
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        //Done!
        public Response<IList<Task>> InProgressTasks(string email)
        { 
            try
            {
                userController.ValidateUserLoggin(email);

                IList<Task> tasks = new List<Task>();

                IList<BusinessLayer.Task> listOFTasks = userController.getUser(email).importInProgressTasks();
                foreach (BusinessLayer.Task t in listOFTasks)
                {
                    Task STask = new Task(t.getTaskId(), t.GetCreationTime(), t.getTitle(), t.getDescription(), t.getDueDate(),t.EmailAssignee);
                    tasks.Add(STask);
                }
                return Response<IList<Task>>.FromValue(tasks);
            }
            catch (Exception e)
            {
                return Response<IList<Task>>.FromError(e.Message);
            }

        }
        public Response DeleteData()
        {
            try
            {
                userController.DeleteData();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
    }
}

