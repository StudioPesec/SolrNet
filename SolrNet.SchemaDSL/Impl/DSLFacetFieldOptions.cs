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

namespace SolrNet.SchemaDSL.Impl
{
    class DSLFacetFieldOptions<T> : DSLRun<T>, IDSLFacetFieldOptions<T> where T : ISchema
    {
        //private readonly ISolrFacetQuery currentFacet;
        private readonly SolrFacetFieldQuery facetQuery;
        //private readonly string excludeTag;

        internal ISolrFacetQuery FacetQuery
        {
            get { return this.facetQuery; }
        }


        internal DSLFacetFieldOptions(DSLRun<T> parentDslRun, SolrFacetFieldQuery facetQuery) 
            : base(parentDslRun) {
            this.facetQuery = facetQuery;
        }

        //internal DSLFacetFieldOptions(DSLRun<T> parentDslRun,  ISolrFacetQuery facet, SolrFacetFieldQuery facetQuery)
        //    : base(parentDslRun)
        //{
        //    this.currentFacet = facet;
        //    this.facetQuery = facetQuery;
        //    this.Facets.Add(facet);
        //}

        //internal DSLFacetFieldOptions(DSLRun<T> parentDslRun, string excludeTagName) 
        //    :base(parentDslRun) {
        //    excludeTag = excludeTagName;

        //    facetQuery = TagQuery(facetQuery);

        //    foreach (var facet in Facets) {
        //        this.Facets.Add(facet);
        //    }
        //}

        //private SolrFacetFieldQuery TagQuery(SolrFacetFieldQuery query)
        //{
        //    if (!string.IsNullOrEmpty(excludeTag))
        //    {
        //        var localParams = new LocalParams { { "ex", excludeTag } };
        //        query = localParams + query;
        //    }
        //    return query;
        //}

        public IDSLFacetFieldOptions<T> LimitTo(int limit) {
            facetQuery.Limit = limit;
            return new DSLFacetFieldOptions<T>(this, facetQuery);
        }

        public IDSLFacetFieldOptions<T> DontSortByCount() {
            facetQuery.Sort = false;
            return new DSLFacetFieldOptions<T>(this, facetQuery);
        }

        public IDSLFacetFieldOptions<T> WithPrefix(string prefix) {
            facetQuery.Prefix = prefix;
            return new DSLFacetFieldOptions<T>(this, facetQuery);
        }

        public IDSLFacetFieldOptions<T> WithMinCount(int count) {
            facetQuery.MinCount = count;
            return new DSLFacetFieldOptions<T>(this, facetQuery);
        }

        public IDSLFacetFieldOptions<T> StartingAt(int offset) {
            facetQuery.Offset = offset;
            return new DSLFacetFieldOptions<T>(this, facetQuery);
        }

        public IDSLFacetFieldOptions<T> IncludeMissing() {
            facetQuery.Missing = true;
            return new DSLFacetFieldOptions<T>(this, facetQuery);
        }

        public IDSLFacetFieldOptions<T> ExcludeTag(string tagName) {

            var localParams = new LocalParams { { "ex", tagName } };
            var newQuery = new SolrFacetFieldQuery(localParams + facetQuery.Field) {
                EnumCacheMinDf = facetQuery.EnumCacheMinDf,
                Limit = facetQuery.Limit,
                MinCount = facetQuery.MinCount,
                Missing = facetQuery.Missing,
                Offset = facetQuery.Offset,
                Prefix = facetQuery.Prefix,
                Sort = facetQuery.Sort,
            };

            var result = new DSLFacetFieldOptions<T>(this, newQuery);
            Facets.Remove(this);
            Facets.Add(result);

            return result;
        }
    }
}