using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestReadObjects
    {
        [Test]
        public void ReadObject_ValidInput_ReturnsDictionary()
        {
            string objectVariableName = "objectVariable";
            string objectVariableType = "TestObject";
            string[] objectVariable = new string[] { "random object content" };

            string listVariableName = "listVariable";
            string listVariableType = "listType";
            string[] listVariable = new string[] { "random list content" };

            string primitiveVariableName = "primitiveVariable";
            string primitiveVariableType = "primitiveType";
            string primitiveVariable = "random primitive content";

            Dictionary<string, int> baseLineOffsets = new Dictionary<string, int>();
            string[] objectString = ComposeObjectString();

            FileTokenIterator tokenIterator = TestUtils.MakeFileTokenIterator(objectString);
            Dictionary<string, ReadObjects.VariableTokenIterator> objectMap = ReadObjects.ReadObject("testObject", tokenIterator);

            AssertVariableNamesAreCorrect();
            AssertVariableTypesAreCorrect();
            AssertVariableValuesAreCorrect();

            return;

            string[] ComposeObjectString()
            {
                List<string> lines = new List<string>();
                lines.Add($"{objectVariableName} = {objectVariableType} {{");
                baseLineOffsets.Add(objectVariableName, lines.Count);
                lines.AddRange(objectVariable);
                lines.Add("},");

                lines.Add($"{listVariableName} = {listVariableType} [");
                baseLineOffsets.Add(listVariableName, lines.Count);
                lines.AddRange(listVariable);
                lines.Add("],");

                lines.Add($"{primitiveVariableName} = {primitiveVariableType}(");
                baseLineOffsets.Add(primitiveVariableName, lines.Count);
                lines.Add($"{primitiveVariable}");
                lines.Add("),");

                return lines.ToArray();
            }

            void AssertVariableNamesAreCorrect()
            {
                TestUtils.AssertSetsAreEqual(new HashSet<string>(objectMap.Keys), new HashSet<string> { objectVariableName, listVariableName, primitiveVariableName });
            }

            void AssertVariableTypesAreCorrect()
            {
                Assert.AreEqual(objectVariableType, objectMap[objectVariableName].type);
                Assert.AreEqual(listVariableType, objectMap[listVariableName].type);
                Assert.AreEqual(primitiveVariableType, objectMap[primitiveVariableName].type);
            }

            void AssertVariableValuesAreCorrect()
            {
                TestUtils.AssertTokenIteratorsAreEqual(
                    TestUtils.MakeFileTokenIterator(objectVariable), 
                    baseLineOffsets[objectVariableName], 
                    objectMap[objectVariableName].tokenIterator
                );

                TestUtils.AssertTokenIteratorsAreEqual(
                    TestUtils.MakeFileTokenIterator(listVariable),
                    baseLineOffsets[listVariableName],
                    objectMap[listVariableName].tokenIterator
                );

                TestUtils.AssertTokenIteratorsAreEqual(
                    TestUtils.MakeFileTokenIterator(primitiveVariable), 
                    baseLineOffsets[primitiveVariableName], 
                    objectMap[primitiveVariableName].tokenIterator
                );
            }
        }

        [Test]
        public void ReadObject_InvalidInput_Throws()
        {
            try
            {
                ReadObjects.ReadObject("testObject", TestUtils.MakeFileTokenIterator("variable = NotObjectListOrPrimitive"));
                Assert.Fail();
            }
            catch(UnityException)
            {
                Assert.Pass();
            }
        }
    }
}
