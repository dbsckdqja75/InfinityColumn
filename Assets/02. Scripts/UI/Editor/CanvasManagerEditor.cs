using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.ReorderableList;

[CustomEditor(typeof(CanvasManager))]
public class CanvasManagerEditor : Editor
{

    bool panelFoldOut = true;
    bool panelSetFoldout = true;
    List<bool> foldoutList = new List<bool>();

    Panel PanelListItem(Rect rect, Panel panel)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), panel ? panel.gameObject.name : "NULL");
        return (Panel)EditorGUI.ObjectField(new Rect((rect.x + rect.width) * 0.3f, rect.y, (rect.x + rect.width) * 0.7f, EditorGUIUtility.singleLineHeight), panel, typeof(Panel), true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CanvasManager cm = (CanvasManager)target;

        int panelCount = cm.panelList.Count;

        EditorGUI.BeginChangeCheck();

        if (panelFoldOut = EditorGUILayout.Foldout(panelFoldOut, "Panel List"))
        {
            ReorderableListGUI.ListField(cm.panelList, PanelListItem, null);
        }

        int panelSetCount = cm.panelSetList.Count;
        if (panelSetCount > 0)
        {
            while (foldoutList.Count != panelSetCount)
            {
                if (foldoutList.Count < panelSetCount)
                {
                    foldoutList.Add(true);
                }
                else
                {
                    foldoutList.RemoveAt(foldoutList.Count - 1);
                }
            }
        }

        EditorGUILayout.Space(20);
        if(panelSetFoldout = EditorGUILayout.Foldout(panelSetFoldout, "PanelSet List"))
        {
            using (new EditorGUILayout.VerticalScope("GroupBox"))
            {
                for (int i = 0; i < panelSetCount; i++)
                {
                    while (cm.panelSetList[i].options.Count != panelCount)
                    {
                        if (cm.panelSetList[i].options.Count < panelCount)
                        {
                            cm.panelSetList[i].options.Add(new CanvasManager.PanelOption());
                        }
                        else
                        {
                            cm.panelSetList[i].options.RemoveAt(cm.panelSetList[i].options.Count - 1);
                        }
                    }

                    using (new EditorGUILayout.VerticalScope("HelpBox"))
                    {
                        if (foldoutList[i] = EditorGUILayout.Foldout(foldoutList[i], cm.panelSetList[i].NAME))
                        {
                            EditorGUILayout.Space(10);

                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("PanelSet Name", GUILayout.MaxWidth(120));
                                cm.panelSetList[i].NAME = EditorGUILayout.TextField(cm.panelSetList[i].NAME, GUILayout.MaxWidth(230));
                            }

                            using (new EditorGUILayout.HorizontalScope("GroupBox"))
                            {
                                EditorGUILayout.LabelField("Panel Name", GUILayout.MaxWidth(120));
                                EditorGUILayout.LabelField("Active", GUILayout.MaxWidth(100));
                                EditorGUILayout.LabelField("Ignore", GUILayout.MaxWidth(100));
                            }

                            for (int j = 0; j < panelCount; j++)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.LabelField(cm.panelList[j] ? cm.panelList[j].gameObject.name : "Null", GUILayout.MaxWidth(145));

                                    cm.panelSetList[i].options[j].isActive = 
                                        EditorGUILayout.Toggle(cm.panelSetList[i].options[j].isActive, GUILayout.Width(100));
                                    cm.panelSetList[i].options[j].isIgnore = 
                                        EditorGUILayout.Toggle(cm.panelSetList[i].options[j].isIgnore, GUILayout.Width(100));
                                }
                            }

                            EditorGUILayout.Space(20);
                            if (GUILayout.Button("Remove PanelSet"))
                            {
                                cm.panelSetList.RemoveAt(i);
                                return;
                            }
                        }
                    }

                    EditorGUILayout.Space(10);
                }

                EditorGUILayout.Space(20);
                if (GUILayout.Button("Add PanelSet"))
                {
                    cm.panelSetList.Add(new CanvasManager.PanelSet());
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(cm);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
