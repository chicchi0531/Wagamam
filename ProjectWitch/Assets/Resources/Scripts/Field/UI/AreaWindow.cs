using UnityEngine;
using UnityEngine.UI;
using System;

namespace ProjectWitch.Field
{
    [RequireComponent(typeof(Animator))]
    public class AreaWindow : MonoBehaviour
    {

        //背景画像のパス
        private string mBackImagePath = "Textures/Field/Back/";

        //各オブジェクト
        [Space(1)]
        [Header("Parts")]
        [SerializeField]
        private Text mcOwner = null;
        [SerializeField]
        private Text mcTime = null;
        [SerializeField]
        private Text mcMana = null;
        [SerializeField]
        private Text mcPAtk = null;
        [SerializeField]
        private Text mcMAtk = null;
        [SerializeField]
        private Text mcPDef = null;
        [SerializeField]
        private Text mcMDef = null;
        [SerializeField]
        private Text mcLead = null;
        [SerializeField]
        private Text mcAgi = null;
        [SerializeField]
        private Image mcBack = null;
        [SerializeField]
        private Image mcOwnerIcon = null;

        //ボタン類
        [Space(1)]
        [Header("ボタン類")]
        [SerializeField]
        private GameObject mDominationButton = null;
        [SerializeField]
        private GameObject mManaButton = null;
        [SerializeField]
        private GameObject mEventTownButton = null;
        [SerializeField]
        private GameObject mTownButton = null;

        [Space(1)]

        //領主画像
        [SerializeField]
        private Sprite[] mcOwnerIconResources = null;

        //閉じるときのSE
        [SerializeField]
        private string mCloseSEName = "162";


        //エリアID
        private int mAreaID = -1;
        public int AreaID { get { return mAreaID; } set { mAreaID = value; } }

        //コントローラ
        public FieldController FieldController { get; set; }
        public FieldUIController FieldUIController { get; set; }

        //呼び出し元のFlagButtonへの参照
        public GameObject AreaNamePrefab { get; set; }
        private GameObject mInstNameWindow = null;

        //アニメータの参照
        private Animator mAnimator = null;

        void Start()
        {
            mAnimator = GetComponent<Animator>();
        }

        void Update()
        {
            var game = Game.GetInstance();

            //ダイアログが出ていたら何もしない
            if (game.IsDialogShowd) return;

            //トーク中なら何もしない
            if (game.IsTalk) return;

            if (Input.GetButtonDown("Cancel"))
            {
                Close();
            }
        }

        public void Init()
        {
            try
            {
                var game = Game.GetInstance();

                //名前ウィンドウ再生成
                mInstNameWindow = Instantiate(AreaNamePrefab);
                mInstNameWindow.transform.SetParent(this.transform.parent,false);
                var comp = mInstNameWindow.GetComponent<AreaName>();
                comp.AreaID = AreaID;
                comp.Init();

                //areaデータ取得
                if (AreaID == -1) throw new ProjectWitchException("エリアIDをセットしてください");
                var area = game.GameData.Area[AreaID];

                //テキストデータのセット
                mcOwner.text = game.GameData.Territory[area.Owner].OwnerName;
                mcTime.text = area.Time.ToString();
                mcMana.text = area.Mana.ToString();
                mcPAtk.text = area.BattleFactor.PAtk.ToString() + "%";
                mcMAtk.text = area.BattleFactor.MAtk.ToString() + "%";
                mcPDef.text = area.BattleFactor.PDef.ToString() + "%";
                mcMDef.text = area.BattleFactor.MDef.ToString() + "%";
                mcLead.text = area.BattleFactor.Leadership.ToString() + "%";
                mcAgi.text = area.BattleFactor.Agility.ToString() + "%";

                //領主画像のセット
                mcOwnerIcon.sprite = mcOwnerIconResources[area.Owner];
                mcOwnerIcon.SetNativeSize();

                //背景画像のセット
                var resoruce = Resources.Load<Sprite>(mBackImagePath + area.BackgroundName);
                mcBack.sprite = resoruce;

                //ボタンの表示非表示
                ShowButton(area);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError(e.Message + " : Inspectorの参照が切れていないかかくにんしてください");
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message + " : AreaIDかTerritoryIDが不正の可能性があります");
            }
            catch (ProjectWitchException e)
            {
                Debug.LogError(e.Message);
            }



        }

        //戦闘を呼び出す
        public void CallBattle()
        {
            var game = Game.GetInstance();

            //侵攻戦の開始
            FieldController.DominationBattle(AreaID, game.GameData.Area[AreaID].Owner);
            
            //メニューを閉じる
            Close(true);


            //メニューを開けなくする
            FieldController.MenuClickable = false;
            FieldController.FlagClickable = false;
        }

        //臨時徴収を呼びだす
        public void CallMana()
        {
            //マナの収集
            FieldController.GetMana(AreaID);
            
            //メニューを閉じる
            Close(true);
        }

        //町へ行く（イベント）ボタン
        public void CallTown()
        {
            //マナの収集
            FieldController.CallTown(AreaID);

            //メニューを閉じる
            Close(true);
        }

        //魔法屋ボタン
        public void CallMagic()
        {
            //マナの収集
            FieldController.CallMagicShop(AreaID);

            //メニューを閉じる
            Close(true);
        }

        //道具屋ボタン
        public void CallTool()
        {
            //マナの収集
            FieldController.CallToolShop(AreaID);

            //メニューを閉じる
            Close(true);
        }

        //ウィンドウを閉じる
        public void Close(bool noAnim = false)
        {
            Destroy(mInstNameWindow);

            if (noAnim)
            {
                //メニューを開けるようにする 
                FieldController.MenuClickable = true;
                FieldController.FlagClickable = true;
                FieldUIController.AreaNameLock = false;
                FieldUIController.SelectedTerritory = -1;
                Destroy(this.gameObject);
            }
            else
            {
                //SE再生
                var game = Game.GetInstance();
                game.SoundManager.Play(mCloseSEName, SoundType.SE);

                //アニメーション再生
                mAnimator.SetTrigger("close");
            }
        }

        //クローズアニメーションが終わったら呼ぶ処理
        public void EndAnimation()
        {
            //メニューを開けるようにする 
            FieldController.MenuClickable = true;
            FieldController.FlagClickable = true;
            FieldUIController.AreaNameLock = false;
            FieldUIController.SelectedTerritory = -1;
            Destroy(this.gameObject);
        }

        //敵の領地なら侵攻可能か判断して侵攻ボタンを
        //味方の領地なら臨時領収ボタンなどを表示
        private void ShowButton(AreaDataFormat area)
        {
            if (AreaDataFormat.IsPlayerArea(area))
            {
                switch (area.Type)
                {
                    case AreaDataFormat.AreaType.Default:
                        mManaButton.SetActive(true);
                        break;
                    case AreaDataFormat.AreaType.EventTown:
                        mEventTownButton.SetActive(true);
                        break;
                    case AreaDataFormat.AreaType.Town:
                        mTownButton.SetActive(true);
                        break;
                    default:
                        mManaButton.SetActive(true);
                        break;
                }
            }
            else
            {
                //その領地が侵攻可能な領地かどうか判定
                if (AreaDataFormat.IsDomiatableArea(area))
                    mDominationButton.SetActive(true);
            }
        }


    }
}