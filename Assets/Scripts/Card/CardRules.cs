using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 出牌规则
/// </summary>
public static class CardRules
{
    private static List<int> singles = new List<int>();
    private static List<int> doubles = new List<int>();
    private static List<int> threes = new List<int>();
    private static List<int> fours = new List<int>();
    private static List<int> laizi = new List<int>();
    private static List<int> temp = new List<int>();

    /// <summary>
    /// 卡牌数组排序
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static void SortCards(List<int> cards)
    {
        // 优先按照牌面大小排序
        cards.Sort(
            (int a, int b) =>
            {
                if (ResourceManager.Instance.CurrentLaizi == CardsLibrary.Instance[a].GetCardWeight || ResourceManager.Instance.CurrentLaizi == CardsLibrary.Instance[b].GetCardWeight)
                {
                    //  如果有赖子则排在最开头
                    if (CardsLibrary.Instance[a].GetCardWeight == CardsLibrary.Instance[b].GetCardWeight)
                    {
                        // 都是赖子比较花色
                        return -CardsLibrary.Instance[a].GetCardSuit.CompareTo(CardsLibrary.Instance[b].GetCardSuit);
                    }
                    else
                    {
                        if (ResourceManager.Instance.CurrentLaizi == CardsLibrary.Instance[a].GetCardWeight)
                        {
                            return -1;
                        }

                        return 1;
                    }
                }
                else
                {
                    //先按照权重降序，再按花色降序
                    return -CardsLibrary.Instance[a].GetCardWeight.CompareTo(CardsLibrary.Instance[b].GetCardWeight) * 2 -
                        CardsLibrary.Instance[a].GetCardSuit.CompareTo(CardsLibrary.Instance[b].GetCardSuit);
                }
            }
        );
    }

    private static void Classification()
    {
        switch (temp.Count)
        {
            case 1:
                singles.Add(temp[0]);
                break;
            case 2:
                doubles.Add(temp[0]);
                doubles.Add(temp[1]);
                break;
            case 3:
                threes.Add(temp[0]);
                threes.Add(temp[1]);
                threes.Add(temp[2]);
                break;
            case 4:
                fours.Add(temp[0]);
                fours.Add(temp[1]);
                fours.Add(temp[2]);
                fours.Add(temp[3]);
                break;
            default:
                break;
        }
        temp.Clear();
    }

    /// <summary>
    /// 把已经按照权重排序好的牌，再按照同一张牌的数量从高到底排序
    /// </summary>
    /// <param name="cards"></param>
    public static void FinalClassification(List<int> cards)
    {
        // 根据同一牌数量归类
        singles.Clear();
        doubles.Clear();
        threes.Clear();
        fours.Clear();
        laizi.Clear();
        temp.Clear();

        Weight currentLaizi = ResourceManager.Instance.CurrentLaizi;
        int lastCardId = 0;

        foreach (int id in cards)
        {
            if (CardsLibrary.Instance[id].GetCardWeight == currentLaizi)
            {
                laizi.Add(id);
            }
            else
            {
                if (lastCardId > 0)
                {
                    if (CardsLibrary.Instance[id].GetCardWeight == CardsLibrary.Instance[lastCardId].GetCardWeight)
                    {
                        temp.Add(id);
                    }
                    else
                    {
                        Classification();
                        temp.Add(id);
                    }
                }
                else
                {
                    temp.Add(id);
                }
				lastCardId = id;
            }
        }
        Classification();

        // 最终放回去

        cards.Clear();

        foreach (int id in laizi)
        {
            cards.Add(id);
        }
        laizi.Clear();

        foreach (int id in fours)
        {
            cards.Add(id);
        }
        fours.Clear();

        foreach (int id in threes)
        {
            cards.Add(id);
        }
        threes.Clear();

        foreach (int id in doubles)
        {
            cards.Add(id);
        }
        doubles.Clear();

        foreach (int id in singles)
        {
            cards.Add(id);
        }
        singles.Clear();
    }

    /// <summary>
    /// 是否是单
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsSingle(int[] cards)
    {
        if (cards.Length == 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 是否是对子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsDouble(int[] cards)
    {
        if (cards.Length == 2)
        {
            if (CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[1]].GetCardWeight)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否是顺子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsStraight(int[] cards)
    {
        if (cards.Length < 5 || cards.Length > 12)
            return false;
        for (int i = 0; i < cards.Length - 1; i++)
        {
            if (CardsLibrary.Instance[cards[i]].GetCardWeight - CardsLibrary.Instance[cards[i + 1]].GetCardWeight != 1)
                return false;

            //顺子里不能出现2，即不能超过A
            if (CardsLibrary.Instance[cards[i]].GetCardWeight > Weight.One)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 是否是双顺子
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsDoubleStraight(int[] cards)
    {
        if (cards.Length < 6 || cards.Length % 2 != 0)
            return false;

        for (int i = 0; i < cards.Length; i += 2)
        {
            if (CardsLibrary.Instance[cards[i + 1]].GetCardWeight != CardsLibrary.Instance[cards[i]].GetCardWeight)
                return false;

            if (i < cards.Length - 2)
            {
				if (CardsLibrary.Instance[cards[i]].GetCardWeight - CardsLibrary.Instance[cards[i + 2]].GetCardWeight != 1)
                    return false;

                //不能超过A
                if (CardsLibrary.Instance[cards[i]].GetCardWeight > Weight.One)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 飞机不带
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsTripleStraight(int[] cards)
    {
        if (cards.Length < 6 || cards.Length % 3 != 0)
            return false;

        for (int i = 0; i < cards.Length; i += 3)
        {
            if (CardsLibrary.Instance[cards[i + 1]].GetCardWeight != CardsLibrary.Instance[cards[i]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[i + 2]].GetCardWeight != CardsLibrary.Instance[cards[i]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[i + 1]].GetCardWeight != CardsLibrary.Instance[cards[i + 2]].GetCardWeight)
                return false;

            if (i < cards.Length - 3)
            {
                if (CardsLibrary.Instance[cards[i + 3]].GetCardWeight - CardsLibrary.Instance[cards[i]].GetCardWeight != 1)
                    return false;

                //不能超过A
                if (CardsLibrary.Instance[cards[i]].GetCardWeight > Weight.One || CardsLibrary.Instance[cards[i + 3]].GetCardWeight > Weight.One)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 飞机带翅膀，带单
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsTripleStraightAndSingle(int[] cards)
    {
        if (cards.Length != 8 || cards.Length != 12 || cards.Length != 16 || cards.Length != 20)
            return false;

        if (cards.Length >= 20)
        {
            if (CardsLibrary.Instance[cards[14]].GetCardWeight != CardsLibrary.Instance[cards[13]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[14]].GetCardWeight != CardsLibrary.Instance[cards[12]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[12]].GetCardWeight != CardsLibrary.Instance[cards[13]].GetCardWeight)
                return false;
        }
        if (cards.Length >= 16)
        {
            if (CardsLibrary.Instance[cards[11]].GetCardWeight != CardsLibrary.Instance[cards[10]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[11]].GetCardWeight != CardsLibrary.Instance[cards[9]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[9]].GetCardWeight != CardsLibrary.Instance[cards[10]].GetCardWeight)
                return false;
        }
        if (cards.Length >= 12)
        {
            if (CardsLibrary.Instance[cards[8]].GetCardWeight != CardsLibrary.Instance[cards[7]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[8]].GetCardWeight != CardsLibrary.Instance[cards[6]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[6]].GetCardWeight != CardsLibrary.Instance[cards[7]].GetCardWeight)
                return false;
        }
        if (cards.Length >= 8)
        {
            if (CardsLibrary.Instance[cards[5]].GetCardWeight != CardsLibrary.Instance[cards[4]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[5]].GetCardWeight != CardsLibrary.Instance[cards[3]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[3]].GetCardWeight != CardsLibrary.Instance[cards[4]].GetCardWeight)
                return false;

            if (CardsLibrary.Instance[cards[2]].GetCardWeight != CardsLibrary.Instance[cards[1]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[2]].GetCardWeight != CardsLibrary.Instance[cards[0]].GetCardWeight)
                return false;
            if (CardsLibrary.Instance[cards[0]].GetCardWeight != CardsLibrary.Instance[cards[1]].GetCardWeight)
                return false;
        }


        return true;
    }

    /// <summary>
    /// 三不带
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsOnlyThree(int[] cards)
    {
        if (cards.Length % 3 != 0)
            return false;
        if (CardsLibrary.Instance[cards[0]].GetCardWeight != CardsLibrary.Instance[cards[1]].GetCardWeight)
            return false;
        if (CardsLibrary.Instance[cards[1]].GetCardWeight != CardsLibrary.Instance[cards[2]].GetCardWeight)
            return false;
        if (CardsLibrary.Instance[cards[0]].GetCardWeight != CardsLibrary.Instance[cards[2]].GetCardWeight)
            return false;

        return true;
    }


    /// <summary>
    /// 三带一
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsThreeAndOne(int[] cards)
    {
        if (cards.Length != 4)
            return false;

        if (CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[1]].GetCardWeight &&
            CardsLibrary.Instance[cards[1]].GetCardWeight == CardsLibrary.Instance[cards[2]].GetCardWeight)
            return true;
        else if (CardsLibrary.Instance[cards[1]].GetCardWeight == CardsLibrary.Instance[cards[2]].GetCardWeight &&
            CardsLibrary.Instance[cards[2]].GetCardWeight == CardsLibrary.Instance[cards[3]].GetCardWeight)
            return true;
        return false;
    }

    /// <summary>
    /// 三代二
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsThreeAndTwo(int[] cards)
    {
        if (cards.Length != 5)
            return false;

        if (CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[1]].GetCardWeight &&
            CardsLibrary.Instance[cards[1]].GetCardWeight == CardsLibrary.Instance[cards[2]].GetCardWeight)
        {
            if (CardsLibrary.Instance[cards[3]].GetCardWeight == CardsLibrary.Instance[cards[4]].GetCardWeight)
                return true;
        }

        else if (CardsLibrary.Instance[cards[2]].GetCardWeight == CardsLibrary.Instance[cards[3]].GetCardWeight &&
            CardsLibrary.Instance[cards[3]].GetCardWeight == CardsLibrary.Instance[cards[4]].GetCardWeight)
        {
            if (CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[1]].GetCardWeight)
                return true;
        }

        return false;
    }

    private static bool IsFourAndTwoSingle(int[] cards)
    {
        if (cards.Length != 6)
            return false;


        if (CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[1]].GetCardWeight &&
            CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[2]].GetCardWeight &&
            CardsLibrary.Instance[cards[0]].GetCardWeight == CardsLibrary.Instance[cards[3]].GetCardWeight)
        {
            return true;
        }

        else if (CardsLibrary.Instance[cards[2]].GetCardWeight == CardsLibrary.Instance[cards[3]].GetCardWeight &&
            CardsLibrary.Instance[cards[2]].GetCardWeight == CardsLibrary.Instance[cards[4]].GetCardWeight &&
            CardsLibrary.Instance[cards[2]].GetCardWeight == CardsLibrary.Instance[cards[5]].GetCardWeight)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 炸弹
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsBoom(int[] cards)
    {
        if (cards.Length != 4)
            return false;

        if (CardsLibrary.Instance[cards[0]].GetCardWeight != CardsLibrary.Instance[cards[1]].GetCardWeight)
            return false;
        if (CardsLibrary.Instance[cards[1]].GetCardWeight != CardsLibrary.Instance[cards[2]].GetCardWeight)
            return false;
        if (CardsLibrary.Instance[cards[2]].GetCardWeight != CardsLibrary.Instance[cards[3]].GetCardWeight)
            return false;

        return true;
    }


    /// <summary>
    /// 王炸
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    private static bool IsJokerBoom(int[] cards)
    {
        if (cards.Length != 2)
            return false;
        if (CardsLibrary.Instance[cards[0]].GetCardWeight == Weight.SJoker)
        {
            if (CardsLibrary.Instance[cards[1]].GetCardWeight == Weight.LJoker)
                return true;
            return false;
        }
        else if (CardsLibrary.Instance[cards[0]].GetCardWeight == Weight.LJoker)
        {
            if (CardsLibrary.Instance[cards[1]].GetCardWeight == Weight.SJoker)
                return true;
            return false;
        }

        return false;
    }

    /// <summary>
    /// 判断是否符合出牌规则
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool PopEnable(int[] cards, out CardsType type)
    {
        type = CardsType.None;
        bool isRule = false;
        switch (cards.Length)
        {
            case 1:
                isRule = true;
                type = CardsType.Single;
                break;
            case 2:
                if (IsDouble(cards))
                {
                    isRule = true;
                    type = CardsType.Double;
                }
                else if (IsJokerBoom(cards))
                {
                    isRule = true;
                    type = CardsType.JokerBoom;
                }
                break;
            case 3:
                if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 4:
                if (IsBoom(cards))
                {
                    isRule = true;
                    type = CardsType.Boom;
                }
                else if (IsThreeAndOne(cards))
                {
                    isRule = true;
                    type = CardsType.ThreeAndOne;
                }

                break;
            case 5:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsThreeAndTwo(cards))
                {
                    isRule = true;
                    type = CardsType.ThreeAndTwo;
                }
                break;
            case 6:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsTripleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.TripleStraight;
                }
                else if (IsFourAndTwoSingle(cards))
                {
                    isRule = true;
                    type = CardsType.FourAndTwoSingle;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                break;
            case 7:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                break;
            case 8:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                else if (IsTripleStraightAndSingle(cards))
                {
                    isRule = true;
                    //  这里是否需要特殊处理下，用以区分飞机带单，和对
                    type = CardsType.TripleStraight;
                }
                break;
            case 9:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 10:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndDouble(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndDouble;
                //}
                break;

            case 11:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                break;
            case 12:
                if (IsStraight(cards))
                {
                    isRule = true;
                    type = CardsType.Straight;
                }
                else if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndSingle(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndSingle;
                //}
                else if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 13:
                break;
            case 14:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                break;
            case 15:
                if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                //else if (IsTripleStraightAndDouble(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndDouble;
                //}
                break;
            case 16:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                //else if (IsTripleStraightAndSingle(cards))
                //{
                //    isRule = true;
                //    type = CardsType.TripleStraightAndSingle;
                //}
                break;
            case 17:
                break;
            case 18:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                else if (IsOnlyThree(cards))
                {
                    isRule = true;
                    type = CardsType.OnlyThree;
                }
                break;
            case 19:
                break;

            case 20:
                if (IsDoubleStraight(cards))
                {
                    isRule = true;
                    type = CardsType.DoubleStraight;
                }
                break;
            default:
                break;
        }

        return isRule;
    }

    /// <summary>
    /// 获取指定数组的权值
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public static int GetWeight(int[] cards, CardsType rule)
    {
        int totalWeight = 0;
        if (rule == CardsType.ThreeAndOne || rule == CardsType.ThreeAndTwo)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (i < cards.Length - 2)
                {
                    if (CardsLibrary.Instance[cards[i]].GetCardWeight == CardsLibrary.Instance[cards[i + 1]].GetCardWeight &&
                        CardsLibrary.Instance[cards[i]].GetCardWeight == CardsLibrary.Instance[cards[i + 2]].GetCardWeight)
                    {
                        totalWeight += (int)CardsLibrary.Instance[cards[i]].GetCardWeight;
                        totalWeight *= 3;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cards.Length; i++)
            {
                totalWeight += (int)CardsLibrary.Instance[cards[i]].GetCardWeight;
            }
        }

        return totalWeight;
    }
}
