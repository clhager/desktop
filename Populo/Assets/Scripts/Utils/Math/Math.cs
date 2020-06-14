using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math
{
    public static int Mod(int number, int modulo)
    {
        return (number + modulo) % modulo;
    }
}
