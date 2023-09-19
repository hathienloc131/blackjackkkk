using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CityPeople
{
    public class CityPeople : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private TMP_Text text;

        void Start()
        {
            animator = GetComponent<Animator>();

        }

        void Update()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                // Play the animation
                animator.Play("locom_m_basicWalk_30f");
            }
            else if (Input.GetKeyDown(KeyCode.Return)){
                if (GlobalVariable.IS_CHANGE_SCENCE_PLAY_CARD)
                {
                    text.text = "CHUYEN MAN";
                }
            }
            else
            {
                animator.Play("idle_m_1_200f");
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.name == "ObjectName")
            {
                Debug.Log("Đối tượng đã va chạm với ObjectName");
            }
        }

    }
}
