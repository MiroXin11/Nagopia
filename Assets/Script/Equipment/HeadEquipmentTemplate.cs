using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagopia {
    public sealed class HeadEquipmentTemplate : EquipmentTemplate {
        public HeadEquipmentTemplate() {
            this.equipmentType = GameDataBase.EquipmentType.HEAD;
        }
    }
}

