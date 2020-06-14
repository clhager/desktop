using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParams<Input, ExpectedOutput>
{
    public Input input;
    public ExpectedOutput expectedOutput;
    public string name;

    public TestParams(Input input, ExpectedOutput expectedOutput, string name)
    {
        this.input = input;
        this.expectedOutput = expectedOutput;
        this.name = name;
    }

    public TestParams(Input input, string name)
    {
        this.input = input;
        this.name = name;
    }
}
