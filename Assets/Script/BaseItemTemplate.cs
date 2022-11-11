using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace Nagopia {
    public abstract class BaseItemTemplate:SerializedScriptableObject {

        [HideInInspector]
        public GameDataBase.ItemRarity ItemRarity { get { return this.rarity; } }

        [NonSerialized,OdinSerialize,ShowInInspector]
        [BoxGroup("Base",showLabel:false)]
        [HorizontalGroup("Base/enum")]
        [LabelText("物品珍稀度")]
        protected GameDataBase.ItemRarity rarity;

        [HideInInspector]
        public GameDataBase.ItemType ITEMTYPE { get { return this.itemType; } }

        [NonSerialized,OdinSerialize,ShowInInspector]
        [DisplayAsString]
        [BoxGroup("Base", showLabel: false)]
        [HorizontalGroup("Base/enum")]
        [LabelText("物品类型")]
        protected GameDataBase.ItemType itemType = GameDataBase.ItemType.INVALID;

        public virtual bool ValidateProf(GameDataBase.CharacterProfession prof) { return true; }
    }
}
