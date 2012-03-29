﻿#region SchemaDSL license
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
using SolrNet.Impl;

namespace SolrNet.SchemaDSL.Impl {
    internal class DSLFilterBy<T, FT> : IDSLFilterBy<T, FT> where T : ISchema {
        private bool negate = false;
        private readonly string fieldName;
        private readonly DSLRun<T> parentDslRun;
        private string tag;


        internal DSLFilterBy(DSLRun<T> parentDslRun, string fieldName) {
            this.fieldName = fieldName;
            this.parentDslRun = parentDslRun;
        }

        public IDSLQuery<T> Is(FT value) {
            IList<ISolrQuery> filters = new List<ISolrQuery>();

            var serializer = parentDslRun.SolrFieldSerializer;

            var nodes = serializer.Serialize(value);

            foreach (var node in nodes) {
                var queryByField = new SolrQueryByField(fieldName, node.FieldValue);
                filters.Add(queryByField);
            }

            ISolrQuery newQuery;
            if (filters.Count == 1) {
                newQuery = filters[0];
            } else {
                newQuery = new SolrMultipleCriteriaQuery(filters); //.Tag(tag);;
            }

            if (this.negate) {
                newQuery = new SolrNotQuery(newQuery);
            }

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        private ISolrQuery TagQuery(ISolrQuery query) {
            if (!string.IsNullOrEmpty(tag)) {
                var localParams = new LocalParams {{"tag", tag}};
                query = localParams + query;
            }
            return query;
        }

        public IDSLQuery<T> HasValue() {
            ICollection<ISolrQuery> filters = new List<ISolrQuery>();

            filters.Add(new SolrQueryByField(fieldName, "*"));

            ISolrQuery newQuery = new SolrMultipleCriteriaQuery(filters); //.Tag(tag);

            if (this.negate) {
                newQuery = new SolrNotQuery(newQuery);
            }

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        public IDSLQuery<T> Between(Interval<FT> range) {
            ISolrQuery newQuery = new SolrQueryByRange<FT>(fieldName, range.Min.Value, range.Max.Value, range.Min.Type == IntervalBorderType.Inclusive, range.Max.Type == IntervalBorderType.Inclusive);

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        public IDSLQuery<T> Between(FT min, FT max, bool minInclusive, bool maxInclusive) {
            ISolrQuery newQuery = new SolrQueryByRange<FT>(fieldName, min, max, minInclusive, maxInclusive);

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        public IDSLQuery<T> BetweenEx(Interval<FT> range) {
            ISolrQuery newQuery = new SolrQueryByRange<FT>(fieldName, range.Min.Value, range.Max.Value, range.Min.Type == IntervalBorderType.Inclusive, range.Max.Type == IntervalBorderType.Inclusive);

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        public IDSLQuery<T> BetweenEx(FT min, FT max, bool minInclusive, bool maxInclusive) {
            ISolrQuery newQuery = new SolrQueryByRange<FT>(fieldName, min, max, minInclusive, maxInclusive);

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        public IDSLQuery<T> In(IEnumerable<FT> values) {
            return inInternal(values);
        }

        public IDSLQuery<T> In<ST>(IEnumerable<ST> values) where ST : struct {
            return this.inInternal(values);
        }

        private IDSLQuery<T> inInternal(System.Collections.IEnumerable values) {
            ICollection<ISolrQuery> filters = new List<ISolrQuery>();

            var serializer = parentDslRun.SolrFieldSerializer;

            foreach (var value in values) {
                var nodes = serializer.Serialize(value);

                foreach (var node in nodes) {
                    filters.Add(new SolrQueryByField(fieldName, node.FieldValue));
                }
            }

            ISolrQuery newQuery = new SolrMultipleCriteriaQuery(filters, SolrMultipleCriteriaQuery.Operator.OR); //.Tag(tag);

            if (this.negate) {
                newQuery = new SolrNotQuery(newQuery);
            }

            newQuery = TagQuery(newQuery);

            return new DSLQuery<T>(parentDslRun, newQuery);
        }

        public IDSLFilterBy<T, FT> Negate() {
            this.negate = true;
            return this;
        }

        public IDSLFilterBy<T, FT> Tag(string tagName) {
            this.tag = tagName;
            return this;
        }
    }
}