using System;
namespace Nagopia {
    public class KnightAnimatorController:CharacterAnimatorController {
        public override void Attack(Action onCompleteCallback = null) {
            animator.Play("Attacking");
            this.AttackEndCallback= onCompleteCallback;
        }
    }
}