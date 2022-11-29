using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia {
    public class HeadEquipment : Equipment {
        public HeadEquipment(ref ProfPair[] profPairs, Sprite sprite, GameDataBase.ItemRarity rarity = GameDataBase.ItemRarity.RANDOM) : base(ref profPairs, sprite, rarity) {
            this.equipmentType = GameDataBase.EquipmentType.HEAD;
        }
    }
}

