using IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;
namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class ColumnD : DalObject
    {
        internal const string BoardIDColumnName = "BoardID";
        internal const string ColumnOrdinalColumnName = "Ordinal";
        internal const string NameColumnName = "Name";
        internal const string LimitColumnName = "Lim";
        private long boardID;
        internal long BoardID { get => boardID; set { boardID = value; if (Id!=-1) _controller.Update(Id, BoardIDColumnName, value);  } }
        private long columnOrdinal;
        internal long ColumnOrdinal { get=>columnOrdinal; set { columnOrdinal = value; if (Id != -1) _controller.Update(Id, ColumnOrdinalColumnName, value);  } }
        private string name;
        internal string Name { get=>name; set { name = value; if (Id != -1) _controller.Update(Id, NameColumnName, value);} }
        private long limit;
        internal long Limit { get=>limit; set { limit = value; if (Id != -1) _controller.Update(Id, LimitColumnName, value);} }
        internal ColumnD(long ColumnID,long Boardid, long columnOrdinal, string name, long limit) :base(-1,new ColumnDcontroller())
        {
            this.BoardID = Boardid;
            this.ColumnOrdinal = columnOrdinal;
            this.Name = name;
            this.Limit = limit;
            this.Id = ColumnID;
        }
        internal ColumnD(long Boardid, long columnOrdinal, string name, long limit) : base(-1, new ColumnDcontroller())
        {
            this.BoardID = Boardid;
            this.ColumnOrdinal = columnOrdinal;
            this.Name = name;
            this.Limit = limit;
        }
        internal override void save()
        {
            ColumnDcontroller controller = new ColumnDcontroller();
            this.Id = controller.Insert(this);
        }
        internal List<TaskD> LoadAllTasks()
        {
            TaskDcontroller controller = new TaskDcontroller();
            return controller.LoadTasks(Id);
        }
        
    }
}
