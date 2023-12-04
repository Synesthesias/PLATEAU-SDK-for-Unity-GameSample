using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleView : ViewBase
{
    [SerializeField] private ExtendButton toGameButton;

    private void Start()
    {
        Cursor.visible = false;
    }

    public override IEnumerator Wait()
    {
        while (true)
        {
            //ゲームスタート
            if (toGameButton.IsClicked)
            {
                yield break;
            }

            yield return null;
        }
    }

}
