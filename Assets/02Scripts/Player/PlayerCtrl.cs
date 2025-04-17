using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DalbitCafe.UI;
using DalbitCafe.Operations;
using DalbitCafe.Inputs;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
//�ڵ��б�
namespace DalbitCafe.Player
{
    public class PlayerCtrl : MonoSingleton<PlayerCtrl>
    {

        [Header("��ġ UI")]
        [SerializeField] private Image touch_feedback; // ��ġ �ǵ��

        [Header("Ŀ�Ǹӽ� ����")]
        [SerializeField] private float interactionRange; // ��ȣ�ۿ�

        [Header("�÷��̾� �̵�")]
        [SerializeField] private float moveSpeed = 3f;

        private SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRender => spriteRenderer;
        private Animator animator;

        private bool startedOverUI = false; // ��ġ�� UI������ �����ߴ���
        private Vector3 targetPosition;
        private bool isMoving = false;
        private bool canMoveControl = true;

        public Vector3 savedPosition;

        public void SavePosition()
        {
            savedPosition = transform.position;
        }

        public void RestorePosition() // ��ġ ����
        {
            transform.position = savedPosition;
        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ٲ� ��

            if (GameManager.Instance.TouchInputManager != null)
            {
                GameManager.Instance.TouchInputManager.OnTouchBegan += HandleTouchBegan;
                GameManager.Instance.TouchInputManager.OnTouchMoved += HandleTouchMoved;
                GameManager.Instance.TouchInputManager.OnTouchEnded += HandleTouchEnded;
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (GameManager.Instance.TouchInputManager != null)
            {
                GameManager.Instance.TouchInputManager.OnTouchBegan -= HandleTouchBegan;
                GameManager.Instance.TouchInputManager.OnTouchMoved -= HandleTouchMoved;
                GameManager.Instance.TouchInputManager.OnTouchEnded -= HandleTouchEnded;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            canMoveControl = scene.name != "DialogueScene";
        }

        public void HandleTouchBegan(Vector2 screenPos)
        {
            if (!canMoveControl) return;
            startedOverUI = GameManager.Instance.UIManager.IsTouchOverUIPosition(screenPos);
        }

        public void HandleTouchMoved(Vector2 screenPos)
        {
            if (!canMoveControl) return;
        }

        public void HandleTouchEnded(Vector2 screenPos) // ��ġ�� �I�� �� ������
        {
            if (!canMoveControl || startedOverUI) 
            {
                startedOverUI = false;  // ����
                return;
            }


            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane));
            worldPos.z = 0;

            Collider2D hit = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("CoffeeMachine")); // Ŀ�� �ӽ� ���̾��ũ ���͸�
            if (hit != null && hit.CompareTag("CoffeeMachine")) // �ݶ��̴� �� ��ü�� �±װ� Ŀ�Ǹӽ��̶��
            {
                TouchCoffeeMachine(hit.gameObject); // �ش� Ŀ�Ǹӽ� �Ѱ��ֱ�
            }
            else // Ŀ�Ǹӽ��� �ƴ϶��
            {
                Collider2D floorCheck = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Floor"));
                if (floorCheck != null) //�ٴ�Ÿ�ϸ����� üũ
                {
                    OnMove(worldPos); // �ٴ� Ÿ�ϸ��̸� �̵�����
                }
            }
        }

        private void TouchCoffeeMachine(GameObject machine)
        {
            if (Vector3.Distance(transform.position, machine.transform.position) < interactionRange) // �÷��̾� �Ÿ��� Ŀ�Ǹӽ��� �Ÿ��� ��ȣ�ۿ�Ÿ� �����
            {
                CoffeeMachine coffeeMachine = machine.GetComponent<CoffeeMachine>();
                CoffeeMachine.SetLastTouchedMachine(coffeeMachine); // ���������� ��ġ�� Ŀ�Ǹӽ����� ����

                if (coffeeMachine.IsRoasting) // �ش� Ŀ�Ǹӽ��� �ν��� ���̶��
                {
                    GameManager.Instance.UIManager.ShowCurrentMenuPopUp();
                    GameObject currentMenuWindow = GameObject.Find("Panel_CurrentMenu"); // Find
                    currentMenuWindow.GetComponent<CurrentMenuWindow>().UpdateMenuPanel(coffeeMachine);
                }
                else
                {
                    GameManager.Instance.UIManager.ShowMakeCoffeePopUp();
                }
            }
            else
            {
                GameManager.Instance.UIManager.ShowCapitonText();
            }
        }

        private void OnMove(Vector3 targetPos)
        {
            targetPosition = targetPos;

            if (!isMoving)
            {
                StartCoroutine(MoveToTarget());
            }
        }

        private IEnumerator MoveToTarget() // �� �������� �̵�
        {
            isMoving = true;
            animator.SetBool("isMoving", true);

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized; // �ִϸ��̼� �뵵
                SetAnimation(direction);

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // �� �̵��ߴٸ� 
            transform.position = targetPosition;
            isMoving = false;
            animator.SetBool("isMoving", false);

            touch_feedback.enabled = false; // �̰͵� UIManager�� �Űܾ���
        }

        private void SetAnimation(Vector3 direction)
        {
            Vector3 normalizedDirection = direction.normalized;
            animator.SetFloat("MoveX", normalizedDirection.x);
            animator.SetFloat("MoveY", normalizedDirection.y);
        }


    }
}
