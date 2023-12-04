using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager: MonoBehaviour
{
    public Image cursorImage;              // UI Image
    public Sprite defaultCursorSprite;     // 通常のカーソルのスプライト
    public Sprite highlightCursorSprite;   // 光らせたいカーソルのスプライト

    public void OnEnable()
    {
        // Mouseデバイスのトラッキングを有効にする
        InputSystem.EnableDevice(Mouse.current);
         Cursor.visible = false;
    }

    public void OnDisable()
    {
        // Mouseデバイスのトラッキングを無効にする
        InputSystem.DisableDevice(Mouse.current);
    }

    public void Update()
    {
        // Mouseの位置を取得してカーソルの位置を更新
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        cursorImage.rectTransform.position = mousePosition;
        cursorImage.sprite = highlightCursorSprite;
        
  
    }

    bool ShouldHighlightCursor()
    {
        // 光らせたい条件をここに追加
        // 例: 特定のオブジェクトにマウスが重なっている場合など
        return false;
    }
}

