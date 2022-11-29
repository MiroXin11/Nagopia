using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class ClothEquipment : Equipment {
        public ClothEquipment(ref ProfPair[] profPairs, Sprite sprite, GameDataBase.ItemRarity rarity = GameDataBase.ItemRarity.RANDOM)
            : base(ref profPairs, sprite, rarity) {
            this.equipmentType = GameDataBase.EquipmentType.CLOTH;
        }
    }
}

