using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class GameOverEvent : BaseEvent {
        public GameOverEvent() {
            this.eventType = GameDataBase.EventType.GAMELOSE;
        }
        public override string ToString() {
            return $"Game Over!";
        }
    }
}