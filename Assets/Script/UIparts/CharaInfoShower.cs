using Nagopia;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CharaInfoShower : MonoBehaviour
{
    public void SetText() {
        if (ReferenceEquals(data, null)) {
            this.text.text=string.Empty;
            return;
        }
        string ShowerText = 
            $"名字:{data.name}\n" +
            $"职业:{GameDataBase.TranslateCharacterProfession(data.Profession)}\n"+
            $"等级:{data.Level} 到达下一级需{data.ExpToNextLevel-data.Exp}点经验\n"+
            $"HP:{data.CurrentHP}/{data.HPMaxValue} SPE:{data.SPE}\n"+
            $"ATK:{data.ATK} DEF:{data.DEF}\n"+
            $"头部:{(ReferenceEquals(data.Head,null)?string.Empty:data.Head.Name)}\n"+
            $"衣服:{(ReferenceEquals(data.Cloth,null)?string.Empty:data.Cloth.Name)}\n"+
            $"鞋子:{(ReferenceEquals(data.Shoes,null)?string.Empty:data.Shoes.Name)}\n"+
            $"武器:{(ReferenceEquals(data.Weapon,null)?string.Empty:data.Weapon.Name)}";
        tmp= ShowerText;
        if (this.textArea.activeSelf) {
            text.text = tmp;
            text.text = text.text.Replace("\\n", "\n");
        }
        else {
            this.ResetFlag = true;
        }
    }

    public void OnEnable() {
        if (ResetFlag) {
            text.text = tmp;
            text.text = text.text.Replace("\\n", "\n");
            ResetFlag= false;
        }
        textArea.SetActive(false);
    }

    public void ImagePointerEnter() {
        textArea.SetActive(true);
        if (ResetFlag) {
            text.text = tmp;
            text.text = text.text.Replace("\\n", "\n");
            ResetFlag = false;
        }
    }

    public void ImagePointerExit() {
        textArea.SetActive(false);
    }

    bool ResetFlag = false;

    public Image Head;

    public GameObject textArea;

    public SuperTextMesh text;

    public CharacterData Data { get { return data; } set { 
            this.data = value;
            SetText();
            Head.sprite = ReferenceEquals(null, data) ? null : data.HeadImage; } }

    private CharacterData data;

    private string tmp=string.Empty;
}
