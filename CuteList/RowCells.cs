using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

namespace CuteUI
{
    /// <summary>
    /// RowCell — базовый класс ячейки строки
    /// </summary>
    public class RowCell
    {
        private Color backColor = Color.Transparent;
        private Color borderColor = Color.Teal;
        private int borderSize = 0;

        private SolidBrush backBrush = new SolidBrush(Color.Transparent);
        private Pen borderPen = new Pen(Color.Teal, 1);

        /// <summary>
        /// Текущий Bounds со смещением переданным в PaintEventArgs
        /// Передается по наследству
        /// </summary>
        protected Rectangle absRect;

        /// <summary>
        /// Базовый метод для рисования ячейки. Рисует фон и границу.
        /// Перегружается в потомках
        /// </summary>
        /// <param name="graphics">Объект Graphics, с помощью которого осуществляется рисование</param>
        /// <param name="clipRect">Ограничивающий прямоугольник для рисования, обычно передается прямоугольник строки</param>
        /// <param name="data">Данные, которые следует отобразить в ячейке</param>
        internal virtual void Draw(Graphics graphics, Rectangle clipRect, object data)
        {
            absRect = Bounds;
            absRect.Offset(clipRect.X, clipRect.Y);
            if (Bounds.Width < 0)
                absRect.Width = clipRect.Width - Bounds.X;
            if (Bounds.Height < 0)
                absRect.Height = clipRect.Height - Bounds.Y;

            if (backColor != Color.Transparent)
            {
                graphics.FillRectangle(backBrush, absRect);
            }

            if (borderColor != Color.Transparent && borderSize != 0)
            {
                graphics.DrawRectangle(borderPen, absRect);
            }
        }

        /// <summary>
        /// Цвет фона
        /// </summary>        
        public Color BackColor
        {
            get { return backColor; }
            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    backBrush.Color = value;
                }
            }
        }

        /// <summary>
        /// Цвет границы
        /// </summary>        
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    borderPen.Color = value;
                }
            }
        }

        /// <summary>
        /// Размер границы
        /// </summary>
        [DefaultValue(0)]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (borderSize != value)
                {
                    borderSize = value;
                    borderPen.Width = value;
                }
            }
        }

        /// <summary>
        /// Размеры и положение ячейки отнсоительно содержащей строки
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// Определяет индекс в массиве данных содержащей строки. 
        /// Установите -1 для установки данных в порядке следования ячеек.
        /// Установите -2 для того, чтобы закрепить за ячейкой постоянное значение.
        /// </summary>
        [DefaultValue(-1)]
        public int DataIndex { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public RowCell()
        {
            Bounds = new Rectangle(0, 0, 100, 20);
            DataIndex = -1;
            Name = "rowCell";
        }

        /// <summary>
        /// Имя ячейки
        /// </summary>
        /// <remarks>Используется для дизайнера</remarks>
        public string Name { get; set; }

        /// <summary>
        /// Переопределенный метод. Возвращает свойство Name, либо base.ToString(), если Name == null
        /// </summary>
        /// <returns>Возвращает свойство Name, либо base.ToString(), если Name == null</returns>
        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }

    /// <summary>
    /// TextCell — класс ячейки, отображающей данные в виде текста
    /// </summary>
    public class TextCell : RowCell
    {
        private VerAlign alignVertical = VerAlign.Middle;
        private HorAlign alignHorizontal = HorAlign.Left;

        /// <summary>
        /// Цвет текста
        /// </summary>
        private Color foreColor = Color.White;

        /// <summary>
        /// Кисть для текста
        /// </summary>
        private SolidBrush textBrush = new SolidBrush(Color.White);

        /// <summary>
        /// Объект StringFormat для вывода текста. 
        /// Используется объявление в классе для предотвращения реаллокации памяти
        /// </summary>
        private StringFormat stringFormat = new StringFormat();

        internal override void Draw(Graphics graphics, Rectangle clipRect, object data)
        {
            base.Draw(graphics, clipRect, data);
            if (data != null)
                graphics.DrawString(data.ToString(), Font, textBrush, absRect, stringFormat);
        }

        /// <summary>
        /// Цвет текста
        /// </summary>        
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    textBrush.Color = value;
                }
            }
        }

        /// <summary>
        /// Шрифт для отображения текста
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Строка с текстом
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Вертикальное выравнивание текста в ячейке
        /// </summary>
        public VerAlign AlignVertical
        {
            get { return alignVertical; }
            set
            {
                alignVertical = value;
                stringFormat.Alignment = Utility.VerMapping[alignVertical];
            }
        }

        /// <summary>
        /// Горизонтальное выравнивание текста в ячейке
        /// </summary>
        public HorAlign AlignHorizontal
        {
            get { return alignHorizontal; }
            set
            {
                alignHorizontal = value;
                stringFormat.LineAlignment = Utility.HorMapping[alignHorizontal];
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public TextCell()
        {
            Font = new Font("Tahoma", 10, FontStyle.Regular);
            Text = "TextCell";
            AlignVertical = VerAlign.Middle;
            AlignHorizontal = HorAlign.Left;
        }
    }

    /// <summary>
    /// SuperCell — класс, объединяющий в себе TextCell и ImageCell
    /// </summary>
    public class SuperCell : TextCell
    {
        /// <summary>
        /// Отображаемое изображение (приватное поле для свойства)
        /// </summary>
        private Image picture = null;

        /// <summary>
        /// Прямоугольник, определяющий часть изображения, которая будет рисоваться
        /// Используется объявление в классе для предотвращения реаллокации памяти
        /// </summary>
        private Rectangle pictureRect;

        internal override void Draw(Graphics graphics, Rectangle clipRect, object data)
        {
            base.Draw(graphics, clipRect, data);
            if (picture != null)
                graphics.DrawImage(picture, ImageBounds, pictureRect, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Отображаемое изображение
        /// </summary>
        [DefaultValue(null)]
        public Image Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                if (value != null)
                {
                    pictureRect = new Rectangle(0, 0, value.Width, value.Height);
                    ImageBounds = new Rectangle(ImageBounds.X, ImageBounds.Y, value.Width, value.Height);
                }
            }
        }

        /// <summary>
        /// Положение и размер изображения
        /// </summary>
        public Rectangle ImageBounds { get; set; }

        /// <summary>
        /// Вспомогательный метод для сериализации объекта дизайнером. 
        /// </summary>
        /// <returns>Возвращает true, если изображение не пустое, иначе false</returns>
        public bool ShouldSerializePicture()
        {
            return (picture != null);
        }
    }
}