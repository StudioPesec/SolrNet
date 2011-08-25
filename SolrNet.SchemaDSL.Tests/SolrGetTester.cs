#region SchemaDSL license
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
using System.Linq;

namespace SolrNet.SchemaDSL.Tests {
    public class SolrGetTester {
        public delegate bool ValidateSolrGetDelegate(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// CallBack deleegate for validation
        /// </summary>
        public readonly ValidateSolrGetDelegate ValidateSolrGet;

        private readonly string expectedRelativeUrl;

        private readonly ICollection<KeyValuePair<string, string>> expectedParametersList;

        /// <summary>
        /// Validation helper for <see cref="ISolrConnection"/>.Get
        /// </summary>
        /// <param name="expectedRelativeUrl"></param>
        /// <param name="parameters"></param>
        /// <example>
        /// <![CDATA[
        ///  var solrGetParameters = new Dictionary<string, string> 
        ///     {
        ///         {"q", "(Id:123456)"},
        ///         {"sort", "Id asc"},
        ///         {"rows", DefaultRows().ToString()},
        ///     };
        ///  With.Mocks(mocks)
        ///     .Expecting(() => Expect
        ///         .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
        ///         .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
        ///         .Return(EmptySolrResponse))
        ///         
        ///     .Verify(() => Solr
        ///         .Query<TestDocument>()
        ///         .FilterBy(f => f.Id).Is(123456)
        ///         .OrderBy(f => f.Id, Order.ASC)
        ///         .Run(0, DefaultRows())
        ///     );
        /// ]]>
        /// </example>
        public SolrGetTester(string expectedRelativeUrl, ICollection<KeyValuePair<string, string>> parameters) {
            this.ValidateSolrGet = ValidateSolrGetDelegateImplementation;
            this.expectedRelativeUrl = expectedRelativeUrl;
            this.expectedParametersList = parameters;
        }

        private bool ValidateSolrGetDelegateImplementation(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            var actualParameterList = parameters.ToList();
            MbUnit.Framework.Assert.AreEqual(this.expectedRelativeUrl, relativeUrl);

            MbUnit.Framework.Assert.AreElementsEqualIgnoringOrder(expectedParametersList, actualParameterList, "Collection check");
            MbUnit.Framework.Assert.AreElementsEqualIgnoringOrder(expectedParametersList.Select(f => f.Key), actualParameterList.Select(f => f.Key), "Key check");
            MbUnit.Framework.Assert.AreElementsEqualIgnoringOrder(expectedParametersList.Select(f => f.Value), actualParameterList.Select(f => f.Value), "Value check");

#region OBSOLETE_ExceptionMemberTest
#if false && OBSOLETE_ExceptionMemberTest
            var sb = new System.Text.StringBuilder();
            if (relativeUrl != this.expectedRelativeUrl) {
                sb.AppendLine();
                sb.AppendFormat("Expected 'relativeUrl': '{0}' Actual: '{1}'", expectedRelativeUrl, relativeUrl);
            }

            var actualParameterGroups = actualParameterList
                .OrderBy(f => f.Key)
                .ThenBy(f => f.Value)
                .GroupBy(f => f.Key)
                .ToDictionary(f => f.Key, f => f.Select(e => e).ToArray());

            var expectedParameterGroups = expectedParametersList
                .OrderBy(f => f.Key)
                .ThenBy(f => f.Value)
                .GroupBy(f => f.Key)
                .ToDictionary(f => f.Key, f => f.Select(e => e).ToArray());

            foreach (var expectedParameterGroup in expectedParameterGroups) {
                if (!actualParameterGroups.ContainsKey(expectedParameterGroup.Key)) {
                    sb.AppendLine();
                    sb.AppendFormat("Expected 'parameter.[\"{0}\"]'. Actual: <Missing>", expectedParameterGroup.Key);
                }

                var actualParametes = actualParameterGroups[expectedParameterGroup.Key].ToArray();

                foreach (var expectedParameter in expectedParameterGroup.Value) {
                    var actualParameter = actualParametes.Where(f => f.Value == expectedParameter.Value).ToArray();
                    if (actualParameter.Length == 0) {
                        sb.AppendLine();
                        sb.AppendFormat("Expected 'parameter.[\"{0}\"]'= '{1}'. Actual: exact value not found.", expectedParameter.Key, expectedParameter.Value);
                    }
                }

            }

            if (expectedParametersList.Count != actualParameterList.Count) {
                sb.AppendLine();
                sb.AppendLine(string.Format("Expected parameter count: {0}, Actual parameter count {1}.", expectedParametersList.Count, actualParameterList.Count));

            }

            if (sb.Length > 0) {
                var actualParamsArray = actualParameterList
                    .OrderBy(f => f.Key)
                    .Select(f => string.Format("{0}={1}", f.Key, f.Value))
                    .ToArray();
                sb.AppendLine("Actual parameters:");
                sb.AppendLine(string.Join("\r\n\t", actualParamsArray));
            }
#endif
#endregion OBSOLETE_ExceptionMemberTest
            return true;
        }
    }

    public class KeyValuePairList : List<KeyValuePair<string, string>> {
        public void Add(string key, string value) {
            Add(new KeyValuePair<string, string>(key, value));
        }
    }
}