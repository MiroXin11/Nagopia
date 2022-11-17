using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class ArcherAnimationControl : CharacterAnimatorController {
        public override void Attack(System.Action onCompleteCallback = null) {
            animator.Play("Shooting");
            this.AttackEndCallback= onCompleteCallback;
        }
    }
}

