using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DalbitCafe.Core;
using DalbitCafe.Operations;
using DG.Tweening;

namespace DalbitCafe.UI
{
    public class ButtonManager : MonoBehaviour
    {
        public static ButtonManager Instance;
        [SerializeField] private AudioClip click_clip;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

        }


        public void LoadButton(string sceneName)
        {
            SoundManager.Instance.PlaySFX(click_clip, 0.6f);

            // 현재 클릭한 버튼 가져오기
            GameObject currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;


            if (currentButton != null)
            {

                if (!PlayerPrefs.HasKey("HasSeenPrologue"))
                {
                    // 프로로그 한 번도 안 봤다면
                    currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                    {
                        PlayerPrefs.SetString("NextDialogue", "Prologue");
                        PlayerPrefs.SetInt("HasSeenPrologue", 1);
                        SceneManager.LoadScene("DialogueScene");
                    });

                }
                else
                {
                    // 이미 봤다면 바로 GameScene으로
                    currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() => SceneManager.LoadScene(sceneName));
                }
            }
        }

        public void CloseWindowButton(string windowName)
        {

            GameObject window = GameObject.Find(windowName);

            window.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
             .OnComplete(() => window.SetActive(false));
        }


        public void RoastingButton(GameObject button)
        {

            button.GetComponent<Button>().interactable = false;

            // 현재 버튼이 속한 Menu Container 찾기
            GameObject menuContainer = button.transform.parent?.gameObject;

            RoastingWindow roastingWindow = FindObjectOfType<RoastingWindow>();
            int index = roastingWindow.menuContainers.IndexOf(menuContainer);

            if (index < 0 || index >= roastingWindow.coffeDataList.Count)
            {
                button.GetComponent<Button>().interactable = true; // 오류 발생 시 버튼 활성화

                return;
            }

            // 해당 인덱스를 이용하여 coffeDataList에서 CoffeeData를 가져오기
            CoffeeData coffeeData = roastingWindow.coffeDataList[index];

            // 원두 소모 체크
            if (GameManager.Instance.playerStats.coffeeBean >= coffeeData.BeanUse)
            {
                GameManager.Instance.playerStats.AddCoffeeBean(-coffeeData.BeanUse);
                CoffeeMachine.LastTouchedMachine.RoastCoffee(coffeeData);
            }

            //// 작업 완료 후 버튼 활성화
            StartCoroutine(EnableButtonAfterDelay(button, 3f));


        }

        public void QuitButton()
        {
            SoundManager.Instance.PlaySFX(click_clip, 0.6f);
            Application.Quit();
        }

        private IEnumerator EnableButtonAfterDelay(GameObject button, float delay)
        {
            yield return new WaitForSeconds(delay);
            button.GetComponent<Button>().interactable = true;
        }


        private void TouchButtonAni()
        {
            // 현재 클릭한 버튼 가져오기
            GameObject currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (currentButton != null)
            {
                currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            }
        }

        public void CameraShakeBtn()
        {
            FindObjectOfType<CameraShake>().Shake();
        }

    }


}


