using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;



namespace Com.MyCompany.MyGame
{
    //Class for the player UI
    public class PayloadUI : MonoBehaviour
    {        
        [Tooltip("UI Slider to display Player's Health")]
        public Slider PayloadHealthSlider;
        RectTransform _RectTransform;
        public Payload _target;

        void Awake()
        {
            this.GetComponent<Transform>().SetParent(GameObject.Find("HUDCanvas").GetComponent<Transform>());
        }

        void Start()
        {
            // Reflect the Player Health
            if (PayloadHealthSlider != null)
            {
                PayloadHealthSlider.value = _target.currentHealth;
            }
            
            if (_target == null)
            {
                Destroy(this.gameObject);
                return;
            }

        }
        public void SetTarget(Payload target)
        {
            if (target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            _target = target;
            
        }
        void Update()
        {
            if (PayloadHealthSlider != null)
            {
                PayloadHealthSlider.value = _target.currentHealth;
            }
        }
    }
}