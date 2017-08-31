using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-02 오후 6:03:16
   Description : 
   Edit Log    : 
   ============================================ */

public class SCStrix_Tools
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */
     
    [MenuItem("Assets/StrixTool/Update Atlas This Folder", false, 0)]
    static public void DoUpdateAtlas()
    {
        if(Selection.objects == null || Selection.objects.Length != 1)
            Debug.LogWarning("한개의 폴더만 선택 후 다시 눌러주세요");
        else
        {
            string strFolderName = Selection.objects[0].name;
            string strPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            MakeAtlas(strPath + "/" + strFolderName);
        }
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    static private void MakeAtlas(string strPath)
    {
        string strPath_Mat = strPath + ".mat";
        string strPath_Prefab = strPath + ".prefab";
        GameObject pObjectAtlas = AssetDatabase.LoadAssetAtPath(strPath_Prefab, typeof(GameObject)) as GameObject;
        Texture pTextureAtlas = null;

        if (pObjectAtlas == null)
        {
            Material mat = null;

            Shader shader = Shader.Find(NGUISettings.atlasPMA ? "Unlit/Premultiplied Colored" : "Unlit/Transparent Colored");
            mat = new Material(shader);

            // Save the material
            AssetDatabase.CreateAsset(mat, strPath_Mat);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Load the material so it's usable
            mat = AssetDatabase.LoadAssetAtPath(strPath_Mat, typeof(Material)) as Material;

            // Create a new prefab for the atlas
            Object prefab = PrefabUtility.CreateEmptyPrefab(strPath_Prefab);

            // Create a new game object for the atlas
            string atlasName = strPath_Prefab.Replace(".prefab", "");
            atlasName = atlasName.Substring(strPath.LastIndexOfAny(new char[] { '/', '\\' }) + 1);
            pObjectAtlas = new GameObject(atlasName);
            pObjectAtlas.AddComponent<UIAtlas>().spriteMaterial = mat;

            // Update the prefab
            PrefabUtility.ReplacePrefab(pObjectAtlas, prefab);

            UnityEngine.GameObject.DestroyImmediate(pObjectAtlas);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Select the atlas
            pObjectAtlas = AssetDatabase.LoadAssetAtPath(strPath_Prefab, typeof(GameObject)) as GameObject;
            NGUISettings.atlas = pObjectAtlas.GetComponent<UIAtlas>();
        }
        else
            pTextureAtlas = pObjectAtlas.GetComponent<UIAtlas>().texture;

        List<Texture> listTexture = GetAllTextureInFolder(strPath, pTextureAtlas);
        List<UIAtlasMaker.SpriteEntry> listSprite = UIAtlasMaker.CreateSprites(listTexture);
        UIAtlasMaker.ExtractSprites(NGUISettings.atlas, listSprite);
        UIAtlasMaker.UpdateAtlas(NGUISettings.atlas, listSprite);
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    static private List<Texture> GetAllTextureInFolder(string strPath, Texture pAtlasTexture)
    {
        List<Texture> listTexture = new List<Texture>();
        DirectoryInfo pDirectory = new DirectoryInfo(strPath);

        pDirectory = pDirectory.Parent;
        FileInfo[] arrAllFileInFolder = pDirectory.GetFiles();
        for (int i = 0; i < arrAllFileInFolder.Length; i++)
        {
            string strFileName = arrAllFileInFolder[i].FullName;
            if (strFileName.Contains(".meta") == false &&
                strFileName.Contains(".png") || strFileName.Contains(".jpg"))
            {
            string strRelativeFilePath = ConvertRelativePath(arrAllFileInFolder[i].FullName);
                Texture pTexture = AssetDatabase.LoadAssetAtPath(strRelativeFilePath, typeof(Texture)) as Texture;
                if (pTexture != null && pTexture != pAtlasTexture)
                    listTexture.Add(pTexture);
            }
        }

        return listTexture;
    }

    static private List<Texture> GetSelectedTextures()
    {
        List<Texture> textures = new List<Texture>();
        if (Selection.objects != null && Selection.objects.Length > 0)
        {
            Object[] objects = EditorUtility.CollectDependencies(Selection.objects);
            foreach (Object o in objects)
            {
                Texture tex = o as Texture;
                if (tex == null || tex.name == "Font Texture") continue;
                if (NGUISettings.atlas == null || NGUISettings.atlas.texture != tex) textures.Add(tex);
            }
        }
        return textures;
    }

    static private string ConvertRelativePath(string strPath)
    {
        return "Assets" + strPath.Substring(Application.dataPath.Length);
    }
}
