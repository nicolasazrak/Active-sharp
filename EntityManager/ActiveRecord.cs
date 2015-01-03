using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ActiveSharp.Database_Helper;


namespace ActiveSharp.Database_Helper
{

    public abstract class ActiveRecord
    {

        [System.ComponentModel.Browsable(false)] 
        public long id { get; set; }
        [System.ComponentModel.Browsable(false)] 
        public abstract String table { get; }


        public Boolean validates()
        {
            return false;
        }

        public virtual void preSave()
        {

        }

        public virtual void afterSave()
        {

        }

        public virtual void afterLoad()
        {

        }

        public virtual void preInsert()
        {

        }

        public void save()
        {
            EntityManager.Instance.save(this);
        }

        public void insert()
        {
            id = EntityManager.Instance.insert(this);
        }

        public void delete()
        {
            EntityManager.Instance.delete(this);
        }

        public void logicalDelete()
        {
            EntityManager.Instance.logicalDelete(this);
        }

        public void fill(Object values)
        {

        }




    }
}
