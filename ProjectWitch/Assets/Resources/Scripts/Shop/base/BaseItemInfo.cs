﻿using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class BaseItemInfo : MonoBehaviour
    {
        //アニメータへの参照
        protected Animator mAnimator = null;

        [SerializeField]
        protected NextManaWindow mNextManaWindow = null;

        //メッセージボックス
        [SerializeField]
        protected MessageBox mMessageBox = null;

        //リストへの参照
        [SerializeField]
        protected BaseList mList = null;

        public int ItemID { get; set; }
        public int ItemNum { get; set; } //購入数、売却数

        public virtual void Start()
        {
            mAnimator = GetComponent<Animator>();

            ItemNum = 1;
            ItemID = -1;
            Reset();
        }

        public virtual void Reset()
        {
        }

        public virtual void Close()
        {
            ItemID = -1;
            Reset();
        }
    }
}
