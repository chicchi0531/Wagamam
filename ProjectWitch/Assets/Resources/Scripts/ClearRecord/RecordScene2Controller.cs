using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearRecord
{
    public class RecordScene2Controller : MonoBehaviour
    {
        //コントローラ
        [SerializeField]
        private RecordController mController = null;

        //一つ当たりの表示タイム
        [SerializeField]
        private float mTimeSpan = 0.8f;

        //SE
        [SerializeField]
        private string mSE1 = "045_shot";
        [SerializeField]
        private string mSE2 = "030_domination";

        //表示するゲームオブジェクト
        [SerializeField]
        private GameObject[] mObjects = null;

        //人数を入れるテキストコンポーネント
        [SerializeField]
        private Text mText = null;

        //メッセージウィンドウ
        [SerializeField]
        private RecordMessageWindow mMessageWindow = null;

        //表示するテキスト,
        //0,少ない、普通、全員の4パターン
        [SerializeField, Multiline(4)]
        private string[] mMessages = null;


        //次へボタン
        [SerializeField]
        private Button mNextButton = null;

        //終了フラグ
        private bool mIsEnd = false;

        //次へ行くフラグ
        public bool ToNext { get; private set; }

        private void Awake()
        {
            ToNext = false;
            foreach (var o in mObjects) o.SetActive(false);
        }

        // Use this for initialization
        public void Begin()
        {
            //テキストセット
            mText.text = mController.GoodCound + "人幸福！";

            mMessageWindow.Init("");
            StartCoroutine(_Update());
        }

        // Update is called once per frame
        void Update()
        {
        }
        
        IEnumerator _Update()
        {
            var game = Game.GetInstance();

            mNextButton.interactable = false;

            yield return new WaitForSeconds(mTimeSpan);

            foreach (var obj in mObjects)
            {
                obj.SetActive(true);
                game.SoundManager.Play(mSE1, SoundType.SE);

                yield return new WaitForSeconds(mTimeSpan);
            }

            //ジングル再生
            game.SoundManager.Play(mSE2,SoundType.SE);
            yield return new WaitForSeconds(1.0f);

            //メッセージ選択
            var mes = "";
            var char_count = mController.CharacterCount;
            var good_count = mController.GoodCound;
            if (good_count == 0)
                mes = mMessages[0];
            else if (good_count < char_count / 2)
                mes = mMessages[1];
            else if (good_count != char_count)
                mes = mMessages[2];
            else
                mes = mMessages[3];

            //メッセージ置換
            mes = mes.Replace("[0]", good_count.ToString());

            //メッセージ表示
            mMessageWindow.Init(mes);

            while (!mMessageWindow.ToNext) yield return null;

            //次へボタンを表示
            mNextButton.interactable = true;

            mIsEnd = true;
        }


        public void OnClick_Next()
        {
            ToNext = true;
        }

    }
}