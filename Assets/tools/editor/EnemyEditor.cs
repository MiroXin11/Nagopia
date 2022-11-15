using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nagopia;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class EnemyEditor : OdinMenuEditorWindow
{
    [MenuItem("Tools/Editors/Enemy Window")]
    private static void OpenWindow() {
        OdinMenuEditorWindow.GetWindow<EnemyEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree() {
        OdinMenuTree tree = new OdinMenuTree();
        tree.Config.DrawSearchToolbar = true;
        tree.AddAllAssetsAtPath("Enemy", FolderPath, typeof(EnemyTemplate),true,true);
        return tree;
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (ReferenceEquals(null, this.AssetCreator)) {
            this.AssetCreator = new AssetCreate<EnemyTemplate>();
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (AssetCreator != null) {
            if (AssetCreator.data != null) {
                DestroyImmediate(AssetCreator.data);
            }
        }
        AssetDatabase.SaveAssets();
    }

    protected override void OnBeginDrawEditors() {
        base.OnBeginDrawEditors();
        SirenixEditorGUI.BeginHorizontalToolbar(this.MenuTree.Config.SearchToolbarHeight);
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(200f), GUILayout.ExpandWidth(false));
            {
                GUILayout.Label("New");
                Rect rect = EditorGUILayout.GetControlRect();
                newItemName = SirenixEditorFields.DelayedTextField(rect, newItemName);
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Play)) {
                    if (!string.IsNullOrWhiteSpace(newItemName)) {
                        CreateItem();
                    }
                }
                EditorGUILayout.Space(5f);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200f), GUILayout.ExpandWidth(false));
            {
                GUILayout.Label("Rename", GUILayout.MinWidth(60f));
                Rect rect = EditorGUILayout.GetControlRect();
                rect = EditorGUI.PrefixLabel(rect, new GUIContent(""));
                rename = SirenixEditorFields.DelayedTextField(rect, rename);
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Play)) {
                    if (!string.IsNullOrWhiteSpace(rename)) {
                        RenameItem();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            {
                if (SirenixEditorGUI.ToolbarButton("Delete")) {
                    DeleteItem();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    private void CreateItem() {
        AssetCreator.CreateData(newItemName, FolderPath);
    }

    private void RenameItem() {
        if (this.MenuTree.Selection.SelectedValue == null)
            return;
        var t = this.MenuTree.Selection.SelectedValue as ScriptableObject;
        var temp = AssetDatabase.GetAssetPath(t);
        AssetDatabase.RenameAsset(temp, this.rename);
        AssetDatabase.SaveAssets();
    }

    private void DeleteItem() {
        if (this.MenuTree.Selection.SelectedValue == null)
            return;
        var t = this.MenuTree.Selection.SelectedValue as ScriptableObject;
        string path = AssetDatabase.GetAssetPath(t);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
    }

    public const string FolderPath = "Assets/Resources_moved/Enemy Template/";
    string rename = string.Empty;
    string newItemName = string.Empty;
    private AssetCreate<EnemyTemplate> AssetCreator;
}
