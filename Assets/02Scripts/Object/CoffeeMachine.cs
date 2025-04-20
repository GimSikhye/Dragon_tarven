using UnityEngine;
using DalbitCafe.Core;

namespace DalbitCafe.Operations
{
    public class CoffeeMachine : MonoBehaviour
    {
        public static CoffeeMachine LastTouchedMachine { get; private set; }

        [Header("Ŀ�Ǹӽ� ���� ����")]
        [SerializeField] private CoffeeData _currentCoffee;
        [SerializeField] private int _remainingMugs;
        [SerializeField] private bool _isRoasting = false;
        [SerializeField] private GameObject _steamParticle;

        [SerializeField] private QuestTracker _questTracker;
        [SerializeField] private int unlockedLevel = 1; // �� ������ Ȱ��ȭ�Ǵ� �ӽ�����

        public int UnlockLevel => unlockedLevel;

        public bool IsRoasting => _isRoasting;
        public CoffeeData CurrentCoffee => _currentCoffee;
        public int RemainingMugs => _remainingMugs;

        private void Awake()
        {
            if (_questTracker == null)
                _questTracker = FindObjectOfType<QuestTracker>();

            GameManager.Instance.CoffeeMachineManager.RegisterMachine(this);
        }

        private void OnDestroy()
        {
            GameManager.Instance.CoffeeMachineManager.UnregisterMachine(this);
        }

        public void RoastCoffee(CoffeeData coffee)
        {
            if (_isRoasting) return;
            _isRoasting = true;
            _currentCoffee = coffee;
            _remainingMugs = coffee.MugQty;
            GameManager.Instance.PlayerStatsManager.AddCoffeeBean(-coffee.BeanUse);
            GameObject particle = Instantiate(_steamParticle);
            particle.transform.position = transform.position;
        }

        public void SellCoffee()
        {
            if (_remainingMugs > 1)
            {
                _remainingMugs--;
                GameManager.Instance.PlayerStatsManager.AddCoin(_currentCoffee.Price);
                // ����Ʈ ���� ������Ʈ
                _questTracker.OnCoffeeSold(_currentCoffee.CoffeeId);

            }
            else
            {
                GameManager.Instance.PlayerStatsManager.AddCoin( _currentCoffee.Price); 
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
