using System.Windows;
using System.Windows.Controls;

namespace WpfPlanning
{
    public class VoteDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is WpfVote)
            {
                WpfVote vote = item as WpfVote;

                Window window = System.Windows.Application.Current.MainWindow;
                ListBox list = window.FindName("votes") as ListBox;

                bool all = true;
                foreach (WpfVote obj in list.Items)
                    if (!obj.HasVoted)
                    {
                        all = false;
                        break;
                    }

                if (all)
                    return list.FindResource("VisibleVote") as DataTemplate;
                else if (vote.HasVoted)
                    return list.FindResource("HiddenVote") as DataTemplate;
                else
                    return list.FindResource("NoVote") as DataTemplate;
            }

            return null;
        }
    }
}
