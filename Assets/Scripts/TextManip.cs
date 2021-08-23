using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextManip
{
    public static string FilePathUp(string incoming)
    {
        string reversed = Reverse(incoming);
        string substring = reversed.Substring(reversed.IndexOf('/') + 1);
        string finished = Reverse(substring);
        return finished;
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
