using UnityEngine;
using UnityEngine.SceneManagement;
// 배치모드 관리
namespace DalbitCafe.Deco
{
    // 손님 비활성화, 플레이어 비활성화
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
        
        [SerializeField] private bool _isDecorateMode = false;

        public DraggableItem targetItem;

        // 배치모드 상태를 외부에서 확인할 수 있도록 public 프로퍼티 추가
        public bool IsDecorateMode => _isDecorateMode;

        // 배치 모드 활성화
        public void ActivateDecorateMode()
        {
            if (_isDecorateMode) return;

            _isDecorateMode = true;
            _player.SetActive(false); // 플레이어 비활성화

            _customers = new GameObject[_customerParent.childCount];
            for (int i = 0; i < _customers.Length; i++) 
            {
                _customers[i] = _customerParent.GetChild(i).gameObject;
                _customers[i].SetActive(false); // 손님들 비활성화
            }

            _decorateModeExitButton.SetActive(true); // 꾸미기 끝내기 버튼 활성화
            _decorateModeMenuBar.SetActive(true);
        }

        // 배치 모드 비활성화(꾸미기 끝내기 버튼)
        public void DeactivateDecorateMode()
        {
            if (!_isDecorateMode) return;

            _isDecorateMode = false;
            _player.SetActive(true); // 플레이어 활성화
            foreach (var customer in _customers)
            {
                customer.SetActive(true); // 손님들 다시 활성화
            }
            _decorateUIElement.SetActive(false); // 배치 UI 비활성화
            _decorateModeExitButton.SetActive(false); // 끝내기 버튼 비활성화
            _decorateModeMenuBar.SetActive(false);

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
        public void OnRotateButtonPressed()
        {
            targetItem.RotateItem();
        }
    }


}

