using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestReadUtils
    {
        static HashSet<char> TokenizeStringDividers = new HashSet<char> { ',' };

        static TestParams<string, List<string>>[] TokenizeString_ValidInputs = new TestParams<string, List<string>>[]
        {
            new TestParams<string, List<string>>(
                "token1, token2, token3", 
                new List<string>{ "token1", ",", "token2", ",", "token3" }, 
                "three tokens"
            ),
            new TestParams<string, List<string>>(
                "\"text, in, a, string\", after", 
                new List<string>{ "\"", "text, in, a, string", "\"", ",", "after" },
                "keep whitespace inside quotes, remove whitespace outside quotes"
            ),
            new TestParams<string, List<string>>(
                "\\backslash", 
                new List<string>{ "backslash" },
                "don't keep the backslash character"
            ),
            new TestParams<string, List<string>>(
                "\"token1\rtoken2\"",
                new List<string>{ "\"", "token1token2", "\"" },
                "remove control characters inside quotes"
            ),
            new TestParams<string, List<string>>(
                "token1\rtoken2",
                new List<string>{ "token1", "token2" },
                "control characters outside quotes separate tokens"
            )
        };
        [Test]
        public void TokenizedString_ValidInputs_ReturnsTokenizedString([ValueSource("TokenizeString_ValidInputs")] TestParams<string, List<string>> testParams)
        {
            TestUtils.AssertListsAreEqual(testParams.expectedOutput, ReadUtils.TokenizeString(testParams.input, TokenizeStringDividers, 0), testParams.name);
        }

        static TestParams<string, Unit>[] TokenizeString_InvalidInputs = new TestParams<string, Unit>[] { 
            new TestParams<string, Unit>("\\", "backslash with no character after")
        };
        [Test]
        public void TokenizeString_InvalidInputs_Throws([ValueSource("TokenizeString_InvalidInputs")] TestParams<string, Unit> testParams)
        {
            try
            {
                ReadUtils.TokenizeString(testParams.input, TokenizeStringDividers, 0);
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }
    }
}
