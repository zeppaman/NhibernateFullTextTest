using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Search;
using NHibernate.Tool.hbm2ddl;

namespace NhibernateFullTextTest
{
    class Program
    {
        public static Configuration GetConfiguration()
        {

            Configuration cfg = new Configuration();

            ModelMapper mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());

            HbmMapping domainMapping =
              mapper.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddMapping(domainMapping);
            cfg.Configure();

            return cfg;
        }

        static void Main(string[] args)
        {

            Configuration configuration = GetConfiguration();
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            //Update database structure
            SchemaMetadataUpdater.QuoteTableAndColumns(configuration);
            var update = new SchemaUpdate(configuration);
            update.Execute(true, true);

            //Creating data
            using (ISession session = sessionFactory.OpenSession())
            {
                ITransaction t=session.BeginTransaction();

                LogEntity entityToSave = null;
                for (int i = 0; i < 10000; i++)
                {
                    entityToSave = GenerateRandomEntity();
                    session.SaveOrUpdate(entityToSave);
                }
               

                t.Commit();
            }

            using (var s = sessionFactory.OpenSession())
            using (var search = Search.CreateFullTextSession(s))
            using (var tx = s.BeginTransaction())
            {
                var list = search.CreateFullTextQuery<LogEntity>("Message:Hatter")
                    .SetMaxResults(5)
                    .List<LogEntity>();

               

                tx.Commit();
            }



        }

        static Random r = new Random();
        static string[] lines= File.ReadAllLines(".\\demo.txt");
        private static LogEntity GenerateRandomEntity()
        {
            
            string[] levels = new string[] { "INFO", "ERROR", "DEBUG" };
            LogEntity le = new LogEntity();
            le.CreateDate = DateTime.Now.AddMilliseconds(r.NextDouble() * 10000000);
            le.Message = lines[r.Next(0, lines.Length)];
            le.SourceDate = le.CreateDate.AddMilliseconds(r.NextDouble() * 10000000);
            le.UpdateDate=le.SourceDate.AddMilliseconds(r.NextDouble() * 10000000);
            le.Uid = Guid.NewGuid();
            le.Level = levels[r.Next(0,levels.Length)];
            le.ApplictionId = Guid.NewGuid();
            

            return le;
        }
    }
}
