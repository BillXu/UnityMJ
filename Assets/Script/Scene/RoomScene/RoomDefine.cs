public enum eMJCardType
{
	eCT_None,
	eCT_Wan,
	eCT_Tong,
	eCT_Tiao,
	eCT_Feng,  // 1 dong , 2 nan , 3 xi  4 bei 
	eCT_Jian, // 1 zhong , 2 fa , 3 bai 
	eCT_Hua, 
    eCT_Max,
};

public enum eArrowDirect
{
    eDirect_Left,
    eDirect_Righ,
    eDirect_Opposite,
};

public enum eCardSate
{
    eCard_Hold,
    eCard_Out,
    eCard_Peng ,
    eCard_MingGang,
    eCard_AnGang,
    eCard_Eat,
    eCard_Hu,
};

public enum eEatType
{
	eEat_Left , // xAB
	eEat_Middle,// AxB
	eEat_Righ, // ABX,
}

public enum eClientRoomState
{
	State_WaitReady,
	State_StartGame,
	State_GameOver,
};

public enum eChatMsgType
{
	eChatMsg_InputText,
	eChatMsg_Voice,
	eChatMsg_Emoji,
	eChatMsg_SysText,
	eChatMsg_Max,
}

