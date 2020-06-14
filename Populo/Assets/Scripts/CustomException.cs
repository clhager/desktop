using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomException
{
    public class DuplicateItemException : UnityException
    {
        public DuplicateItemException(string item) : base($"Duplicate item {item}: cannot create multiple items for singleton classes") { }

    }
}
