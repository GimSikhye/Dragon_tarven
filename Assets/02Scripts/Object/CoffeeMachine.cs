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

        public bool IsRoasting => _isRoasting;
        public CoffeeData CurrentCoffee => _currentCoffee;
        public int RemainingMugs => _remainingMugs;

        public void RoastCoffee(CoffeeData coffee)
        {
            _isRoasting = true;
            _currentCoffee = coffee;
            _remainingMugs = coffee.MugQty;
            Debug.Log($"{coffee.CoffeeName} 커피를 로스팅 시작! 잔 수: {_remainingMugs}");

            GameObject particle = Instantiate(_steamParticle);
            particle.transform.position = transform.position;
        }

        public void SellCoffee()
        {
            if (_remainingMugs > 1)
            {
                _remainingMugs--;
                GameManager.Instance.Coin += _currentCoffee.Price;
                Debug.Log($"{_currentCoffee.CoffeeName} 판매! 남은 잔 수: {_remainingMugs}");
            }
            else
            {
                GameManager.Instance.Coin += _currentCoffee.Price;
                _isRoasting = false;
                _currentCoffee = null;
                Debug.Log("더 이상 판매할 커피가 없습니다!");
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
