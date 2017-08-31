using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
public class CNGUITweenExtendBaseEditor : UITweenerEditor
{
    public override void OnInspectorGUI()
    {
    }

    protected void EventDrawCommonProperties(Object pTarget, STweenInfoBase sTweenInfoBase)
    {
        NGUIEditorTools.BeginContents();
        NGUIEditorTools.SetLabelWidth(110f);

        GUI.changed = false;

        UITweener.Style eStyle = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", sTweenInfoBase.eStyle);
        AnimationCurve pAnimationCurve = EditorGUILayout.CurveField("Animation Curve", sTweenInfoBase.pAnimationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
		//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

		GUILayout.BeginHorizontal();
        bool bPlayOnEnable = EditorGUILayout.ToggleLeft("PlayOnEnable", sTweenInfoBase.bStartOnEnable, GUILayout.Width(170f));
        GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		bool bAutoDisableThis = EditorGUILayout.ToggleLeft("AutoDisableThis", sTweenInfoBase.bAutoDisableThis, GUILayout.Width(170f));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		bool bAutoDisableFrame = EditorGUILayout.ToggleLeft("AutoDisableFrame", sTweenInfoBase.bAutoDisableFrame, GUILayout.Width(170f));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
        float fDuration = EditorGUILayout.FloatField("Duration", sTweenInfoBase.fDuration, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        float fStartDelay = EditorGUILayout.FloatField("Start Delay", sTweenInfoBase.fStartDelay, GUILayout.Width(170f));
        GUILayout.Label("seconds");
        GUILayout.EndHorizontal();
        
        if (GUI.changed)
        {
            NGUIEditorTools.RegisterUndo("Tween Change", pTarget);
            sTweenInfoBase.bStartOnEnable = bPlayOnEnable;
			sTweenInfoBase.bAutoDisableThis = bAutoDisableThis;
			sTweenInfoBase.bAutoDisableFrame = bAutoDisableFrame;
			sTweenInfoBase.pAnimationCurve = pAnimationCurve;
            sTweenInfoBase.eStyle = eStyle;
            sTweenInfoBase.fDuration = fDuration;
            sTweenInfoBase.fStartDelay = fStartDelay;
            NGUITools.SetDirty(pTarget);
            GUI.changed = false;
        }
        NGUIEditorTools.EndContents();

        NGUIEditorTools.DrawEvents("On Finished", pTarget, sTweenInfoBase.listOnFinished);
    }
}
