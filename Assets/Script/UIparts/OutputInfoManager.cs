using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputInfoManager : MonoBehaviour
{
    [Button]
    public void AddNewInfo(string text) {
        int count = textMeshes.Count;
        if (count >= 20) {
            var t = textMeshes[0];
            textMeshes.RemoveAt(0);
            GameObject.DestroyImmediate(t);
        }
        GameObject obj = GameObject.Instantiate(prefab, m_transform);
        var textmesh = obj.GetComponentInChildren<Text>();
        textmesh.text = text;
        textMeshes.Add(textmesh);
        obj.transform.SetAsFirstSibling();
    }

    public List<Text>textMeshes= new List<Text>();

    public GameObject prefab;

    public Transform m_transform;

    public Scrollbar scrollbar;
}
