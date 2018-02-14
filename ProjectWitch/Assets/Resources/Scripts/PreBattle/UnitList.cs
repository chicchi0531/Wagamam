using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using ProjectWitch.Extention;

namespace ProjectWitch.PreBattle
{

    public class UnitList : MonoBehaviour
    {
        [SerializeField]
        private GameObject mContentParent = null;

        [SerializeField]
        private GameObject mUnitPrefab = null;

        [SerializeField]
        private PreBattleController mController = null;

        [SerializeField]
        private Dropdown mDropDown = null;

        //プロパティ
        public List<Unit> UnitComponentList { get; set; }


        private EUnitListSortType mSortType = EUnitListSortType.ID;

        // Use this for initialization
        void Start()
        {
            UnitComponentList = new List<Unit>();

            mDropDown.value = (int)Game.GetInstance().SystemData.Config.UnitListSortType;
            ChangeSortType(mDropDown);
        }

        // Update is called once per frame
        public void ChangeSortType(Dropdown dropdown)
        {
            mSortType = EnumConverter.ToEnum<EUnitListSortType>(dropdown.value);
            
            //ソートを実行
            var game = Game.GetInstance();
            var group = game.GameData.Group[game.GameData.Territory[0].GroupList[0]];
            var unitList = group.UnitList;
            group.UnitList = game.GameData.SortUnitList(unitList, mSortType);

            //システムデータに歳入
            game.SystemData.Config.UnitListSortType = mSortType;

            //リストの入れ直し
            UnitSet();
        }

        //リストにコンテンツをセットする
        void UnitSet()
        {
            //もとからあるオブジェクトを削除
            var children = mContentParent.GetComponentsInChildren<Unit>();
            foreach (var child in children)
                Destroy(child.gameObject);

            var game = Game.GetInstance();

            var territory = game.GameData.Territory[0];
            var group = game.GameData.Group[territory.GroupList[0]];
            foreach (var unitid in group.UnitList)
            {
                //コンテンツを追加
                var inst = Instantiate(mUnitPrefab);
                var cUnit = inst.GetComponent<Unit>();
                cUnit.UnitID = unitid;
                cUnit.Controller = mController;
                inst.transform.SetParent(mContentParent.transform, false);

                UnitComponentList.Add(cUnit);
            }
        }
    }
}