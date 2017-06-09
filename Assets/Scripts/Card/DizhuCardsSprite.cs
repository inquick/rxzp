using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 用于卡片的显示
/// </summary>
public class DizhuCardsSprite : MonoBehaviour
{
    // 地主三张牌
    public CardSprite cardSprite1;
    public CardSprite cardSprite2;
    public CardSprite cardSprite3;

    // 柰子
    public CardSprite cardSprite4;

    void Start()
    {
    }

    public void HideCards()
    {
        cardSprite1.ShowBack();
        cardSprite2.ShowBack();
        cardSprite3.ShowBack();
        cardSprite4.ShowBack();
    }

    public void ShowCards(List<int> dizhuCards, int laizi)
    {
        cardSprite1.CardId = dizhuCards[0];
        cardSprite2.CardId = dizhuCards[1];
        cardSprite3.CardId = dizhuCards[2];
        cardSprite4.CardId = laizi;
    }
}
