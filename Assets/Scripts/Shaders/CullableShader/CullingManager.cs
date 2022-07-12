using System.Collections.Generic;
using UnityEngine;

public class CullingManager : MonoBehaviour
{
    [SerializeField] private float occlusionCapsuleHeight = 0f;

    [SerializeField] private float occlusionCapsuleRadius = 1f;

    // list of objects that will trigger the culling effect
    [SerializeField] private List<GameObject> importantObjects = new List<GameObject>();

    // include the mouse in the important objects
    [SerializeField] private bool includeMouse;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private MousePositionManager mouseManager;

    // List of all the objects that we've set to occluding state
    private List<Cullable> occludingObjects = new List<Cullable>();

    private List<Cullable> cullableList = new List<Cullable>();

    // Update is called once per frame // Handle per frame logic
    public void Update()
    {
        // Can only do occlusion checks if we have a camera
        if (Camera.main != null)
        {
            // This is the list of positions we're trying not to occlude
            List<Vector3> importantPositions = FindImportantPositions();

            // This is the list of objects whihc are in the way
            List<Cullable> newOccludingObjects = FindOccludingObjects(importantPositions);

            SetOccludingObjects(newOccludingObjects);
        }
    }

    private List<Vector3> FindImportantPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        // All units are important
        foreach (GameObject unit in importantObjects)
        {
            positions.Add(unit.transform.position);
        }

        if (Physics.Raycast(Camera.main.ScreenPointToRay(mouseManager.GetMousePosition()), out RaycastHit hit, 999f, layerMask)
            && includeMouse)
        {
            Vector3 mousePos = hit.point;
            if (!positions.Contains(mousePos))
            {
                positions.Add(mousePos);
            }
        }

        return positions;
    }

    private List<Cullable> FindOccludingObjects(List<Vector3> importantPositions)
    {
        List<Cullable> occludingObjects = new List<Cullable>();

        Camera camera = Camera.main;

        // We want to do a capsule check from each position to the camera, any cullable object we hit should be culled
        foreach (Vector3 pos in importantPositions)
        {
            Vector3 capsuleStart = (pos);
            capsuleStart.y += occlusionCapsuleHeight;

            Collider[] colliders = Physics.OverlapCapsule(capsuleStart, camera.transform.position, occlusionCapsuleRadius, layerMask, QueryTriggerInteraction.Ignore);

            // Add cullable objects we found to the list
            foreach (Collider collider in colliders)
            {
                Cullable cullable = collider.GetComponent<Cullable>();
                Debug.Log("Found an object on the occlusion layer without the occlusion component!");

                if (!occludingObjects.Contains(cullable))
                {
                    occludingObjects.Add(cullable);
                }
            }
        }
        return occludingObjects;
    }

    // Update the stored list of occluding objects
    private void SetOccludingObjects(List<Cullable> newList)
    {
        foreach (Cullable cullable in newList)
        {
            int foundIndex = occludingObjects.IndexOf(cullable);

            if (foundIndex < 0)
            {
                // This object isnt in the old list, so we need to mark it as occluding
                try
                {
                    cullable.Occluding = true;
                }
                catch
                {
                    Debug.Log("Cs 104 cullingManager script exception");
                }
            }
            else
            {
                // This object was already in the list, so remove it from the old list
                occludingObjects.RemoveAt(foundIndex);
            }
        }

        // Any object left in the old list, was not in the new list, so it's no longer occludding
        foreach (Cullable cullable in occludingObjects)
        {
            cullable.Occluding = false;
        }

        occludingObjects = newList;
    }
}
