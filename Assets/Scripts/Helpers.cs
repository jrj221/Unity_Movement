
using UnityEngine;

public static class Helpers // static means it can be accessed anywhere, but cannot change state or have instance properties, which is fine
{
    
    public static void CheckNull<T>(T variable, string variableName)
    {
        if (variable == null) {
            Debug.Log($"{variableName} is null");
        }
    }
}
