using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    internal class HashTable
    {
        const ulong baseOffset = 14695981039346656037;
        const ulong prime = 1099511628211;
        private LinkedList<KeyValuePair<string, int>>[] table;
        private int count;
        private string mostFrequentWord;
        private int mostFrequentWordCount;

        public HashTable(int size)
        {
            table = new LinkedList<KeyValuePair<string, int>>[size];
            mostFrequentWord = string.Empty;
            mostFrequentWordCount = 1;
            count = 0;
        }

        private static int GetElementIndex(string key, ulong tableSize)
        {
            ulong hash = baseOffset;
            foreach (byte b in Encoding.UTF8.GetBytes(key))
            {
                hash ^= b;
                hash *= prime;
            }

            return (int)(hash % tableSize);
        }

        public void Add(string element)
        {
            if ((float)count / table.Length > 0.75) Rehash();
            int index = GetElementIndex(element, (ulong)table.Length);
            if (table[index] == null)
            {
                table[index] = new LinkedList<KeyValuePair<string, int>>();
                table[index].AddLast(new KeyValuePair<string, int>(element, 1));
            }
            else
            {
                foreach (var pair in table[index])
                {
                    if (pair.Key == element)
                    {
                        int newCount = pair.Value + 1;
                        KeyValuePair<string, int> newPair = new(element, newCount);
                        table[index].Remove(pair);
                        table[index].AddLast(newPair);
                        if (newCount > mostFrequentWordCount)
                        {
                            mostFrequentWordCount = newCount;
                            mostFrequentWord = element;
                        }
                        return;
                    }
                }
                table[index].AddLast(new KeyValuePair<string, int>(element, 1));
            }
            count++;
        }

        public void Remove(string element)
        {
            int index = GetElementIndex(element, (ulong)table.Length);
            if (table[index] == null) return;

            foreach (var pair in table[index])
            {
                if (pair.Key == element)
                {
                    if (pair.Value > 1)
                        table[index].AddLast(new KeyValuePair<string, int>(pair.Key, pair.Value - 1));
                    table[index].Remove(pair);
                    RefindMostFrequentWord();
                    return;
                }
            }
        }

        public void Rehash(int size = 0)
        {
            LinkedList<KeyValuePair<string, int>>[] newTable = new LinkedList<KeyValuePair<string, int>>[table.Length * 2];
            if (size != 0 && size > 0)
            {
                newTable = new LinkedList<KeyValuePair<string, int>>[size];
            }
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] == null) continue;
                foreach (var pair in table[i])
                {
                    int index = GetElementIndex(pair.Key, (ulong)newTable.Length);
                    if (newTable[index] == null)
                        newTable[index] = new();

                    newTable[index].AddLast(pair);
                }
            }
            table = newTable;
        }

        private void RefindMostFrequentWord()
        {
            mostFrequentWordCount = 1;
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] == null) continue;
                foreach (var pair in table[i])
                {
                    if (pair.Value >= mostFrequentWordCount)
                    {
                        mostFrequentWord = pair.Key;
                        mostFrequentWordCount = pair.Value;
                    }
                }
            }
            if (mostFrequentWordCount == 1) mostFrequentWord = string.Empty;
        }

        public string GetNumberOfRepetitions(string word)
        {
            int index = GetElementIndex(word, (ulong)table.Length);
            if (table[index] == null) return $"Word '{word}' not founded!";
            foreach (var pair in table[index])
            {
                if (pair.Key == word) return $"Word '{word}' appears {pair.Value} times";
            }
            return $"Word '{word}' not founded!";
        }

        public string GetMostFrequentWord()
        {
            if (mostFrequentWord == string.Empty)
                return "No most frequent word";
            else
                return $"Most frequent word is '{mostFrequentWord}', appears {mostFrequentWordCount} times";
        }

        public string GetHashTableContent()
        {
            string result = "";
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] == null) continue;
                foreach (var pair in table[i])
                {
                    result += $"{pair.Key}: {pair.Value}\n";
                }

            }
            return result;
        }
    }
}
