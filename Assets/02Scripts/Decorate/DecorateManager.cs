using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ��ġ��� ����
namespace DalbitCafe.Deco
{
    public class DecorateManager : MonoSingleton<DecorateManager>
    {
        [Header("��� ���� �� ��Ȱ��ȭ�� ������Ʈ��")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _customerParent;
        private GameObject[] _customers;

        [SerializeField] private GridManager _gridManager;
        [SerializeField] private GameObject _decorateModeExitButton;
        [SerializeField] private GameObject _decorateModeMenuBar;
        [SerializeField] private GameObject _decorateUIElement; // ��ġ�� ������ ��ġ �� ���� �ߴ� ��ġ UI Ȱ��ȭ/��Ȱ��ȭ

        [Header("Ķ���� �г�")]
        [SerializeField] private GameObject _calendarPanel; // daycycleManager�� UI���� �ִ� �θ�

        [SerializeField] private bool _isDecorateMode = false;
        public DraggableItem targetItem;

        // DayCycleManager ����
        private DayCycleManager _dayCycleManager;

        // ��ġ��� ���¸� �ܺο��� Ȯ���� �� �ֵ��� public ������Ƽ �߰�
        public bool IsDecorateMode => _isDecorateMode;
        public GameObject Player => _player;
        public Transform CustomerParent => _customerParent;
        public GameObject[] Customers => _customers;
        public GridManager GridManager => _gridManager;
        public GameObject DecorateUIElement => _decorateUIElement;

        private void Start()
        {
            // DayCycleManager ã��
            _dayCycleManager = FindObjectOfType<DayCycleManager>();
            if (_dayCycleManager == null)
            {
                Debug.LogWarning("[DecorateManager] DayCycleManager�� ã�� �� �����ϴ�!");
            }
        }

        private void Update()
        {
            // ��ġ��尡 �ƴϸ� ����
            if (!_isDecorateMode) return;

            // ��ġ ó��
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    // UI ��ư�� Ŭ���ߴ��� ���� Ȯ��
                    if (IsTouchingButton(touch.position))
                    {
                        Debug.Log("[DecorateManager] ��ư ��ġ ������ - �� ���� ��ġ ó�� ����");
                        return;
                    }

                    CheckForEmptySpaceTouch(touch.position);
                }
            }
            // �����Ϳ��� �׽�Ʈ�� (���콺 Ŭ��)
            else if (Input.GetMouseButtonDown(0))
            {
                // UI ��ư�� Ŭ���ߴ��� ���� Ȯ��
                if (IsTouchingButton(Input.mousePosition))
                {
                    Debug.Log("[DecorateManager] ��ư Ŭ�� ������ - �� ���� ��ġ ó�� ����");
                    return;
                }

                CheckForEmptySpaceTouch(Input.mousePosition);
            }
        }

        /// <summary>
        /// ��ư�� ��ġ�ߴ��� Ȯ��
        /// </summary>
        private bool IsTouchingButton(Vector2 screenPosition)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                // ��ġ ���� ��ư�� Ȯ��
                if (result.gameObject.name.Contains("Confirm") ||
                    result.gameObject.name.Contains("Cancel") ||
                    result.gameObject.name.Contains("Rotate") ||
                    result.gameObject.GetComponent<Button>() != null)
                {
                    Debug.Log($"[DecorateManager] ��ư ��ġ ����: {result.gameObject.name}");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ȭ���� �� ���� ��ġ�ߴ��� Ȯ���ϰ� ��ġ ��� ���� ������ ó��
        /// </summary>
        private void CheckForEmptySpaceTouch(Vector2 screenPosition)
        {
            // UI ������ ��ġ�� ��� ����
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI ��ġ�̹Ƿ� �ƹ��͵� ���� ����
            }

            // ����Ͽ����� ��ġ ID�� Ȯ��
            if (Input.touchCount > 0)
            {
                int touchId = Input.GetTouch(0).fingerId;
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touchId))
                {
                    return;
                }
            }

            // ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);

            // ����ĳ��Ʈ�� ��ġ�� ������Ʈ Ȯ��
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            // �ƹ��͵� ��ġ���� �ʾҰų�, DraggableItem�� �ƴ� ���
            if (hit.collider == null || hit.collider.GetComponent<DraggableItem>() == null)
            {
                // ���� Ÿ�� �������� �ְ� ��ġ ��� ���̶�� ���
                if (targetItem != null && targetItem.IsPendingPlacement)
                {
                    targetItem.CancelPendingPlacement();
                    targetItem = null;
                    _decorateUIElement.SetActive(false);
                }
            }
        }
        // ��ġ ��� Ȱ��ȭ
        public void ActivateDecorateMode()
        {
            if (_isDecorateMode) return;

            _isDecorateMode = true;
            _player.SetActive(false); // �÷��̾� ��Ȱ��ȭ

            // �մԵ� ��Ȱ��ȭ
            _customers = new GameObject[_customerParent.childCount];
            for (int i = 0; i < _customers.Length; i++) 
            {
                _customers[i] = _customerParent.GetChild(i).gameObject;
                _customers[i].SetActive(false); 
            }

            // ��ġ��� UI Ȱ��ȭ
            _decorateModeExitButton.SetActive(true);
            _decorateModeMenuBar.SetActive(true);

            // Ķ���� �г� ��Ȱ��ȭ
            if (_calendarPanel != null)
            {
                _calendarPanel.SetActive(false);
            }

            // �ð� �帧 ����
            if (_dayCycleManager != null)
            {
                _dayCycleManager.PauseTime();
            }
        }

        // ��ġ ��� ��Ȱ��ȭ(�ٹ̱� ������ ��ư)
        public void DeactivateDecorateMode()
        {
            if (!_isDecorateMode) return;

            // ��ġ ��� ���� �������� �ִٸ� ���� ��ġ�� �ǵ���
            if (targetItem != null && targetItem.IsPendingPlacement)
            {
                targetItem.CancelPendingPlacement();
            }

            _isDecorateMode = false;

            // �÷��̾� Ȱ��ȭ
            _player.SetActive(true);

            // �մԵ� �ٽ� Ȱ��ȭ
            foreach (var customer in _customers)
            {
                if (customer != null)
                    customer.SetActive(true);
            }

            // ��ġ��� UI ��Ȱ��ȭ
            _decorateUIElement.SetActive(false);
            _decorateModeExitButton.SetActive(false);
            _decorateModeMenuBar.SetActive(false);

            // Ķ���� �г� Ȱ��ȭ
            if (_calendarPanel != null)
            {
                _calendarPanel.SetActive(true);
            }

            // �ð� �帧 �簳
            if (_dayCycleManager != null)
            {
                _dayCycleManager.ResumeTime();
            }

            // Ÿ�� ������ �ʱ�ȭ
            targetItem = null;
        }
        // ������ ��ġ ��� ���� üũ
        public bool CanPlaceItem(Vector2Int position, Vector2Int size)
        {
            return _gridManager.CanPlaceItem(position, size); //�׸��� �Ŵ����� ��ũ��Ʈ
        }

        // ������ ��ġ
        public void PlaceItem(Vector2Int position, Vector2Int size)
        {
            _gridManager.PlaceItem(position, size); // ������ ��ġ ó��
        }

        // ������ ����
        public void RemoveItem(Vector2Int position, Vector2Int size)
        {
            _gridManager.RemoveItem(position, size);
        }

        public void OnRotateButtonPressed()
        {
            targetItem.RotateItem();
        }

        /// <summary>
        /// Ȯ�� ��ư�� ������ �� ȣ�� (��ġ Ȯ��)
        /// </summary>
        public void OnConfirmButtonPressed()
        {
            Debug.Log($"[DecorateManager] Ȯ�� ��ư Ŭ����");
            Debug.Log($"[DecorateManager] targetItem�� null�ΰ�? {targetItem == null}");

            if (targetItem != null)
            {
                Debug.Log($"[DecorateManager] targetItem �̸�: {targetItem.gameObject.name}");
                Debug.Log($"[DecorateManager] targetItem.IsPendingPlacement: {targetItem.IsPendingPlacement}");
                Debug.Log($"[DecorateManager] targetItem ���� ��ġ: {targetItem.transform.position}");
            }

            if (targetItem != null && targetItem.IsPendingPlacement)
            {
                Debug.Log("[DecorateManager] ��ġ Ȯ�� ����...");

                // ��ġ Ȯ�� ���� �� ���� ����
                bool wasPending = targetItem.IsPendingPlacement;
                Vector3 positionBefore = targetItem.transform.position;

                targetItem.ConfirmPlacement();

                // ��ġ Ȯ�� �� ���� Ȯ��
                Debug.Log($"[DecorateManager] ��ġ Ȯ�� �� IsPendingPlacement: {targetItem.IsPendingPlacement}");
                Debug.Log($"[DecorateManager] ��ġ Ȯ�� �� ��ġ: {targetItem.transform.position}");
                Debug.Log($"[DecorateManager] ��ġ �����? {positionBefore != targetItem.transform.position}");

                // ��ġ�� Ȯ���Ǹ� UI ��Ȱ��ȭ�ϰ� targetItem �ʱ�ȭ
                if (!targetItem.IsPendingPlacement)
                {
                    Debug.Log("[DecorateManager] UI ��Ȱ��ȭ �� targetItem �ʱ�ȭ");
                    _decorateUIElement.SetActive(false);
                    targetItem = null;
                }
                else
                {
                    Debug.LogWarning("[DecorateManager] ��ġ Ȯ�� �Ŀ��� ������ Pending ����!");
                }
            }
            else
            {
                Debug.LogWarning("[DecorateManager] targetItem�� null�̰ų� Pending ���°� �ƴ�");
            }
        }


        /// <summary>
        /// ��� ��ư�� ������ �� ȣ�� (��ġ ���)
        /// </summary>
        public void OnCancelButtonPressed()
        {
            if (targetItem != null && targetItem.IsPendingPlacement)
            {
                targetItem.CancelPendingPlacement();
                _decorateUIElement.SetActive(false);
                targetItem = null;
            }
        }
    }

}

