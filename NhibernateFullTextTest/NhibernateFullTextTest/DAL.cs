using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Search.Attributes;

namespace NhibernateFullTextTest
{
    [Indexed]
    public class LogEntity 
    {


        public virtual Guid Id { get; set; }

        [DocumentId]
        public virtual int IndexId { get; set; }

        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual DateTime SourceDate { get; set; }
        [Field(Index.Tokenized, Store = Store.Yes)]
        public virtual string Message { get; set; }
        public virtual string Level { get; set; }
        //[BsonId]
        public virtual Guid Uid
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = value;
            }
        }
        public virtual Guid ApplictionId { get; set; }

    }

    public class DAL : ClassMapping<LogEntity>
    {

        

        public DAL()
        {
            Table("WL_LogEntity");
            Schema("dbo");
            
            Id(x => x.IndexId, map => {  map.Generator(Generators.Identity); });
            Property(x => x.Uid, map => { map.Unique(true); map.Index("idx_uid"); });
            Property(x => x.Message, map => { map.Length(2000); });
            Property(x => x.Level);
            Property(x => x.SourceDate);
            Property(x => x.ApplictionId);
            Property(x => x.UpdateDate);
        }
    }
}
