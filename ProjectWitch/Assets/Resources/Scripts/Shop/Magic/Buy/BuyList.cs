﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Shop.Magic
{
    public class BuyList : BaseList
    {

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();

            //子供を追加
            for (int i = 0; i < game.GameData.Card.Count; i++)
            {
                //フラグを満たしていない商品は除外
                if (!game.GameData.Card[i].ShopList.Contains(game.TownDataIn.AreaID)) continue;

                var inst = Instantiate(mListContent);
                var cp = inst.GetComponent<BuyItem>();
                cp.ItemID = i;
                cp.InfoWindow = mInfoWindow;
                cp.ItemNum = mItemNum;
                cp.MesBox = mMessageBox;
                cp.Reset();

                inst.transform.SetParent(mListContentParent.transform, false);
            }
        }
    }
}