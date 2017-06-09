using UnityEngine;
using System.Collections;

/// <summary>
/// 牌类
/// </summary>
public class Card
{
    private readonly int cardId;
    private readonly Weight weight;
    private readonly Suits color;

    public Card(int id, Weight weight, Suits color)
    {
        cardId = id;
        this.weight = weight;
        this.color = color;
    }

    /// <summary>
    /// 返回牌Id
    /// </summary>
    public int ID
    {
        get { return cardId; }
    }

    /// <summary>
    /// 返回权值
    /// </summary>
    public Weight GetCardWeight
    {
        get { return weight; }
    }

    /// <summary>
    /// 返回花色
    /// </summary>
    public Suits GetCardSuit
    {
        get { return color; }
    }
}
