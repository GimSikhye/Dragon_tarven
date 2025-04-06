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

        public bool IsRoasting => _isRoasting;
        public CoffeeData CurrentCoffee => _currentCoffee;
        public int RemainingMugs => _remainingMugs;

        public void RoastCoffee(CoffeeData coffee)
        {
            _isRoasting = true;
            _currentCoffee = coffee;
            _remainingMugs = coffee.MugQty;
            Debug.Log($"{coffee.CoffeeName} Ŀ�Ǹ� �ν��� ����! �� ��: {_remainingMugs}");

            GameObject particle = Instantiate(_steamParticle);
            particle.transform.position = transform.position;
        }

        public void SellCoffee()
        {
            if (_remainingMugs > 1)
            {
                _remainingMugs--;
                GameManager.Instance.playerStats.coin += _currentCoffee.Price;
                Debug.Log($"{_currentCoffee.CoffeeName} �Ǹ�! ���� �� ��: {_remainingMugs}");
            }
            else
            {
                GameManager.Instance.playerStats.coin += _currentCoffee.Price;
                _isRoasting = false;
                _currentCoffee = null;
                Debug.Log("�� �̻� �Ǹ��� Ŀ�ǰ� �����ϴ�!");
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
