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

#pragma warning disable 1591

namespace SolrNet.SchemaDSL.Impl
{
    public interface IDSLFilterBy<TSchema, TField> where TSchema : ISchema
    {
        IDSLQuery<TSchema> Is(TField value);
        IDSLQuery<TSchema> In(IEnumerable<TField> values);
        IDSLQuery<TSchema> In<TStruct>(IEnumerable<TStruct> values) where TStruct : struct;
        IDSLQuery<TSchema> Between(Interval<TField> interval);
        IDSLQuery<TSchema> Between(TField start, TField end, bool minInclusive, bool maxInclusive);
        IDSLQuery<TSchema> BetweenEx(Interval<TField> interval);
        IDSLQuery<TSchema> BetweenEx(TField start, TField end, bool minInclusive, bool maxInclusive);
        IDSLQuery<TSchema> HasValue();
        IDSLFilterBy<TSchema, TField> Negate();

        IDSLFilterBy<TSchema, TField> Tag(string tagName);
    }
}
#pragma warning restore 1591
