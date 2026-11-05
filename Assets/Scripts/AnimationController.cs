using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    public UnityEvent event01;
    public UnityEvent event02;
    public UnityEvent event03;
    public UnityEvent event04;
    public UnityEvent event05;

    public void RunEvent01()
    {
        event01.Invoke();
    }
    public void RunEvent02()
    {
        event02.Invoke();
    }
    public void RunEvent03()
    {
        event03.Invoke();
    }
    public void RunEvent04()
    {
        event04.Invoke();
    }
    public void RunEvent05()
    {
        event05.Invoke();
    }

}
