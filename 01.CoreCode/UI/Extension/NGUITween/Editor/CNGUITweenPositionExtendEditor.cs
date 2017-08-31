using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CNGUITweenPositionExtend))]
public class CNGUITweenPositionExtendEditor : CNGUITweenExtendBaseEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(6f);
        NGUIEditorTools.SetLabelWidth(120f);

        CNGUITweenPositionExtend pTarget = target as CNGUITweenPositionExtend;

        GUI.changed = false;
        GUILayout.BeginHorizontal();
        int iGroupSizeNew = EditorGUILayout.IntField("TweenInfoCount", pTarget.listTweenInfo.Count, GUILayout.Width(170f));
        GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		bool bIgnoreTimeScale = EditorGUILayout.Toggle( "IgnoreTimeScale", pTarget.ignoreTimeScale, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float fTweenSpeed = EditorGUILayout.FloatField( "Speed", pTarget.p_fTweenSpeed, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.FloatField( "Tween Amount", pTarget.p_fTweenAmount, GUILayout.Width( 170f ) );
		GUILayout.EndHorizontal();

		if (GUI.changed)
        {
			pTarget.p_fTweenSpeed = fTweenSpeed;
			pTarget.ignoreTimeScale = bIgnoreTimeScale;

			pTarget.SetTweenInfoSize(iGroupSizeNew);
            NGUITools.SetDirty(pTarget);
            GUI.changed = false;
        }

        for (int i = 0; i < pTarget.listTweenInfo.Count; i++)
        {
            if (NGUIEditorTools.DrawHeader("Tweener_" + i))
            {
                GUI.changed = false;
                Vector3 from = EditorGUILayout.Vector3Field("From", pTarget.listTweenInfo[i].vecFrom);
                Vector3 to = EditorGUILayout.Vector3Field("To", pTarget.listTweenInfo[i].vecTo);

				if (GUI.changed)
                {
                    NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
                    pTarget.listTweenInfo[i].vecFrom = from;
                    pTarget.listTweenInfo[i].vecTo = to;

					NGUITools.SetDirty(pTarget);
                    GUI.changed = false;
                }

                EventDrawCommonProperties(this, pTarget.listTweenInfo[i]);
            }
        }
    }
}
