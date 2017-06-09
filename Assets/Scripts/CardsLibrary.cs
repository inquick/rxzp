using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 牌库
/// </summary>
public class CardsLibrary
{
    private static CardsLibrary instance;
    private List<Card> library;

    public static CardsLibrary Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CardsLibrary();
            }
            return instance;
        }
    }

    /// <summary>
    /// 索引器
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public Card this[int cardId]
    {
        get
        {
            return library[cardId - 1];
        }
    }

    /// <summary>
    /// 私有构造
    /// </summary>
    private CardsLibrary()
    {
        library = new List<Card>();
        CreateDeck();
    }

    /// <summary>
    /// 创建一副牌
    /// </summary>
    void CreateDeck()
    {
        //创建普通扑克
        int id = 1;
        for (int color = 0; color < 4; color++)
        {
            for (int value = 0; value < 13; value++)
            {
                Weight w = (Weight)value;
                Suits s = (Suits)color;
                Card card = new Card(id, w, s);
                library.Add(card);
                ++id;
            }
        }

        //创建小王，大王
        Card smallJoker = new Card(53, Weight.SJoker, Suits.None);
        Card largeJoker = new Card(54, Weight.LJoker, Suits.None);
        library.Add(smallJoker);
        library.Add(largeJoker);
    }
}
