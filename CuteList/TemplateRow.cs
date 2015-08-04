using System.ComponentModel;
using System.Drawing;

namespace CuteUI
{
    /// <summary>
    /// TemplateRow — класс шаблона строки для CuteList
    /// </summary>
    public class TemplateRow : Component
    {
        private int height = 40;
        private CellCollection cells = new CellCollection();
        private ColorGradient backGradient = new ColorGradient();
        private Color borderColor = Color.Black;
        private int borderSize = 0;

        /// <summary>
        /// Коллекция шаблонов ячеек
        /// </summary>
        public CellCollection TemplateCells
        {
            get { return cells; }
        }

        /// <summary>
        /// Высота строки
        /// </summary>
        [DefaultValue(40)]
        public int Height
        {
            get { return height; }
            set
            {
                if (value < 0)
                    value = 0;
                if (height != value)
                {
                    height = value;
                }
            }
        }

        /// <summary>
        /// Градиент фона строки
        /// </summary>                        
        public ColorGradient BackGradient
        {
            get { return backGradient; }
            set
            {
                backGradient = value;
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
                if (value < 0)
                    value = 0;

                if (borderSize != value)
                {
                    borderSize = value;
                }
            }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        public TemplateRow()
        {

        }
    }
}
