using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearBonus
{
    public class Music : MonoBehaviour
    {
        [SerializeField]
        private MusicController mController = null;

        [SerializeField]
        private SoundInfo mInfo = null;

        //各アイコン
        [SerializeField]
        private Image mPlayIcon = null;
        [SerializeField]
        private Image mPauseIcon = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //アイコンの表示非表示
            if (mController.Info == mInfo)
            {
                if (mController.IsPause)
                {
                    mPlayIcon.enabled = false;
                    mPauseIcon.enabled = true;
                }
                else
                {
                    mPlayIcon.enabled = true;
                    mPauseIcon.enabled = false;
                }
            }
            else
            {
                mPlayIcon.enabled = false;
                mPauseIcon.enabled = false;
            }
        }

        //クリックされたときの処理
        public void OnClick()
        {
            mController.Play(mInfo);
        }
    }
}