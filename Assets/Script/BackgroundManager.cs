using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    private void Awake() {
        piece1Trans = piece1.transform;
        piece2Trans = piece2.transform;
        StartPos = piece2Trans.localPosition;
        EndPos = -StartPos;
    }

    [Button]
    public void MovePiece(float time=0.5f,System.Action completeCallback = null) {
        if (firstFlag) {
            piece1Trans.DOLocalMoveX(EndPos.x, time).SetEase(Ease.InOutQuad).OnComplete(() => piece1Trans.localPosition = StartPos);
            piece2Trans.DOLocalMoveX(0f, time).SetEase(Ease.InOutQuad).OnComplete(() => completeCallback?.Invoke());
        }
        else {
            piece2Trans.DOLocalMoveX(EndPos.x, time).SetEase(Ease.InOutQuad).OnComplete(() => piece2Trans.localPosition = StartPos);
            piece1Trans.DOLocalMoveX(0f,time).SetEase(Ease.InOutQuad).OnComplete(()=>completeCallback?.Invoke());
        }
        firstFlag = !firstFlag;
    }
    bool firstFlag = true;
    public GameObject piece1;
    Transform piece1Trans;
    public GameObject piece2;
    Transform piece2Trans;
    Vector3 StartPos;
    Vector3 EndPos;
}
