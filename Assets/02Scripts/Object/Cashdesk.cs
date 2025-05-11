using DalbitCafe.Player;
using UnityEngine;

namespace DalbitCafe.Operations
{

    public class Cashdesk : MonoBehaviour
    {

        [SerializeField] private float behind_transparency;
        private void OnCollisionEnter2D(Collision2D collision)
        {

        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("Player"))
        //    {
        //        //PlayerCtrl.Instance.SpriteRender.color = new Color(1, 1, 1, behind_transparency); //투명도 바꾸기
        //        PlayerCtrl.Instance.SpriteRender.sortingLayerName = "Default";
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    //PlayerCtrl.Instance.SpriteRender.color = new Color(1, 1, 1, 1); //투명도 바꾸기
        //    PlayerCtrl.Instance.SpriteRender.sortingLayerName = "Character";

        //}
    }

}

