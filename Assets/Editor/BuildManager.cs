using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildManager
{
    // Путь для сохранения WebGL-версии игры
    private static readonly string WebGLBuildPath = "Builds/WebGL";

    /// <summary>
    /// Автоматический метод сборки игры под платформу WebGL
    /// </summary>
    public static void BuildWebGL()
    {
        Debug.Log("[CI/CD] Запущен автоматический процесс сборки WebGL...");

        // 1. Получаем список сцен, включенных в настройки проекта (Build Settings)
        string[] levels = GetScenes();
        if (levels.Length == 0)
        {
            Debug.LogError("[CI/CD] Ошибка: В Настройках Сборки (Build Settings) не найдено ни одной активной сцены!");
            ExitWithCode(1);
            return;
        }

        // 2. Конфигурируем параметры сборки
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = levels,
            locationPathName = WebGLBuildPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None // Для отладочной сборки можно использовать BuildOptions.Development
        };

        // 3. Запускаем компиляцию
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        // 4. Анализируем результаты
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("[CI/CD] УСПЕХ! WebGL билд успешно создан.");
            Debug.Log($"[CI/CD] Время сборки: {summary.totalTime.TotalSeconds:F2} сек. Размер: {summary.totalSize} байт.");
            ExitWithCode(0);
        }
        else
        {
            Debug.LogError($"[CI/CD] ОШИБКА СБОРКИ! Количество ошибок: {summary.totalErrors}");
            ExitWithCode(1);
        }
    }

    /// <summary>
    /// Вспомогательный метод для сбора всех активных сцен из проекта
    /// </summary>
    private static string[] GetScenes()
    {
        var editorScenes = EditorBuildSettings.scenes;

        // Считаем только те сцены, у которых стоит галочка "активна"
        int activeCount = 0;
        foreach (var scene in editorScenes)
        {
            if (scene.enabled) activeCount++;
        }

        string[] scenePaths = new string[activeCount];
        int index = 0;
        foreach (var scene in editorScenes)
        {
            if (scene.enabled)
            {
                scenePaths[index] = scene.path;
                index++;
            }
        }

        return scenePaths;
    }

    /// <summary>
    /// Корректное завершение процесса Unity в зависимости от режима запуска
    /// </summary>
    private static void ExitWithCode(int code)
    {
        // Если Unity запущена в batchmode (из консоли) – принудительно закрываем редактор с кодом возврата
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(code);
        }
    }
}