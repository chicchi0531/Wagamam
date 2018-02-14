using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.ClearRecord
{
    public class RecordListItem : MonoBehaviour {

        //アイコン
        [SerializeField]
        private GameObject mGoodIcon = null;
        [SerializeField]
        private GameObject mBadIcon = null;

        //条件
        //生存が必要なユニット
        [SerializeField]
        private int[] mAliveUnit = new int[] { };
        //折れていることが必要なゲームフラグ（ノーマルエンド回避くらいしか条件がない）
        [SerializeField]
        private int[] mBadFlags = new int[] { };

        //表示するときのSE
        [SerializeField]
        private string mIconSE = "044_kick";

        //グッドかどうかのフラグ
        public bool IsGood { get; private set; }

        private void Awake()
        {
            var game = Game.GetInstance();
            var result = true;

            //フラグ判定
            foreach (var f in mBadFlags)
            {
                if (game.GameData.Memory[f] != 0)
                    result = false;
            }

            //ユニットに対する判定
            foreach (var u in mAliveUnit)
            {
                //自領地にいるかの判定
                var g = game.GameData.Territory[0].GroupList[0];
                var units = game.GameData.Group[g].UnitList;

                if (!units.Contains(u))
                    result = false;

                //生きているかの判定
                if (game.GameData.Unit[u].IsAlive == false)
                    result = false;
            }

            IsGood = result;

        }

        //アイコンを表示する
        public void ShowIcon()
        {
            var game = Game.GetInstance();
            
            //SE再生
            game.SoundManager.Play(mIconSE, SoundType.SE);

            //アイコンを表示
            if (IsGood)
                mGoodIcon.SetActive(true);
            else
                mBadIcon.SetActive(true);
        }
    }
}