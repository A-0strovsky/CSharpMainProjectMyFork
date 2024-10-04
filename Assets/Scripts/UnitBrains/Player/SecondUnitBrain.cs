using System.Collections.Generic;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f; // макс тем 
        private const float OverheatCooldown = 2f; // время восстановления перегрева 
        private float _temperature = 0f;
        private float _cooldownTime = 0f; // время простоя при охл
        private bool _overheated; // перегретый 
        
        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature; // макс темп // ниже условие по ДЗ 
            ///////////////////////////////////////
            GetTemperature();
            
            if (GetTemperature() <= overheatTemperature)
            {
                for (int i = 0; i < 3; i++)
                {
                    var projectile = CreateProjectile(forTarget);
                    AddProjectileToList(projectile, intoList);
                }
            }
            else
            {
                return;
            }

            IncreaseTemperature();
            /////////////////
        }

        public override Vector2Int GetNextStep()
        {
            return base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            ///////////////////////////////////////
            ///Тут должон быть код, но либо я тупой, либо задания к ДЗ невнятные, как и ДЗ в целом. 
            ///////////////////////////////////////
            List<Vector2Int> result = GetReachableTargets(); // old
            //слизано начало 

            IEnumerable<Vector2Int> allTargets = GetAllTargets(); // так как слизано, не совсем понимаю на основе чего список - но он считает всех на поле

            float minDistance = float.MaxValue; // ну тут мы считаем минимальную дистанцию до базы 
            Vector2Int res = Vector2Int.zero; // создаем новый вектор который отслеживает не понимаю что, так как его точка в пространстве = 0 ?!?!
            foreach (var target in allTargets) // ну тут понятно
            {
                var distanceTarget = DistanceToOwnBase(target); // присваемваем нашей новой переменной с неявным типом наш магический метод и включаем в него наш таргет, он следит за расстоянием
                if (distanceTarget < minDistance) // если дистанция цели меньше чем минимальная, то:
                {
                    minDistance = distanceTarget; // мниммальная дистанция равна дистанции цели ??? WTF
                    res = target; // наш новый векто рес равен цели ??? WTF
                }
            }

            result.Clear(); // мы очищаем наш список → 
            if (minDistance != float.MaxValue) // → если  минимальная дистанция не равно максимальному значения типа данны float ? WTF
            {
                result.Add(res); // ТО:  мы добавляем даные позиции из нашего нового вектора в список "result" 
            }

            //слизано конец // в проекте цель не бросаются хаотично при этих изменениях и частично выполняют защиту башни устраняя ближайшие объекты. 
            //old

            while (result.Count > 1) // прошу прощения, вообще не пониманию за что отвечает данная часть, но без нее не работает. 
            {
                result.RemoveAt(result.Count - 1);
            }
            return result;

            /* решение кода слизано из "стремные вопросы". Исходят из ТЗ в ДЗ задача невнятная и непонятная. Я приношу свои искрение извинения, но у меня нет понимаю, как выполнять ДЗ...
            ... так как их описания желаю лучшего, а то, что мне нужно воспользоваться методом, принцип которого я не вижу и не понимаю, меня вводит в тупик. */
            //Уважаемый проверяющий, я прошу дать мне развернутый фитбек, как это решается и объяснить по какому принципы. Я не самый умный человек возможно, но я и бесплатно это знал, заплатил я для того, чтобы это исправить, а не убедиться в этом окончатель.

            ///////////////////////////////////////
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {              
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown/10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature() // метод получения температуры 
        {
            if(_overheated) return (int) OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature() // увелечение температуры 
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}