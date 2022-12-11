using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
namespace Nagopia {

    public abstract class EquipmentTemplate : BaseItemTemplate {

        public EquipmentTemplate() {
            this.itemType = GameDataBase.ItemType.EQUIPMENT;
            var profs = System.Enum.GetValues(typeof(GameDataBase.CharacterProfession));
            int length = profs.Length;
            requirements = new ProfPair[length];
            for (int i = 0; i < length; ++i) {
                requirements[i].prof = (GameDataBase.CharacterProfession)profs.GetValue(i);
                requirements[i].available = false;
            }
        }

        public override bool ValidateProf(GameDataBase.CharacterProfession type) {
            return requirements[(int)type].available;
        }

        [HideInInspector]
        public GameDataBase.EquipmentType EquipmentType { get { return this.equipmentType; } }

        [System.NonSerialized, OdinSerialize, ShowInInspector]
        [DisplayAsString]
        [BoxGroup("Equipment",showLabel:false)]
        [HorizontalGroup("Equipment/a")]
        [LabelText("装备类型")]
        [Space()]
        protected GameDataBase.EquipmentType equipmentType = GameDataBase.EquipmentType.INVALID;

        [System.NonSerialized,OdinSerialize,ShowInInspector]
        [BoxGroup("Equipment",showLabel:false)]
        [LabelText("图像")]
        [PreviewField]
        [AssetSelector(Paths ="Assets/Resources_moved/icons/Equipments/")]
        public readonly Sprite image;

        [HideInInspector]
        public EquipmentAbility[] Abilities { get { return this.abilities.ToArray(); } }

        [System.NonSerialized,OdinSerialize,ShowInInspector]
        [BoxGroup("Equipment")]
        [LabelText("基础属性值")]
        [Space()]
        private List<EquipmentAbility> abilities=new List<EquipmentAbility>();

        [System.NonSerialized, OdinSerialize, ShowInInspector]
        [BoxGroup("Equipment")]
        [LabelText("适用的职业")]
        [Space()]
        [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        public ProfPair[] requirements;

        [HideInInspector]
        public RarityPair_Template[] AdditionalPair { get { return this.additional_abilities.ToArray(); } }

        [System.NonSerialized, OdinSerialize, ShowInInspector]
        [BoxGroup("Equipment")]
        [LabelText("可能出现的属性及该属性珍稀度")]
        [Tooltip("珍稀度越高，出现的概率也越低，具体的概率可以参见RandomNumberGenerator中的Happened(byte)")]
        [Space()]
        [ListDrawerSettings(DraggableItems =true)]
        private List<RarityPair_Template> additional_abilities = new List<RarityPair_Template>();

#if UNITY_EDITOR
        public void OnValidate() {

            //检查职业类型是否发生改变
            var profs = System.Enum.GetValues(typeof(GameDataBase.CharacterProfession));
            int length = profs.Length;
            int oldLength = this.requirements.Length;
            if (length != oldLength) {
                ProfPair[] newrequirement = new ProfPair[length];
                for(int i = 0; i < length; ++i) {
                    newrequirement[i].prof = (GameDataBase.CharacterProfession)profs.GetValue(i);
                    newrequirement[i].available = i < oldLength ? this.requirements[i].available : false;
                }
                this.requirements = newrequirement;
            }
        }
#endif
    }
    public struct EquipmentAbility {
        public EquipmentAbility(GameDataBase.AbilityType type, short values) {
            this.type = type;
            value = values;
        }

        [LabelText("数值类型")]
        [HorizontalGroup("Ability")]
        public readonly GameDataBase.AbilityType type;

        [OdinSerialize,ShowInInspector]
        [LabelText("数值")]
        [HorizontalGroup("Ability")]
        [PropertyTooltip("基准数值，可以理解为随着游戏进度发展，该数值的变化情况，同一游戏进度下该值越大，则武器基础数值越高")]
        public float Value { get { return this.value; } private set { this.value = value; } }

        [System.NonSerialized,OdinSerialize,HideInInspector]
        private float value;
    }

    public struct RarityPair_Template {

        [LabelText("属性类型")]
        [HorizontalGroup("Pair")]
        [LabelWidth(50)]
        public GameDataBase.AbilityType type;
        
        [LabelText("稀有度"),HorizontalGroup("Pair")]
        [Range(0,10)]
        [LabelWidth(50)]
        [Tooltip("当数值为0时，该词条必定会出现")]
        public byte rarity;

        [LabelText("数值"),HorizontalGroup("Pair")]
        [Indent()]
        [LabelWidth(50)]
        [Tooltip("基准数值，可以理解为随着游戏进度发展，该数值的变化情况，同一游戏进度下该值越大，则武器基础数值越高")]
        public float value;
    }

    public struct ProfPair {
        [LabelText("职业")]
        [HorizontalGroup("ProfPair")]
        [ReadOnly]
        public GameDataBase.CharacterProfession prof;

        [LabelText("可用")]
        [HorizontalGroup("ProfPair")]
        public bool available;
    }
}
