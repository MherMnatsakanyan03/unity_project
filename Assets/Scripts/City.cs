using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class City : MonoBehaviour
{
    private bool s = true;
    private List<Matrix4x4> streetMatricesN;
    private List<Matrix4x4> edgeMatricesN;

    [SerializeField]
    private float districtSquareMeterSize;
    private float previousdistrictSquareMeterSize = 0;

    [SerializeField]
    private Mesh streetMesh, edgeMesh;

    [SerializeField]
    private Material streetMateria0, streetMateria1, edgeMateria0, edgeMateria1;

    [SerializeField]
    private GameObject cube;

    private Vector3 streetSize;
    private Vector3 edgeSize;
    private float randomNumber;
    private GameObject[] folderObjects;

    private Grid grid;

    void renderStreets()
    {
        if (streetMatricesN != null)
        {
            Graphics.DrawMeshInstanced(streetMesh, 0, streetMateria0, streetMatricesN.ToArray(), streetMatricesN.Count);
            Graphics.DrawMeshInstanced(streetMesh, 1, streetMateria1, streetMatricesN.ToArray(), streetMatricesN.Count);
        }

        if (edgeMatricesN != null)
        {
            Graphics.DrawMeshInstanced(edgeMesh, 0, edgeMateria0, edgeMatricesN.ToArray(), edgeMatricesN.Count);
            Graphics.DrawMeshInstanced(edgeMesh, 1, edgeMateria1, edgeMatricesN.ToArray(), edgeMatricesN.Count);
        }
    }

    void createStreet()
    {
        streetMatricesN = new List<Matrix4x4>();
        edgeMatricesN = new List<Matrix4x4>();

        var b = Mathf.Sqrt(districtSquareMeterSize / randomNumber) * 10;
        var a = randomNumber * b;

        int streetCount = Mathf.Max(1, (int)(b / streetSize.y));
        float scale = (b / streetCount) / streetSize.y;

        grid.update((int)b, (int)a, (int)(b / 2), 0, (float)(a), (float)(b));

        var t = transform.position + new Vector3(-b / 2- edgeSize.y/2 + 2.8f, 0, -streetSize.x / 2);
        var r = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, -90, 0));
        var s = new Vector3(1, 1, 1);
        var mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);
        t = transform.position + new Vector3(b / 2 + edgeSize.y / 2 - 2.8f, 0, -streetSize.x / 2);
        r = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, 180, 0));
        mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);
        t = transform.position + new Vector3(-b / 2 - edgeSize.y / 2 + 2.8f, 0, a + streetSize.x / 2);
        r = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
        mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);
        t = transform.position + new Vector3(b / 2 + edgeSize.y / 2 - 2.8f, 0, a + streetSize.x / 2);
        r = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, 90, 0));
        mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);

        for (int i = 0; i < streetCount; i++)
        {
            t = transform.position + new Vector3(-b/2 + streetSize.y * scale /2 + i * scale * streetSize.y, 0, -streetSize.x/2);
            r = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, 90, 0));
            s = new Vector3(1, scale, 1);

            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);

            t = transform.position + new Vector3(-b / 2 + streetSize.y * scale / 2 + i * scale * streetSize.y, 0, a + streetSize.x / 2);
            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);
        }

        streetCount = Mathf.Max(1, (int)(a / streetSize.y));
        scale = (a / streetCount) / streetSize.y;

        for (int i = 0; i < streetCount; i++)
        {
            t = transform.position + new Vector3(-b/2-streetSize.x/2, 0,streetSize.y * scale / 2 + i * scale * streetSize.y);
            r = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
            s = new Vector3(1, scale, 1);

            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);

            t = transform.position + new Vector3(b/2+ streetSize.x / 2, 0,streetSize.y * scale / 2 + i * scale * streetSize.y);
            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        randomNumber = (float)random.NextDouble() * 0.2f + 0.4f;
        streetSize = streetMesh.bounds.size;
        edgeSize = edgeMesh.bounds.size;

        List<GameObject> game_objects = new List<GameObject>(GameObject.FindGameObjectsWithTag("test"));
        Debug.Log(game_objects.Count);
        //GameObject randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        grid = new Grid(0, 0, 1f, 0, 0, game_objects);
    }

    void Update()
    {
        if (previousdistrictSquareMeterSize != districtSquareMeterSize)
        {
            createStreet();
            previousdistrictSquareMeterSize = districtSquareMeterSize;
        }
        renderStreets();
    }

}
