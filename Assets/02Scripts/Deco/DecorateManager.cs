using DalbitCafe.Customer;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
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
        [SerializeField] private GameObject _decoEndBtn;
        [SerializeField] private GameObject _decorateUI; // ��ġ UI Ȱ��ȭ/��Ȱ��ȭ

        [SerializeField] private bool _isDecorateMode = false;

        [SerializeField] private DraggableItem targetItem;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += InitElement;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= InitElement;
        }

        private void InitElement(Scene scene, LoadSceneMode sceneMode)
        {
            if(scene.name == "GameScene")
            {
                _player = GameObject.Find("Player");
                _customerParent = GameObject.Find("Customers").transform;
                _gridManager = GridManager.Instance;
                targetItem = GameObject.FindObjectOfType(typeof(DraggableItem)).GetComponent<DraggableItem>();
                //_decorateUI
            }
        }
        // ��ġ ��� Ȱ��ȭ
        public void ActivateDecorateMode()
        {
            if (_isDecorateMode) return;

            _isDecorateMode = true;
            _player.SetActive(false); // �÷��̾� ��Ȱ��ȭ
            _customers = new GameObject[_customerParent.childCount];

            for (int i = 0; i < _customers.Length; i++) //
            {
                _customers[i] = _customerParent.GetChild(i).gameObject;
                _customers[i].SetActive(false); // �մԵ� ��Ȱ��ȭ
            }
            _decorateUI.SetActive(true); // ��ġ UI Ȱ��ȭ
            _decoEndBtn.SetActive(true); // �ٹ̱� ������ ��ư Ȱ��ȭ
        }

        // ��ġ ��� ��Ȱ��ȭ(�ٹ̱� ������ ��ư)
        public void DeactivateDecorateMode()
        {
            if (!_isDecorateMode) return;

            _isDecorateMode = false;
            _player.SetActive(true); // �÷��̾� Ȱ��ȭ
            foreach (var customer in _customers)
            {
                customer.SetActive(true); // �մԵ� �ٽ� Ȱ��ȭ
            }
            _decorateUI.SetActive(false); // ��ġ UI ��Ȱ��ȭ
            _decoEndBtn.SetActive(false); // ������ ��ư ��Ȱ��ȭ

        }

        // ������ ��ġ ��� ���� üũ
        public bool CanPlaceItem(Vector2Int position, Vector2Int size)
        {
            Debug.Log(_gridManager.CanPlaceItem(position, size));
            return _gridManager.CanPlaceItem(position, size); //�׸��� �Ŵ����� ��ũ��Ʈ
        }

        // ������ ��ġ
        public void PlaceItem(Vector2Int position, Vector2Int size)
        {
            _gridManager.PlaceItem(position, size); // ������ ��ġ ó��
        }

        public void OnRotateButtonPressed()
        {
            targetItem.RotateItem();
        }
    }


}

