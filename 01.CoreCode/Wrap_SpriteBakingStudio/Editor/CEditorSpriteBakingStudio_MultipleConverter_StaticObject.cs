//#define SpriteBakingStudio

#if SpriteBakingStudio

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-04-05 오전 1:31:37
   Description : 
   Edit Log    : 
   ============================================ */
   
public class CEditorSpriteBakingStudio_MultipleConverter_StaticObject : Editor
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */
    private class SpriteInfo
    {
        public Texture2D tex;
        public string name;
        public Vector2 pivot;

        public SpriteInfo(Texture2D tex, string name, Vector2 pivot)
        {
            this.tex = tex;
            this.name = name;
            this.pivot = pivot;
        }
    }
    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */
    private static List<GameObject> _listPrefabInstance = new List<GameObject>();
    private static List<Object> _listSpriteInstance = new List<Object>();

    private static SpriteBakingStudio _pStudio;
    private static GameObject _pObjectCurrent;

    private static string _strFolderPath;
    private static int _iSpriteIndex = 0;
    private static bool _bIsCombine;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    [MenuItem("Assets/StrixTool/Convert 3D To 2D", false, 0)]
    static public void DoConvert3DTo2D()
    {
        if (Selection.objects == null || Selection.objects.Length != 1)
            Debug.LogWarning("한개의 폴더만 선택 후 다시 눌러주세요");
        else
        {
            _pStudio = FindObjectOfType<SpriteBakingStudio>();
            if(_pStudio == null)
            {
                Debug.Log("씬에 SpriteBakingStudio가 존재하지 않습니다.");
                return;
            }

            _strFolderPath = Selection.objects[0].name;
            string strPath = AssetDatabase.GetAssetPath(Selection.objects[0]);

            _bIsCombine = false;
            DestroyResource();
            MakeResource(strPath + "/" + _strFolderPath);

            _pStudio.directoryPath = strPath + "/" + _strFolderPath;
            _iSpriteIndex = 0;
            ProcConvert3DTo2D();
        }
    }

    [MenuItem("Assets/StrixTool/Convert 3D To 2D And Combine", false, 0)]
    static public void DoConvert3DTo2DCombine()
    {
        if (Selection.objects == null || Selection.objects.Length != 1)
            Debug.LogWarning("한개의 폴더만 선택 후 다시 눌러주세요");
        else
        {
            _pStudio = FindObjectOfType<SpriteBakingStudio>();
            if (_pStudio == null)
            {
                Debug.Log("씬에 SpriteBakingStudio가 존재하지 않습니다.");
                return;
            }

            _strFolderPath = Selection.objects[0].name;
            string strPath = AssetDatabase.GetAssetPath(Selection.objects[0]);

            _bIsCombine = true;
            _listSpriteInstance.Clear();
            DestroyResource();
            MakeResource(strPath + "/" + _strFolderPath);

            _pStudio.directoryPath = strPath + "/" + _strFolderPath;
            _iSpriteIndex = 0;
            ProcConvert3DTo2D();
        }
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private static void CombineSprites()
    {
        try
        {
            Object[] selectedObjects = Selection.objects;
            if (selectedObjects.Length <= 1)
                return;

            List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();
            List<Texture2D> textures = new List<Texture2D>();

            foreach (Object obj in selectedObjects)
            {
                if (!(obj is Texture2D))
                    continue;
                Texture2D selSpriteSheet = obj as Texture2D;

                TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj));
                if (importer == null)
                    continue;
                importer.textureType = TextureImporterType.Default;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.mipmapEnabled = false;
                importer.isReadable = true;
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(obj));
                AssetDatabase.Refresh();

                int count = 0;
                foreach (SpriteMetaData metaData in importer.spritesheet)
                {
                    Texture2D tex = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height,
                                                  TextureFormat.RGBA32, false);
                    for (int y = 0; y < tex.height; y++)
                    {
                        for (int x = 0; x < tex.width; x++)
                        {
                            Color color = selSpriteSheet.GetPixel((int)metaData.rect.x + x, (int)metaData.rect.y + y);
                            tex.SetPixel(x, y, color);
                        }
                    }

                    SpriteInfo spriteInfo = new SpriteInfo(tex, selSpriteSheet.name + count, metaData.pivot);
                    spriteInfoList.Add(spriteInfo);
                    textures.Add(tex);

                    count++;
                }
            }

            Texture2D newSpriteSheet = new Texture2D(4096, 4096, TextureFormat.ARGB32, false);
            Rect[] texRects = newSpriteSheet.PackTextures(textures.ToArray(), 2);
            for (int i = 0; i < spriteInfoList.Count; i++)
            {
                Texture2D tex = spriteInfoList[i].tex;
                float newX = texRects[i].x * newSpriteSheet.width;
                float newY = texRects[i].y * newSpriteSheet.height;
                texRects[i] = new Rect(newX, newY, (float)tex.width, (float)tex.height);
            }

            Texture2D firstSpriteSheet = selectedObjects[0] as Texture2D;
            string filePath = AssetDatabase.GetAssetPath(firstSpriteSheet);
            filePath = filePath.Remove(filePath.LastIndexOf('/')) + "/CombinedSpriteSheet.png";
            byte[] bytes = newSpriteSheet.EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
            AssetDatabase.Refresh();

            TextureImporter texImporter = (TextureImporter)AssetImporter.GetAtPath(filePath);
            if (texImporter != null)
            {
                texImporter.textureType = TextureImporterType.Sprite;
                texImporter.spriteImportMode = SpriteImportMode.Multiple;
                texImporter.maxTextureSize = 4096;

                int texCount = spriteInfoList.Count;
                SpriteMetaData[] metaData = new SpriteMetaData[texCount];
                for (int i = 0; i < texCount; i++)
                {
                    metaData[i].name = spriteInfoList[i].name;
                    metaData[i].rect = texRects[i];
                    metaData[i].alignment = (int)SpriteAlignment.Custom;
                    metaData[i].pivot = spriteInfoList[i].pivot;
                }
                texImporter.spritesheet = metaData;

                AssetDatabase.ImportAsset(filePath);
                AssetDatabase.Refresh();
            }
        }
        catch (System.NullReferenceException) { }
        catch (System.IndexOutOfRangeException) { }
        catch (UnassignedReferenceException) { }
        catch (MissingReferenceException) { }
    }

    static private void ProcConvert3DTo2D()
    {
        if (_iSpriteIndex < _listPrefabInstance.Count)
        {
            _pObjectCurrent = _listPrefabInstance[_iSpriteIndex];
            _pObjectCurrent.SetActive(true);

            FrameSampler sampler = FrameSampler.GetInstance();
            sampler.SampleFrames(_pStudio);
            sampler.OnEnd = ShowSelectorAndViewer;
        }
        else
        {
            Debug.Log("컨버팅 모두 완료");
            DestroyResource();

            if (_bIsCombine)
            {
                List<Texture2D> listSprite = GetAllPrefabInFolder<Texture2D>(_pStudio.directoryPath + "/" + _strFolderPath);
                Selection.objects = listSprite.ToArray();
                CombineSprites();
            }
        }
    }

    static private void DestroyResource()
    {
        for (int i = 0; i < _listPrefabInstance.Count; i++)
            DestroyImmediate(_listPrefabInstance[i]);

        _listPrefabInstance.Clear();
    }

    static private void MakeResource(string strResourcePath)
    {
        List<GameObject> listPrefab = GetAllPrefabInFolder<GameObject>(strResourcePath);
        for (int i = 0; i < listPrefab.Count; i++)
        {
            GameObject pObjectMake = Instantiate<GameObject>(listPrefab[i]);
            pObjectMake.SetActive(false);
            Transform pTransMake = pObjectMake.transform;
            pTransMake.position = Vector3.one;
            pTransMake.rotation = Quaternion.identity;
            pTransMake.localScale = Vector3.one;
            _listPrefabInstance.Add(pObjectMake);
        }
    }

    static private List<T> GetAllPrefabInFolder<T>(string strPath)
        where T : UnityEngine.Object
    {
        List<T> listAssetResource = new List<T>();
        DirectoryInfo pDirectory = new DirectoryInfo(strPath);

        string strFileExtensionName = "";
        string strTypeName = typeof(T).FullName;
        if (strTypeName.Contains("GameObject"))
            strFileExtensionName = ".prefab";
        else if (strTypeName.Contains("Sprite") || strTypeName.Contains("Texture2D"))
            strFileExtensionName = ".png";

        pDirectory = pDirectory.Parent;
        FileInfo[] arrAllFileInFolder = pDirectory.GetFiles();
        for (int i = 0; i < arrAllFileInFolder.Length; i++)
        {
            string strFileName = arrAllFileInFolder[i].FullName;
            if (strFileName.Contains(".meta") == false &&
                strFileName.Contains(strFileExtensionName))
            {
                string strRelativeFilePath = ConvertRelativePath(arrAllFileInFolder[i].FullName);
                T pPrefab = AssetDatabase.LoadAssetAtPath(strRelativeFilePath, typeof(T)) as T;
                if (pPrefab != null)
                    listAssetResource.Add(pPrefab);
            }
        }

        return listAssetResource;
    }

    static public void ShowSelectorAndViewer()
    {
        try
        {
            _pStudio.selectedFrames.Clear();
            _pStudio.selectedFrames.Add(0);
            _pStudio.samplingCount = 1;
            _pStudio.fileName = _pObjectCurrent.name.Substring(0, _pObjectCurrent.name.Length - "(clone)".Length);

            _pStudio._pOnEndBakeStudio = OnFinishBakeSprite;
            _pStudio.BakeSprites();
        }
        catch (System.NullReferenceException e)
        {
            throw e;
        }
        catch (UnassignedReferenceException e)
        {
            throw e;
        }
    }

    static private void OnFinishBakeSprite()
    {
        _iSpriteIndex++;
        _pObjectCurrent.SetActive(false);
        ProcConvert3DTo2D();
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    static private string ConvertRelativePath(string strPath)
    {
        return "Assets" + strPath.Substring(Application.dataPath.Length);
    }
}
#endif