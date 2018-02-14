using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearBonus
{
    public class MuseumItem : MonoBehaviour
    {

        //コントローラ
        [SerializeField]
        private MuseumController mController = null;

        //CG解除のフラグ
        [SerializeField]
        private int mUnlockID = -1;

        //ロックスクリーンへの参照
        [SerializeField]
        private Image mLockScreen = null;

        //表示CGへの参照
        [SerializeField]
        private Image mImage = null;

        //ボタンへの参照
        [SerializeField]
        private Button mButton = null;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            if (mUnlockID == -1 || game.SystemData.Memory[mUnlockID] != 0)
            {
                mButton.interactable = true;
                mLockScreen.enabled = false;
            }
            else
            {
                mButton.interactable = false;
                mLockScreen.enabled = true;
            }
        }

        //クリック時の動作
        public void OnClick()
        {
            mController.ShowScaleImage(mImage.sprite);
        }
    }
}