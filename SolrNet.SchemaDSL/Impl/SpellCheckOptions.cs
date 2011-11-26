using SolrNet.Commands.Parameters;

namespace SolrNet.SchemaDSL.Impl {
    internal class SpellCheckOptions<T> : DSLRun<T>, ISpellCheckOptions<T> where T : ISchema {
        private readonly SpellCheckingParameters spellCheckingParameters;

        internal SpellCheckOptions(DSLRun<T> parentDslRun, string query)
            : this(parentDslRun, new SpellCheckingParameters {Query = query}) {}

        internal SpellCheckOptions(DSLRun<T> parentDslRun, SpellCheckingParameters spellCheckingParameters)
            : base(parentDslRun, spellCheckingParameters) {
            this.spellCheckingParameters = spellCheckingParameters;
        }

        public ISpellCheckOptions<T> SetDictionary(string dictionary) {
            spellCheckingParameters.Dictionary = dictionary;

            return new SpellCheckOptions<T>(this, spellCheckingParameters);
        }

        public ISpellCheckOptions<T> SetCount(int maxSuggestionCount) {
            spellCheckingParameters.Count = maxSuggestionCount;

            return new SpellCheckOptions<T>(this, spellCheckingParameters);
        }

        public ISpellCheckOptions<T> SetOnlyMorePopular(bool onlyMorePopular) {
            spellCheckingParameters.OnlyMorePopular = onlyMorePopular;

            return new SpellCheckOptions<T>(this, spellCheckingParameters);
        }
    }
}