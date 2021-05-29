#if UNITY_EDITOR
using System;
using UnityEditor.Events;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Класс для обновления сцен, включённых в список BuildSettings (активные и неактивные)
/// </summary>
public class SceneUpdater : EditorWindow
{
    [MenuItem("Custom Tools/Scene Updater")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SceneUpdater));
    }

    private void OnGUI()
    {
        // пример использования
        if (GUILayout.Button("Update scenes"))
            RunSceneUpdateCycle((() =>
            {
                // изменение тэга и слоя объекта
                var cube = GameObject.Find("Cube");
                cube.tag = "Player";
                ChangeObjectLayer(cube, "MainLayer");
                
                // добавление компонента к объекту с уникальным названием
                AddComponentToObject<BoxCollider>("Plane");
                
                // удаление объекта с уникальным названием
                DestroyObjectWithName("Sphere");
                
                // создание нового объекта на сцене и добавление его в иерархию к существующему
                InstantiateNewGameObject("Assets/Prefabs/Capsule.prefab", cube, 1);

                // изменение настроек отображения Canvas
                var canvas = GameObject.Find("Canvas");
                ChangeCanvasSettings(canvas, RenderMode.ScreenSpaceOverlay, CanvasScaler.ScaleMode.ScaleWithScreenSize);

                // изменение настроект шрифта
                var tmp = canvas.GetComponentInChildren<TextMeshProUGUI>();
                ChangeTMPSettings(tmp, 36, 72, TextAlignmentOptions.BottomRight);

                // изменение RectTransform
                ChangeRectTransformSettings(tmp.GetComponent<RectTransform>(), AnchorPresets.MiddleCenter, Vector3.zero, new Vector2(100f, 20f));
                
                // добавление события на кнопку
                AddPersistentListenerToButton(canvas.GetComponentInChildren<Button>(), FindObjectOfType<SampleClass>().QuitApp);

                // копирование настроек компонента
                CopyTransformPositionRotationScale(GameObject.Find("Plane"), cube);
            }));
    }

    /// <summary>
    /// Запускает цикл обновления сцен в Build Settings
    /// </summary>
    /// <param name="onSceneLoaded"> действие при открытии сцены </param>
    private void RunSceneUpdateCycle(UnityAction onSceneLoaded)
    {
        // получение путей к сценам для дальнейшего открытия
        var scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToList();
        foreach (var scene in scenes)
        {
            // открытие сцены
            EditorSceneManager.OpenScene(scene);
            
            // пометка для сохранения, что на сцене были произведены изменения
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            
            // проведение изменений
            onSceneLoaded?.Invoke();
            
            // сохранение
            EditorApplication.SaveScene();
            
            Debug.Log($"UPDATED {scene}");
        }
    }

    /// <summary>
    /// Добавление обработчика события на кнопку (чтобы было видно в инстпекторе)
    /// </summary>
    /// <param name="uiButton"> кнопка </param>
    /// <param name="action"> требуемое действие </param>
    private static void AddPersistentListenerToButton(Button uiButton, UnityAction action)
    {
        try
        {
            // сработает, если уже есть пустое событие
            if (uiButton.onClick.GetPersistentTarget(0) == null)
                UnityEventTools.RegisterPersistentListener(uiButton.onClick, 0, action);
        }
        catch (ArgumentException)
        {
            UnityEventTools.AddPersistentListener(uiButton.onClick, action);
        }
    }

    /// <summary>
    /// Изменение параметров RectTransform
    /// </summary>
    /// <param name="rectTransform"> изменяемый элемент </param>
    /// <param name="alignment"> выравнивание </param>
    /// <param name="position"> позиция в 3D-пространстве </param>
    /// <param name="size"> размер </param>
    private void ChangeRectTransformSettings(RectTransform rectTransform, AnchorPresets alignment, Vector3 position, Vector2 size)
    {
        rectTransform.anchoredPosition3D = position;
        rectTransform.sizeDelta = size;
        rectTransform.SetAnchor(alignment);
    }

    /// <summary>
    /// Изменение настроек для TextMeshPro
    /// </summary>
    /// <param name="textMeshPro"> тестовый элемент </param>
    /// <param name="fontSizeMin"> минимальный размер шрифта </param>
    /// <param name="fontSizeMax"> максимальный размер шрифта </param>
    /// <param name="textAlignmentOption"> выравнивание текста </param>
    private void ChangeTMPSettings(TextMeshProUGUI textMeshPro, int fontSizeMin, int fontSizeMax, TextAlignmentOptions textAlignmentOption = TextAlignmentOptions.Center)
    {
        // замена стандартного шрифта
        textMeshPro.font = (TMP_FontAsset) AssetDatabase.LoadAssetAtPath("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset", typeof(TMP_FontAsset));
        textMeshPro.enableAutoSizing = true;
        textMeshPro.fontSizeMin = fontSizeMin;
        textMeshPro.fontSizeMax = fontSizeMax;
        textMeshPro.alignment = textAlignmentOption;
    }

    /// <summary>
    /// Изменение отображения Canvas
    /// </summary>
    /// <param name="canvasGameObject"> объект, в компонентам которого будет производиться обращение </param>
    /// <param name="renderMode"> способ отображения </param>
    /// <param name="scaleMode"> способ изменения масштаба </param>
    private void ChangeCanvasSettings(GameObject canvasGameObject, RenderMode renderMode, CanvasScaler.ScaleMode scaleMode)
    {
        canvasGameObject.GetComponentInChildren<Canvas>().renderMode = renderMode;

        var canvasScaler = canvasGameObject.GetComponentInChildren<CanvasScaler>();
        canvasScaler.uiScaleMode = scaleMode;

        // выставление стандартного разрешения
        if (scaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            canvasScaler.referenceResolution = new Vector2(720f, 1280f);
            canvasScaler.matchWidthOrHeight = 1f;
        }
    } 
    
    /// <summary>
    /// Получение всех верхних дочерних элементов
    /// </summary>
    /// <param name="parentGameObject"> родительский элемент </param>
    /// <returns> список дочерних элементов </returns>
    private static List<GameObject> GetAllChildren(GameObject parentGameObject)
    {
        var children = new List<GameObject>();
        
        for (int i = 0; i< parentGameObject.transform.childCount; i++)
            children.Add(parentGameObject.transform.GetChild(i).gameObject);
        
        return children;
    }

    /// <summary>
    /// Копирование позиции, поворота и размера с компонента Transform у одного объекта
    /// на такой же компонент другого объекта.
    /// Для корректного переноса координат у parent root объеков должны быть нулевые координаты
    /// </summary>
    /// <param name="objectToCopyFrom"> объект, с которого копируются части компонента </param>
    /// <param name="objectToPasteTo"> объект, на который вставляются части компонента </param>
    /// <param name="copyPosition"> по умолчанию позиция копируется, с помощью данного параметра это можно отключить </param>
    /// <param name="copeRotation"> по умолчанию поворот копируется, с помощью данного параметра это можно отключить </param>
    /// <param name="copyScale"> по умолчанию размер копируется, с помощью данного параметра это можно отключить </param>
    private static void CopyTransformPositionRotationScale(GameObject objectToCopyFrom, GameObject objectToPasteTo, 
        bool copyPosition = true, bool copeRotation = true, bool copyScale = true)
    {
        var newTransform = objectToCopyFrom.GetComponent<Transform>();
        var currentTransform = objectToPasteTo.GetComponent<Transform>();
        
        if (copyPosition) currentTransform.localPosition = newTransform.localPosition;
        if (copeRotation) currentTransform.localRotation = newTransform.localRotation;
        if (copyScale) currentTransform.localScale = newTransform.localScale;
    }
    
    /// <summary>
    /// Копирование позиции, поворота и размера с компонента RectTransform у UI-панели одного объекта
    /// на такой же компонент другого объекта. Не копируется размер самой панели (для этого использовать sizeDelta)
    /// Для корректного переноса координат у parent root объеков должны быть нулевые координаты
    /// </summary>
    /// <param name="objectToCopyFrom"> объект, с которого копируются части компонента </param>
    /// <param name="objectToPasteTo"> объект, на который вставляются части компонента </param>
    /// <param name="copyPosition"> по умолчанию позиция копируется, с помощью данного параметра это можно отключить </param>
    /// <param name="copeRotation"> по умолчанию поворот копируется, с помощью данного параметра это можно отключить </param>
    /// <param name="copyScale"> по умолчанию размер копируется, с помощью данного параметра это можно отключить </param>
    private static void CopyRectTransformPositionRotationScale(GameObject objectToCopyFrom, GameObject objectToPasteTo,
        bool copyPosition = true, bool copeRotation = true, bool copyScale = true)
    {
        var newTransform = objectToCopyFrom.GetComponent<RectTransform>();
        var currentTransform = objectToPasteTo.GetComponent<RectTransform>();
        
        if (copyPosition) currentTransform.localPosition = newTransform.localPosition;
        if (copeRotation) currentTransform.localRotation = newTransform.localRotation;
        if (copyScale) currentTransform.localScale = newTransform.localScale;
    }

    /// <summary>
    /// Уничтожение объекта с конктретным названием
    /// </summary>
    /// <param name="objectName"> название объекта </param>
    private void DestroyObjectWithName(string objectName)
    {
        DestroyImmediate(GameObject.Find(objectName)?.gameObject);
    }

    /// <summary>
    /// Добавление компонента к объекту с конктретным названием
    /// </summary>
    /// <param name="objectName"> название объекта </param>
    /// <typeparam name="T"> тип компонента </typeparam>
    private void AddComponentToObject<T>(string objectName) where T : Component
    {
        GameObject.Find(objectName)?.gameObject.AddComponent<T>();
    }

    /// <summary>
    /// Изменение слоя объекта по названию слоя
    /// </summary>
    /// <param name="gameObject"> объект </param>
    /// <param name="layerName"> название слоя </param>
    private void ChangeObjectLayer(GameObject gameObject, string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// Добавление префаба на сцену с возможностью определения родительского элемента и порядка в иерархии
    /// </summary>
    /// <param name="prefabPath"> путь к префабу </param>
    /// <param name="parentGameObject"> родительский объект </param>
    /// <param name="hierarchyIndex"> порядок в иерархии родительского элемента </param>
    private void InstantiateNewGameObject(string prefabPath, GameObject parentGameObject, int hierarchyIndex = 0)
    {
        if (parentGameObject)
        {
            var newGameObject = Instantiate((GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)), parentGameObject.transform);
            
            // изменение порядка в иерархии сцены внутри родительского элемента
            newGameObject.transform.SetSiblingIndex(hierarchyIndex);
        }
        else
            Instantiate((GameObject) AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)));
    }
}
#endif