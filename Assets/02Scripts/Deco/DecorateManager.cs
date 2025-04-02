using DalbitCafe.Customer;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
// 배치모드 관리
namespace DalbitCafe.Deco
{
    public class DecorateManager : MonoBehaviour
    {
        public static DecorateManager Instance;

        [Header("모드 진입 시 비활성화할 오브젝트들")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _customerParent;
        private GameObject[] _customers;

        [SerializeField] private GridManager _gridManager;
        [SerializeField] private GameObject _decoEndBtn;
        [SerializeField] private GameObject _decorateUI; // 배치 UI 활성화/비활성화

        [SerializeField] private bool _isDecorateMode = false;


        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }


        // 배치 모드 활성화
        public void ActivateDecorateMode()
        {
            if (_isDecorateMode) return;

            _isDecorateMode = true;
            _player.SetActive(false); // 플레이어 비활성화
            _customers = new GameObject[_customerParent.childCount];

            for (int i = 0; i < _customers.Length; i++) //
            {
                _customers[i] = _customerParent.GetChild(i).gameObject;
                _customers[i].SetActive(false); // 손님들 비활성화
            }
            _decorateUI.SetActive(true); // 배치 UI 활성화
            _decoEndBtn.SetActive(true); // 꾸미기 끝내기 버튼 활성화
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
            _decorateUI.SetActive(false); // 배치 UI 비활성화
            _decoEndBtn.SetActive(false); // 끝내기 버튼 비활성화

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
    }

}
