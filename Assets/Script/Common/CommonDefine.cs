 public enum ePayChannel
{
	ePay_AppStore,
	ePay_WeChat,
	ePay_WeChat_365Niu = ePay_WeChat,
	ePay_ZhiFuBao,
	ePay_XiaoMi,
	ePay_WeChat_365Golden,
	ePay_Max,
};

public enum eClubPrivilige
{
	eClubPrivilige_Forbid, // can not enter room 
	eClubPrivilige_Normal,
	eClubPrivilige_Manager,
	eClubPrivilige_Creator,
	eClubPrivilige_Max,
};

public enum eClubEvent
{
	eClubEvent_ApplyJoin,// some body apply to join club , { uid : 23, respUID : 11, isAgree : 0  }, when processed , contain key : respUID : 11, isAgree 
	eClubEvent_Kick, // { uid : 23 , mgrUID : 23 }
	eClubEvent_Leave, // { uid : 23 }
	eClubEvent_UpdatePrivlige, // { uid : 23 , privilige : eClubPrivilige , actUID : 234 }
	eClubEvent_RespInvite,// { uid : 234 , nIsAgree : 0 }
	eClubEvent_ResetPlayerPoints, // { uid : 23 , mgrUID : 23 }
	eClubEvent_SetPlayerInitPoints, // { uid : 23 , mgrUID : 23, points : 23 }
	eClubEvent_Max,
};

public enum eEventState
{
	eEventState_WaitProcesse,
	eEventState_Processed,
	eEventState_TimeOut,
	eEventState_Max,
};

public enum ePayRoomCardType
{
	ePayType_None,
	ePayType_RoomOwner = ePayType_None,
	ePayType_AA,
	ePayType_Winer,
	ePayType_Max,
};

public enum eGameType
{
	eGame_None,
	eGame_NiuNiu,
	eGame_BiJi,
	eGame_CYDouDiZhu,
	eGame_JJDouDiZhu,
	eGame_TestMJ,
	eGame_Golden,
	eGame_SCMJ,
	eGame_Max,
};

public enum eRoomState
{
	// new state 
	eRoomSate_WaitReady,
	eRoomState_StartGame,
	eRoomState_Common_Max = 20,

	// niu niu special ;
	eRoomState_DecideBanker,
	eRoomState_RobotBanker = eRoomState_DecideBanker,
	eRoomState_DistributeFirstCard,
	eRoomState_DoBet ,
	eRoomState_DistributeCard, 
	eRoomState_DistributeFinalCard = eRoomState_DistributeCard,
	eRoomState_CaculateNiu,
	eRoomState_GameEnd,
	eRoomState_NN_Max = 50,
	
	// mj specail ;
	eRoomState_WaitPlayerAct,  // �ȴ���Ҳ��� { idx : 0 , huaCard : 23 }
	eRoomState_DoPlayerAct,  // ��Ҳ��� // { idx : 0 ,huIdxs : [1,3,2,], act : eMJAct_Chi , card : 23, invokeIdx : 23, eatWithA : 23 , eatWithB : 22 }
	eRoomState_AskForRobotGang, // ѯ��������ܺ��� { invokeIdx : 2 , card : 23 }
	eRoomState_WaitPlayerChu, // �ȴ���ҳ��� { idx : 2 }
	eRoomState_MJ_Common_Max = 80, 

	// bj specail 
	eRoomState_BJ_Make_Group,
	eRoomState_BJ_Max = 100,
	// dou di zhu specail 
	eRoomState_DDZ_Chu,
	eRoomState_JJ_DDZ_Ti_La_Chuai, 
	eRoomState_JJ_DDZ_Chao_Zhuang,

	// above is new ;
	eRoomState_None,
	eRoomState_WaitOpen,
	eRoomState_Opening,
	eRoomState_Pasue,
	eRoomState_Dead,
	eRoomState_WillDead,
	eRoomState_TP_Dead = eRoomState_Dead,
	eRoomState_WaitJoin,
	eRoomState_NN_WaitJoin = eRoomState_WaitJoin ,
	eRoomState_TP_WaitJoin = eRoomState_WaitJoin,
	eRoomState_WillClose,
	eRoomState_Close,
	eRoomState_DidGameOver,
	eRoomState_NN_Disribute4Card = eRoomState_StartGame,


	// state for golden
	eRoomState_Golden_Bet,
	eRoomState_Golden_PK,
	eRoomState_Golden_GameResult,

	// state for si chuan ma jiang 
	eRoomState_WaitExchangeCards, //  �ȴ���һ���
	eRoomState_DoExchangeCards, // ��һ���
	eRoomState_WaitDecideQue,  // �ȴ���Ҷ�ȱ
	eRoomState_DoDecideQue,  //  ��Ҷ�ȱ
	eRoomState_DoFetchCard, // �������
	
	
	
	eRoomState_WaitOtherPlayerAct,  // �ȴ���Ҳ��������˳����� { invokerIdx : 0 , card : 0 ,cardFrom : eMJActType , arrNeedIdxs : [2,0,1] } 
	eRoomState_DoOtherPlayerAct,  // ������Ҳ����ˡ�
	
	eRoomState_AskForHuAndPeng, // ѯ����������ߺ�  { invokeIdx : 2 , card : 23 }
	eRoomState_WaitSupplyCoin, // �ȴ���Ҳ�����  {nextState: 234 , transData : { ... } }
	eRoomState_WaitPlayerRecharge = eRoomState_WaitSupplyCoin,  //  �ȴ���ҳ�ֵ
	eRoomState_NJ_Auto_Buhua, // �Ͼ��齫�Զ����� 
	
	eRoomState_Max,
};


public enum eMJActType
{
	eMJAct_None,
	eMJAct_Mo = eMJAct_None, // ����
	eMJAct_Chi, // ��
	eMJAct_Peng,  // ����
	eMJAct_MingGang,  // ����
	eMJAct_AnGang, // ����
	eMJAct_BuGang,  // ���� 
	eMJAct_BuGang_Pre, // ���ܵ�һ�׶�
	eMJAct_BuGang_Declare = eMJAct_BuGang_Pre, // ����Ҫ���� 
	eMJAct_BuGang_Done, //  ���ܵڶ��׶Σ�ִ�и���
	eMJAct_Hu,  //  ����
	eMJAct_Chu, // ����
	eMJAct_Pass, //  �� 
	eMJAct_BuHua,  // ����
	eMJAct_HuaGang, // ����
	eMJAct_Followed, // ��������4���ƣ�Ҫ��Ǯ��
	eMJAct_4Feng, // ǰ4�ų���4�Ų�һ���ķ���
	eMJAct_Ting,
	eMJAct_Max,
};

public enum eFanxingType
{
	eFanxing_PingHu, // ƽ��

	eFanxing_DuiDuiHu, //  �ԶԺ�

	eFanxing_QingYiSe, // ��һɫ
	eFanxing_QiDui, //  �߶�
	eFanxing_QuanQiuDuDiao, // ȫ�����
	eFanxing_TianHu, //���
	eFanxing_ShuangQiDui, // ˫�߶�

	eFanxing_MengQing, // ����

	eFanxing_YaJue, // ѹ�� 
	eFanxing_HunYiSe, // ��һɫ
	eFanxing_WuHuaGuo, // �޻���
	eFanxing_DiHu,//�غ�

	eFanxing_HaiDiLaoYue, // ��������
	eFanxing_DaMenQing, // ������
	eFanxing_XiaoMenQing, // С����

	eFanxing_GangKai, //�ܿ�
	eFanxing_QiangGang, //����
	eFanxing_GangHouPao, //�ܺ���
	
	eFanxing_SC_ZhongZhang, //�Ĵ��齫����
	eFanxing_SC_19JiangDui, //�Ĵ��齫19����
	eFanxing_SC_Gen, //�Ĵ��齫��(4����ͬ����)

	eFanxing_Max, // û�к�
};

public enum eSettleType    // ���ö�ٶ����ֻ��һ���������¼������ڷ����¼���˫�����з���һ�������磺 Ӯ���˽б����ڣ������ �� ���ڡ�
{
	eSettle_DianPao,  // ����
	eSettle_MingGang, // ����
	eSettle_AnGang, //  ����
	eSettle_BuGang,  //  ����
	eSettle_ZiMo,  // ����
	eSettle_HuaZhu,  //   �黨��
	eSettle_DaJiao,  //  ����
	eSettle_Max,
};

public enum eTime
{
	eTime_WaitPlayerReady = 15,
	eTime_WaitRobotBanker = 5,
	eTime_ExeGameStart = 1,			// ִ����Ϸ��ʼ ��ʱ��
	eTime_WaitChoseExchangeCard = 5, //  �ȴ����ѡ���Ƶ�ʱ��
	eTime_DoExchangeCard = 3, //   ִ�л��Ƶ�ʱ��
	eTime_WaitDecideQue = 10, // �ȴ���Ҷ�ȱ
	eTime_DoDecideQue = 2, // ��ȱʱ��
	eTime_WaitPlayerAct = 10,  // �ȴ���Ҳ�����ʱ��
	eTime_WaitPlayerChoseAct = eTime_WaitPlayerAct,
	eTime_GoldenChoseAct = 120, // ���ŵȴ���Ҳ���ʱ��
	eTime_DoPlayerMoPai = 0,  //  �������ʱ��
	eTime_DoPlayerActChuPai = 1,  // ��ҳ��Ƶ�ʱ��
	eTime_DoPlayerAct_Gang = 0, // ��Ҹ���ʱ��
	eTime_DoPlayerAct_Hu = 1,  // ��Һ��Ƶ�ʱ��
	eTime_DoPlayerAct_Peng = 0, // �������ʱ��
	eTime_GameOver = 1, // ��Ϸ����״̬����ʱ��
	eTime_WaitForever = 999999999,
};

// player State 
public enum eRoomPeerState
{
	eRoomPeer_None,
	// peer state for taxas poker peer
	eRoomPeer_SitDown = 1,
	eRoomPeer_StandUp = 1 << 1,
	eRoomPeer_Ready = (1 << 12) | eRoomPeer_SitDown,
	eRoomPeer_StayThisRound = ((1 << 2) | eRoomPeer_SitDown),
	eRoomPeer_WaitCaculate = ((1 << 7) | eRoomPeer_StayThisRound),
	eRoomPeer_AllIn = ((1 << 3) | eRoomPeer_WaitCaculate),
	eRoomPeer_GiveUp = ((1 << 4) | eRoomPeer_StayThisRound),
	eRoomPeer_CanAct = ((1 << 5) | eRoomPeer_WaitCaculate),
	eRoomPeer_WaitNextGame = ((1 << 6) | eRoomPeer_SitDown),
	eRoomPeer_DoMakedCardGroup = (1 << 8) | eRoomPeer_CanAct,
	eRoomPeer_WillLeave = (1 << 10) | eRoomPeer_StandUp,
	eRoomPeer_Looked = (1 << 13) | eRoomPeer_CanAct,
	eRoomPeer_PK_Failed = (1 << 14) | eRoomPeer_StayThisRound,
	eRoomPeer_ShowedHoldCard = ( 1 << 16 ),
	eRoomPeer_SysAutoAct = ( 1 << 18), // �й�״̬
	eRoomPeer_AlreadyHu = ((1 << 15) | eRoomPeer_CanAct),  //  �Ѿ����Ƶ�״̬
	eRoomPeer_DelayLeave = (1 << 17),  //  �ƾֽ�������뿪
	eRoomPeer_TiLaChuai = (1 << 19),  //  ������
	eRoomPeer_Max,
};


public enum eSex
{
	eSex_Male,
	eSex_Female,
	eSex_Max,
};

public enum eRoomPeerAction
{
	eRoomPeerAction_None,
	eRoomPeerAction_EnterRoom,
	eRoomPeerAction_Ready,
	eRoomPeerAction_Follow,
	eRoomPeerAction_Add,
	eRoomPeerAction_PK,
	eRoomPeerAction_GiveUp,
	eRoomPeerAction_ShowCard,
	eRoomPeerAction_ViewCard,
	eRoomPeerAction_TimesMoneyPk,
	eRoomPeerAction_LeaveRoom,
	eRoomPeerAction_Speak_Default,
	eRoomPeerAction_Speak_Text,
	// action for 
	eRoomPeerAction_Pass,
	eRoomPeerAction_AllIn,
	eRoomPeerAction_SitDown,
	eRoomPeerAction_StandUp,
	eRoomPeerAction_Max
};

public enum eRoomFlag
{
	eRoomFlag_None ,
	eRoomFlag_ShowCard  ,
	eRoomFlag_TimesPK ,
	eRoomFlag_ChangeCard,
	eRoomFlag_Max,
};
// mail Module 
public enum eMailType
{
	eMail_Wechat_Pay, // { ret : 0 , diamondCnt : 23 }  // ret : 1 means verify error 
	eMail_AppleStore_Pay, // { ret : 0 , diamondCnt : 23 }   // ret : 1 means verify error 
	eMail_Agent_AddCard, // { agentID : 23 , serialNo : 2345 , cardOffset : 23 }
	eMail_Consume_Diamond, // { diamond : 23 , roomID :23, reason : 0 } 
	eMail_GiveBack_Diamond, // { diamond : 23 , roomID :23, reason : 0  } 
	eMail_Consume_Emoji, // { roomID :23, cnt : 0 }
	eMail_Agent_AddEmojiCnt, // { agentID : 23 ,addCnt : 23 }
	// club 
	eMail_ResponeClubApplyJoin,// { clubID : 23 , clubName : "abc", nIsAgree : 0  }
	eMail_ClubInvite , //  { clubID : 23 , clubName : "abc",mgrID : 23  }
	eMail_ClubBeKick, // { clubID : 23 , clubName : "abc",mgrID : 23  }
	eMail_ClubDismiss, // { clubID : 23 , clubName : "abc"  }
	eMail_ClubJoin, // { clubID : 23 } , sys pro
	eMail_ClubLeave, // { clubID : 23 } , sys pro

	// above is new ;
	eMail_SysOfflineEvent,// { event: concret type , arg:{ arg0: 0 , arg 1 = 3 } }  // processed in svr , will not send to client ;
	eMail_DlgNotice, // content will be send by , stMsgDlgNotice 
	eMail_Sys_End = 499,

	eMail_RealMail_Begin = 500, // will mail will show in golden server windown ;
	eMail_PlainText,  // need not parse , just display the content ;
	eMail_InvitePrize, // { targetUID : 2345 , addCoin : 300 } // you invite player to join game ,and give prize to you 
	eMail_Max,
};

public enum eMailState
{
	eMailState_Unread,
	eMailState_WaitSysAct,
	eMailState_WaitPlayerAct,
	eMailState_SysProcessed,
	eMailState_Delete,
	eMailState_PlayerProcessed,
	eMailState_Max,
};




