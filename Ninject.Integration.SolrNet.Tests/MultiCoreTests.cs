#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using MbUnit.Framework;
using SolrNet;

namespace Ninject.Integration.SolrNet.Tests {
    [TestFixture]
    public class MultiCoreTests {

        [Test]
        [Category("Integration")]
        public void MultiCoreTest() {

            var c = new StandardKernel();

            const string Core1Url = "http://localhost:1001/Core1";
            const string Core2Url = "http://localhost:1002/Core2";
            {
                //System.Diagnostics.Debugger.Launch();


                c.Load(new SolrNetModule<Entity1>(Core1Url));
                c.Load(new SolrNetModule<Entity2>(Core2Url));

                {
                    var solr = c.Get<ISolrOperations<Entity1>>();

                    try {
                        solr.Ping();
                    } catch (global::SolrNet.Exceptions.SolrConnectionException solrException) {
                        System.Diagnostics.Debug.WriteLine(solrException.Url, "Core1.Ping");
                        Assert.AreEqual(Core1Url + "/admin/ping?version=2.2", solrException.Url);
                    }
                }

                {
                    var solr2 = c.Get<ISolrOperations<Entity2>>();

                    try {
                        solr2.Ping();
                    } catch (global::SolrNet.Exceptions.SolrConnectionException solrException) {
                        System.Diagnostics.Debug.WriteLine(solrException.Url, "Core2.Ping");
                        Assert.AreEqual(Core2Url + "/admin/ping?version=2.2", solrException.Url);
                    }
                }
            }

        }

        public class Entity1 {}

        public class Entity2 {}
    }
}