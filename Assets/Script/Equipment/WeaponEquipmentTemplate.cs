using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class WeaponEquipmentTemplate : EquipmentTemplate {
        public WeaponEquipmentTemplate() {
            this.equipmentType = GameDataBase.EquipmentType.WEAPON;
        }
    }
}

