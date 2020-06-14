using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestReadLists
    {
        [Test]
        public void ReadFloatList_ValidInput_ReturnsFileTokenIterator()
        {
            TestUtils.AssertListsAreEqual(
                new List<float> { 1.0f, 2.1f, 3.2f },
                ReadLists.ReadFloatList("test", TestUtils.MakeFileTokenIterator("(1.0), (2.1), (3.2)"))
            );
        }

        static TestParams<string[], List<int>>[] ReadIntList_VariousValidInputs = new TestParams<string[], List<int>>[] {
            new TestParams<string[], List<int>>(new string[]{ "(1), (2), (3)" }, new List<int> { 1, 2, 3 }, "one line list, no trailing comma"),
            new TestParams<string[], List<int>>(new string[]{ "(1), (2), (3)," }, new List<int> { 1, 2, 3 }, "one line list, trailing comma"),
            new TestParams<string[], List<int>>(new string[]{ "(1),", "(2),", "(3)" }, new List<int> { 1, 2, 3 }, "multi line list, no trailing comma"),
            new TestParams<string[], List<int>>(new string[]{ "(1),", "(2),", "(3)," }, new List<int> { 1, 2, 3 }, "multi line list, no trailing comma")
        };
        [Test]
        public void ReadIntList_VariousValidInputs_ReturnsFileTokenIterator([ValueSource("ReadIntList_VariousValidInputs")] TestParams<string[], List<int>> testParams)
        {
            TestUtils.AssertListsAreEqual(
                testParams.expectedOutput,
                ReadLists.ReadIntList("test", TestUtils.MakeFileTokenIterator(testParams.input)),
                testParams.name
            );
        }

        static TestParams<string[], Unit>[] ReadIntList_VariousInvalidInputs = new TestParams<string[], Unit>[] {
            new TestParams<string[], Unit>(new string[]{ "=" }, "invalid character '='"),
            new TestParams<string[], Unit>(new string[]{ "[" }, "invalid character '['"),
            new TestParams<string[], Unit>(new string[]{ "}" }, "invalid character '}'"),
            new TestParams<string[], Unit>(new string[]{ ")" }, "invalid character ')'"),
            new TestParams<string[], Unit>(new string[]{ "," }, "list with only a comma"),
            new TestParams<string[], Unit>(new string[]{ "(1),," }, "double comma"),
            new TestParams<string[], Unit>(new string[]{ "(1) (2) (3)" }, "missing commas to separate elements"),
            new TestParams<string[], Unit>(new string[]{ "1, 2, 3" }, "incorrectly formatted elements"),
        };
        [Test]
        public void ReadIntList_VariousInvalidInputs_Throws([ValueSource("ReadIntList_VariousInvalidInputs")] TestParams<string[], Unit> testParams)
        {
            try
            {
                ReadLists.ReadIntList("test", TestUtils.MakeFileTokenIterator(testParams.input));
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void ReadStringList_ValidInput_ReturnsFileTokenIterator()
        {
            TestUtils.AssertListsAreEqual(
                new List<string> { "I'm", "a", "test" },
                ReadLists.ReadStringList("test", TestUtils.MakeFileTokenIterator("(\"I'm\"), (\"a\"), (\"test\")"))
            );
        }

        
    }
}
