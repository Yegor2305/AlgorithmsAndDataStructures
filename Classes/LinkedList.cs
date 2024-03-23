using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmsAndDataStructures.Classes
{
    internal class LinkedList
    {
        private class Node(int data = 0, Node? next = null, Node? previous = null)
        {
            private readonly int data = data;
            public Node? next = next, previous = previous;

            public int GetData() { return data; }
        }

        private Node head, last;
        private int lenght;
        public LinkedList()
        {
            head = new Node();
            head.next = last;
            head.previous = last;
            last = head;
            lenght = 0;
        }

        // Метод додавання елементу, без другого параметра додається на початок, з другим після заданого елементу
        public void Add(int data, int afterWhat = 0)
        {
            if (afterWhat < 0 || afterWhat > lenght) { return; }

            if (lenght == 0)
            {
                head = new Node(data);
                last = head;
            }
            else if (afterWhat > 0 && afterWhat <= lenght)
            {
                Node current = head;
                for (int i = 0; i < afterWhat - 1; i++)
                {
                    current = current.next!;
                }
                Node newNode = new(data, current.next, current);
                current.next!.previous = newNode;
                current.next = newNode;
            }
            else if (afterWhat <= lenght)
            {
                Node newNode = new(data, head, last);
                head.previous = newNode;
                head = newNode;
                last.next = head;
            }
            lenght++;
        }

        // Метод видалення заданого елементу
        public void Remove(int nodeNumber)
        {
            if (head is null) return;
            if (nodeNumber < 1 || nodeNumber > lenght) { return; }
            if (nodeNumber == 1)
            {
                head = head.next!;
                head.previous = last;
                last.next = head;
                lenght--;
                return;
            }
            if (nodeNumber == lenght)
            {
                last = last.previous!;
                last.next = head;
                head.previous = last;
                lenght--;
                return;
            }

            Node? current = head;

            for (int i = 1; i < lenght; i++)
            {
                if (i == nodeNumber) break;
                current = current!.next;
            }

            current!.previous!.next = current.next;
            current!.next!.previous = current.previous;
            current.next = null;
            current.previous = null;
            lenght--;

        }

        // Пошук елемента по значенню, повертає його порядковий номер
        /// <returns>Returns the number of the first node with the given data. If no node found returns -1</returns>
        public int FindNodeByData(int data)
        {
            Node current = head;
            int number = -1;
            for (int i = 0; i < lenght; i++)
            {
                if (current.GetData() == data)
                {
                    number = i + 1;
                    break;
                }
                current = current.next!;
            }
            return number;
        }

        // Виведення списку, параметр "З першого по останній" відповідно до назви виводить список,
        // якщо false, виводить навпаки
        public string GetList(bool fromfirstToLast = true)
        {
            string result = "";
            Node current = fromfirstToLast ? head : head.previous!;
            for (int i = 0; i < lenght; i++)
            {
                result += current.GetData();
                if (i < lenght - 1) result += " <-> ";
                current = fromfirstToLast ? current.next! : current.previous!;
            }
            return result;
        }

        // Геттер, повертає довжину списку
        public int GetLenght()
        {
            return lenght;
        }
    }
}
