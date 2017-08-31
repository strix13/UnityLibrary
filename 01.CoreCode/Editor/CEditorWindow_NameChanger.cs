using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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
    }

    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    static public CEditorWindow_NameChanger instance;
	
    /* private - Variable declaration           */
    private List<GameObject> _listObject = new List<GameObject>();

    private EChangeType _eChangeType;
    private string _strNameFormat;
    private int _iStartNum;

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
        GameObject[] arrObject = Selection.gameObjects;
        if (arrObject.Length != 0)
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
            
            GUILayout.Space(20f);
            if(GUILayout.Button("Change Name!"))
            {
				_listObject.Clear();
				_listObject.AddRange(arrObject);
				_listObject.Sort(Comparer_Object);

				int iStartNum = _iStartNum;
				char chAlphabet = 'A';
                for(int i = 0; i < _listObject.Count; i++)
                {
                    if(_eChangeType == EChangeType.Number)
                        _listObject[i].name = string.Format(_strNameFormat, iStartNum++);
                    else
                        _listObject[i].name = string.Format(_strNameFormat, chAlphabet++);
                }
            }
            GUILayout.Space(20f);
        }
    }

    // ========================================================================== //
	
    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    private int Comparer_Object(GameObject pObjectX, GameObject pObjectY)
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

}
