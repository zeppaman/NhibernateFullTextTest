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
using NHibernate.Tool.hbm2ddl;
using LuceneFullTextTest;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using LuceneFullTextTest.Lucene;

namespace LuceneFullTextTest
{
    
    class Program
    {
        private static string indexPath = @".\\LuceneIndex";
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

        static void ReadWriteTest()
        {
            LuceneIndexManager im = new LuceneIndexManager("Test");

            Dictionary<string, object> item;
            for (int i = 0; i < 100; i++)
            {
                item = new Dictionary<string, object>();
                item["Name"] = "name_" + DateTime.Now.ToString("yyyy-MM-ddHHmm") + i;
                item["Order"] = i.ToString();
                item["OrderInt"] = i;
                im.AddDocument(item);
            }



            List<Document> result = im.Query("OrderInt:{9 TO 13}", "Order", SortField.INT, true, 0, 20);

            PrintAll(result);

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
                session.CacheMode = CacheMode.Ignore;
                ITransaction t = session.BeginTransaction();

                LogEntity entityToSave = null;
                for (int i = 0; i < 100; i++)
                {
                    entityToSave = GenerateRandomEntity();
                    session.Save(entityToSave);
                   
                }


                t.Commit();
            }

          List<Document> results=  LuceneIndexContext.Current.DefaultIndexManager.Query("Message:Hatter", "Level", SortField.STRING, true, 0,100);


            PrintAll(results);



        }

        private static void PrintAll(List<Document> result)
        {
            foreach (var Document in result)
            {
                Console.WriteLine(Document.ToString()+" - "+ Document.Get("Name"));
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
