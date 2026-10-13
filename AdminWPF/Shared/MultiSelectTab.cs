using System.Windows.Controls;

namespace AdminWPF.Shared;

public class MultiSelectTab<T> : UserControl
{
    readonly ListBox _list = new() { SelectionMode = SelectionMode.Multiple };
    readonly Func<T, int> _idSelector;

    public MultiSelectTab(List<T> catalog, IEnumerable<int> selectedIds, Func<T, int> idSelector, string displayPath)
    {
        _idSelector = idSelector;
        Content = _list;
        _list.ItemsSource = catalog;
        _list.DisplayMemberPath = displayPath;
        foreach (var item in catalog)
            if (selectedIds.Contains(idSelector(item)))
                _list.SelectedItems.Add(item);
    }

    public List<int> GetSelectedIds() => _list.SelectedItems.Cast<T>().Select(_idSelector).ToList();
}