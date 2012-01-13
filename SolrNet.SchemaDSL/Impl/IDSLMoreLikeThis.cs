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
using SolrNet.Commands.Parameters;

#pragma warning disable 1591

namespace SolrNet.SchemaDSL.Impl {
    public interface IDSLMoreLikeThis<TSchema> : IDSLRun<TSchema> where TSchema : ISchema {
        IDSLMoreLikeThis<TSchema> WithMinTermFreq(int minTermFreq);
        IDSLMoreLikeThis<TSchema> WithMinDocFreq(int minDocFreq);
        IDSLMoreLikeThis<TSchema> WithCount(int mltCount);
    }

    internal class DSLMoreLikeThisOptions<TSchema> : DSLRun<TSchema>, IDSLMoreLikeThis<TSchema> where TSchema : ISchema {
        internal DSLMoreLikeThisOptions(DSLRun<TSchema> parentDslRun, MoreLikeThisParameters mltParameters)
            : base(parentDslRun, mltParameters) {}

        public IDSLMoreLikeThis<TSchema> WithMinTermFreq(int minTermFreq) {
            MoreLikeThisParams.MinTermFreq = minTermFreq;

            return this;
        }

        public IDSLMoreLikeThis<TSchema> WithMinDocFreq(int minDocFreq) {
            MoreLikeThisParams.MinDocFreq = minDocFreq;

            return this;
        }

        public IDSLMoreLikeThis<TSchema> WithCount(int mltCount) {
            MoreLikeThisParams.Count = mltCount;

            return this;
        }
    }
}

#pragma warning restore 1591
