using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using CMNCMD;
using STCCMD;
using CTSCMD;
using TinyWar;

/// <summary>
/// 团队副本队伍信息
/// </summary>
public class GBTeamListItem
{
    /// <summary>
    /// 队伍id
    /// </summary>
    public ulong TeamID;
    /// <summary>
    /// 创建君主id
    /// </summary>
    public ulong LordID;
    /// <summary>
    /// 创建君主头像id
    /// </summary>
    public uint LordModelID;
    /// <summary>
    /// 创建君主级别
    /// </summary>
    public uint LordLevel;
    /// <summary>
    /// 创建君主名字
    /// </summary>
    public string LordName;
    /// <summary>
    /// 已加入队员数量
    /// </summary>
    public uint JoinedCount;
    /// <summary>
    /// 满员数量，目前固定为5
    /// </summary>
    public const int FullCount = 5;

    public static implicit operator GBTeamListItem(STC_GAMECMD_ALLI_INSTANCE_GET_LIST_T.GBTeamData room)
    {
        GBTeamListItem item = new GBTeamListItem
        {
            JoinedCount = room.nPlayerNum2,
            LordID = room.nCreatorID3,
            TeamID = room.nInstanceID1,
            LordModelID = room.nHeadID7,
            LordLevel = room.nLevel6,
            LordName = DataConvert.BytesToStr(room.szCreatorName5)
        };
        return item;
    }
    /// <summary>
    /// 所有的队伍中的队员信息
    /// </summary>
    public List<GBTeamMemberItem> Members;
}

/// <summary>
/// 团队副本成员信息
/// </summary>
public class GBTeamMemberItem
{
    public ulong LordID;
    public uint LordModelID;
    public string LordName;
    public uint LordForce;
    public List<GBLordTroopHero> Heros;
    public bool Selected;                   //是否被选中
    public bool IsLeader;                   //是否队长 
    public static implicit operator GBTeamMemberItem(STC_GAMECMD_ALLI_INSTANCE_GET_CHAR_DATA_T.GBCharData data)
    {
        GBTeamMemberItem item = new GBTeamMemberItem
        {
            LordID = data.nAccountID2,
            LordForce = 0,
            LordModelID = data.nHeadID6,
            LordName = DataConvert.BytesToStr(data.szName4),
            Heros = new List<GBLordTroopHero>(),
            Selected = false
        };
        return item;
    }

    public override string ToString()
    {
        return string.Format("GBTeamMemberItem:LordID {0}\t\t\tLordForce {1}\t\t\tLordName {2}", this.LordID, this.LordForce, this.LordName);
    }
}

/// <summary>
/// 君主分配将领信息
/// </summary>
public class GBLordTroopHero
{
    /// <summary>
    /// 所属君主id
    /// </summary>
    public ulong LordAccountID;
    /// <summary>
    /// 英雄id
    /// </summary>
    public ulong HeroID;
    /// <summary>
    /// 英雄头像id
    /// </summary>
    public uint HeroModelID;
    /// <summary>
    /// 英雄健康度
    /// </summary>
    public uint HeroHealth;
    /// <summary>
    /// 英雄名字
    /// </summary>
    public string HeroName;
    /// <summary>
    /// 英雄等级
    /// </summary>
    public uint HeroLevel;
    /// <summary>
    /// 兵种类型
    /// </summary>
    public uint TroopID;
    /// <summary>
    /// 带兵等级
    /// </summary>
    public uint TroopLevel;
    /// <summary>
    /// 带兵数量
    /// </summary>
    public uint TroopAmount;
    /// <summary>
    /// 英雄战力
    /// </summary>
    public uint HeroForce;
    /// <summary>
    /// 是否选中出征 
    /// </summary>
    public bool IsCampIn;
    /// <summary>
    /// 出征序号
    /// </summary>
    public uint CampSeq;
    public uint HeroSeq;
    public uint PosID;
    /// <summary>
    /// 位置是否变化过
    /// </summary>
    public bool PosChanged;

    public static implicit operator GBLordTroopHero(STC_GAMECMD_ALLI_INSTANCE_GET_HERO_DATA_T.GBHeroData data)
    {
        GBLordTroopHero hero = new GBLordTroopHero
        {
            HeroID = data.nHeroID1,
            LordAccountID = data.nAccountID4,
            HeroForce = data.nProf7,
            HeroHealth = data.nHealth6,
            HeroLevel = data.nLevel5,
            HeroModelID = data.nHeadID13,
            TroopAmount = data.nArmyNum10,
            TroopLevel = data.nArmyLevel9,
            TroopID = data.nArmyType8,
            CampSeq = data.nCol12,
            HeroSeq = data.nRow11,
            PosID = data.nCol12 == 0 ? 0 : (data.nRow11 - 1) + (data.nCol12 - 1) * 5,
            PosChanged = (data.nCol12 == 0 && data.nRow11 == 0) ? false : true,
            HeroName = DataConvert.BytesToStr(data.szHeroName3)
        };
        return hero;
    }

    public static int Compare(GBLordTroopHero x, GBLordTroopHero y)
    {
        if (x.CampSeq == y.CampSeq)
        {
            return 0;
        }
        else if (x.CampSeq > y.CampSeq)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public override string ToString()
    {
        return string.Format("GBLordTroopHero \t\t\t PosID:{0} \t\t\t Row:{1} \t\t\t Column:{2}", this.PosID, this.HeroSeq, this.CampSeq);
    }
}

/// <summary>
/// 将领简单信息，在配置将领界面使用
/// </summary>
public class GBHero
{
    public string name;
    public int heroId;
    public uint level;
    public uint amount;
    public int troopId;
    public static implicit operator GBHero(GBLordTroopHero cur)
    {
        if (cur == null)
        {
            return null;
        }
        GBHero data = new GBHero
        {
            heroId = (int)cur.HeroModelID,
            amount = cur.TroopAmount,
            level = cur.HeroLevel,
            name = cur.HeroName,
            troopId = (int)cur.TroopID
        };
        return data;
    }
}

public class GBWindowPath {
    public const string GBLevelWin = "GuildBattle/GBLevelWin";
    public const string GBTeamWin = "GuildBattle/GBTeamWin";
    public const string GBKickWin = "GuildBattle/GBKickWin";
    public const string GBLordTroopWin = "GuildBattle/GBLordTroopWin";
    public const string GBBattleFieldWin = "GuildBattle/GBBattleField";
    public const string GBLordDeployWin = "GuildBattle/GBLordDeployWin";
}
