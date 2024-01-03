using UnityEngine;
using System.Collections;

public class GBTeamMemberItemUI : MonoBehaviour {
    public UILabel lordName;
    public UILabel lordForce;
    public UISprite lordIcon;
    public UIImageButton KickBtn;
    public GameObject SoldierTemplate;

    private GBTeamMemberItem _me;
    public GBTeamListItem TeamInfo;
    public GBTeamWin TeamWin;
    void Awake() {
        KickBtn.gameObject.SetActiveRecursively(false);
    }

    public void SetItemInfo(GBTeamMemberItem data){
        _me = data;
        this.lordName.text = data.LordName;
        this.lordForce.text = string.Format(this.lordForce.text, data.LordForce);
        this.lordIcon.spriteName = U3dCmn.GetCharIconName((int)data.LordModelID);

        if (this.TeamInfo.LordID != data.LordID  && CommonData.player_online_info.AccountID == this.TeamInfo.LordID)
        {
            KickBtn.gameObject.SetActiveRecursively(true);
        }
        //展示兵种
        for (int i = 0; i < 5; ++i)
        {
            GameObject obj = NGUITools.AddChild(this.gameObject, SoldierTemplate);
            obj.name = string.Format("Soldier{0:D2}", i);
            UISprite sp = obj.GetComponent<UISprite>();
            sp.transform.localPosition = new Vector3(85 - i * 25, 0);
            sp.transform.localScale = new Vector3(20, 20, 1);
            string spName = "transparent";
            if (i <= data.Heros.Count - 1)
            {
                spName = U3dCmn.GetTroopIconFromID((int)data.Heros[i].TroopID);
            }
            sp.spriteName = spName;
        }
    }

    public void KickMember() {
        ulong lordID = this._me.LordID;
        if (TeamWin != null) {
            TeamWin.KickMember(lordID);
        }
    }
}
