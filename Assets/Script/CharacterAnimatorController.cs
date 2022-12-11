using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nagopia {
    public class CharacterAnimatorController : MonoBehaviour {
        public void Awake() {
            animator = gameObject.GetComponent<Animator>();
            this.spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
        }

        [Button]
        public virtual void StartWalking() {
            animator.Play("Walking");
        }

        [Button]
        public virtual void ResetAnimation() {
            animator.Play("Idle");
        }

        [Button]
        public virtual void Attack(Action onCompleteCallback = null) { }

        public void OnAttackEnd() {
            ResetAnimation();
            AttackEndCallback?.Invoke();
        }

        [Button]
        public virtual void Hurt(Action onCompleteCallback = null) {
            animator.Play("Hurt");
            HurtEndCallback = onCompleteCallback;
            //HurtEndCallback = delegate { Debug.Log("test"); };
        }

        protected virtual void OnHurtEnd() {
            ResetAnimation();
            HurtEndCallback?.Invoke();
            //Debug.Log("message");
        }

        [Button]
        public virtual void Die(Action CompleteCallback = null) {
            animator.Play("Dying");
            DieEndCallback = CompleteCallback;
        }

        protected virtual void DieEnd() {
            DieEndCallback?.Invoke();
        }

        [Button]
        public virtual void Fade(float endValue = 0f,float time = 0.5f,System.Action completeCallback=null) {
            foreach (var item in spriteRenderers) {
                item.DOFade(endValue, time).SetEase(Ease.OutExpo).OnComplete(()=>completeCallback?.Invoke());
            }
        }

        protected Action AttackEndCallback = null;

        protected Action HurtEndCallback= null;

        protected Action DieEndCallback= null;

        [SerializeField]
        protected Animator animator;

        private SpriteRenderer[] spriteRenderers;
        //private List<SpriteRenderer>spriteRenderers= new List<SpriteRenderer>();
    }
}