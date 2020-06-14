using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestReadPrimitives
    {
        [Test]
        public void ReadColor_ValidInput_ReturnsColor()
        {
            Assert.AreEqual(new Color32(0, 1, 2, 3), ReadPrimitives.ReadColor(TestUtils.MakeFileTokenIterator("0, 1, 2, 3")));
        }

        static TestParams<string[], Unit>[] ReadColor_InvalidInputs = new TestParams<string[], Unit>[] {
            new TestParams<string[], Unit>(new string[]{ "" }, "missing rgba"),
            new TestParams<string[], Unit>(new string[]{ "1" }, "missing gba and comma"),
            new TestParams<string[], Unit>(new string[]{ "1," }, "missing gba"),
            new TestParams<string[], Unit>(new string[]{ "1,2" }, "missing ba and comma"),
            new TestParams<string[], Unit>(new string[]{ "1,2," }, "missing ba"),
            new TestParams<string[], Unit>(new string[]{ "1,2,3" }, "missing a and comma"),
            new TestParams<string[], Unit>(new string[]{ "1,2,3," }, "missing a")
        };
        [Test]
        public void ReadColor_InvalidInput_Throws([ValueSource("ReadColor_InvalidInputs")] TestParams<string[], Unit> testParams)
        {
            try
            {
                ReadPrimitives.ReadColor(TestUtils.MakeFileTokenIterator(testParams.input));
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }
          

        [Test]
        public void ReadCoordinates_ValidInput_ReturnsCoordinate()
        {
            Assert.AreEqual(new Vector2Int(0, 1), ReadPrimitives.ReadCoordinates(TestUtils.MakeFileTokenIterator("0, 1")));
        }

        static TestParams<string[], Unit>[] ReadCoordinates_InvalidInputs = new TestParams<string[], Unit>[] {
            new TestParams<string[], Unit>(new string[]{ "" }, "missing x and y"),
            new TestParams<string[], Unit>(new string[]{ "1" }, "missing y and comma"),
            new TestParams<string[], Unit>(new string[]{ "1," }, "missing y")
        };
        [Test]
        public void ReadCoordinates_InvalidInput_Throws([ValueSource("ReadCoordinates_InvalidInputs")] TestParams<string[], Unit> testParams)
        {
            try
            {
                ReadPrimitives.ReadCoordinates(TestUtils.MakeFileTokenIterator(testParams.input));
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }


        [Test]
        public void ReadFloat_ValidInput_ReturnsFloat()
        {
            Assert.AreEqual(12.34f, ReadPrimitives.ReadFloat(TestUtils.MakeFileTokenIterator("12.34")));
        }

        static TestParams<string[], Unit>[] ReadFloat_InvalidInputs = new TestParams<string[], Unit>[] {
            new TestParams<string[], Unit>(new string[]{ ")" }, "not a float - special character"),
            new TestParams<string[], Unit>(new string[]{ ")" }, "not a float - text"),
        };
        [Test]
        public void ReadFloat_InvalidInput_Throws([ValueSource("ReadFloat_InvalidInputs")] TestParams<string[], Unit> testParams)
        {
            try
            {
                ReadPrimitives.ReadFloat(TestUtils.MakeFileTokenIterator(testParams.input));
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }


        [Test]
        public void ReadInt_ValidInput_ReturnsInt()
        {
            Assert.AreEqual(1234, ReadPrimitives.ReadInt(TestUtils.MakeFileTokenIterator("1234")));
        }

        static TestParams<string[], Unit>[] ReadInt_InvalidInputs = new TestParams<string[], Unit>[] {
            new TestParams<string[], Unit>(new string[]{ ")" }, "not an int - special character"),
            new TestParams<string[], Unit>(new string[]{ ")" }, "not an int - text"),
        };
        [Test]
        public void ReadInt_InvalidInput_Throws([ValueSource("ReadInt_InvalidInputs")] TestParams<string[], Unit> testParams)
        {
            try
            {
                ReadPrimitives.ReadInt(TestUtils.MakeFileTokenIterator(testParams.input));
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }


        static TestParams<string[], string>[] ReadString_ValidInputs = new TestParams<string[], string>[]
        {
            new TestParams<string[], string>(
                new string[]{ "\"Hello World\"" }, 
                "Hello World",
                "single string"
            ),
            new TestParams<string[], string>(
                new string[]{ "\"Hello\"", ",", "\" World\"" }, 
                "Hello World",
                "2-part string"
            ),
            new TestParams<string[], string>(
                new string[]{ "\"Hell\"", ",", "\"o Wo\"", ",", "\"rld\"" }, 
                "Hello World",
                "3-part string"
            ),
        };
        [Test]
        public void ReadString_ValidInput_ReturnsString([ValueSource("ReadString_ValidInputs")] TestParams<string[], string> testParams)
        {
            Assert.AreEqual(
                testParams.expectedOutput, 
                ReadPrimitives.ReadString(TestUtils.MakeFileTokenIterator(testParams.input)),
                testParams.name
            );
        }

        static TestParams<string[], string>[] ReadString_EmptyInputs = new TestParams<string[], string>[]
        {
            new TestParams<string[], string>(
                new string[]{ },
                "",
                "no inputs"
            ),
            new TestParams<string[], string>(
                new string[]{ "" },
                "",
                "single empty input"
            ),
            new TestParams<string[], string>(
                new string[]{ "\"\"" },
                "",
                "single empty string"
            ),
            new TestParams<string[], string>(
                new string[]{ "\"\"", ",", "\"\"" },
                "",
                "two empty strings"
            )
        };
        [Test]
        public void ReadString_EmptyInput_ReturnsString([ValueSource("ReadString_EmptyInputs")] TestParams<string[], string> testParams)
        {
            Assert.AreEqual(
                testParams.expectedOutput,
                ReadPrimitives.ReadString(TestUtils.MakeFileTokenIterator(testParams.input)),
                testParams.name
            );
        }

        static TestParams<string[], Unit>[] ReadString_InvalidInputs = new TestParams<string[], Unit>[]
        {
            new TestParams<string[], Unit>(
                new string[]{ "Hello World" },
                "missing quotes"
            ),
            new TestParams<string[], Unit>(
                new string[]{ "\"Hello World" },
                "missing close quote"
            ),
            new TestParams<string[], Unit>(
                new string[]{ "Hello World\"" },
                "missing open quote"
            ),
            new TestParams<string[], Unit>(
                new string[]{ "\"correct\"", "incorrect" },
                "first correct, second incorrect"
            )
        };
        [Test]
        public void ReadString_InvalidInput_Throws([ValueSource("ReadString_InvalidInputs")] TestParams<string[], Unit> testParams)
        {
            try
            {
                ReadPrimitives.ReadString(TestUtils.MakeFileTokenIterator(testParams.input));
                Assert.Fail(testParams.name);
            }
            catch (UnityException)
            {
                Assert.Pass();
            }
        }
    }
}
