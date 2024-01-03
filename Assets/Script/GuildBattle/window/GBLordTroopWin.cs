using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CMNCMD;
using STCCMD;

public class GBLordTroopWin : MonoBehaviour {
    public GameObject ListPanel;
    private List<GBHeroUI> _heroPanels;
    private List<GBLordTroopHero> _selectedHeros;
    private int _maxHeroCount = 5;
    private ulong[] _lastSelectedHeroIDs;
    public GBTeamWin TeamWin;

	void Awake () {
        U3dCmn.ScaleScreen(this.transform);
        _heroPanels = new List<GBHeroUI>();
        _selectedHeros = GBTeamManager.Instance.MyTeamMember.Heros;
        GBTeamManager.Instance.OnReady += this.OnReady;
        GBTeamManager.Instance.OnDeployHero += this.OnDeployComplete;

        if (_selectedHeros.Count > 0) {
            _lastSelectedHeroIDs = new ulong[_selectedHeros.Count];
            for (int i = 0; i < _selectedHeros.Count; ++i) {
                _lastSelectedHeroIDs[i] = _selectedHeros[i].HeroID;
            }
        }
	}

    //配置武将回调 
    public void OnApplyGBHeroList(List<GBLordTroopHero> list)
    {
        ClearTroop();
        for (int i = 0; i < list.Count; ++i) {
            GBLordTroopHero cur = list[i];
            GBHeroUI heroPanel = _heroPanels[i];
            heroPanel.SetHeroInfo(cur);
        }
        this._selectedHeros = list;
    }

    void ClearTroop() {
        Transform[] ts = ListPanel.GetComponentsInChildren<Transform>();
        foreach (Transform s in ts) {
            GBHeroUI hero = s.GetComponent<GBHeroUI>();
            if (hero != null) {
                hero.SetHeroInfo(null);
            }
        }
    }

    void OpenHeroListWin() {
        GameObject go = U3dCmn.GetObjFromPrefab(GBWindowPath.GBLordDeployWin);
        GBLordDeployWin t = go.GetComponent<GBLordDeployWin>();
        if (t != null)
        {
            t.OnApplyHeroList = this.OnApplyGBHeroList;
            t.AssignHero(HeroState.COMBAT_INSTANCE_GUILD);
        }
    }

    //获取现有武将布阵情况
    public void LoadLordTroop() {
        GameObject template = GameObject.Find("heroTemplate") as GameObject;
        for (int i = 0; i < _maxHeroCount; ++i)
        {
            GameObject troopItem = Instantiate(template) as GameObject;
            troopItem.transform.parent = ListPanel.transform;
            GBHeroUI hero = troopItem.GetComponent<GBHeroUI>();
            //如果已经配兵过了，则显示之前的配兵数据
            if (i > this._selectedHeros.Count - 1)
            {
                hero.SetHeroInfo(null);
            }
            else {
                hero.SetHeroInfo(this._selectedHeros[i]);
            }
            _heroPanels.Add(hero);
            troopItem.transform.localPosition = new Vector3(i * 80, 0, -1);
            troopItem.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void DismissPanel() {
        GBTeamManager.Instance.OnReady -= this.OnReady;
        GBTeamManager.Instance.OnDeployHero -= this.OnDeployComplete;
        Destroy(gameObject);
    }

    //保存君主出征的武将
    void SaveGBHero() {
        if (this._selectedHeros.Count == 0)
        {
            PopTipDialog.instance.VoidSetText2(true, false, U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.GB_DEPLOY_ONE_HERO));
        }
        else {
            GBTeamManager.Instance.DeployHero(this._selectedHeros);
            LoadingManager.instance.AddLoadingCmd((int)GAME_SUB_RST.STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO);
        }
    }

    private void OnDeployComplete(STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T.enum_rst result) {
        if (result == STC_GAMECMD_ALLI_INSTANCE_CONFIG_HERO_T.enum_rst.RST_NOT_IN_ISNTANCE) {//不在副本中
            U3dCmn.ShowWarnWindow(U3dCmn.GetWarnErrTipFromMB((int)MB_WARN_ERR.NOT_IN_GB_TEAM));
        }
    }

    private void OnReady(STC_GAMECMD_ALLI_INSTANCE_READY_T.enum_rst result)
    {
        if (result == STC_GAMECMD_ALLI_INSTANCE_READY_T.enum_rst.RST_OK) {
            this.DismissPanel();
            if (this.TeamWin != null)
            {
                if (this._lastSelectedHeroIDs != null) {
                    JiangLingManager.UpdateHeroStatus(this._lastSelectedHeroIDs, HeroState.NORMAL);
                }
                TeamWin.OnMemberListGet(GBTeamManager.Instance.TeamMemberList);
                GBTeamManager.Instance.UpdateHeroStatus(HeroState.COMBAT_INSTANCE_GUILD);
            }
        }
    }
}