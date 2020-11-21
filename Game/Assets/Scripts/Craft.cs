using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft : MonoBehaviour
{

    [SerializeField]
    private UIItemSlot[] craftSlots = null;

    [SerializeField]
    private UIItemSlot result = null;

    [SerializeField]
    private UIController uicontroller = null;

    [SerializeField]
    private RecipesBook recipesBook = null;

    private void Update()
    {

        if (!uicontroller.IsInUI)
        {

            return;

        }

        foreach (var slot in craftSlots)
        {

            if (slot.IsClicked)
            {

                CheckForRecipe();

                slot.IsClicked = false;

            }

        }

        if (result.IsClicked)
        {

            foreach (var slot in craftSlots)
            {

                slot.Take(1);

            }

            result.IsClicked = false;

            CheckForRecipe();

        }

    }

    private void CheckForRecipe()
    {

        foreach (var recipe in recipesBook.Recipes)
        {

            if (CompareRecipe(recipe))
            {                

                result.PutStack(new ItemStack(recipe.Result, recipe.ResultAmount, 64));

                return;

            }

        }

        result.Clear();

    }

    private bool CompareRecipe(Recipe recipe)
    {

        if (recipe.IsFormless)
        {

            bool[] checkIngredients = new bool[craftSlots.Length];//Same ID can be used in ricepe multiple times

            for (int i = 0; i < recipe.Slots.Length; ++i)
            {

                bool isIngredientFound = false;

                for (int j = 0; j < craftSlots.Length; ++j)
                {

                    if (recipe.Slots[i] == craftSlots[j].ID && !checkIngredients[j])
                    {

                        isIngredientFound = true;
                        checkIngredients[j] = true;

                        break;

                    }

                }

                if (!isIngredientFound)
                {

                    return false;

                }

            }

            for (int i = 0; i < craftSlots.Length; ++i)
            {

                if (!checkIngredients[i] && craftSlots[i].ID != 0)
                {

                    return false;

                }

            }

            return true;

        }

        if (recipe.Slots.Length != craftSlots.Length)
        {

            return false;

        }

        for (int i = 0; i < recipe.Slots.Length; ++i)
        {

            if (recipe.Slots[i] != craftSlots[i].ID)
            {

                return false;

            }

        }

        return true;

    }

}
