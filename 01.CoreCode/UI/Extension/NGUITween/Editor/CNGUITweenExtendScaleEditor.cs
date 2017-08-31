using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CNGUITweenScaleExtend))]
public class CNGUITweenExtendScaleEditor : CNGUITweenExtendBaseEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(6f);
        NGUIEditorTools.SetLabelWidth(120f);

        CNGUITweenScaleExtend pTarget = target as CNGUITweenScaleExtend;

        GUI.changed = false;
        GUILayout.BeginHorizontal();
        int iGroupSizeNew = EditorGUILayout.IntField("TweenInfoCount", pTarget.listTweenInfo.Count, GUILayout.Width(170f));
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
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
