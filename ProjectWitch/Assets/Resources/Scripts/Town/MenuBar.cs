using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectWitch.Town
{
    public class MenuBar : MonoBehaviour
    {
        [SerializeField]
        private TownMenu mTownMenu = null;

        //各ボタンへの参照
        [SerializeField]
        private Button[] mButtons = null;

        // Update is called once per frame
        void Update()
        {
            foreach(var b in mButtons)
            {
                b.interactable = mTownMenu.Closable;
            }
        }

        //タイトルへをクリックしたときの挙動
        public void OnClick_GotoTitle()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().GoTitle.Show();
        }

        //戻るをクリックしたときの挙動
        public void OnClick_Back()
        {
            mTownMenu.Close();
        }
    }
}
