using System;
using System.Collections.Generic;
using System.Linq;
using TinyWar;

public class GBLord{
    public string Name;
    public int Id;
    public int ModelId;
    public int Force;
    public int Sequence;
    public static int Compare(GBLord x, GBLord y)
    {
        if (x.Sequence == y.Sequence)
        {
            return 0;
        }
        else if (x.Sequence > y.Sequence)
        {
            return 1;
        }
        return -1;
    }
}

public delegate void GBLordSequenceChangeEventHandler(GBLord lord);
 
public  class GBLeaderDeployManager
{
    #region singleton
    static GBLeaderDeployManager _instance;
    private GBLeaderDeployManager() { }
    static GBLeaderDeployManager()
    {
        _instance = new GBLeaderDeployManager();
    }
    public static GBLeaderDeployManager Instance
    {
        get { return _instance; }
    }
    #endregion

    private List<GBLord> _list;
    public event GBLordSequenceChangeEventHandler GBLordStateChange;
    /// <summary>
    /// 所有君主列表
    /// </summary>
    public List<GBLord> DeployList {
        get {
            return this._list;
        }
        set {
            this._list = value;
        }
    }

    /// <summary>
    /// 返回已排序出征的君主列表
    /// </summary>
    public List<GBLord> ActiveList {
        get {
            List<GBLord> result = new List<GBLord>();
            if (this._list != null)
            {
                for (int i = 0; i < this._list.Count; ++i)
                {
                    if (this._list[i].Sequence != 0)
                    {
                        result.Add(this._list[i]);
                    }
                }
            }
            result.Sort(GBLord.Compare);
            return result;
        }
    }

    /// <summary>
    /// 选中某个君主
    /// </summary>
    /// <param name="lord"></param>
    public void ActiveLord(GBLord lord) {
        lord.Sequence = this.ActiveList.Count + 1;
        if (this.GBLordStateChange != null) { 
            this.GBLordStateChange(lord);
        }
        
    }

    /// <summary>
    /// 去掉某个君主
    /// </summary>
    /// <param name="lord"></param>
    public void DeactiveLord(GBLord lord) {
        for (int i = 0; i < this._list.Count; ++i) {
            GBLord cur = this._list[i];
            if (cur.Sequence > lord.Sequence) {
                cur.Sequence--;
                if (this.GBLordStateChange != null)
                {
                    this.GBLordStateChange(cur);
                }
            }
        }
        lord.Sequence = 0;
        if (this.GBLordStateChange != null)
        {
            this.GBLordStateChange(lord);
        }
    }
}
