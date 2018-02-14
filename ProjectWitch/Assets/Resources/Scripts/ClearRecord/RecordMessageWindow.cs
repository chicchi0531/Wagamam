using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectWitch.ClearRecord
{
    public class RecordMessageWindow : MonoBehaviour {

        //テキストコンポーネント
        [SerializeField]
        private Text mcText = null;

        //テキストの表示速度
        [SerializeField]
        private float mTextSpeed = 10.0f;

        //ページ送りアイコン
        [SerializeField]
        private GameObject mNextIcon = null;

        //SE
        [SerializeField]
        private string mSE = "042_hover";

        //表示する最終テキスト
        private string mTargetText = "";

        //現在の文字数
        private int mTextCount = 0;

        //表示終了フラグ
        private bool isEnd = false;

        //改ページ可能フラグ
        public bool ToNext { get; set; }

        //最後の表示からの時間
        private float mInterval = 0.0f;

        private void Awake()
        {
            ToNext = false;
        }

        // Use this for initialization
        void Start() {
        }

        public void Init(string text)
        {
            mTargetText = text;

            isEnd = false;
            ToNext = false;
            mTextCount = 0;
            mInterval = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();
    
        if (!isEnd)
            {
                mNextIcon.SetActive(false);

                mInterval += Time.deltaTime;
                if (mInterval >= 1.0f / mTextSpeed)
                {
                    game.SoundManager.Play(mSE, SoundType.SE);
                    mTextCount++;
                }

                mcText.text = mTargetText.Substring(0, mTextCount);

                //終了フラグ
                if (mTextCount >= mTargetText.Length)
                    isEnd = true;
            }
            else
            {
                if (Input.GetButtonDown("TalkNext"))
                    ToNext = true;
                mNextIcon.SetActive(true);
            }
        }
    }
}