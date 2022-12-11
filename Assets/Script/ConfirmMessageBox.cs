using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Nagopia {
    [RequireComponent(typeof(CanvasGroup))]
    public class ConfirmMessageBox : MonoBehaviour {
        private void Awake() {
            this.m_transform = transform;
            this.canvasGroup = m_transform.GetComponent<CanvasGroup>();
        }

        public void ShowUp(float time=0.3f) {
            this.gameObject.SetActive(true);
            this.transform.localPosition = startPos;
            this.canvasGroup.alpha = 0.2f;
            canvasGroup.DOFade(1.0f, time);
            this.transform.DOLocalMove(endPos, time);
        }

        public void Hide(float time=0.3f) {
            confirm.onClick.RemoveAllListeners();
            cancel.onClick.RemoveAllListeners();
            canvasGroup.DOFade(0.0f, time).OnComplete(() => { DescriptionText = string.Empty;CancelText = DefaultCancelText;ConfirmText = DefaultConfirmText;this.gameObject.SetActive(false); });
            this.transform.DOLocalMove(startPos, time);
        }

        public void AddConfirmCallback(UnityAction callback) {
            confirm.onClick.AddListener(callback);
        }

        public void AddCancelCallback(UnityAction callback) {
            cancel.onClick.AddListener(callback);
        }

        public void RemoveConfirmEvent(UnityAction callback) {
            confirm.onClick.RemoveListener(callback);
        }

        public void RemoveCancelEvent(UnityAction callback) {
            cancel.onClick.RemoveListener(callback);
        }

        [SerializeField]
        public Button confirm;

        [SerializeField]
        public Button cancel;

        private CanvasGroup canvasGroup;

        public Transform m_transform;

        private Vector3 endPos = new Vector3(0,-100);

        private Vector3 startPos = new Vector3(0, 300);

        [SerializeField]
        private SuperTextMesh confirmText;

        public string ConfirmText { set=>confirmText.text= value; }

        [SerializeField]
        private SuperTextMesh cancelText;

        public string CancelText { set=>cancelText.text= value; }

        [SerializeField]
        private SuperTextMesh descriptionText;

        public string DescriptionText { set => descriptionText.text = value; }

        public static readonly string DefaultConfirmText="确认";

        public static readonly string DefaultCancelText="取消";
    }
}