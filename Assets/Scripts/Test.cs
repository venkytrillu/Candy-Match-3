using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<int> Numbers =new List<int>(8);
    public List<int> NewNum1 = new List<int>();
    void Start()
    {
        Numbers.Add(0);
        Numbers.Add(5);
        Numbers.Add(3);
        Numbers.Add(4);
        Numbers.Add(1);
        Numbers.Add(2);
        Numbers.Add(8);
        Numbers.Add(7);
        for (int i = 0; i <Numbers.Count; i++)
        {
            if(Numbers[i]%2==0)
            {
                
                NewNum1.Add(Numbers[i]);
            }
        }
        NewNum1.Sort();
        List<int> num = new List<int>();
        for (int i = 0; i < Numbers.Count; i++)
        {
            if (Numbers[i] % 2 != 0)
            {

                num.Add(Numbers[i]);
            }
        }

        num.Sort();
        for (int i = 0; i < num.Count; i++)
        {


            NewNum1.Add(num[i]);
            
        }

    }


}
