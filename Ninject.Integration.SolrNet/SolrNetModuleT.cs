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

using System;
using Ninject.Modules;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;

namespace Ninject.Integration.SolrNet {
    /// <summary>
    /// Ninject Integration module for SolrNet with Multi-Core support
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrNetModule<T> : NinjectModule {
        private readonly string serverUrl;
        
        /// <summary>
        /// SolrNet Mapper - default: <see cref="MemoizingMappingManager"/>
        /// </summary>
        public IReadOnlyMappingManager Mapper { get; set; }

        /// <summary>
        /// Constructor for Ninject Integration module for SolrNet with Multi-Core support
        /// </summary>
        /// <param name="serverUrl"></param>
        public SolrNetModule(string serverUrl) {
            this.serverUrl = serverUrl;
        }

        public override void Load() {

            if (!Kernel.HasModule(SoleNetModuleCommon.ModuleName)) {
                Kernel.Load<SoleNetModuleCommon>();
            }

            var mapper = Mapper ?? new MemoizingMappingManager(new AttributesMappingManager());
            Bind<IReadOnlyMappingManager>().ToConstant(mapper).When(IsInjectedIntoGenericT);

            Bind<ISolrDocumentActivator<T>>().To<SolrDocumentActivator<T>>();
            Bind<ISolrDocumentResponseParser<T>>().To<SolrDocumentResponseParser<T>>();
            //Bind<<T>>().To<<T>>();

            Bind<ISolrResponseParser<T>>().To<ResultsResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<HeaderResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<FacetsResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<HighlightingResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<MoreLikeThisResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<SpellCheckResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<StatsResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<CollapseResponseParser<T>>();
            Bind<ISolrResponseParser<T>>().To<GroupingResponseParser<T>>();


            Bind<ISolrConnection>().ToConstant(new SolrConnection(serverUrl)).When(IsInjectedIntoGenericT);

            Bind<ISolrQueryResultParser<T>>().To<SolrQueryResultParser<T>>();
            Bind<ISolrQueryExecuter<T>>().To<SolrQueryExecuter<T>>();
            Bind<ISolrDocumentSerializer<T>>().To<SolrDocumentSerializer<T>>();
            Bind<ISolrBasicOperations<T>>().To<SolrBasicServer<T>>();
            Bind<ISolrBasicReadOnlyOperations<T>>().To<SolrBasicServer<T>>();
            Bind<ISolrOperations<T>>().To<SolrServer<T>>();
            Bind<ISolrReadOnlyOperations<T>>().To<SolrServer<T>>();
        }

        // ReSharper disable StaticFieldInGenericType
        private static readonly Type MyType = typeof (T);
        // ReSharper restore StaticFieldInGenericType

        private static bool IsInjectedIntoGenericT(Activation.IRequest request) {

            var parentRequest = request.ParentRequest;
            while (parentRequest != null) {

                var genericArgs = parentRequest.Service.GetGenericArguments();
                if (genericArgs.Length == 1 && genericArgs[0] == MyType) {
                    return true;
                }
                parentRequest = parentRequest.ParentRequest;
            }

            return false;
        }
    }


    class SoleNetModuleCommon : NinjectModule
    {

        public static readonly string ModuleName = typeof(SoleNetModuleCommon).AssemblyQualifiedName;

        public override string Name
        {
            get { return ModuleName; }
        }

        public override void Load()
        {
            Bind<ISolrDocumentPropertyVisitor>().To<DefaultDocumentVisitor>();
            Bind<ISolrFieldParser>().To<DefaultFieldParser>();
            Bind<ISolrFieldSerializer>().To<DefaultFieldSerializer>();
            Bind<ISolrQuerySerializer>().To<DefaultQuerySerializer>();
            Bind<ISolrFacetQuerySerializer>().To<DefaultFacetQuerySerializer>();
            Bind<ISolrHeaderResponseParser>().To<HeaderResponseParser<string>>();
            Bind<ISolrExtractResponseParser>().To<ExtractResponseParser>();

            Bind<IValidationRule>().To<MappedPropertiesIsInSolrSchemaRule>();
            Bind<IValidationRule>().To<RequiredFieldsAreMappedRule>();
            Bind<IValidationRule>().To<UniqueKeyMatchesMappingRule>();

            Bind<ISolrSchemaParser>().To<SolrSchemaParser>();
            Bind<ISolrDIHStatusParser>().To<SolrDIHStatusParser>();
            Bind<IMappingValidator>().To<MappingValidator>();
        }
    }
}