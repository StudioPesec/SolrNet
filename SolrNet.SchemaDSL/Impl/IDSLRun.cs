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
using SolrNet.Commands.Parameters;

namespace SolrNet.SchemaDSL.Impl
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
    public interface IDSLRun<T> where T : ISchema
    {
        ISolrQueryResults<T> Run();
        ISolrQueryResults<T> Run(int start, int rows);
        IDSLRun<T> OrderBy<TField>(Expression<Func<T, TField>> fieldExpression);
        IDSLRun<T> OrderBy<TField>(Expression<Func<T, TField>> fieldExpression, Order o);
        IDSLFacetFieldOptions<T> WithFacetField<TField>(Expression<Func<T, TField>> fieldExpression);
        IDSLFacetFieldOptions<T> WithFacetField<TField>(string fieldName);
        IDSLRun<T> WithFacetQuery(string query);
        IDSLRun<T> WithFacetQuery(ISolrQuery query);
        IDSLRun<T> WithHighlighting(HighlightingParameters parameters);
        IDSLRun<T> WithHighlightingFields<TField>(params Expression<Func<T, TField>>[] fieldExpression);
        IDSLRun<T> DefType(string defType);
        IDSLRun<T> WithQueryType(string queryType);
        ISpellCheckOptions<T> WithSpellCheck(string query);
        IDSLRun<T> AddExtraParams(ICollection<KeyValuePair<string, string>> extraParams);
        IDSLMoreLikeThis<T> MoreLikeThis<TField>(params Expression<Func<T, TField>>[] fieldExpression);
    }
}