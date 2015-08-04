using System;
using System.Drawing;
using System.ComponentModel;

namespace CuteUI
{
    /// <summary>
    /// Структура ColorGradient описывает градиент
    /// </summary>    
    public class ColorGradient : Component
    {
        /// <summary>
        /// Закэшированное изображение
        /// </summary>
        private Bitmap cache;

        private Color start, end;

        /// <summary>
        /// Сбрасывает кэш-изображение
        /// </summary>
        private void DropCache()
        {
            if (cache != null)
            {
                cache.Dispose();
                cache = null;
            }
        }

        /// <summary>
        /// Возвращает новый Bitmap с градиентом        
        /// </summary>
        /// <param name="width">Требуемая ширина изображения</param>
        /// <param name="height">Требуемая высота изображения</param>
        /// <returns>Bitmap с градиентом</returns>
        private Bitmap DrawToBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(bitmap))
            using (Pen pen = new Pen(Start, 1))
            {
                if (Start == End)
                    graphics.FillRectangle(new SolidBrush(Start), 0, 0, width, height);
                else
                    for (int i = 0; i < height; ++i)
                    {
                        float t = (float)i / height;
                        int R = Start.R + (int)Math.Floor((End.R - Start.R) * t);
                        int G = Start.G + (int)Math.Floor((End.G - Start.G) * t);
                        int B = Start.B + (int)Math.Floor((End.B - Start.B) * t);
                        pen.Color = Color.FromArgb(R, G, B);
                        graphics.DrawLine(pen, 0, i, width - 1, i);
                    }
            }
            return bitmap;
        }

        /// <summary>
        /// Определяет валидно ли кэш-изображение
        /// </summary>
        /// <param name="width">Запрашиваемая ширина</param>
        /// <param name="height">Запрашиваемая высота</param>
        /// <returns></returns>
        private bool IsCacheValid(int width, int height)
        {
            return cache != null && cache.Width == width && cache.Height == height;
        }

        /// <summary>
        /// Начальный (верхний) цвет градиента
        /// </summary>        
        public Color Start
        {
            get { return start; }
            set
            {
                if (start != value)
                {
                    start = value;
                    DropCache();
                }
            }
        }

        /// <summary>
        /// Конечный (нижний) цвет градиента
        /// </summary>
        public Color End
        {
            get { return end; }
            set
            {
                if (end != value)
                {
                    end = value;
                    DropCache();
                }
            }
        }

        /// <summary>
        /// Иницилизирует новый экземпляр класса
        /// </summary>
        /// <param name="start">Начальный (верхний) цвет градиента</param>
        /// <param name="end">Конечный (нижний) цвет градиента</param>
        public ColorGradient(Color start, Color end)
        {
            this.start = start;
            this.end = end;
            this.cache = null;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public ColorGradient() : this(Color.LightBlue, Color.Teal) { }

        /// <summary>
        /// Возвращает Bitmap заданного размера, залитый градиентом. Использует кэширование
        /// </summary>
        /// <param name="width">Требуемая ширина</param>
        /// <param name="height">Требуемая высота</param>
        /// <returns>Bitmap с градиентом</returns>
        public Bitmap GetBitmap(int width, int height)
        {
            return GetBitmap(width, height, false);
        }

        /// <summary>
        /// Возвращает Bitmap заданного размера, залитый градиентом. Использует кэширование
        /// </summary>
        /// <param name="width">Требуемая ширина</param>
        /// <param name="height">Требуемая высота</param>
        /// <param name="forceRedraw">Сбросить кэш и принудительно нарисовать новый Bitmap</param>
        /// <returns>Bitmap с градиентом</returns>
        public Bitmap GetBitmap(int width, int height, bool forceRedraw)
        {
            if (forceRedraw || !IsCacheValid(width, height))
                cache = DrawToBitmap(width, height);

            return cache;
        }
    }
}