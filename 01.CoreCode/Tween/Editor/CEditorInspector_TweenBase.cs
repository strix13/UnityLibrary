﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(CTweenBase), true)]
public class CEditorInspector_TweenBase : Editor
{
    static List<CTweenBase> g_listTweenTestPlay = new List<CTweenBase>();

    public override void OnInspectorGUI()
    {
        CTweenBase.g_bIsDrawGizmo = GUILayout.Toggle(CTweenBase.g_bIsDrawGizmo, "  기즈모를 그릴지");

        base.OnInspectorGUI();

        CTweenBase pTarget = target as CTweenBase;

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Start에 현재 값을 대입"))
        {
            EditorGUI.BeginChangeCheck();
            pTarget.DoSetTarget(pTarget.p_pObjectTarget);
            pTarget.OnEditorButtonClick_SetStartValue_IsCurrentValue();
            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(target, "OnEditorButtonClick_SetStartValue_IsCurrentValue");
        }
        if (GUILayout.Button("Dest에 현재 값을 대입"))
        {
            EditorGUI.BeginChangeCheck();
            pTarget.DoSetTarget(pTarget.p_pObjectTarget);
            pTarget.OnEditorButtonClick_SetDestValue_IsCurrentValue();
            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(target, "OnEditorButtonClick_SetDestValue_IsCurrentValue");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (g_listTweenTestPlay.Contains(pTarget))
        {
            if (GUILayout.Button("테스트 중지(이 트윈만)"))
            {
                g_listTweenTestPlay.Remove(pTarget);
                pTarget.OnReleaseTween_EditorOnly();
            }
            if (GUILayout.Button("테스트 중지(트윈 전체)"))
            {
                Clear_TestPlay();
            }
        }
        else
        {
            if (GUILayout.Button("테스트 플레이(이 트윈만)"))
            {
                Add_TweenTestPlay(pTarget);
            }
            if (GUILayout.Button("테스트 플레이(트윈 전체)"))
            {
                CTweenBase[] arrComponent = pTarget.GetComponents<CTweenBase>();
                foreach (var pTargetOther in arrComponent)
                    Add_TweenTestPlay(pTargetOther);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("현재 값에 Start값을 대입"))
        {
            EditorGUI.BeginChangeCheck();
            pTarget.DoSetTarget(pTarget.p_pObjectTarget);
            pTarget.OnEditorButtonClick_SetCurrentValue_IsStartValue();
            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(target, "OnEditorButtonClick_SetStartValue_IsCurrentValue");
        }
        if (GUILayout.Button("현재 값에 Dest값을 대입"))
        {
            EditorGUI.BeginChangeCheck();
            pTarget.DoSetTarget(pTarget.p_pObjectTarget);
            pTarget.OnEditorButtonClick_SetCurrentValue_IsDestValue();
            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(target, "OnEditorButtonClick_SetDestValue_IsCurrentValue");
        }
        EditorGUILayout.EndHorizontal();


    }

    private void OnEnable()
    {
        EditorApplication.update += Update;
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
        Clear_TestPlay();
    }


    private void Update()
    {
        foreach (var pTweenTestPlay in g_listTweenTestPlay)
        {
            pTweenTestPlay.DoSetTweening(Time.deltaTime);

            CTweenPosition pTweenPos = pTweenTestPlay as CTweenPosition;
            if (pTweenPos)
            {
                Vector3 vecPos = (Vector3)pTweenTestPlay.OnTween_EditorOnly(pTweenTestPlay.p_fProgress_0_1);
                if (pTweenPos.p_bIsLocal)
                    pTweenPos.transform.localPosition = vecPos;
                else
                    pTweenPos.transform.position = vecPos;
            }

            //CTweenRotation pTweenRot = pTweenTestPlay as CTweenRotation;
            //if (pTweenRot)
            //{
            //    //Vector3 vecPos = (Vector3)pTweenTestPlay.OnTween_EditorOnly(pTweenTestPlay.p_fProgress_0_1);
            //    //if (pTweenPos.p_bIsLocal)
            //    //    pTweenPos.transform.localPosition = vecPos;
            //    //else
            //    //    pTweenPos.transform.position = vecPos;
            //}

            EditorUtility.SetDirty(pTweenTestPlay);
        }
    }

    private void Add_TweenTestPlay(CTweenBase pTween)
    {
        if (g_listTweenTestPlay.Contains(pTween) == false)
        {
            g_listTweenTestPlay.Add(pTween);
            pTween.DoSetTarget(pTween.p_pObjectTarget);
            pTween.DoInitTween(CTweenBase.ETweenDirection.Forward, true);
            pTween.OnInitTween_EditorOnly();
        }
    }

    private void Clear_TestPlay()
    {
        foreach (var pTweenTestPlay in g_listTweenTestPlay)
        {
            pTweenTestPlay.OnReleaseTween_EditorOnly();
            EditorUtility.SetDirty(pTweenTestPlay);
        }

        g_listTweenTestPlay.Clear();
    }
}
