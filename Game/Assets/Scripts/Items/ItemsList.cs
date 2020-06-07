using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsList", menuName = "Items/ItemsList")]
public class ItemsList : ScriptableObject, IEnumerable<Item>
{

    [SerializeField, HideInInspector]
    private List<Item> items;

    [SerializeField]
    private Item currentItem;

    private int currentIndex = 0;

    public void AddElement()
    {

        if (items == null)
        {

            items = new List<Item>();
            items.Add(new Item());

        }

        currentItem = new Item();
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
        currentItem = new Item();
        currentIndex = 0;
        items.Add(currentItem);

    }

    public Item GetItem(byte ID)
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

    public IEnumerator<Item> GetEnumerator()
    {

        return ((IEnumerable<Item>)items).GetEnumerator();

    }

    IEnumerator IEnumerable.GetEnumerator()
    {

        return ((IEnumerable<Item>)items).GetEnumerator();

    }

    public Item this[int index]
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

                items = new List<Item>();

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
public class Item
{

    [SerializeField]
    private byte id;

    [SerializeField]
    private string Name;

    [SerializeField]
    private Sprite icon = null;

    public enum ItemType
    {
        Item,
        Axe,
        Pickaxe,
        Shovel
    }

    [SerializeField]
    private ItemType type = ItemType.Item;

    [SerializeField]
    private float power;

    public byte ID { get => id; }
    public Sprite Icon { get => icon; }
    public ItemType Type { get => type; }
    public float Power { get => power; }

}