using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMenu : ScaleTween, IMenu
{
    // [Tooltip("Object to animate open/close. If null, defaults to this object")]
    // public GameObject hostEntity;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // if (hostEntity == null) { hostEntity = this.gameObject; }        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    // *** IMenu Implementation ***
    private bool m_toggleValue;
    public bool ToggleValue
    {
        get => m_toggleValue;
        set { m_toggleValue = value; }
    }

    public bool Visible => ToggleValue;

    public void Open()
    {
        ToggleValue = true;
        gameObject.SetActive(true);
        base.TweenIn();
    }

    public void Close()
    {
        ToggleValue = false;
        base.TweenOut(delegate ()
        {
            gameObject.SetActive(false);
        });
    }
    
    public void Toggle()
    {
        // ToggleValue = !ToggleValue;
        ToggleValue = !ToggleValue;
        if (ToggleValue == true) { Open(); }
        else { Close(); }
    }
    // *** end IMenu ***
}
