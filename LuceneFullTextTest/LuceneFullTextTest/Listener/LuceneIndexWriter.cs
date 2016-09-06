
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using LuceneFullTextTest.Lucene;
using NHibernate.Cfg;
using NHibernate.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//LuceneFullTextTest.Listener.IndexWriter,LuceneFullTextTest
namespace LuceneFullTextTest.Listener
{

        public class LuceneIndexWriter : IPostDeleteEventListener, IPostInsertEventListener,
                                                  IPostUpdateEventListener
        {


        public virtual void OnPostDelete(PostDeleteEvent e)
        {
        
        }

            public virtual void OnPostInsert(PostInsertEvent e)
            {

                    AddToIndex(e.Entity);
            
            }

        private void AddToIndex(object entity)
        {
            if (entity == null) return;
            
            Dictionary<string, object> document = GetDocument(entity);

            LuceneIndexContext.Current.DefaultIndexManager.AddDocument(document);
        }

        private Dictionary<string, object> GetDocument(object entity)
        {
            Dictionary<string, object> doc = new Dictionary<string, object>();
            PropertyInfo[] properties = entity.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                doc.Add(property.Name, property.GetValue(entity));
            }
            return doc;
        }

        public virtual void OnPostUpdate(PostUpdateEvent e)
        {
                
        }

  

            

            
        }
    
}
