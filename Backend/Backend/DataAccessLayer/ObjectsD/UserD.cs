using IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class UserD : DalObject
    {
    //  internal const string UserTableName = "User";
        internal const string EmailColumnName = "Email";
        internal const string PasswordColumnName = "Password";
        private string email;
        internal string Email { get => email; set { email = value; if (Id != -1) _controller.Update(Id, EmailColumnName, value); } }
        private string password;
        internal string _password { get => password; set { password = value; if (Id != -1) _controller.Update(Id, password, value);} }
        internal UserD( string Email, string Password) : base(-1 ,new UserDcontroller())
        {
            this.Email = Email;
            this._password = Password;
        }
        internal UserD(long id, string Email,  string Password) : base(-1 ,new UserDcontroller())
        {
            this.Email = Email;
            this._password = Password;
            this.Id = id;
        }
        internal override void save()
        {
            UserDcontroller controller = new UserDcontroller();
            Id = controller.Insert(this);
        }
    }
}
