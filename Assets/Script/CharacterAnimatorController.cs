using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public abstract class CharacterAnimatorController : MonoBehaviour {
        public void Awake() {
            animator=gameObject.GetComponent<Animator>();
        }

        [SerializeField]
        protected Animator animator;
    }
}