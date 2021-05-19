using IntroSE.Kanban.Backend.DataAccessLayer.objectsControllers;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class TaskD: DalObject
    {
        internal const string CreationDateColumnName = "CreationTime";
        internal const string DueDateColumnName = "DueDate";
        internal const string ColumnIDColumnName = "ColumnID";
        internal const string TitleColumnName = "Title";
        internal const string DescriptionColumnName = "Description";
        internal const string AssigneeColumnName = "Assignee";

        private long columnId;
        internal long ColumnId { get => columnId; set { columnId = value; if (Id != -1) _controller.Update(Id, ColumnIDColumnName, value); } }
        private string creationDate;
        internal string CreationDate { get => creationDate; set { creationDate = value; if (Id != -1) _controller.Update(Id, CreationDateColumnName, value); } }
        private string dueDate;
        internal string DueDate { get=>dueDate; set { dueDate = value; if (Id != -1) _controller.Update(Id, DueDateColumnName, value);  } }
        private string title;
        internal string Title { get=>title; set { title = value; if (Id != -1) _controller.Update(Id, TitleColumnName, value);} }
        private string description;
        internal string Description { get=>description; set { description = value; if (Id != -1) _controller.Update(Id, DescriptionColumnName, value);} }
        private string assignee;
        internal string Assignee { get => assignee; set { assignee = value; if (Id != -1) _controller.Update(Id, AssigneeColumnName, value); } }
        internal TaskD(long TaskID, long ColumnID, string creationTime, string dueDate, string title, string description,string assignee) : base(-1,new TaskDcontroller())
        {
            this.ColumnId = ColumnID;
            this.CreationDate = creationTime;
            this.DueDate = dueDate;
            this.Title = title;
            this.Description = description;
            this.Assignee = assignee;
            this.Id = TaskID;
        }
        internal TaskD(long ColumnID, string creationTime, string dueDate, string title, string description, string assignee) : base(-1, new TaskDcontroller())
        {
            this.ColumnId = ColumnID;
            this.CreationDate = creationTime;
            this.DueDate = dueDate;
            this.Title = title;
            this.Description = description;
            this.Assignee = assignee;
        }

        internal override void save()
        {
            TaskDcontroller controller = new TaskDcontroller();
            Id = controller.Insert(this);
        }
    }
}
