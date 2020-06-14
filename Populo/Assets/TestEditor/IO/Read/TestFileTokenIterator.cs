using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestFileTokenIterator
    {
        [Test]
        public void Current_FirstToken_ReturnsNull()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("");
            Assert.Null(tokenIterator.Current());
        }

        [Test]
        public void Current_IterationFinished_ReturnsNull()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Item1");
            Assert.AreEqual(tokenIterator.Next(), "Item1");
            Assert.Null(tokenIterator.Next());
            Assert.Null(tokenIterator.Current());
        }

        [Test]
        public void Current_ValidToken_ReturnsToken()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Item1");
            Assert.AreEqual(tokenIterator.Next(), "Item1");
            Assert.AreEqual(tokenIterator.Current(), "Item1");
        }

        [Test]
        public void HasNext_ContainsNext_ReturnsTrue()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Line 1");
            Assert.True(tokenIterator.HasNext());
        }

        [Test]
        public void HasNext_DoesNotContainNext_ReturnsFalse()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("");
            Assert.False(tokenIterator.HasNext());
        }

        [Test]
        public void PeekNext_ValidToken_ReturnsToken()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Item1", "Item2");
            Assert.AreEqual(tokenIterator.PeekNext(), "Item1");
            Assert.AreEqual(tokenIterator.Next(), "Item1");
            Assert.AreEqual(tokenIterator.PeekNext(), "Item2");
            Assert.AreEqual(tokenIterator.Next(), "Item2");
        }

        [Test]
        public void PeekNext_IterationFinished_ReturnsNull()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Item1");
            Assert.AreEqual(tokenIterator.PeekNext(), "Item1");
            Assert.AreEqual(tokenIterator.Next(), "Item1");
            Assert.Null(tokenIterator.PeekNext());
            Assert.Null(tokenIterator.Next());
        }

        [Test]
        public void Next_ContainsTokens_ReturnsAllTokens()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Item1, Item2", "Item3");
            Assert.AreEqual(tokenIterator.Next(), "Item1");
            Assert.AreEqual(tokenIterator.Next(), ",");
            Assert.AreEqual(tokenIterator.Next(), "Item2");
            Assert.AreEqual(tokenIterator.Next(), "Item3");
        }

        [Test]
        public void Next_ContainsNoTokens_ReturnsNull()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("");
            Assert.Null(tokenIterator.Next());
        }

        [Test]
        public void GetCurrentLine_HasLines_ReturnsCurrentLine()
        {
            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator("Line1", "Line2", "Line3");
            tokenIterator.Next();
            Assert.AreEqual(tokenIterator.GetCurrentLine(), 0);
            tokenIterator.Next();
            Assert.AreEqual(tokenIterator.GetCurrentLine(), 1);
            tokenIterator.Next();
            Assert.AreEqual(tokenIterator.GetCurrentLine(), 2);
            tokenIterator.Next();
            Assert.AreEqual(tokenIterator.GetCurrentLine(), 3);
            tokenIterator.Next();
            Assert.AreEqual(tokenIterator.GetCurrentLine(), 3);
        }


        [Test]
        public void MakeFileTokenIteratorForCurrentObject_ValidInput_ReturnsIterator()
        {
            string[] objectString = new string[] { "random object content" };
            string[] afterObjectString = new string[] { ",", "nextVariable" };
            string[] inputString = objectString.Concatenate(new string[] { "}" }).Concatenate(afterObjectString);

            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator(inputString);
            FileTokenIterator comparisonIterator = TestUtils.MakeFileTokenIterator(objectString);
            FileTokenIterator objectTokenIterator = FileTokenIterator.MakeFileTokenIteratorForCurrentObject(tokenIterator, "objectName", "objectType");

            TestUtils.AssertTokenIteratorsAreEqual(comparisonIterator, 0, objectTokenIterator);
            AssertAfterObjectStringIsCorrect();

            return;

            void AssertAfterObjectStringIsCorrect()
            {
                foreach (string afterObjectToken in afterObjectString)
                {
                    Assert.AreEqual(afterObjectToken, tokenIterator.Next());
                }
            }
        }

        [Test]
        public void MakeFileTokenIteratorForCurrentObject_InvalidInput_Throws()
        {
            string[] inputString = new string[] { "random object content but no close" };
            try
            {
                FileTokenIterator.MakeFileTokenIteratorForCurrentObject(TestUtils.MakeFileTokenIterator(inputString), "objectName", "objectType");
                Assert.Fail();
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }


        [Test]
        public void MakeFileTokenIteratorForCurrentList_ValidInput_ReturnsIterator()
        {
            string[] listString = new string[] { "random list content" };
            string[] afterListString = new string[] { ",", "nextVariable" };
            string[] inputString = listString.Concatenate(new string[] { "]" }).Concatenate(afterListString);

            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator(inputString);
            FileTokenIterator comparisonIterator = TestUtils.MakeFileTokenIterator(listString);
            FileTokenIterator listTokenIterator = FileTokenIterator.MakeFileTokenIteratorForCurrentList(tokenIterator, "listName", "listType");

            TestUtils.AssertTokenIteratorsAreEqual(comparisonIterator, 0, listTokenIterator);
            AssertAfterListStringIsCorrect();

            return;

            void AssertAfterListStringIsCorrect()
            {
                foreach (string afterListToken in afterListString)
                {
                    Assert.AreEqual(afterListToken, tokenIterator.Next());
                }
            }
        }

        [Test]
        public void MakeFileTokenIteratorForCurrentList_InvalidInput_Throws()
        {
            string[] inputString = new string[] { "random list content but no close" };
            try
            {
                FileTokenIterator.MakeFileTokenIteratorForCurrentList(TestUtils.MakeFileTokenIterator(inputString), "listName", "listType");
                Assert.Fail();
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }


        [Test]
        public void MakeFileTokenIteratorForCurrentPrimitive_ValidInput_ReturnsIterator()
        {
            string[] primitiveString = new string[] { "random primitive content" };
            string[] afterPrimitiveString = new string[] { ",", "nextVariable" };
            string[] inputString = primitiveString.Concatenate(new string[] { ")" }).Concatenate(afterPrimitiveString);

            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator(inputString);
            FileTokenIterator comparisonIterator = TestUtils.MakeFileTokenIterator(primitiveString);
            FileTokenIterator primitiveTokenIterator = FileTokenIterator.MakeFileTokenIteratorForCurrentPrimitive(tokenIterator, "primitiveName", "primitiveType");

            TestUtils.AssertTokenIteratorsAreEqual(comparisonIterator, 0, primitiveTokenIterator);
            AssertAfterPrimitiveStringIsCorrect();

            return;

            void AssertAfterPrimitiveStringIsCorrect()
            {
                foreach (string afterPrimitiveToken in afterPrimitiveString)
                {
                    Assert.AreEqual(afterPrimitiveToken, tokenIterator.Next());
                }
            }
        }

        [Test]
        public void MakeFileTokenIteratorForCurrentPrimitive_InvalidInput_Throws()
        {
            string[] inputString = new string[] { "random primitive content but no close" };
            try
            {
                FileTokenIterator.MakeFileTokenIteratorForCurrentPrimitive(TestUtils.MakeFileTokenIterator(inputString), "primitiveName", "primitiveType");
                Assert.Fail();
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }
    }
}
