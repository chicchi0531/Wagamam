using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ProjectWitch;

namespace ProjectWitch.ClearBonus
{
    public enum EState { Top, Museum, Music };

    public class ClearBonusController : MonoBehaviour
    {
        //各コントローラ
        [SerializeField]
        private MusicController mMusicController = null;
        [SerializeField]
        private MuseumController mMuseumController = null;

        //メッセージウィンドウ
        [SerializeField]
        private TopMessage mMessageWindow = null;

        //キャラクター投票のURL
        [SerializeField]
        private string mCharacterRankingURL = "https://goo.gl/forms/MV47xFO0qU8ns11Q2";

        //一回でも訪れたことがあるかどうかのフラグ
        [SerializeField]
        private int mHasWentFlagID = 23;
        public bool HasWent()
        {
            var game = Game.GetInstance();
            return game.SystemData.Memory[mHasWentFlagID] != 0;
        }

        [SerializeField]
        private string mBGMName = "020_event5";

        //現在どこの画面にいるかのステート
        public EState State { get; private set; }

        //連続で画面が閉じるのを防ぐウェイト地
        private float mWaitTime = 0.0f;

        private void Awake()
        {
            State = EState.Top;
        }

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            game.HideNowLoading();
            game.SoundManager.Play(mBGMName, SoundType.BGM);
        }

        // Update is called once per frame
        void Update()
        {
            if (mWaitTime - Time.deltaTime <= 0.0f)
            {
                if (Input.GetButtonDown("Cancel") && State == EState.Top)
                {
                    Close();
                }
            }
            mWaitTime -= Time.deltaTime;
        }

        public void Close()
        {
            var game = Game.GetInstance();

            //SE
            game.SoundManager.PlaySE(SE.Cancel);

            //訪問済みフラグを立てる
            game.SystemData.Memory[mHasWentFlagID] = 1;

            //タイトルを呼び出す
            StartCoroutine(game.CallTitle());
        }

        //美術館へ
        public void OnClick_Museum()
        {
            OnClickBase();
            State = EState.Museum;
            mMuseumController.Show();
        }
        
        //サウンドルームへ
        public void OnClick_Sound()
        {
            OnClickBase();

            State = EState.Music;
            mMusicController.Show();
            
        }

        //トップ画面に戻るときの処理
        public void ShowTop()
        {
            State = EState.Top;
            mWaitTime = 0.5f;
            mMessageWindow.Init();
        }

        //キャラクターランキングへ
        public void OnClick_CharacterRanking()
        {
            Application.OpenURL(mCharacterRankingURL);
        }

        //クリックされたときに呼び出される固定の項目
        void OnClickBase()
        {
            var game = Game.GetInstance();

            //メッセージウィンドウを表示済みにする
            mMessageWindow.AnimationSkip();

            //訪問済みフラグを立てる
            game.SystemData.Memory[mHasWentFlagID] = 1;
            
        }
    }
}