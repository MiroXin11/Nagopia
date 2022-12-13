using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Nagopia
{
    public class EnemyBattleCharacter:IBattleCharacter
    {
        public EnemyBattleCharacter(EnemyData data,List<IBattleCharacter>teammates,List<IBattleCharacter>opponent) {
            this.data = data;
            this.teammates = teammates;
            this.opponents = opponent;
        }

        public int HP { get => data.currentHP; set { data.currentHP = Mathf.Clamp(value, 0, data.MaxHP); } }

        public uint MaxHP { get { return System.Convert.ToUInt32(data.MaxHP); } }

        public uint ATK => data.ATK;

        public uint DEF => data.DEF;

        public uint SPE => data.SPE;

        public float ATB { get => this.atb; 
            set {
                this.atb = Mathf.Clamp(value, 0, GameDataBase.Config.MovedRequireATB);
            } }

        public int POSITION { get => data.Position; }

        public string Name { get => data.name; }

        public bool Attacker { get { return this.data.duty == GameDataBase.EnemyDuty.ATTACKER; } }

        public bool Curer { get { return this.data.duty == GameDataBase.EnemyDuty.CURE; } }

        public GameObject avatar { get => data.avatar; }

        public Sprite HeadImage { get => this.data.HeadImage; }

        public CharacterAnimatorController animatorController { get { return data.animatorController; } }

        void IBattleCharacter.ThinkMove(BattleInfo battleInfo,System.Action thinkFinishedCallback){
            if (handle.IsRunning) {
                Timing.KillCoroutines(handle);
            }
            handle=Timing.RunCoroutine(ThinkMoveCoroutine(battleInfo, thinkFinishedCallback));
        }

        private IEnumerator<float> ThinkMoveCoroutine(BattleInfo battleInfo,System.Action thinkFinishedCallback) {
            bool flag = false;
            switch (this.data.duty) {//敌人只会尝试攻击
                case GameDataBase.EnemyDuty.ATTACKER:
                    NormalAttackerAttack(ref battleInfo,()=>flag=true);
                    break;
                case GameDataBase.EnemyDuty.CURE:
                    CureAttack(ref battleInfo,()=>flag=true);
                    break;
                default:
                    break;
            }
            yield return Timing.WaitUntilTrue(() => flag);//相当于只有一个角色完整的执行完自己的操作后才允许战斗总进程继续进行
            thinkFinishedCallback?.Invoke();
            yield break;
        } 

        private void NormalAttackerAttack(ref BattleInfo battleInfo,System.Action completeCallback=null){
            var target = battleInfo.enemy_sortByPos[0];
            IBattleCharacter temp = this;
            int damage = BattleManager.CalculateDamage(temp, target);
            AttackEventData attackEventData = new AttackEventData(ref temp, ref target,damage ,ref battleInfo,this.data.attackAnimationType);
            SingletonMonobehaviour<EventHandler>.Instance.AttackEventHandle(attackEventData,completeCallback);
        }

        private void CureAttack(ref BattleInfo battleInfo,System.Action completeCallback=null){
            var teammates = battleInfo.teammate_sortBySPE;
            teammates.Sort((x, y) => x.HP.CompareTo(y.HP));
            var target = teammates[0];//选择血量最少的友方进行治疗，虽然默认都是最前排
            CureEvent cureEvent = new CureEvent(this, target, (int)this.ATK);
            SingletonMonobehaviour<EventHandler>.Instance.CureEventHandle(ref cureEvent,completeCallback);
        }

        /// <summary>
        /// 返回为true时，代表愿意做出承伤
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public bool TeammateUnderAttack(AttackEventData eventData){
            if (data.rank >= GameDataBase.EnemyRarity.BOSS) {
                return false;
            }
            var damage = eventData.Damage;
            if (eventData.target.HP - damage <= 0&&this.HP+this.DEF-eventData.attacker.ATK>0) {
                bool underPressure = this.UnderPressure();
                double[] param = GameDataBase.mentalBuffs;
                double baseProb = GameDataBase.Config.SubstitudeEventBaseProbability;
                double p1 = CalculateMentalBuffParam(data.MOR, ref param, true);//道德越高，越容易发生
                double p2=CalculateMentalBuffParam(data.COO, ref param, true);//合作能力越强，越容易发生
                double finalProb = baseProb * p1 * p2;
                finalProb = CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                Debug.Log($"Substitude:{finalProb}");
                return RandomNumberGenerator.Happened(finalProb);
            }
            return false;
        }

        public void UnderAttack(AttackEventData attackEvent,out EscapeEvent escape) {
            var damage = attackEvent.Damage;
            escape= null;
            if(this.HP-damage <= 0) {//表示自己受到下一次攻击就会死去
                bool underPressure = UnderPressure();
                double[] param = GameDataBase.mentalBuffs;
                double baseProb = GameDataBase.Config.EscapeEventBaseProbability;

                //计算逃跑事件的发生概率，处于高压力状态下概率会增大。
                double p1 = CalculateMentalBuffParam(data.MOR, ref param, false);//道德越高，越不容易发生
                double p2 = CalculateMentalBuffParam(data.COO, ref param, false);//合作能力越高，越不容易发生独自逃跑
                double finalProb = baseProb * p1 * p2*1.05;

                //接下来要考虑镇静度对参数的影响，原公式中谈到要计算预期收益。
                //我们用这个概率代表期望收益，镇静度越高越会倾向于选择期望收益高的选项。
                finalProb = CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                if (RandomNumberGenerator.Happened(finalProb) && data.rank < GameDataBase.EnemyRarity.BOSS) {
                    escape = new EscapeEvent(this);
                    Debug.Log($"probability:{finalProb}");
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
                    finalProb=CalculateCalmnessInfluence(ref finalProb, ref underPressure, ref param);
                    if (RandomNumberGenerator.Happened(finalProb)&&data.rank<GameDataBase.EnemyRarity.BOSS) {
                        escape=new EscapeEvent(this);
                        //Debug.Log($"probability:{finalProb}");
                    }
                }
            }
        }

        public uint CalculateAbility() {
            return System.Convert.ToUInt32(HP + ATK * 5 + DEF * 5 + SPE * 10);
        }

        /// <summary>
        /// 计算精神属性的概率参数
        /// </summary>
        /// <param name="buff">精神数值</param>
        /// <param name="param">从GameDataBase中获取的概率数组</param>
        /// <param name="Positive">是否是正向修正</param>
        /// <returns></returns>
        private double CalculateMentalBuffParam(byte buff,ref double[]param,bool Positive) {
            return Positive ? param[buff] : 2.0 - param[buff];
        }

        /// <summary>
        /// 计算镇静度对于概率的影响
        /// </summary>
        /// <param name="probability">该事件发生的概率，也代表期望收益</param>
        /// <param name="underPressure">是否处于压力下</param>
        /// <param name="CalculateProb">镇静度的补正率，主要是对随机范围进行收束</param>
        /// <returns>修正后的概率</returns>
        private double CalculateCalmnessInfluence(ref double probability,ref bool underPressure,ref double[]CalculateProb) {
            byte Calmness = this.data.CAL;
            if (probability >= 0.5) {//此时代表期望收益为正,镇静度越高，越倾向于选择该事件
                double min = 0.7, max = 1.2;
                if (underPressure) {
                    min = 0.5;max = 1.5;
                }
                min =min*CalculateMentalBuffParam(Calmness,ref CalculateProb,true);//如果镇静度高，那么min将会乘大于一的数，使得范围被收缩;镇静度低，范围会扩大
                double finalParam=RandomNumberGenerator.Average_GetRandomNumber(min, max);
                return probability* finalParam;
            }
            else {//此时代表期望收益为负，镇静度越高，越不倾向于选择该事件。高压环境下，镇静度越低，越倾向于选择该事件
                double min = 0.8, max = 1.3;
                if(underPressure) {
                    min = 0.7;max = 1.5;
                }
                max = max * CalculateMentalBuffParam(Calmness, ref CalculateProb, false);
                double finalParam = RandomNumberGenerator.Average_GetRandomNumber(min, max);
                return probability* finalParam;
            }
        }

        private bool UnderPressure() {//判断目前是否处于高压环境
            double opponentsAbility = 0;
            foreach (var item in opponents) {
                opponentsAbility += item.CalculateAbility();
            }
            double teammatesAbility = 0;
            foreach (var item in teammates) {
                teammatesAbility += item.CalculateAbility();
            }
            var config = GameDataBase.Config;
            bool currentHPFlag = (this.data.currentHP * 1.0f) / (this.data.MaxHP * 1.0f) < config.SingleCharaLowHPParam;
            float teamCurrentHP=0.0f;
            float teamMaxHP=0.0f;
            foreach (var item in teammates) {
                teamCurrentHP += item.HP;
                teamMaxHP += item.MaxHP;
            }
            bool teamHPFlag = (teamCurrentHP / teamMaxHP)<config.PlayerTeamLowHPParam;
            bool abilityFlag = teammatesAbility * 1.2 < opponentsAbility;
            return abilityFlag || teamHPFlag || currentHPFlag;
        }

        public float atb;

        public EnemyData data;

        public List<IBattleCharacter> teammates;

        public List<IBattleCharacter> opponents;

        private CoroutineHandle handle;
    }
}

