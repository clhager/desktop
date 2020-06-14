using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceModel : Model
{
    public ResourceModel(ReadMap.ObjectMap map) : base(map) { }

    public static void Read(ReadMap.ObjectMap map)
    {
        new ResourceModel(map);
    }
}
