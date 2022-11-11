using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.IO;

public class AssetCreate<T> where T : ScriptableObject {
    bool abstact;
    public AssetCreate() {
        abstact = typeof(T).IsAbstract;
        if(!abstact)
        this.data = ScriptableObject.CreateInstance<T>();
    }

    public void CreateData(string Name,string Folder) {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Folder)) {
            return;
        }
        string FolderPath = Folder + Name + ".asset";
        if (File.Exists(FolderPath)) {
            Debug.Log("File already exists");
            return;
        }
        AssetDatabase.CreateAsset(data, FolderPath);
        AssetDatabase.SaveAssets();
        if (!abstact)
            this.data = ScriptableObject.CreateInstance<T>();
        else
            this.data = null;
        Name = string.Empty;
    }

    public void CreateChildData<S>(string name,string Folder)where S:T {
        this.data = ScriptableObject.CreateInstance<S>();
        CreateData(name, Folder);
    }

    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    public T data;
}
