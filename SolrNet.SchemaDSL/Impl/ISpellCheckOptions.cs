using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.SchemaDSL.Impl {
    /// <summary>
    /// Spell checking parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpellCheckOptions<T> : IDSLRun<T> where T : ISchema {
        /// <summary>
        /// The name of the spellchecker to use. This defaults to "default". 
        /// Can be used to invoke a specific spellchecker on a per request basis.
        /// </summary>
        /// <param name="dictionary">Dictionary name. Defaults to "default".</param>
        /// <returns></returns>
        ISpellCheckOptions<T> SetDictionary(string dictionary);

        /// <summary>
        /// The maximum number of suggestions to return. 
        /// Note that this value also limits the number of candidates considered as suggestions. 
        /// You might need to increase this value to make sure you always get the best suggestion, 
        /// even if you plan to only use the first item in the list.
        /// </summary>
        /// <param name="maxSuggestionCount">The maximum number of suggestions to return.</param>
        /// <returns></returns>
        ISpellCheckOptions<T> SetCount(int maxSuggestionCount);

        /// <summary>
        /// Only return suggestions that result in more hits for the query than the existing query. 
        /// Note that even if the given query term is correct (i.e. present in the index), 
        /// a more popular suggestion will be returned (if one exists).
        /// </summary>
        /// <param name="onlyMorePopular">Only return suggestions that result in more hits for the query than the existing query.</param>
        /// <returns></returns>
        ISpellCheckOptions<T> SetOnlyMorePopular(bool onlyMorePopular);
    }
}
