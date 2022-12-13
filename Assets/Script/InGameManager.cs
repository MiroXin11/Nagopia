using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.UI;

namespace Nagopia {
    public class InGameManager : SingletonMonobehaviour<InGameManager> {

        private void Awake() {
            this.eventHandler = EventHandler.Instance;
            this.uiCanvas = GameObject.FindGameObjectWithTag("uiCanvas").GetComponent<Canvas>();
            this.RestUIGroup = uiCanvas.transform.Find("RestUIGroup").gameObject;
        }

        private void FixedUpdate() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ExitGame();
            }
        }

        public void ExitGame() {
            Application.Quit();
        }

        private void SelectEvent() {
            int stage = GameDataBase.GameStage;
            if (stage % 3 == 0) {//留给特殊事件
                var config = GameDataBase.Config;
                if (RandomNumberGenerator.Happened(config.NewTeammateProbability)) {
                    GenerateNewTeammateEvent newTeammateEvent = new GenerateNewTeammateEvent();
                    eventHandler.MeetNewTeammateHandler(newTeammateEvent, () => EndEvent());
                    return;
                }
                else if (RandomNumberGenerator.Happened(config.RestoreProbability)) {
                    audioPlayer.PlayOnce("Rest");
                    RestoreHPEvent restoreHPEvent = new RestoreHPEvent(1.0f, true);
                    eventHandler.RestoreHPEventHandle(restoreHPEvent, () => EndEvent());
                    return;
                }
                NothingHappenedEvent nothingHappened = new NothingHappenedEvent(2f);
                eventHandler.NothingEventHandler(ref nothingHappened,() => EndEvent());
                return;
            }
            else if (stage % 10 == 0) {//Boss战?
                var startBattle = BattleStartEvent.GenerateBossFightEvent();
                eventHandler.StartBattle(startBattle,battleEndCallback:()=> { EndEvent();TeamInfo.ResetPosition(); });
            }
            else {
                var startBattle = BattleStartEvent.GenerateRandomBattleEvent();
                eventHandler.StartBattle(startBattle,battleEndCallback: () => { EndEvent(); TeamInfo.ResetPosition(); });
            }
        }

        public void StartEvent() {
            SelectEvent();
        }

        public void EndEvent() {
            StartRest();
        }

        public void StartRest() {
            this.RestUIGroup.gameObject.SetActive(true);
        }

        public void EndRest() {
            //this.RestUIGroup.gameObject.SetActive(false);
            //GameDataBase.GameStage += 1;
            //StartEvent();
            Timing.RunCoroutine(WalkToNextScene());
        }

        private IEnumerator<float> WalkToNextScene() {
            TeamInfo.ResetPosition();
            this.RestUIGroup.gameObject.SetActive(false);
            yield return Timing.WaitForOneFrame;
            var character = TeamInfo.CharacterDatas;
            foreach (var item in character) {
                item.animatorController.StartWalking();
            }
            bool flag = false;
            bgManager.MovePiece(3f, () => flag = true);
            yield return Timing.WaitUntilTrue(() => flag);
            foreach (var item in character) {
                item.animatorController.ResetAnimation();
            }
            yield return Timing.WaitForOneFrame;
            GameDataBase.GameStage += 1;
            StartEvent();
            yield break;
        }

        public void FirstGameStart() {
            audioPlayer.PlayAudioClip("InGameAdventure");
            this.GameStartShowerCamera.gameObject.SetActive(false);
            RandomNumberGenerator.InitialRandom();
            var text = createGroup.introduction.text;
            CharaProfTemplate template;
            switch (text) {
                case "弓箭手": { template = GameDataBase.GetCharaTemplate("RangerTemplate"); } break;
                case "牧师": { template = GameDataBase.GetCharaTemplate("PriestTemplate"); }break;
                case "勇士": { template = GameDataBase.GetCharaTemplate("WarriorTemplate"); }break;
                default: { template = GameDataBase.GetCharaTemplate("WarriorTemplate"); }break;
            }
            int level = 3;
            text = createGroup.NameField.text;
            PlayerCharacter playerCharacter = new PlayerCharacter(ref template, ref level, name: text);
            createGroup.gameObject.SetActive(false);
            normalUIGroup.gameObject.SetActive(true);
            TeamInfo.AddCharacter(playerCharacter);
            StartEvent();
        }

        private EventHandler eventHandler;
        
        private Canvas uiCanvas;

        public Canvas UiCanvas => this.uiCanvas;

        private GameObject RestUIGroup;

        [SerializeField]
        private CreateCharacterGroup createGroup;

        [SerializeField]
        private BackgroundManager bgManager;

        [SerializeField]
        private GameObject normalUIGroup;

        [SerializeField]
        private AudioManager audioPlayer;

        public Camera GameStartShowerCamera;
    }

}

