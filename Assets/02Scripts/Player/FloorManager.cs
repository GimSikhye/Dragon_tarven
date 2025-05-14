using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

namespace DalbitCafe.Map
{
    public class FloorManager : MonoSingleton<FloorManager>
    {
        [SerializeField] private Tilemap floorTilemap;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameScene" && floorTilemap == null)
            {
                floorTilemap = GameObject.Find("1FFloor").GetComponent<Tilemap>();
            }
        }

        public bool IsFloor(Vector3 worldPos)
        {
            if (floorTilemap == null)
            {
                Debug.LogWarning("[FloorManager] floorTilemap�� ��� �ֽ��ϴ�. �ٴ� üũ ����.");
                return false;
            }

            Vector3Int cellPos = floorTilemap.WorldToCell(worldPos); // ���� ��ǥ�� Ÿ�ϸ��� �� ��ǥ�� ��ȯ����
            return floorTilemap.HasTile(cellPos);
        }
    }
}
