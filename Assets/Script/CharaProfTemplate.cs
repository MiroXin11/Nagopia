using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
namespace Nagopia {
    public class CharaProfTemplate : SerializedScriptableObject {

        [NonSerialized, OdinSerialize]
        [ShowInInspector]
        [BoxGroup("Base")]
        [HorizontalGroup("Base/a")]
        [OnValueChanged("CheckEquipment")]
        private GameDataBase.CharacterProfession adaptProf;

        [HideInInspector]
        public GameDataBase.CharacterProfession AdaptProf => adaptProf;

        [NonSerialized, OdinSerialize]
        [ShowInInspector]
        [BoxGroup("Base")]
        [HorizontalGroup("Base/a")]
        [PropertyRange(0.0f, 1.00f)]
        [Tooltip("出现的几率")]
        private float probability;

        [HideInInspector]
        public float Probability => probability;

        [Tooltip("角色可能出现在的关卡阶段")]
        [NonSerialized,OdinSerialize]
        [BoxGroup("Base")]
        public MinMaxPair<int> ShowUPStage;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("Base")]
        [HorizontalGroup("Base/b")]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public GameObject prefab;

        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        [BoxGroup("Base")]
        [HorizontalGroup("Base/b")]
        [PreviewField(Alignment =ObjectFieldAlignment.Left)]
        public Sprite HeadImage;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("Base")]
        [InlineProperty]
        [LabelWidth(120)]
        public MinMaxPair<int> PossiblePosition;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("HP Curve")]
        private RandomRangeCurve hp;

        [HideInInspector]
        public RandomRangeCurve HP => hp;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("ATK Curve")]
        private RandomRangeCurve atk;

        [HideInInspector]
        public RandomRangeCurve ATK => atk;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("DEF Curve")]
        private RandomRangeCurve def;

        [HideInInspector]
        public RandomRangeCurve DEF => def;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("SPE Curve")]
        private RandomRangeCurve spe;

        [HideInInspector]
        public RandomRangeCurve SPE => spe;

        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        [BoxGroup("Mental")]
        [Tooltip("精神属性的范围，如果未添加在该字典的精神属性，则会按照GameConfig中设置的最大值最小值范围来生成。添加在该字典的精神属性则会按照所设定的范围来生成")]
        public Dictionary<GameDataBase.MentalType, MinMaxPair<byte>> MentalRange = new Dictionary<GameDataBase.MentalType, MinMaxPair<byte>>();

        [Tooltip("精神补正的集合，在生成初步的值后再通过该值进一步修正。该值为乘数，相乘后不会超出GameConfig中设定的最大值最小值范围")]
        [BoxGroup("Mental")]
        [NonSerialized,OdinSerialize]
        public List<MentalPairRevise> Revises = new List<MentalPairRevise>();

        [NonSerialized, OdinSerialize]
        [ShowInInspector]
        [BoxGroup("Equipment")]
        [ListDrawerSettings(AlwaysAddDefaultValue = false, CustomAddFunction = "AddNewHead")]
        public List<ProbableEquipmentPair<HeadEquipmentTemplate>> AvalibleHead = new List<ProbableEquipmentPair<HeadEquipmentTemplate>>();

        [NonSerialized, OdinSerialize]
        [ShowInInspector]
        [BoxGroup("Equipment")]
        [ListDrawerSettings(AlwaysAddDefaultValue = false, CustomAddFunction = "AddNewCloth")]
        public List<ProbableEquipmentPair<ClothEquipmentTemplate>> AvalibleCloth = new List<ProbableEquipmentPair<ClothEquipmentTemplate>>();

        [NonSerialized, OdinSerialize]
        [ShowInInspector]
        [BoxGroup("Equipment")]
        [ListDrawerSettings(AlwaysAddDefaultValue = false, CustomAddFunction = "AddNewShoes")]
        public List<ProbableEquipmentPair<ShoesEquipmentTemplate>> AvalibleShoes = new List<ProbableEquipmentPair<ShoesEquipmentTemplate>>();

        [NonSerialized, OdinSerialize]
        [ShowInInspector]
        [BoxGroup("Equipment")]
        [ListDrawerSettings(AlwaysAddDefaultValue = false, CustomAddFunction = "AddNewWeapon")]
        public List<ProbableEquipmentPair<WeaponEquipmentTemplate>> AvalibleWeapon = new List<ProbableEquipmentPair<WeaponEquipmentTemplate>>();

        private void CheckEquipment() {
            this.AvalibleHead.RemoveAll((item) => !item.Equipment.ValidateProf(this.AdaptProf));
            this.AvalibleCloth.RemoveAll((item) => !item.Equipment.ValidateProf(this.AdaptProf));
            this.AvalibleShoes.RemoveAll((item) => !item.Equipment.ValidateProf(this.AdaptProf));
            this.AvalibleWeapon.RemoveAll((item) => !item.Equipment.ValidateProf(this.AdaptProf));
        }

        public bool CanAppear(int stage) {
            return ShowUPStage.min<=stage&&stage<=ShowUPStage.max;
        }

        private ProbableEquipmentPair<HeadEquipmentTemplate> AddNewHead() { var res=new ProbableEquipmentPair<HeadEquipmentTemplate>();res.SetTemplate(this); return res; }
        private ProbableEquipmentPair<ClothEquipmentTemplate> AddNewCloth() { var res = new ProbableEquipmentPair<ClothEquipmentTemplate>(); res.SetTemplate(this); return res; }
        private ProbableEquipmentPair<ShoesEquipmentTemplate> AddNewShoes() { var res = new ProbableEquipmentPair<ShoesEquipmentTemplate>(); res.SetTemplate(this); return res; }
        private ProbableEquipmentPair<WeaponEquipmentTemplate> AddNewWeapon() { var res = new ProbableEquipmentPair<WeaponEquipmentTemplate>(); res.SetTemplate(this); return res; }
    }
    public struct MentalPairRevise {
        [HorizontalGroup("Group")]
        [LabelWidth(50)]
        [Tooltip("补正的类型")]
        public GameDataBase.MentalType mental;

        [HorizontalGroup("Group")]
        [PropertyRange(0,20)]
        [LabelWidth(50)]
        [Tooltip("补正的倍率")]
        public float revise;

        [HorizontalGroup("Group")]
        [PropertyRange(0,1.0f)]
        [LabelWidth(50)]
        [Tooltip("补正出现的概率")]
        public float probability;
    }
    public struct RandomRangeCurve {

        [HideInInspector]
        public MinMaxPair<uint> MinRange => minRange;

        [HideInInspector]
        public MinMaxPair<uint> MaxRange => maxRange;

        [HideInInspector]
        public MinMaxPair<uint> InflectRange => inflectRange;

        [HideInInspector]
        public MinMaxPair<float> SigmaRange => sigmaRange;
        
        [NonSerialized,OdinSerialize]
        [ShowInInspector]
        [InlineProperty(LabelWidth = 30)]
        [Tooltip("最小值的生成范围")]
        private MinMaxPair<uint> minRange;

        [NonSerialized,OdinSerialize]
        [ShowInInspector]
        [InlineProperty(LabelWidth =30)]
        [Tooltip("最大值的生成范围")]
        private MinMaxPair<uint> maxRange;

        [NonSerialized,OdinSerialize]
        [ShowInInspector]
        [InlineProperty(LabelWidth = 30)]
        [Tooltip("函数曲线的拐点范围")]
        private MinMaxPair<uint> inflectRange;

        [NonSerialized,OdinSerialize]
        [ShowInInspector]
        [InlineProperty(LabelWidth = 30)]
        [Tooltip("曲线抖度的范围")]
        private MinMaxPair<float> sigmaRange;
    }

    public struct ProbableEquipmentPair<T>where T : EquipmentTemplate {
        [HideInInspector]
        [NonSerialized,OdinSerialize]
        private CharaProfTemplate template;

        [HorizontalGroup("Group")]
        [AssetsOnly]
        [AssetSelector(FlattenTreeView =true)]
        [LabelWidth(80f)]
        [OnValueChanged("validate")]
        public T Equipment;

        [PropertyRange(0.0,1.00)]
        [HorizontalGroup("Group")]
        [LabelWidth(80f)]
        public float probability;

        public void validate() {
            if (ReferenceEquals(template, null)) {
                return;
            }
            if (!Equipment.ValidateProf(template.AdaptProf)) {
                Debug.Log("You may select a equipment not fit current profession");
                this.Equipment = null;
            }
        }

        public void SetTemplate(CharaProfTemplate template) {
            this.template = template;
        }
    }

    public struct MinMaxPair<T> where T : IComparable {

        public MinMaxPair(T min,T max) {
            this.min = min;
            this.max = max;
        }

        [HorizontalGroup("pair")]
        [OnValueChanged("validate")]
        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        public T min;

        [HorizontalGroup("pair")]
        [OnValueChanged("validate")]
        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        public T max;

#if UNITY_EDITOR
        public void validate() {
            if (max.CompareTo(min) < 0) {//此时max小于min
                max = min;
            }
        }
#endif
    }
    public struct EquipmentList {
        public EquipmentTemplate head;

        public EquipmentTemplate cloth;

        public EquipmentTemplate shoes;

        public EquipmentTemplate weapon;
    }
}
