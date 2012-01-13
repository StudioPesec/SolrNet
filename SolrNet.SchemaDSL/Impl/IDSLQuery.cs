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

namespace SolrNet.SchemaDSL.Impl
{
    ///<summary>
    ///</summary>
    ///<typeparam name="TSchema"></typeparam>
    public interface IDSLQuery<TSchema> : IDSLRun<TSchema> where TSchema : ISchema
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="fieldExpression"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IDSLQueryRange<TSchema> ByRange<TField>(Expression<Func<TSchema, TField>> fieldExpression, TField from, TField to);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="fieldExpression"></param>
        /// <returns></returns>
        IDSLQueryBy<TSchema, TField> By<TField>(Expression<Func<TSchema, TField>> fieldExpression);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="fieldExpression"></param>
        /// <returns></returns>
        IDSLFilterBy<TSchema, TField> FilterBy<TField>(Expression<Func<TSchema, TField>> fieldExpression);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        IDSLFilterBy<TSchema, TField> FilterBy<TField>(string fieldName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IDSLQuery<TSchema> Query(string query);

        /// <summary>
        /// Appends any SolrNet query
        /// </summary>
        /// <param name="solrQueries"></param>
        /// <returns></returns>
        IDSLQuery<TSchema> AppednQuery(params ISolrQuery[] solrQueries);
    }
}