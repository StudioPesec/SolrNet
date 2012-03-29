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
using MbUnit.Framework;
using Microsoft.Practices.Unity;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Impl;
using Unity.SolrNetIntegration.Config;

namespace SolrNet.SchemaDSL.Tests {
    /// <summary>
    /// These tests are more to define DSL syntax than anything else.
    /// </summary>
    [TestFixture]
    public class DSLTests {
        public class TestDocument : ISchema {
            [SolrField]
            public int Id { get; set; }

            [SolrField("name")]
            public string Name { get; set; }

            [SolrField]
            public string MakeDesc { get; set; }

            [SolrField]
            public string ModelDesc { get; set; }

            [SolrField]
            public string Category { get; set; }
        }

        private const string EmptySolrResponse = "<response />";

        private ISolrConnection conn;
        private MockRepository mocks;

        private readonly IUnityContainer unityContainer = new UnityContainer();

        private SolrNet.SchemaDSL.Solr<TestDocument> Solr;



        [FixtureSetUp]
        public void FixtureSetup() {
            //System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();


            mocks = new MockRepository();
            conn = mocks.StrictMock<ISolrConnection>();


            var locatorProvider = new UnityServiceLocator(unityContainer);
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => locatorProvider);

            var solrServerTestDocument = new SolrServerElement {
                DocumentType = typeof (TestDocument).AssemblyQualifiedName,
                Id = typeof (TestDocument).Name,
                Url = "http://localhost:1234/solr/TestDocument",
            };
            var solrServers = new SolrServers {
                solrServerTestDocument,
            };

            var solrConfiguration = new Unity.SolrNetIntegration.SolrNetContainerConfiguration();
            solrConfiguration.ConfigureContainer(solrServers, unityContainer);

            var coreConnectionId = solrServerTestDocument.Id + typeof (SolrConnection);

            //unityContainer.RegisterType<ISolrConnection, SolrConnection>(coreConnectionId, new InjectionConstructor());
            unityContainer.RegisterInstance(coreConnectionId, conn);


            Solr = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<SolrNet.SchemaDSL.Solr<TestDocument>>();
            var testSerializer = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISolrFieldSerializer>();


            //Startup.Init<TestDocument>(conn);
        }

        [Test]
        public void Add() {
            mocks.BackToRecordAll();
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Post("/update", @"<add><doc><field name=""Id"">0</field></doc></add>"))
                                     .Return(EmptySolrResponse))
                .Verify(() => Solr.Add(new TestDocument()));
        }

        [Test]
        public void Commit() {
            mocks.BackToRecordAll();
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Post("/update", @"<commit waitSearcher=""true"" waitFlush=""true"" />"))
                                     .Return(EmptySolrResponse))
                .Verify(Solr.Commit);
        }

        [Test]
        public void CommitWithParams() {
            mocks.BackToRecordAll();
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Post("/update", @"<commit waitSearcher=""false"" waitFlush=""false"" />"))
                                     .Return(EmptySolrResponse))
                .Verify(() => Solr.Commit(false, false));
        }

        //[Test]
        //public void DeleteById()
        //{
        //    const string Id = "123456";
        //    mocks.BackToRecordAll();

        //    With.Mocks(mocks)
        //        .Expecting(() => Expect
        //            .Call(conn.Post("/update", string.Format(@"<delete><id>{0}</id></delete>", Id)))
        //            .Return(EmptySolrResponse))
        //        .Verify(() => Solr.Delete<TestDocument>().ById(Id));
        //}

        //[Test]
        //public void DeleteByIds()
        //{
        //    var ids = new[] { "123", "456" };
        //    mocks.BackToRecordAll();

        //    With.Mocks(mocks)
        //        .Expecting(() => Expect
        //            .Call(conn.Post("/update", string.Format(@"<delete><id>{0}</id><id>{1}</id></delete>", ids[0], ids[1])))
        //            .Return(EmptySolrResponse))
        //        .Verify(() => Solr.Delete<TestDocument>().ByIds(ids));
        //}

        //[Test]
        //public void DeleteByQueryString()
        //{
        //    const string Q = "id:123456";
        //    mocks.BackToRecordAll();

        //    With.Mocks(mocks)
        //        .Expecting(() => Expect
        //            .Call(conn.Post("/update", string.Format(@"<delete><query>{0}</query></delete>", Q)))
        //            .Return(EmptySolrResponse))
        //        .Verify(() => Solr.Delete<TestDocument>().ByQuery(Q));
        //}

        [Test]
        public void Optimize() {
            mocks.BackToRecordAll();

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Post("/update", string.Format(@"<optimize waitSearcher=""true"" waitFlush=""true"" />")))
                                     .Return(EmptySolrResponse))
                .Verify(Solr.Optimize);
        }

        [Test]
        public void OptimizeWithParams() {
            mocks.BackToRecordAll();

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Post("/update", string.Format(@"<optimize waitSearcher=""false"" waitFlush=""false"" />")))
                                     .Return(EmptySolrResponse))
                .Verify(() => Solr.Optimize(false, false));
        }

        public int DefaultRows() {
            return SolrQueryExecuter<TestDocument>.ConstDefaultRows;
        }

        [Test]
        public void OrderBy() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "(Id:123456)"},
                {"sort", "Id asc"},
                {"rows", DefaultRows().ToString()},
            };

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .FilterBy(f => f.Id).Is(123456)
                                  .OrderBy(f => f.Id, Order.ASC)
                                  .Run()
                );

        }

        [Test]
        public void OrderBy2Multiple() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"rows", DefaultRows().ToString()},
                {"sort", "Id asc,name desc"},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .OrderBy(f => f.Id)
                                  .OrderBy(f => f.Name, Order.DESC)
                                  .Run()
                );
        }

        [Test]
        public void QueryByRange() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "Id:[123 TO 456]"},
                {"rows", DefaultRows().ToString()},
            };

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .ByRange(f => f.Id, 123, 456)
                                  .Run()
                );
        }

        [Test]
        public void QueryByRangeAnotherSyntax() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "Id:[123 TO 456]"},
                {"rows", DefaultRows().ToString()},
            };

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .By(f => f.Id).Between(123).And(456)
                                  .Run()
                );
        }

        [Test]
        public void QueryByRangeConcatenable() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "Id:[123 TO 456]"},
                {"fq", "name:[a TO z]"},
                {"rows", DefaultRows().ToString()},
            };

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .ByRange(f => f.Id, 123, 456)
                                  .ByRange(f => f.Name, "a", "z")
                                  .Run()
                );
        }

        [Test]
        public void QueryByRangeExclusive() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "Id:{123 TO 456}"},
                {"rows", DefaultRows().ToString()},
            };

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .ByRange(f => f.Id, 123, 456).Exclusive()
                                  .Run()
                );
        }

        [Test]
        public void QueryByRangeInclusive() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "Id:[123 TO 456]"},
                {"rows", DefaultRows().ToString()},
            };

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .ByRange(f => f.Id, 123, 456).Inclusive()
                                  .Run()
                );
        }



        [Test]
        public void QueryISolrQueryWithPagination() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "(Id:123)"},
                {"start", 10.ToString()},
                {"rows", 20.ToString()},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .By(f => f.Id).Is(123)
                                  .Run(10, 20)
                );
        }



        [Test]
        public void FacetField() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"facet", "true"},
                {"facet.field", "ModelDesc"},
                {"fq", "(MakeDesc:bmw)"},
                {"q", "*:*"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .By(f => f.MakeDesc).Is("bmw")
                                  .WithFacetField(f => f.ModelDesc)
                                  .Run(0, 10)
                );
        }

        [Test]
        public void FacetFieldOptions() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"f.ModelDesc.facet.limit", "100"},
                {"f.ModelDesc.facet.mincount", "10"},
                {"f.ModelDesc.facet.missing", "true"},
                {"f.ModelDesc.facet.offset", "20"},
                {"f.ModelDesc.facet.prefix", "xx"},
                {"f.ModelDesc.facet.sort", "false"},
                {"facet", "true"},
                {"facet.field", "ModelDesc"},
                {"fq", "(MakeDesc:bmw)"},
                {"q", "*:*"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .By(f => f.MakeDesc).Is("bmw")
                                  .WithFacetField(f => f.ModelDesc)
                                  .LimitTo(100)
                                  .DontSortByCount()
                                  .WithPrefix("xx")
                                  .WithMinCount(10)
                                  .StartingAt(20)
                                  .IncludeMissing()
                                  .Run(0, 10)
                );
        }

        [Test]
        public void FacetFieldWithTag()
        {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"facet", "true"},
                {"facet.field", "{!ex=tMake}ModelDesc"},
                {"fq", "{!tag=tMake}(MakeDesc:bmw)"},
                {"q", "*:*"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
            };

            //System.Diagnostics.Debugger.Launch();

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .FilterBy(f => f.MakeDesc).Tag("tMake").Is("bmw")
                                  .WithFacetField(f => f.ModelDesc).ExcludeTag("tMake")
                                  .Run(0, 10)
                );
        }

        [Test]
        public void FacetFieldWithTag2() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"facet", "true"},
                {"facet.field", "{!ex=tMake}ModelDesc"},
                {"facet.field", "{!ex=tCat}Category"},
                {"facet.query","test:[* TO *]"},
                {"fq", "{!tag=tMake}(MakeDesc:bmw)"},
                {"fq", "{!tag=tCat}(Category:car)"},
                {"q", "*:*"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
            };

            //System.Diagnostics.Debugger.Launch();

            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .FilterBy(f => f.MakeDesc).Tag("tMake").Is("bmw")
                                  .FilterBy(f => f.Category).Tag("tCat").Is("car")
                                  .WithFacetField(f => f.ModelDesc).ExcludeTag("tMake")
                                  .WithFacetField(f => f.Category).ExcludeTag("tCat")
                                  .WithFacetQuery(new SolrHasValueQuery("test"))
                                  .Run(0, 10)
                );
        }

        [Test]
        public void HighlightingFields() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "(MakeDesc:bmw)"},
                {"hl", "true"},
                {"hl.fl", "MakeDesc,Category"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .By(f => f.MakeDesc).Is("bmw")
                                  .WithHighlightingFields(f => f.MakeDesc, f => f.Category)
                                  .Run(0, 10)
                );

        }

        [Test]
        public void ExtraParam() {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"abc", "123"},
                {"abc", "456"},
                {"xyz", "0"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
                {"d", "v"},
                {"d", "v"},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .AddExtraParams(new KeyValuePairList {
                                      {"abc", "123"},
                                      {"abc", "456"},
                                      {"xyz", "0"},
                                      {"d", "v"},
                                      {"d", "v"},
                                  })
                                  .Run(0, 10)
                );
        }


        [Test]
        public void SpellCheck()
        {
            mocks.BackToRecordAll();
            var solrGetParameters = new KeyValuePairList {
                {"q", "*:*"},
                {"fq", "(MakeDesc:bmw)"},
                {"spellcheck", "true"},
                {"spellcheck.q", "bmw"},
                {"spellcheck.count", "10"},
                {"spellcheck.onlyMorePopular", "true"},
                {"rows", 10.ToString()},
                {"start", 0.ToString()},
            };
            With.Mocks(mocks)
                .Expecting(() => Expect
                                     .Call(conn.Get("/select", solrGetParameters)).IgnoreArguments()
                                     .Callback(new SolrGetTester("/select", solrGetParameters).ValidateSolrGet)
                                     .Return(EmptySolrResponse))

                .Verify(() => Solr
                                  .Query()
                                  .By(f => f.MakeDesc).Is("bmw")
                                  .WithSpellCheck("bmw")
                                  .SetCount(10)
                                  .SetOnlyMorePopular(true)
                                  .Run(0, 10)
                );

        }
    }
}