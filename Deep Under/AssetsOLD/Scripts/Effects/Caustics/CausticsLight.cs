using UnityEngine;

public class CausticsLight : MonoBehaviour {

	public float Framerate = 30.0f;
    [SerializeField] private Texture2D[] CookieFrames;
    private int FrameIndex;
    private Light Light;
 
    void Awake()
    {
        this.Light = GetComponent<Light>();
    }
 
    void Start()
    {
        InvokeRepeating("NextFrame", 0, 1/Framerate);
    }
 
    void NextFrame()
    {
        this.Light.cookie = CookieFrames[this.FrameIndex];
        FrameIndex = (FrameIndex + 1) % CookieFrames.Length;
    }
    
    [ContextMenu ("Sort Frames by Name")]
    void DoSortFrames() {
        System.Array.Sort(CookieFrames, (a,b) => a.name.CompareTo(b.name));
        Debug.Log(gameObject.name + ".frames have been sorted alphabetically.");
    }
}
