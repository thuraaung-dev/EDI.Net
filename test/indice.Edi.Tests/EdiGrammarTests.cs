﻿using Xunit;

namespace indice.Edi.Tests
{
    public class EdiGrammarTests
    {
        [Fact]
        [Trait(Traits.Tag, "Grammar")]
        public void EdiGrammarSetAdvice_ChangesSpecialCharacters() {

            var grammar = EdiGrammar.NewEdiFact();
            Assert.True(grammar.IsSpecial('\''));
            grammar.SetAdvice(new[] { ':', '+', '.', '?', ' ', '\r' });
            Assert.True(grammar.IsSpecial('\r'));
        }
    }
}
