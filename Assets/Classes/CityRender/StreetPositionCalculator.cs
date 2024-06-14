using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetPositionCalculator : MonoBehaviour
{
    public static (List<Matrix4x4> street, List<Matrix4x4> edge) createStreet_Matrix4x4(GameObject parent, GameObject street, GameObject street_edge, float a, float b)
    {
        Vector3 streetSize = street.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        Vector3 edgeSize = street_edge.GetComponent<MeshFilter>().sharedMesh.bounds.size;

        List<Matrix4x4> streetMatricesN = new List<Matrix4x4>();
        List<Matrix4x4> edgeMatricesN = new List<Matrix4x4>();

        int streetCount = Mathf.Max(1, (int)(b / streetSize.y));
        float scale = (b / streetCount) / streetSize.y;

        //Edge Streets
        var t = parent.transform.position + new Vector3(-b / 2 - edgeSize.y / 2 + 2.8f, 0, -streetSize.x / 2);
        var r = Quaternion.Euler(parent.transform.rotation.eulerAngles + new Vector3(-90, -90, 0));
        var s = new Vector3(1, 1, 1);
        var mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);
        t = parent.transform.position + new Vector3(b / 2 + edgeSize.y / 2 - 2.8f, 0, -streetSize.x / 2);
        r = Quaternion.Euler(parent.transform.rotation.eulerAngles + new Vector3(-90, 180, 0));
        mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);
        t = parent.transform.position + new Vector3(-b / 2 - edgeSize.y / 2 + 2.8f, 0, a + streetSize.x / 2);
        r = Quaternion.Euler(parent.transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
        mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);
        t = parent.transform.position + new Vector3(b / 2 + edgeSize.y / 2 - 2.8f, 0, a + streetSize.x / 2);
        r = Quaternion.Euler(parent.transform.rotation.eulerAngles + new Vector3(-90, 90, 0));
        mat = Matrix4x4.TRS(t, r, s);
        edgeMatricesN.Add(mat);

        //Straight Streets
        for (int i = 0; i < streetCount; i++)
        {
            t = parent.transform.position + new Vector3(-b / 2 + streetSize.y * scale / 2 + i * scale * streetSize.y, 0, -streetSize.x / 2);
            r = Quaternion.Euler(parent.transform.rotation.eulerAngles + new Vector3(-90, 90, 0));
            s = new Vector3(1, scale, 1);

            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);

            t = parent.transform.position + new Vector3(-b / 2 + streetSize.y * scale / 2 + i * scale * streetSize.y, 0, a + streetSize.x / 2);
            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);
        }

        streetCount = Mathf.Max(1, (int)(a / streetSize.y));
        scale = (a / streetCount) / streetSize.y;

        for (int i = 0; i < streetCount; i++)
        {
            t = parent.transform.position + new Vector3(-b / 2 - streetSize.x / 2, 0, streetSize.y * scale / 2 + i * scale * streetSize.y);
            r = Quaternion.Euler(parent.transform.rotation.eulerAngles + new Vector3(-90, 0, 0));
            s = new Vector3(1, scale, 1);

            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);

            t = parent.transform.position + new Vector3(b / 2 + streetSize.x / 2, 0, streetSize.y * scale / 2 + i * scale * streetSize.y);
            mat = Matrix4x4.TRS(t, r, s);
            streetMatricesN.Add(mat);
        }
        return (streetMatricesN, edgeMatricesN);
    }

    //###############################################################################################
    //Translation from Matrix4x4 to transform GameObjects
    //###############################################################################################

    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    {
        transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }


    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }

    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}
