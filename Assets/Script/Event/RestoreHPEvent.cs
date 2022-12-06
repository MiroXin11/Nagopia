using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class RestoreHPEvent : BaseEvent {
        public RestoreHPEvent() {

        }

        public RestoreHPEvent(float rate, bool isRate) {
            IsRate = isRate;
            if(IsRate)
            this.rate = Mathf.Clamp(rate,0f,1.0f);
        }

        [HideInInspector]
        public float Rate => rate;

        [SerializeField]
        [OnValueChanged("Validate")]
        private float rate;

        public bool IsRate;

        public override string ToString() {
            string str = IsRate ? $"{rate*100}%" : $"{rate}点";
            return $"队伍的HP恢复了{str}";
        }

        public void Validate() {
            if (IsRate) {
                rate=Mathf.Clamp(rate,0f,1.0f);
            }
        }
    }
}