using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nagopia;
using MEC;

public class ATBUpdateTest : MonoBehaviour
{
    [Sirenix.OdinInspector.Button]
    public void StartCoroutine() {
        Timing.RunCoroutine(Battle());
    }

    IEnumerator<float> Battle() {
        float updateFrequency = 1.0f / (GameDataBase.Config.BattleSysUpdateTimesPerSec*1.0f);
        float updatePerSec = GameDataBase.Config.ATBUpPerSec;
        float ATB = 0.0f;
        float time = 0.0f;
        int times = 1;
        double Precision = 0.00001;
        while (true) {
            ATB = ATB + updateFrequency * updatePerSec;
            time += updateFrequency;
            if ((1.0f-time)<=Precision) {
                Debug.Log($"Wanted loop times:{GameDataBase.Config.BattleSysUpdateTimesPerSec}");
                Debug.Log($"Reality times:{times}");
                Debug.Log($"Wanted ATB:{updatePerSec}");
                Debug.Log($"{time}s: {ATB}");
                yield break;
            }
            ++times;
            yield return Timing.WaitForSeconds(updateFrequency);
        }
    }
}
