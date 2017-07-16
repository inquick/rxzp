using UnityEngine;
using System.Collections;

/// <summary>
/// 花色
/// </summary>
public enum Suits
{
    Diamond,
    Club,
    Heart,
    Spade,
    Laizi,
    None
}

/// <summary>
/// 卡牌权值
/// </summary>
public enum Weight
{
    Three = 0,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    One,
    Two,
    SJoker,
    LJoker,
}

/// <summary>
/// 身份
/// </summary>
public enum Identity
{
    Farmer,
    Landlord
}

/// <summary>
/// 出牌类型
/// </summary>
public enum CardsType
{
    //未知类型
    None = 0,
    //王炸
    JokerBoom = 1,
    //炸弹
    Boom = 2,
    //四带二单
    FourAndTwoSingle = 3,
    //四带二对
    FourAndTwoDouble = 4,
    //三个不带
    OnlyThree = 5,
    //三个带一
    ThreeAndOne = 6,
    //三个带二
    ThreeAndTwo = 7,
    //顺子 五张或更多的连续单牌
    Straight = 8,
    //双顺 三对或更多的连续对牌
    DoubleStraight = 9,
    //飞机 三顺 二个或更多的连续三张牌
    TripleStraight = 10,
    //对子
    Double = 11,
    //单个
    Single = 12
}

public enum FlagType
{
    // 扑克牌右上角显示状态空
    None = 0,
    // 地主
    Dizhu = 1,
    // 癞子
    Laizi = 2
}

public enum Sex
{
    // 女孩
    Girl = 0,
    // 男孩
    Boy = 1,
}

public enum PayTypes
{
    PT_NONE = 0,
    // 表示房费房主出
    PT_PAY_ROOM_OWNER = 1,
    // 表示房费AA
    PT_PAY_AA = 2,
    // 表示房费赢家出
    PT_PAY_WIN = 3,
}