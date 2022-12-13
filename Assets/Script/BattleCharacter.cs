using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Nagopia {

    /// <summary>
    /// 玩家方角色
    /// </summary>
    public class BattleCharacter : IBattleCharacter {
        public BattleCharacter(CharacterData data,List<IBattleCharacter>teammates,List<IBattleCharacter>enemy) {
            this.data = data;
            this.teammates = teammates;
            this.opponents=enemy;
        }
        public int HP { get => data.CurrentHP; set => data.CurrentHP = value; }
        public uint MaxHP { get => data.HPMaxValue; }
        public uint ATK { get => data.ATK; set { } }
        public uint DEF { get => data.DEF; set { } }
        public uint SPE { get => data.SPE; set { } }
        public float ATB { get => atb; 
            set {
                this.atb = Mathf.Clamp(value, 0f, GameDataBase.Config.MovedRequireATB);
            } }
        public int POSITION { get => data.Position; set => data.Position = value; }

        public string Name { get => data.name; }

        public bool Attacker { get { return this.data.Profession != GameDataBase.CharacterProfession.PRIEST; } }

        public bool Curer { get { return this.data.Profession == GameDataBase.CharacterProfession.PRIEST; } }

        public GameObject avatar { get { return this.data.obj; } }

        public CharacterAnimatorController animatorController { get { return data.animatorController; } }

        public Sprite HeadImage { get => this.data.HeadImage; }

        public float atb;

        [Sirenix.OdinInspector.ShowInInspector]
        public CharacterData data;

        public void ThinkMove(BattleInfo battleInfo,System.Action thinkFinishedCallback) {
            //var teammate = battleInfo.teammate_sortByPos;
            if(ConsiderEscape(ref battleInfo)) {
                EscapeEvent escapeEvent = new EscapeEvent(this);
                SingletonMonobehaviour<EventHandler>.Instance.CharacterEscapeEventHandle(ref escapeEvent, () => { thinkFinishedCallback?.Invoke(); });
                return;
            }

            ConsiderAttack(ref battleInfo,()=>thinkFinishedCallback?.Invoke());
        }

        /// <summary>
        /// 是否主动逃跑的思考
        /// </summary>
        /// <param name="battleInfo"></param>
        private bool ConsiderEscape(ref BattleInfo battleInfo) {
            var myIndex = battleInfo.teammate_sortByPos.IndexOf(this);
            bool pressure = UnderPressure();
            if (myIndex == 0) {//代表我是下一次受击的对象
                bool canDefeat = DefeatEnemyInRound(ref battleInfo, out bool selfDieFlag);
                //四种情况
                if (selfDieFlag == true && canDefeat == true) {//自己身死但是这轮能击败对手
                    var config = GameDataBase.Config;
                    var t = config.MaxMental * 0.75f;
                    if (this.data.MOR >= t && this.data.COO >= t) {
                        return false;//道德高且合作能力强时，一定不会逃跑
                    }
                    var Probability = config.EscapeDieAndDefeatProbability;
                    var param = GameDataBase.mentalBuffs;
                    double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);
                    double p2=CalculateMentalBuffParam(data.COO,ref param, false);
                    Probability = Probability * p1 * p2;
                    Probability = CalculateCalmnessInfluence(ref Probability, ref pressure, ref param);
                    return RandomNumberGenerator.Happened(Probability);
                }
                if (selfDieFlag == true && canDefeat == false) {//身死但却不知晓是否能击败对手
                    var config= GameDataBase.Config;
                    var Probability = config.EscapeEventBaseProbability;
                    var param = GameDataBase.mentalBuffs;
                    double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);
                    double p2 = CalculateMentalBuffParam(data.COO, ref param, false);
                    Probability = Probability * p1 * p2;
                    Probability = CalculateCalmnessInfluence(ref Probability, ref pressure, ref param);
                    return RandomNumberGenerator.Happened(Probability);
                }
                //之后的情况中，自己不会身死，无需考虑逃跑事件
            }
            else {
                if (UnderPressure()) {
                    var config= GameDataBase.Config;
                    var probability = config.EscapeUnderPressureProbability;
                    var param = GameDataBase.mentalBuffs;
                    probability = CalculateCalmnessInfluence(ref probability, ref pressure, ref param);
                    return RandomNumberGenerator.Happened(probability);
                }
            }
            return false;
        }

        /// <summary>
        /// 被攻击时，会采取何种思考
        /// </summary>
        /// <param name="attackEvent"></param>
        /// <param name="escape"></param>
        public void UnderAttack(AttackEventData attackEvent,out EscapeEvent escape) {
            var damage = attackEvent.Damage;
            escape = null;
            if (this.HP - damage <= 0) {//表示自己受到下一次攻击就会死去
                bool underPressure = UnderPressure();
                double[] param = GameDataBase.mentalBuffs;
                double baseProb = GameDataBase.Config.EscapeEventBaseProbability;
                //计算逃跑事件的发生概率，处于高压力状态下概率会增大。
                double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);//道德越高，越不容易发生
                double p2 = CalculateMentalBuffParam(data.COO, ref param, false);//合作能力越高，越不容易发生独自逃跑
                double finalProb = baseProb * p1 * p2 * 1.05;
                //接下来要考虑镇静度对参数的影响，原公式中谈到要计算预期收益。
                //我们用这个概率代表期望收益，镇静度越高越会倾向于选择期望收益高的选项。
                finalProb = CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                if (RandomNumberGenerator.Happened(finalProb)) {
                    escape = new EscapeEvent(this);
                }
            }
            else {//受到攻击并不会死去，但是我们要检查理智程度来判断ai会不会产生错误行为
                if (this.HP - damage * 2 <= 0) {
                    bool underPressure = UnderPressure();
                    double[] param = GameDataBase.mentalBuffs;
                    double baseProb = GameDataBase.Config.EscapeEventBaseProbability;
                    //计算逃跑事件的发生概率，处于高压力状态下概率会增大。
                    double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);//道德越高，越不容易发生
                    double p2 = CalculateMentalBuffParam(data.COO, ref param, false);//合作能力越高，越不容易发生独自逃跑
                    double finalProb = baseProb * p1 * p2;
                    //接下来要考虑镇静度对参数的影响，原公式中谈到要计算预期收益。
                    //我们用这个概率代表期望收益，镇静度越高越会倾向于选择期望收益高的选项。
                    finalProb = CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                    if (RandomNumberGenerator.Happened(finalProb)) {
                        escape = new EscapeEvent(this);
                    }
                }
            }
        }

        /// <summary>
        /// 返回值为true时，代表愿意代替队友承担这次伤害
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TeammateUnderAttack(AttackEventData data) {
            var attacker = data.attacker;
            var victim = data.target;
            if (victim.HP - attacker.ATK <= 0) {
                var config = GameDataBase.Config;
                var param = GameDataBase.mentalBuffs;
                if (this.HP - attacker.ATK > 0) {//承伤后不死
                    var probability = config.SubstitudeEventBaseProbability;
                    double p1 = CalculateMentalBuffParam(this.data.MOR, ref param, true);
                    double p2 = CalculateMentalBuffParam(this.data.COO, ref param, true);
                    double p3 = CalculateMentalBuffParam(this.data.LEA, ref param, true);
                    CharacterData CharaData= (victim is BattleCharacter)?(victim as BattleCharacter).data:null;
                    var relationBuffer = GetRelationBuff(CharaData);
                    probability = probability * (p1*0.4 + p2*0.3 + p3*0.3) * relationBuffer;
                    bool underPressure = UnderPressure();
                    probability=CalculateCalmnessInfluence(ref probability, ref underPressure, ref param);
                    return RandomNumberGenerator.Happened(probability);
                }
                else {//自己承伤后会死亡
                    var probability = config.SubstituteOnDeadProbability;
                    double p1 = CalculateMentalBuffParam(this.data.MOR, ref param, true);
                    double p2 = CalculateMentalBuffParam(this.data.COO, ref param, true);
                    double p3 = CalculateMentalBuffParam(this.data.LEA, ref param, true);
                    CharacterData CharaData = (victim is BattleCharacter) ? (victim as BattleCharacter).data : null;
                    var relationBuffer = GetRelationBuff(CharaData);
                    probability = probability * (p1 * 0.5 + p2 * 0.1 + p3 * 0.4) * relationBuffer;
                    bool underPressure = UnderPressure();
                    probability = CalculateCalmnessInfluence(ref probability, ref underPressure, ref param);
                    return RandomNumberGenerator.Happened(probability);
                }
            }
            return false;
        }

        /// <summary>
        /// 计算镇静度对于概率的影响
        /// </summary>
        /// <param name="probability">该事件发生的概率，也代表期望收益</param>
        /// <param name="underPressure">是否处于压力下</param>
        /// <param name="CalculateProb">镇静度的补正率，主要是对随机范围进行收束</param>
        /// <returns>修正后的概率</returns>
        private double CalculateCalmnessInfluence(ref double probability, ref bool underPressure, ref double[] CalculateProb) {
            byte Calmness = this.data.CAL;
            if (probability >= 0.5) {//此时代表期望收益为正,镇静度越高，越倾向于选择该事件
                double min = 0.7, max = 1.2;
                if (underPressure) {
                    min = 0.5; max = 1.5;
                }
                min = min * CalculateMentalBuffParam(Calmness, ref CalculateProb, true);//如果镇静度高，那么min将会乘大于一的数，使得范围被收缩;镇静度低，范围会扩大
                double finalParam = RandomNumberGenerator.Average_GetRandomNumber(min, max);
                return probability * finalParam;
            }
            else {//此时代表期望收益为负，镇静度越高，越不倾向于选择该事件。高压环境下，镇静度越低，越倾向于选择该事件
                double min = 0.8, max = 1.3;
                if (underPressure) {
                    min = 0.7; max = 1.5;
                }
                max = max * CalculateMentalBuffParam(Calmness, ref CalculateProb, false);
                double finalParam = RandomNumberGenerator.Average_GetRandomNumber(min, max);
                return probability * finalParam;
            }
        }

        /// <summary>
        /// 计算精神属性的概率参数
        /// </summary>
        /// <param name="buff">精神数值</param>
        /// <param name="param">从GameDataBase中获取的概率数组</param>
        /// <param name="Positive">是否是正向修正</param>
        /// <returns></returns>
        private double CalculateMentalBuffParam(byte buff, ref double[] param, bool Positive) {
            return Positive ? param[buff] : 2.0 - param[buff];
        }

        private void ConsiderAttack(ref BattleInfo battleInfo,System.Action completeCallback=null) {
            IBattleCharacter target;
            switch (this.data.Profession) {
                case GameDataBase.CharacterProfession.KNIGHT: {
                        target = battleInfo.enemy_sortByPos[0];//骑士的攻击选择敌方最近的目标
                        int damage = BattleManager.CalculateDamage(this, target);
                        BattleInfo info = SingletonMonobehaviour<BattleManager>.Instance.GetBattleInfo(this);
                        IBattleCharacter attacker = this;
                        AttackEventData attackEvent = new AttackEventData(ref attacker, ref target, damage, ref info, ComfirmAttackAnimationType());
                        SingletonMonobehaviour<EventHandler>.Instance.AttackEventHandle(attackEvent,()=>completeCallback?.Invoke());
                        break;
                    }
                case GameDataBase.CharacterProfession.RANGER: {
                        target = battleInfo.enemy_sortByPos[0];//游侠的攻击选择敌方最近的目标
                        int damage = BattleManager.CalculateDamage(this, target);
                        BattleInfo info = SingletonMonobehaviour<BattleManager>.Instance.GetBattleInfo(this);
                        IBattleCharacter attacker = this;
                        AttackEventData attackEvent = new AttackEventData(ref attacker, ref target, damage, ref info, ComfirmAttackAnimationType());
                        SingletonMonobehaviour<EventHandler>.Instance.AttackEventHandle(attackEvent, () => completeCallback?.Invoke());
                        break;
                    }
                case GameDataBase.CharacterProfession.PRIEST: {
                        var possibleTargets = new List<IBattleCharacter>(battleInfo.teammate_sortByPos);
                        possibleTargets.Sort((x, y) => { float rateX = (x.HP * 1.0f) / (x.MaxHP*1.0f);float rateY = (y.HP * 1.0f) / (y.MaxHP * 1.0f);return rateX.CompareTo(rateY); });
                        CureEvent cureEvent = new CureEvent(this, possibleTargets[0], (int)this.ATK);
                        SingletonMonobehaviour<EventHandler>.Instance.CureEventHandle(ref cureEvent, () => completeCallback?.Invoke());
                        break;
                    }
                case GameDataBase.CharacterProfession.WARRIOR: {
                        target = battleInfo.enemy_sortByPos[0];//骑士的攻击选择敌方最近的目标
                        int damage = BattleManager.CalculateDamage(this, target);
                        BattleInfo info = SingletonMonobehaviour<BattleManager>.Instance.GetBattleInfo(this);
                        IBattleCharacter attacker = this;
                        AttackEventData attackEvent = new AttackEventData(ref attacker, ref target, damage, ref info, ComfirmAttackAnimationType());
                        SingletonMonobehaviour<EventHandler>.Instance.AttackEventHandle(attackEvent, () => completeCallback?.Invoke());
                        break;
                    }
                default:
                    completeCallback?.Invoke();
                    break;
            }
        }

        public uint CalculateAbility() {
            return System.Convert.ToUInt32(this.HP + this.ATK * 5 + this.DEF * 5 + this.SPE * 10);
        }

        /// <summary>
        /// 用于判断当前是否处于镇静状态，主要跟当前队伍的血量，以及自身的CAL有关
        /// 当自身血量低于20%或者队伍总血量低于40%时
        /// 或者面对强敌时
        /// </summary>
        /// <param name="battleInfo"></param>
        /// <returns></returns>
        private bool UnderPressure() {
            double opponentsAbility = 0;
            var config = GameDataBase.Config;
            foreach (var item in opponents) {
                opponentsAbility += item.CalculateAbility();
            }
            double teammatesAbility = 0;
            foreach (var item in teammates) {
                teammatesAbility += item.CalculateAbility();
            }
            bool currentHPFlag = (this.data.CurrentHP * 1.0f) / (this.data.HPMaxValue * 1.0f) < config.SingleCharaLowHPParam;
            float teamCurrentHP = 0.0f;
            float teamMaxHP = 0.0f;
            foreach (var item in teammates) {
                teamCurrentHP += item.HP;
                teamMaxHP += item.MaxHP;
            }
            bool teamHPFlag = (teamCurrentHP / teamMaxHP) < config.PlayerTeamLowHPParam;
            bool abilityFlag = teammatesAbility * 1.2 < opponentsAbility;
            return abilityFlag || teamHPFlag || currentHPFlag;
        }

        private GameDataBase.AttackAnimationType ComfirmAttackAnimationType() {
            switch (this.data.Profession) {
                case GameDataBase.CharacterProfession.KNIGHT:
                    return GameDataBase.AttackAnimationType.CLOSE;
                case GameDataBase.CharacterProfession.RANGER:
                    return GameDataBase.AttackAnimationType.REMOTE;
                case GameDataBase.CharacterProfession.PRIEST:
                    return GameDataBase.AttackAnimationType.REMOTE;
                case GameDataBase.CharacterProfession.WARRIOR:
                    return GameDataBase.AttackAnimationType.CLOSE;
                default:
                    return GameDataBase.AttackAnimationType.REMOTE;
            }
        }

        private double GetRelationBuff(CharacterData target) {
            if(ReferenceEquals(target, null)) return 1.0;
            var relation = this.data.GetRelationData(target);
            var buffer = 1.0 + relation / 100.0;
            return buffer;
        }

        /// <summary>
        /// 返回为true时，代表这一轮可以在自己死亡前击败对手。返回为false，可能代表自己没有战死也没有全灭敌人，也可能代表自己战死而没有全灭敌人
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool DefeatEnemyInRound(ref BattleInfo info,out bool selfDieFlag) {
            selfDieFlag= false;
            List<IBattleCharacter> allCharacters = new List<IBattleCharacter>();
            allCharacters.AddRange(info.enemy_sortBySPE);
            allCharacters.AddRange(info.teammate_sortBySPE);
            allCharacters.Sort((x, y) => {
                var config = GameDataBase.Config;
                float moveRequired = config.MovedRequireATB, perSec = config.ATBUpPerSec;
                var xMoveSec = (moveRequired - x.ATB) / (perSec + x.SPE * 3.0f);
                var yMoveSec = (moveRequired - y.ATB) / (perSec + y.SPE * 3.0f);
                return xMoveSec.CompareTo(yMoveSec);
            });
            Dictionary<IBattleCharacter, int> hps = new Dictionary<IBattleCharacter, int>();
            foreach (var item in info.enemy_sortBySPE) {
                hps.Add(item, item.HP);
            }
            foreach (var item in info.teammate_sortByPos) {
                hps.Add(item, item.HP);
            }
            BattleManager manager = SingletonMonobehaviour<BattleManager>.Instance;
            foreach (var item in allCharacters) {
                //每个角色依次执行自己操作
                if (item.HP > 0) {
                    if (manager.ValidateIdentification(item)) {
                        if (item.Attacker) {
                            var target = info.enemy_sortByPos.Find((x) => hps[x] > 0);
                            if (target == null) {
                                return true;
                            }
                            var dmg = BattleManager.CalculateDamage(item, target);
                            hps[target] -= dmg;
                        }
                        else if(item.Curer){//进入此函数的条件是自己会挨打，可以放心的只管奶自己
                            IBattleCharacter target = null;
                            int minHP = int.MaxValue;
                            foreach (var t in info.teammate_sortByPos) {
                                if (t.HP > 0) {
                                    if (t.HP < minHP) {
                                        minHP= t.HP;
                                        target = t;
                                    }
                                }
                            }
                            var newHP = hps[target] + (int)item.ATK;
                            newHP = Mathf.Clamp(newHP, 0, (int)MaxHP);
                            hps[target]=newHP;
                        }
                    }
                    else {//敌方
                        if (item.Attacker) {
                            var target = info.teammate_sortByPos.Find((x) => hps[x] > 0);
                            if(target== null) { selfDieFlag = true; return false; }
                            var dmg = BattleManager.CalculateDamage(item, target);
                            hps[target] -= dmg;
                        }
                        else if (item.Curer) {
                            IBattleCharacter target = null;
                            int minHP = int.MaxValue;
                            foreach (var t in info.enemy_sortByPos) {
                                if (t.HP > 0) {
                                    if (t.HP < minHP) {
                                        minHP= t.HP;
                                        target = t;
                                    }
                                }
                            }
                            var newHP = hps[target] + (int)item.ATK;
                            newHP = Mathf.Clamp(newHP, 0, (int)MaxHP);
                            hps[target] = newHP;
                        }
                    }
                    if (hps[this] <= 0) {
                        selfDieFlag = true;
                        return false;
                    }
                }
            }

            selfDieFlag = hps[this] <= 0;
            bool tempFlag = true;
            foreach (var item in info.enemy_sortByPos) {
                if (hps[item] > 0) {
                    tempFlag = false;
                    break;
                }
            }
            return tempFlag;//代表敌人全灭的标志
        }

        public List<IBattleCharacter> teammates;

        public List<IBattleCharacter> opponents;
    }
}
