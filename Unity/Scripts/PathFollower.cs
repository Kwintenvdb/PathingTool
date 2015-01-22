using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFollower : MonoBehaviour
{
    // Path to follow
    public Path Path = new Path();
    // How long the animation should last
    public float AnimationTime;

    private float _timer;
    
    public void Start()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        _timer = 0;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > AnimationTime) _timer = 0;
        var t = _timer/AnimationTime;      
        
        var pt = Path.GetPoint(t);
        pt = pt/5;
        transform.localPosition = new Vector3(pt.x, 0, pt.y);
    }
}