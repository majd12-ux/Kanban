using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using log4net;
using System.Reflection;
using System.Globalization;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class User
    {
        private volatile bool login;
        public string email;
        private long id;//we have to check that.
        private UserD userD;//we add this to do save and delete in database.
        private string password { get; set; }
        List<string> oldPasswords = new List<string>();
        private const int minPassLength = 4;
        private const int maxPassLength = 20;
        public Dictionary<string, Board> boards = new Dictionary<string, Board>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal User(string email,string password)
        {
                if (IsValidEmail(email) && isValidPass(password))
                {
                    this.email = email;
                    this.password = password;
                    oldPasswords.Add(password);
                    login = false;
                    this.userD = new UserD(email, password);
                  userD.save();//SAVE IN DATABASE.
                 this.id = userD.Id;

                }
        }
        internal User(UserD D)
        {
            this.id = D.Id;
            this.email = D.Email;
            this.password = D._password;
            this.userD = D;
        }
        internal bool isLoggedIn()
        {
            return login;
        }

        internal bool passwordMatch(string password)
        {
            return (isValidPass(password) && this.password == password);            
        }
        internal void Login(string email,string password)
        {
            if (this.email.Equals(email) && passwordMatch(password))
                login = true;
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))//true if the value parameter of email is null or Empty, or if value consists exclusively of white-space characters.
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        private bool isValidPass(String password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                log.Debug("throwing Exception: password is not valid");
                return false;
            }
            if (password.Length > maxPassLength | password.Length < minPassLength)
            {
                log.Debug("throwing Exception:password must be in length of 4 to 20");
                return false;
            }
            else
            {
                Boolean smauppercase = false;
                Boolean smallcase = false;
                Boolean number = false;
                for (int i = 0; i < password.Length; i++)
                {
                    if (password[i] >= 'A' & password[i] <= 'Z') smauppercase = true;// at least one uppercase letter
                    if (password[i] >= 'a' & password[i] <= 'z') smallcase = true;// at least one small letter
                    if (password[i] >= '0' & password[i] <= '9') number = true;// a number
                }
                if (!(smallcase & number & smauppercase))
                {
                    log.Debug("throwing Exception: must include at least one uppercase char, one small char and a number");
                    return false;
                }
            }
            return true;
        }

        internal void logout()
        {
            if (!login)
            {
                log.Debug("The user already logged out");
                throw new Exception("The user already logged out");
            }
            login = false;
        }

        internal Board AddTheExistBoard(string name, Board board)
        {
            if (!login)
            {
                log.Debug("User not loged in, Please log in");
                throw new Exception("User not loged in, Please log in");
            }
            boards[name] = board;
            return board;
        }

        internal void removeBoard(string name)
        {
            if (!login)
            {
                log.Debug("User not loged in, Please log in");
                throw new Exception("User not loged in, Please log in");
            }
            if (!boards.ContainsKey(name))
            {
                log.Debug("Board not found/ wrong board name given");
                throw new Exception("Board not found/ wrong board name given");
            }
            boards.Remove(name);
            
        }

        internal IList<Task> importInProgressTasks()
        {
            if (!login)
            {
                log.Debug("User not loged in, Please log in");
                throw new Exception("User not loged in, Please log in");
            }
            List<Task> inProgressTasks = new List<Task>();
            foreach (var board in boards)
            {
                log.Debug(board.Value.GetColumn(1).getTasksList());
                inProgressTasks.AddRange(board.Value.GetColumn(1).getTasksList());
            }
            return inProgressTasks;
            
        }


    }
}
