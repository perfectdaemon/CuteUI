using System;
using System.Drawing;

namespace CuteUI
{
    /// <summary>
    /// ExtensionMethods — статический класс, содержащий методы расширения
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Ограничивает значение заданными минимальным и максимальным значениями
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <param name="value">Значение, которое требуется ограничить</param>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns>Значение, лежащее в пределах min..max</returns>
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            else if (value.CompareTo(max) > 0)
                return max;
            else
                return value;
        }

        /// <summary>
        /// Рисует градиент
        /// </summary>
        /// <param name="graphics">Объект Graphics, осуществляющий рисование</param>
        /// <param name="gradient">Градиент, который требуется нарисовать</param>
        /// <param name="clipRect">Ограничивающий прямоугольник</param>
        public static void DrawGradient(this Graphics graphics, ColorGradient gradient, Rectangle clipRect)
        {
            DrawGradient(graphics, gradient, clipRect, false);
        }

        /// <summary>
        /// Рисует градиент
        /// </summary>
        /// <param name="graphics">Объект Graphics, осуществляющий рисование</param>
        /// <param name="gradient">Градиент, который требуется нарисовать</param>
        /// <param name="clipRect">Ограничивающий прямоугольник</param>
        /// <param name="invert">Инвертировать градиент</param>
        public static void DrawGradient(this Graphics graphics, ColorGradient gradient, Rectangle clipRect, bool invert)
        {
            var bitmap = gradient.GetBitmap(clipRect.Width, clipRect.Height);
            if (invert)
                graphics.DrawImage(bitmap, clipRect, new Rectangle(0, bitmap.Height, bitmap.Width, -bitmap.Height), GraphicsUnit.Pixel);
            else
                graphics.DrawImage(bitmap, clipRect.X, clipRect.Y);
        }
    }
}
