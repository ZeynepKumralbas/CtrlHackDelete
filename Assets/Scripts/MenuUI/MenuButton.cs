using UnityEngine;
using DG.Tweening;

public class MenuButton : MonoBehaviour
{
    public void OnHover()
    {
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnExit()
    {
        transform.DOScale(1f, 0.2f).SetEase(Ease.InBack);
    }
}
