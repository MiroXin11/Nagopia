using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nagopia {
    public class CreateCharacterGroup : MonoBehaviour {
        private void Start() {
            count = objs.Count;
            foreach (var item in objs) {
                CharacterAnimatorController animator = item.GetComponent<CharacterAnimatorController>();
                if (!ReferenceEquals(animator, null)) {
                    animators.Add(item,animator);
                    //animator.Fade(time: 0.02f);
                }
            }
            if (count > 0) {
                index= 0;
                objs[0].transform.localPosition= new Vector3(0, objs[0].transform.localPosition.y,4f);
                //Vector3 initial = new Vector3(8, -2, 4);
                for(int i = 1; i < count; ++i) {
                    var transform = objs[i].transform;
                    var localPos = transform.localPosition;
                    transform.localPosition = new Vector3(8,localPos.y,4);
                }
                if (animators.ContainsKey(objs[0])) {
                    animators[objs[0]].ResetAnimation();
                    animators[objs[0]].Fade(255f, 0.1f);
                }
                introduction.text = objs[0].name;
            }
        }
        public void IndexMove(bool add) {
            if (waitFlag) {
                return;
            }
            if(count<=1) return;
            waitFlag = true;
            int tmp = add ? 1 : -1;
            int newIndex=(index+tmp)%count;
            newIndex=newIndex<0?count-1:newIndex;
            float StartPos = add ? -8 : 8;
            var newobj= objs[newIndex];
            var localPos = newobj.transform.localPosition;
            var Transform= newobj.transform;
            Transform.localPosition = new Vector3(StartPos,localPos.y,localPos.z);
            Transform.DOLocalMoveX(0f, 0.5f);
            if (animators.ContainsKey(newobj)) {
                animators[newobj].ResetAnimation();
                animators[newobj].Fade(255f);
            }
            var oldobj = objs[index];
            Transform = oldobj.transform;
            Transform.DOLocalMoveX(-StartPos, 0.5f);
            if (animators.ContainsKey(oldobj)) {
                animators[oldobj].ResetAnimation();
                animators[oldobj].Fade(completeCallback:()=>waitFlag=false);
            }
            index = newIndex;
            introduction.text = newobj.name;
        }

        public void OnEnable() {
            this.CameraForChara.gameObject.SetActive(true);
        }

        public void OnDisable() {
            //this.CameraForChara?.gameObject.SetActive(false);
        }

        public SuperTextMesh introduction;
        public int index=0;
        int count = 0;
        public List<GameObject>objs= new List<GameObject>();

        public Dictionary<GameObject,CharacterAnimatorController>animators= new Dictionary<GameObject,CharacterAnimatorController>();

        bool waitFlag = false;

        public Camera CameraForChara;

        public InputField NameField;
    }
}