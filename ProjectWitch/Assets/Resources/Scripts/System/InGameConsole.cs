using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProjectWitch
{
    public class InGameConsole : MonoBehaviour
    {
        //表示部分
        [SerializeField]
        private GameObject mcUI = null;


        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
#if DEBUG
            if (mcUI.activeSelf)
            {
                if (Input.GetButtonDown("Console"))
                    Hide();
            }
            else
            {
                if (Input.GetButtonDown("Console"))
                    Show();
            }
#endif
        }

        public void Show()
        {
            mcUI.SetActive(true);
        }

        public void Hide()
        {
            mcUI.SetActive(false);
        }
    }

    public class InGameConsoleCommand
    {
        //敵ユニットのステータスをcsvから再ロードする
        [uREPL.Command(name = "reloadenemyunit")]
        public static void ReloadEnemyUnit()
        {
            var game = Game.GetInstance();

            //コピーに使うゲームデータの作成
            var gamedata = new GameData();
            gamedata.Reset();

            //全敵ユニットを抽出
            var enemyList = new List<int>();
            for(int i=0; i < game.GameData.Unit.Count; i++)
            {
                //自領地は飛ばす
                if (game.GameData.Group[game.GameData.Territory[0].GroupList[0]].UnitList.Contains(i)) continue;

                enemyList.Add(i);
            }

            //敵のデータをコピー
            foreach(var e in enemyList)
            {
                game.GameData.Unit[e] = gamedata.Unit[e];
            }
                
        }

        //敵グループのステータスをcsvから再ロードする
        [uREPL.Command(name = "reloadenemygroup")]
        public static void ReloadEnemyGroup(int territoryID)
        {
            var game = Game.GetInstance();

            //コピーに使うゲームデータの作成
            var gamedata = new GameData();
            gamedata.Reset();

            //指定領地のグループリストを抽出1
            var groupList = gamedata.Territory[territoryID].GroupList;

            //自領地のユニットリストを取得（比較用）
            var playerUnits = game.GameData.Group[game.GameData.Territory[0].GroupList[0]].UnitList;

            foreach(var g in groupList)
            {
                var data = gamedata.Group[g];

                //ユニットリストをコピー
                //死亡しているか、自領地にいる場合は除外
                var unitList = new List<int>(data.UnitList);
                foreach(var u in data.UnitList)
                {
                    var udata = game.GameData.Unit[u];
                    if (!udata.IsAlive || playerUnits.Contains(u))
                        unitList.Remove(u);
                }
                game.GameData.Group[g].UnitList = unitList;

                //カードリストをコピー
                game.GameData.Group[g].CardList = gamedata.Group[g].CardList;
            }


        }

        //ユニットを雇う
        [uREPL.Command(name = "unitemploy")]
        public static void UnitEmploy(int unit)
        {
            var game = Game.GetInstance();
            game.GameData.Territory[0].AddUnit(unit);

            var unitData = game.GameData.Unit[unit];

            uREPL.Log.Output("ID:" + unit + " [" + unitData.Name + "]を雇いました");
        }

        //ユニットを解雇
        [uREPL.Command(name = "unitfire")]
        public static void UnitFire(int unit)
        {
            var game = Game.GetInstance();
            game.GameData.Territory[0].RemoveUnit(unit);

            var unitData = game.GameData.Unit[unit];

            uREPL.Log.Output("ID:" + unit + " [" + unitData.Name + "]を解雇しました");
        }

        //拠点のオーナーを変更する
        [uREPL.Command(name = "changeareaowner")]
        public static void ChangeAreaOwner(int area, int owner)
        {
            var game = Game.GetInstance();

            game.ChangeAreaOwner(area, owner);
        }
        

        //指定の領地のグループの状態を一覧する
        [uREPL.Command(name ="printgroupstate")]
        public static void PrintGroupState(int territory)
        {
            var game = Game.GetInstance();
            uREPL.Log.Output(game.GameData.Territory[territory].OwnerName + "領グループ状況\n");

            foreach(var gID in game.GameData.Territory[territory].GroupList)
            {
                var gData = game.GameData.Group[gID];

                uREPL.Log.Output(gData.Name + "\t" + 
                                "状態:" + gData.State + "\t" + 
                                "残存ユニット数:" + gData.UnitList.Count + "\t\n");
            }
        }

        //クラスのプロパティをすべて出力する
        [uREPL.Command(name = "printproperty")]
        public static void PrintProperty<T>(T obj)
        {
            string outStr = "";

            Type t = typeof(T);
            MemberInfo[] members = t.GetMembers
                (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var m in members)
            {
                if (m.MemberType.ToString().Equals("Property"))
                {
                    var pr = t.GetProperty(m.Name);
                    outStr += String.Format("{0,20}", m.Name) + " : ";
                    outStr += pr.GetValue(obj, null);
                    outStr += "\n";
                }

            }
            uREPL.Log.Output(outStr);
        }

        //クラスのプロパティをセットする
        [uREPL.Command(name = "setproperty")]
        public static void SetProperty<T>(T obj, string member, object value)
        {
            Type t = typeof(T);
            MemberInfo[] members = t.GetMembers
                (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var m in members)
            {
                if (m.Name.Equals(member))
                {
                    if (m.MemberType.ToString().Equals("Property"))
                    {
                        var pr = t.GetProperty(m.Name);
                        pr.SetValue(obj, value, null);
                    }
                }
            }
        }

        //指定のリストの要素をすべて出力する
        [uREPL.Command(name = "printlistall")]
        public static void PrintListAll<T>(List<T> list)
        {
            string outStr = "";

            Type t = typeof(T);
            MemberInfo[] members = t.GetMembers
                (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            //メンバ名の列挙
            if (members.Length == 0) return;
            foreach (var m in members)
            {
                if (m.MemberType.ToString().Equals("Property"))
                {
                    outStr += "\t" + m.Name;
                }
            }
            uREPL.Log.Output(outStr);
            outStr = "";

            //データの列挙
            for (int i = 0; i < list.Count; i++)
            {
                outStr += i.ToString();
                foreach (var m in members)
                {
                    if (m.MemberType.ToString().Equals("Property"))
                    {
                        var pr = t.GetProperty(m.Name);
                        var obj = pr.GetValue(list[i], null);
                        outStr += "\t" + obj.ToString();
                    }
                }
                uREPL.Log.Output(outStr);
                outStr = "";
            }
        }

        //指定のリストのインデックスで指定した要素を出力する
        [uREPL.Command(name = "printlist")]
        public static void PrintList<T>(List<T> list, int index)
        {
            PrintProperty(list[index]);
        }

        //指定のリストのインデックスで指定した要素のメンバを書き換える
        [uREPL.Command(name = "setlist")]
        public static void SetList<T>(List<T> list, int index, string member, object value)
        {
            SetProperty(list[index], member, value);
        }

        #region GameData
        //index番目の領地データを表示
        [uREPL.Command(name = "printterritory")]
        public static void PrintTerritory(int index) { PrintList(Game.GetInstance().GameData.Territory, index); }
        //index番目の領地データの指定のメンバを変更
        [uREPL.Command(name = "setterritory")]
        public static void SetTerritory(int index, string member, object value) { SetList(Game.GetInstance().GameData.Territory, index, member, value); }

        //index番目のグループデータを表示
        [uREPL.Command(name = "printgroup")]
        public static void PrintGroup(int index) { PrintList(Game.GetInstance().GameData.Group, index); }
        //index番目のグループデータの指定のメンバを変更
        [uREPL.Command(name = "setgroup")]
        public static void SetGroup(int index, string member, object value) { SetList(Game.GetInstance().GameData.Group, index, member, value); }

        //index番目のユニットデータを表示
        [uREPL.Command(name = "printunit")]
        public static void PrintUnit(int index) { PrintList(Game.GetInstance().GameData.Unit, index); }
        //index番目のユニットデータの指定のメンバを変更
        [uREPL.Command(name = "setunit")]
        public static void SetUnit(int index, string member, object value) { SetList(Game.GetInstance().GameData.Unit, index, member, value); }

        //index番目の地域データを表示
        [uREPL.Command(name = "printarea")]
        public static void PrintArea(int index) { PrintList(Game.GetInstance().GameData.Area, index); }
        //index番目の地域データの指定のメンバを変更
        [uREPL.Command(name = "setarea")]
        public static void SetArea(int index, string member, object value) { SetList(Game.GetInstance().GameData.Area, index, member, value); }

        //index番目のスキルデータを表示
        [uREPL.Command(name = "printskill")]
        public static void PrintSkill(int index) { PrintList(Game.GetInstance().GameData.Skill, index); }
        //index番目のスキルデータの指定のメンバを変更
        [uREPL.Command(name = "setskill")]
        public static void SetSkill(int index, string member, object value) { SetList(Game.GetInstance().GameData.Skill, index, member, value); }

        //index番目のカードデータを表示
        [uREPL.Command(name = "printcard")]
        public static void PrintCard(int index) { PrintList(Game.GetInstance().GameData.Card, index); }
        //index番目のカードデータの指定のメンバを変更
        [uREPL.Command(name = "setcard")]
        public static void SetCard(int index, string member, object value) { SetList(Game.GetInstance().GameData.Card, index, member, value); }

        //index番目の装備データを表示
        [uREPL.Command(name = "printequipment")]
        public static void PrintEquipment(int index) { PrintList(Game.GetInstance().GameData.Equipment, index); }
        //index番目の装備データの指定のメンバを変更
        [uREPL.Command(name = "setequipment")]
        public static void SetEquipment(int index, string member, object value) { SetList(Game.GetInstance().GameData.Equipment, index, member, value); }

        //index番目のAIデータを表示
        [uREPL.Command(name = "printai")]
        public static void PrintAI(int index) { PrintList(Game.GetInstance().GameData.AI, index); }
        //index番目のAIデータの指定のメンバを変更
        [uREPL.Command(name = "setai")]
        public static void SetAI(int index, string member, object value) { SetList(Game.GetInstance().GameData.AI, index, member, value); }

        //index番目のゲームメモリを表示
        [uREPL.Command(name = "printmemory")]
        public static void PrintMemory(int index) { uREPL.Log.Output(Game.GetInstance().GameData.Memory[index].ToString()); }
        //index番目のゲームメモリを書き換え
        [uREPL.Command(name = "setmemory")]
        public static void SetMemory(int index, object value) { Game.GetInstance().GameData.Memory[index] = int.Parse(value.ToString()); }

        //index番目のシステムメモリを表示
        [uREPL.Command(name = "printsysmemory")]
        public static void PrintSysMemory(int index) { uREPL.Log.Output(Game.GetInstance().SystemData.Memory[index].ToString()); }
        //index番目のシステムメモリを書き換え
        [uREPL.Command(name = "setsysmemory")]
        public static void SetSysMemory(int index, object value) { Game.GetInstance().SystemData.Memory[index] = int.Parse(value.ToString()); }

        //コンフィグ表示
        [uREPL.Command(name = "printconfig")]
        public static void PrintConfig(int index) { PrintProperty(Game.GetInstance().SystemData.Config); }
        //index番目のゲームメモリを書き換え
        [uREPL.Command(name = "setmemory")]
        public static void SetMemory(int index, string member, object value) { SetProperty(Game.GetInstance().SystemData.Config, member, value); }

        //セーブ関連
        [uREPL.Command(name = "save")]
        public static void Save(int slot) { Game.GetInstance().GameData.Save(slot); }
        [uREPL.Command(name ="load")]
        public static void Load(int slot) { Game.GetInstance().GameData.Load(slot); Game.GetInstance().CallField(); }

        //システムセーブ・ロード
        [uREPL.Command(name = "syssave")]
        public static void SysSave() { Game.GetInstance().SystemData.Save(); }
        [uREPL.Command(name = "sysload")]
        public static void SysLoad() { Game.GetInstance().SystemData.Load(); }

        #endregion
    }
}