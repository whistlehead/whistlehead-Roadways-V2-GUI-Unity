using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO.Pipes;
using System.Text;
using System.Security.Principal;
using System.IO.MemoryMappedFiles;

public class MeshMaker : MonoBehaviour
{

    private class profData
    {
        public int len;
        public eTex mat;
        public float[] pnts;
        public float[] hgts;
        public float[] texs;
        public float[] reps;

        public profData(int in_len, eTex in_mat, float[] in_pnts, float[] in_hgts, float[] in_texs, float[] in_reps)
        {
            len = in_len;
            mat = in_mat;

            pnts = fillArray(in_pnts, in_len);
            hgts = fillArray(in_hgts, in_len);
            texs = fillArray(in_texs, in_len);
            reps = fillArray(in_reps, in_len);
        }

        public profData(int in_len, eTex in_mat, float[] in_pnts, float in_hgt, float[] in_texs, float in_rep)
        {
            len = in_len;
            mat = in_mat;

            pnts = fillArray(in_pnts, in_len);
            hgts = fillArray(new float[] { in_hgt }, in_len);
            texs = fillArray(in_texs, in_len);
            reps = fillArray(new float[] { in_rep }, in_len);
        }

        public profData(int in_len, eTex in_mat, float[] in_pnts, float[] in_hgts, float[] in_texs, float in_rep)
        {
            len = in_len;
            mat = in_mat;

            pnts = fillArray(in_pnts, in_len);
            hgts = fillArray(in_hgts, in_len);
            texs = fillArray(in_texs, in_len);
            reps = fillArray(new float[] { in_rep }, in_len);
        }
    }

    private static float[] fillArray(float[] in_arr, int in_len)
    {
        // protection against non-allowable 1 value profiles
        int len = Math.Max(in_len, 2);

        float[] arr = new float[len];
        int in_arr_len = in_arr.Length;
        for (int i = 0; i < len; i++)
        {
            // if i exists, copy it
            if (i < in_arr_len)
            {
                arr[i] = in_arr[i];
                // else duplicate the last value
            }
            else if (i > 0)
            {
                arr[i] = arr[i - 1];
            }
            else
            {
                throw new System.InvalidOperationException("Error in filling profile array");
            }
        }
        return arr;
    }

    private static class eLaneW
    {
        public const float std = 3.65f;
        public const float nar = 2.9f;

        public static float Get(int input)
        {
            switch (input)
            {
                case 1:
                    return nar;
                default:
                    return std;
            }
        }
    }

    private static class eHLineW
    {
        public const float std = 0.05f;
        public const float wid = 0.075f;
    }

    private class texData
    {
        private string name;
        private int alpha;
        private int index = 0;

        public texData(string in_name, int in_alpha)
        {
            name = in_name;
            alpha = in_alpha;
        }

        public void SetIndex(int in_index)
        {
            index = in_index;
        }

        public (string, int) GetNameAlpha()
        {
            return (name, alpha);
        }

        public int GetIndex()
        {
            return index;
        }
    }

    private enum eTex
    {
        aspha,
        aspst,
        verge,
        white,
        yello,
        hatch,
        kerbs,
        pavmt,
        terra
    }

    private static readonly float centre = 0;
    private static readonly float h_lane = 0.1f;
    private static readonly float h_line = 0.105f;
    private static readonly float rep_5m = 0.2f;
    private static readonly float rep_6m = 0.167f;


    private static readonly List<profData> data_lane_std = new List<profData> { new profData(2, eTex.aspha, new float[] { centre, eLaneW.std }, h_lane, new float[] { 1.000f, 0.000f }, rep_5m) };
    private static readonly List<profData> data_lane_nar = new List<profData> { new profData(2, eTex.aspha, new float[] { centre, eLaneW.nar }, h_lane, new float[] { 1.000f, 0.000f }, rep_5m) };
    private List<List<profData>> data_lane = new List<List<profData>> { data_lane_std, data_lane_nar };

    private static readonly List<profData> data_centreline_norm_std = new List<profData> { new profData(2, eTex.white, new float[] { -eHLineW.std, eHLineW.std }, h_line, new float[] { 0.406f, 0.438f }, rep_6m) };

    Texture tex_asphalt;
    Texture tex_white;

    private float meshRenderLength = 30f;

    List<GameObject> gameObjects;

    private void DestroyGameObjects(List<GameObject> list)
    {
        foreach (GameObject g in list)
        {
            Destroy(g);
        }
    }

    private Mesh ProfileToMesh(profData profile)
    {
        int len = profile.len;
        int vert_count = 2 * len;
        int quad_count = (len - 1);
        int tri_length = 2 * 3 * quad_count;

        Mesh m = new Mesh();

        //Vector3[] vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 0, 0) };
        //Vector2[] uv = new Vector2[] { new Vector3(0, 0), new Vector3(1, 0), new Vector3(1, 1), new Vector3(0, 1) };

        Vector3[] vertices = new Vector3[vert_count];
        Vector2[] uv = new Vector2[vert_count];

        for (int i = 0; i < len; i++)
        {
            vertices[i] = new Vector3(profile.pnts[i], profile.hgts[i], 0f);
            vertices[i + len] = new Vector3(profile.pnts[i], profile.hgts[i], meshRenderLength);

            uv[i] = new Vector2(profile.texs[i], 0f);
            uv[i + len] = new Vector2(profile.texs[i], meshRenderLength / (10 * profile.reps[i]));
        }

        int[] triangles = new int[tri_length];

        for (int i = 0; i < quad_count; i++)
        {
            triangles[i] = i;
            triangles[i + 1] = i + len;
            triangles[i + 2] = i + len + 1;

            triangles[i + 3] = i;
            triangles[i + 4] = i + len + 1;
            triangles[i + 5] = i + 1;
        }

        m.vertices = vertices;
        m.uv = uv;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.Optimize();

        return m;
    }

    private GameObject MakeProfileGameObject(profData profile, Texture texture)
    {
        GameObject g = new GameObject();
        MeshFilter m = (MeshFilter)g.AddComponent(typeof(MeshFilter));
        m.mesh = ProfileToMesh(profile);
        Renderer r = g.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        r.material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        r.material.SetTexture("_MainTex", texture);
        g.transform.position = new Vector3(0f, 0f, - (meshRenderLength / 2));
        return g;
    }

    private void RenderSpline()
    {
        //gameObjects = new List<GameObject>();
        //gameObjects.Add(MakeProfileGameObject(data_lane_std[0], tex_asphalt));
        //gameObjects.Add(MakeProfileGameObject(data_centreline_norm_std[0], tex_white));
    }

        // Start is called before the first frame update
    void Start()
    {
        tex_asphalt = Resources.Load("Textures/asphalt") as Texture;
        tex_white = Resources.Load("Textures/white") as Texture;

        //RenderSpline();


    }

    // Update is called once per frame
    void Update()
    {
        //DestroyGameObjects(gameObjects);
    }
}

