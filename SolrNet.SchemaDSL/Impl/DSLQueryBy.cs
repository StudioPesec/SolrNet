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
using SolrNet.Impl;

#pragma warning disable 1591

namespace SolrNet.SchemaDSL.Impl
{
    class DSLQueryBy<T, TField> : IDSLQueryBy<T, TField> where T : ISchema
    {
        private readonly string fieldName;
        private readonly DSLRun<T> parentDslRun;

        internal DSLQueryBy(DSLRun<T> parentDslRun, string fieldName) {
            this.parentDslRun = parentDslRun;
            this.fieldName = fieldName;
        }

        public IDSLQuery<T> Is(TField value) {
            var serializer = parentDslRun.SolrFieldSerializer;

            var nodes = serializer.Serialize(value);

            List<ISolrQuery> queries = new List<ISolrQuery>();

            //queries.Add(this.parentDslRun);

            foreach (var node in nodes)
            {
                queries.Add(new SolrQueryByField(fieldName, node.FieldValue));
            }

            //if (queries.Count == 0) {
            //    return new DSLQuery<T>(this.query);
            //}

            //if (queries.Count == 1)
            //{
            //    return new DSLQuery<T>(queries[0]);
            //}

            return new DSLQuery<T>(this.parentDslRun, queries.ToArray());
        }

        public IDSLQueryBetween<T, TField> Between(TField i)
        {
            return new DSLQueryBetween<T, TField>(this.parentDslRun, fieldName, i);
        }
    }
}
#pragma warning restore 1591
