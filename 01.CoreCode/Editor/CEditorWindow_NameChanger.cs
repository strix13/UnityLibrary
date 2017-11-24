using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-04 오후 4:33:17
   Description : 
   Edit Log    : 
   ============================================ */

public class CEditorWindow_NameChanger : EditorWindow
{
    public enum EChangeType
    {
        Number,
        Alphabet,
		Alphabet_Grade,
	}

	public enum EAlphabetGrade
	{
		F,
		D,
		C,
		B,
		A,
		S,
		SS,
		SSS,
	}

	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	static public CEditorWindow_NameChanger instance;

	/* private - Variable declaration           */
	private List<GameObject> _listGameObject = new List<GameObject>();
	private List<Object> _listObject = new List<Object>();

	private EChangeType _eChangeType;
    private string _strNameFormat;
    private int _iStartNum;
	private EAlphabetGrade _eAlphabetGrade = EAlphabetGrade.F;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	[MenuItem("Strix_Tools/NameChanger")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(CEditorWindow_NameChanger));
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //
	
	/* protected - Override & Unity API         */

	void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    void OnGUI()
    {
		EditorGUILayout.HelpBox("String.Format 함수의 인자처럼 사용하시면 됩니다.\n 예) : name_{0}", MessageType.Info);
        GUILayout.Space(20f);

        GUILayout.BeginHorizontal();
        _strNameFormat = EditorGUILayout.TextField("Name Format", _strNameFormat, GUILayout.Width(500f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _eChangeType = (EChangeType)EditorGUILayout.EnumPopup("Fill Format", _eChangeType);
        GUILayout.EndHorizontal();

        if (_eChangeType == EChangeType.Number)
        {
            GUILayout.BeginHorizontal();
            _iStartNum = EditorGUILayout.IntField("StartNum", _iStartNum);
            GUILayout.EndHorizontal();
        }
		else if(_eChangeType == EChangeType.Alphabet_Grade)
		{
			GUILayout.BeginHorizontal();
			_eAlphabetGrade = (EAlphabetGrade)EditorGUILayout.EnumPopup( "StartGrade", _eAlphabetGrade );
			GUILayout.EndHorizontal();
		}

		GUILayout.Space(20f);
        if(GUILayout.Button("Change Name!"))
        {
			int iStartNum = _iStartNum;
			char chAlphabet = 'A';
			EAlphabetGrade eAlphabetGrade = _eAlphabetGrade;

			GameObject[] arrObject = Selection.gameObjects;
			_listGameObject.Clear();
			_listGameObject.AddRange( arrObject );
			_listGameObject.Sort( Comparer_GameObject );

			for (int i = 0; i < _listGameObject.Count; i++)
            {
                if(_eChangeType == EChangeType.Number)
					_listGameObject[i].name = string.Format(_strNameFormat, iStartNum++);
                else if(_eChangeType == EChangeType.Alphabet)
					_listGameObject[i].name = string.Format(_strNameFormat, chAlphabet++);
				else if (_eChangeType == EChangeType.Alphabet_Grade)
					_listGameObject[i].name = string.Format( _strNameFormat, eAlphabetGrade++ );
			}

			Object[] arrFile = Selection.objects;
			_listObject.Clear();
			_listObject.AddRange( arrFile );
			_listObject.Sort( Comparer_Object );

			for (int i = 0; i < _listObject.Count; i++)
			{
				string strAssetPath = AssetDatabase.GetAssetPath( _listObject[i] );
				string strFilePath = Path.Combine( Directory.GetCurrentDirectory(), strAssetPath );
				DirectoryInfo pDirectoryInfo = new DirectoryInfo( strFilePath );
				string strExtension = pDirectoryInfo.Extension;
				pDirectoryInfo = pDirectoryInfo.Parent;
				string strName = "";
				if (_eChangeType == EChangeType.Number)
					strName = string.Format( _strNameFormat, iStartNum++ );
				else if (_eChangeType == EChangeType.Alphabet)
					strName = string.Format( _strNameFormat, chAlphabet++ );
				else if (_eChangeType == EChangeType.Alphabet_Grade)
					strName = string.Format( _strNameFormat, eAlphabetGrade++ );

				string strFilePathNew = Path.Combine( pDirectoryInfo.ToString(), strName );
				strFilePathNew += strExtension;

				AssetDatabase.RenameAsset( strFilePath, strName );
				File.Move( strFilePath, strFilePathNew );
			}
		}
        GUILayout.Space(20f);
    }

    // ========================================================================== //
	
    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    private int Comparer_GameObject(GameObject pObjectX, GameObject pObjectY)
    {
		int iSiblingIndexX = pObjectX.transform.GetSiblingIndex();
		int iSiblingIndexY = pObjectY.transform.GetSiblingIndex();

		if (iSiblingIndexX < iSiblingIndexY)
			return -1;
		else if (iSiblingIndexX > iSiblingIndexY)
			return 1;
		else
			return 0;
    }

	private int Comparer_Object( Object pObjectX, Object pObjectY )
	{
		return System.String.CompareOrdinal( pObjectX.name, pObjectY.name );
	}
}