﻿using UnityEngine.UI;
using UnityEngine;
using System.Linq;

namespace ProjectWitch.Shop.Magic
{
    public class SellItem : BaseShopItem
    {
        //所持数
        [SerializeField]
        private Text mNum = null;

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();
            var item = game.GameData.Card[ItemID];
            var itemList = game.GameData.Group[game.GameData.Territory[0].GroupList[0]].CardList;

            //名前をセット
            mName.text = item.Name;

            //個数をセット
            var num = (itemList.Where(x => x == ItemID).ToList()).Count;
            mNum.text = num.ToString();

            //価格をセット
            var price = game.GameData.Card[ItemID].SellingPrice;
            mPrice.text = price.ToString();
        }

        public override void OnClicked()
        {
            base.OnClicked();

            //売却可能な最大数をリセット
            ItemNum.MaxNum = int.Parse(mNum.text);
        }
    }
}