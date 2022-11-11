using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nagopia;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

public class CharaTemplateEditor : OdinMenuEditorWindow
{
    [MenuItem("Tools/Editors/Character Window")]
    private static void OpenWindow() {
        OdinMenuEditorWindow.GetWindow<CharaTemplateEditor>().Show();
    }
    protected override OdinMenuTree BuildMenuTree() {
        OdinMenuTree tree = new OdinMenuTree();
        tree.Config.DrawSearchToolbar = true;
        tree.AddAllAssetsAtPath("Character", FolderPath, typeof(CharaProfTemplate), true, true);
        return tree;
    }

    protected override void OnBeginDrawEditors() {
        base.OnBeginDrawEditors();

        SirenixEditorGUI.BeginHorizontalToolbar(this.MenuTree.Config.SearchToolbarHeight);
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(200f), GUILayout.ExpandWidth(false));
            {
                GUILayout.Label("New");
                Rect rect = EditorGUILayout.GetControlRect();
                rect = EditorGUI.PrefixLabel(rect, new GUIContent(""));
                newItemName = SirenixEditorFields.DelayedTextField(rect, newItemName);
                GUILayout.FlexibleSpace();
                if (SirenixEditorGUI.ToolbarButton(EditorIcons.Play)) {
                    if (!string.IsNullOrWhiteSpace(newItemName)) {
                        AssetCreator.CreateData(newItemName, FolderPath);
                    }
                }
                EditorGUILayout.Space(5f);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200f), GUILayout.ExpandWidth(false));
            {
                //EditorGUILayout.LabelField("Rename");
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

    protected override void OnEnable() {
        base.OnEnable();
        if (ReferenceEquals(null, this.AssetCreator)) {
            this.AssetCreator = new AssetCreate<CharaProfTemplate>();
        }
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (!ReferenceEquals(null, this.AssetCreator)) {
            if (ReferenceEquals(null, this.AssetCreator.data)) {
                UnityEngine.Object.DestroyImmediate(this.AssetCreator.data);
            }
        }
        AssetDatabase.SaveAssets();
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

    public const string FolderPath = "Assets/Resources_moved/Chara Templates/";
    private string rename = string.Empty;
    private string newItemName = string.Empty;
    private AssetCreate<CharaProfTemplate> AssetCreator;
}
