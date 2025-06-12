using UnityEngine;
using UnityEngine.SceneManagement;
// 배치모드 관리
namespace DalbitCafe.Deco
{
    public class DecorateManager : MonoSingleton<DecorateManager>
    {
        [Header("모드 진입 시 비활성화할 오브젝트들")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _customerParent;
        private GameObject[] _customers;

        [SerializeField] private GridManager _gridManager;
        [SerializeField] private GameObject _decorateModeExitButton;
        [SerializeField] private GameObject _decorateModeMenuBar;
        [SerializeField] private GameObject _decorateUIElement; // 배치된 아이템 터치 시 위에 뜨는 배치 UI 활성화/비활성화

        [Header("캘린더 패널")]
        [SerializeField] private GameObject _calendarPanel; // daycycleManager의 UI들이 있는 부모

        [SerializeField] private bool _isDecorateMode = false;
        public DraggableItem targetItem;

        // DayCycleManager 참조
        private DayCycleManager _dayCycleManager;

        // 배치모드 상태를 외부에서 확인할 수 있도록 public 프로퍼티 추가
        public bool IsDecorateMode => _isDecorateMode;
        public GameObject Player => _player;
        public Transform CustomerParent => _customerParent;
        public GameObject[] Customers => _customers;
        public GridManager GridManager => _gridManager;
        public GameObject DecorateUIElement => _decorateUIElement;

        private void Start()
        {
            // DayCycleManager 찾기
            _dayCycleManager = FindObjectOfType<DayCycleManager>();
            if (_dayCycleManager == null)
            {
                Debug.LogWarning("[DecorateManager] DayCycleManager를 찾을 수 없습니다!");
            }
        }

        // 배치 모드 활성화
        public void ActivateDecorateMode()
        {
            if (_isDecorateMode) return;

            _isDecorateMode = true;
            _player.SetActive(false); // 플레이어 비활성화

            // 손님들 비활성화
            _customers = new GameObject[_customerParent.childCount];
            for (int i = 0; i < _customers.Length; i++) 
            {
                _customers[i] = _customerParent.GetChild(i).gameObject;
                _customers[i].SetActive(false); 
            }

            // 배치모드 UI 활성화
            _decorateModeExitButton.SetActive(true);
            _decorateModeMenuBar.SetActive(true);

            // 캘린더 패널 비활성화
            if (_calendarPanel != null)
            {
                _calendarPanel.SetActive(false);
            }

            // 시간 흐름 정지
            if (_dayCycleManager != null)
            {
                _dayCycleManager.PauseTime();
            }
        }

        // 배치 모드 비활성화(꾸미기 끝내기 버튼)
        public void DeactivateDecorateMode()
        {
            if (!_isDecorateMode) return;

            _isDecorateMode = false;

            // 플레이어 활성화
            _player.SetActive(true);

            // 손님들 다시 활성화
            foreach (var customer in _customers)
            {
                if(customer != null)
                    customer.SetActive(true);
            }

            // 배치모드 UI 비활성화
            _decorateUIElement.SetActive(false); 
            _decorateModeExitButton.SetActive(false); 
            _decorateModeMenuBar.SetActive(false);

            // 캘린더 패널 활성화
            if (_calendarPanel != null)
            {
                _calendarPanel.SetActive(true);
            }

            // 시간 흐름 재개
            if (_dayCycleManager != null)
            {
                _dayCycleManager.ResumeTime();
            }

            // 타겟 아이템 초기화
            targetItem = null;
        }

        // 아이템 배치 기능 여부 체크
        public bool CanPlaceItem(Vector2Int position, Vector2Int size)
        {
            return _gridManager.CanPlaceItem(position, size); //그리드 매니저의 스크립트
        }

        // 아이템 배치
        public void PlaceItem(Vector2Int position, Vector2Int size)
        {
            _gridManager.PlaceItem(position, size); // 아이템 배치 처리
        }

        // 아이템 제거
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

