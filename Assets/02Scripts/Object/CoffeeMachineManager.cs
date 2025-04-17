using System.Collections.Generic;
using UnityEngine;

namespace DalbitCafe.Operations
{
    public class CoffeeMachineManager : MonoBehaviour
    {
        private List<CoffeeMachine> machines = new List<CoffeeMachine>();

        public void RegisterMachine(CoffeeMachine machine) // 등록
        {
            if (!machines.Contains(machine))
                machines.Add(machine);
        }

        public void UnregisterMachine(CoffeeMachine machine) // 등록해제
        {
            if (machines.Contains(machine))
                machines.Remove(machine);
        }

        public CoffeeMachine GetMachineAtPosition(Vector2 position, float threshold = 0.5f)
        {
            foreach (var machine in machines)
            {
                if (Vector2.Distance(machine.transform.position, position) < threshold)
                    return machine;
            }
            return null;
        }

        public void UpdateMachineActivation(int currentLevel)
        {
            foreach (var machine in machines)
            {
                machine.gameObject.SetActive(currentLevel >= machine.UnlockLevel); // 현재 레벨이 해금레벨 이상이면
            }
        }


        public List<CoffeeMachine> GetAllMachines() => machines;
    }
}
