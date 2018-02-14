using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    //アイテムの購入数、売却数を管理するクラス
    public class ItemNum : MonoBehaviour
    {
        [SerializeField]
        private Button mUpButton = null;

        [SerializeField]
        private Button mDownButton = null;

        [SerializeField]
        private Text mText = null;

        [SerializeField]
        private BaseItemInfo mInfoWindow = null;

        public int MaxNum { get; set; }
        public int MinNum { get; set; }

        public int Num { get; set; }

        // Use this for initialization
        void Start()
        {
            MinNum = 1;
            Reset();
        }

        public void Reset()
        {
            MaxNum = 10;
            Num = MinNum;
        }

        // Update is called once per frame
        void Update()
        {
            //ダウンボタンの有効化
            if (Num <= MinNum) mDownButton.interactable = false;
            else mDownButton.interactable = true;

            //アップボタンの有効化
            if (Num >= MaxNum) mUpButton.interactable = false;
            else mUpButton.interactable = true;

            mText.text = Num.ToString();

        }

        public void Click_CountUp()
        {
            Num++;
            
            //インフォウィンドウ本体に個数を渡す
            mInfoWindow.ItemNum = Num;
            mInfoWindow.Reset();
        }

        public void Click_CountDown()
        {
            Num--;
            
            //インフォウィンドウ本体に個数を渡す
            mInfoWindow.ItemNum = Num;
            mInfoWindow.Reset();
        }
    }
}