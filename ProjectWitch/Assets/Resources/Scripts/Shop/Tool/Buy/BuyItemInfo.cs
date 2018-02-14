using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop.Tool
{
    [RequireComponent(typeof(Animator))]
    public class BuyItemInfo : ToolBaseItemInfo
    {
        [SerializeField]
        private Button mBuyButton = null;

        public override void Reset()
        {
            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Equipment[ItemID];

                var nextMana = game.GameData.PlayerMana - (item.BuyingPrice * ItemNum);
                mNextManaWindow.SetMana(nextMana);

                //マナが足りない場合は購入できないようにする
                if (nextMana > 0) mBuyButton.interactable = true;
                else mBuyButton.interactable = false;

                //文字列にアイテム名を差し込む
                var messageA = mMessageA.Replace("[0]", item.Name);
                messageA = messageA.Replace("[1]", (item.BuyingPrice * ItemNum).ToString());
                messageA = messageA.Replace("[2]", ItemNum.ToString());
                var messageB = mMessageB.Replace("[0]", item.Name);
                messageB = messageB.Replace("[1]", ItemNum.ToString());

                //メッセージセット、マナが足りるか足りないかでメッセージが変わる
                if (nextMana > 0)
                    mMessageBox.SetText(mMesNameA, messageA);
                else
                    mMessageBox.SetText(mMesNameB, messageB);
            }

            base.Reset();
        }

        public void ClickBuyButton()
        {
            //プレイヤーのデータに装備データを入れる
            var game = Game.GetInstance();
            for (int i = 0; i < ItemNum; i++)
                game.GameData.Territory[0].EquipmentList[ItemID].Add(-1);

            //マナを減らす
            game.GameData.PlayerMana -= (game.GameData.Equipment[ItemID].BuyingPrice * ItemNum);

            //メッセージを表示
            mMessageBox.SetText(mMesNameC, mMessageC);
            
            //データをリセット
            mList.Reset();
            

            Close();
        }
    }
}