using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TinyWar;

public  class GBLeaderDeployWin : MonoBehaviour
{
    private List<GBLordUI> _lordUIList;
    public GameObject ListPanel;

    void Awake() {
        this._lordUIList = new List<GBLordUI>();
    }

    public void CloseWin()
    {
        Destroy(gameObject);
    }

    public void UpdateLordList(List<GBLord> list) {
        GameObject template = GameObject.Find("lordTemplate");
        GBLeaderDeployManager.Instance.DeployList = list;
        if (template != null) {
            for (int i = 0; i < list.Count; ++i) {
                GameObject lordItem = Instantiate(template) as GameObject;
                lordItem.transform.parent = ListPanel.transform;
                GBLordUI lordUI = lordItem.GetComponent<GBLordUI>();
                this._lordUIList.Add(lordUI);
                lordUI.SetLordInfo(list[i]);
                lordItem.transform.localPosition = new Vector3(i * 80, 0, -1);
                lordItem.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void ConfirmOrder() {
        List<GBLord> selectedList = GBLeaderDeployManager.Instance.ActiveList;
        //获取所有的参战君主
        Application.LoadLevel("GuildBattle");
    }

}

