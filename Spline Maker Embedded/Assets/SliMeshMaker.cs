using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;
using B83.Image.BMP;
using System.Diagnostics;

public class SliMeshMaker : MonoBehaviour
{
    private Dictionary<Texture2D, bool> textures;
    private Dictionary<Texture2D, bool> oldTextures;
    private List<GameObject> profiles;
    private float meshRenderLength = 300f;

    private static string datPath;

    public void Regenerate(string sli, string dat)
    {
        datPath = dat;

        // clear previously used gameobjects and textures
        DestroyGameObjects(profiles); // accepts null
        profiles = new List<GameObject>();

        // initialise texture lists on first loop
        if (oldTextures == null) oldTextures = new Dictionary<Texture2D, bool>();
        if (textures == null) textures = new Dictionary<Texture2D, bool>();

        // add any previously unseen textures from last loop to the old texture list
        if (textures.Count > 0) foreach (KeyValuePair<Texture2D, bool> p in textures) if (!oldTextures.ContainsKey(p.Key)) oldTextures.Add(p.Key, p.Value);
        textures = new Dictionary<Texture2D, bool>();

        // remove unnecessary line breaks
        string last_sli;
        do
        {
            last_sli = sli;
            sli = sli.Replace("\r\n\r\n", "\r\n");
        }
        while (sli != last_sli);

        // split by start of tag
        string[] sli_delimiter = { "[" };
        string[] sli_split = sli.Split(sli_delimiter, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < sli_split.Length; i++)
        {
            string[] s_delimiter = { "\r\n" };
            string[] s = sli_split[i].Split(s_delimiter, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                switch (s[0])
                {
                    case "texture]":
                        (Texture2D t, bool b) = HandleTextureSequentially(sli_split, i, datPath);
                        textures.Add(t, b);
                        break;
                    case "profile]":
                        // profile needs to be able to acquire data from profilepnts following
                        profiles.Add(HandleProfile(sli_split, i));
                        break;
                    default:
                        //Debug.Log("No case to handle " + s_split[1]);
                        break;
                }
            }
            catch (Exception e)
            {
                // breakpoint here
                UnityEngine.Debug.Log(e);
            }
        }
        
    }

    // textures must be added to the list in the order of the sli file
    public (Texture2D, bool) HandleTextureSequentially(string[] t_in, int index, string datPath)
    {
        string[] delimiter = { "\r\n" };

        string t = t_in[index];
        string t_fullname = t.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[1];

        string[] t_delimiter = { "." };
        string[] t_name_extension = t_fullname.Split(t_delimiter, StringSplitOptions.RemoveEmptyEntries);
        string t_name = t_name_extension[0];
        string t_extension = t_name_extension[1];
        //Texture texture = Resources.Load("Textures/" + t_name) as Texture;

        Texture2D texture = new Texture2D(2, 2);

        // see if the texture already exists in the old texture dictionary
        bool exists = false;
        foreach (Texture2D tex in oldTextures.Keys)
        {
            if (tex.name == t_name)
            {
                exists = true;
                texture = tex;
            }
        }

        // if it doesn't, load it afresh
        if (!exists)
        {
            switch (t_extension)
            {
                case "bmp":
                    //texture = LoadTextureBMP(datPath + "\\texture\\" + t_fullname);
                    //break;
                case "dds":
                    //texture = LoadTextureDXT(datPath + "\\texture\\" + t_fullname);
                    //break;
                case "jpg":
                case "jpeg":
                case "png":
                    //texture = LoadTexturePNGJPG(datPath + "\\texture\\" + t_fullname);
                    //break;
                case "tga":
                case "tif":
                case "tiff":
                case "hdr":
                case "wdp":
                case "hdp":
                case "jxr":
                    texture = LoadTextureOther(datPath + "\\texture\\" + t_fullname);
                    break;
                default:
                    throw new Exception("File format not valid. Image file must be in one of the supported formats:\r\nWindows BMP: .bmp\r\nDirectDraw Surface: .dds (DXT1/3/5)\r\nRadiance RGBE: .hdr\r\nJoint Photographic Experts Group: .jpg, .jpeg\r\nPortable Network Graphics: .png\r\nTruevision Graphics Adapter: .tga\r\nTagged Image File Format: .tif, .tiff\r\nWindows Media Photo: .wdp, .hdp, .jxr");
            }
        }

        texture.name = t_name;

        bool alpha = false;

        // assume there will be no alpha tag if the material has no alpha
        if (t_in[index + 1].Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[0] == "matl_alpha]")
        {
            alpha = true;
        }

        return (texture, alpha);
    }

    // p must be (5 * n) + 2 long where n is the number of profile points
    public GameObject HandleProfile(string[] p_in, int index)
    {
        GameObject g = new GameObject();
        string p = p_in[index];

        string[] delimiter = { "\r\n" };

        // get following profile points
        bool morePoints = true;
        int points = 0;
        while (morePoints == true)
        {
            string np = "";
            try
            {
                np = p_in[index + points + 1];
            }
        catch
            {
                morePoints = false;
            }

            if (morePoints && np.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[0] == "profilepnt]")
            {
                p += np;
                points++;
            }
            else
            {
                morePoints = false;
            }
        }

        string[] p_split = p.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

        // string index 1 is profile material
        int textureIndex = int.Parse(p_split[1]);
        Texture2D texture = textures.ElementAt(textureIndex).Key;

        // len is the number of profile points in this profile
        //int len = (p.Length - 2) / 5;
        float[] pnts = new float[points];
        float[] hgts = new float[points];
        float[] texs = new float[points];
        float[] reps = new float[points];
        for (int i = 0; i < points; i++)
        {
            int baseIndex = (i * 5) + 2;
            pnts[i] = float.Parse(p_split[baseIndex + 1]);
            hgts[i] = float.Parse(p_split[baseIndex + 2]);
            texs[i] = float.Parse(p_split[baseIndex + 3]);
            reps[i] = float.Parse(p_split[baseIndex + 4]);
        }

        int vert_count = 2 * points;
        int quad_count = (points - 1);
        int tri_length = 2 * 3 * quad_count;

        Mesh m = new Mesh();

        Vector3[] vertices = new Vector3[vert_count];
        Vector2[] uv = new Vector2[vert_count];

        for (int i = 0; i < points; i++)
        {
            vertices[i] = new Vector3(pnts[i], hgts[i], -meshRenderLength / 2);
            vertices[i + points] = new Vector3(pnts[i], hgts[i], meshRenderLength / 2);

            //uv[i] = new Vector2(texs[i], 0f);
            //uv[i + points] = new Vector2(texs[i], meshRenderLength * reps[i]);

            uv[i] = new Vector2(texs[i], 0f);
            uv[i + points] = new Vector2(texs[i], meshRenderLength * reps[i]);
        }

        int[] triangles = new int[tri_length];

        for (int i = 0; i < quad_count; i++)
        {
            int base_index = i * 6;
            triangles[base_index] = i;
            triangles[base_index + 1] = i + points;
            triangles[base_index + 2] = i + points + 1;

            triangles[base_index + 3] = i;
            triangles[base_index + 4] = i + points + 1;
            triangles[base_index + 5] = i + 1;
        }

        m.vertices = vertices;
        m.uv = uv;
        m.triangles = triangles;
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.Optimize();

        MeshFilter mf = (MeshFilter)g.AddComponent(typeof(MeshFilter));
        mf.mesh = m;
        Renderer r = g.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        bool alpha = textures.ElementAt(textureIndex).Value;
        if (alpha)
        {
            r.material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            // layer 8 is transparent objects layer
            g.layer = 8;
        }
        else
        {
            r.material.shader = Shader.Find("Legacy Shaders/Diffuse");
        }
        r.material.SetTexture("_MainTex", texture);
        return g;
    }

    //private void DestroyTextures(Dictionary<Texture, bool> list)
    //{
    //    if (list != null)
    //    {
    //        foreach (KeyValuePair<Texture, bool> d in list)
    //        {
    //            Destroy(d.Key);
    //        }
    //    }
    //}

    private void DestroyGameObjects(List<GameObject> list)
    {
        if (list != null)
        {
            foreach (GameObject g in list)
            {
                Destroy(g);
            }
        }
    }

    // https://answers.unity.com/questions/555984/can-you-load-dds-textures-during-runtime.html
    public static Texture2D LoadTextureDXT(string filePath)
    {
        Texture2D tex;
        byte[] fileData;

        if (File.Exists(filePath) && Path.GetExtension(filePath) == ".dds")
        {
            fileData = File.ReadAllBytes(filePath);

            TextureFormat textureFormat = TextureFormat.ARGB32; // this will be discarded
            string headerTextureFormat = "" + Convert.ToChar(fileData[84]) + Convert.ToChar(fileData[85]) + Convert.ToChar(fileData[86]) + Convert.ToChar(fileData[87]);
            switch (headerTextureFormat)
            {
                case "DXT1":
                    textureFormat = TextureFormat.DXT1;
                    break;
                case "DXT5":
                    textureFormat = TextureFormat.DXT5;
                    break;
                case "DXT3":
                    try {
                        string filePathTemp = datPath + "\\temp";
                        string args = "-y -f DXT5 \"" + filePath + "\" -o \"" + filePathTemp + "\"";
                        Process texconv = new Process();
                        texconv.StartInfo.FileName = datPath + "\\texconv.exe";
                        texconv.StartInfo.Arguments = args;
                        texconv.StartInfo.RedirectStandardOutput = true;
                        texconv.StartInfo.RedirectStandardError = true;
                        texconv.StartInfo.UseShellExecute = false;
                        texconv.StartInfo.CreateNoWindow = true;
                        texconv.Start();
                        texconv.WaitForExit();
                        filePathTemp += "\\" + Path.GetFileName(filePath);
                        if (File.Exists(filePathTemp) && Path.GetExtension(filePathTemp) == ".dds")
                        {
                            fileData = File.ReadAllBytes(filePathTemp);
                        }
                        else
                        {
                            throw new Exception("DXT3 to DXT5 conversion unsucessful");
                        }
                        File.Delete(filePathTemp);
                        textureFormat = TextureFormat.DXT5;
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                    break;
                default:
                    throw new Exception("Could not read valid DDS format from header (only DXT1 and DXT5 are supported)");
                }

            byte ddsSizeCheck = fileData[4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            int height = fileData[13] * 256 + fileData[12];
            int width = fileData[17] * 256 + fileData[16];

            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[fileData.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(fileData, DDS_HEADER_SIZE, dxtBytes, 0, fileData.Length - DDS_HEADER_SIZE);

            tex = new Texture2D(width, height, textureFormat, false);
            tex.LoadRawTextureData(dxtBytes);
            tex.Apply();
        }
        else
        {
            throw new Exception("File not found or not in specified format");
        }

        return tex;
    }

    public static Texture2D LoadTextureOther(string filePath)
    {
        Texture2D tex;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);

            TextureFormat textureFormat = TextureFormat.ARGB32; // this will be discarded
            try
            {
                string filePathTemp = datPath + "\\temp";
                string args = "-y -f DXT5 \"" + filePath + "\" -o \"" + filePathTemp + "\"";
                Process texconv = new Process();
                texconv.StartInfo.FileName = datPath + "\\texconv.exe";
                texconv.StartInfo.Arguments = args;
                texconv.StartInfo.RedirectStandardOutput = true;
                texconv.StartInfo.RedirectStandardError = true;
                texconv.StartInfo.UseShellExecute = false;
                texconv.StartInfo.CreateNoWindow = true;
                texconv.Start();
                texconv.WaitForExit();
                filePathTemp += "\\" + Path.GetFileNameWithoutExtension(filePath) + ".dds";
                if (File.Exists(filePathTemp))
                {
                    fileData = File.ReadAllBytes(filePathTemp);
                }
                else
                {
                    throw new Exception("DXT5 conversion unsucessful");
                }
                File.Delete(filePathTemp);
                textureFormat = TextureFormat.DXT5;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e);
            }

            byte ddsSizeCheck = fileData[4];
            if (ddsSizeCheck != 124)
                throw new Exception("Invalid DDS DXTn texture. Unable to read");  //this header byte should be 124 for DDS image files

            int height = fileData[13] * 256 + fileData[12];
            int width = fileData[17] * 256 + fileData[16];

            int DDS_HEADER_SIZE = 128;
            byte[] dxtBytes = new byte[fileData.Length - DDS_HEADER_SIZE];
            Buffer.BlockCopy(fileData, DDS_HEADER_SIZE, dxtBytes, 0, fileData.Length - DDS_HEADER_SIZE);

            tex = new Texture2D(width, height, textureFormat, false);
            tex.LoadRawTextureData(dxtBytes);
            tex.Apply();
        }
        else
        {
            throw new Exception("File not found or not in specified format");
        }

        return tex;
    }

    // https://stackoverflow.com/questions/51975990/how-can-i-use-a-bmp-file-and-create-a-texture-in-unity-at-runtime
    public static Texture2D LoadTextureBMP(string filePath)
    {
        Texture2D tex;
        byte[] fileData;

        if (File.Exists(filePath) && Path.GetExtension(filePath) == ".bmp")
        {
            fileData = File.ReadAllBytes(filePath);

            BMPLoader bmpLoader = new BMPLoader();
            //bmpLoader.ForceAlphaReadWhenPossible = true; //Uncomment to read alpha too

            //Load the BMP data
            BMPImage bmpImg = bmpLoader.LoadBMP(fileData);

            //Convert the Color32 array into a Texture2D
            tex = bmpImg.ToTexture2D();
            tex.Apply();
        }
        else
        {
            throw new Exception("File not found or not in specified format");
        }

        return tex;
    }

    public static Texture2D LoadTexturePNGJPG(string filePath)
    {
        Texture2D tex;
        byte[] fileData;


        if (File.Exists(filePath) && (Path.GetExtension(filePath) == ".png" || Path.GetExtension(filePath) == ".jpg" || Path.GetExtension(filePath) == ".jpeg"))
        {
            fileData = File.ReadAllBytes(filePath);

            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            tex.Apply();
        }
        else
        {
            throw new Exception("File not found or not in specified format");
        }

        return tex;
    }
}
