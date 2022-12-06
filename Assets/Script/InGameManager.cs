using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Nagopia {
    public class InGameManager : SingletonMonobehaviour<InGameManager> {

        public void StartEvent() {

        }

        public void EndEvent() {

        }

        public void StartRest() {

        }

        public void EndRest() {

        }
        
        public byte currentStage { get; private set; }
    }

}

