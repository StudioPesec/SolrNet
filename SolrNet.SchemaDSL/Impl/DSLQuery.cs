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
using System.Linq.Expressions;
using SolrNet.Impl;
using SolrNet.SchemaDSL.Utils;

namespace SolrNet.SchemaDSL.Impl {
    class DSLQuery<T> : DSLRun<T>, IDSLQuery<T> where T : ISchema {
        internal DSLQuery(ISolrQueryExecuter<T> solrQueryExecuter, ISolrFieldSerializer solrFieldSerializer)
            : base(solrQueryExecuter, solrFieldSerializer) {}

        internal DSLQuery(DSLRun<T> parentDSLRun)
            : base(parentDSLRun) {}
        internal DSLQuery(DSLRun<T> parentDSLRun, string searchText)
            : base(parentDSLRun, new SolrQuery(searchText)) { }

        internal DSLQuery(DSLRun<T> parentDSLRun, params ISolrQuery[] filters)
            : base(parentDSLRun) {
            foreach (var filter in filters) {
                if (filter == null) {
                    continue;
                }

                //overwirite possible existing filtes
                if (this.Filters.ContainsKey(this.ParentDslRunId)) {
                    this.Filters[this.ParentDslRunId] = filter;
                } else {
                    this.Filters.Add(this.ParentDslRunId, filter);
                }
            }
        }

        public IDSLQueryRange<T> ByRange<TField>(Expression<Func<T, TField>> fieldExpression, TField from, TField to) {
            return new DSLQueryRange<T, TField>(this, fieldExpression, from, to);
        }

        public IDSLQueryBy<T, TField> By<TField>(Expression<Func<T, TField>> fieldExpression) {
            return new DSLQueryBy<T, TField>(this, fieldExpression.GetSolrFieldName());
        }

        public IDSLFilterBy<T, TField> FilterBy<TField>(Expression<Func<T, TField>> fieldExpression) {
            return new DSLFilterBy<T, TField>(this, fieldExpression.GetSolrFieldName());
        }

        public IDSLFilterBy<T, TField> FilterBy<TField>(string fieldName) {
            return new DSLFilterBy<T, TField>(this, fieldName);
        }

        public IDSLQuery<T> Query(string searchString) {
            return new DSLQuery<T>(this, searchString);
        }

        public IDSLQuery<T> AppednQuery(params ISolrQuery[] solrQueries) {
            return new DSLQuery<T>(this, solrQueries);
        }
    }
}