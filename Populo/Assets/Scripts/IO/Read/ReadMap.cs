using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadMap
{
    public string Type { get; }
    public string FilePath { get; }
    public int Line { get; }

    public ReadMap(string type, string filePath, int line)
    {
        Type = type;
        FilePath = filePath;
        Line = line;
    }
    
    public class ObjectMap : ReadMap
    {
        public Dictionary<string, ObjectMap> Objects { get; }
        public Dictionary<string, ObjectList> ObjectLists { get; }

        public Dictionary<string, PrimitiveMap> Primitives { get; }
        public Dictionary<string, PrimitiveList> PrimitiveLists { get; }

        public ObjectMap(
            Dictionary<string, ObjectMap> objects,
            Dictionary<string, ObjectList> objectLists,
            Dictionary<string, PrimitiveMap> primitives,
            Dictionary<string, PrimitiveList> primitiveLists,
            string type,
            string filePath,
            int line
        ) : base(type, filePath, line)
        {
            Objects = objects;
            ObjectLists = objectLists;

            Primitives = primitives;
            PrimitiveLists = primitiveLists;
        }
    }

    public class ObjectList : ReadMap
    {
        public List<ObjectMap> ObjectItems { get; }

        public ObjectList(List<ObjectMap> objectMaps, string type, string filePath, int line) : base(type, filePath, line)
        {
            ObjectItems = objectMaps;
        }
    }

    public class PrimitiveMap : ReadMap
    {
        public string Value { get; }

        public PrimitiveMap(string value, string type, string filePath, int line) : base(type, filePath, line)
        {
            Value = value;
        }
    }

    public class PrimitiveList : ReadMap
    {
        public List<PrimitiveMap> PrimitiveItems { get; }

        public PrimitiveList(List<PrimitiveMap> primitiveMaps, string type, string filePath, int line) : base(type, filePath, line)
        {
            PrimitiveItems = primitiveMaps;
        }
    }

}
