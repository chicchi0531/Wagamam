using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

namespace ProjectWitch.Field
{
    public class FlagButton : MonoBehaviour
        , IPointerEnterHandler
        , IPointerExitHandler
    {

        //地点番号
        public int AreaID { get; set; }

        //領主番号
        private int mTerritoryID = -1;

        //各エリアメニューのプレハブ
        [SerializeField]
        private GameObject mAreaPrefab = null;
        [SerializeField]
        private GameObject mAreaNamePrefab = null;

        //サブエフェクトへの参照
        [SerializeField]
        private Animator mAnimSubEffect = null;
        [SerializeField]
        private Image mImageSubEffect = null;

        //サブエフェクトで表示する画像
        [SerializeField]
        private Sprite[] mSpritesSubEffect = null;

        //押したときのSE
        [SerializeField]
        private string mPushSEName = "161";

        //各コントローラ
        public FieldUIController FieldUIController { get; set; }
        public FieldController FieldController { get; set; }

        //生成した子オブジェクトのインスタンス
        private GameObject mInstAreaWindow = null;
        private GameObject mInstAreaName = null;

        //ボタンコントローラ
        private Button mcButton = null;

        //コルーチンが発動中かどうか
        private bool mCoroutineIsDoing = false;

        void Start()
        {
            var game = Game.GetInstance();
            mTerritoryID = game.GameData.Area[AreaID].Owner;

            var obj = GameObject.FindWithTag("FieldController");
            FieldController = obj.GetComponent<FieldController>();

            mcButton = GetComponent<Button>();
            if (!mcButton) Debug.LogError("Buttonコンポーネントを付けてください");
        }

        void Update()
        {
            if (FieldController.FlagClickable)
            {
                mcButton.interactable = true;
                mAnimSubEffect.SetBool("IsShow", true);
            }
            else
            {
                CloseAreaName();
                mcButton.interactable = false;
                mAnimSubEffect.SetBool("IsShow", false);
            }

            if (!mCoroutineIsDoing)
                StartCoroutine(_Update());
        }

        private IEnumerator _Update()
        {
            SetSubEffectImage();
            yield return new WaitForSeconds(0.5f);
            yield return null;
        }

        //サブエフェクトの画像を差し替える
        private void SetSubEffectImage()
        {
            var game = Game.GetInstance();
            var area = game.GameData.Area[AreaID];

            if (AreaDataFormat.IsPlayerArea(area))
            {
                switch (area.Type)
                {
                    case AreaDataFormat.AreaType.Default:
                        //プレイヤーの領地で、アイテム取得可能なら、アイテムアイコンを表示
                        if (area.HasItem && area.ItemID != -1)
                        {
                            mImageSubEffect.sprite = mSpritesSubEffect[0];
                        }
                        //マナが大量に残っていたらマナアイコンを表示
                        else if (AreaDataFormat.AreaManaIsLarge(area))
                        {
                            mImageSubEffect.sprite = mSpritesSubEffect[3];
                        }
                        else
                        {
                            mImageSubEffect.sprite = mSpritesSubEffect[5];
                        }
                        break;

                    case AreaDataFormat.AreaType.Town:
                        mImageSubEffect.sprite = mSpritesSubEffect[2];
                        break;

                    case AreaDataFormat.AreaType.EventTown:
                        mImageSubEffect.sprite = mSpritesSubEffect[4];
                        break;

                    default:
                        mImageSubEffect.sprite = mSpritesSubEffect[5];
                        break;
                }
            }
            else
            {
                //その領地が侵攻可能な領地かどうか判定
                if (AreaDataFormat.IsDomiatableArea(area))
                    mImageSubEffect.sprite = mSpritesSubEffect[1];
                else
                    mImageSubEffect.sprite = mSpritesSubEffect[5];
            }
        }

        //メニューを開く
        public void OpenMenu()
        {
            var game = Game.GetInstance();

            //クリック音再生
            game.SoundManager.Play(mPushSEName, SoundType.SE);

            //メニューを開く
            ShowAreaWindow(mAreaPrefab);

            //フラグメニューの２重起動防止
            FieldController.FlagClickable = false;

            //メニュー操作無効
            FieldController.MenuClickable = false;

            //オーナーパネルをロック
            FieldUIController.AreaNameLock = true;
            
        }

        //エリアウィンドウの表示処理
        private void ShowAreaWindow(GameObject menu)
        {
            //描画先のキャンバス
            var canvas = FieldUIController.CameraCanvas;

            //メニュープレハブを生成
            mInstAreaWindow = Instantiate(menu);
            mInstAreaWindow.transform.SetParent(canvas.transform, false);

            var comp = mInstAreaWindow.GetComponent<AreaWindow>();
            comp.AreaID = AreaID;
            comp.FieldController = FieldController;
            comp.FieldUIController = FieldUIController;
            comp.AreaNamePrefab = mAreaNamePrefab;
            comp.Init();

            //エリアウィンドウ側で再生成するので、ネームウィンドウは削除
            CloseAreaName();
        }

        //マウスがポップしたときのイベント
        public void OnPointerEnter(PointerEventData e)
        {
            var game = Game.GetInstance();

            //ホバー音再生
            game.SoundManager.PlaySE(SE.Hover);


            if (FieldUIController.AreaNameLock == false)
            {
                FieldUIController.SelectedTerritory = mTerritoryID;
                ShowAreaName();
            }
        }

        //マウスが外れた時のイベント
        public void OnPointerExit(PointerEventData e)
        {
            CloseAreaName();
        }

        //AreaNameウィンドウを表示
        private void ShowAreaName()
        {
            //描画先のキャンバス
            var canvas = FieldUIController.CameraCanvas;

            mInstAreaName = Instantiate(mAreaNamePrefab);
            mInstAreaName.transform.SetParent(canvas.transform, false);

            var comp = mInstAreaName.GetComponent<AreaName>();
            comp.AreaID = AreaID;
            comp.Init();
        }

        //エリア名を閉じる
        private void CloseAreaName()
        {
            Destroy(mInstAreaName);
            FieldUIController.SelectedTerritory = -1;
        }
    }
}