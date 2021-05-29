using UnityEngine;

/// <summary>
/// Расширение возможностей работы с Transform
/// </summary>
public static class TransformExtension
{
    /// <summary>
    /// Рекурсивный поиск дочернего элемента с определённым именем
    /// </summary>
    /// <param name="parent"> родительский элемент </param>
    /// <param name="childName"> название искомого дочернего элемента </param>
    /// <returns> null - если элемент не найден,
    ///           Transform элемента, если элемент найден
    /// </returns>
    public static Transform FindChildWithName(this Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            var result = child.FindChildWithName(childName);
            if (result)
                return result;
        }

        return null;
    }
}