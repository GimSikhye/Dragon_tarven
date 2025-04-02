using UnityEngine;

// 배치 가능 여부 체크
namespace DalbitCafe.Deco
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int _gridWidth = 10;  // 맵 가로 크기
        [SerializeField] private int _gridHeight = 10; // 맵의 세로 크기(배치할 수 있는 공간)
        [SerializeField] private float _tileSize = 0.5f; // 타일 크기 (그리드 셀의 크기)

        private bool[,] _grid;  // 그리드 상태를 나타내는 2D 배열 (배치 여부) / Flase이면 배치할 수 있는 상태

        private void Start()
        {
            // 그리드 초기화
            _grid = new bool[_gridWidth, _gridHeight];
        }

        // 그리드에서 특정 위치에 아이템을 배치할 수 있는지 체크
        public bool CanPlaceItem(Vector2Int position, Vector2Int size)
        {
            // 그리드 밖으로 나가지 않도록 체크
            // 왼쪽 바깥이거나, 아래쪽 넘어가거나, 현재위치+사이즈(영역차지하는크기)가 오른쪽을 넘어가거나, 현재위치+사이즈가 마지막높이를 넘어간다면
            if (position.x < 0 || position.y < 0 || position.x + size.x > _gridWidth || position.y + size.y > _gridHeight)
            {
                return false; // 배치 불가
            }

            // 아이템이 차지하는 영역에 다른 아이템이 있는지 확인
            for (int x = position.x; x < position.x + size.x; x++)
            {
                for (int y = position.y; y < position.y + size.y; y++)
                {
                    if (_grid[x, y])
                    {
                        return false;  // 이미 배치된 곳이라면 불가능
                    }
                }
            }

            return true;  // 배치 가능
        }

        // 그리드에 아이템 배치
        public void PlaceItem(Vector2Int position, Vector2Int size)
        {
            // 아이템이 차지하는 영역에 아이템 배치
            for (int x = position.x; x < position.x + size.x; x++)
            {
                for (int y = position.y; y < position.y + size.y; y++)
                {
                    _grid[x, y] = true;  // 해당 위치에 아이템 배치됨 // 흠... 그리드에 배치되었는지만 체크하고, 그 위치에 게임오브젝트(스프라이트는 배치안하네)
                }
            }
        }

        // 그리드에서 아이템을 제거 (배치 취소)
        public void RemoveItem(Vector2Int position, Vector2Int size)
        {
            // 아이템이 차지하는 영역에서 아이템을 제거
            for (int x = position.x; x < position.x + size.x; x++)
            {
                for (int y = position.y; y < position.y + size.y; y++)
                {
                    _grid[x, y] = false;  // 해당 위치에서 아이템 제거
                    // 여기도 이 위치에 있는 데코아이템 삭제 안하네
                }
            }
        }

        // 그리드 상태를 디버그하기 위한 함수 (원하는 경우)
        private void OnDrawGizmos()
        {
            if (_grid == null) return;

            // 그리드를 그려보기 위한 Gizmos
            Gizmos.color = Color.green;
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    if (_grid[x, y]) // 해당 그리드에 아이템이 배치되어있다면 (true라면)
                    {
                        Gizmos.DrawCube(new Vector3(x * _tileSize, y * _tileSize, 0), new Vector3(_tileSize, _tileSize, 0.1f));
                    }
                }
            }
        }
    }
}

