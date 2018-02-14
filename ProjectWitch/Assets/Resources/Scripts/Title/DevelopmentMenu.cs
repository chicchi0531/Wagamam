using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch
{
    public class DevelopmentMenu : MonoBehaviour {
        [SerializeField]
        private string mImpressionFormURL = "";

        [SerializeField]
        private string mDebugFormURL = "";

        public void OnClick_ImpressionFormButton()
        {
            Application.OpenURL(mImpressionFormURL);
        }

        public void OnClick_DebugFormButton()
        {
            Application.OpenURL(mDebugFormURL);
        }
    }
}