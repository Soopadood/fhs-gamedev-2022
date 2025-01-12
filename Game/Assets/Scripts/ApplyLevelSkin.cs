using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ApplyLevelSkin : MonoBehaviour

    //This doesnt do anything yet -- work in progress (will olson 2.7.23)
{


    [SerializeField] private GameObject _worldSkin;





    // Start is called before the first frame update
    void Start()
    {
        var groundParent = GameObject.Find("Ground").transform;
        var groundChildren = new List<SpriteRenderer>();
        for (int i=groundParent.childCount - 1;i >= 0; i--)
        {
            var instance = Instantiate(_worldSkin, groundParent.GetChild(i).transform.position, Quaternion.identity);
            instance.GetComponent<SpriteRenderer>().size=(Vector2) groundParent.GetChild(i).localScale * 2.5f * 0.96f;
            groundParent.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
            groundChildren.Add(instance.GetComponent<SpriteRenderer>());
        }
        groundChildren.OrderByDescending(s => s.size.magnitude);
        groundChildren.Reverse();
        var g = groundChildren.Count + 2; //+2 to give leeway with other objects in lower sorting layers
        foreach (var child in groundChildren)
        {
            child.GetComponent<SpriteRenderer>().sortingOrder = g;
            g--;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
