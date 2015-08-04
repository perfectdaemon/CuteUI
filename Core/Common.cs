using System.Collections.Generic;
using System.Drawing;
using System;

namespace CuteUI
{
    /// <summary>
    /// SelectionChangedEventArgs — аргументы для события изменения выбранных строк
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Экземпляр, представляющий собой пустые аргументы события
        /// </summary>
        public static new readonly SelectionChangedEventArgs Empty = new SelectionChangedEventArgs(null);
        
        /// <summary>
        /// Строка, которая была выделена, либо с которой снято выделение.
        /// Ее новое состояние определяется по свойству ChangedRow.Selected
        /// </summary>
        public Row ChangedRow;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="row">Строка, которая изменила выделение</param>
        public SelectionChangedEventArgs(Row row)
        {
            ChangedRow = row;
        }
    }

    /// <summary>
    /// HorAlign определяет горизонтальное выравнивание элемента
    /// </summary>
    public enum HorAlign 
    { 
        /// <summary>
        /// Выравнивание по левому краю
        /// </summary>
        Left, 
        /// <summary>
        /// Выравнивание по центру
        /// </summary>
        Middle, 
        /// <summary>
        /// Выравнивание по правому краю
        /// </summary>
        Right 
    }

    /// <summary>
    /// VerAlign определяет вертикальное выравнивание элемента
    /// </summary>
    public enum VerAlign 
    { 
        /// <summary>
        /// Выравнивание по верху
        /// </summary>
        Top, 
        /// <summary>
        /// Выравнивание по центру
        /// </summary>
        Middle, 
        /// <summary>
        /// Выравнивание по низу
        /// </summary>
        Bottom 
    }

    /// <summary>
    /// Utility — внутренний вспомогательный класс
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Предоставляет mapping между VerAlign и StringAlignment для текста
        /// </summary>
        public static readonly Dictionary<VerAlign, StringAlignment> VerMapping = new Dictionary<VerAlign, StringAlignment>();
        
        /// <summary>
        /// Предоставляет mapping между HorAlign и StringAlignment для текста
        /// </summary>
        public static readonly Dictionary<HorAlign, StringAlignment> HorMapping = new Dictionary<HorAlign, StringAlignment>();

        static Utility()
        {
            VerMapping.Add(VerAlign.Top, StringAlignment.Near);
            VerMapping.Add(VerAlign.Middle, StringAlignment.Center);
            VerMapping.Add(VerAlign.Bottom, StringAlignment.Far);
            HorMapping.Add(HorAlign.Left, StringAlignment.Near);
            HorMapping.Add(HorAlign.Middle, StringAlignment.Center);
            HorMapping.Add(HorAlign.Right, StringAlignment.Far);
        }
    }
}
