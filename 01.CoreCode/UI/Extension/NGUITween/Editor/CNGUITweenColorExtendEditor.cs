using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CNGUITweenColorExtend))]
public class CNGUITweenColorExtendEditor : CNGUITweenExtendBaseEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(6f);
        NGUIEditorTools.SetLabelWidth(120f);

        CNGUITweenColorExtend pTarget = target as CNGUITweenColorExtend;

        GUI.changed = false;
        GUILayout.BeginHorizontal();
        int iGroupSizeNew = EditorGUILayout.IntField("TweenInfoCount", pTarget.listTweenInfo.Count, GUILayout.Width(170f));
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
                Color pColorFrom = EditorGUILayout.ColorField("From", pTarget.listTweenInfo[i].pColorFrom);
                Color pColorTo = EditorGUILayout.ColorField("From", pTarget.listTweenInfo[i].pColorTo);

                if (GUI.changed)
                {
                    NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
                    pTarget.listTweenInfo[i].pColorFrom = pColorFrom;
                    pTarget.listTweenInfo[i].pColorTo = pColorTo;
                    NGUITools.SetDirty(pTarget);
                }

                EventDrawCommonProperties(this, pTarget.listTweenInfo[i]);
            }
        }
    }
}
