using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


using System.Collections;


namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {        
        #region Public Properties
        
        [Tooltip("UI Text to display Player's Name")]
        public Image PlayerNameText;
        
        [Tooltip("UI Slider to display Player's Health")]
        public Slider PlayerHealthSlider;

        RectTransform _RectTransform;

        #endregion


        #region Private Properties
        public PlayerHealth _target;
        #endregion


        #region MonoBehaviour Messages
        void Awake()
        {
            this.GetComponent<Transform>().SetParent(GameObject.Find("HUDCanvas").GetComponent<Transform>());
        }

        void Update()
        {
            // Reflect the Player Health
            if (PlayerHealthSlider != null)
            {
                PlayerHealthSlider.value = _target.currentHealth;
            }
            
            if (_target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        #endregion


        #region Public Methods
        public void SetTarget(PlayerHealth target)
        {
            if (target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            _target = target;

            Debug.Log(message: "NAME OF PLAYER" + _target.photonView.owner.name);
            if (PlayerNameText != null)
            {

            }
        }

        #endregion


    }
}