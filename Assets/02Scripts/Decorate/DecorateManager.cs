using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        private void Update()
        {
            // 배치모드가 아니면 리턴
            if (!_isDecorateMode) return;

            // 터치 처리
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    // UI 버튼을 클릭했는지 먼저 확인
                    if (IsTouchingButton(touch.position))
                    {
                        Debug.Log("[DecorateManager] 버튼 터치 감지됨 - 빈 공간 터치 처리 무시");
                        return;
                    }

                    CheckForEmptySpaceTouch(touch.position);
                }
            }
            // 에디터에서 테스트용 (마우스 클릭)
            else if (Input.GetMouseButtonDown(0))
            {
                // UI 버튼을 클릭했는지 먼저 확인
                if (IsTouchingButton(Input.mousePosition))
                {
                    Debug.Log("[DecorateManager] 버튼 클릭 감지됨 - 빈 공간 터치 처리 무시");
                    return;
                }

                CheckForEmptySpaceTouch(Input.mousePosition);
            }
        }

        /// <summary>
        /// 버튼을 터치했는지 확인
        /// </summary>
        private bool IsTouchingButton(Vector2 screenPosition)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = screenPosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                // 배치 관련 버튼들 확인
                if (result.gameObject.name.Contains("Confirm") ||
                    result.gameObject.name.Contains("Cancel") ||
                    result.gameObject.name.Contains("Rotate") ||
                    result.gameObject.GetComponent<Button>() != null)
                {
                    Debug.Log($"[DecorateManager] 버튼 터치 감지: {result.gameObject.name}");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 화면의 빈 곳을 터치했는지 확인하고 배치 대기 중인 아이템 처리
        /// </summary>
        private void CheckForEmptySpaceTouch(Vector2 screenPosition)
        {
            // UI 위에서 터치된 경우 무시
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI 터치이므로 아무것도 하지 않음
            }

            // 모바일에서는 터치 ID도 확인
            if (Input.touchCount > 0)
            {
                int touchId = Input.GetTouch(0).fingerId;
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touchId))
                {
                    return;
                }
            }

            // 스크린 좌표를 월드 좌표로 변환
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);

            // 레이캐스트로 터치된 오브젝트 확인
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            // 아무것도 터치되지 않았거나, DraggableItem이 아닌 경우
            if (hit.collider == null || hit.collider.GetComponent<DraggableItem>() == null)
            {
                // 현재 타겟 아이템이 있고 배치 대기 중이라면 취소
                if (targetItem != null && targetItem.IsPendingPlacement)
                {
                    targetItem.CancelPendingPlacement();
                    targetItem = null;
                    _decorateUIElement.SetActive(false);
                }
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

            // 배치 대기 중인 아이템이 있다면 원래 위치로 되돌림
            if (targetItem != null && targetItem.IsPendingPlacement)
            {
                targetItem.CancelPendingPlacement();
            }

            _isDecorateMode = false;

            // 플레이어 활성화
            _player.SetActive(true);

            // 손님들 다시 활성화
            foreach (var customer in _customers)
            {
                if (customer != null)
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

        /// <summary>
        /// 확인 버튼이 눌렸을 때 호출 (배치 확정)
        /// </summary>
        public void OnConfirmButtonPressed()
        {
            Debug.Log($"[DecorateManager] 확인 버튼 클릭됨");
            Debug.Log($"[DecorateManager] targetItem이 null인가? {targetItem == null}");

            if (targetItem != null)
            {
                Debug.Log($"[DecorateManager] targetItem 이름: {targetItem.gameObject.name}");
                Debug.Log($"[DecorateManager] targetItem.IsPendingPlacement: {targetItem.IsPendingPlacement}");
                Debug.Log($"[DecorateManager] targetItem 현재 위치: {targetItem.transform.position}");
            }

            if (targetItem != null && targetItem.IsPendingPlacement)
            {
                Debug.Log("[DecorateManager] 배치 확정 진행...");

                // 배치 확정 실행 전 상태 저장
                bool wasPending = targetItem.IsPendingPlacement;
                Vector3 positionBefore = targetItem.transform.position;

                targetItem.ConfirmPlacement();

                // 배치 확정 후 상태 확인
                Debug.Log($"[DecorateManager] 배치 확정 후 IsPendingPlacement: {targetItem.IsPendingPlacement}");
                Debug.Log($"[DecorateManager] 배치 확정 후 위치: {targetItem.transform.position}");
                Debug.Log($"[DecorateManager] 위치 변경됨? {positionBefore != targetItem.transform.position}");

                // 배치가 확정되면 UI 비활성화하고 targetItem 초기화
                if (!targetItem.IsPendingPlacement)
                {
                    Debug.Log("[DecorateManager] UI 비활성화 및 targetItem 초기화");
                    _decorateUIElement.SetActive(false);
                    targetItem = null;
                }
                else
                {
                    Debug.LogWarning("[DecorateManager] 배치 확정 후에도 여전히 Pending 상태!");
                }
            }
            else
            {
                Debug.LogWarning("[DecorateManager] targetItem이 null이거나 Pending 상태가 아님");
            }
        }


        /// <summary>
        /// 취소 버튼이 눌렸을 때 호출 (배치 취소)
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

