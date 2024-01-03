using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GBTeamItemUI : MonoBehaviour {
    public UILabel LordNameLabel;
    public UILabel JoinLabel;
    public UILabel TeamInfoLabel;
    public UISprite LordIcon;
	public UIImageButton OperateBtn;
    private GBTeamListItem _item;
    private AllianceWin _allianWin;
	void Start () {
        GameObject obj = GameObject.Find("AllianceWin") as GameObject;
        _allianWin = obj.GetComponent<AllianceWin>();
	}

    void DismissPanel() {
        _allianWin.SendMessage("OpenGuildWarPanel");
    }

    public void SetTeamInfo(GBTeamListItem item) {
		_item = item;
		if(item.LordID != 0)
		{
			this.LordNameLabel.text = item.LordName;
	        this.TeamInfoLabel.text = string.Format(this.TeamInfoLabel.text, item.JoinedCount, GBTeamListItem.FullCount);
			this.LordIcon.gameObject.SetActiveRecursively(true);
	        this.LordIcon.spriteName = U3dCmn.GetCharIconName((int)item.LordModelID);
			OperateBtn.gameObject.SetActiveRecursively(true);
		}
      	else
		{
			this.LordNameLabel.text = "";
			this.TeamInfoLabel.text = "";
			this.LordIcon.gameObject.SetActiveRecursively(false);
			OperateBtn.gameObject.SetActiveRecursively(false);
		}
    }

    public void JoinTeam() {
        _allianWin.JoinTeam(this._item);
    }
}
