using System.Collections;
using System.Collections.Generic;

public class SecureValue<T>
{
    public SecureValue(T initValue)
    {
        targetValue = initValue;

        UpdateHash();
    }

    public T targetValue;
    public int checksumHashA, checksumHashB;

    public void SetValue(T newValue)
    {
        if(!IsValidHash())
        {
            targetValue = (T)(object)0;
            return;
        }

        if(typeof(T) == typeof(int))
        {
            targetValue = (T)(object)AntiCheatManager.SecureInt((int)(object)newValue);
        }
        else
        {
            targetValue = newValue;
        }

         UpdateHash();   
    }

    public T GetValue()
    {
        if(!IsValidHash())
        {
            return (T)(object)0;
        }

        if(typeof(T) == typeof(int))
        {
            return (T)(object)AntiCheatManager.SecureInt((int)(object)targetValue);
        }
        
        return targetValue;
    }

    void UpdateHash()
    {
        checksumHashA = ((int)(object)targetValue-1).ToString().GetHashCode();
        checksumHashB = ((int)(object)targetValue+1).ToString().GetHashCode();
    }

    bool IsValidHash()
    {
        int compareHashA = ((int)(object)targetValue-1).ToString().GetHashCode();
        int compareHashB = ((int)(object)targetValue+1).ToString().GetHashCode();

        if(compareHashA == checksumHashA && compareHashB == checksumHashB)
        {
            return true;
        }

        return false;
    }
}
