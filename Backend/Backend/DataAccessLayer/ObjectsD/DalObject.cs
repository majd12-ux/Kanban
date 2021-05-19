using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace IntroSE.Kanban.Backend.DataAccessLayer.ObjectsD
{
    abstract class DalObject
    {
        internal const string IDColumnName = "Id";
        internal DalController _controller;

        internal long Id { get; set; } = -1;
        internal DalObject(long id, DalController controller)
        {
            this.Id = id;
            _controller = controller;
        }
        internal abstract void save();
        internal void delete()
        {
            _controller.Delete(this);
        }
    }
}
