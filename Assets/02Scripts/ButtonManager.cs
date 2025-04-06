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
                SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �� ȣ��

            }
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // ���� �ٲ� �� GameManager�� �ٽ� ã�������� ����
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager �ν��Ͻ��� ã�� �� ����!");
            }
        }

        public void LoadButton(string sceneName)
        {
            SoundManager.Instance.PlaySFX(click_clip, 0.6f);

            // ���� Ŭ���� ��ư ��������
            GameObject currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (currentButton != null)
            {
                currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() => SceneManager.LoadScene(sceneName));
            }
        }

        public void CloseWindowButton(string windowName)
        {

            GameObject window = GameObject.Find(windowName);
            if (window == null)
            {
                Debug.LogError("��Ȱ��ȭ Ȱ �����츦 ã�� ���߽��ϴ�!");
                return;
            }

            window.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
             .OnComplete(() => window.SetActive(false));
        }


        public void RoastingButton(GameObject button)
        {

            button.GetComponent<Button>().interactable = false;

            // ���� ��ư�� ���� Menu Container ã��
            GameObject menuContainer = button.transform.parent?.gameObject;

            RoastingWindow roastingWindow = FindObjectOfType<RoastingWindow>();
            int index = roastingWindow.menuContainers.IndexOf(menuContainer);

            if (index < 0 || index >= roastingWindow.coffeDataList.Count)
            {
                Debug.LogError("��ȿ���� ���� Ŀ�� �޴� ����!");
                button.GetComponent<Button>().interactable = true; // ���� �߻� �� ��ư Ȱ��ȭ

                return;
            }

            // �ش� �ε����� �̿��Ͽ� coffeDataList���� CoffeeData�� ��������
            CoffeeData coffeeData = roastingWindow.coffeDataList[index];

            // ���� �Ҹ� üũ
            if (GameManager.Instance.playerStats.coffeeBean >= coffeeData.BeanUse)
            {
                Debug.Log($"�ν��� ���� �� CoffeeBean: {GameManager.Instance.playerStats.coffeeBean}, �Ҹ�: {coffeeData.BeanUse}");
                GameManager.Instance.playerStats.coffeeBean -= coffeeData.BeanUse;
                Debug.Log($"�ν��� �� CoffeeBean: {GameManager.Instance.playerStats.coffeeBean}");
                CoffeeMachine.LastTouchedMachine.RoastCoffee(coffeeData);
                Debug.Log($"{coffeeData.CoffeeName} �ν��� ����!");

            }
            else
            {
                Debug.LogError("Ŀ������ �����մϴ�!");
            }

            //// �۾� �Ϸ� �� ��ư Ȱ��ȭ
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
            // ���� Ŭ���� ��ư ��������
            GameObject currentButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (currentButton != null)
            {
                currentButton.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
            }
        }

    }


}


