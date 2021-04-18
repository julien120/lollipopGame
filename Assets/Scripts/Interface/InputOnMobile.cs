using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class InputOnMobile : IInputInterface
{
    /// <summary>
    /// 押した場所と最後に話した場所の座標差で上下左右の出力を変更する
    /// </summary>
    /// <returns></returns>
    public InputDirection InputKey()
    {
        if (Input.touchCount > 0)
        {
            // タッチ情報の取得
            Touch touch = Input.GetTouch(0);
            Vector2 startPos = Input.GetTouch(0).position;
            Vector3 endPos = Input.GetTouch(0).position;

            if (touch.phase == TouchPhase.Began)
            {
                startPos = Input.mousePosition;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                endPos = Input.mousePosition;

                //startPos.x - endPos.xがマイナス且つ、startPos.y - endPos.y > startPos.x - endPos.x
                //右
                if (endPos.x > startPos.x && endPos.x - startPos.x > endPos.y - startPos.y)
                {
                    return InputDirection.Right;
                }

                //左
                else if (endPos.x < startPos.x && startPos.x - endPos.x > endPos.y - startPos.y)
                {
                    return InputDirection.Left;
                }

                //上
                else if (endPos.y > startPos.y && endPos.x - startPos.x < endPos.y - startPos.y)
                {
                    return InputDirection.Up;
                }

                //下
                else if (endPos.y < startPos.y && endPos.x - startPos.x < startPos.y - endPos.y)
                {
                    return InputDirection.Down;
                }
            }


        }

        return InputDirection.None;
    }
}