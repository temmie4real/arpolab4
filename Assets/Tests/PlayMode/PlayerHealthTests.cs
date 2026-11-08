using System.Collections;
using NUnit.Framework;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.TestTools;

// Тестовая сборка PlayModeTests (includePlatforms: Editor не указан) — тесты PlayMode
public class PlayerHealthTests
{
    // PLAYMODE ТЕСТ: симуляция уменьшения здоровья персонажа
    [UnityTest]
    public IEnumerator PlayMode_PlayerTakesDamage_HealthDecreases()
    {
        // Создаем временный объект и добавляем ему реальный скрипт здоровья из игры
        GameObject playerObj = new GameObject("TestPlayer");

        // Откладываем вызов Awake: неактивный объект вызывает Awake только после активации
        playerObj.SetActive(false);
        Health health = playerObj.AddComponent<Health>();

        // Задаем стартовое здоровье (3 хитпоинта) ДО инициализации компонента
        health.maxHP = 3;
        playerObj.SetActive(true); // Awake выставит currentHP = maxHP = 3

        Assert.IsTrue(health.IsAlive, "Ошибка: персонаж должен быть жив на старте!");

        // Имитируем получение урона: Health.Decrement() уменьшает HP на 1
        health.Decrement();
        Assert.IsTrue(health.IsAlive, "Ошибка: персонаж не должен погибнуть после одного урона!");

        // Наносим оставшийся урон до смерти
        health.Decrement();
        health.Decrement();
        Assert.IsFalse(health.IsAlive, "Ошибка: логика нанесения урона работает некорректно!");

        // Удаляем объект после теста, чтобы очистить память
        Object.Destroy(playerObj);

        // Пропускаем один кадр для симуляции игрового времени
        yield return null;
    }
}
