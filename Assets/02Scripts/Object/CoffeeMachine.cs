using UnityEngine;
using DalbitCafe.Core;

namespace DalbitCafe.Operations
{
    public class CoffeeMachine : MonoBehaviour
    {
        public static CoffeeMachine LastTouchedMachine { get; private set; }

        [Header("커피머신 상태 변수")]
        [SerializeField] private CoffeeData _currentCoffee;
        [SerializeField] private int _remainingMugs;
        [SerializeField] private bool _isRoasting = false;
        [SerializeField] private GameObject _steamParticle;

        [SerializeField] private QuestTracker _questTracker;
        [SerializeField] private int unlockedLevel = 1; // 몇 레벨에 활성화되는 머신인지

        public int UnlockLevel => unlockedLevel;

        public bool IsRoasting => _isRoasting;
        public CoffeeData CurrentCoffee => _currentCoffee;
        public int RemainingMugs => _remainingMugs;

        private void Awake()
        {
            if (_questTracker == null)
                _questTracker = FindObjectOfType<QuestTracker>();

            CoffeeMachineManager.Instance.RegisterMachine(this);
        }

        private void OnDestroy()
        {
            CoffeeMachineManager.Instance.UnregisterMachine(this);
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
            if (_remainingMugs > 1)
            {
                _remainingMugs--;
                PlayerStatsManager.Instance.AddCoin(_currentCoffee.Price);
                // 퀘스트 조건 업데이트
                _questTracker.OnCoffeeSold(_currentCoffee.CoffeeId);

            }
            else
            {
                PlayerStatsManager.Instance.AddCoin( _currentCoffee.Price); 
                _isRoasting = false;
                _currentCoffee = null;
            }
        }

        public static void SetLastTouchedMachine(CoffeeMachine machine)
        {
            LastTouchedMachine = machine;
        }

        public bool HasCoffee()
        {
            return _remainingMugs > 0;
        }
    }

}
