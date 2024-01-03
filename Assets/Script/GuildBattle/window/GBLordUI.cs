using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public   class GBLordUI : MonoBehaviour
{
    public UILabel LordName;
    public UILabel LordForce;
    public UISprite LordIcon;
    public UILabel LordOrder;
    private GBLord _lord;

    void Awake() {
        this.LordOrder.enabled = false;
        GBLeaderDeployManager.Instance.GBLordStateChange += this._OnLordSequenceChange;
    }
    void OnDestroy() {
        GBLeaderDeployManager.Instance.GBLordStateChange -= this._OnLordSequenceChange;
    }

    private void _OnLordSequenceChange(GBLord lord){
        if(lord == this._lord){
            
            if(lord.Sequence == 0){
                this.LordOrder.enabled = false;
            }
            else{
                this.LordOrder.enabled = true;
                this.LordOrder.text = string.Format("[{0}]", lord.Sequence);
            }
        }
    }

    public void SetLordInfo(GBLord lord) {
        this.LordName.text = lord.Name;
        this.LordForce.text = lord.Force.ToString();
        this.LordIcon.spriteName = U3dCmn.GetCharIconName(lord.ModelId);
        this._lord = lord;
    }

    void OnSelectLord() {
        if (this._lord.Sequence == 0)
        {
            GBLeaderDeployManager.Instance.ActiveLord(this._lord);
        }
        else {
            GBLeaderDeployManager.Instance.DeactiveLord(this._lord);
        }
    }
}

