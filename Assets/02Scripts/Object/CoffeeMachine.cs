using UnityEngine;
using DalbitCafe.Core;

namespace DalbitCafe.Operations
{
    public class CoffeeMachine : MonoBehaviour
    {
        [Header("커피머신 상태 변수")]
        [SerializeField] private CoffeeData _currentCoffee;
        [SerializeField] private int _remainingMugs; // 남은 잔수
        [SerializeField] private bool _isRoasting = false; // 현재 로스팅 중인지
        [SerializeField] private GameObject _steamParticle; // 연기 파티클

        [SerializeField] private QuestTracker _questTracker;
        [SerializeField] private int unlockedLevel = 1; // 몇 레벨에 활성화되는 머신인지
        [SerializeField] private CoffeeMachineManager _machineManager;
        [SerializeField] private float coffeeSellCooldown = 2f;

        public int UnlockLevel => unlockedLevel;
        public bool IsRoasting => _isRoasting;
        public CoffeeData CurrentCoffee => _currentCoffee;
        public int RemainingMugs => _remainingMugs;

        private void Awake()
        {
            if (_questTracker == null)
                _questTracker = FindObjectOfType<QuestTracker>();

            _machineManager.RegisterMachine(this);
        }

        private void Update()
        {
            if (CurrentCoffee == null) return;

            if(coffeeSellCooldown > 0)
            {
                coffeeSellCooldown -= Time.deltaTime;
            }
            else
            {
                coffeeSellCooldown = 2f;
                SellCoffee();
            }
        }

        private void OnDestroy()
        {
            _machineManager.UnregisterMachine(this);
        }

        public void RoastCoffee(CoffeeData coffee)
        {
            if (_isRoasting) return;
            _isRoasting = true;

            _currentCoffee = coffee;
            _remainingMugs = coffee.MugQty; 

            PlayerStatsManager.Instance.AddCoffeeBean(-coffee.BeanUse); 

            GameObject particle = Instantiate(_steamParticle);
            particle.transform.position = transform.position;
        }

        public void SellCoffee()
        {
            if (_remainingMugs > 1) // 남은 커피가 1잔보다 많다면
            {
                _remainingMugs--;
                PlayerStatsManager.Instance.AddCoin(_currentCoffee.Price);

                // 퀘스트 조건 업데이트
                //_questTracker.OnCoffeeSold(_currentCoffee.CoffeeId);

            }
            else // 1잔이라면
            {
                PlayerStatsManager.Instance.AddCoin(_currentCoffee.Price);
                _isRoasting = false;
                _currentCoffee = null;
            }
        }



        public bool HasCoffee()
        {
            return _remainingMugs > 0;
        }
    }

}
