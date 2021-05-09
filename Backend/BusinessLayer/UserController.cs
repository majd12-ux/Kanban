using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Config;
using System.Text;
using System.Reflection;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    internal class UserController
    {
        private const int minPassLength = 4;
        private const int maxPassLength = 20;
        private  Dictionary<string, User> listOfUsers = new Dictionary<string, User>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

     
        internal void ValidateEmailAndPassword(string email, string password)
        {
            if (!ContainEmail(email.ToLower()))
            {
                log.Debug("email do not exist in the system(login), Enter a valid email");
                throw new Exception("email do not exist in the system, Enter a valid email");
            }
            if (listOfUsers[email.ToLower()].isLoggedIn())
            {
                log.Debug("User already logged in");
                throw new Exception("User already logged in");
            }
            if (!listOfUsers[email.ToLower()].passwordMatch(password))
            {
                log.Debug("Wrong password. Try again or click ‘Forgot password’ to reset it.");
                throw new Exception("Wrong password. Try again or click ‘Forgot password’ to reset it.");
            }
        }

        internal User register(string email, string password)
        {
            if (!(IsValidEmail(email)))
            {
                log.Debug("throwing Exception: email is not valid");
                throw new Exception("not valid Email");
            }
            if (listOfUsers.ContainsKey(email.ToLower()))//we check if the email is taken
            {
                log.Debug("throwing Exception: This email had taken try another email");
                throw new Exception("This email had taken try another email");
            }
            if (!this.isValidPass(password))
            {
                throw new Exception("valid password must be in length 4-20 chars and include small,upper cases and a number");
            }

            User user = new User(email.ToLower(),password);
            listOfUsers.Add(email.ToLower(), user);
            return listOfUsers[email.ToLower()];
        }
        internal User Login(string email, string pass)
        {
            ValidateEmailAndPassword(email,pass);
            listOfUsers[email.ToLower()].Login(email.ToLower(), pass);
            return listOfUsers[email.ToLower()];
        }
        internal User getUser(string email)
        {
            if (!ContainEmail(email.ToLower()))
            {
                log.Debug("User not registered");
                throw new Exception("User not registered");
            }
             
            return listOfUsers[email.ToLower()];
        }
        private bool ContainEmail(string email)
        {
            return listOfUsers.ContainsKey(email.ToLower());
        }

        internal void ValidateUserLoggin(string email)
        {
            if (!ContainEmail(email.ToLower()))
            {
                log.Debug("user is not registered(ValidateUserLoggin),Enter a valid email");
                throw new Exception("user is not registered(ValidateUserLoggin),Enter a valid email");
            }
            if (!listOfUsers[email.ToLower()].isLoggedIn())
                throw new Exception("User not logged in");
        }

        internal void Logout(string email)
        {
            if (!ContainEmail(email.ToLower()))
            {
                log.Debug("user is not registered(logout),Enter a valid email");
                throw new Exception("Enter a valid email");
            }
            listOfUsers[email.ToLower()].logout();
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
      
    }
}
