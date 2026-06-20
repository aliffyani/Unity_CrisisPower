using UnityEngine;

public class TrashItem : MonoBehaviour
{
    public TrashType type;
}

public enum TrashType
{
    Paper,
    Plastic,
    Glass
}