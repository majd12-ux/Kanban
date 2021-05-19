using IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class BoardD : DalObject
    {
        internal const string EmailColumnName = "Email";
        private string email;
        internal string Email { get=>email; set { email = value; if (Id != -1) _controller.Update(Id, EmailColumnName, value); } }
        internal BoardD(long id, string Email) : base(-1,new BoardDcontroller())
        {
            this.Email = Email;
            this.Id = id;
        }
        internal BoardD( string Email) : base(-1, new BoardDcontroller())
        {
            this.Email = Email;
        }
        internal override void save()
        {
            BoardDcontroller controller = new BoardDcontroller();
            this.Id=controller.Insert(this);
        }
        internal List<ColumnD> LoadAllColumns(long id)
        {
            ColumnDcontroller controller = new ColumnDcontroller();
            return controller.LoadColumns(id);
        }
        

    }
}
