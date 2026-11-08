using NUnit.Framework;
using Platformer.Mechanics;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Тестовая сборка Tests (includePlatforms: Editor) — тесты EditMode
public class PlayerTests
{
    // Путь к сцене игры из Build Settings проекта
    private const string MainScenePath = "Assets/Scenes/SampleScene.unity";

    // EDITMODE ТЕСТ: проверка правильности настройки сцены
    [Test]
    public void EditMode_PlayerTag_IsCorrect()
    {
        // Открываем сцену игры, чтобы получить доступ к её объектам
        var scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
        Assert.IsTrue(scene.IsValid(), "Ошибка: не удалось открыть сцену SampleScene!");

        // Ищем объект игрока по имени. Внимание: FindWithTag("Player") здесь
        // ненадежен — в микроигре тегом Player помечены и враги (Enemy.prefab)
        GameObject player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Ошибка: на сцене не найден объект игрока 'Player'!");

        // Проверяем, что у объекта игрока не сброшен обязательный тег "Player"
        Assert.AreEqual("Player", player.tag, "Ошибка: у объекта игрока сброшен тег 'Player'!");

        // Проверяем, что у игрока есть обязательные компоненты
        Assert.IsNotNull(player.GetComponent<PlayerController>(),
            "Ошибка: на объекте игрока отсутствует скрипт PlayerController!");
        Assert.IsNotNull(player.GetComponent<Health>(),
            "Ошибка: на объекте игрока отсутствует скрипт Health!");
    }
}
