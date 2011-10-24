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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.SchemaDSL.Utils;
#pragma warning disable 1591

namespace SolrNet.SchemaDSL.Impl
{
    class DSLRun<T> : IDSLRun<T> where T : ISchema
    {
        internal readonly ISolrFieldSerializer SolrFieldSerializer;
        private readonly ISolrQueryExecuter<T> solrQueryExecuter;

        protected readonly IDictionary<Guid, ISolrQuery> Filters = new Dictionary<Guid, ISolrQuery>();
        protected readonly ICollection<SortOrder> Order = new List<SortOrder>();
        protected readonly ICollection<ISolrFacetQuery> Facets = new List<ISolrFacetQuery>();
        protected readonly HighlightingParameters Highlight;
        protected readonly ISolrQuery QueryField;
        //protected readonly string DefTypeField;
        protected readonly IDictionary<string, string> ExtraParams = new Dictionary<string, string>();

        protected readonly Guid DslRunId = Guid.NewGuid();
        protected readonly Guid ParentDslRunId;

        public DSLRun(ISolrQueryExecuter<T> solrQueryExecuter, ISolrFieldSerializer solrFieldSerializer) {
            this.SolrFieldSerializer = solrFieldSerializer;
            this.solrQueryExecuter = solrQueryExecuter;
        }

        internal DSLRun(DSLRun<T> parentDslRun)
        {
            //Guard.ArgumentNotNull(parentDslRun, "parentDslRun");

            this.Filters = parentDslRun.Filters;
            this.Order = parentDslRun.Order;
            this.Facets = parentDslRun.Facets;
            this.Highlight = parentDslRun.Highlight;
            this.QueryField = parentDslRun.QueryField;
            //this.DefTypeField = parentDslRun.DefTypeField;
            this.ExtraParams = parentDslRun.ExtraParams;
            this.solrQueryExecuter = parentDslRun.solrQueryExecuter;
            this.SolrFieldSerializer = parentDslRun.SolrFieldSerializer;

            this.ParentDslRunId = parentDslRun.DslRunId;
        }

        internal DSLRun(DSLRun<T> parentDslRun, ISolrQuery newQuery)
            : this(parentDslRun)
        {
            // combine new query if neccesary
            if (this.QueryField != null || newQuery != null)
            {
                if (this.QueryField == null)
                {
                    this.QueryField = newQuery;
                }
                else if (newQuery != null)
                {
                    this.QueryField = new SolrMultipleCriteriaQuery(new List<ISolrQuery> { parentDslRun.QueryField, QueryField });
                }
            }
        }

        //internal DSLRun(ISolrQuery query)
        //    : this()
        //{
        //    this.query = query;
        //}

        internal DSLRun(DSLRun<T> dslRun, ICollection<SortOrder> order)
            : this(dslRun)
        {
            this.Order = new List<SortOrder>(this.Order);
            this.Order.AddCollection(order);
        }

        internal DSLRun(DSLRun<T> dslRun, ICollection<ISolrFacetQuery> facets)
            : this(dslRun) {
            this.Facets = new List<ISolrFacetQuery>(this.Facets);
            this.Facets.AddCollection(facets);
        }

        internal DSLRun(DSLRun<T> dslRun, HighlightingParameters highlight)
            : this(dslRun)
        {
            this.Highlight = highlight;
        }

        internal DSLRun(DSLRun<T> dslRun, ICollection<KeyValuePair<string, string>> extraParams)
            : this(dslRun)
        {
            this.ExtraParams = new Dictionary<string, string>(this.ExtraParams);
            this.ExtraParams.AddCollection(extraParams);
        }


        //internal DSLRun(ISolrQuery query, ICollection<SortOrder> order, ICollection<ISolrFacetQuery> facets, HighlightingParameters highlight)
        //    : this(query, order)
        //{
        //    this.facets = facets;
        //    this.highlight = highlight;
        //}

        public ISolrQueryResults<T> Run()
        {
            return RunInternal(null, null, null);
        }

        public ISolrQueryResults<T> Run(int start, int rows)
        {
            return RunInternal(start, rows, null);
        }


        internal ISolrQueryResults<T> RunInternal(int? start, int? rows, string handler)
        {
            var extraParams = new Dictionary<string, string>(this.ExtraParams);

            if (handler != null)
            {
                extraParams.Add("qt", handler);
            }

            //if (DefTypeField != null)
            //{
            //    extraParams.Add("defType", DefTypeField);
            //}

            //if (boostFunction != null)
            //{
            //    extraParams.Add("bf", string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}^{1}", boostFunction.function, boostFunction.boost));
            //}

            var solrQuery = QueryField;

            if (QueryField == null) {
                solrQuery = SolrQuery.All;
            }

            return solrQueryExecuter.Execute(solrQuery, new QueryOptions
            {
                OrderBy = Order,
                Facet = new FacetParameters
                {
                    Queries = Facets,
                },
                Start = start,
                Rows = rows,
                Highlight = Highlight,
                //MoreLikeThis = moreLikeThis, //TODO: mlt
                FilterQueries = Filters.Values, //TODO: fq
                //BoostQueries = boosts, //TODO: bq
                //Fields = fields, //TODO: fl
                ExtraParams = extraParams,
                //Stats = this.stats, //TODO: stats
            });
        }

        public IDSLRun<T> OrderBy<TField>(Expression<Func<T, TField>> fieldExpression)
        {
            var newOrder = new List<SortOrder> { new SortOrder(fieldExpression.GetSolrFieldName()) };
            return new DSLRun<T>(this, newOrder);
        }

        public IDSLRun<T> OrderBy<TField>(Expression<Func<T, TField>> fieldExpression, Order o)
        {
            var newOrder = new List<SortOrder> { new SortOrder(fieldExpression.GetSolrFieldName(), o) };
            return new DSLRun<T>(this, newOrder);
        }

        public IDSLFacetFieldOptions<T> WithFacetField<TField>(Expression<Func<T, TField>> fieldExpression)
        {
            var facetFieldQuery = new SolrFacetFieldQuery(fieldExpression.GetSolrFieldName());
            var newFacets = new List<ISolrFacetQuery> { facetFieldQuery };
            return new DSLFacetFieldOptions<T>(this, newFacets, facetFieldQuery);
        }

        public IDSLFacetFieldOptions<T> WithFacetField<TField>(string fieldName) {
            var facetFieldQuery = new SolrFacetFieldQuery(fieldName);
            var newFacets = new List<ISolrFacetQuery> { facetFieldQuery };
            return new DSLFacetFieldOptions<T>(this, newFacets, facetFieldQuery);
        }

        public IDSLRun<T> WithFacetQuery(string q)
        {
            return WithFacetQuery(new SolrQuery(q));
        }

        public IDSLRun<T> WithFacetQuery(ISolrQuery q)
        {
            var newFacets = new List<ISolrFacetQuery>(Facets) { new SolrFacetQuery(q) };
            return new DSLRun<T>(this, newFacets);
        }

        //TODO: implement DSL HighlightingParameters
        public IDSLRun<T> WithHighlighting(HighlightingParameters parameters)
        {
            return new DSLRun<T>(this, parameters);
        }

        public IDSLRun<T> WithHighlightingFields<TField>(params Expression<Func<T, TField>>[] fieldExpression)
        {
            return WithHighlighting(new HighlightingParameters
            {
                Fields = fieldExpression.Select(f => f.GetSolrFieldName()).ToList(),
            });
        }

        public IDSLRun<T> DefType(string defType) {
            var dslRun = new DSLRun<T>(this, new Dictionary<string, string> {{"defType", defType}});
            return dslRun;
        }

        public IDSLRun<T> WithQueryType(string queryType) {
            var dslRun = new DSLRun<T>(this, new Dictionary<string, string> { { "qt", queryType } });
            return dslRun;
        }

        //public override string ToString()
        //{
        //    var l = new List<string>();
        //    var serializer = ServiceLocator.Current.GetInstance<ISolrQuerySerializer>();
        //    if (QueryField != null)
        //        l.Add(serializer.Serialize(QueryField));
        //    if (Highlight != null)
        //        l.Add("highlight=" + Highlight);
        //    if (Facets != null)
        //        l.Add("facets=" + string.Join("\n", Facets.Select(f => f.ToString()).ToArray()));

        //    return string.Join("\n", l.ToArray());
        //}
    }

    static class Helpers {
        public static void AddCollection<T>(this ICollection<T> baseCollection, ICollection<T> newItems) {
            if (baseCollection == null)
            {
                throw new ArgumentNullException("baseCollection", "Can't add emements on null collection.");
            }
            if (newItems == null || newItems.Count == 0) {
                return;
            }
           
            foreach (var newItem in newItems) {
                baseCollection.Add(newItem);
            }
        }
    }
}
#pragma warning restore 1591
