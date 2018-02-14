using System.Collections;
using System;
using UnityEngine;

namespace ProjectWitch.ClearBonus
{
    public class MusicController : MonoBehaviour
    {
        //トップ画面のコントローラ
        [SerializeField]
        private ClearBonusController mController = null;

        //自身のキャンバス
        [SerializeField]
        private Canvas mCanvas = null;

        //音楽のリスト
        [SerializeField]
        private Music[] mMusics = null;

        //現在再生中の番号 -1は再生なし
        private int mIndex = -1;
        public int Index { get { return mIndex; } }

        //現在のポーズ状態
        public bool IsPause { get; private set; }

        //現在の音楽情報
        public SoundInfo Info { get; set; }

        // Use this for initialization
        void Awake()
        {
            IsPause = false;
            Info = new SoundInfo(); //ダミー情報
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            if(Input.GetButtonDown("Cancel") && mController.State == EState.Music)
            {
                Close();
            }
        }

        //再生する
        public void Play(SoundInfo info)
        {
            Info = info;

            IsPause = false;
            Game.GetInstance().SoundManager.Stop(SoundType.BGM);
            Game.GetInstance().SoundManager.Play(info.CueName, SoundType.BGM);
        }

        //停止する
        public void Stop()
        {
            if (Info.ID == -1) return;

            IsPause = false;
            Game.GetInstance().SoundManager.Stop(SoundType.BGM);

            Info = new SoundInfo();//ダミー情報
        }

        //ポーズする
        public void Pause()
        {
            if (Info.ID == -1) return;

            IsPause = IsPause ? false : true;
            Game.GetInstance().SoundManager.Pause(IsPause);
        }

        public void Show()
        {
            mCanvas.enabled = true;
        }

        public void Close()
        {
            var game = Game.GetInstance();

            mCanvas.enabled = false;
            game.SoundManager.PlaySE(SE.Cancel);
            mController.ShowTop();
        }
    }

    //サウンドの情報
    [Serializable]
    public class SoundInfo
    {
        public int ID = -1;
        public string CueName = "";
        public string Title = "";
        public string Author = "";
    }
}