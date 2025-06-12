using UnityEngine;
using UnityEngine.SceneManagement;
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

            _isDecorateMode = false;

            // �÷��̾� Ȱ��ȭ
            _player.SetActive(true);

            // �մԵ� �ٽ� Ȱ��ȭ
            foreach (var customer in _customers)
            {
                if(customer != null)
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
    }

}

