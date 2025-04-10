using UnityEngine;

public class ShoeGazeRaycaster : MonoBehaviour
{
    public float gazeDistance = 10f;
    private ShoeData lastLookedShoe = null;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, gazeDistance))
        {
            ShoeData shoe = hit.collider.GetComponent<ShoeData>();
            if (shoe != null && shoe != lastLookedShoe)
            {
                ShoeCanvasController.Instance.ShowShoeInfo(shoe.shoeName, shoe.price, shoe.shoeImage, shoe.shoeDescription);
                lastLookedShoe = shoe;
            }
        }
    }
}
