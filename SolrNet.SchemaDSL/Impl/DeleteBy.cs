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
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.SchemaDSL.Impl
{
    class DeleteBy<T> : IDeleteBy<T> where T : ISchema
    {
        private readonly ISolrBasicOperations<T> solrBasicOperations;

        internal DeleteBy(ISolrBasicOperations<T> solrBasicOperations) {
            this.solrBasicOperations = solrBasicOperations;
        } 

        //private ISolrQuerySerializer GetQuerySerializer()
        //{
        //    return ServiceLocator.Current.GetInstance<ISolrQuerySerializer>();
        //}

        public void ById(string id)
        {
            solrBasicOperations.Delete(new[] { id }, null);
        }

        public void ByIds(IEnumerable<string> ids)
        {
            solrBasicOperations.Delete(ids, null);
        }

        public void ByQuery(ISolrQuery q)
        {
            solrBasicOperations.Delete(null, q);
        }

        public void ByQuery(string q)
        {
            ByQuery(new SolrQuery(q));
        }
    }
}