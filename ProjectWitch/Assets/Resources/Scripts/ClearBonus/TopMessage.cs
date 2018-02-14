using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearBonus
{
    public class TopMessage : MonoBehaviour
    {
        //コントローラ
        [SerializeField]
        private ClearBonusController mController = null;

        //テキストのポポポ音
        [SerializeField]
        private string mTextSE = "042_hover";

        //テキストの速さ
        [SerializeField]
        private int mTextSpeed = 10;

        //表示するテキスト
        //先頭のテキストは最初に訪れたときのみ表示される
        [SerializeField,Multiline(4)]
        private string[] mMessages = null;

        //表示するスペシャルテキスト
        [SerializeField,Multiline(4)]
        private string[] mSpecialMessages = null;
        private int mSpecialMessageID = -1;
        private bool mIsSpecialMessage = false;

        private Text mcText = null;

        //表示する文字数
        private int mCharacterCount = 0;

        //選択されたメッセージのインデックス
        private int mMessageID = 0;

        //経過時間
        private float mInterval = 0.0f;


        private void Awake()
        {
            mcText = GetComponent<Text>();
        }

        // Use this for initialization
        void Start()
        {
            Init();
        }

        public void Init()
        {
            if(mController.HasWent())
            {
                //初回ではないので、ランダムな開発秘話を表示
                mMessageID = Random.Range(1, mMessages.Length - 1);
            }
            else
            {
                //初回なので、0番目のテキストを表示
                mMessageID = 0;
            }

            mCharacterCount = 0;
            mInterval = 0.0f;
        }

        //文字送りをスキップして、最後まで表示済みにします
        public void AnimationSkip()
        {
            //表示するテキスト
            var str = mIsSpecialMessage ? mSpecialMessages[mSpecialMessageID] : mMessages[mMessageID];

            mCharacterCount = str.Length;
        }

        //すぺしゃるなボタンを押したときの動作
        public void OnClick_SpecialButton()
        {
            //呼び出しまくるとゲームを落とす
            if (++mSpecialMessageID >= mSpecialMessages.Length)
            {
                Debug.LogWarning("ゲームを落としました");

                //確認画面なしでアプリケーションを落とす
                Game.GetInstance().IsEnableQuitWindow = false;
                Application.Quit();

                mSpecialMessageID = 0; //エディタ実行用
            }

            mCharacterCount = 0;
            mTextSpeed = 60;
            mIsSpecialMessage = true;
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            //表示するテキスト
            var str = mIsSpecialMessage ? mSpecialMessages[mSpecialMessageID] : mMessages[mMessageID];

            mInterval += Time.deltaTime;
            if(mInterval >= 1.0f / mTextSpeed &&
                mCharacterCount < str.Length)
            {
                mInterval = 0.0f;
                mCharacterCount++;

                //Se再生
                //game.SoundManager.Play(mTextSE,SoundType.SE);
            }

            //テキストを先頭からの指定文字数で表示
            mcText.text = str.Substring(0, mCharacterCount);
        }
    }

}