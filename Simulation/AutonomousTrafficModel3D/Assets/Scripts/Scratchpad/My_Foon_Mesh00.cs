using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class My_Foon_Mesh00 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Test");
        var vertices = new Vector3[3];
        var uv = new Vector2[3];
        var triangles = new int[3];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 100, 0);
        vertices[2] = new Vector3(100, 100, 0);
        triangles[0] = 0;

        
        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uv,
            triangles = triangles
        };
                
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
