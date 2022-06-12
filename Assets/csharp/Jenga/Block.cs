using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody), typeof(MeshCollider))]
//[RequireComponent(typeof(JengaController), typeof(ObjectControl))]
public class Block : MonoBehaviour
{
    public const float scale = 1.0f;
    public const float width = 1.25f;
    public const float height = 0.75f;
    public const float length = 3.75f;
    public const float deformation = 0.02f;
    public const float weight = 0.0f;

    private Mesh mesh;

    public int BlockType = 0;
    public bool IsClosedToDevice = false;
    
    private Vector3 DeformRandomly(Vector3 point)
    {
        return new Vector3(
            Random.Range(point.x - deformation, point.x + deformation),
            Random.Range(point.y - deformation, point.y + deformation),
            Random.Range(point.z - deformation, point.z + deformation)
        );
    }

    private void InitMesh()
    {
        if (mesh != null)
        {
            return;
        }

        mesh = new Mesh();
        mesh.name = "Jenga block";

        var vertices = new Vector3[]
        {
            DeformRandomly(new Vector3(0f, 0f, 0f)),
            DeformRandomly(new Vector3(0f, height, 0f)),
            DeformRandomly(new Vector3(width, height, 0f)),
            DeformRandomly(new Vector3(width, 0f, 0f)),
            DeformRandomly(new Vector3(0f, 0f, length)),
            DeformRandomly(new Vector3(0f, height, length)),
            DeformRandomly(new Vector3(width, height, length)),
            DeformRandomly(new Vector3(width, 0f, length)),
        };

        // 블럭 축 변경
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x -= width / 2.0f;
            vertices[i].y -= height / 2.0f;
            vertices[i].z -= length;
        }

        var triangles = new int[] {
            0, 1, 2,
            2, 3, 0,

            7, 6, 5,
            5, 4, 7,

            1, 5, 6,
            6, 2, 1,

            4, 0, 3,
            3 ,7, 4,

            0, 4, 5,
            5, 1, 0,

            2, 6, 7,
            7, 3, 2,
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; ++i)
        {
            uvs[i] = new Vector2(vertices[i].x / mesh.bounds.size.x, vertices[i].z / mesh.bounds.size.z);
        }
        mesh.uv = uvs;

        GetComponent<MeshFilter>().mesh = mesh;
        /*
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, height, length);
        */
        var collider = GetComponent<MeshCollider>();

        collider.convex = true;
        collider.sharedMesh = mesh;

        Rigidbody body = GetComponent<Rigidbody>();
        body.mass = weight;
    }

    private void Start()
    {
        InitMesh();
        GetComponent<Outline>().enabled = false;
    }

    private void FixedUpdate()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        if (body.velocity.magnitude < 0.1f)
        {
            body.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }

        if (body.angularVelocity.magnitude < 0.1f)
        {
            body.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        }

        if(IsClosedToDevice)
        {
            GetComponent<Outline>().enabled = true;
        }
        else
        {
            GetComponent<Outline>().enabled = false;
        }
    }
}
