using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshExtensions
{
    public static Mesh CombineMeshes(params Mesh[] meshes)
    {
        if (meshes.Length < 1) return new Mesh();

        (List<Vector3> vertices, List<int> triangles) = (new List<Vector3>(meshes[0].vertices), new List<int>(meshes[0].triangles));
        for (int i = 1; i < meshes.Length; i++)
        {
            int vertexBaseline = vertices.Count;
            vertices.AddRange(meshes[i].vertices);
            foreach(int point in meshes[i].triangles)
            {
                triangles.Add(point + vertexBaseline);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
