using UnityEngine;
using System.Collections;

public class GBKickLordUI : MonoBehaviour {
    public UILabel LordNameLabel;
    public UILabel LordForceLabel;
    public UISprite LordIcon;
    public UISprite Background;
    private GBTeamMemberItem _lord;

    public void SetLordInfo(GBTeamMemberItem lord) {
        if (lord != null)
        {
            this.LordForceLabel.gameObject.active = true;
            this.LordIcon.gameObject.active = true;
            this.LordNameLabel.gameObject.active = true;
            this.LordNameLabel.text = lord.LordName;
            this.LordForceLabel.text = string.Format("Force: {0}", lord.LordForce);
            this.LordIcon.spriteName = U3dCmn.GetCharIconName((int)lord.LordModelID);
            this._lord = lord;
        }
        else {
            this.LordForceLabel.gameObject.active = false;
            this.LordIcon.gameObject.active = false;
            this.LordNameLabel.gameObject.active = false;
            this.LordIcon.spriteName = "transparent";
            this.Background.spriteName = "sliced_bg";
            this._lord = null;
        }
    }

    void OnLordSelect() {
        if (_lord == null || _lord.LordID == GBStatusManager.Instance.MyStatus.nCreatorID) {
            return;
        }
        if (_lord.Selected)
        {
            _lord.Selected = false;
            this.Background.spriteName = "sliced_bg";
        }
        else {
            _lord.Selected = true;
            this.Background.spriteName = "sliced_bg4";
        }
    }
}
