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
using System.Linq.Expressions;
using SolrNet.SchemaDSL.Utils;

namespace SolrNet.SchemaDSL.Impl {
#pragma warning disable 1591
    class DSLQueryRange<T, TField> : DSLQuery<T>, IDSLQueryRange<T> where T : ISchema {
        private readonly string fieldName;
        private readonly TField from;
        private readonly TField to;
        //private readonly ISolrQuery prevQuery;

        //internal DSLQueryRange(ISolrQuery query, Expression<Func<T, TField>> fieldExpression, TField from, TField to)
        //    : base(GetSolrQueryByRangeCombined(query,  fieldExpression,  from,  to))
        //{
        //    this.fieldName = fieldExpression.GetSolrFieldName();
        //    this.from = from;
        //    this.to = to;
        //    this.prevQuery = query;
        //}

        internal DSLQueryRange(DSLRun<T> parentDslRun,
                               Expression<Func<T, TField>> fieldExpression, TField from, TField to
            )
            : base(parentDslRun) {
            this.fieldName = fieldExpression.GetSolrFieldName();
            this.from = from;
            this.to = to;

            var newQueryRange = new SolrQueryByRange<TField>(fieldName, from, to);

            if (this.Filters.ContainsKey(this.DslRunId)) {
                this.Filters[this.DslRunId] = newQueryRange;
            } else {
                this.Filters.Add(this.DslRunId, newQueryRange);
            }
        }

        //private static ISolrQuery GetSolrQueryByRangeCombined(ISolrQuery query, Expression<Func<T, TField>> fieldExpression, TField from, TField to)
        //{
        //    var fieldName = fieldExpression.GetSolrFieldName();
        //    var solrQueryByRange= new SolrQueryByRange<TField>(fieldName, from, to);
        //    return new SolrMultipleCriteriaQuery(new[] {
        //        query,
        //        solrQueryByRange,
        //    });
        //}

        private ISolrQuery BuildFinalQuery(bool inclusive) {
            return new SolrQueryByRange<TField>(fieldName, from, to, inclusive);
        }

        public IDSLQuery<T> Exclusive() {
            return new DSLQuery<T>(this, BuildFinalQuery(false));
        }

        public IDSLQuery<T> Inclusive() {
            return new DSLQuery<T>(this, BuildFinalQuery(true));
        }
    }
}

#pragma warning restore 1591
