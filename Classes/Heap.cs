using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    struct ThingsData
    {
        public string name, person;
        public int number;
        public float price;
        public DateTime date;

        public ThingsData(string name = "", string person = "", int number = 0, float price = 0, DateTime date = default)
        {
            this.name = name;
            this.person = person;
            this.number = number;
            this.price = price;
            this.date = date;
        }

        // Перевантаження оператора менше
        public static bool operator <(ThingsData left, ThingsData right)
        {
            if (left.number * left.price < right.number * right.price) return true;
            if (left.number * left.price == right.number * right.price)
            {
                if (left.price < right.price) return true;
            }
            if (left.price == right.price)
            {
                if (left.date < right.date) return true;
            }
            return false;
        }

        // Перевантаження оператора більше
        public static bool operator >(ThingsData left, ThingsData right)
        {
            if (left.number * left.price > right.number * right.price) return true;
            if (left.number * left.price == right.number * right.price)
            {
                if (left.price > right.price) return true;
            }
            if (left.price == right.price)
            {
                if (left.date > right.date) return true;
            }
            return false;
        }
    }
    internal class Heap
    {
        private ThingsData[] heap;
        private int size;

        // Конструктор для створення пустої купи відповідної довжини
        public Heap(int capacity)
        {
            heap = new ThingsData[capacity];
            size = 0;
        }

        // Конструктор для створення купи з масиву
        public Heap(ThingsData[] arr)
        {
            heap = new ThingsData[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                Insert(elem: arr[i]);
            size = arr.Length;
        }

        // Перевірка купи на відсутніть елементів
        public bool IsEmpty()
        {
            return size == 0;
        }

        // Геттер, повертає розмір купи
        public int GetSize()
        {
            return size;
        }

        // Метод міняє місцями елементи, приймає їх індекси
        private void Swap(int toPut, int where)
        {
            (heap[toPut], heap[where]) = (heap[where], heap[toPut]);
        }

        // Метод піднімає доданий елемент, щоб виконувалась властивість купи
        private void Up(int index)
        {
            while (index != 0 && heap[index] < heap[(index - 1) / 2])
            {
                Swap(index, (index - 1) / 2);
                index = (index - 1) / 2;
            }
        }

        // Метод опускає перший елемент, щоб виконувалась властивість купи
        private void Down(int index)
        {
            while (index * 2 + 1 < size)
            {
                int minChild = index * 2 + 1;
                if (minChild + 1 < size && heap[minChild] > heap[minChild + 1]) minChild++;

                if (heap[index] < heap[minChild]) break;

                Swap(index, minChild);
                index = minChild;
            }
        }

        // Метод додає елемент до купи
        public void Insert(string name = "", string person = "", int number = 0,
            float price = 0, DateTime date = default, ThingsData elem = default)
        {
            if (size == heap.Length) return;
            heap[size] = elem.Equals(default(ThingsData)) ? new ThingsData(name, person, number, price, date) : elem;
            Up(size);
            size++;
        }

        // Метод видаляє елемент з купи та повертає його значення
        public ThingsData ExstractMinimum()
        {
            ThingsData temp = heap[0];
            heap[0] = new ThingsData();
            Swap(0, size - 1);
            size--;
            Down(0);
            return temp;
        }

        // Метод бульбашкового сортування 
        public void BubbleSort()
        {
            for (int i = 0; i < size - 1; i++)
            {
                if (heap[i + 1] < heap[i])
                {
                    Swap(i, i + 1);
                    i = -1;
                }
            }
        }

        // Метод пірамідального сортування
        public void HeapSort()
        {
            ThingsData[] sortedHeap = new ThingsData[heap.Length];
            int originalSize = this.size;
            for (int i = 0; i < originalSize; i++)
            {
                sortedHeap[i] = ExstractMinimum();
            }
            heap = sortedHeap;
            size = originalSize;
        }

        // Метод виведення купи, з параметром виводиться від більшого до меншого
        public string PrintHeap(bool fromLeastToMost = true)
        {
            string result = "";
            for (int i = 0, j = size - 1; i < size; i++, j--)
            {
                if (fromLeastToMost)
                {
                    result += $"{i + 1}. {heap[i].name} - {heap[i].person}\n" +
                    $"Загальна сума: {heap[i].number * heap[i].price} = " +
                    $"{heap[i].number} штук по {heap[i].price} | {heap[i].date:dd.MM.yyyy}\n";
                }
                else
                {
                    result += $"{i + 1}. {heap[j].name} - {heap[j].person}\n" +
                    $"Загальна сума: {heap[j].number * heap[j].price} = " +
                    $"{heap[j].number} штук по {heap[j].price} | {heap[j].date:dd.MM.yyyy}\n";
                }
            }
            return result;
        }
    }
}
