using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
using UnityEditor;

namespace Nagopia {
    
    public class GameDataBase {
        static bool hasIni = false;

        /// <summary>
        /// 可能会有几段代码在MonoBehaviour的Awake中调用，但实际游戏中会自动运行
        /// </summary>
       [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize() {
            if (hasIni)
                return;
            InitializeConfig();
            InitializeItem();
            InitializeProfTemplate();
            hasIni = true;
        }
#if UNITY_EDITOR
        [InitializeOnLoadMethod()]
#endif
        private static void InitializeConfig() {
#if UNITY_EDITOR
            if (ReferenceEquals(null, config)) {
                config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/Resources_moved/Config/DefaultConfig.asset");
                //Debug.Log("Load Config");
            }
#else
            var handle=Addressables.LoadAsset<GameConfig>("GameConfig");
            handle.WaitForCompletion();
            config=handle.Result;
#endif
        }

        private static void InitializeProfTemplate() {
            //var array = Enum.GetValues(typeof(GameDataBase.CharacterProfession));
            var array = Enum.GetValues(typeof(GameDataBase.CharacterProfession));
            foreach (var item in array) {
                CharaTemplates.Add((GameDataBase.CharacterProfession)item, new List<CharaProfTemplate>());
            }
            GetAllCharaTemplate();
        }

        private static void InitializeItem() {
            var array = Enum.GetValues(typeof(GameDataBase.ItemType));
            foreach (var item in array) {
                ItemTemplates.Add((GameDataBase.ItemType)item, new List<BaseItemTemplate>());
            }
            GetAllEquipmentTemplate();
        }

        /// <summary>
        /// 需要在运行模式下才能正常执行
        /// </summary>
        private static void GetAllEquipmentTemplate() {
            var handle = Addressables.LoadAssetsAsync<EquipmentTemplate>("EquipmentTemplate", null);
            handle.WaitForCompletion();
            var list = ItemTemplates[ItemType.EQUIPMENT];
            foreach (var item in handle.Result) {
                list.Add(item);
            }
        }

        /// <summary>
        /// 需要在运行模式下才能正常执行
        /// </summary>
        private static void GetAllCharaTemplate() {
            var handle = Addressables.LoadAssetsAsync<CharaProfTemplate>("CharacterTemplate", null);
            handle.WaitForCompletion();
            var res = handle.Result;
            foreach (var item in res) {
                CharaTemplates[item.AdaptProf].Add(item);
            }
        }

        public static CharaProfTemplate GetCharaTemplate(GameDataBase.CharacterProfession[]requiredProfs) {
            List<CharaProfTemplate> wanted = new List<CharaProfTemplate>();
            foreach (var item in requiredProfs) {
                wanted.AddRange(CharaTemplates[item]);
            }
            wanted=wanted.OrderByDescending((item) => item.Probability).ToList();
            foreach (var item in wanted) {
                if (RandomNumberGenerator.Happened(item.Probability)) {
                    return item;
                }
            }
            return wanted[0];
        }

        public static EquipmentTemplate GetEquipmentTemplate(GameDataBase.ItemRarity rarity, GameDataBase.CharacterProfession[] usable, GameDataBase.EquipmentType type) {
            var EquipmentList = ItemTemplates[ItemType.EQUIPMENT];
            var final = EquipmentList.Where((template) => { return template.ItemRarity == rarity; })
                        .Where((template) => {
                            bool temp = true;
                            int length = usable.Length;
                            for (int i = 0; i < length && temp; ++i) {
                                temp = temp && template.ValidateProf(usable[i]);
                            }
                            return temp;
                        })
                        .Where((template) => { return type == (template as EquipmentTemplate).EquipmentType; });
            int index = 0;
            int count = final.Count();
            if (count > 1) {
                index = RandomNumberGenerator.Average_GetRandomNumber(0, count);
            }
            return final.ElementAt(index) as EquipmentTemplate;
        }

        public enum CharacterProfession {
            [InspectorName("骑士")]
            KNIGHT,

            [InspectorName("游侠")]
            RANGER,

            [InspectorName("牧师")]
            PRIEST,
        }

        public enum ItemRarity {
            [InspectorName("普通")]
            COMMON = 0,

            [InspectorName("优秀")]
            GREAT,

            [InspectorName("珍贵")]
            PRECIOUS,

            [InspectorName("史诗")]
            EPIC,

            [InspectorName("传说")]
            LEGENDARY,

            [InspectorName("随机生成")]
            RANDOM,
        }

        public enum ItemType {
            [InspectorName("不合法")]
            INVALID = -1,

            [InspectorName("装备")]
            EQUIPMENT = 0,

            [InspectorName("消耗品")]
            COMSUMABLE = 1,

            [InspectorName("可重复使用")]
            REUSABLE = 2,
        }

        public enum EquipmentType {
            [InspectorName("不合法")]
            INVALID = -1,

            [InspectorName("头部装备")]
            HEAD,

            [InspectorName("衣服")]
            CLOTH,

            [InspectorName("鞋子")]
            SHOES,

            [InspectorName("武器")]
            WEAPON,
        }

        public enum AbilityType {
            [InspectorName("物理攻击")]
            ATTACK,

            [InspectorName("物理防御")]
            DEFEND,

            [InspectorName("特殊攻击")]
            SPE_ATTACK,

            [InspectorName("特殊防御")]
            SPE_DEFEND,

            [InspectorName("速度")]
            SPEED,
        }

        public enum MentalType {
            [InspectorName("领导力")]
            LEA,

            [InspectorName("合作能力")]
            COO,

            [InspectorName("镇静度")]
            CAL,

            [InspectorName("道德")]
            MOR,
        }

        public static byte GameStage = 1;

        [NonSerialized,OdinSerialize]
        [AssetsOnly]
        [AssetSelector]
        [ShowInInspector]
        private static GameConfig config;

        [HideInInspector]
        public static GameConfig Config => config;

        private static readonly Dictionary<GameDataBase.CharacterProfession, List<CharaProfTemplate>> CharaTemplates = new Dictionary<CharacterProfession, List<CharaProfTemplate>>();
        private static readonly Dictionary<GameDataBase.ItemType, List<BaseItemTemplate>> ItemTemplates = new Dictionary<ItemType, List<BaseItemTemplate>>();
        //private static readonly Dictionary<GameDataBase.CharacterProfession, CharacterCurveTemple> ProfAbiTemplates = new Dictionary<CharacterProfession, CharacterCurveTemple>();
    }


    public class CharacterCurveTemple{

        [HideInInspector]
        public RandomRangeCurve HP { get { return hp; } }

        [HideInInspector]
        public RandomRangeCurve ATK { get { return atk; } }

        [HideInInspector]
        public RandomRangeCurve DEF { get { return def; } }

        [HideInInspector]
        public RandomRangeCurve SPE { get { return spe; } }

        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        [BoxGroup("HP")]
        RandomRangeCurve hp;

        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        [BoxGroup("ATK")]
        RandomRangeCurve atk;

        [ShowInInspector]
        [NonSerialized, OdinSerialize]
        [BoxGroup("DEF")]
        RandomRangeCurve def;

        [ShowInInspector]
        [NonSerialized,OdinSerialize]
        [BoxGroup("SPE")]
        RandomRangeCurve spe;
    }
}

