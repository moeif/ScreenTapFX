using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTapFX : MonoBehaviour
{
    /// <summary>
    /// 屏幕特效原始资源
    /// </summary>
    public GameObject fxSample;

    /// <summary>
    /// 屏幕特效的生命时长，超过后会进行缓存
    /// </summary>
    public float fxLifeTime = 1.0f;

    /// <summary>
    /// 屏幕特效的容器（父对象）
    /// </summary>
    public RectTransform fxContainer;

    /// <summary>
    /// 屏幕特效渲染使用的相机
    /// </summary>
    public Camera fxRenderCamera;

    private Queue<GameObject> pool = new Queue<GameObject>(20);
    private List<GameObject> activatedFXList = new List<GameObject>();

    private void Awake()
    {
        if(fxSample == null)
        {
            Debug.LogErrorFormat("没有找到屏幕特效");
            this.enabled = false;
        }
        else
        {
            fxSample.SetActive(false);
        }
    }

    private void Update()
    {
        for (int i = activatedFXList.Count - 1; i >= 0; --i)
        {
            GameObject fx = activatedFXList[i];
            float fxTime = float.Parse(fx.name);
            if(Time.time - fxTime > fxLifeTime)
            {
                RecycleFX(fx);
                activatedFXList.RemoveAt(i);
            }
        }

        if (Application.isMobilePlatform)
        {
            for(int i = 0; i < Input.touchCount; ++i)
            {
                Touch touch = Input.GetTouch(i);
                if(touch.phase == TouchPhase.Began)
                {
                    PlayFX(touch.position);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayFX(Input.mousePosition);
            }
        }
    }


    private void PlayFX(Vector2 tapPos)
    {
        GameObject fx = CreateFX();
        fx.name = Time.time.ToString();
        activatedFXList.Add(fx);

        RectTransform fxRectTrans = fx.GetComponent<RectTransform>();
        Vector2 fxLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(fxContainer, tapPos, fxRenderCamera, out fxLocalPos);
        fxRectTrans.SetParent(fxContainer);
        fxRectTrans.anchoredPosition3D = fxLocalPos;
        fx.SetActive(true);
    }


    private GameObject CreateFX()
    {
        GameObject newFX = null;
        if(pool.Count > 0)
        {
            newFX = pool.Dequeue();
        }
        else
        {
            newFX = Instantiate(fxSample);
        }
        return newFX;
    }

    private void RecycleFX(GameObject fx)
    {
        fx.SetActive(false);
        pool.Enqueue(fx);
    }

}
