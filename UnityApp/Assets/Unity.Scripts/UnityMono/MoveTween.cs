using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTween : MonoBehaviour
{
    public Vector3 ToPosition = Vector3.zero;
    public float Duration = 1f;
    private float StartTime { get; set; }
    private Vector3 StartPosition { get; set; }


    // Start is called before the first frame update
    void Awake()
    {
        StartPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        var elapsedTime = Time.time - StartTime;
        if (elapsedTime > Duration)
        {
            //gameObject.SetActive(false);
            return;
        }
        var progress = elapsedTime / Duration;
        var addPosition = (ToPosition - StartPosition) * progress;
        transform.localPosition = StartPosition + addPosition;
    }
}
