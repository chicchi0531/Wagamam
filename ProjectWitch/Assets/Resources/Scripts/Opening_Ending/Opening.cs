using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

namespace ProjectWitch
{
    public class Opening : MonoBehaviour
    {

        //最初に実行するスクリプト
        [SerializeField]
        private string mScript_First = "s0000";

        [SerializeField]
        private int mTutorialFlag = 4012;

        //戦闘勝利後に実行するスクリプト
        [SerializeField]
        private int mNextScriptA = 1;

        //戦闘敗北後に実行するスクリプト
        [SerializeField]
        private int mNextScriptB = 1;

        //チュートリアルパネル
        [SerializeField]
        private Animator mChoiceTutorial = null;

        //動画
        [SerializeField]
        private VideoPlayer mVideoPlayer = null;

        //動画のオーディオソース
        [SerializeField]
        private AudioSource mAudioSource = null;

        [SerializeField]
        private float mSkipTime = 2.0f;
        private float mSkipProgress = 0.0f;

        [SerializeField]
        private Canvas mSkipGUI = null;
        [SerializeField]
        private Image mSkipProgressImage = null;

        //動画終了待機
        private bool mMovieEnd = false;

        // Use this for initialization
        void Start()
        {
            //音量調整
            var game = Game.GetInstance();
            mAudioSource.volume = game.SystemData.Config.BGMVolume * game.SystemData.Config.MasterVolume;

            //skipGUI非表示
            mSkipGUI.enabled = false;

            StartCoroutine(CoOpening());

        }

        private void Update()
        {
            if (Input.GetButton("Submit") && mVideoPlayer.isPlaying)
            {
                mSkipProgress += Time.deltaTime;
                mSkipGUI.enabled = true;
                mSkipProgressImage.fillAmount = mSkipProgress / mSkipTime;
            }
            else
            {
                mSkipProgress = 0.0f;
                mSkipGUI.enabled = false;
            }

            if ((long)mVideoPlayer.frameCount == mVideoPlayer.frame || mSkipProgress >= mSkipTime)
            {
                mMovieEnd = true;
                mVideoPlayer.Stop();
            }
        }


        //イベント制御
        private IEnumerator CoOpening()
        {
            var game = Game.GetInstance();

            //イベントデータの設定
            var ev = new EventDataFormat();
            ev.FileName = mScript_First;
            ev.NextA = mNextScriptA;
            ev.NextB = mNextScriptB;

            //スクリプト実行
            game.CallScript(ev);
            yield return null;
            while (game.IsTalk) yield return null;

            //戦闘オンだったら戦闘開始
            if (game.BattleIn.IsEvent)
                yield return StartCoroutine(CallBattle(game.BattleIn.AreaID, 0, true));

            //ビデオ再生
            mVideoPlayer.Play();
            yield return null;
            mMovieEnd = false;

            //ビデオ終了待機
            while (!mMovieEnd) yield return null;

            //チュートリアルを開始するかの選択肢を表示
            mChoiceTutorial.SetBool("IsShow", true);

            yield break;
        }

        //戦闘処理
        private IEnumerator CallBattle(int area, int territory, bool invation)
        {
            var game = Game.GetInstance();

            //戦闘情報の格納
            game.BattleIn.AreaID = area;
            game.BattleIn.EnemyTerritory = territory;
            game.BattleIn.IsInvasion = invation;

            //戦闘呼び出し
            yield return StartCoroutine(game.CallPreBattle());
            yield return null;

            //戦闘終了まで待機
            while (game.IsBattle) yield return null;

            //戦闘終了処理
            yield return StartCoroutine(AfterBattle());
        }

        //戦闘後処理
        public IEnumerator AfterBattle()
        {
            var game = Game.GetInstance();

            //戦闘情報をリセット
            game.BattleIn.Reset();

            //戦闘後スクリプトの開始
            //勝敗で実行されるスクリプトの分岐
            //戦闘後スクリプトの終了
            if (game.BattleOut.IsWin)    //戦闘勝利時のスクリプト
            {
                var exescript = game.ScenarioIn.NextA;
                if (exescript >= 0)
                    game.CallScript(game.GameData.FieldEvent[exescript]);
                yield return null;
            }
            else                        //戦闘敗北時の
            {
                var exescript = game.ScenarioIn.NextB;
                if (exescript >= 0)
                    game.CallScript(game.GameData.FieldEvent[exescript]);
                yield return null;

            }
            while (game.IsTalk) yield return null;

            game.BattleIn.Reset();
            game.ScenarioIn.Reset();

            yield return null;
        }

        //チュートリアルアリをクリック
        public void OnClickTutorialOn()
        {
            var game = Game.GetInstance();

            game.GameData.Memory[mTutorialFlag] = 1;
            StartCoroutine(game.CallField());
        }

        //チュートリアルなしをクリック
        public void OnClickTutorialOff()
        {
            var game = Game.GetInstance();
            StartCoroutine(game.CallField());
        }
    }

}