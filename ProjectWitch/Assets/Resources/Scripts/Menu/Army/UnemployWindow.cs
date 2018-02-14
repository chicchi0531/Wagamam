using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace ProjectWitch.Menu
{
    public class UnemployWindow : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPanelA = null;
        [SerializeField]
        private GameObject mPanelB = null;

        [SerializeField]
        private Text mPanelBText = null;

        [Header("取得アイテムのレベル境界")]
        [SerializeField]
        private int mItemLevelA = 30;
        [SerializeField]
        private int mItemLevelB = 50;

        [Header("取得アイテム")]
        [SerializeField]
        private int mItemA = 58;
        [SerializeField]
        private int mItemB = 59;
        [SerializeField]
        private int mItemC = 60;

        [SerializeField]
        private ArmyMenu mArmyMenu = null;
        [SerializeField]
        public StatusWindow mStatusWindow = null;
        [SerializeField]
        public UnitList mList = null;

        //ID
        public int UnitID { get; set; }

        // Use this for initialization
        void Start()
        {
            UnitID = -1;
        }

        public void Show(int unitID)
        {
            UnitID = unitID;
            mArmyMenu.Closable = false;
            mPanelA.SetActive(true);
        }

        public void OnClickOK_PanelA()
        {
            //ユニットを除外
            var game = Game.GetInstance();
            game.GameData.Territory[0].RemoveUnit(UnitID);

            //取得するアイテムを決定
            var unitData = game.GameData.Unit[UnitID];
            var item = mItemA;
            if (unitData.Level < mItemLevelA) item = mItemA;
            else if (unitData.Level < mItemLevelB) item = mItemB;
            else item = mItemC;

            //パネルBのテキストをセット
            var itemData = game.GameData.Equipment[item];
            mPanelBText.text = unitData.Name + "の解雇を完了\n\n" + 
                               unitData.Name + "は手切れ品として\n" + 
                               itemData.Name + "を置いていった！";

            //アイテムを取得
            game.GameData.Territory[0].EquipmentList[item].Add(-1);

            //ステータスウィンドウをリセット
            mStatusWindow.UnitID = -1;
            mStatusWindow.Init();

            //リストをリセット
            mList.Init();

            mPanelA.SetActive(false);
            mPanelB.SetActive(true);
        }

        public void OnClickCancel_PanelA()
        {
            mPanelA.SetActive(false);
            Close();
        }

        public void OnClickOK_PanelB()
        {
            mPanelB.SetActive(false);
            Close();
        }

        private void Close()
        {
            mArmyMenu.Closable = true;
        }
    }
}