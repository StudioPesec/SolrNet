using System.Collections.Generic;

namespace SolrNet.SchemaDSL.Impl {
    public interface IDeleteBy<TSchema> where TSchema : ISchema {
        void ById(string id);
        void ByIds(IEnumerable<string> ids);
        void ByQuery(ISolrQuery q);
        void ByQuery(string q);
    }
}