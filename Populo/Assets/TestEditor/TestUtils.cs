using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestUtils
    {
        public static List<string> MakeStringList(params string[] strings)
        {
            return new List<string>(strings);
        }

        public static FileTokenIterator MakeFileTokenIterator(params string[] strings)
        {
            return new FileTokenIterator("test", MakeStringList(strings));
        }

        public static void AssertListsAreEqual<T>(List<T> list1, List<T> list2)
        {
            AssertListsAreEqual(list1, list2, "");
        }

        public static void AssertListsAreEqual<T>(List<T> list1, List<T> list2, string message)
        {
            if (list1.Count != list2.Count) Assert.Fail();
            for (int i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i], list2[i], message);
            }
        }

        public static void AssertSetsAreEqual<T>(HashSet<T> set1, HashSet<T> set2)
        {
            AssertSetsAreEqual(set1, set2, "");
        }

        public static void AssertSetsAreEqual<T>(HashSet<T> set1, HashSet<T> set2, string message)
        {
            foreach (T item in set1)
            {
                Assert.True(set2.Contains(item));
            }
        }

        public static void AssertTokenIteratorsAreEqual(FileTokenIterator tokenIterator1, int baseLineOffset, FileTokenIterator tokenIterator2)
        {
            AssertTokenIteratorsAreEqual(tokenIterator1, baseLineOffset, tokenIterator2, "");
        }

        public static void AssertTokenIteratorsAreEqual(FileTokenIterator tokenIterator1, int baseLineOffset, FileTokenIterator tokenIterator2, string message)
        {
            while (tokenIterator1.HasNext())
            {
                Assert.AreEqual(tokenIterator1.Next(), tokenIterator2.Next(), message);
                Assert.AreEqual(tokenIterator1.GetCurrentLine() + baseLineOffset, tokenIterator2.GetCurrentLine(), message);
            }

            Assert.Null(tokenIterator1.Next(), message);
            Assert.Null(tokenIterator2.Next(), message);
        }

        [Test]
        public void Concatenate_GivenTwoArrays_ReturnsConcatenatedArray()
        {
            AssertListsAreEqual(
                new List<int> { 1, 2, 3, 4 }, 
                new List<int>(new int[] { 1, 2 }.Concatenate(new int[] { 3, 4 }))
            );
        }
    }
}
