using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [SerializeField] private float lifetime = 30f;

    public static void Create(Vector3 position, Vector3 normal)
    {
        var mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mark.name = "BulletHole";
        mark.transform.SetPositionAndRotation(position + normal * 0.006f, Quaternion.LookRotation(normal));
        mark.transform.localScale = new Vector3(0.12f, 0.12f, 0.008f);
        Destroy(mark.GetComponent<Collider>());

        var shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Color");
        var material = new Material(shader);
        material.color = new Color(0.035f, 0.025f, 0.02f);
        mark.GetComponent<Renderer>().material = material;
        mark.AddComponent<BulletHole>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
