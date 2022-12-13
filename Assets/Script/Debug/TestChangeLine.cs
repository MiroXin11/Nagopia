using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChangeLine : MonoBehaviour
{
    [Button]
    public void SetText(string text) {
        textMesh.Text = text;
        textMesh.text = textMesh.text.Replace("\\n", "\n");
    }
    public SuperTextMesh textMesh;
}
