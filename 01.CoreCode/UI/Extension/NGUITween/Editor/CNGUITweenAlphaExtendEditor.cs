using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CNGUITweenAlphaExtend))]
public class CNGUITweenAlphaExtendEditor : CNGUITweenExtendBaseEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(6f);
        NGUIEditorTools.SetLabelWidth(120f);

        CNGUITweenAlphaExtend pTarget = target as CNGUITweenAlphaExtend;

        GUI.changed = false;
        GUILayout.BeginHorizontal();
        int iGroupSizeNew = EditorGUILayout.IntField("TweenInfoCount", pTarget.listTweenInfo.Count, GUILayout.Width(170f));
        GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		pTarget.p_fTweenSpeed = EditorGUILayout.FloatField( "Speed", pTarget.p_fTweenSpeed, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.FloatField( "TweenAmmount", pTarget.p_fTweenAmount, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();


		if (GUI.changed)
        {
            pTarget.SetTweenInfoSize(iGroupSizeNew);
            NGUITools.SetDirty(pTarget);
        }

        for (int i = 0; i < pTarget.listTweenInfo.Count; i++)
        {
            if (NGUIEditorTools.DrawHeader("Tweener_" + i))
            {
                GUI.changed = false;
                float fFrom = EditorGUILayout.Slider("From", pTarget.listTweenInfo[i].fFrom, 0f, 1f);
                float fTo = EditorGUILayout.Slider("To", pTarget.listTweenInfo[i].fTo, 0f, 1f);

                if (GUI.changed)
                {
                    NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
                    pTarget.listTweenInfo[i].fFrom = fFrom;
                    pTarget.listTweenInfo[i].fTo = fTo;
                    NGUITools.SetDirty(pTarget);
                }

                EventDrawCommonProperties(this, pTarget.listTweenInfo[i]);
            }
        }
    }
}
