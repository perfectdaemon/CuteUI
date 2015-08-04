using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System;

namespace CuteUI
{
    /// <summary>
    /// Класс строки для CuteList
    /// </summary>
    public class Row
    {
        private TemplateRow defaultTemplate = null, selectedTemplate = null;
        private bool selected = false, visible = true;

        private ArrayList rowData = new ArrayList();
        private StringFormat stringFormat = new StringFormat();
        private Rectangle rect, borderRect;
        private TemplateRow currentTemplate = null;
        private Pen borderPen = new Pen(Color.Teal, 1);        
        
        /// <summary>
        /// Хранит координату начала строки по отношению к родительскому контейнеру.
        /// Используется для определении строки при клике по контейнеру
        /// Обновляется в методе Draw, исходя из параметра clipRect
        /// </summary>
        internal int YposAtContainer;

        /// <summary>
        /// Обновляет ссылку на текущий шаблон в зависимости от того, 
        /// выбрана ли строка и есть ли шаблон для выбранной строки
        /// </summary>
        private void UpdateCurrentTemplate()
        {
            currentTemplate = selected && selectedTemplate != null ? selectedTemplate : defaultTemplate;
        }

        /// <summary>
        /// Рисует строку с помощью объекта Graphics в указанном ограничивающем прямоугольнике
        /// </summary>
        /// <param name="graphics">Объект Graphics, с помощью которого строка рисует себя</param>
        /// <param name="clipRect">Ограничиивающий прямоугольник</param>
        internal void Draw(Graphics graphics, Rectangle clipRect)
        {
            if (!Visible || currentTemplate == null)
                return;

            // Запоминаем свою позицию в контейнере по оси Y, это пригодится, когда мы будем определять какая строка кликнута
            YposAtContainer = clipRect.Y;

            // Заливаем фон, учитывая высоту строки (clipRect может содержать бОльшую высоту, чем необходимо)
            rect = new Rectangle(clipRect.X, clipRect.Y, clipRect.Width, this.Height);
            graphics.DrawGradient(currentTemplate.BackGradient, rect);

            // Рисуем ячейки            
            for (int i = 0; i < currentTemplate.TemplateCells.Count; ++i)
            {
                var cell = currentTemplate.TemplateCells[i];

                // Если DataIndex == -1, то данные вставляются согласно позиции ячейки в коллекции
                // Если DataIndex == -2, то вместо данных показывается константа - поле Text ячейки
                // Во всех остальных случаях, просто получаем данные по индексу. 
                // Если индекс за пределами диапазона, то будет возвращен null                
                object data = null;
                switch (cell.DataIndex)
                {
                    case -1: data = this[i]; break;
                    case -2: data = cell.Text; break;
                    default: data = this[cell.DataIndex]; break;
                }                
                // Рисуем ячейку с данными
                cell.Draw(graphics, rect, data);
            }

            // Рисуем границу, если необходимо
            if (currentTemplate.BorderSize > 0 && currentTemplate.BorderColor != Color.Transparent)
            {
                borderPen.Color = currentTemplate.BorderColor;
                borderPen.Width = currentTemplate.BorderSize;
                borderRect = new Rectangle(
                    currentTemplate.BorderSize / 2, currentTemplate.BorderSize / 2,
                    rect.Width - currentTemplate.BorderSize, rect.Height - currentTemplate.BorderSize);
                // Смещаемся на координаты ограничивающего прямоугольника
                borderRect.Offset(clipRect.X, clipRect.Y);

                graphics.DrawRectangle(borderPen, borderRect);
            }
        }

        /// <summary>
        /// Инициализирует новый экземляр класса строки
        /// </summary>
        public Row()
        {

        }

        /// <summary>
        /// Инициализирует новый экземпляр класса строки и задает шаблоны
        /// </summary>
        /// <param name="defaultTemplate">Стандартный шаблон</param>
        /// <param name="selectedTemplate">Шаблон, когда строка выбрана</param>
        public Row(TemplateRow defaultTemplate, TemplateRow selectedTemplate)
        {
            DefaultTemplate = defaultTemplate;
            SelectedTemplate = selectedTemplate;
        }

        /// <summary>
        /// Стандартный шаблон для строки
        /// </summary>
        public TemplateRow DefaultTemplate
        {
            get { return defaultTemplate; }
            set
            {
                if (defaultTemplate != value)
                {
                    defaultTemplate = value;
                    UpdateCurrentTemplate();
                }
            }
        }

        /// <summary>
        /// Шаблон, когда строка выбрана
        /// </summary>
        public TemplateRow SelectedTemplate
        {
            get { return selectedTemplate; }
            set
            {
                if (selectedTemplate != value)
                {
                    selectedTemplate = value;
                    UpdateCurrentTemplate();
                }
            }
        }

        /// <summary>
        /// Загружает данные в строку
        /// </summary>
        /// <param name="data">Данные</param>
        public void SetRowData(params object[] data)
        {
            rowData.Clear();
            rowData.AddRange(data);
        }

        /// <summary>
        /// Возвращает объект с данными из ячейки. Не саму ячейку
        /// </summary>
        /// <param name="index">Индекс ячейки</param>
        /// <returns>Объект данных</returns>
        /// <exception cref="IndexOutOfRangeException">При индексе за пределами диапазона</exception>
        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < rowData.Count)
                    return rowData[index];
                else
                    return null;
            }
            set
            {
                rowData[index] = value;
            }
        }

        /// <summary>
        /// Свойство для установки данных в Design-time
        /// </summary>
        public string[] DesignTimeData 
        {
            get { return (string[])rowData.ToArray(typeof(string)); }
            set
            {
                SetRowData(value);
            }
        }

        /// <summary>
        /// Выбрана ли данная строчка. Доступно только для чтения
        /// </summary>          
        [DefaultValue(false)]
        public bool Selected
        {
            get { return selected; }
            internal set
            {
                if (value != selected)
                {
                    selected = value;
                    UpdateCurrentTemplate();                    
                }
            }
        }

        /// <summary>
        /// Определяет видимость строки
        /// </summary>
        [DefaultValue(true)]
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                }
            }
        }

        /// <summary>
        /// Текущая высота строки. Доступна только для чтения.
        /// </summary>
        public int Height
        {
            get
            {
                return currentTemplate != null ? currentTemplate.Height : 0;
            }
        }

        /// <summary>
        /// Вспомогательный метод для сериализатора конструктора (Designer Serializer)
        /// </summary>
        /// <returns>False, если строка видна (сериализация не нужна, так как это значение по умолчанию)</returns>
        public bool ShouldSerializeVisible()
        {
            return !Visible;
        }

        /// <summary>
        /// Вспомогательный метод для сериализатора конструктора (Designer Serializer)
        /// </summary>
        /// <returns>False, если строка не выбрана (сериализация не нужна, так как это значение по умолчанию)</returns>
        public bool ShouldSerializeSelected()
        {
            return Selected;
        }
    }
}
