using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CuteUI
{
    /// <summary>
    /// CuteButton — расширенный класс кнопки
    /// </summary>
    public class CuteButton : Control
    {
        private ColorGradient backGradient = new ColorGradient(Color.Teal, Color.LightBlue);
        private int borderSize = 2;
        private Color borderColor = Color.Teal;

        private Bitmap backBuffer;
        private SolidBrush textBrush = new SolidBrush(Color.White);
        private Pen borderPen = new Pen(Color.Teal, 2);
        private StringFormat format = new StringFormat();
        private Rectangle rect, borderRect;
        private bool swappedGradient = false;

        /// <summary>
        /// Рисует компонент в бэкбуфер
        /// </summary>
        private void DrawToBackBuffer()
        {
            using (var graphics = Graphics.FromImage(backBuffer))
            {
                graphics.DrawGradient(backGradient, rect, swappedGradient);
                borderRect = new Rectangle(borderSize / 2, borderSize / 2, rect.Width - borderSize, rect.Height - borderSize);
                graphics.DrawRectangle(borderPen, borderRect);
                graphics.DrawString(Text, Font, textBrush, rect, format);
            }
        }

        /// <summary>
        /// Переопределенный метод родителя. Рисует бэк-буфер
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            DrawToBackBuffer();
            e.Graphics.DrawImage(backBuffer, 0, 0);
        }

        /// <summary>
        /// Переопределенный метод родителя. Перерисовывает бэк-буфер
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            rect = new Rectangle(0, 0, Width > 0 ? Width : 1, Height > 0 ? Height : 1);
            backBuffer = new Bitmap(Width > 0 ? Width : 1, Height > 0 ? Height : 1);
            DrawToBackBuffer();
            this.Invalidate();
        }

        /// <summary>
        /// Переопределенный метод родителя. Переворачивает градиент при клике, если такое свойство активно
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (SwapGradientOnClick)
            {
                swappedGradient = true;
                this.Invalidate();
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Переопределенный метод родителя. Возвращает градиент в обычное состояние, если такое свойство активно
        /// </summary>
        /// <param name="e">Параметры события</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (SwapGradientOnClick)
            {
                swappedGradient = false;
                this.Invalidate();
            }
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Переопределенный метод родителя. Перерисовывает бэк-буфер
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// Размер границы
        /// </summary>
        [DefaultValue(2)]
        public int BorderSize
        {
            get { return borderSize; }
            set
            {
                if (borderSize != value)
                {
                    borderSize = value;
                    borderPen.Width = value;
                    this.Invalidate();
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
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Градиент фона кнопки
        /// </summary>
        public ColorGradient BackGradient
        {
            get { return backGradient; }
            set
            {
                if (backGradient != value)
                {
                    backGradient = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Цвет текста
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                textBrush.Color = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Переворачивать ли градиент при нажатии
        /// </summary>
        [DefaultValue(true)]
        public bool SwapGradientOnClick { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public CuteButton()
        {
            backBuffer = new Bitmap(Width > 0 ? Width : 1, Height > 0 ? Height : 1);
            rect = new Rectangle(0, 0, Width > 0 ? Width : 1, Height > 0 ? Height : 1);

            ForeColor = Color.White;
            SwapGradientOnClick = true;

            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
        }
    }
}
