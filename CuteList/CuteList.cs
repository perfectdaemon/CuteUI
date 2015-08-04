using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CuteUI
{
    /// <summary>
    /// CuteList — список с широкими возможностями по кастомизации и поддержкой сенсорной прокрутки
    /// </summary>
    public class CuteList : UserControl
    {
        private RowCollection rows = new RowCollection();
        private TemplateRowCollection templates = new TemplateRowCollection();
        private int separatorSize = 0;
        private bool multipleSelect = false;        
        private RowCollection selectedRows = new RowCollection();

        private Bitmap backBuffer;
        private SolidBrush backBrush = new SolidBrush(Color.Gray);
        private Rectangle rect;

        private Point touchStart = Point.Empty, clickStart = Point.Empty;
        private bool touched = false;
        private int scrollPosition = 0;
        private int scrollPositionMax = 0;

        /// <summary>
        /// Пустая подписка на событие изменения выбора, для threadsafe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmptySelectionChanged(object sender, SelectionChangedEventArgs e)
        { }

        /// <summary>
        /// Получает строку, которая находится по данной координате
        /// </summary>
        /// <param name="Y">Координата Y в базисе компонента</param>
        /// <returns>Строку, либо null, если кликнули в пустоту</returns>
        private Row GetRowAtPosition(int Y)
        {
            for (int i = 0; i < rows.Count; ++i)
            {
                if (Y > rows[i].YposAtContainer
                    && Y < rows[i].YposAtContainer + rows[i].Height
                    && rows[i].Visible)
                    return rows[i];
            }
            return null;
        }

        /// <summary>
        /// Вызывается при тапе на экран
        /// </summary>
        private void OnTouchClick()
        {
            var row = GetRowAtPosition(clickStart.Y);
            if (row != null)            
                SelectDeselectRow(row);                            
        }

        /// <summary>
        /// Пересчитывает максимальную позицию скролла
        /// </summary>
        private void RecalculateMaxScrollPosition()
        {
            int contentHeight = 0;
            foreach (Row row in rows)
                contentHeight += row.Height;

            contentHeight += separatorSize * (rows.Count - 1);
            if (Height > contentHeight)
                scrollPositionMax = 0;
            else
                scrollPositionMax = contentHeight - Height;
        }

        /// <summary>
        /// Подписка на событие изменения коллекции строк (Rows)
        /// </summary>
        /// <param name="sender">Коллекция Rows</param>
        /// <param name="e">Параметры события</param>
        private void OnRowListChange(object sender, ListChangedEventArgs e)
        {
            RecalculateMaxScrollPosition();
            if (e.Action == ListChangedEventArgs.ListAction.Add)
            {
                var row = e.ListItem as Row;
                if (templates.IndexInRange(TemplateIndexDefault))
                    row.DefaultTemplate = templates[TemplateIndexDefault];
                if (templates.IndexInRange(TemplateIndexSelected))
                    row.SelectedTemplate = templates[TemplateIndexSelected];                

                DrawToBackBuffer();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Подписка на событие изменения коллекции шаблонов строк
        /// </summary>
        /// <param name="sender">Коллекция Templates</param>
        /// <param name="e">Параметры события</param>
        private void OnTemplateListChange(object sender, ListChangedEventArgs e)
        {
            if (e.Action == ListChangedEventArgs.ListAction.Add)
            {
                bool setDefault = templates.IndexInRange(TemplateIndexDefault);
                bool setSelected = templates.IndexInRange(TemplateIndexSelected);
                foreach (Row row in rows)
                {
                    if (row.SelectedTemplate == null && setSelected)
                        row.SelectedTemplate = templates[TemplateIndexSelected];
                    if (row.DefaultTemplate == null && setDefault)
                        row.DefaultTemplate = templates[TemplateIndexDefault];
                }
            }
        }

        /// <summary>
        /// Рисует компонент в бэкбуфер
        /// </summary>
        private void DrawToBackBuffer()
        {
            using (var graphics = Graphics.FromImage(backBuffer))
            {
                rect = new Rectangle(0, 0, backBuffer.Width, backBuffer.Height);
                graphics.FillRectangle(backBrush, rect);

                rect.Y = scrollPosition;
                foreach (Row row in rows)
                {
                    if (rect.Y + rect.Height > 0)
                        row.Draw(graphics, rect);
                    
                    // Проверяем, что строка видима и имеет ненулевую высоту. 
                    // Если это не так, значит строка не рисовалась, и сдвигать координату не надо
                    if (row.Visible && row.Height > 0)
                        rect.Y += row.Height + separatorSize;

                    if (rect.Height - rect.Y <= 0)
                        break;
                }
            }
        }

        /// <summary>
        /// Переопределенный метод родителя, очищает шаблоны и удаляет подиски
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            rows.ListChanged -= OnRowListChange;
            templates.ListChanged -= OnTemplateListChange;

            for (int i = 0; i < templates.Count; ++i)
                templates[i].Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Переопределенный метод родителя, необходим для определения жеста прокрутки и выбора строки
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                touchStart.X = e.X;
                touchStart.Y = e.Y;
                clickStart = touchStart;
                touched = true;
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Переопределенный метод родителя, необходим для определения жеста прокуртки и выбора строки
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                touched = false;

            // Эмулируем клик для выбора строки, так как обычный OnClick нам не подходит,
            // он срабатывает даже при листании, если мы не покидаем пределы контрола
            // Поэтому вычисляем расстояние от точки MouseDown до текущей MouseUp. 
            // Обычный пифагор, только без корня, для ускорения. Потом сравниваем с квадратом чувствительности
            double distance2 = Math.Pow(clickStart.X - e.X, 2) + Math.Pow(clickStart.Y - e.Y, 2);
            
            // Если оно не больше, чем "чувствительность" (TouchSensivity)
            if (distance2 < TouchSensivity * TouchSensivity)
                OnTouchClick();
            
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Переопределенный метод родителя, необходим для непосредственной прокрутки
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (touched)
            {
                ScrollPosition += (e.Y - touchStart.Y);
                touchStart.X = e.X;
                touchStart.Y = e.Y;
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Переопределенный метод родителя, рисующий бэк-буфер
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(backBuffer, 0, 0);
        }

        /// <summary>
        /// Переопределенный метод родителя. Пустой для исправления моргания
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
        }

        /// <summary>
        /// Переопределенный метод родителя. Перерисовывает бэк-буфер и пересчитывает 
        /// максимально возможное значение прокрутки
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            backBuffer = new Bitmap(Width > 0 ? Width : 1, Height > 0 ? Height : 1);
            DrawToBackBuffer();
            RecalculateMaxScrollPosition();
            this.Invalidate();            
        }

        /// <summary>
        /// Инициализирует новый экземляр класса
        /// </summary>
        public CuteList()
        {
            TemplateIndexDefault = -1;
            TemplateIndexSelected = -1;
            rows.ListChanged += OnRowListChange;
            templates.ListChanged += OnTemplateListChange;
            backBuffer = new Bitmap(Width > 0 ? Width : 1, Height > 0 ? Height : 1);
            TouchSensivity = 10;
            SelectionChanged += EmptySelectionChanged;
        }

        /// <summary>
        /// Выбирает либо исключает из выбора строку, в зависимости от ее текущего состояния
        /// </summary>
        /// <param name="row">Строка</param>
        public void SelectDeselectRow(Row row)
        {            
            row.Selected = !row.Selected;            
            if (row.Selected)
            {
                if (!multipleSelect)
                {
                    foreach (Row sr in selectedRows)
                        sr.Selected = false;
                    selectedRows.Clear();

                }
                selectedRows.Add(row);
            }
            else
            {
                selectedRows.Remove(row);
            }

            //TODO - selectedChange

            RecalculateMaxScrollPosition();
            DrawToBackBuffer();
            this.Invalidate();
        }

        /// <summary>
        /// Выбирает либо исключает из выбора строку, в зависимости от ее текущего состояния
        /// </summary>
        /// <param name="index">Индекс строки в коллекции Rows</param>
        public void SelectDeselectRow(int index)
        {
            if (rows.IndexInRange(index))
                SelectDeselectRow(rows[index]);
            else
            { 
                // Кара небесная
            }
        }

        /// <summary>
        /// Текущая позиция скролла в диапазоне [-X, 0]
        /// </summary>
        [DefaultValue(0)]
        public int ScrollPosition
        {
            get { return scrollPosition; }
            set
            {
                value = value.Clamp(-scrollPositionMax, 0);
                if (scrollPosition != value)
                {
                    scrollPosition = value;
                    DrawToBackBuffer();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Строки с данными
        /// </summary>        
        public RowCollection Rows
        {
            get { return rows; }
        }

        /// <summary>
        /// Шаблоны для строк
        /// </summary>
        public TemplateRowCollection Templates
        {
            get { return templates; }
        }

        /// <summary>
        /// Индекс шаблона, который по умолчанию будет считаться обычным
        /// </summary>
        [DefaultValue(-1)]
        public int TemplateIndexDefault { get; set; }

        /// <summary>
        /// Индекс шаблона, который по умолчанию будет считать шаблоном для выделенных строк
        /// </summary>
        [DefaultValue(-1)]
        public int TemplateIndexSelected { get; set; }

        /// <summary>
        /// Определяет размер отступов между строками
        /// </summary>
        [DefaultValue(0)]
        public int SeparatorSize
        {
            get { return separatorSize; }
            set
            {
                if (separatorSize != value)
                {
                    separatorSize = value;
                    RecalculateMaxScrollPosition();
                    DrawToBackBuffer();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (BackColor != value)
                {
                    base.BackColor = value;
                    backBrush.Color = value;
                    DrawToBackBuffer();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Выбранные строки
        /// </summary>
        public IEnumerable<Row> SelectedRows
        {
            get { return selectedRows as IEnumerable<Row>; }
        }

        /// <summary>
        /// Устанавливает возможность выбора сразу нескольких строк
        /// </summary>
        [DefaultValue(false)]
        public bool MultipleRowSelect
        {
            get { return multipleSelect; }
            set
            {
                multipleSelect = value;
            }
        }

        /// <summary>
        /// Определяет максимальное расстояние между нажатием и отпусканием, 
        /// которое будет считаться «кликом»
        /// </summary>
        [DefaultValue(10)]
        public int TouchSensivity { get; set; }

        /// <summary>
        /// Возникает при изменении коллекции выбранных строк 
        /// (либо при изменеии выбранной строки при запрете множественного выбора)
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
    }
}
