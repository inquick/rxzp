using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 窗口界面id
/// </summary>
public enum WINDOW_ID
{
    WINDOW_ID_ROOT = 0, // 根ui
    WINDOW_ID_LOGIN = 1, // 登录界面
    WINDOW_ID_HOME = 2, // 主界面
    WINDOW_ID_CREATE_DDZ = 3, // 创建斗地主房间界面
    WINDOW_ID_ENTER_DDZ = 4, // 进入斗地主房间界面
    WINDOW_ID_CREATE_NN = 5,  // 创建斗牛房间界面
    WINDOW_ID_ENTER_NN = 6, // 进入斗牛房间界面
    WINDOW_ID_TIP = 7, // 提示框界面
    WINDOW_ID_LOADING = 8, // 加载进度条界面
    WINDOW_ID_GAME_DDZ = 9, // 斗地主游戏界面
    WINDOW_ID_SETTLEMENT_ONE = 10,  // 小局结算
    WINDOW_ID_DDZSETTLEMENT_FINAL = 11, // 大局结算
    WINDOW_ID_SHOP = 12, // 商城
    WINDOW_ID_HELP = 13, // 帮助
    WINDOW_ID_SETTING = 14, // 设置
    WINDOW_ID_GAME_NN = 15, // 牛牛游戏界面
    WINDOW_ID_NNSETTLEMENT_FINAL = 16, // 牛牛大局结算
    WINDOW_ID_DIALONG_FINAL = 17 // 二次确认

}

public enum SuitsSpriteSize
{
    sss_small = 0,  // 小
    sss_big = 1,    // 大
}

public enum ServerIp
{
    IP_DDZ_Client = 0, // 斗地主本地
    IP_DDZ_Server = 1, // 斗地主线上
    IP_NN_Client = 2, //  牛牛本地
    IP_NN_Server = 3, //  牛牛线上
    IP_NN_Meng = 4, //  牛牛本地
    IP_NN_Meng2 = 5, //  牛牛本地2(打洞)
}

/// <summary>
/// 游戏类型
/// </summary>
public enum GameType
{
    GT_DDZ = 0, // 斗地主
    GT_NN = 1,   // 牛牛
    GT_NONE = 99 // 错误游戏类型
}

public enum NNOperationGroup
{
    NNOG_None = 0,  // 全部隐藏
    NNOG_Play = 1,  // 开始、邀请、解散
    NNOG_Yafen = 2, // 压分
    NNOG_Open = 3,   // 看牌、开牌
    NNOG_Ready = 4   // 准备
}