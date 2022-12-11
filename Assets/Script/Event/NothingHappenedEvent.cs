using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class NothingHappenedEvent : BaseEvent {
        public NothingHappenedEvent() {
            this.eventType = GameDataBase.EventType.NOTHINGHAPPENED;
        }

        public NothingHappenedEvent(float waitTime) {
            this.waitTime = waitTime;
        }

        public float waitTime=2f;
        
        public override string ToString() {
            return $"什么事都没有发生";
        }
    }
}