using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsAttributes", menuName = "Items/ItemsAttributes")]
public class ItemsAttributes : ScriptableObject, IEnumerable<ItemType>
{

    [SerializeField, HideInInspector]
    private List<ItemType> items = null;

    [SerializeField]
    private ItemType currentItem = null;

    private int currentIndex = 0;

    public void AddElement()
    {

        if (items == null)
        {

            items = new List<ItemType>();
            items.Add(new ItemType());

        }

        currentItem = new ItemType();
        items.Add(currentItem);
        currentIndex = items.Count - 1;

    }

    public void RemoveCurrentElement()
    {

        if (items.Count > 1)
        {

            int nextIndex;

            if (currentIndex < items.Count - 1)
            {

                nextIndex = currentIndex;

            }
            else
            {

                nextIndex = currentIndex - 1;

            }

            Debug.Log(currentIndex + " " + nextIndex);
            
            items.RemoveAt(currentIndex);
            currentItem = items[nextIndex];

            currentIndex = nextIndex;

        }
        else
        {

            RemoveAll();

        }

    }

    public void GetNext()
    {

        if (currentIndex < items.Count - 1)
        {

            ++currentIndex;
            currentItem = this[currentIndex];

        }

    }

    public void GetPrev()
    {

        if (currentIndex > 0)
        {

            --currentIndex;
            currentItem = this[currentIndex];

        }

    }

    public void RemoveAll()
    {

        items.Clear();
        currentItem = new ItemType();
        currentIndex = 0;
        items.Add(currentItem);

    }

    public ItemType GetItem(byte ID)
    {

        foreach (var item in items)
        {

            if (ID == item.ID)
            {

                return item;

            }

        }

        return null;

    }

    public IEnumerator<ItemType> GetEnumerator()
    {

        return ((IEnumerable<ItemType>)items).GetEnumerator();

    }

    IEnumerator IEnumerable.GetEnumerator()
    {

        return ((IEnumerable<ItemType>)items).GetEnumerator();

    }

    public ItemType this[int index]
    {

        get
        {

            if (items != null && index >= 0 && index < items.Count)
            {

                return items[index];

            }

            return null;

        }

        set
        {

            if (items == null)
            {

                items = new List<ItemType>();

            }

            if (value != null)
            {

                if (index >= 0 && index < items.Count)
                {

                    items[index] = value;

                }
                else
                {

                    Debug.LogError("Index out of range!");

                }

            }
            else
            {

                Debug.LogError("Value == null!");

            }

        }

    }

}

[System.Serializable]
public class ItemType
{

    [SerializeField]
    private byte id = 0;

    [SerializeField]
    private string name = "";

    [SerializeField]
    private Sprite icon = null;

    public enum ItemClass
    {
        Item,
        Axe,
        Pickaxe,
        Shovel
    }

    [SerializeField]
    private ItemClass type = ItemClass.Item;

    [SerializeField]
    private float power = 0;

    public byte ID { get => id; }
    public string Name { get => name; }
    public Sprite Icon { get => icon; }
    public ItemClass Type { get => type; }
    public float Power { get => power; }
    
}