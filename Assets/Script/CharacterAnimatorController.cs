using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

namespace Nagopia {
    public class CharacterAnimatorController : MonoBehaviour {
        public void Awake() {
            animator=gameObject.GetComponent<Animator>();
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
        public virtual void Attack(Action onCompleCallback=null) { }

        public void OnAttackEnd() {
            ResetAnimation();
            AttackEndCallback?.Invoke();
        }

        [Button]
        public virtual void Hurt(Action onCompledCallback=null) {
            animator.Play("Hurt");
            HurtEndCallback=onCompledCallback;
            //HurtEndCallback = delegate { Debug.Log("test"); };
        }

        protected virtual void OnHurtEnd() {
            ResetAnimation();
            HurtEndCallback?.Invoke();
            //Debug.Log("message");
        }

        protected Action AttackEndCallback = null;

        protected Action HurtEndCallback= null;

        [SerializeField]
        protected Animator animator;
    }
}