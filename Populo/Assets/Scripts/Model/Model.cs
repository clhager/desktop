using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Model
{
    public Dictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();

    public virtual void Init() { }

    public Model() { }

    public Model(ReadMap.ObjectMap map)
    {
        Dictionary<string, System.Reflection.PropertyInfo> properties = GetProperties();

        foreach (string propertyName in map.Objects.Keys)
        {
            ReadMap.ObjectMap objectMap = map.Objects[propertyName];

            if (properties.ContainsKey(propertyName))
            {
                Type propertyType = properties[propertyName].PropertyType;
            }
            else
            {
                Array.Find(typeof(Model).Assembly.GetTypes(), (Type type) => { return type.Name.Equals(propertyName); });
                
            }
            

            if (!propertyType.IsModel() || propertyType.ToStringCustom() != objectMap.Type) OnIncorrectType(propertyName, propertyType.ToStringCustom(), objectMap);
            Model value = (Model)Activator.CreateInstance(propertyType, new object[] { objectMap });
            properties[propertyName].SetValue(this, value);
        }


        foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
        {
            string propertyName = property.Name.LowercaseFirstLetter();
            Type propertyType = property.PropertyType;

            if (propertyType.IsSubclassOf(typeof(Array)))
            {
                if (propertyType.GetElementType().IsSubclassOf(typeof(Model))) HandleModelArrayProperty(propertyName, propertyType, property);
                else HandlePrimitiveArrayProperty(propertyName, propertyType, property);
            }
            else if (propertyType.IsSubclassOf(typeof(Model))) HandleModelProperty(propertyName, propertyType, property);
            else HandlePrimitiveProperty(propertyName, propertyType, property);
        }
        Init();
        return;

        Dictionary<string, System.Reflection.PropertyInfo> GetProperties()
        {
            Dictionary<string, System.Reflection.PropertyInfo> modelProperties = new Dictionary<string, System.Reflection.PropertyInfo>();
            foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
            {
                modelProperties.Add(property.Name.LowercaseFirstLetter(), property);
            }
            return modelProperties;
        }

        void HandleModelArrayProperty(string propertyName, Type propertyType, System.Reflection.PropertyInfo property)
        {
            if (!map.ObjectLists.ContainsKey(propertyName)) OnMissingProperty(propertyName);
            ReadMap.ObjectList objectList = map.ObjectLists[propertyName];
            if (!Types.GenericTypes.Contains(Types.MapTypeName(propertyType.GetElementType().ToString())))
            {
                if (Types.MapTypeName(propertyType.GetElementType().ToString()) != objectList.Type) OnIncorrectType(propertyName, Types.MapTypeName(property.PropertyType.GetElementType().ToString()), objectList);
            }

            List<Model> modelList = new List<Model>();
            foreach (ReadMap.ObjectMap objectMap in objectList.ObjectItems) modelList.Add((Model)Activator.CreateInstance(property.PropertyType.GetElementType(), new object[] { objectMap }));

            property.SetValue(this, Array.CreateInstance(propertyType.GetElementType(), modelList.Count));
            Model[] modelArray = (Model[])property.GetValue(this);
            for (int i = 0; i < modelArray.Length; i++) modelArray[i] = modelList[i];
        }

        void HandlePrimitiveArrayProperty(string propertyName, Type propertyType, System.Reflection.PropertyInfo property)
        {
            if (!map.PrimitiveLists.ContainsKey(propertyName)) OnMissingProperty(propertyName);
            ReadMap.PrimitiveList primitiveList = map.PrimitiveLists[propertyName];
            if (Types.MapTypeName(propertyType.GetElementType().ToString()) != primitiveList.Type) OnIncorrectType(propertyName, Types.MapTypeName(property.PropertyType.GetElementType().ToString()), primitiveList);

            List<object> listOfPrimitives = new List<object>();
            foreach (ReadMap.PrimitiveMap primitiveMap in primitiveList.PrimitiveItems) listOfPrimitives.Add(ReadPrimitives.ReadPrimitive(primitiveMap));
            Array primitiveArray = Array.CreateInstance(propertyType.GetElementType(), listOfPrimitives.Count);
            property.SetValue(this, primitiveArray);
            for (int i = 0; i < primitiveArray.Length; i++) primitiveArray.SetValue(listOfPrimitives[i], i);
        }

        void HandleModelProperty(string propertyName, Type propertyType, System.Reflection.PropertyInfo property)
        {
            if (!map.Objects.ContainsKey(propertyName)) OnMissingProperty(propertyName);
            ReadMap.ObjectMap objectMap = map.Objects[propertyName];
            if (Types.MapTypeName(propertyType.ToString()) != objectMap.Type) OnIncorrectType(propertyName, Types.MapTypeName(propertyType.ToString()), objectMap);

            Model value = (Model)Activator.CreateInstance(propertyType, new object[] { objectMap });
            property.SetValue(this, value);
        }

        void HandlePrimitiveProperty(string propertyName, Type propertyType, System.Reflection.PropertyInfo property)
        {
            if (!map.Primitives.ContainsKey(propertyName)) OnMissingProperty(propertyName);
            ReadMap.PrimitiveMap primitiveMap = map.Primitives[propertyName];
            if (Types.MapTypeName(propertyType.ToString()) != primitiveMap.Type) OnIncorrectType(propertyName, Types.MapTypeName(propertyType.ToString()), primitiveMap);

            property.SetValue(this, ReadPrimitives.ReadPrimitive(primitiveMap));
        }

        void OnMissingProperty(string missingPropertyName)
        {
            throw new ModelReadException(map.FilePath, map.Line, $"{map.Type} missing property {missingPropertyName}");
        }

        void OnIncorrectType(string propertyName, string desiredType, ReadMap value)
        {
            throw new ModelReadException(value.FilePath, value.Line, $"{map.Type} attribute {propertyName} must be of type {desiredType}, not {value.Type}");
        }
    }

    public override string ToString()
    {
        string modelString = "";
        foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
        {
            string name = property.Name;
            object value = property.GetValue(this);
            string type = value.GetTypeName();

            modelString += $"{name.LowercaseFirstLetter()}=";

            if (value.GetType().IsSubclassOf(typeof(Array)))
            {
                modelString += ArrayToString((Array)value);
            }
            else
            {
                if (value.GetType().IsSubclassOf(typeof(Model)))
                {
                    modelString += $"{type}{{{value.ToString()}}}";
                }
                else if (typeof(string).IsInstanceOfType(value))
                {
                    modelString += $"{type}(\"{value.ToString()}\")";
                }
                else
                {
                    string extractPrimitiveValue = value.ToString().ExtractBetweenCharacters('(', ')');
                    if (extractPrimitiveValue != string.Empty) modelString += $"{type}({extractPrimitiveValue})";
                    else modelString += $"{type}({value.ToString()})";
                }
            }

            modelString += ",";
        }
        return modelString;

        string ArrayToString(Array array)
        {
            if (array.GetType().GetElementType().IsSubclassOf(typeof(Model)))
            {
                return ArrayElementsToString("{", array, "}");
            }
            else
            {
                return ArrayElementsToString("(", array, ")");
            }
        }

        string ArrayElementsToString(string beforeElement, Array array, string afterElement)
        {
            string arrayElementsString = $"{Types.MapTypeName(array.GetType().GetElementType().ToString())}[";
            foreach (object element in array)
            {
                arrayElementsString += $"{beforeElement}{element.ToString()}{afterElement},";
            }
            return arrayElementsString + "]";
        }
    }

    public override bool Equals(object obj)
    {
        if (!IsObjectAModel(obj)) return false;

        Model otherModel = (Model)obj;
        if (this.GetTypeName() != otherModel.GetTypeName()) return false;

        HashSet<string> otherModelPropertyNames = GetPropertyNames(otherModel);
        if (GetType().GetProperties().Length != otherModelPropertyNames.Count) return false;

        foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
        {
            if (!DoesModelContainProperty(otherModelPropertyNames, property.Name)) return false;

            object otherModelPropertyValue;
            try
            {
                otherModelPropertyValue = property.GetValue(otherModel);
            }
            catch (UnityException)
            {
                return false;
            }

            if (!property.GetValue(this).Equals(otherModelPropertyValue)) return false;
        }

        return true;

        HashSet<string> GetPropertyNames(Model model)
        {
            HashSet<string> names = new HashSet<string>();
            foreach (System.Reflection.PropertyInfo property in model.GetType().GetProperties())
            {
                names.Add(property.Name);
            }
            return names;
        }

        bool IsObjectAModel(object o)
        {
            return obj.GetType().IsSubclassOf(typeof(Model));
        }

        bool DoesModelContainProperty(HashSet<string> propertyNames, string propertyName)
        {
            return propertyNames.Contains(propertyName);
        }
    }

    public override int GetHashCode()
    {
        int hashCode = GetType().ToString().GetHashCode();
        foreach (System.Reflection.PropertyInfo property in GetType().GetProperties())
        {
            hashCode += property.GetValue(this).GetHashCode();
        }
        return hashCode;
    }

    public class ModelReadException : Read.ReadException
    {
        public ModelReadException(string filePath, int line, string message) : base(filePath, line, message) { }
    }

    public class ModelException : UnityException
    {
        public ModelException(string message) : base(message) { }
        public ModelException(string filePath, int line, string message) : base($"{filePath} (Line {line}) : {message}") { }
    }

}
