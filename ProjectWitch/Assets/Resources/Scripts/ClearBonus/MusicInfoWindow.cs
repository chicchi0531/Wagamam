using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearBonus
{
    public class MusicInfoWindow : MonoBehaviour
    {
        [SerializeField]
        private MusicController mController = null;

        //表示コンポーネント関係
        [SerializeField]
        private Text mTitle = null;
        [SerializeField]
        private Text mAuthor = null;

        [SerializeField]
        private Button mPlayButton = null;
        [SerializeField]
        private Button mPauseButton = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            mTitle.text = mController.Info.Title;
            mAuthor.text = mController.Info.Author;
            
            if (mController.IsPause)
            {
                mPlayButton.interactable = true;
                mPauseButton.interactable = false;
            }
            else
            {
                mPlayButton.interactable = false;
                mPauseButton.interactable = true;
            }
        }
    }
}