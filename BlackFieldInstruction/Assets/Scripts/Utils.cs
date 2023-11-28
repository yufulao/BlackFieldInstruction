using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Utils
{
    /// <summary>
    /// 在场景中生成文字
    /// </summary>
    /// <param name="text">文字内容</param>
    /// <param name="parent">文字父物体</param>
    /// <param name="localPosition">文字相对父物体的偏移</param>
    /// <param name="localScale">文字缩放</param>
    /// <param name="fontSize">文字大小</param>
    /// <param name="color">文字颜色</param>
    /// <param name="textAnchor">文字锚点</param>
    /// <param name="textAlignment">文字对齐方式</param>
    /// <param name="sortingOrder">文字显示图层</param>
    /// <returns></returns>
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3),Vector3 localScale=default, int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 0) {
        if (color == null) color = Color.white;
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = (Color)color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
