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
using SolrNet.Attributes;

namespace SolrNet.SchemaDSL.Utils
{
    /// <summary>
    /// asdfas
    /// </summary>
    public static class ReflectionUtility
    {
        /// <summary>
        /// Returns name of field defined with <see cref="SolrNet.Attributes.SolrFieldAttribute"/>
        /// </summary>
        /// <param name="lambdaExpression">Lambda expression returning object property - must be Solr field - <see cref="SolrNet.Attributes.SolrFieldAttribute"/></param>
        /// <returns>Solr field</returns>
        /// <exception cref="System.ArgumentException">ArgumentException is thrown if expression is not an property/field with <see cref="SolrNet.Attributes.SolrFieldAttribute"/></exception>
        public static string GetSolrFieldName<T, TField>(this Expression<Func<T, TField>> lambdaExpression) where T : ISchema
        {
            return GetSolrFieldNameInternal(lambdaExpression);
        }

        /////<summary>
        /////</summary>
        /////<param name="lambdaExpression"></param>
        /////<typeparam name="T"></typeparam>
        /////<returns></returns>
        //public static string GetSolrFieldName<T>(this Expression<Func<T>> lambdaExpression)
        //{
        //    return GetSolrFieldNameInternal(lambdaExpression);
        //}

        /// <summary>
        /// Returns name of field defined with <see cref="SolrNet.Attributes.SolrFieldAttribute"/>
        /// </summary>
        /// <param name="lambdaExpression">Lambda expression returning object property - must be Solr field - <see cref="SolrNet.Attributes.SolrFieldAttribute"/></param>
        /// <returns>Solr field</returns>
        /// <exception cref="System.ArgumentException">ArgumentException is thrown if expression is not an property/field with <see cref="SolrNet.Attributes.SolrFieldAttribute"/></exception>
        public static string[] GetSolrFieldNames<T>(this Expression<Func<T, object[]>> lambdaExpression) where T : ISchema
        {
            return GetSolrFieldNamesInternal(lambdaExpression);
        }

        private static string GetSolrFieldNameInternal(LambdaExpression lambdaExpression) {
            var memberExpressionValue = lambdaExpression.Body as MemberExpression;

            if (memberExpressionValue != null) {
                var solrAttributes = memberExpressionValue.Member.GetCustomAttributes(typeof (SolrFieldAttribute), true);

                if (solrAttributes.Length == 0) {
                    throw new ArgumentException("Missing attribute SolrNet.Attributes.SolrFieldAttribute on " + memberExpressionValue.Member.GetType().FullName + "." + memberExpressionValue.Member.Name, "lambdaExpression");
                }

                SolrFieldAttribute solrAttribute = (SolrFieldAttribute) solrAttributes[0];
                return string.IsNullOrEmpty(solrAttribute.FieldName) ? memberExpressionValue.Member.Name : solrAttribute.FieldName;
            }
            throw new ArgumentException("Can't get member name.", "lambdaExpression");
        }

        private static string[] GetSolrFieldNamesInternal(LambdaExpression lambdaExpression)
        {
            var names = new List<string>();

            var memberExpressionValue = lambdaExpression.Body as MemberExpression;

            if (memberExpressionValue != null)
            {
                var solrAttributes = memberExpressionValue.Member.GetCustomAttributes(typeof(SolrFieldAttribute), true);

                if (solrAttributes.Length != 1)
                {
                    throw new ArgumentException("Missing attribute SolrNet.Attributes.SolrFieldAttribute on " + memberExpressionValue.Member.GetType().FullName + "." + memberExpressionValue.Member.Name, "lambdaExpression");
                }

                //SolrFieldAttribute solrAttribute = solrAttributes[0] as SolrFieldAttribute;

                return names.ToArray(); // string.IsNullOrEmpty(solrAttribute.FieldName) ? memberExpressionValue.Member.Name : solrAttribute.FieldName;
            }
            throw new ArgumentException("Can't get member name.", "lambdaExpression");
        }
    }
}
