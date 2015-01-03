using ActiveSharp.Database_Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveSharp.Connection
{
    class Program
    {
        static void Main(string[] args)
        {

            Country argentina = new Country();
            argentina.Name = "Argentina";
            argentina.save();

            Person juancito = new Person();
            juancito.Age = 20;
            juancito.Name = "Juancito";
            juancito.country = argentina;
            juancito.save();

            Person searchedPerson = EntityManager.Instance.findById<Person>(juancito.id);
            Console.WriteLine(searchedPerson.Name);

        }
    }

    class Country : ActiveRecord
    {
        public String Name { get; set; }
        public override String table { get { return "countries"; } }
    }

    class Person : ActiveRecord
    {
        
        public int Age { get; set; }
        public String Name { get; set; }
        public Country country { get; set; }
        public override String table { get { return "people"; } }
           

        public override void preInsert()
        {
            base.preInsert();
            Console.WriteLine("Before save!");
        }

    }

}
