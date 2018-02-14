using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.ClearRecord
{
    public class RecordController : MonoBehaviour
    {

        [SerializeField]
        private string mBGM = "003_alice2";

        //各シーンのゲームオブジェクト
        [SerializeField]
        private GameObject mScene1 = null;
        [SerializeField]
        private GameObject mScene2 = null;

        //各シーンコントローラ
        [SerializeField]
        private RecordScene1Controller mController1;
        [SerializeField]
        private RecordScene2Controller mController2;

        //メッセージウィンドウ
        [SerializeField]
        private RecordMessageWindow mMessageWindow;

        //さようならメッセージ
        [SerializeField,Multiline(4)]
        private string[] mByeMessage;

        //全員達成で立てるフラグ
        [SerializeField]
        private int mGoodnessFlag = 0022;

        //人数
        public int CharacterCount { get; set; }
        //幸福の人数
        public int GoodCound { get; set; }

        private void Awake()
        {
            CharacterCount = 0;
            GoodCound = 0;
        }

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
            game.SoundManager.Play(mBGM, SoundType.BGM);

            game.HideNowLoading();

            StartCoroutine(_Update());
        }

        // Update is called once per frame
        IEnumerator _Update()
        {
            //シーン1開始
            mScene1.SetActive(true);
            yield return new WaitForEndOfFrame();
            mController1.Begin();
            while (!mController1.ToNext) yield return null;
            mScene1.SetActive(false);

            //シーン２開始
            mScene2.SetActive(true);
            yield return new WaitForEndOfFrame();
            mController2.Begin();
            while (!mController2.ToNext) yield return null;
            mScene2.SetActive(false);

            //終了メッセージを出して終了
            foreach(var m in mByeMessage)
            {
                mMessageWindow.Init(m);
                while (!mMessageWindow.ToNext) yield return null;
            }

            //全員達成でフラグ立てる
            if (GoodCound == CharacterCount)
                Game.GetInstance().SystemData.Memory[mGoodnessFlag] = 1;

            yield return StartCoroutine(Game.GetInstance().CallTitle());
        }
    }
}