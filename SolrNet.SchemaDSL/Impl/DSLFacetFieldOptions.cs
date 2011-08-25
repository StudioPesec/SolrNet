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

namespace SolrNet.SchemaDSL.Impl
{
    class DSLFacetFieldOptions<T> : DSLRun<T>, IDSLFacetFieldOptions<T> where T : ISchema
    {
        private readonly SolrFacetFieldQuery facetQuery;

        internal DSLFacetFieldOptions(DSLRun<T> parentDslRun, SolrFacetFieldQuery facetQuery) 
            : base(parentDslRun) {
            this.facetQuery = facetQuery;
        }

        internal DSLFacetFieldOptions(DSLRun<T> parentDslRun,  List<ISolrFacetQuery> facet, SolrFacetFieldQuery facetQuery)
            : base(parentDslRun)
        {
            this.facetQuery = facetQuery;
            this.Facets.AddCollection(facet);
        }

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
    }
}