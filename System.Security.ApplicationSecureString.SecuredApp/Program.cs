using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Security;
using System.Text;

class Program {
    static void Main(string[] args) {
        var delineator = System.Environment.NewLine;
        var headPointer = -1;
        var tailPointer = 3;

        var delineatorByteArray = Encoding.Default.GetBytes(delineator);
        var targetByteArray = File.ReadAllBytes("Program.TextFile.Secured.txt");
        GC.Collect();
        GC.WaitForPendingFinalizers();

        var prviouseGCLatencyMode = GCSettings.LatencyMode;
        try {
            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            do {
                headPointer = targetByteArray.MorrisPrattSearchFirst(delineatorByteArray, tailPointer);
                if (headPointer >= 0) {
                    var length = headPointer - tailPointer;
                    var buffer = new Byte[length];
                    Array.Copy(targetByteArray, tailPointer, buffer, 0, length);
                    var characters = Encoding.UTF8.GetChars(buffer);
                    var applicationSecureString = new ApplicationSecureString(characters);
                    Array.Clear(characters, 0, characters.Length);
                    Array.Clear(buffer, 0, buffer.Length);

                    var tempString = String.Empty;
                    using (var stringDisposable = applicationSecureString.CreateStringDisposable()) {
                        tempString = stringDisposable.UnsecuredString;
                    }

                    tailPointer = (headPointer + delineatorByteArray.Length);
                }
            } while (headPointer >= 0 && tailPointer < targetByteArray.Length);

            Array.Clear(targetByteArray, 0, targetByteArray.Length);
            Array.Clear(delineatorByteArray, 0, delineatorByteArray.Length);
        } finally {
            GCSettings.LatencyMode = prviouseGCLatencyMode;
        }
        Console.WriteLine("Enter any letter to exit");
        Console.ReadKey();
    }
}

/// <summary>
/// Provides a set of extention methods to search or split a collection.
/// </summary>
public static class IEnumerableExtentions {
    ///// <summary>
    ///// Reports the index of the first occurrence of the specified sub-array in this instance, using Morris-Pratt algorithm.
    ///// </summary>
    ///// <typeparam name="T">The type of two arrays.</typeparam>
    ///// <param name="t">The longer array.</param>
    ///// <param name="p">The shorter array to seek.</param>
    ///// <returns>The zero-based index position of value if <paramref name="p"/> is found, or -1 if it is not.</returns>
    ///// <exception cref="ArgumentNullException"><paramref name="t"/> or <paramref name="p"/> is null.</exception>
    //public static int MorrisPrattSearchFirst<T>(this T[] t, T[] p)
    //    where T : IEquatable<T> {
    //    return MorrisPrattSearchFirst(t, p, 0);
    //}

    /// <summary>
    /// Reports the index of the first occurrence of the specified sub-array in this instance, using Morris-Pratt algorithm.
    /// </summary>
    /// <typeparam name="T">The type of two arrays.</typeparam>
    /// <param name="t">The longer array.</param>
    /// <param name="p">The shorter array to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <returns>The zero-based index position of value if <paramref name="p"/> is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="t"/> or <paramref name="p"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="startIndex"/> is greater than the length of <paramref name="t"/> array.</para>
    /// <para>-- Or --</para>
    /// <para><paramref name="startIndex"/> is negative.</para> 
    /// </exception>
    public static int MorrisPrattSearchFirst<T>(this T[] t, T[] p, int startIndex)
        where T : IEquatable<T> {
        return MorrisPrattSearchFirst(t, p, startIndex, t.Length - startIndex);
    }

    /// <summary>
    /// Reports the index of the first occurrence of the specified sub-array in this instance, using Morris-Pratt algorithm.
    /// </summary>
    /// <typeparam name="T">The type of two arrays.</typeparam>
    /// <param name="t">The longer array.</param>
    /// <param name="p">The shorter array to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <returns>The zero-based index position of value if <paramref name="p"/> is found, or -1 if it is not.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="t"/> or <paramref name="p"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="startIndex"/> is greater than the length of <paramref name="t"/> array.</para>
    /// <para>-- Or --</para>
    /// <para><paramref name="startIndex"/> or <paramref name="count"/> is negative.</para>
    /// <para>-- Or --</para>
    /// <para><paramref name="count"/> is greater than the length of <paramref name="t"/> array minus <paramref name="startIndex"/>.</para>
    /// </exception>
    public static int MorrisPrattSearchFirst<T>(this T[] t, T[] p, int startIndex, int count)
        where T : IEquatable<T> {
        if (t == null)
            throw new ArgumentNullException("t");

        if (p == null)
            throw new ArgumentNullException("p");

        int tLength = t.Length, pLength = p.Length, rLength = startIndex + count;

        if (startIndex >= tLength)
            throw new ArgumentOutOfRangeException("Value is greater than the length of this array.", "startIndex");

        if (startIndex < 0)
            throw new ArgumentOutOfRangeException("Value is negative.", "startIndex");

        if (rLength > tLength)
            throw new ArgumentOutOfRangeException("Value is greater than the length of this array minus startIndex.", "count");

        if (count < 0)
            throw new ArgumentOutOfRangeException("Value is negative.", "count");

        if (p.Length > t.Length)
            return -1;

        int[] failure = new int[100];
        try {
            for (int i = 1, j = failure[0] = -1; i < pLength; i++) {
                while (j >= 0 && !p[j + 1].Equals(p[i]))
                    j = failure[j];

                if (p[j + 1].Equals(p[i]))
                    j++;

                failure[i] = j;
            }

            for (int i = startIndex, j = -1; i < rLength; i++) {
                while (j >= 0 && !p[j + 1].Equals(t[i]))
                    j = failure[j];

                if (p[j + 1].Equals(t[i]))
                    j++;

                if (j == p.Length - 1)
                    return i - p.Length + 1;
            }

            return -1;
        } finally {
            Array.Clear(failure, 0, failure.Length);
        }
    }

    public static int MorrisPrattSearchFirst<T>(this IEnumerable<T> t, IEnumerable<T> p, int startIndex, int count)
        where T : IEquatable<T> {
        if (t == null)
            throw new ArgumentNullException("t");

        if (p == null)
            throw new ArgumentNullException("p");

        int tLength = t.Count(), pLength = p.Count(), rLength = startIndex + count;

        if (startIndex >= tLength)
            throw new ArgumentOutOfRangeException("Value is greater than the length of this array.", "startIndex");

        if (startIndex < 0)
            throw new ArgumentOutOfRangeException("Value is negative.", "startIndex");

        if (rLength > tLength)
            throw new ArgumentOutOfRangeException("Value is greater than the length of this array minus startIndex.", "count");

        if (count < 0)
            throw new ArgumentOutOfRangeException("Value is negative.", "count");

        if (pLength > tLength)
            return -1;

        int[] failure = new int[100];
        try {
            for (int i = 1, j = failure[0] = -1; i < pLength; i++) {
                while (j >= 0 && !p.ElementAt(j + 1).Equals(p.ElementAt(i)))
                    j = failure[j];

                if (p.ElementAt(j + 1).Equals(p.ElementAt(i)))
                    j++;

                failure[i] = j;
            }

            for (int i = startIndex, j = -1; i < rLength; i++) {
                while (j >= 0 && !p.ElementAt(j + 1).Equals(t.ElementAt(i)))
                    j = failure[j];

                if (p.ElementAt(j + 1).Equals(t.ElementAt(i)))
                    j++;

                if (j == pLength - 1)
                    return i - pLength + 1;
            }

            return -1;
        } finally {
            Array.Clear(failure, 0, failure.Length);
        }
    }
}
