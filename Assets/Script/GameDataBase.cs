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
            InitializeEnemyTemplate();
            TeamInfo.Initialize();
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
            var handle=Addressables.LoadAssetAsync<GameConfig>("GameConfig");
            handle.WaitForCompletion();
            config=handle.Result;
#endif
            var MentalBuffFloor = config.MentalBuffFloor;
            var MentalBuffCeiling = config.MentalBuffCeiling;
            var maxMental = config.MaxMental;
            var minMental = config.MinMental;
            int difference = maxMental - minMental+1;
            double gap = (MentalBuffCeiling - MentalBuffFloor) / (difference - 1.0);
            MentalBuffParams = new List<double>(difference+1);
            MentalBuffParams.Add(0);
            for(int i=1;i<MentalBuffParams.Capacity;i++){
                MentalBuffParams.Add(MentalBuffFloor + (i - 1) * gap);
            }
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

        private static void InitializeEnemyTemplate() {
            var array = Enum.GetValues(typeof(GameDataBase.EnemyRarity));
            foreach (var item in array) {
                EnemyTemplates.Add((GameDataBase.EnemyRarity)item, new List<EnemyTemplate>());
            }
            GetAllEnemyTemplate();
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

        /// <summary>
        /// 需在运行模式下才能正常执行
        /// </summary>
        private static void GetAllEnemyTemplate() {
            var handle = Addressables.LoadAssetsAsync<EnemyTemplate>("EnemyTemplate", null);
            handle.Completed += (handle) => {
                var res=handle.Result;
                foreach (var item in res) {
                    EnemyTemplates[item.rank].Add(item);
                }
            };
        }

        /// <summary>
        /// 根据模板的名字去寻找角色模板，如果没有则会返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CharaProfTemplate GetCharaTemplate(string name) {
            var templates = CharaTemplates.Values.ToList();
            CharaProfTemplate template = null;
            foreach (var item in templates) {
                template=item.Find((x)=>x.name== name);
                if(template!=null) 
                    return template;
            }
            return template;
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

        /// <summary>
        /// 根据装备的名字来筛选出装备
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EquipmentTemplate GetEquipmentTemplate(string name) {
            var equipmentList = ItemTemplates[ItemType.EQUIPMENT];
            var targets=equipmentList.FindAll((item)=>item.name== name);
            if(targets.Count==0) return null;
            return targets[RandomNumberGenerator.Average_GetRandomNumber(0, targets.Count,false)] as EquipmentTemplate;
        }

        /// <summary>
        /// 根据筛选条件来获得装备
        /// </summary>
        /// <param name="rarity">要获得的装备的稀有度</param>
        /// <param name="usable">可使用该装备的职业</param>
        /// <param name="type">装备的类型</param>
        /// <returns>搜索出来的装备，如果存在多个指定条件的装备则会随机返回</returns>
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
                index = RandomNumberGenerator.Average_GetRandomNumber(0, count,false);
            }
            return final.ElementAt(index) as EquipmentTemplate;
        }

        /// <summary>
        /// 根据名字获得敌人模板
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EnemyTemplate GetEnemyTemplate(string name) {
            var lists = EnemyTemplates.Values;
            foreach (var item in lists) {
                var template=item.Find((x)=>x.name== name);
                if (template != null) 
                    return template;
            }
            return null;
        }

        /// <summary>
        /// 获得敌人模板，若存在复数选项，则随机返回一个
        /// </summary>
        /// <param name="rarity"></param>
        /// <param name="duty"></param>
        /// <returns></returns>
        public static EnemyTemplate GetEnemyTemplate(GameDataBase.EnemyRarity rarity,GameDataBase.EnemyDuty duty) {
            var list = EnemyTemplates[rarity];
            var res=list.FindAll((x)=>x.duty== duty);
            if(res.Count==0)
                return null;
            return res[RandomNumberGenerator.Average_GetRandomNumber(0, res.Count, false)];
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

        public enum EnemyRarity {
            [InspectorName("弱")]
            UNDERDOG=0,

            [InspectorName("普通")]
            NORMAL=1,

            [InspectorName("精英")]
            ELITE=3,

            BOSS=10,
        }

        public enum EnemyDuty{
            ATTACKER,
            CURE,
        }

        public enum AttackAnimationType {
            CLOSE=1,
            REMOTE=2,
        }

        public static byte GameStage = 1;

        [NonSerialized,OdinSerialize]
        [AssetsOnly]
        [AssetSelector]
        [ShowInInspector]
        private static GameConfig config;

        [HideInInspector]
        public static GameConfig Config => config;

        public static double[] mentalBuffs=>MentalBuffParams.ToArray();

        private static List<double> MentalBuffParams;

        private static readonly Dictionary<GameDataBase.CharacterProfession, List<CharaProfTemplate>> CharaTemplates = new Dictionary<CharacterProfession, List<CharaProfTemplate>>();
        private static readonly Dictionary<GameDataBase.EnemyRarity, List<EnemyTemplate>> EnemyTemplates = new Dictionary<EnemyRarity, List<EnemyTemplate>>(); 
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

