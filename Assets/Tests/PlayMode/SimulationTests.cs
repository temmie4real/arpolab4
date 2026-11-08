using System.Collections;
using NUnit.Framework;
using Platformer.Core;
using UnityEngine;
using UnityEngine.TestTools;

// Тестовая сборка PlayModeTests (includePlatforms: Editor не указан) — тесты PlayMode
public class SimulationTests
{
    // Тестовое событие для проверки дискретного планировщика:
    // при выполнении Execute() выставляет статический флаг
    private class TestEvent : Simulation.Event
    {
        public static bool Executed;

        public override void Execute()
        {
            Executed = true;
        }
    }

    // PLAYMODE ТЕСТ (индивидуальный вариант №20):
    // запланированное событие выполняется после Tick()
    [UnityTest]
    public IEnumerator PlayMode_ScheduledEvent_ExecutesAfterTick()
    {
        // Сбрасываем очередь событий, чтобы тест не зависел от других тестов
        Simulation.Clear();
        TestEvent.Executed = false;

        // Планируем событие через 1.5 секунды игрового времени
        TestEvent scheduled = Simulation.Schedule<TestEvent>(1.5f);
        Assert.IsNotNull(scheduled, "Ошибка: Simulation.Schedule не вернул запланированное событие!");
        Assert.IsFalse(TestEvent.Executed, "Ошибка: событие не должно выполняться до своего срока и до Tick()!");

        // Ждем 2 секунды игрового времени: срок события уже наступил,
        // но без Tick() симулятор ничего выполнять не должен
        yield return new WaitForSeconds(2f);
        Assert.IsFalse(TestEvent.Executed,
            "Ошибка: событие выполнилось само, хотя Tick() еще не вызывался!");

        // Tick() выполняет все события, чей срок наступил
        int remaining = Simulation.Tick();

        Assert.IsTrue(TestEvent.Executed,
            "Ошибка: запланированное событие не выполнилось после Tick()!");
        Assert.AreEqual(0, remaining,
            "Ошибка: после выполнения события очередь событий должна опустеть!");
    }
}
