using System;
using System.Collections;

namespace CuteUI
{
    /// <summary>
    /// ListChangedEventArgs — класс аргументов события изменения коллекции
    /// </summary>
    public class ListChangedEventArgs : EventArgs
    {
        /// <summary>
        /// ListAction — перечисление возможных событий, совершенных в коллекции
        /// </summary>
        public enum ListAction 
        { 
            /// <summary>
            /// Действие не определено
            /// </summary>
            Undefined, 
            /// <summary>
            /// Добавление элемента в коллекцию
            /// </summary>
            Add, 
            /// <summary>
            /// Обновление элемента в коллекции
            /// </summary>
            Update, 
            /// <summary>
            /// Удаление элемента из коллекции
            /// </summary>
            Remove, 
            /// <summary>
            /// Очистка коллекции
            /// </summary>
            Clear 
        }

        /// <summary>
        /// Объект класса, представляющий пустые параметры события
        /// </summary>
        public static ListChangedEventArgs ListEmpty = new ListChangedEventArgs(null, ListAction.Undefined);

        /// <summary>
        /// Элемент коллекции, явившийся причиной события
        /// </summary>
        public object ListItem = null;

        /// <summary>
        /// Определяет совершенное действие
        /// </summary>
        public ListAction Action;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="listItem">Элемент, явившийся причиной события</param>
        /// <param name="action">Совершенное действие</param>
        public ListChangedEventArgs(object listItem, ListAction action)
        {
            this.ListItem = listItem;
            this.Action = action;
        }
    }

    /// <summary>
    /// GenericCollection — базовый класс для коллекции, который позволяет отслеживать изменение этой коллекции через подписку на событие
    /// </summary>
    /// <typeparam name="T">Тип элемента коллекции</typeparam>
    public class GenericCollection<T> : CollectionBase
    {
        /// <summary>
        /// Вызывается после очищении коллекции
        /// </summary>
        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            ListChanged(this, new ListChangedEventArgs(null, ListChangedEventArgs.ListAction.Clear));
        }

        /// <summary>
        /// Вызывается после вставки элемента в коллекцию
        /// </summary>
        /// <param name="index">Индекс нового элемента в коллекции</param>
        /// <param name="value">Новый элемент коллекции</param>
        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);
            ListChanged(this, new ListChangedEventArgs(value, ListChangedEventArgs.ListAction.Add));
        }

        /// <summary>
        /// Вызывается после удаления элемента из коллекции
        /// </summary>
        /// <param name="index">Индекс удаленного элемента</param>
        /// <param name="value">Удаленный элемент</param>
        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);
            ListChanged(this, new ListChangedEventArgs(value, ListChangedEventArgs.ListAction.Remove));
        }

        /// <summary>
        /// Вызывается после установки нового значения по индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="oldValue">Старый элемент коллекции</param>
        /// <param name="newValue">Новый элемент коллекции</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            base.OnSetComplete(index, oldValue, newValue);
            ListChanged(this, new ListChangedEventArgs(newValue, ListChangedEventArgs.ListAction.Update));
        }

        /// <summary>
        /// Пустой делегат для события ListChanged
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e">Параметры события</param>
        /// <remarks>Используется для threadsafe-вызова события</remarks>
        private static void EmptyOnListChanged(object sender, ListChangedEventArgs e)
        {
        }

        /// <summary>
        /// Событие изменения коллекции — добавление, удаления, очистки, изменения элемента
        /// </summary>
        public event EventHandler<ListChangedEventArgs> ListChanged;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public GenericCollection()
        {
            ListChanged += EmptyOnListChanged;
        }

        /// <summary>
        /// Возвращает или задает элемент по индексу
        /// </summary>
        /// <param name="index">Индекс элемента в коллекции</param>
        /// <returns>Элемент коллекции</returns>        
        public T this[int index]
        {
            get
            {
                return ((T)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Добавляет элемент в коллекцию
        /// </summary>
        /// <param name="value">Элемент</param>
        /// <returns>Индекс элемента в коллекции</returns>
        public int Add(T value)
        {
            return (List.Add(value));
        }

        /// <summary>
        /// Возвращает индекс элемента в коллекции
        /// </summary>
        /// <param name="value">Элемент</param>
        /// <returns>Индекс элемента в коллекции, если он найден, иначе -1</returns>
        public int IndexOf(T value)
        {
            return (List.IndexOf(value));
        }

        /// <summary>
        /// Добавляет элемент в коллекцию на заданную позицию
        /// </summary>
        /// <param name="index">Позиция добавляемого элемента</param>
        /// <param name="value">Элемент</param>
        public void Insert(int index, T value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Удаляет элемент из коллекции
        /// </summary>
        /// <param name="value">Элемент</param>
        public void Remove(T value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Проверяет, содержит ли коллекция данный элемент
        /// </summary>
        /// <param name="value">Элемент</param>
        /// <returns>True, если элемент присутствует в коллекции, иначе - false</returns>
        public bool Contains(T value)
        {
            return (List.Contains(value));
        }

        /// <summary>
        /// Проверяет, входит ли данный индекс в диапазон коллекции
        /// </summary>
        /// <param name="index">Индекс, который необходимо проверить</param>
        /// <returns>True, если индекс попадает в диапазон, иначе - false</returns>
        public bool IndexInRange(int index)
        {
            return (0 <= index && index < Count);
        }
    }
}
