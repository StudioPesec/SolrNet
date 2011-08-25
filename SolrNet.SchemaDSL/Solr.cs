#region SchemaDSL license
// Copyright (c) 2007-2010 Mauricio Scheffer
// Copyright 2011 Matej Skubic - Studio Pešec d.o.o. - SchemaDSL
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion SchemaDSL license

using System.Collections.Generic;
using System.Linq;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.SchemaDSL.Impl;

namespace SolrNet.SchemaDSL
{
    /// <summary>
    /// Solr DSL Entry point
    /// </summary>
    /// <remarks>SchemaDSL by Matej Skubic @ StudioPesec.com - http://lab.studiopesec.com</remarks>
    public class Solr<T> where T : ISchema
    {
        private readonly ISolrDocumentSerializer<T> solrDocumentSerializer;
        private readonly ISolrBasicOperations<T> solrBasicOperations;
        private readonly ISolrQueryExecuter<T> solrQueryExecuter;
        private readonly SolrNet.Impl.ISolrFieldSerializer solrFieldSerializer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="solrDocumentSerializer"></param>
        /// <param name="solrBasicOperations"></param>
        /// <param name="solrQueryExecuter"></param>
        public Solr(ISolrDocumentSerializer<T> solrDocumentSerializer, ISolrBasicOperations<T> solrBasicOperations, ISolrQueryExecuter<T> solrQueryExecuter
            , SolrNet.Impl.ISolrFieldSerializer solrFieldSerializer)
        {
            //Guard.ArgumentNotNull(solrDocumentSerializer, "solrDocumentSerializer");
            //Guard.ArgumentNotNull(solrBasicOperations, "solrBasicOperations");
            //Guard.ArgumentNotNull(solrQueryExecuter, "solrQueryExecuter");

            this.solrDocumentSerializer = solrDocumentSerializer;
            this.solrBasicOperations = solrBasicOperations;
            this.solrQueryExecuter = solrQueryExecuter;
            this.solrFieldSerializer = solrFieldSerializer;
        }

        ///<summary>
        /// Deletes document
        ///</summary>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public IDeleteBy<T> Delete()
        {
            return new DeleteBy<T>(solrBasicOperations);
        }

        /// <summary>
        /// Adds/updates a document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        public void Add(T document)
        {
            Add(new[] { document });
        }

        /// <summary>
        /// Adds/updates a document with an optional Boost Value to the entire document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        /// <param name="boostValue">The boost value to apply to the document.</param>
        public  void Add(T document, double? boostValue)
        {
            Add(new[] { document }, boostValue);
        }

        /// <summary>
        /// Adds/updates a list of documents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents"></param>
        public void Add(IEnumerable<T> documents) 
        {
            var docs = documents.Select(d => new KeyValuePair<T, double?>(d, null));
            //var cmd = new AddCommand<T>(docs, ServiceLocator.Current.GetInstance<ISolrDocumentSerializer<T>>(), null);
            var cmd = new AddCommand<T>(docs,solrDocumentSerializer, null);
            //ISolrBasicOperations<T> solrExe = ServiceLocator.Current.GetInstance<ISolrBasicOperations<T>>();
            solrBasicOperations.Send(cmd);
        }

        /// <summary>
        /// Adds/updates a list of documents with an optional Boost Value to all documents specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents">The documents.</param>
        /// <param name="boostValue">The boost value to apply to all documents.</param>
        public void Add(IEnumerable<T> documents, double? boostValue)
        {
            var docs = documents.Select(d => new KeyValuePair<T, double?>(d, boostValue));
            var cmd = new AddCommand<T>(docs, solrDocumentSerializer, null);
            solrBasicOperations.Send(cmd);
        }

        ///// <summary>
        ///// Executes a query
        ///// </summary>
        ///// <typeparam name="T">Document type</typeparam>
        ///// <param name="s">Query</param>
        ///// <param name="start">Pagination item start</param>
        ///// <param name="rows">Pagination item count</param>
        ///// <returns>Query results</returns>
        //public  ISolrQueryResults<T> Query(string s, int start, int rows)
        //{
        //    //var solrQueryExe = ServiceLocator.Current.GetInstance<ISolrQueryExecuter<T>>();
        //    //solrBasicOperations.
        //    return solrQueryExecuter.Execute(new SolrQuery(s), new QueryOptions { Start = start, Rows = rows });
        //}

        ///// <summary>
        ///// Executes a query
        ///// </summary>
        ///// <typeparam name="T">Document type</typeparam>
        ///// <param name="s">Query</param>
        ///// <returns>Query results</returns>
        //public static ISolrQueryResults<T> Query<T>(string s) where T : ISchema
        //{
        //    var solrQueryExe = ServiceLocator.Current.GetInstance<ISolrQueryExecuter<T>>();
        //    return solrQueryExe.Execute(new SolrQuery(s), null);
        //}

        ///// <summary>
        ///// Executes a query
        ///// </summary>
        ///// <typeparam name="T">Document type</typeparam>
        ///// <param name="s">Query</param>
        ///// <param name="order">Sort order</param>
        ///// <returns>Query results</returns>
        //public static ISolrQueryResults<T> Query<T>(string s, SortOrder order) where T : ISchema
        //{
        //    return Query<T>(s, new[] { order });
        //}

        ///// <summary>
        ///// Executes a query
        ///// </summary>
        ///// <typeparam name="T">Document type</typeparam>
        ///// <param name="s">Query</param>
        ///// <param name="order">Sort orders</param>
        ///// <returns>Query results</returns>
        //public static ISolrQueryResults<T> Query<T>(string s, ICollection<SortOrder> order) where T : ISchema
        //{
        //    var solrQueryExe = ServiceLocator.Current.GetInstance<ISolrQueryExecuter<T>>();
        //    return solrQueryExe.Execute(new SolrQuery(s), new QueryOptions { OrderBy = order });
        //}

        ///// <summary>
        ///// Executes a query
        ///// </summary>
        ///// <typeparam name="T">Document type</typeparam>
        ///// <param name="s">Query</param>
        ///// <param name="order">Sort order</param>
        ///// <param name="start">Pagination item start</param>
        ///// <param name="rows">Pagination item count</param>
        ///// <returns>Query results</returns>
        //public static ISolrQueryResults<T> Query<T>(string s, SortOrder order, int start, int rows) where T : ISchema
        //{
        //    return Query<T>(s, new[] { order }, start, rows);
        //}

        ///// <summary>
        ///// Executes a query
        ///// </summary>
        ///// <typeparam name="T">Document type</typeparam>
        ///// <param name="s">Query</param>
        ///// <param name="order">Sort orders</param>
        ///// <param name="start">Pagination item start</param>
        ///// <param name="rows">Pagination item count</param>
        ///// <returns>Query results</returns>
        //public static ISolrQueryResults<T> Query<T>(string s, ICollection<SortOrder> order, int start, int rows) where T : ISchema
        //{
        //    var solrQueryExe = ServiceLocator.Current.GetInstance<ISolrQueryExecuter<T>>();
        //    return solrQueryExe.Execute(new SolrQuery(s),
        //        new QueryOptions
        //        {
        //            OrderBy = order,
        //            Start = start,
        //            Rows = rows,
        //        }
        //    );
        //}

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="q">Query</param>
        /// <returns>Query results</returns>
        public  ISolrQueryResults<T> Query(ISolrQuery q) 
        {
            return solrQueryExecuter.Execute(q, null);
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="q">Query</param>
        /// <param name="start">Pagination item start</param>
        /// <param name="rows">Pagination item count</param>
        /// <returns>Query results</returns>
        public  ISolrQueryResults<T> Query(ISolrQuery q, int start, int rows)
        {
            return solrQueryExecuter.Execute(q, new QueryOptions { Start = start, Rows = rows });
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="query">Query</param>
        /// <param name="order">Sort order</param>
        /// <returns>Query results</returns>
        public  ISolrQueryResults<T> Query(SolrQuery query, SortOrder order) 
        {
            return Query(query, new[] { order });
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="query">Query</param>
        /// <param name="orders">Sort orders</param>
        /// <returns>Query results</returns>
        public ISolrQueryResults<T> Query(SolrQuery query, ICollection<SortOrder> orders) 
        {
            return solrQueryExecuter.Execute(query, new QueryOptions { OrderBy = orders });
        }

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <typeparam name="T">Document type</typeparam>
        /// <param name="query">Query</param>
        /// <param name="options">The QueryOptions to use.</param>
        /// <returns>Query results</returns>
        public ISolrQueryResults<T> Query(SolrQuery query, QueryOptions options)
        {
            return solrQueryExecuter.Execute(query, options);
        }

        ///<summary>
        /// Returns new <see cref="IDSLQuery{TSchema}"/> query object
        ///</summary>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public IDSLQuery<T> Query()
        {
            return new DSLQuery<T>(solrQueryExecuter, solrFieldSerializer);
        }

        /// <summary>
        /// Commits posted documents
        /// </summary>
        public void Commit() 
        {
            solrBasicOperations.Commit(null);
        }

        /// <summary>
        /// Commits posted documents
        /// </summary>
        /// <param name="waitFlush">wait for flush</param>
        /// <param name="waitSearcher">wait for new searcher</param>
        public void Commit(bool waitFlush, bool waitSearcher) 
        {
            solrBasicOperations.Commit(new CommitOptions { WaitFlush = waitFlush, WaitSearcher = waitSearcher });
        }

        /// <summary>
        /// Commits posted documents
        /// </summary>
        /// <param name="waitFlush">wait for flush</param>
        /// <param name="waitSearcher">wait for new searcher</param>
        /// <param name="expungeDeletes">Merge segments with deletes away</param>
        public  void Commit(bool waitFlush, bool waitSearcher, bool expungeDeletes) 
        {
            solrBasicOperations.Commit(new CommitOptions { WaitFlush = waitFlush, WaitSearcher = waitSearcher, ExpungeDeletes = expungeDeletes });
        }

        /// <summary>
        /// Commits posted documents
        /// </summary>
        /// <param name="waitFlush">wait for flush</param>
        /// <param name="waitSearcher">wait for new searcher</param>
        /// <param name="expungeDeletes">Merge segments with deletes away</param>
        /// <param name="maxSegments">Optimizes down to, at most, this number of segments</param>
        public  void Commit(bool waitFlush, bool waitSearcher, bool expungeDeletes, int maxSegments) 
        {
            solrBasicOperations.Commit(new CommitOptions { WaitFlush = waitFlush, WaitSearcher = waitSearcher, ExpungeDeletes = expungeDeletes, MaxSegments = maxSegments });
        }

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        public  void Optimize() 
        {
            solrBasicOperations.Optimize(null);
        }

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        /// <param name="waitFlush">Wait for flush</param>
        /// <param name="waitSearcher">Wait for new searcher</param>
        public  void Optimize(bool waitFlush, bool waitSearcher) 
        {
            solrBasicOperations.Optimize(new CommitOptions { WaitFlush = waitFlush, WaitSearcher = waitSearcher });
        }

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        /// <param name="waitFlush">Wait for flush</param>
        /// <param name="waitSearcher">Wait for new searcher</param>
        /// <param name="expungeDeletes">Merge segments with deletes away</param>
        public  void Optimize(bool waitFlush, bool waitSearcher, bool expungeDeletes) 
        {
            solrBasicOperations.Optimize(new CommitOptions { WaitFlush = waitFlush, WaitSearcher = waitSearcher, ExpungeDeletes = expungeDeletes });
        }

        /// <summary>
        /// Optimizes Solr's index
        /// </summary>
        /// <param name="waitFlush">Wait for flush</param>
        /// <param name="waitSearcher">Wait for new searcher</param>
        /// <param name="expungeDeletes">Merge segments with deletes away</param>
        /// <param name="maxSegments">Optimizes down to, at most, this number of segments</param>
        public  void Optimize(bool waitFlush, bool waitSearcher, bool expungeDeletes, int maxSegments) 
        {
            solrBasicOperations.Optimize(new CommitOptions { WaitFlush = waitFlush, WaitSearcher = waitSearcher, ExpungeDeletes = expungeDeletes, MaxSegments = maxSegments });
        }
    }
}