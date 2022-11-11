using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class WeaponEquipment : Equipment {
        public WeaponEquipment(ref ProfPair[] profPairs, GameDataBase.ItemRarity rarity = GameDataBase.ItemRarity.RANDOM)
            :base(ref profPairs,rarity){
            this.equipmentType = GameDataBase.EquipmentType.WEAPON;
        }
    }
}

