namespace CuteUI
{
    /// <summary>
    /// CellCollection — класс коллекции элементов типа «Ячейка».
    /// </summary>
    /// <remarks>Не расширяет функционал базового класса</remarks>
    public class CellCollection : GenericCollection<SuperCell> { }

    /// <summary>
    /// RowCollection — класс коллекции элементов типа «Строка».
    /// </summary>
    /// <remarks>Не расширяет функционал базового класса</remarks>
    public class RowCollection : GenericCollection<Row> { }

    /// <summary>
    /// TemplateRowCollection — класс коллекции элементов типа «Шаблон строки».
    /// </summary>
    /// <remarks>Не расширяет функционал базового класса</remarks>
    public class TemplateRowCollection : GenericCollection<TemplateRow> { }
}
