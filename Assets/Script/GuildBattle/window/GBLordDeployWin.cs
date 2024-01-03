using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;

public class GBLordDeployWin : MonoBehaviour {
    public PagingStorage uiGrid;
    public UILabel CheckLabel;
    public GameObject CheckBtn;
    public GameObject SaveBtn;
    public UIButtonMessage CheckMseeage;
    public bool ShowCheckAllBtn = false;
    /// <summary>
    /// 最大可选择将领数
    /// </summary>
    public int MaxHeroCapacity = 5;
    public OnApplyHeroListHandler OnApplyHeroList;

    void Awake() {
        U3dCmn.ScaleScreen(this.transform);
        this.CheckBtn.gameObject.SetActiveRecursively(false);
    }

    void Start() {
        if (ShowCheckAllBtn)
        {
            this.CheckBtn.gameObject.SetActiveRecursively(true);
            Vector3 saveBtnPos = this.SaveBtn.transform.localPosition;
            this.SaveBtn.transform.localPosition = new Vector3(55, saveBtnPos.y, saveBtnPos.z);
        }
    }

    void DismissPanel() {
        Destroy(gameObject);
    }

    public void AssignHero(HeroState state) {
        GBLordTroopManager.Instance.ApplyHero(state);
        List<GBLordTroopHero> list = GBLordTroopManager.Instance.HeroList;
        uiGrid.SetCapacity(list.Count);
        uiGrid.ResetAllSurfaces();
    }

    void CompleteDeploy() { 
        List<GBLordTroopHero> list = new List<GBLordTroopHero>();
        List<GBLordTroopHero> all = GBLordTroopManager.Instance.HeroList;
        for (int i = 0; i < all.Count; i++) {
            if (all[i].IsCampIn) {
                list.Add(all[i]);
            }
        }
        if (list.Count > 0)
        {
            list.Sort(GBLordTroopHero.Compare);
        }
        if (this.OnApplyHeroList != null) {
            this.OnApplyHeroList(list);
			this.OnApplyHeroList = null;
        }
        this.DismissPanel();
    }

    void CheckAll() {
        UncheckAll();
        GBDeployItemUI[] allUI = uiGrid.GetComponentsInChildren<GBDeployItemUI>();
        for (int i = 0; i < allUI.Length; ++i) {
            allUI[i].CheckItem();
        }
        this.CheckLabel.text = "UnSelect";
        this.CheckMseeage.functionName = "UncheckAll";
    }

    void UncheckAll() {
        GBDeployItemUI[] allUI = uiGrid.GetComponentsInChildren<GBDeployItemUI>();
        for (int i = 0; i < allUI.Length; ++i)
        {
            allUI[i].UnCheckItem();
        }
        this.CheckLabel.text = "SelectAll";
        this.CheckMseeage.functionName = "CheckAll";
    }
}
public delegate void OnApplyHeroListHandler(List<GBLordTroopHero> list);
