using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using MEC;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nagopia {
    public class BattleATBIndicator : SingletonMonobehaviour<BattleATBIndicator>,IBattleElement {
        string IBattleElement.Name => "ATBIndicator";

        private void Awake() {
            rect = this.gameObject.GetComponent<RectTransform>();
            this.ATBEnd = GameDataBase.Config.MovedRequireATB;
            Vector2 min = rect.rect.min;
            Vector2 max=rect.rect.max;
            StartPosition = new Vector2(min.x, (min.y + max.y) / 2.0f);
            EndPosition = new Vector2(max.x, (max.y + min.y) / 2.0f);
            Gap=(EndPosition- StartPosition)/(ATBEnd-ATBStart);
            Debug.Log($"Gap:{Gap}");
        }

        private void OnEnable() {
            battleManager=SingletonMonobehaviour<BattleManager>.Instance;
            battleManager.onBattleStart.AddListener(StartBattle);
            battleManager.onATBUpdate.AddListener(UpdateCharacterATB);
            battleManager.onCharacterDefeated.AddListener(RemoveCharacter);
            battleManager.onBattleEnd.AddListener(BattleEnd);
        }

        private void OnDisable() {
            battleManager.onBattleStart.RemoveListener(StartBattle);
            battleManager.onATBUpdate.RemoveListener(UpdateCharacterATB);
            battleManager.onCharacterDefeated.RemoveListener(RemoveCharacter);
            battleManager.onBattleEnd.RemoveListener(BattleEnd);
            RemoveData();
        }

        private void StartBattle(BattleInfo info) {
            Dictionary<int, List<int>> SpeedIndex = new Dictionary<int, List<int>>();
            foreach (var item in info.teammate_sortBySPE) {
                var obj = GameObject.Instantiate(HeadPrefab, this.transform);
                var placement = obj.transform.Find("HeadPlacement");
                placement.GetComponent<Image>().sprite = item.HeadImage;
                transforms.Add(item, obj.transform);
                var color = item.avatar.gameObject.tag == "player" ? PlayerColor : TeammteColor;
                obj.GetComponent<Image>().color = color;
                if(SpeedIndex.TryGetValue((int)item.SPE,out var value)) {
                    PosOffsets.Add(item, value.Count);
                    value.Add(value.Count);
                }
                else {
                    value = new List<int>();
                    PosOffsets.Add(item, value.Count);
                    value.Add(value.Count);
                    SpeedIndex.Add((int)item.SPE, value);
                }
                obj.transform.SetAsFirstSibling();
            }
            foreach (var item in info.enemy_sortBySPE) {
                var obj = GameObject.Instantiate(HeadPrefab, this.transform);
                var placement = obj.transform.Find("HeadPlacement");
                placement.GetComponent<Image>().sprite = item.HeadImage;
                transforms.Add(item, obj.transform);
                obj.GetComponent<Image>().color = EnemyColor;
                if (SpeedIndex.TryGetValue((int)item.SPE, out var value)) {
                    PosOffsets.Add(item, value.Count);
                    value.Add(value.Count);
                }
                else {
                    value = new List<int>();
                    PosOffsets.Add(item, value.Count);
                    value.Add(value.Count);
                    SpeedIndex.Add((int)item.SPE, value);
                }
                obj.transform.SetAsFirstSibling();
            }
        }

        private void UpdateCharacterATB(IBattleCharacter character) {
            if (transforms.ContainsKey(character)) {
                transforms[character].localPosition = StartPosition + Gap * character.ATB + offset[PosOffsets[character] % offset.Length];
            }
        }

        private void BattleEnd(BattleInfo info) {
            RemoveData(info);
            //this.gameObject.SetActive(false);
        }

        private void RemoveCharacter(IBattleCharacter character) {
            GameObject.DestroyImmediate(transforms[character].gameObject);
            transforms.Remove(character);
        }

        private void RemoveData(BattleInfo info=null) {
            foreach (var item in transforms) {
                GameObject.DestroyImmediate(item.Value.gameObject);
            }
            transforms.Clear();
            PosOffsets.Clear();
        }

        private float ATBStart = 0;

        private float ATBEnd;

        private BattleManager battleManager;

        private RectTransform rect;

        private Vector2 StartPosition;

        private Vector2 EndPosition;

        private Vector2 Gap;

        [SerializeField]
        private GameObject HeadPrefab;

        private Dictionary<IBattleCharacter, Transform> transforms = new Dictionary<IBattleCharacter, Transform>();

        private Dictionary<IBattleCharacter, int> PosOffsets = new Dictionary<IBattleCharacter, int>();

        public Color PlayerColor = new Color(0.8f, 0.8f, 0.8f);

        public Color TeammteColor = new Color(0.7f, 0.7f, 0.7f);

        public Color EnemyColor = new Color(0.5f, 0.5f, 0.5f);

        private readonly Vector2[] offset = { new Vector2(0f, 0f), new Vector2(0, 30f), new Vector2(0, -30f), new Vector2(0, 60f), new Vector2(0, -60f), new Vector2(0, 90f), new Vector2(0, -90f) };
    }
}