using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICardHoldAn
{
    void clear();
    void refresh( List<int> vCards, int nCnt ) ;
    void onMo( int nCard, Vector3 ptWallCardWorldPos ); // when mo , we need keep space

    void onHu( int nCard );
    void onDistribute( List<int> vCards , int nCnt );
    Vector3 doChu( int nCard );
    void removeCard( int nCard, int cnt );
    void onWaitChu();
    void rearrangeCard();
    void showCards( List<int> vCards ) ; // when game end do shou cards ;
    float getHoldAnXSize();
}
