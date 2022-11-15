using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class TestRNG : MonoBehaviour
{
    [Button]
    public void TestAVE() {
        Dictionary<int, int> count = new Dictionary<int, int>();
        for(int i = 0; i <= 9; ++i) {
            count.Add(i, 0);
        }
        const int LoopTime = 10000;
        for(int i = 1; i <= LoopTime; ++i) {
            int t = Random.Range(0, 10);
            count[t]++;
        }
        foreach (var item in count) {
            Debug.Log($"{item.Key}:{item.Value}");
        }
    }
}
