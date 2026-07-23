using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject player;
    public GameObject cat;
    public GameObject microwave;
    public GameObject microwave_canvas;
    public GameObject ui_canvas;

    private void Awake()
    {
        Instantiate(ui_canvas);
        Instantiate(microwave_canvas);
    }
    void Start()
    {
        Instantiate(player);
        Instantiate(cat);
        Instantiate(microwave);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
