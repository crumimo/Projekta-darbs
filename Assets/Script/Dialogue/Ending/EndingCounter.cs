using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCounter : MonoBehaviour
{
   public static int ingredientsCollected;

   public void AddIngredient()
   {
      ingredientsCollected += 1;
   }
}
