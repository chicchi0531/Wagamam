using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.ClearRecord
{
    public class RecordScene1Controller : MonoBehaviour {

        //コントローラ
        [SerializeField]
        private RecordController mController = null;

        //一つ当たりの表示タイム
        [SerializeField]
        private float mTimeSpan = 0.3f;

        //スクロールを開始するインデックス
        [SerializeField]
        private int mScrollBeginIndex = 4;

        //スクロールバーへの参照
        [SerializeField]
        private Scrollbar mScrollBar = null;

        //スクロール速度
        [SerializeField]
        private float mScrollSpeed = 1.0f;

        //アイテムデータ
        [SerializeField]
        private GameObject mItemParents = null;
        private RecordListItem[] mItems = null;

        //メッセージウィンドウ
        [SerializeField]
        private RecordMessageWindow mMessageWindow = null;

        //表示するテキスト
        [SerializeField, Multiline(4)]
        private string[] mMessages = null;

        //次へボタン
        [SerializeField]
        private Button mNextButton = null;

        //ターゲットポジション
        private float mTargetPosition = 0.0f;

        //現在のポジション
        private float mCurrentPosition = 0.0f;

        //終了フラグ
        private bool mIsEnd = false;

        //次へ行くフラグ
        public bool ToNext { get; private set; }

        private void Awake()
        {
            ToNext = false;
        }

        // Use this for initialization
        public void Begin() {
            //リストアイテムを取得
            mItems = mItemParents.GetComponentsInChildren<RecordListItem>();

            StartCoroutine(_Update());
        }

        // Update is called once per frame
        void Update() {

            if (!mIsEnd)
            {
                //スクロールバーをスクロール
                var delta = mTargetPosition - mCurrentPosition;
                mCurrentPosition += delta * mScrollSpeed * Time.deltaTime;
                mScrollBar.value = mCurrentPosition;
            }
        }
        


        IEnumerator _Update()
        {
            mNextButton.interactable = false;

            //テキストをすべて表示するまで待つ
            foreach (var m in mMessages)
            {
                mMessageWindow.Init(m);
                while (!mMessageWindow.ToNext) yield return null;
            }

            //人の数をカウント
            mController.CharacterCount = mItems.Length;

            for (int next=0;next<mItems.Length;next++)
            {
                //ターゲット位置を求める
                if (next >= mScrollBeginIndex)
                {
                    mTargetPosition = (float)(next+1) / (mItems.Length);
                }
                else
                    mTargetPosition = 0.0f;
                
                //アイテムを表示
                mItems[next].ShowIcon();

                //幸福の数をカウント
                if(mItems[next].IsGood) mController.GoodCound++;

                yield return new WaitForSeconds(mTimeSpan);
                
            }

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